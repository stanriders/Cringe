using System;
using System.Threading.Tasks;
using Cringe.Bancho.Services;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;
using Microsoft.Extensions.Logging;

namespace Cringe.Bancho.Bancho.RequestPackets
{
    public class FriendRemove : RequestPacket
    {
        private readonly FriendsService _friendsService;

        public FriendRemove(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _friendsService = (FriendsService) serviceProvider.GetService(typeof(FriendsService));
        }

        public override ClientPacketType Type => ClientPacketType.FriendRemove;
        protected override string ApiPath => "user/friends/remove";

        public override Task Execute(PlayerSession session, byte[] data)
        {
            var friendId = ReadInt(data);

            Logger.LogInformation("{Token} | Removing friend {Friend}...", session.Token, friendId);

            return _friendsService.RemoveFriend(session.Player.Id, friendId);
        }
    }
}
