using System;
using System.IO;
using osuLocalBancho.Bancho;
using osuLocalBancho.Types.Enums;

namespace osuLocalBancho.Types
{
    public class Panel
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public byte Timezone { get; set; }
        public byte Country { get; set; }
        public UserRanks UserRank { get; set; }
        public float Longitude { get; set; }
        public float Latitude { get; set; }
        public uint GameRank { get; set; }

        public byte[] Pack()
        {
            using var panelStream = new MemoryStream();

            panelStream.Write(DataPacket.PackData(UserId));
            panelStream.Write(DataPacket.PackData(Username));
            panelStream.WriteByte(Timezone);
            panelStream.WriteByte(Country);
            panelStream.WriteByte((byte)UserRank);
            panelStream.Write(DataPacket.PackData(Longitude));
            panelStream.Write(DataPacket.PackData(Latitude));
            panelStream.Write(DataPacket.PackData(GameRank));

            return panelStream.ToArray();
        }
    }
}
