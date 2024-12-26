using System;
using System.IO;
using System.Threading.Tasks;
using Cringe.Bancho.Bancho.ResponsePackets;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;

namespace Cringe.Bancho.Bancho.RequestPackets.Tournament
{
    public class TournamentMatchInfoRequest : RequestPacket
    {
        public TournamentMatchInfoRequest(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override ClientPacketType Type => ClientPacketType.TournamentMatchInfoRequest;

        public override Task Execute(PlayerSession session, byte[] data)
        {
            using var reader = new BinaryReader(new MemoryStream(data));
            var id = reader.ReadInt32();

            var lobby = Lobby.GetSession(id);

            if (lobby is null)
            {
                session.Queue.EnqueuePacket(new Notification("Lobbeshnik zakonchilsya"));

                return Task.CompletedTask;
            }


            session.UpdateMatch(lobby.Match);
            return Task.CompletedTask;
        }
    }
}
