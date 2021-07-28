using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Cringe.Bancho.Bancho;
using Cringe.Bancho.Bancho.RequestPackets;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;
using Microsoft.Extensions.Logging;

namespace Cringe.Bancho.Services
{
    public class InvokeService
    {
        private readonly Dictionary<ClientPacketType, RequestPacket> _handlers;
        private readonly ILogger<InvokeService> _logger;

        public InvokeService(IServiceProvider serviceProvider, ILogger<InvokeService> logger)
        {
            _logger = logger;
            _handlers = new RequestPacket[]
            {
                new ChangeActionPacket(serviceProvider),
                new ChannelJoinPacket(serviceProvider),
                new ChannelPart(serviceProvider),
                new CreateMatch(serviceProvider),
                new JoinLobby(serviceProvider),
                new JoinMatch(serviceProvider),
                new LogoutPacket(serviceProvider),
                new MatchChangeMods(serviceProvider),
                new MatchChangeSettings(serviceProvider),
                new MatchChangeSlot(serviceProvider),
                new MatchLock(serviceProvider),
                new PartLobby(serviceProvider),
                new PartMatch(serviceProvider),
                new PingPacket(serviceProvider),
                new RequestStatusUpdate(serviceProvider),
                new SendPrivateMessagePacket(serviceProvider),
                new SendPublicMessagePacket(serviceProvider),
                new UserStatsRequest(serviceProvider),
                new UserPresenceRequest(serviceProvider)
            }.ToDictionary(x => x.Type);
        }

        public void Invoke(PlayerSession session, byte[] body)
        {
            using var reader = new BinaryReader(new MemoryStream(body));
            var packets = new List<(ClientPacketType type, byte[] data)>();
            while (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                var type = (ClientPacketType) reader.ReadInt16();
                reader.ReadByte();
                var length = reader.ReadInt32();
                var packetData = reader.ReadBytes(length);
                packets.Add((type, packetData));
            }

            _logger.LogDebug("{Token} | Invokes these packets\n{Packets}", session.Token, packets.Select(x => x.type));

            foreach (var (type, data) in packets)
            {
                if (!_handlers.TryGetValue(type, out var request)) continue;

                var requestTask = request.Execute(session, data);

                if (requestTask.Exception is not null) throw requestTask.Exception;
            }
        }
    }
}
