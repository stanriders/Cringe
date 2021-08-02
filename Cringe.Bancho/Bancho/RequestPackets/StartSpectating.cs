using System;
using System.IO;
using System.Threading.Tasks;
using Cringe.Bancho.Services;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;
using Microsoft.Extensions.Logging;

namespace Cringe.Bancho.Bancho.RequestPackets
{
    public class StartSpectating : RequestPacket
    {
        public StartSpectating(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override ClientPacketType Type => ClientPacketType.StartSpectating;

        public override Task Execute(PlayerSession session, byte[] data)
        {
            using var reader = new BinaryReader(new MemoryStream(data));
            var id = reader.ReadInt32();

            if (session.SpectateSession is not null)
            {
                if (session.SpectateSession.Host.Id == id)
                {
                    session.SpectateSession.Reconnect(session);
                    return Task.CompletedTask;
                }

                session.SpectateSession.Disconnect(session);
            }

            var host = PlayersPool.GetPlayer(id);
            if (host is null)
            {
                Logger.LogError("{Token} | Attempted to spectate offline of non-existing player {Id}", session.Token, id);
                return Task.CompletedTask;
            }

            Spectate.StartSpectating(host, session);
            return Task.CompletedTask;
        }
    }
}
