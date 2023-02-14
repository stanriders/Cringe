using System.Collections.Generic;
using Cringe.Types.Common;

namespace Cringe.Bancho.Events.Multiplayer;

public record MatchSkippedEvent(List<int> Players) : BaseEvent;
