using System;
using System.IO;
using System.Threading.Tasks;
using Cringe.Types;
using Cringe.Types.Enums;

namespace Cringe.Bancho.RequestPackets
{
    public class JoinMatch : RequestPacket
    {
        public JoinMatch(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override ClientPacketType Type => ClientPacketType.JoinMatch;
        public override async Task Execute(UserToken token, byte[] data)
        {
            using var reader = new BinaryReader(new MemoryStream(data));
            var id = reader.ReadInt32();
            var password = ReadString(reader.BaseStream);
            var player = await Token.GetPlayerWithoutScores(token.PlayerId);
            Multiplayer.Connect(player, id, password);
        }
    }
}