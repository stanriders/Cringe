using System.Collections.Concurrent;
using Cringe.Bancho.Types;
using Microsoft.Extensions.Logging;

namespace Cringe.Bancho.Services;

public class SpectateService
{
    private readonly ILogger<SpectateService> _logger;
    public readonly ConcurrentDictionary<int, SpectateSession> Pool;

    public SpectateService(ILogger<SpectateService> logger)
    {
        _logger = logger;
        Pool = new ConcurrentDictionary<int, SpectateSession>();
    }

    public void StartSpectating(PlayerSession host, PlayerSession spectator)
    {
        if (!Pool.TryGetValue(host.Id, out var spectate))
        {
            _logger.LogDebug("SpectateService | No spectate session for {HostId}, creating new one", host.Id);
            spectate = new SpectateSession(host, Destroy);
            host.SpectateSession = spectate;
            host.ChatConnected(GlobalChat.SpectateCount(1));
            host.ChatInfo(GlobalChat.SpectateCount(1));
            if (!Pool.TryAdd(host.Id, spectate))
            {
                _logger.LogError(
                    "SpectateService | Can't create a session. Current sessions: {@Sessions} and the service tries to add {@Session}",
                    Pool.Values, spectate);
            }
        }

        if (spectator.SpectateSession is not null)
        {
            _logger.LogDebug("{Token} | Already in spec session {@Session}", spectator.Token,
                spectator.SpectateSession);
            spectator.SpectateSession.Disconnect(spectator);
        }

        if (spectate.Viewers.ContainsKey(spectator.Id))
            return;

        spectate.Connect(spectator);
        _logger.LogDebug("{Token} | Connected to {@Spec}", spectator.Token, spectate);
    }

    public void NukeOrLogout(PlayerSession session)
    {
        if (session.SpectateSession is null) return;

        var spec = session.SpectateSession;
        if (spec.Host == session)
            Destroy(session.Id);
        else
            session.SpectateSession.Disconnect(session);
    }

    private void Destroy(int id)
    {
        if (Pool.TryRemove(id, out var spec))
        {
            spec.Host.SpectateSession = null;
            _logger.LogDebug("SpectateService | {@Spec} removed", spec);

            return;
        }

        _logger.LogError("SpectateService | Can't remove {Id} session", id);
    }
}
