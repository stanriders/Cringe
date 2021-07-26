﻿using System;
using System.IO;
using System.Threading.Tasks;
using Cringe.Bancho.Services;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;
using Microsoft.Extensions.Logging;

namespace Cringe.Bancho.Bancho.RequestPackets
{
    public class ChannelPart : RequestPacket
    {
        public ChannelPart(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override ClientPacketType Type => ClientPacketType.ChannelPart;

        public override Task Execute(PlayerSession session, byte[] data)
        {
            using var stream = new MemoryStream(data);
            var chat = ReadString(stream);
            Logger.LogDebug("{Token} | Leaves the {Chat} chat", session.Token, chat);
            ChatService.GetChat(chat)?.Disconnect(session);

            return Task.CompletedTask;
        }
    }
}
