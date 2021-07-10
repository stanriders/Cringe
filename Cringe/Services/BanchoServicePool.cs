using System;
using System.Collections.Generic;
using System.Linq;
using Cringe.Types;

namespace Cringe.Services
{
    public class BanchoServicePool
    {
        private readonly Dictionary<int, PacketQueue> _pool;

        public BanchoServicePool()
        {
            _pool = new Dictionary<int, PacketQueue>();
        }

        public PacketQueue GetFromPool(int user)
        {
            if (_pool.TryGetValue(user, out var value)) return value;

            var service = new PacketQueue();
            _pool.Add(user, service);
            return service;
        }

        public void ActionMap(Action<PacketQueue> action)
        {
            foreach (var packet in _pool)
                action(packet.Value);
        }

        public void ActionMapFilter(Action<PacketQueue> action, Func<int, bool> predicate)
        {
            foreach (var packet in _pool.Where(x => !predicate(x.Key)))
                action(packet.Value);
        }

        public void ActionOn(int id, Action<PacketQueue> action)
        {
            var success = _pool.TryGetValue(id, out var res);
            if (!success)
                return;
            action(res);
        }
    }
}