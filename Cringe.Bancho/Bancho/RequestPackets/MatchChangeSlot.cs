using System;
using System.IO;
using System.Threading.Tasks;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;

namespace Cringe.Bancho.Bancho.RequestPackets
{
    public class MatchChangeSlot : RequestPacket
    {
        public MatchChangeSlot(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override ClientPacketType Type => ClientPacketType.MatchChangeSlot;
        public override Task Execute(PlayerSession session, byte[] data)
        {
            if (session.MatchSession is null)
                return Task.CompletedTask;
            
            var match = session.MatchSession;
            
            using var reader = new BinaryReader(new MemoryStream(data));
            var slot = reader.ReadInt32();
            
            match.ChangeSlot(session, slot);
            
            return Task.CompletedTask;
        }
    }
}