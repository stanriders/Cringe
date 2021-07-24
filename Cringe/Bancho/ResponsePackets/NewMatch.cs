using System;
using System.Linq;
using Cringe.Types;
using Cringe.Types.Enums;
using Cringe.Types.Enums.Multiplayer;

namespace Cringe.Bancho.ResponsePackets
{
    public class NewMatch : ResponsePacket
    {
        public NewMatch(Match match)
        {
            Match = match;
        }

        public override ServerPacketType Type => ServerPacketType.NewMatch;
        public Match Match { get; }

        public override byte[] GetBytes()
        {
            var slots = Match.Slots.ToArray();
            var status = slots.Select(x => (byte) x.Status);
            var teams = slots.Select(x => (byte) x.Team);
            var host = Match.Host;
            var players = Match.Slots.Select(x => (x.Status & SlotStatus.has_player) != 0 ? PackData(x.Player.Token.PlayerId) : Array.Empty<byte>()).SelectMany(x => x).ToArray();
            var playerMods = Match.FreeMode ? Match.Slots.Select(x => (byte) x.Mods).ToArray() : Array.Empty<byte>();
            var mods = (int) Match.Mods;
            return ConcatData(
                PackData(Match.Id),
                PackData(Match.InProgress),
                new byte[] {0},
                PackData(mods),
                PackData(Match.Name),
                Match.Password != "" ? PackData("") : new byte[] {0},
                PackData(Match.MapName),
                PackData(Match.MapId),
                PackData(Match.MapMd5),
                status,
                teams,
                PackData(host),
                players,
                new[]
                {
                    (byte) Match.Mode, (byte) Match.WinConditions, (byte) Match.TeamTypes,
                    (byte) (Match.FreeMode ? 1 : 0)
                },
                playerMods);
        }
    }
}