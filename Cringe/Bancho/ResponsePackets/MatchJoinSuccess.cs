using System;
using System.Linq;
using Cringe.Types;
using Cringe.Types.Enums;

namespace Cringe.Bancho.ResponsePackets
{
    public class MatchJoinSuccess : ResponsePacket
    {
        public MatchJoinSuccess(Lobby match)
        {
            Match = match;
        }

        public Lobby Match { get; }
        public override ServerPacketType Type => ServerPacketType.MatchJoinSuccess;

        public override byte[] GetBytes()
        {
            var slots = Match.Slots.OrderBy(x => x.Index).ToArray();
            var status = slots.Select(x => (byte) x.Status);
            var teams = slots.Select(x => (byte) x.Team);
            var host = Match.Host;
            var players = Match.Players.Select(x => PackData(x.Id)).SelectMany(x => x);
            var playerMods = Match.FreeMode ? Match.Slots.Select(x => (byte) x.Mods).ToArray() : Array.Empty<byte>();
            var mods = (int)Match.Mods;
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
                PackData(host),
                players,
                PackData((byte) Match.Mode),
                PackData((byte) Match.WinConditions),
                PackData(Match.FreeMode ? 1 : 0),
                playerMods);
        }
    }
}