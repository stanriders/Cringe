using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Cringe.Services;
using Cringe.Types;
using Cringe.Types.Enums;
using Cringe.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cringe.Bancho
{
    public abstract class RequestPacket
    {
        protected BanchoServicePool Pool => _serviceProvider.GetService<BanchoServicePool>();
        protected TokenService Token => _serviceProvider.GetService<TokenService>();
        protected ChatServicePool Chats => _serviceProvider.GetService<ChatServicePool>();
        protected StatsService Stats => _serviceProvider.GetService<StatsService>();
        protected IConfiguration Configuration => _serviceProvider.GetService<IConfiguration>();
        private readonly IServiceProvider _serviceProvider;
        protected RequestPacket(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        } 
        public abstract ClientPacketType Type { get; }
        public abstract Task Execute(UserToken token, byte[] data);

        public static int ReadInt(BinaryReader reader)
        {
            return reader.ReadInt32();
        }
        public static string ReadString(Stream stream)
        {
            stream.ReadByte();
            var len = (int) stream.ReadLEB128Unsigned();
            var buffer = new byte[len];
            stream.Read(buffer, 0, len);
            return Encoding.Latin1.GetString(buffer);
        }

        public static int SkipToData(BinaryReader reader)
        {
            reader.ReadBytes(3);
            var length = reader.ReadInt32();
            reader.ReadBytes(2);
            return length;
        }

        public static IEnumerable<int> ReadI32(BinaryReader reader, int length)
        {
            var buffer = new int[length];
            for (var i = 0; i < length; i++) buffer[i] = reader.ReadInt32();
            return buffer;
        } 
    }
}