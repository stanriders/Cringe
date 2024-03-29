﻿using System;
using System.Threading.Tasks;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;

namespace Cringe.Bancho.Bancho.RequestPackets
{
    public class PingPacket : RequestPacket
    {
        public PingPacket(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override ClientPacketType Type => ClientPacketType.Ping;

        public override Task Execute(PlayerSession session, byte[] data)
        {
            return Task.CompletedTask;
        }
    }
}
