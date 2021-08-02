using System;
using System.IO;
using System.Threading.Tasks;
using Cringe.Bancho.Bancho.ResponsePackets;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;
using Cringe.Types.Enums.Multiplayer;
using Microsoft.Extensions.Logging;

namespace Cringe.Bancho.Bancho.RequestPackets.Match
{
    public class MatchLock : RequestPacket
    {
        public MatchLock(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override ClientPacketType Type => ClientPacketType.MatchLock;

        public override Task Execute(PlayerSession session, byte[] data)
        {
            if (session.MatchSession is null)
            {
                Logger.LogError("{Token} | User tries to lock slot while his MatchSession is null", session.Token);

                return Task.CompletedTask;
            }

            if (session.MatchSession.Match.Host != session.Id)
                Logger.LogInformation("{Token} | User tries to lock slot while not being a host", session.Token);
            using var reader = new BinaryReader(new MemoryStream(data));
            var toLock = reader.ReadInt32();
            var lockedSlot = session.MatchSession.Match.Slots[toLock];
            if (lockedSlot.Status == SlotStatus.locked)
            {
                lockedSlot.Status = SlotStatus.open;
            }
            else
            {
                if (lockedSlot.Player is not null && lockedSlot.Player == session)
                {
                    Logger.LogInformation("{Token} | User tries to kill himself with a lock xd", session.Token);

                    return Task.CompletedTask;
                }

                if (lockedSlot.Player is not null)
                {
                    Logger.LogInformation("{Token} | User kicks {KickedPlayerId} with a lock", session.Token,
                        lockedSlot.Player.Id);
                    lockedSlot.Player.Queue.EnqueuePacket(new Notification("You've been killed with a fucking lock"));

                    session.MatchSession.Disconnect(lockedSlot.Player);
                }

                lockedSlot.Status = SlotStatus.locked;
            }

            session.MatchSession.OnUpdateMatch(true);
            Logger.LogDebug("{Token} | User locks a slot. Match info: {@Match}", session.Token,
                session.MatchSession.Match);

            return Task.CompletedTask;
        }
    }
}
