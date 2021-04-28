using System;
using System.Buffers;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using osuLocalBancho.Bancho;
using osuLocalBancho.Services;
using osuLocalBancho.Types;
using osuLocalBancho.Types.Enums;

namespace osuLocalBancho.Controllers
{
    [ApiController]
    [Route("/")]
    public class BanchoController : ControllerBase
    {
        private readonly BanchoService _banchoService;

        public BanchoController(BanchoService banchoService)
        {
            _banchoService = banchoService;
        }

        [HttpPost]
        public IActionResult MainEndpoint()
        {
            HttpContext.Response.Headers.Add("Connection", "keep-alive");
            HttpContext.Response.Headers.Add("Keep-Alive", "timeout=5, max=100");
            HttpContext.Response.Headers.Add("cho-protocol", "19");
            HttpContext.Response.Headers.Add("cho-token", "token");

            if (!HttpContext.Request.Headers.ContainsKey("osu-token"))
            {
                HandleLogin();
            }

            HandleIncomingPackets();

            return new FileContentResult(_banchoService.GetDataToSend(), "text/html; charset=UTF-8");
        }

        private void HandleLogin()
        {
            var rank = UserRanks.Normal | UserRanks.Supporter | UserRanks.BAT | UserRanks.TournamentStaff;

            _banchoService.EnqueuePacket(DataPacket.PackData("Hurrep"), ServerPacketType.Notification);

            _banchoService.EnqueuePacket(DataPacket.PackData((uint)0), ServerPacketType.SilenceEnd);

            _banchoService.EnqueuePacket(DataPacket.PackData(Player.DummyPlayer.Rank), ServerPacketType.UserId);
            _banchoService.EnqueuePacket(DataPacket.PackData((uint)19), ServerPacketType.ProtocolVersion);

            _banchoService.EnqueuePacket(DataPacket.PackData((uint)rank), ServerPacketType.Supporter);

            _banchoService.EnqueuePacket(HandlePanel(), ServerPacketType.UserPanel);
            _banchoService.EnqueuePacket(HandleStats(), ServerPacketType.UserStats);

            _banchoService.EnqueuePacket(DataPacket.PackData((uint)0), ServerPacketType.ChannelInfoEnd);

            _banchoService.EnqueuePacket(DataPacket.PackData(new int[] { 2 }), ServerPacketType.FriendsList);

            _banchoService.EnqueuePacket(DataPacket.PackData("https://assets.ppy.sh/beatmaps/637386/covers/card.jpg?1499585149"), ServerPacketType.MainMenuIcon);

            _banchoService.EnqueuePacket(HandlePanel(), ServerPacketType.UserPanel);
        }

        private void HandleIncomingPackets()
        {
            using var inStream = new MemoryStream();

            while (true)
            {
                var readResult = HttpContext.Request.BodyReader.ReadAsync().Result;
                var buffer = readResult.Buffer;
                inStream.Write(buffer.ToArray());

                HttpContext.Request.BodyReader.AdvanceTo(buffer.Start, buffer.End);
                if (readResult.IsCompleted)
                    break;
            }

            var data = inStream.ToArray();
            data[0] = 0x0; // uhh??? first byte seem to be a copy of second (which is supposed to be first)

            var packetType = (ClientPacketType)BitConverter.ToUInt16(data[1..3].ToArray());
            switch (packetType)
            {
                case ClientPacketType.ChangeAction:
                {
                    _banchoService.EnqueuePacket(HandlePanel(), ServerPacketType.UserPanel);
                    _banchoService.EnqueuePacket(HandleStats(), ServerPacketType.UserStats);
                    break;
                }
                case ClientPacketType.RequestStatusUpdate:
                {
                    _banchoService.EnqueuePacket(HandleStats(), ServerPacketType.UserStats);
                    break;
                }
                case ClientPacketType.UserPanelRequest:
                {
                    _banchoService.EnqueuePacket(HandlePanel(), ServerPacketType.UserPanel);
                    break;
                }
                /*case ClientPacketType.UserStatsRequest:
                {
                    _banchoService.EnqueuePacket(HandleStats(), ServerPacketType.UserStats);
                }*/
            }
        }

        private byte[] HandleStats()
        {
            return new Stats
            {
                UserId = (uint) Player.DummyPlayer.Id,
                ActionId = 0,
                ActionText = "",
                ActionMd5 = "",
                ActionMods = 0,
                GameMode = 0,
                BeatmapId = 0,
                RankedScore = Player.DummyPlayer.TotalScore,
                Accuracy = Player.DummyPlayer.Accuracy,
                Playcount = Player.DummyPlayer.Playcount,
                TotalScore = Player.DummyPlayer.TotalScore,
                GameRank = Player.DummyPlayer.Rank,
                Pp = Player.DummyPlayer.Pp
            }.Pack();
        }

        private byte[] HandlePanel()
        {
            return new Panel
            {
                UserId = Player.DummyPlayer.Id,
                Username = Player.DummyPlayer.Username,
                Timezone = 24,
                Country = 0,
                UserRank = UserRanks.Peppy,
                Longitude = 0.0f,
                Latitude = 0.0f,
                GameRank = Player.DummyPlayer.Rank
            }.Pack();
        }
    }
}
