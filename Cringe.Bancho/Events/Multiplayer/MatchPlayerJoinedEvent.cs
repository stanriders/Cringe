using Cringe.Types.Common;

namespace Cringe.Bancho.Events.Multiplayer;

public record MatchPlayerJoinedEvent(int PlayerId, short MatchId) : BaseEvent;
