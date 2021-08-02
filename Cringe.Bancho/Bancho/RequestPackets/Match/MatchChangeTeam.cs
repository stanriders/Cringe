using System;
using System.Threading.Tasks;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;
using Cringe.Types.Enums.Multiplayer;

namespace Cringe.Bancho.Bancho.RequestPackets.Match
{
    public class MatchChangeTeam : RequestPacket
    {
        public MatchChangeTeam(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override ClientPacketType Type => ClientPacketType.MatchChangeTeam;

        public override Task Execute(PlayerSession session, byte[] data)
        {
            if (session is null)
            {
                return Task.CompletedTask;
            }

            var match = session.MatchSession.Match;
            var slot = match.GetPlayer(session.Id);
            slot.Team = slot.Team == MatchTeams.blue ? MatchTeams.red : MatchTeams.blue;
            session.MatchSession.OnUpdateMatch();
            return Task.CompletedTask;
        }
    }
}
