using System;
using System.Threading.Tasks;
using Cringe.Bancho.Services;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;
using Microsoft.Extensions.Logging;

namespace Cringe.Bancho.Bancho.RequestPackets
{
    public class FriendAdd : RequestPacket
    {
        private readonly FriendsService _friendsService;

        public FriendAdd(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _friendsService = (FriendsService) serviceProvider.GetService(typeof(FriendsService));
        }

        public override ClientPacketType Type => ClientPacketType.FriendAdd;
        protected override string ApiPath => "user/friends/add";

        public override Task Execute(PlayerSession session, byte[] data)
        {
            var friendId = ReadInt(data);

            Logger.LogInformation("{Token} | Adding friend {Friend}...", session.Token, friendId);

            return _friendsService.AddFriend(session.Player.Id, friendId);
        }
    }
}
