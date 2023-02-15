using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Cringe.Bancho.Events;
using Cringe.Bancho.Events.Multiplayer;
using Cringe.Bancho.Types;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Cringe.Bancho.Services;

public class LobbyService
{
    private readonly ILogger<LobbyService> _logger;
    private readonly ConcurrentDictionary<short, Match> _pool = new();

    public LobbyService(ILogger<LobbyService> logger)
    {
        _logger = logger;
    }

    public Match CreateLobby(Match match)
    {
        //TODO: extract it to match
        match.Id = (short) (_pool.Count + 1);

        _logger.LogInformation("{MatchId} | Creating multiplayer {Name}", match.Id, match.RoomName);
        _pool.TryAdd(match.Id, match);

        return match;
    }

    public void Transform(short matchId, Action<Match> transformer)
    {
        var match = AssertLobbyExistence(matchId);
        transformer(match);
    }

    public T GetValue<T>(short matchId, Func<Match, T> selector)
    {
        return selector(AssertLobbyExistence(matchId));
    }

    public short FindMatch(int playerId)
    {
        //TODO: (refactoring) if we want we may extract it to Dictionary<int, short> because it is one-to-one relation between player and match
        var matches = _pool.ToArray();
        foreach (var match in matches)
        {
            if (!match.Value.PlayerIsInTheMatch(playerId)) continue;

            return match.Key;
        }

        throw new Exception("ty gde ;D");
    }

    public Match JoinLobby(int userId, short matchId, string password)
    {
        var match = AssertLobbyExistence(matchId);
        match.AddPlayer(userId, password);

        return match;
    }

    public Match LeaveLobby(int userId, short matchId)
    {
        var match = AssertLobbyExistence(matchId);
        match.RemovePlayer(userId);

        return match;
    }

    public void RemoveMatch(short matchId)
    {
        _pool.TryRemove(matchId, out _);
    }

    private Match AssertLobbyExistence(short matchId)
    {
        if (!_pool.TryGetValue(matchId, out var match))
        {
            throw new Exception("Lobby doesn't exist");
        }

        return match;
    }
}

public class ForceRemovePlayerFromLobbyOnPlayerLeftEventHandler : INotificationHandler<PlayerLeftEvent>
{
    private readonly LobbyService _lobby;

    public ForceRemovePlayerFromLobbyOnPlayerLeftEventHandler(LobbyService lobby)
    {
        _lobby = lobby;
    }

    public Task Handle(PlayerLeftEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            var matchId = _lobby.FindMatch(notification.PlayerId);
            _lobby.LeaveLobby(notification.PlayerId, matchId);
        }
        catch (Exception)
        {
            // ignored
        }

        return Task.CompletedTask;
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
