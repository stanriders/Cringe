using System.Collections.Generic;
using Cringe.Types.Common;

namespace Cringe.Bancho.Events.Multiplayer;

public record MatchHostTransferedEvent(List<int> Players) : BaseEvent;
