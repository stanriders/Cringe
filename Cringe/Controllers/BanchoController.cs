using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Cringe.Bancho;
using Cringe.Services;
using Cringe.Types;
using Cringe.Types.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Cringe.Controllers
{
    [ApiController]
    [Route("/")]
    public class BanchoController : ControllerBase
    {
        private const uint protocol_version = 19;
        private readonly BanchoServicePool _banchoServicePool;
        private readonly InvokeService _invoke;
        private readonly ILogger<BanchoController> _logger;
        private readonly TokenService _tokenService;

        public BanchoController(ILogger<BanchoController> logger, BanchoServicePool pool, TokenService tokenService,
            InvokeService invoke)
        {
            _logger = logger;
            _banchoServicePool = pool;
            _tokenService = tokenService;
            _invoke = invoke;
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
            await _invoke.InvokeOne(ClientPacketType.Login, token, null);
            return _banchoServicePool.GetFromPool(token.PlayerId);
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

            await _invoke.Invoke(token, data);
            return queue;
        }
    }
}