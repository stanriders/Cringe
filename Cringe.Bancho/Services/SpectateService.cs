using System.Collections.Concurrent;
using System.Linq;
using Cringe.Bancho.Bancho.ResponsePackets;
using Cringe.Bancho.Types;
using Microsoft.Extensions.Logging;

namespace Cringe.Bancho.Services
{
    public class SpectateService
    {
        private readonly ConcurrentDictionary<int, SpectateSession> _pool;
        private readonly ILogger<SpectateService> _logger;
        public SpectateService(ILogger<SpectateService> logger)
        {
            _logger = logger;
            _pool = new ConcurrentDictionary<int, SpectateSession>();
        }

        public void StartSpectating(PlayerSession host, PlayerSession spectator)
        {
            if (!_pool.TryGetValue(host.Id, out var spectate))
            {
                spectate = new SpectateSession(host, Destroy);
                host.SpectateSession = spectate;
                _pool.TryAdd(host.Id, spectate);
            }

            if (spectator.SpectateSession != spectate)
            {
                spectator.SpectateSession.Disconnect(spectator);
            }

            if (spectate.Viewers.Contains(spectator))
            {
                return;
            }

            spectate.Connect(spectator);
            _logger.LogDebug("{Token} | Connected to {@Spec}", spectator.Token, spectate);
        }

        private void Destroy(int id)
        {
            if (_pool.TryRemove(id, out var spec))
            {
                spec.Host.SpectateSession = null;
                _logger.LogDebug("SpectateService | {@Spec} removed", spec);
                return;
            }

            _logger.LogError("SpectateService | Can't remove {Id} session", id);
        }
    }
}
