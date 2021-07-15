using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Cringe.Bancho;
using Cringe.Bancho.Packets;
using Cringe.Database;
using Cringe.Services;
using Cringe.Types;
using Cringe.Types.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Cringe.Controllers
{
    [ApiController]
    [Route("/")]
    public class BanchoController : ControllerBase
    {
        private const uint protocol_version = 19;
        private readonly BanchoServicePool _banchoServicePool;
        private readonly ChatServicePool _chats;
        private readonly IConfiguration _configuration;
        private readonly ILogger<BanchoController> _logger;

        private readonly StatsService _statsService;
        private readonly TokenService _tokenService;

        public BanchoController(StatsService statsService, ILogger<BanchoController> logger, BanchoServicePool pool,
            ChatServicePool chats, TokenService tokenService,
            IConfiguration configuration)
        {
            _statsService = statsService;
            _logger = logger;
            _banchoServicePool = pool;
            _chats = chats;
            _tokenService = tokenService;
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> MainEndpoint()
        {
            HttpContext.Response.Headers.Add("Connection", "keep-alive");
            HttpContext.Response.Headers.Add("Keep-Alive", "timeout=5, max=100");
            HttpContext.Response.Headers.Add("cho-protocol", protocol_version.ToString());

            PacketQueue queue;
            if (!HttpContext.Request.Headers.ContainsKey("osu-token"))
                queue = await HandleLogin();
            else
                queue = await HandleIncomingPackets();

            return queue?.GetResult() ?? Fail(PacketQueue.NullUser().GetDataToSend());
        }

        private IActionResult Fail(byte[] data)
        {
            HttpContext.Response.Headers.Add("cho-token", " ");
            return new FileContentResult(data, "application/octet-stream; charset=UTF-8");
        }

        private async Task<PacketQueue> HandleLogin()
        {
            _logger.LogDebug("Logging " + Request.HttpContext.Connection.RemoteIpAddress);
            var loginData = (await new StreamReader(Request.Body).ReadToEndAsync()).Split('\n');

            var token = await _tokenService.AddToken(loginData[0].Trim());
            if (token == null)
                return null;

            var player = await _tokenService.GetPlayer(token.Token);
            if (player == null)
                return null;

            HttpContext.Response.Headers.Add("cho-token", token.Token);

            var queue = _banchoServicePool.GetFromPool(player.Id);

            queue.EnqueuePacket(new ProtocolVersion(protocol_version));
            queue.EnqueuePacket(new UserId(token.PlayerId));
            queue.EnqueuePacket(new UserPresenceBundle(_banchoServicePool.Apply(x => x)));
            queue.EnqueuePacket(new Supporter(player.UserRank));
            if (!string.IsNullOrEmpty(_configuration["LoginMessage"]))
                queue.EnqueuePacket(new Notification(_configuration["LoginMessage"]));
            queue.EnqueuePacket(new SilenceEnd(0));


            queue.EnqueuePacket(new UserPresence(player.Presence));
            queue.EnqueuePacket(new UserStats(player.Stats));

            queue.EnqueuePacket(new ChannelInfoEnd());

            queue.EnqueuePacket(new FriendsList(new[] {2}));

            if (!string.IsNullOrEmpty(_configuration["MainMenuBanner"]))
                queue.EnqueuePacket(new MainMenuIcon(_configuration["MainMenuBanner"]));

            queue.EnqueuePacket(new UserPresence(player.Presence));
            var servers = new List<string>
            {
                "#osu",
                "#announce",
                "#russian"
            };
            if (player.UserRank.HasFlag(UserRanks.Admin) || player.UserRank.HasFlag(UserRanks.Peppy))
                servers.Add("#vacman");
            servers.ForEach(x => _chats.AutoJoinOrPackInfo(token.PlayerId, x));
            return queue;
        }

        private async Task<PacketQueue> HandleIncomingPackets()
        {
            var token = _tokenService.GetToken(HttpContext.Request.Headers["osu-token"][0]);
            if (token == null)
                // force update login
                return PacketQueue.NullUser();
            HttpContext.Response.Headers.Add("cho-token", token.Token);
            var player = await _tokenService.GetPlayer(token.Token);

            var queue = _banchoServicePool.GetFromPool(token.PlayerId);

            await using var inStream = new MemoryStream();
            await Request.Body.CopyToAsync(inStream);
            var data = inStream.ToArray();

            var packetType = (ClientPacketType) BitConverter.ToUInt16(data[..2].ToArray());
            switch (packetType)
            {
                case ClientPacketType.UserStatsRequest:
                {
                    using var reader = new BinaryReader(new MemoryStream(data));
                    var statsIdsTasks = DataPacket.ReadI32(reader).Select(x => _tokenService.GetPlayerWithoutScores(x));
                    var statsPlayers = (await Task.WhenAll(statsIdsTasks)).Where(x => x is not null).ToArray();
                    var presencePlayers = statsPlayers.Where(x => x.Id != token.PlayerId).ToArray();

                    foreach (var stats in statsPlayers)
                    {
                        var st = _statsService.GetUpdates(stats.Id);
                        if (st is null)
                            continue;
                        queue.EnqueuePacket(new UserStats(st));
                    }

                    foreach (var players in presencePlayers) queue.EnqueuePacket(new UserPresence(players.Presence));

                    return queue;
                }

                case ClientPacketType.Logout:
                {
                    _banchoServicePool.Nuke(token.PlayerId);
                    _chats.NukeUser(token.PlayerId);
                    return queue;
                }
                case ClientPacketType.SendPrivateMessage:
                {
                    var dest = data[9..];
                    var message = await Message.Parse(dest, token.Username);
                    await using var players = new PlayerDatabaseContext();
                    var receivePlayer = await players.Players.FirstOrDefaultAsync(x => x.Username == message.Receiver);
                    if (receivePlayer is null)
                        return null;
                    _banchoServicePool.ActionOn(receivePlayer.Id, x => x.EnqueuePacket(message));
                    break;
                }
                case ClientPacketType.SendPublicMessage:
                {
                    var dest = data[9..];
                    var message = await Message.Parse(dest, token.Username);
                    _banchoServicePool.ActionMapFilter(x => x.EnqueuePacket(message), id => id != token.PlayerId);
                    break;
                }
                case ClientPacketType.ChangeAction:
                {
                    var action = ChangeAction.Parse(data);
                    var pl = player.Stats;
                    pl.Action = action;
                    _statsService.SetUpdates(token.PlayerId, pl);
                    queue.EnqueuePacket(new UserStats(pl));
                    queue.EnqueuePacket(new UserPresence(player.Presence));
                    break;
                }
                case ClientPacketType.ChannelJoin:
                {
                    var str = DataPacket.ReadString(new MemoryStream(data[7..]));
                    _chats.Connect(token.PlayerId, str);
                    return queue;
                }
                case ClientPacketType.ChannelPart:
                {
                    var server = DataPacket.ReadString(new MemoryStream(data[7..]));
                    _chats.Disconnect(token.PlayerId, server);
                    return queue;
                }
                case ClientPacketType.RequestStatusUpdate:
                {
                    queue.EnqueuePacket(new UserStats(player.Stats));
                    break;
                }
                case ClientPacketType.UserPresenceRequest:
                {
                    queue.EnqueuePacket(new UserPresence(player.Presence));
                    break;
                }
                case ClientPacketType.Ping:
                    return queue;
                /*case ClientPacketType.UserStatsRequest:
                {
                    queue.EnqueuePacket(new UserStats(player.Stats));
                }*/
            }

            return queue;
        }
    }
}