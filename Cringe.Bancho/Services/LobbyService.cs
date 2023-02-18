using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cringe.Bancho.Bancho.ResponsePackets.Match;
using Cringe.Bancho.Events;
using Cringe.Bancho.Events.Lobby;
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
    private static readonly HashSet<int> _connectedPlayers = new();
    public IEnumerable<int> ConnectedPlayers => _connectedPlayers.ToList();

    public LobbyService(ILogger<LobbyService> logger, IPublishingStrategy publishingStrategy)
    {
        _logger = logger;
        _publishingStrategy = publishingStrategy;
    }

    public async Task JoinLobby(int playerId)
    {
        _connectedPlayers.Add(playerId);
        var matches = _matches.Select(x => new NewMatch(x.Value)).ToList();
        PlayersPool.Notify(playerId, matches);
        await _publishingStrategy.Publish(new LobbyPlayerJoinedEvent(playerId));
    }

    public async Task LeaveLobby(int playerId)
    {
        _connectedPlayers.Remove(playerId);
        await _publishingStrategy.Publish(new LobbyPlayerLeftEvent(playerId));
    }

    #region Matches
    public async Task Transform(short matchId, Action<Match> transformer)
    {
        var match = AssertMatchExistence(matchId);
        await match.Dispatch(x => transformer((Match) x),
            events => _publishingStrategy.Publish(events));
    }

    public T GetValue<T>(short matchId, Func<Match, T> selector)
    {
        return selector(AssertMatchExistence(matchId));
    }

    public async Task<Match> CreateMatch(Match match)
    {
        match.Id = (short) (_matches.Count + 1);

        _logger.LogInformation("{MatchId} | Creating multiplayer {Name}", match.Id, match.RoomName);

        _matches.Add(match.Id, match);

        PlayersPool.Notify(_connectedPlayers, new NewMatch(match));
        await _publishingStrategy.Publish(new MatchCreatedEvent(match));

        return match;
    }

    public short FindMatch(int playerId)
    {
        if (_playerMatches.TryGetValue(playerId, out var matchId))
        {
            return matchId;
        }

        throw new Exception("ty gde ;D");
    }

    public async Task<Match> JoinMatch(int userId, short matchId, string password)
    {
        var match = AssertMatchExistence(matchId);
        await match.Dispatch(x => ((Match) x).AddPlayer(userId, password), x => _publishingStrategy.Publish(x));

        _playerMatches.Add(userId, matchId);

        return match;
    }

    public async Task<Match> LeaveMatch(int userId, short matchId)
    {
        var match = AssertMatchExistence(matchId);
        await match.Dispatch(x => ((Match) x).RemovePlayer(userId), x => _publishingStrategy.Publish(x));

        _playerMatches.Remove(userId);

        return match;
    }

    public void RemoveMatch(short matchId)
    {
        if (!_matches.TryGetValue(matchId, out var match)) return;

        _matches.Remove(matchId);
        PlayersPool.Notify(_connectedPlayers, new DisposeMatch(match));
    }

    private static Match AssertMatchExistence(short matchId)
    {
        if (_matches.TryGetValue(matchId, out var match))
        {
            return match;
        }

        throw new Exception("Lobby doesn't exist");
    }
    #endregion
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
            await _lobby.LeaveMatch(notification.PlayerId, matchId);
        }
        catch (Exception)
        {
            // ignored
        }
    }
}

public class RemoveMatchOnDisbandHandler : INotificationHandler<MatchDisbandedEvent>
{
    private readonly ILogger<RemoveMatchOnDisbandHandler> _logger;
    private readonly LobbyService _lobby;

    public RemoveMatchOnDisbandHandler(ILogger<RemoveMatchOnDisbandHandler> logger, LobbyService lobby)
    {
        _logger = logger;
        _lobby = lobby;
    }

    public Task Handle(MatchDisbandedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{matchId} | Match disbanded", notification.MatchId);
        _lobby.RemoveMatch(notification.MatchId);

        return Task.CompletedTask;
    }
}

public class UpdateLobbyEventHandler : INotificationHandler<UpdateLobbyEvent>
{
    private readonly ILogger<UpdateLobbyEventHandler> _logger;
    private readonly LobbyService _lobby;

    public UpdateLobbyEventHandler(ILogger<UpdateLobbyEventHandler> logger, LobbyService lobby)
    {
        _logger = logger;
        _lobby = lobby;
    }

    public Task Handle(UpdateLobbyEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogDebug("{matchId} | Match requests lobby update", notification.Match.Id);
        PlayersPool.Notify(_lobby.ConnectedPlayers, new UpdateMatch(notification.Match));

        return Task.CompletedTask;
    }
}
