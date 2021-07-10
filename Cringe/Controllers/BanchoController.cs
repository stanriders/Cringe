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

            return service is null ? Fail(PacketQueue.NullUser().GetDataToSend()) : service.GetResult();
        }

        private IActionResult Fail(byte[] data)
        {
            HttpContext.Response.Headers.Add("cho-token", " ");
            return new FileContentResult(data, "application/octet-stream; charset=UTF-8");
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

            var queue = _banchoServicePool.GetFromPool(player.Id);

            queue.EnqueuePacket(new ProtocolVersion(protocol_version));
            queue.EnqueuePacket(new UserId(token.PlayerId));
            queue.EnqueuePacket(new Supporter(player.UserRank));
            if (!string.IsNullOrEmpty(_configuration["LoginMessage"]))
                queue.EnqueuePacket(new Notification(_configuration["LoginMessage"]));
            queue.EnqueuePacket(new SilenceEnd(0));


            queue.EnqueuePacket(new UserPanel(player.Panel));
            queue.EnqueuePacket(new UserStats(player.Stats));

            queue.EnqueuePacket(new ChannelInfoEnd());

            queue.EnqueuePacket(new FriendsList(new[] {2}));

            if (!string.IsNullOrEmpty(_configuration["MainMenuBanner"]))
                queue.EnqueuePacket(new MainMenuIcon(_configuration["MainMenuBanner"]));

            queue.EnqueuePacket(new UserPanel(player.Panel));
            return queue;
        }

        private async Task<PacketQueue> HandleIncomingPackets()
        {
            var token = _tokenService.GetToken(HttpContext.Request.Headers["osu-token"][0]);
            if (token == null)
                // force update login
                return PacketQueue.NullUser();

            var player = await _tokenService.GetPlayer(token.Token);

            var queue = _banchoServicePool.GetFromPool(token.PlayerId);

            await using var inStream = new MemoryStream();
            await Request.Body.CopyToAsync(inStream);
            var data = inStream.ToArray();

            var packetType = (ClientPacketType) BitConverter.ToUInt16(data[..2].ToArray());
            switch (packetType)
            {
                case ClientPacketType.ChangeAction:
                {
                    queue.EnqueuePacket(new UserPanel(player.Panel));
                    queue.EnqueuePacket(new UserStats(player.Stats));
                    break;
                }
                case ClientPacketType.RequestStatusUpdate:
                {
                    queue.EnqueuePacket(new UserStats(player.Stats));
                    break;
                }
                case ClientPacketType.UserPresenceRequest:
                {
                    queue.EnqueuePacket(new UserPanel(player.Panel));
                    break;
                }
                /*case ClientPacketType.UserStatsRequest:
                {
                    queue.EnqueuePacket(new UserStats(player.Stats));
                }*/
            }

            return queue;
        }
    }
}