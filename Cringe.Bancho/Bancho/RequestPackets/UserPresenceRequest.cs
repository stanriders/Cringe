using System;
using System.IO;
using System.Threading.Tasks;
using Cringe.Bancho.Bancho.ResponsePackets;
using Cringe.Bancho.Services;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;

namespace Cringe.Bancho.Bancho.RequestPackets
{
    public class UserPresenceRequest : RequestPacket
    {
        public UserPresenceRequest(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override ClientPacketType Type => ClientPacketType.UserPresenceRequest;

        public override Task Execute(PlayerSession session, byte[] data)
        {
            using var reader = new BinaryReader(new MemoryStream(data));
            var ids = ReadI32(reader);
            var pool = Pool;
            foreach (var id in ids)
            {
                var user = PlayersPool.GetPlayer(id);
                session.Queue.EnqueuePacket(new UserPresence(user.GetPresence()));
            }
            return Task.CompletedTask;
        }
    }
}