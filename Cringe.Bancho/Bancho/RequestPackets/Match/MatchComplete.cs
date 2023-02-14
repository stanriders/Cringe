﻿using System.Threading;
using System.Threading.Tasks;
using Cringe.Bancho.Services;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;
using MediatR;

namespace Cringe.Bancho.Bancho.RequestPackets.Match;

public class MatchCompleteHandler : IRequestHandler<MatchComplete>
{
    private static readonly object _lock = new();
    private readonly LobbyService _lobby;
    private readonly PlayerSession _session;

    public MatchCompleteHandler(LobbyService lobby, CurrentPlayerProvider currentPlayerProvider)
    {
        _lobby = lobby;
        _session = currentPlayerProvider.Session;
    }

    public Task Handle(MatchComplete request, CancellationToken cancellationToken)
    {
        var match = _lobby.FindMatch(_session.Id);
        lock (_lock)
        {
            _lobby.Transform(match, x => x.Complete(_session.Id));
        }

        return Task.CompletedTask;
    }
}

public class MatchComplete : RequestPacket, IRequest
{
    public override ClientPacketType Type => ClientPacketType.MatchComplete;
}
