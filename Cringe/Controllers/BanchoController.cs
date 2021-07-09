using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Cringe.Bancho.Packets;
using Cringe.Services;
using Cringe.Types;
using Cringe.Types.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Cringe.Controllers
{
    [ApiController]
    [Route("/")]
    public class BanchoController : ControllerBase
    {
        private const uint protocol_version = 19;

        private readonly BanchoServicePool _banchoServicePool;
        private readonly IConfiguration _configuration;
        private readonly TokenService _tokenService;

        public BanchoController(BanchoServicePool pool, TokenService tokenService, IConfiguration configuration)
        {
            _banchoServicePool = pool;
            _tokenService = tokenService;
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> MainEndpoint()
        {
            HttpContext.Response.Headers.Add("Connection", "keep-alive");
            HttpContext.Response.Headers.Add("Keep-Alive", "timeout=5, max=100");
            HttpContext.Response.Headers.Add("cho-protocol", protocol_version.ToString());
            PacketQueue service;
            if (!HttpContext.Request.Headers.ContainsKey("osu-token"))
                service = await HandleLogin();
            else
                service = await HandleIncomingPackets();
            return service is null ? PacketQueue.NullUser() : service.GetResult();
        }

        private async Task<PacketQueue> HandleLogin()
        {
            var loginData = (await new StreamReader(Request.Body).ReadToEndAsync()).Split('\n');

            var token = await _tokenService.AddToken(loginData[0].Trim());
            if (token == null)
                return null;

            var player = await _tokenService.GetPlayer(token.Token);
            if (player == null)
                return null;

            HttpContext.Response.Headers.Add("cho-token", token.Token);
            var banchoService = _banchoServicePool.GetFromPool(player.Id);
            if (!string.IsNullOrEmpty(_configuration["LoginMessage"]))
                banchoService.EnqueuePacket(new Notification(_configuration["LoginMessage"]));

            banchoService.EnqueuePacket(new SilenceEnd(0));

            banchoService.EnqueuePacket(new UserId(token.PlayerId));
            banchoService.EnqueuePacket(new ProtocolVersion(protocol_version));

            banchoService.EnqueuePacket(new Supporter(player.UserRank));

            banchoService.EnqueuePacket(new UserPanel(await HandlePanel(token.Token)));
            banchoService.EnqueuePacket(new UserStats(await HandleStats(token.Token)));

            banchoService.EnqueuePacket(new ChannelInfoEnd());

            banchoService.EnqueuePacket(new FriendsList(new[] {2}));

            if (!string.IsNullOrEmpty(_configuration["MainMenuBanner"]))
                banchoService.EnqueuePacket(new MainMenuIcon(_configuration["MainMenuBanner"]));

            banchoService.EnqueuePacket(new UserPanel(await HandlePanel(token.Token)));
            return banchoService;
        }

        private async Task<PacketQueue> HandleIncomingPackets()
        {
            var token = _tokenService.GetToken(HttpContext.Request.Headers["osu-token"][0]);
            if (token == null)
            {
                // force update login
                var crashQueue = new PacketQueue();
                crashQueue.EnqueuePacket(new UserId(-1));
                return crashQueue;
            }
           
            var queue = _banchoServicePool.GetFromPool(token.PlayerId);
            await using var inStream = new MemoryStream();
            await Request.Body.CopyToAsync(inStream);
            var data = inStream.ToArray();

            var packetType = (ClientPacketType) BitConverter.ToUInt16(data[..2].ToArray());
            switch (packetType)
            {
                case ClientPacketType.ChangeAction:
                {
                    queue.EnqueuePacket(new UserPanel(await HandlePanel(token.Token)));
                    queue.EnqueuePacket(new UserStats(await HandleStats(token.Token)));
                    break;
                }
                case ClientPacketType.RequestStatusUpdate:
                {
                    queue.EnqueuePacket(new UserStats(await HandleStats(token.Token)));
                    break;
                }
                case ClientPacketType.UserPanelRequest:
                {
                    queue.EnqueuePacket(new UserPanel(await HandlePanel(token.Token)));
                    break;
                }
                /*case ClientPacketType.UserStatsRequest:
                {
                    _banchoService.EnqueuePacket(new UserStats(HandleStats()), ServerPacketType.UserStats);
                }*/
            }

            return queue;
        }

        private async Task<Stats> HandleStats(string token)
        {
            var player = await _tokenService.GetPlayer(token);
            if (player == null)
                return null;

            return new Stats
            {
                UserId = (uint) player.Id,
                ActionId = 0,
                ActionText = "",
                ActionMd5 = "",
                ActionMods = 0,
                GameMode = 0,
                BeatmapId = 0,
                RankedScore = player.TotalScore,
                Accuracy = player.Accuracy,
                Playcount = player.Playcount,
                TotalScore = player.TotalScore,
                GameRank = player.Rank,
                Pp = player.Pp
            };
        }

        private async Task<Panel> HandlePanel(string token)
        {
            var player = await _tokenService.GetPlayer(token);
            if (player == null)
                return null;

            return new Panel
            {
                UserId = player.Id,
                Username = player.Username,
                Timezone = 24,
                Country = 0,
                UserRank = player.UserRank,
                Longitude = 0.0f,
                Latitude = 0.0f,
                GameRank = player.Rank
            };
        }
    }
}