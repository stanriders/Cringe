using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cringe.Bancho.Bancho.ResponsePackets;
using Cringe.Bancho.Events.Lobby;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Cringe.Bancho.Services;

public class ChatService
{
    public const string LobbyName = "#lobby";

    private static readonly IReadOnlyList<GlobalChat> _globalChats = new List<GlobalChat>
    {
        new("#osu", "THIS GAME SUCKS QOQ", true),
        new("#announce", "Shitpost your 300pp here", true),
        new("#vacman", "Admin only secret chat", false, UserRanks.Admin),
        new("#russian", "KRYM NASH!!! :DDD"),
        new(LobbyName, "LOBESHNIQ")
    }.AsReadOnly();

    private readonly ILogger<ChatService> _logger;

    private readonly Action<Message> _sendPrivateMessage;

    public ChatService(ILogger<ChatService> logger)
    {
        _logger = logger;
        _sendPrivateMessage = message => PlayersPool.GetPlayer(message.Receiver)?.ReceiveMessage(message);
    }

    public void Initialize(PlayerSession player)
    {
        var rank = player.Player.UserRank;
        foreach (var globalChat in _globalChats.Where(globalChat => IsAllowed(rank, globalChat.Accessibility)))
        {
            if (globalChat.Accessibility != UserRanks.Normal)
                _logger.LogInformation("{Token} | Connected to a private chat {Name}", player.Token,
                    globalChat.Name);
            globalChat.ReceiveUpdates += player;
            globalChat.OnStatusUpdated();
            if (globalChat.AutoConnect)
                globalChat.Connect(player);
        }
    }

    public static GlobalChat GetChat(string name)
    {
        return _globalChats.FirstOrDefault(x => x.Name == name);
    }

    public static void Purge(PlayerSession player)
    {
        var rank = player.Player.UserRank;
        foreach (var globalChat in _globalChats.Where(globalChat => IsAllowed(rank, globalChat.Accessibility)))
            globalChat.Disconnect(player);
    }

    public static bool SendGlobalMessage(Message message)
    {
        var chat = _globalChats.FirstOrDefault(x => x.Name == message.Receiver);

        if (chat is null) return false;
        if (!IsAllowed(message.Sender.UserRank, chat.Accessibility)) return false;

        chat.OnSendMessage(message);

        return true;
    }

    public void SendPrivateMessage(Message message)
    {
        _sendPrivateMessage(message);
    }

    /// <summary>
    ///     If player is not allowed to even see this channel 0_0
    /// </summary>
    /// <param name="userRanks">The rank of user</param>
    /// <param name="chatRanks">The accessibility level of chat</param>
    /// <returns></returns>
    private static bool IsAllowed(UserRanks userRanks, UserRanks chatRanks)
    {
        return chatRanks == UserRanks.Normal || (userRanks & chatRanks) != 0;
    }

    public class ConnectPlayerToLobbyChatHandler : INotificationHandler<LobbyPlayerJoinedEvent>
    {
        public async Task Handle(LobbyPlayerJoinedEvent notification, CancellationToken cancellationToken)
        {

        }
    }
}
