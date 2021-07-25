using System;
using System.Collections.Generic;

namespace Cringe.Bancho.Types
{
    public class PlayerEvent
    {
        public PlayerEvent()
        {
            SessionList = new List<PlayerSession>();
        }

        public List<PlayerSession> SessionList { get; }
        public int Count => SessionList.Count;

        public void Add(PlayerSession session)
        {
            if (!SessionList.Contains(session))
                SessionList.Add(session);
        }

        public void Remove(PlayerSession session)
        {
            SessionList.Remove(session);
        }

        public static PlayerEvent operator +(PlayerEvent pevent, PlayerSession session)
        {
            pevent.Add(session);

            return pevent;
        }

        public static PlayerEvent operator -(PlayerEvent pevent, PlayerSession session)
        {
            pevent.Remove(session);

            return pevent;
        }

        public void Invoke(Action<PlayerSession> invoke)
        {
            SessionList.ForEach(invoke);
        }
    }
}
