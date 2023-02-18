using Cringe.Types.Common;

namespace Cringe.Bancho.Events.Lobby;

public record LobbyPlayerLeftEvent(int PlayerId) : BaseEvent;
