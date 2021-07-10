using System.Collections.Generic;
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
    }
}