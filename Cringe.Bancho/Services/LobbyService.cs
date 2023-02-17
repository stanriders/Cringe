using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cringe.Bancho.Events;
using Cringe.Bancho.Events.Multiplayer;
using Cringe.Bancho.Types;
using Cringe.Types.Common;
using MediatR;
using Microsoft.Extensions.Logging;

// ReSharper disable MemberCanBeMadeStatic.Global

namespace Cringe.Bancho.Services;

public class LobbyService
{
    private readonly ILogger<LobbyService> _logger;

    private readonly IPublishingStrategy _publishingStrategy;

    //TODO: very unlikely that we change it but still *not good*
    private static readonly Dictionary<short, Match> _matches = new();
    private static readonly Dictionary<int, short> _playerMatches = new();

    public LobbyService(ILogger<LobbyService> logger, IPublishingStrategy publishingStrategy)
    {
        _logger = logger;
        _publishingStrategy = publishingStrategy;
    }

    public Match CreateLobby(Match match)
    {
        match.Id = (short) (_matches.Count + 1);

        _logger.LogInformation("{MatchId} | Creating multiplayer {Name}", match.Id, match.RoomName);

        _matches.Add(match.Id, match);

        return match;
    }

    public async Task Transform(short matchId, Action<Match> transformer)
    {
        var match = AssertLobbyExistence(matchId);
        await match.Dispatch(x => transformer((Match) x),
            events => _publishingStrategy.Publish(events));
    }

    public T GetValue<T>(short matchId, Func<Match, T> selector)
    {
        return selector(AssertLobbyExistence(matchId));
    }

    public short FindMatch(int playerId)
    {
        if (_playerMatches.TryGetValue(playerId, out var matchId))
        {
            return matchId;
        }

        throw new Exception("ty gde ;D");
    }

    public async Task<Match> JoinLobby(int userId, short matchId, string password)
    {
        var match = AssertLobbyExistence(matchId);
        await match.Dispatch(x => ((Match) x).AddPlayer(userId, password), x => _publishingStrategy.Publish(x));

        _playerMatches.Add(userId, matchId);

        return match;
    }

    public async Task<Match> LeaveLobby(int userId, short matchId)
    {
        var match = AssertLobbyExistence(matchId);
        await match.Dispatch(x => ((Match) x).RemovePlayer(userId), x => _publishingStrategy.Publish(x));

        _playerMatches.Remove(userId);

        return match;
    }

    public void RemoveMatch(short matchId)
    {
        _matches.Remove(matchId);
    }

    private static Match AssertLobbyExistence(short matchId)
    {
        if (_matches.TryGetValue(matchId, out var match))
        {
            return match;
        }

        throw new Exception("Lobby doesn't exist");
    }
}

public class ForceRemovePlayerFromLobbyOnPlayerLeftEventHandler : INotificationHandler<PlayerLeftEvent>
{
    private readonly LobbyService _lobby;

    public ForceRemovePlayerFromLobbyOnPlayerLeftEventHandler(LobbyService lobby)
    {
        _lobby = lobby;
    }

    public async Task Handle(PlayerLeftEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            var matchId = _lobby.FindMatch(notification.PlayerId);
            await _lobby.LeaveLobby(notification.PlayerId, matchId);
        }
        catch (Exception)
        {
            // ignored
        }
    }
}

public class RemoveMatchOnDisbandHandler : INotificationHandler<MatchDisbandedEvent>
{
    private readonly LobbyService _lobby;

    public RemoveMatchOnDisbandHandler(LobbyService lobby)
    {
        _lobby = lobby;
    }

    public Task Handle(MatchDisbandedEvent notification, CancellationToken cancellationToken)
    {
        _lobby.RemoveMatch(notification.MatchId);

        return Task.CompletedTask;
    }
}
