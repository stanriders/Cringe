﻿using System;
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
                new JoinLobby(serviceProvider),
                new LogoutPacket(serviceProvider),
                new PartLobby(serviceProvider),
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

        public Task Invoke(PlayerSession session, byte[] body)
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
                var requestTask = request.Execute(session, data);
                if (requestTask.Exception is not null) throw requestTask.Exception;
            }

            return Task.CompletedTask;
        }
    }
}