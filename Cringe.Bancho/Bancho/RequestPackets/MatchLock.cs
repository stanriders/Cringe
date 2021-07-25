using System;
using System.IO;
using System.Threading.Tasks;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;
using Cringe.Types.Enums.Multiplayer;

namespace Cringe.Bancho.Bancho.RequestPackets
{
    public class MatchLock : RequestPacket
    {
        public MatchLock(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override ClientPacketType Type => ClientPacketType.MatchLock;

        public override Task Execute(PlayerSession session, byte[] data)
        {
            if (session.MatchSession is null || session.MatchSession.Match.Host != session.Player.Id)
                return Task.CompletedTask;

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
                    return Task.CompletedTask;

                if (lockedSlot.Player is not null)
                    session.MatchSession.Disconnect(lockedSlot.Player);

                lockedSlot.Status = SlotStatus.locked;
            }

            session.MatchSession.OnUpdateMatch();

            return Task.CompletedTask;
        }
    }
}
