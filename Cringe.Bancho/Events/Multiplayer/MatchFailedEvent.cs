using MediatR;

namespace Cringe.Bancho.Events.Multiplayer;

public record MatchFailedEvent(short MatchId, int SlotId) : INotification;
