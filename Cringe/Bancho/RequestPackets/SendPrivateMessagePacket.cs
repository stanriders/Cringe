using System;
using System.Threading.Tasks;
using Cringe.Bancho.ResponsePackets;
using Cringe.Database;
using Cringe.Types;
using Cringe.Types.Enums;
using Microsoft.EntityFrameworkCore;

namespace Cringe.Bancho.RequestPackets
{
    public class SendPrivateMessagePacket : RequestPacket
    {
        public SendPrivateMessagePacket(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override ClientPacketType Type => ClientPacketType.SendPrivateMessage;

        public override async Task Execute(UserToken token, byte[] data)
        {
            var dest = data[2..];
            var message = await Message.Parse(dest, token.Username);
            await using var players = new PlayerDatabaseContext();
            var receivePlayer = await players.Players.FirstOrDefaultAsync(x => x.Username == message.Receiver);
            Pool.ActionOn(receivePlayer.Id, x => x.EnqueuePacket(message));
        }
    }
}