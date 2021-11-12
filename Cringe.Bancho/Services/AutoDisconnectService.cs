using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Cringe.Bancho.Services
{
    public class AutoDisconnectService : IHostedService
    {
        private readonly PlayersPool _pool;
        private Timer _timer = null;

        public AutoDisconnectService(PlayersPool pool)
        {
            _pool = pool;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(TimerCallback, null, TimeSpan.Zero, TimeSpan.FromMinutes(5));
            return Task.CompletedTask;
        }

        private void TimerCallback(object o)
        {
            var players = PlayersPool.GetPlayerSessions().ToList();
            var date = DateTime.Now;
            foreach (var player in players.Where(player => date - player.LastUpdate < TimeSpan.FromMinutes(5)))
            {
                _pool.Disconnect(player.Token);
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _timer.DisposeAsync();
        }
    }
}
