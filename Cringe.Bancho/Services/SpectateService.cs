using System.Collections.Concurrent;
using System.Linq;
using Cringe.Bancho.Types;
using Microsoft.Extensions.Logging;

namespace Cringe.Bancho.Services
{
    public class SpectateService
    {
        private readonly Logger<SpectateService> _logger;
        private readonly ConcurrentDictionary<int, SpectateSession> _pool;

        public SpectateService(Logger<SpectateService> logger)
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
                spectator.SpectateSession.Disconnect(spectator);

            if (spectate.Viewers.Contains(spectator))
                return;

            spectate.Connect(spectator);
        }

        private void Destroy(int id)
        {
            if (_pool.TryRemove(id, out _)) return;

            _logger.LogError("SpectateService | Can't remove {Id} session", id);
        }
    }
}
