using System;
using System.Linq;
using System.Threading.Tasks;
using Cringe.Bancho.ResponsePackets;
using Cringe.Types;
using Cringe.Types.Enums;

namespace Cringe.Bancho.RequestPackets
{
    public class MatchChangeSettings : RequestPacket
    {
        public MatchChangeSettings(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override ClientPacketType Type => ClientPacketType.MatchChangeSettings;

        public override Task Execute(UserToken token, byte[] data)
        {
            var lobby = Lobby.Parse(data);
            var curLobby = Multiplayer.GetFromUser(token.PlayerId);
            if (lobby.Id != curLobby.Id || curLobby.Host != token.PlayerId) return Task.CompletedTask;
            curLobby.Host = lobby.Host;
            curLobby.Mode = lobby.Mode;
            curLobby.Name = lobby.Name;
            curLobby.Password = lobby.Password;
            curLobby.FreeMode = lobby.FreeMode;
            curLobby.InProgress = lobby.InProgress;
            curLobby.MapId = lobby.MapId;
            curLobby.MapMd5 = lobby.MapMd5;
            curLobby.MapName = lobby.MapName;
            curLobby.TeamTypes = lobby.TeamTypes;
            curLobby.WinConditions = lobby.WinConditions;
            Pool.ActionOn(curLobby.Players.Select(x => x.Id), queue => queue.EnqueuePacket(new UpdateMatch(curLobby)));
            return Task.CompletedTask;
        }
    }
}