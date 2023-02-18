using Cringe.Types.Common;

namespace Cringe.Bancho.Events.Lobby;

public record LobbyPlayerJoinedEvent(int PlayerId) : BaseEvent;
