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
        public SpectateSession(PlayerSession host, Action<int> nuke)
        {
            Host = host;
            _nuke = nuke;
            Viewers = new ConcurrentBag<PlayerSession>();
        }

        public PlayerSession Host { get; }
        public ConcurrentBag<PlayerSession> Viewers { get; }
        private readonly Action<int> _nuke;

        public void Disconnect(PlayerSession session)
        {
            session.SpectateSession = null;

            if (!Viewers.Contains(session))
            {
                Log.Error("{Token} | Attempted to disconnect from spectate", session.Token);
                return;
            }

            if (!Viewers.TryTake(out _))
            {
                Log.Error("{Token} | Cannot remove from Viewers. Viewers: {@Viewers}", session.Token, Viewers);
                return;
            }

            var chan = GlobalChat.SpectateCount(Viewers.Count);
            var info = new ChannelInfo(chan);
            session.ChatKick(chan);
            session.Queue.EnqueuePacket(info);

            if (Viewers.IsEmpty)
            {
                _nuke(Host.Id);
                return;
            }

            var disconnectPacket = new FellowSpectatorLeft(session.Id);
            foreach (var viewer in Viewers)
            {
                viewer.Queue.EnqueuePacket(info);
                viewer.Queue.EnqueuePacket(disconnectPacket);
            }

            Host.Queue.EnqueuePacket(new SpectatorLeft(session.Id));
        }

        public void Connect(PlayerSession session)
        {
            session.SpectateSession = this;

            var connectPacket = new FellowSpectatorJoined(session.Id);
            var chan = GlobalChat.SpectateCount(Viewers.Count + 1);
            session.ChatConnected(chan);
            session.ChatInfo(chan);

            foreach (var viewer in Viewers)
            {
                viewer.ChatInfo(chan);
                viewer.Queue.EnqueuePacket(connectPacket);
                session.Queue.EnqueuePacket(new FellowSpectatorJoined(viewer.Id));
            }

            Viewers.Add(session);
            Host.Queue.EnqueuePacket(new SpectatorJoined(session.Id));
            Host.ChatInfo(chan);
        }

        public void Reconnect(PlayerSession session)
        {
            if (!Viewers.Contains(session))
            {
                session.SpectateSession = null;
                return;
            }

            Host.Queue.EnqueuePacket(new SpectatorJoined(session.Id));
            var reconnectPacket = new FellowSpectatorJoined(session.Id);
            foreach (var viewer in Viewers.Where(x => x != session))
            {
                viewer.Queue.EnqueuePacket(reconnectPacket);
            }
        }
    }
}
