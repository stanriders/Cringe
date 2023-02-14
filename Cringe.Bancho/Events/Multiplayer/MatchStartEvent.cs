using System.Collections.Generic;
using Cringe.Types.Common;

namespace Cringe.Bancho.Events.Multiplayer;

public record MatchStartEvent(List<int> Players) : BaseEvent;
