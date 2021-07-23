using System;
using System.Threading.Tasks;
using Cringe.Bancho.ResponsePackets;
using Cringe.Types;

namespace Cringe.Services
{
    public class LobbyService : ISocial
    {
        private event Action<Match> NewMatch;
        private event Action<Match> DisposeMatch;
        public Task<bool> Connect(PlayerSession player)
        {
            NewMatch += player.NewMatch;
            DisposeMatch += player.DisposeMatch;
            return Task.FromResult(true);
        }

        public bool Disconnect(PlayerSession player)
        {
            NewMatch -= player.NewMatch;
            DisposeMatch -= player.DisposeMatch;
            return true;
        }

        protected virtual void OnNewMatch(Match obj)
        {
            NewMatch?.Invoke(obj);
        }

        protected virtual void OnDisposeMatch(Match obj)
        {
            DisposeMatch?.Invoke(obj);
        }
    }
}