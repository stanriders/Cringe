using MediatR;

namespace Cringe.Bancho.Events;

public record PlayerLeftEvent(int PlayerId) : INotification;
