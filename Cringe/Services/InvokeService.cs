using System;
using System.Collections.Generic;
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
                new LoginPacket(serviceProvider),
                new LogoutPacket(serviceProvider),
                new PingPacket(serviceProvider),
                new RequestStatusUpdatePacket(serviceProvider),
                new SendPublicMessagePacket(serviceProvider),
                new SendPrivateMessagePacket(serviceProvider),
                new UserStatsRequest(serviceProvider),
                new UserPresenceRequestPacket(serviceProvider)
            }.ToDictionary(x => x.Type);
        }

        public RequestPacket Get(ClientPacketType packetType)
        {
            return _handlers.TryGetValue(packetType, out var request) ? request : null;
        }

        public Task Invoke(ClientPacketType packetType, UserToken token, byte[] body)
        {
            return _handlers.TryGetValue(packetType, out var request) ? request.Execute(token, body) : null;
        }
    }
}