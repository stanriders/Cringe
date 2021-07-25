using System;
using System.Threading.Tasks;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;

namespace Cringe.Bancho.Bancho.RequestPackets
{
    public class CreateMatch : RequestPacket
    {
        public CreateMatch(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override ClientPacketType Type => ClientPacketType.CreateMatch;

        public override Task Execute(PlayerSession session, byte[] data)
        {
            session.MatchSession = null;
            var match = Match.Parse(data);
            Lobby.CreateMatch(session, match);

            return Task.CompletedTask;
        }
    }
}
