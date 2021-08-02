using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Cringe.Bancho.Bancho;
using Cringe.Bancho.Bancho.RequestPackets;
using Cringe.Bancho.Bancho.RequestPackets.Match;
using Cringe.Bancho.Bancho.RequestPackets.Spectate;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;
using Microsoft.Extensions.Logging;
using ChangeAction = Cringe.Bancho.Bancho.RequestPackets.ChangeAction;

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
                new ChangeAction(serviceProvider),
                new ChannelJoin(serviceProvider),
                new ChannelPart(serviceProvider),
                new CreateMatch(serviceProvider),
                new FriendAdd(serviceProvider),
                new FriendRemove(serviceProvider),
                new JoinLobby(serviceProvider),
                new JoinMatch(serviceProvider),
                new LogoutPacket(serviceProvider),

                new MatchChangeMods(serviceProvider),
                new MatchChangeSettings(serviceProvider),
                new MatchChangeSlot(serviceProvider),
                new MatchChangeTeam(serviceProvider),
                new MatchChangePassword(serviceProvider),
                new MatchHasBeatmap(serviceProvider),
                new MatchLoadComplete(serviceProvider),
                new MatchLock(serviceProvider),
                new MatchInvite(serviceProvider),
                new MatchNoBeatmap(serviceProvider),
                new MatchNotReady(serviceProvider),
                new MatchReady(serviceProvider),
                new MatchStart(serviceProvider),
                new MatchFailed(serviceProvider),
                new MatchSkipRequest(serviceProvider),
                new MatchTransferHost(serviceProvider),
                new MatchScoreUpdate(serviceProvider),
                new MatchComplete(serviceProvider),

                new PartLobby(serviceProvider),
                new PartMatch(serviceProvider),
                new PingPacket(serviceProvider),
                new RequestStatusUpdate(serviceProvider),
                new SendPrivateMessagePacket(serviceProvider),
                new SendPublicMessagePacket(serviceProvider),
                new SpectateFrame(serviceProvider),
                new StartSpectating(serviceProvider),
                new StopSpectating(serviceProvider),
                new CantSpectate(serviceProvider),
                new UserStatsRequest(serviceProvider),
                new UserPresenceRequest(serviceProvider)
            }.ToDictionary(x => x.Type);
        }

        public async Task Invoke(PlayerSession session, byte[] body)
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

            if (!(packets.Count == 1 && packets[0].type is ClientPacketType.Ping or ClientPacketType.MatchScoreUpdate))
                _logger.LogDebug("{Token} | Invokes these packets\n{Packets}", session.Token,
                    packets.Select(x => x.type));

            foreach (var (type, data) in packets)
            {
                if (!_handlers.TryGetValue(type, out var request))
                    continue;

                try
                {
                    await request.Execute(session, data);
                }
                catch (Exception e)
                {
                    // we don't want a single packet crashing the rest in the queue
                    _logger.LogError($"Packet {request.Type} failed: {e}");
                }

            }
        }
    }
}
