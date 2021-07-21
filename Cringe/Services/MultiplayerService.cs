using System;
using System.Collections.Generic;
using System.Linq;
using Cringe.Bancho.ResponsePackets;
using Cringe.Types;

namespace Cringe.Services
{
    public class MultiplayerService
    {
        private readonly BanchoServicePool _pool;
        private readonly HashSet<Lobby> _lobbies;

        public MultiplayerService(BanchoServicePool pool)
        {
            _pool = pool;
            _lobbies = new HashSet<Lobby>();
        }
        
        public void CreateLobby(Player creator, string lobbyName, string lobbyPassword = "")
        {
            _lobbies.Add(new Lobby());
        }

        public void Connect(Player player, string lobbyName, string lobbyPassword)
        {
            
        }

        public void NukePlayer(Player player, string lobbyName)
        {
            var lobby = _lobbies.FirstOrDefault(x => x.Name == lobbyName);
            if(lobby is null) return;
            lobby.Disconnect(player);
            if (lobby.Players.Count == 0)
                _lobbies.Remove(lobby);
        }

        public void SendMessage(Message message)
        {
            var lobby = _lobbies.FirstOrDefault(x => x.Contains(message.Sender));
            if(lobby is null) return;
            
            foreach (var lobbyPlayer in lobby.Players)
            {
                _pool.ActionOn(lobbyPlayer.Id, queue => queue.EnqueuePacket(message));
            }
        }
    }
}