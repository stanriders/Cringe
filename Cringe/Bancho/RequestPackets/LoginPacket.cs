using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cringe.Bancho.ResponsePackets;
using Cringe.Types;
using Cringe.Types.Enums;

namespace Cringe.Bancho.RequestPackets
{
    public class LoginPacket : RequestPacket
    {
        private const uint protocol_version = 19;

        public LoginPacket(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override ClientPacketType Type => ClientPacketType.Login;

        public override async Task Execute(UserToken token, byte[] data)
        {
            var pool = Pool;
            var config = Configuration;
            var player = await Token.GetPlayerWithoutScores(token.PlayerId);
            var queue = pool.GetFromPool(token.PlayerId);
            queue.EnqueuePacket(new ProtocolVersion(protocol_version));
            queue.EnqueuePacket(new UserId(token.PlayerId));
            queue.EnqueuePacket(new UserPresenceBundle(pool.Apply(x => x)));
            queue.EnqueuePacket(new Supporter(player.UserRank));
            if (!string.IsNullOrEmpty(config["LoginMessage"]))
                queue.EnqueuePacket(new Notification(config["LoginMessage"]));
            queue.EnqueuePacket(new SilenceEnd(0));

            queue.EnqueuePacket(new UserPresence(player.Presence));
            queue.EnqueuePacket(new UserStats(player.Stats));

            queue.EnqueuePacket(new ChannelInfoEnd());

            queue.EnqueuePacket(new FriendsList(new[] {2}));

            if (!string.IsNullOrEmpty(config["MainMenuBanner"]))
                queue.EnqueuePacket(new MainMenuIcon(config["MainMenuBanner"]));

            queue.EnqueuePacket(new UserPresence(player.Presence));
            var servers = new List<string>
            {
                "#osu",
                "#announce",
                "#russian"
            };
            if (player.UserRank.HasFlag(UserRanks.Admin) || player.UserRank.HasFlag(UserRanks.Peppy))
                servers.Add("#vacman");
            var chats = Chats;
            servers.ForEach(x => chats.AutoJoinOrPackInfo(token.PlayerId, x));
        }
    }
}