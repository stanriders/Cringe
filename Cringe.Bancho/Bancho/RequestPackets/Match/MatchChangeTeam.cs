using System;
using System.Threading.Tasks;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;
using Cringe.Types.Enums.Multiplayer;

namespace Cringe.Bancho.Bancho.RequestPackets.Match
{
    public class MatchChangeTeam : MatchChanged
    {
        public MatchChangeTeam(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override ClientPacketType Type => ClientPacketType.MatchChangeTeam;

        public override Task Execute(PlayerSession session, byte[] data)
        {
            if (session is null)
                return Task.CompletedTask;

            var match = session.MatchSession.Match;
            var slot = match.GetPlayer(session.Id);
            slot.Team = slot.Team == MatchTeams.Blue ? MatchTeams.Red : MatchTeams.Blue;
            session.MatchSession.OnUpdateMatch();

            return Task.CompletedTask;
        }
    }
}
