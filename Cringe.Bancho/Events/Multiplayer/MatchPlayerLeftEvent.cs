using Cringe.Types.Common;

namespace Cringe.Bancho.Events.Multiplayer;

public record MatchPlayerLeftEvent(int PlayerId, short MatchId) : BaseEvent;
