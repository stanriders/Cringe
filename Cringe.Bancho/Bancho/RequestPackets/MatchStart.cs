using System;
using System.Threading.Tasks;
using Cringe.Bancho.Bancho.ResponsePackets;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;
using Cringe.Types.Enums.Multiplayer;
using Microsoft.Extensions.Logging;

namespace Cringe.Bancho.Bancho.RequestPackets
{
    public class MatchStart : RequestPacket
    {
        public MatchStart(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override ClientPacketType Type => ClientPacketType.MatchStart;

        public override Task Execute(PlayerSession session, byte[] data)
        {
            if (session.MatchSession is null)
            {
                Logger.LogError("{Token} | MatchStart packet while not in match", session.Token);

                return Task.CompletedTask;
            }

            var match = session.MatchSession.Match;
            if (match.Host != session.Token.PlayerId)
            {
                Logger.LogInformation("{Token} | Attempted to start match as non-host", session.Token);

                return Task.CompletedTask;
            }

            match.InProgress = true;
            foreach (var player in match.Players)
            {
                if (player.Status == SlotStatus.no_map) continue;

                player.Status = SlotStatus.playing;
            }

            var response = new ResponsePackets.MatchStart(match);
            foreach (var player in match.Players)
            {
                if (player.Status != SlotStatus.playing) continue;

                player.Player.Queue.EnqueuePacket(response);
            }

            session.MatchSession.OnUpdateMatch(true);

            return Task.CompletedTask;
        }
    }
}
