using System.Threading;
using System.Threading.Tasks;
using Cringe.Types.Enums;
using MediatR;

namespace Cringe.Bancho.Bancho.RequestPackets;

public class PingPacketRequest : RequestPacket, IRequest
{
    public override ClientPacketType Type => ClientPacketType.Ping;
}

public class PingPacket : IRequestHandler<PingPacketRequest>
{
    public Task<Unit> Handle(PingPacketRequest request, CancellationToken cancellationToken)
    {
        return Unit.Task;
    }
}
