using System;
using System.Linq;
using Cringe.Bancho.Bancho.ResponsePackets;
using Cringe.Types.Enums.Multiplayer;

namespace Cringe.Bancho.Types
{
    public class MatchSession
    {
        public Action<Match> Dispose;

        public MatchSession(short id, PlayerSession host, Match match, Action<Match> dispose)
        {
            match.Id = id;
            Dispose = dispose;
            Match = match;
            Match.Host = host.Token.PlayerId;
        }

        public Match Match { get; private set; }

        public event Action<Match> UpdateMatch;

        public void Connect(PlayerSession session)
        {
            Register(session);
            session.Queue.EnqueuePacket(new MatchJoinSuccess(Match));
            session.Queue.EnqueuePacket(new ChannelJoinSuccess(GlobalChat.Multiplayer));
            OnUpdateMatch();
        }

        private void Register(PlayerSession session)
        {
            var slot = Match.Slots.FirstOrDefault(x => x.Player is null);
            if (slot is null)
            {
                session.Queue.EnqueuePacket(new MatchJoinFail());
                session.Queue.EnqueuePacket(new Notification("Lobby is full D:"));

                return;
            }

            slot.Player = session;
            slot.Status = SlotStatus.not_ready;
            session.MatchSession = this;

            UpdateMatch += session.UpdateMatch;
        }

        public void Disconnect(PlayerSession session)
        {
            if (session.MatchSession != this)
                return;

            session.Queue.EnqueuePacket(new ChannelKick(GlobalChat.Multiplayer));
            session.MatchSession = null;

            var slots = Match.Slots.Where(x => x.Player is not null).ToArray();
            slots.FirstOrDefault(x => x.Player == session)?.Wipe();

            if (slots.Length <= 1)
            {
                Dispose(Match);
                OnUpdateMatch();
                UpdateMatch -= session.UpdateMatch;

                return;
            }

            if (Match.Host == session.Player.Id)
            {
                var host = slots[new Random().Next(0, slots.Length)].Player;

                Match.Host = host.Player.Id;

                host.Queue.EnqueuePacket(new MatchTransferHost());
            }

            UpdateMatch -= session.UpdateMatch;
            OnUpdateMatch();
        }

        public virtual void OnUpdateMatch()
        {
            UpdateMatch?.Invoke(Match);
        }

        public void ChangeSlot(PlayerSession session, int slotId)
        {
            if (slotId is > 16 or < 0) return;

            var slot = Match.Slots[slotId];

            if (slot.Player is not null) return;

            var oldSlot = Match.Slots.FindIndex(x => x.Player == session);

            if (oldSlot == -1)
                throw new Exception($"{session.Player.Id} tries to change the slot to {slotId} when not in the lobby");

            Match.Slots[slotId] = Match.Slots[oldSlot];
            Match.Slots[oldSlot] = slot;
            OnUpdateMatch();
        }

        public void Update(Match match)
        {
            var oldMatch = Match;
            match.Slots = oldMatch.Slots;
            Match = match;
            OnUpdateMatch();
        }

        public override string ToString()
        {
            return Match.ToString();
        }
    }
}
