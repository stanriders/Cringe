using System.Collections.Generic;
using System.Linq;
using Cringe.Bancho.ResponsePackets;
using Cringe.Types;
using Cringe.Types.Database;

namespace Cringe.Services
{
    public class MultiplayerService
    {
        private readonly HashSet<Lobby> _lobbies;
        private readonly BanchoServicePool _pool;

        public MultiplayerService(BanchoServicePool pool)
        {
            _pool = pool;
            _lobbies = new HashSet<Lobby>();
        }

        public void Register(Lobby lobby)
        {
            _lobbies.Add(lobby);
        }

        public Lobby GetFromUser(int id)
        {
            return _lobbies.FirstOrDefault(x => x.Players.Any(v => v.Id == id));
        }
        public void NukePlayer(Player player)
        {
            var lobby = _lobbies.FirstOrDefault(x => x.Players.Contains(player));
            if(lobby is null) return;
            lobby.Disconnect(player);
            if (lobby.Players.Count == 0)
                _lobbies.Remove(lobby);
        }

        public void SendMessage(Message message)
        {
            var lobby = _lobbies.FirstOrDefault(x => x.Players.Any(player => player.Username == message.Sender));
            if(lobby is null) return;
            
            foreach (var lobbyPlayer in lobby.Players.Where(x => x.Username != message.Sender))
                _pool.ActionOn(lobbyPlayer.Id, queue => queue.EnqueuePacket(message));
        }
    }
}