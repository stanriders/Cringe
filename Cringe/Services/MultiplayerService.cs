using System.Collections.Generic;
using System.Linq;
using Cringe.Bancho.ResponsePackets;
using Cringe.Types;
using Cringe.Types.Bancho;
using Cringe.Types.Database;

namespace Cringe.Services
{
    public class MultiplayerService
    {
        private readonly ChatServicePool _chats;
        private readonly HashSet<Lobby> _lobbies;
        private readonly BanchoServicePool _pool;

        public MultiplayerService(BanchoServicePool pool, ChatServicePool chats)
        {
            _pool = pool;
            _chats = chats;
            _lobbies = new HashSet<Lobby>();
        }

        public void Register(Lobby lobby)
        {
            _lobbies.Add(lobby);
            _chats.Create(Chat.Multiplayer(lobby.Id.ToString()));
        }

        public Lobby GetFromUser(int id)
        {
            return _lobbies.FirstOrDefault(x => x.Players.Any(v => v.Id == id));
        }

        public void SetLobby(Lobby old, Lobby newLobby)
        {
            _lobbies.Remove(old);
            _lobbies.Add(newLobby);
        }

        public void NukePlayer(Player player)
        {
            var lobby = _lobbies.FirstOrDefault(x => x.Players.Any(p => p.Id == player.Id));
            if (lobby is null) return;
            lobby.Disconnect(player);
            _chats.NukeUserFromPrivateChat(player.Id, "#multiplayer" + lobby.Id);
            if (lobby.Players.Count == 0)
                _lobbies.Remove(lobby);
        }

        public void SendMessage(Message message)
        {
            var lobby = _lobbies.FirstOrDefault(x => x.Players.Any(player => player.Username == message.Sender));
            if (lobby is null) return;

            foreach (var lobbyPlayer in lobby.Players.Where(x => x.Username != message.Sender))
                _pool.ActionOn(lobbyPlayer.Id, queue => queue.EnqueuePacket(message));
        }
    }
}