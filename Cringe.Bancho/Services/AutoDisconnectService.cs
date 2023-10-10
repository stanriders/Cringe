using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Cringe.Bancho.Services
{
    public class AutoDisconnectService : IHostedService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private Timer _timer = null;

        public AutoDisconnectService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(TimerCallback, null, TimeSpan.Zero, TimeSpan.FromMinutes(5));
            return Task.CompletedTask;
        }

        private void TimerCallback(object o)
        {
            using var scope = _serviceScopeFactory.CreateScope();

            var pool = scope.ServiceProvider.GetRequiredService<PlayersPool>();
            if (pool == null)
                return;

            var players = PlayersPool.GetPlayerSessions().ToList();
            foreach (var player in players.Where(player => player.LastUpdate.AddMinutes(5) < DateTime.Now))
            {
                pool.Disconnect(player.Token);
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _timer.DisposeAsync();
        }
    }
}
