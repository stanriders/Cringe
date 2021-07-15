using System;
using System.Threading.Tasks;
using Cringe.Bancho.ResponsePackets;
using Cringe.Types;
using Cringe.Types.Enums;

namespace Cringe.Bancho.RequestPackets
{
    public class SendPublicMessagePacket : RequestPacket
    {
        public SendPublicMessagePacket(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override ClientPacketType Type => ClientPacketType.SendPublicMessage;

        public override async Task Execute(UserToken token, byte[] data)
        {
            var dest = data[9..];
            var message = await Message.Parse(dest, token.Username);
            Pool.ActionMapFilter(x => x.EnqueuePacket(message), id => id != token.PlayerId);
        }
    }
}