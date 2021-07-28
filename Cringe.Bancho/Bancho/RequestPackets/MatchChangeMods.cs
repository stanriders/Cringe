using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Cringe.Bancho.Bancho.ResponsePackets;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;
using Microsoft.Extensions.Logging;

namespace Cringe.Bancho.Bancho.RequestPackets
{
    public class MatchChangeMods : RequestPacket
    {
        public MatchChangeMods(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override ClientPacketType Type => ClientPacketType.MatchChangeMods;

        public override Task Execute(PlayerSession session, byte[] data)
        {
            using var reader = new BinaryReader(new MemoryStream(data));
            var mods = (Mods) reader.ReadInt32();

            if (session.MatchSession is null)
            {
                Logger.LogError("{Token} | User tries to change mods while his session is null", session.Token);
                session.Queue.EnqueuePacket(new UpdateMatch(Match.NullMatch));

                return Task.CompletedTask;
            }

            var match = session.MatchSession;
            var slot = match.Match.Slots.FirstOrDefault(x => x.Player == session);

            if (slot is null)
            {
                Logger.LogCritical("{Token} | User has MatchSession but not found in any slot of the match.\n{@Match}",
                    session.Token, match);
                session.Queue.EnqueuePacket(new UpdateMatch(Match.NullMatch));

                return Task.CompletedTask;
            }

            if (match.Match.FreeMode)
            {
                if (match.Match.Host == session.Token.PlayerId)
                    match.Match.Mods = mods & Mods.SpeedChangingMods;

                slot.Mods = mods & ~Mods.SpeedChangingMods;
            }
            else
            {
                if (match.Match.Host == session.Token.PlayerId)
                    match.Match.Mods = mods;
                else
                    Logger.LogInformation("{Token} | Attempted to change mods as non-host. The host is {Host}",
                        session.Token, match.Match.Host);
            }

            match.OnUpdateMatch();

            return Task.CompletedTask;
        }
    }
}
