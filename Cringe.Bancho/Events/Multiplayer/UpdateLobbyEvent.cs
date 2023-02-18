using Cringe.Bancho.Types;
using Cringe.Types.Common;

namespace Cringe.Bancho.Events.Multiplayer;

public record UpdateLobbyEvent(Match Match) : BaseEvent;
