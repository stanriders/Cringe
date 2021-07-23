using System.IO;
using System.Threading.Tasks;
using Cringe.Bancho;
using Cringe.Bancho.ResponsePackets;
using Cringe.Services;
using Cringe.Types;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Cringe.Controllers
{
    [ApiController]
    [Route("/")]
    public class BanchoController : ControllerBase
    {
        private const uint protocol_version = 19;
        private readonly ChatService _chat;
        private readonly IConfiguration _config;
        private readonly InvokeService _invoke;
        private readonly ILogger<BanchoController> _logger;
        private readonly PlayersPool _playersPool;
        private readonly TokenService _tokenService;

        public BanchoController(IConfiguration config, ILogger<BanchoController> logger, TokenService tokenService,
            InvokeService invoke, PlayersPool playersPool, ChatService chat)
        {
            _config = config;
            _logger = logger;
            _tokenService = tokenService;
            _invoke = invoke;
            _playersPool = playersPool;
            _chat = chat;
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
            var loginData = (await new StreamReader(Request.Body).ReadToEndAsync()).Split('\n');

            var token = await _tokenService.AddToken(loginData[0].Trim());
            if (token == null)
                return null;
            if (!await _playersPool.Connect(token)) return null;
            ;
            var session = _playersPool.GetPlayer(token.PlayerId);
            if (session == null) return null;

            HttpContext.Response.Headers.Add("cho-token", token.Token);
            await Login(session);
            return session.Queue;
        }

        private async Task Login(PlayerSession session)
        {
            var queue = session.Queue;
            queue.EnqueuePacket(new ProtocolVersion(protocol_version));
            queue.EnqueuePacket(new UserId(session.Token.PlayerId));
            queue.EnqueuePacket(new UserPresenceBundle(_playersPool.GetPlayersId()));
            queue.EnqueuePacket(new Supporter(session.Player.UserRank));
            if (!string.IsNullOrEmpty(_config["LoginMessage"]))
                queue.EnqueuePacket(new Notification(_config["LoginMessage"]));
            queue.EnqueuePacket(new SilenceEnd(0));

            queue.EnqueuePacket(new UserPresence(session.Player.Presence));
            queue.EnqueuePacket(new UserStats(session.Player.Stats));

            queue.EnqueuePacket(new ChannelInfoEnd());

            queue.EnqueuePacket(new FriendsList(new[] {2}));

            if (!string.IsNullOrEmpty(_config["MainMenuBanner"]))
                queue.EnqueuePacket(new MainMenuIcon(_config["MainMenuBanner"]));

            queue.EnqueuePacket(new UserPresence(session.Player.Presence));
            await _chat.Initialize(session);
        }

        private async Task<PacketQueue> HandleIncomingPackets()
        {
            var token = _tokenService.GetToken(HttpContext.Request.Headers["osu-token"][0]);
            if (token == null)
                // force update login
                return PacketQueue.NullUser();
            HttpContext.Response.Headers.Add("cho-token", token.Token);
            var session = _playersPool.GetPlayer(token.PlayerId);

            await using var inStream = new MemoryStream();
            await Request.Body.CopyToAsync(inStream);
            var data = inStream.ToArray();

            await _invoke.Invoke(session, data);
            return session.Queue;
        }
    }
}