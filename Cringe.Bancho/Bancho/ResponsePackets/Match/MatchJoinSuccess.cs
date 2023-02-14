using System;
using System.Linq;
using Cringe.Types.Enums;

namespace Cringe.Bancho.Bancho.ResponsePackets.Match;

public class MatchJoinSuccess : ResponsePacket
{
    public MatchJoinSuccess(Types.Match match)
    {
        Match = match;
    }

    public Types.Match Match { get; }
    public override ServerPacketType Type => ServerPacketType.MatchJoinSuccess;

    public override byte[] GetBytes()
    {
        return PeppyConverter.Serialize(Match);
    }
}
