using System.Collections.Generic;
using Cringe.Types.Common;

namespace Cringe.Bancho.Events.Multiplayer;

public record MatchUpdatedEvent(short MatchId, List<int> PlayerIds) : BaseEvent;
