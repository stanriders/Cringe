using System;
using System.Threading.Tasks;
using Cringe.Types;
using Cringe.Types.Enums;

namespace Cringe.Bancho.RequestPackets
{
    public class PingPacket : RequestPacket
    {
        public PingPacket(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override ClientPacketType Type => ClientPacketType.Ping;
        public override Task Execute(UserToken token, byte[] data)
        {
            return Task.CompletedTask;
        }
    }
}