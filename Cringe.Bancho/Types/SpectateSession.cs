using System;
using System.Collections.Concurrent;
using System.Linq;
using Cringe.Bancho.Bancho.ResponsePackets;
using Cringe.Bancho.Bancho.ResponsePackets.Spectate;
using Serilog;

namespace Cringe.Bancho.Types
{
    public class SpectateSession
    {
        private readonly Action<int> _nuke;

        public SpectateSession(PlayerSession host, Action<int> nuke)
        {
            Host = host;
            _nuke = nuke;
            Viewers = new ConcurrentDictionary<int, PlayerSession>();
        }

        public PlayerSession Host { get; }
        public ConcurrentDictionary<int, PlayerSession> Viewers { get; }

        public void Disconnect(PlayerSession session)
        {
            session.SpectateSession = null;

            if (!Viewers.ContainsKey(session.Id))
            {
                Log.Error("{Token} | Attempted to disconnect from spectate", session.Token);

                return;
            }

            if (!Viewers.TryRemove(session.Id, out _))
            {
                Log.Error("{Token} | Cannot remove from Viewers. Viewers: {@Viewers}", session.Token, Viewers);

                return;
            }

            var chan = GlobalChat.SpectateCount(Viewers.Count);
            var info = new ChannelInfo(chan);
            session.ChatKick(chan);
            session.Queue.EnqueuePacket(info);

            Host.Queue.EnqueuePacket(new SpectatorLeft(session.Id));

            if (Viewers.IsEmpty)
            {
                _nuke(Host.Id);

                return;
            }

            var disconnectPacket = new FellowSpectatorLeft(session.Id);
            foreach (var viewer in Viewers.Values)
            {
                viewer.Queue.EnqueuePacket(info);
                viewer.Queue.EnqueuePacket(disconnectPacket);
            }
        }

        public void Connect(PlayerSession session)
        {
            session.SpectateSession = this;

            var connectPacket = new FellowSpectatorJoined(session.Id);
            var chan = GlobalChat.SpectateCount(Viewers.Count + 1);
            session.ChatConnected(chan);
            session.ChatInfo(chan);

            foreach (var viewer in Viewers.Values)
            {
                viewer.ChatInfo(chan);
                viewer.Queue.EnqueuePacket(connectPacket);
                session.Queue.EnqueuePacket(new FellowSpectatorJoined(viewer.Id));
            }

            Viewers.TryAdd(session.Id, session);
            Host.Queue.EnqueuePacket(new SpectatorJoined(session.Id));
            Host.ChatInfo(chan);
        }

        public void Reconnect(PlayerSession session)
        {
            if (!Viewers.ContainsKey(session.Id))
            {
                session.SpectateSession = null;

                return;
            }

            Host.Queue.EnqueuePacket(new SpectatorJoined(session.Id));
            var reconnectPacket = new FellowSpectatorJoined(session.Id);
            foreach (var viewer in Viewers.Values.Where(x => x != session))
                viewer.Queue.EnqueuePacket(reconnectPacket);
        }
    }
}
