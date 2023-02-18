using Cringe.Types.Enums;

namespace Cringe.Bancho.Bancho.ResponsePackets.Match;

public class NewMatch : ResponsePacket
{
    public NewMatch(Types.Match match)
    {
        Match = match;
    }

    public override ServerPacketType Type => ServerPacketType.NewMatch;
    public Types.Match Match { get; }

    public override byte[] GetBytes()
    {
        return PeppyConverter.Serialize(Match, true);
    }
}
