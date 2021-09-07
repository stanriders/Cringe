using System;

namespace Cringe.Bancho.Bancho.RequestPackets.Match
{
    public abstract class MatchChanged : RequestPacket
    {
        protected MatchChanged(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        protected override string ApiPath => "match/changed";
    }
}
