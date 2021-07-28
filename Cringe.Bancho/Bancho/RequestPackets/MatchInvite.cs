using System;
using System.IO;
using System.Threading.Tasks;
using Cringe.Bancho.Bancho.ResponsePackets;
using Cringe.Bancho.Services;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;
using Microsoft.Extensions.Logging;

namespace Cringe.Bancho.Bancho.RequestPackets
{
    public class MatchInvite : RequestPacket
    {
        public MatchInvite(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override ClientPacketType Type => ClientPacketType.MatchInvite;

        public override Task Execute(PlayerSession session, byte[] data)
        {
            using var reader = new BinaryReader(new MemoryStream(data));
            var id = reader.ReadInt32();
            if (session.MatchSession is null)
            {
                Logger.LogError("{Token} | Invite while not in match", session.Token);

                return Task.CompletedTask;
            }

            var user = PlayersPool.GetPlayer(id);
            if (user is null)
            {
                session.Queue.EnqueuePacket(new Notification("User is not online"));

                return Task.CompletedTask;
            }

            var embed = $"Zahodi pojugama: [{session.MatchSession.Match.Embed} {session.MatchSession.Match.Name}";
            Logger.LogInformation("{Token} | Invited {UserName} to the match", session.Token, user.Player.Username);
            user.Queue.EnqueuePacket(new ResponsePackets.MatchInvite(session.Player, user.Player.Username, embed));

            return Task.CompletedTask;
        }
    }
}
