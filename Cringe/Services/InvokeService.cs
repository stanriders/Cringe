using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Cringe.Bancho;
using Cringe.Bancho.RequestPackets;
using Cringe.Types;
using Cringe.Types.Enums;

namespace Cringe.Services
{
    public class InvokeService
    {
        private readonly Dictionary<ClientPacketType, RequestPacket> _handlers;

        public InvokeService(IServiceProvider serviceProvider)
        {
            _handlers = new RequestPacket[]
            {
                new ChangeActionPacket(serviceProvider),
                new ChannelJoinPacket(serviceProvider),
                new ChannelPart(serviceProvider),
                new CreateMatch(serviceProvider),
                new JoinLobby(serviceProvider),
                new LoginPacket(serviceProvider),
                new LogoutPacket(serviceProvider),
                new MatchChangeSlot(serviceProvider),
                new MatchChangeMods(serviceProvider),
                new MatchChangeSettings(serviceProvider),
                new MatchNotReady(serviceProvider),
                new MatchReady(serviceProvider),
                new PartLobby(serviceProvider),
                new PartMatch(serviceProvider),
                new PingPacket(serviceProvider),
                new RequestStatusUpdatePacket(serviceProvider),
                new SendPrivateMessagePacket(serviceProvider),
                new SendPublicMessagePacket(serviceProvider),
                new UserStatsRequest(serviceProvider),
                new UserPresenceRequestPacket(serviceProvider)
            }.ToDictionary(x => x.Type);
        }

        public RequestPacket Get(ClientPacketType packetType)
        {
            return _handlers.TryGetValue(packetType, out var request) ? request : null;
        }

        public Task InvokeOne(ClientPacketType packetType, UserToken token, byte[] body)
        {
            return _handlers.TryGetValue(packetType, out var request) ? request.Execute(token, body) : null;
        }

        public Task Invoke(UserToken token, byte[] body)
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

            foreach (var (type, data) in packets)
            {
                if (!_handlers.TryGetValue(type, out var request)) continue;
                var requestTask = request.Execute(token, data);
                if (requestTask.Exception is not null) throw requestTask.Exception;
            }

            return Task.CompletedTask;
        }
    }
}