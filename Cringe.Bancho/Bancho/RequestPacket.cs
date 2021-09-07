using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Cringe.Bancho.Services;
using Cringe.Bancho.Services.Bots;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;
using Cringe.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Cringe.Bancho.Bancho
{
    public abstract class RequestPacket
    {
        private readonly IServiceProvider _serviceProvider;

        protected readonly ILogger Logger;

        protected RequestPacket(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            Logger = _serviceProvider.GetService<ILogger<RequestPacket>>(); //TODO: idk if it works
        }

        protected IBotService Bots => _serviceProvider.GetService<IBotService>();
        protected PlayersPool Pool => _serviceProvider.GetService<PlayersPool>();
        protected LobbyService Lobby => _serviceProvider.GetService<LobbyService>();
        protected ChatService Chats => _serviceProvider.GetService<ChatService>();
        protected StatsService Stats => _serviceProvider.GetService<StatsService>();
        protected SpectateService Spectate => _serviceProvider.GetService<SpectateService>();
        public abstract ClientPacketType Type { get; }
        protected abstract string ApiPath { get; }
        public abstract Task Execute(PlayerSession session, byte[] data);

        public static string ReadString(Stream stream)
        {
            stream.ReadByte();
            var len = (int) stream.ReadLEB128Unsigned();
            var buffer = new byte[len];
            stream.Read(buffer, 0, len);

            return Encoding.Latin1.GetString(buffer);
        }

        protected static int ReadInt(byte[] data)
        {
            using var reader = new BinaryReader(new MemoryStream(data));

            return reader.ReadInt32();
        }

        protected static IEnumerable<int> ReadI32(BinaryReader reader)
        {
            var length = reader.ReadInt16();
            var buffer = new int[length];
            for (var i = 0; i < length; i++) buffer[i] = reader.ReadInt32();

            return buffer;
        }
    }
}
