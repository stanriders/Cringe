using System.Collections.Generic;
using System.Linq;
using Cringe.Bancho;
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
            lobby.Id = GetId();
            _lobbies.Add(lobby);
            _chats.Create(Chat.Multiplayer(lobby.Id.ToString()));
            UpdateLobby(new NewMatch(lobby));
        }

        private void UpdateLobby(ResponsePacket packet)
        {
            var users = _chats.GetLobbyUsers();
            _pool.ActionOn(users, queue =>
            {
                queue.EnqueuePacket(packet);
            });
        }
        
        public Lobby GetFromUser(int id)
        {
            return _lobbies.FirstOrDefault(x => x.Players.Any(v => v == id));
        }

        public void Connect(Player player, int lobbyId, string password)
        {
            var userId = player.Id;
            var lobby = _lobbies.FirstOrDefault(x => x.Id == lobbyId);
            if (lobby is null)
            {
                _pool.ActionOn(userId, queue =>
                { 
                    queue.EnqueuePacket(new MatchJoinFail());
                    queue.EnqueuePacket(new Notification("AYE LOBBI DOES NOT EXIST"));
                });
                return;
            }

            if (lobby.Password != password)
            {
                _pool.ActionOn(userId, queue =>
                {
                    queue.EnqueuePacket(new MatchJoinFail());
                    queue.EnqueuePacket(new Notification("WRONG NUMBERS"));
                });
                return;
            }

            var res = lobby.Connect(player);
            if (!res)
            {
                _pool.ActionOn(userId, queue =>
                {
                    queue.EnqueuePacket(new MatchJoinFail());
                    queue.EnqueuePacket(new Notification("DOUBLE SHTAK LOBBI FULL"));
                });
                return;
            }
            _chats.Connect(userId, "#multiplayer" + lobbyId);
            _pool.ActionOn(lobby.Players, queue => queue.EnqueuePacket(new UpdateMatch(lobby)));
            _pool.ActionOn(userId, queue =>
            {
                queue.EnqueuePacket(new MatchJoinSuccess(lobby));
            });
            UpdateLobby(new NewMatch(lobby));
        }
        public void SetLobby(Lobby old, Lobby newLobby)
        {
            _lobbies.Remove(old);
            _lobbies.Add(newLobby);
        }

        public void EnqueueLobbies(int id)
        {
            _pool.ActionOn(id, queue =>
            {
                foreach (var lobby in _lobbies)
                {
                    queue.EnqueuePacket(new NewMatch(lobby));
                }
            });
            
        }
        public void NukePlayer(Player player)
        {
            var lobby = _lobbies.FirstOrDefault(x => x.Players.Any(p => p == player.Id));
            if (lobby is null) return;
            lobby.Disconnect(player);
            _pool.ActionOn(lobby.Players, queue => queue.EnqueuePacket(new UpdateMatch(lobby)));
            _chats.NukeUserFromPrivateChat(player.Id, "#multiplayer" + lobby.Id);
            if (lobby.Players.Count != 0) return;
            
            _lobbies.Remove(lobby);
            UpdateLobby(new DisposeMatch(lobby));
        }

        public void SendMessage(Message message)
        {
            var lobby = _lobbies.FirstOrDefault(x => x.Players.Any(player => player == message.SenderId));
            if (lobby is null) return;

            foreach (var lobbyPlayer in lobby.Players.Where(x => x != message.SenderId))
                _pool.ActionOn(lobbyPlayer, queue => queue.EnqueuePacket(message));
        }

        private short GetId()
        {
            return (short) _lobbies.Count;
        }
    }
}