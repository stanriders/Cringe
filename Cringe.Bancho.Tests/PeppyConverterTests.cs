using Cringe.Bancho.Bancho;
using Cringe.Bancho.Bancho.RequestPackets;
using Cringe.Bancho.Bancho.RequestPackets.Spectate;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;
using Cringe.Types.Enums.Multiplayer;
using FluentAssertions;
using Xunit;

namespace Cringe.Bancho.Tests;

public class PeppyConverterTests
{
    [Theory]
    [InlineData("01 00 00 13 00 00 00 0b 00 0b 05 68 65 6c 6c 6f 0b 04 23 6f 73 75 00 00 00 00")]
    public void ValidPublicMessageBody_Should_DeserializeProperly(string bytes)
    {
        // Arrange
        var body = Helpers.FromByteString(bytes, true);

        // Act
        var message = PeppyConverter.Deserialize<SendPublicMessageRequest>(body);

        // Assert
        message.Text.Should().Be("hello");
        message.Receiver.Should().Be("#osu");
    }

    [Fact]
    public void ValidUserStatsRequest_Should_DeserializeProperly()
    {
        // Arrange
        var body = Helpers.FromByteString("55 00 00 06 00 00 00 02 00 01 00 00 00 10 00 00 00", true);

        // Act
        var message = PeppyConverter.Deserialize<UserStatsRequest>(body);

        // Assert
        message.Users.Should().BeEquivalentTo(new[] { 1, 16 });
    }

    [Fact]
    public void ValidMatchRequest_Should_DeserializeProperly()
    {
        // Arrange
        const string bytes =
            "1f 00 00 8e 00 00 00 ff ff 00 00 00 00 00 00 0b 03 4e 69 67 0b 05 31 32 33 34 35 0b 26 43 61 6d 65 6c 6c 69 61 20 2d 20 66 61 72 65 77 65 6c 6c 20 74 6f 20 74 6f 64 61 79 20 5b 63 68 61 6e 67 65 73 5d bd 98 12 00 0b 20 38 37 34 39 35 31 35 30 38 39 30 30 61 64 38 62 34 30 32 66 33 39 33 32 39 37 64 31 65 36 38 30 01 01 01 01 01 01 01 01 02 02 02 02 02 02 02 02 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 00 00 00 00 00 00 00 00";
        var body = Helpers.FromByteString(bytes, true);

        // Act
        var match = PeppyConverter.Deserialize<Match>(body);
        var serialized = PeppyConverter.Serialize(match);

        // Assert
        match.RoomName.Should().Be("Nig");
        match.Password.Should().Be("12345");
        match.Slots.Should().HaveCount(16)
            .And
            .AllSatisfy(x =>
            {
                x.Team.Should().Be(MatchTeams.Neutral);
                x.Mods.Should().Be(Mods.None);
            });
        serialized.Should().BeEquivalentTo(body);
    }

    [Fact]
    public void RawSpectateFrames_Should_PreserveBytes()
    {
        const string bytes = "00 01 02 03 04 05 11 12 13 14 15";
        var body = Helpers.FromByteString(bytes, false);

        var spectatorFrame = PeppyConverter.Deserialize<SpectateFrameRequest>(body);

        spectatorFrame.Payload.Should().BeEquivalentTo(body);
    }
}
