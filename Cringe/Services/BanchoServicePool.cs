using System;
using System.Collections.Generic;
using System.Linq;
using Cringe.Types;
using Microsoft.Extensions.Logging;

namespace Cringe.Services
{
    public class BanchoServicePool
    {
        private readonly ILogger<BanchoServicePool> _logger;
        private readonly Dictionary<int, PacketQueue> _pool;

        public BanchoServicePool(ILogger<BanchoServicePool> logger)
        {
            _logger = logger;
            _pool = new Dictionary<int, PacketQueue>();
        }

        public PacketQueue GetFromPool(int user)
        {
            if (_pool.TryGetValue(user, out var value)) return value;

            var service = new PacketQueue();
            _pool.Add(user, service);
            return service;
        }

        public IEnumerable<T> Apply<T>(Func<int, T> func)
        {
            return _pool.Select(x => func(x.Key));
        }

        public void ActionMap(Action<PacketQueue> action)
        {
            foreach (var packet in _pool)
                action(packet.Value);
        }

        public void ActionMapFilter(Action<PacketQueue> action, Func<int, bool> predicate)
        {
            foreach (var packet in _pool.Where(x => predicate(x.Key)))
                action(packet.Value);
        }

        public void ActionOn(int id, Action<PacketQueue> action)
        {
            var success = _pool.TryGetValue(id, out var res);
            if (!success)
                return;
            action(res);
        }

        public void Nuke(int tokenPlayerId)
        {
            if (!_pool.ContainsKey(tokenPlayerId)) return;
            _logger.LogInformation($"{tokenPlayerId} logged out. Nuking the queue");
            _pool.Remove(tokenPlayerId);
        }
    }
}