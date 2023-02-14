using System.Threading;
using System.Threading.Tasks;
using Cringe.Bancho.Bancho.ResponsePackets;
using Cringe.Bancho.Services;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;
using MediatR;

namespace Cringe.Bancho.Bancho.RequestPackets;

public class StatusUpdateRequest : RequestPacket, IRequest
{
    public override ClientPacketType Type => ClientPacketType.RequestStatusUpdate;
}

public class StatusUpdateRequestHandler : IRequestHandler<StatusUpdateRequest>
{
    private readonly PlayerSession _session;

    public StatusUpdateRequestHandler(CurrentPlayerProvider currentPlayerProvider)
    {
        _session = currentPlayerProvider.Session;
    }

    public Task<Unit> Handle(StatusUpdateRequest request, CancellationToken cancellationToken)
    {
        _session.Queue.EnqueuePacket(new UserStats(_session.Stats));

        return Unit.Task;
    }
}
