using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Cringe.Bancho.Bancho;
using Cringe.Bancho.Bancho.ResponsePackets;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Cringe.Bancho.Services;

public class InvokeService
{
    //Create all IRequest handlers and map their ClientPacketType to corresponding type
    private static readonly Dictionary<ClientPacketType, Type> _requestHandlers = Assembly.GetEntryAssembly()!
        .GetTypes()
        .Where(x => !x.IsAbstract)
        .Where(x => typeof(RequestPacket).IsAssignableFrom(x))
        .Select(x => (RequestPacket) Activator.CreateInstance(x))
        .Where(x => x is not null)
        .ToDictionary(x => x.Type, x => x.GetType());

    private readonly IMediator _mediator;
    private readonly ILogger<InvokeService> _logger;

    public InvokeService(IMediator mediator, ILogger<InvokeService> logger)
    {
        _mediator = mediator;
        _logger = logger;
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
            if (!_requestHandlers.TryGetValue(type, out var request))
                continue;

            try
            {
                var handler = PeppyConverter.Deserialize(request, data);
                await _mediator.Send(handler);
            }
            catch (Exception e)
            {
                // we don't want a single packet crashing the rest in the queue
                _logger.LogError(e, "Packet {packet} failed", request.Name);
                session.Queue.EnqueuePacket(new Notification(e.Message));
            }
        }
    }
}
