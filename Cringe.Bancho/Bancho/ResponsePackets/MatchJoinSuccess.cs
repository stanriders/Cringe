using System;
using System.Linq;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;

namespace Cringe.Bancho.Bancho.ResponsePackets
{
    public class MatchJoinSuccess : ResponsePacket
    {
        public MatchJoinSuccess(Match match)
        {
            Match = match;
        }

        public Match Match { get; }
        public override ServerPacketType Type => ServerPacketType.MatchJoinSuccess;

        public override byte[] GetBytes()
        {
            var slots = Match.Slots.ToArray();
            var status = slots.Select(x => (byte) x.Status);
            var teams = slots.Select(x => (byte) x.Team);
            var host = Match.Host;
            var players = Match.Slots
                .Select(x => x.Player is not null ? PackData(x.Player.Token.PlayerId) : Array.Empty<byte>())
                .SelectMany(x => x).ToArray();
            var playerMods = Match.Slots.Select(x => PackData((int) x.Mods)).SelectMany(x => x);
            var mods = (int) Match.Mods;

            return ConcatData(
                PackData(Match.Id),
                PackData(Match.InProgress),
                new byte[] {0},
                PackData(mods),
                PackData(Match.Name),
                PackData(Match.Password),
                PackData(Match.MapName),
                PackData(Match.MapId),
                PackData(Match.MapMd5),
                status,
                teams,
                players,
                PackData(host),
                new[]
                {
                    (byte) Match.Mode, (byte) Match.WinConditions, (byte) Match.TeamTypes,
                    (byte) (Match.FreeMode ? 1 : 0)
                },
                Match.FreeMode ? playerMods : Array.Empty<byte>());
        }
    }
}
