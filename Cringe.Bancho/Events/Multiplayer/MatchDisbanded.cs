using Cringe.Bancho.Types;
using Cringe.Types.Common;

namespace Cringe.Bancho.Events.Multiplayer;

public record MatchDisbandedEvent(short MatchId) : BaseEvent;
