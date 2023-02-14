﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Cringe.Bancho.Services;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;
using MediatR;
using Microsoft.Extensions.Logging;
using SQLitePCL;

namespace Cringe.Bancho.Bancho.RequestPackets.Match;

public class PartMatchHandler : IRequestHandler<PartMatch>
{
    private readonly LobbyService _lobby;
    private readonly PlayerSession _session;

    public PartMatchHandler(LobbyService lobby, CurrentPlayerProvider currentPlayerProvider)
    {
        _lobby = lobby;
        _session = currentPlayerProvider.Session;
    }

    public Task<Unit> Handle(PartMatch request, CancellationToken cancellationToken)
    {
        var matchId = _lobby.FindMatch(_session.Id);
        _lobby.LeaveLobby(_session.Id, matchId);

        return Unit.Task;
    }
}

public class PartMatch : RequestPacket, IRequest<Unit>
{
    public override ClientPacketType Type => ClientPacketType.PartMatch;
}
