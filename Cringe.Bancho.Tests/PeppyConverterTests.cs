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

    [Theory]
    [InlineData("Nig", "12345",
        "1f 00 00 8e 00 00 00 ff ff 00 00 00 00 00 00 0b 03 4e 69 67 0b 05 31 32 33 34 35 0b 26 43 61 6d 65 6c 6c 69 61 20 2d 20 66 61 72 65 77 65 6c 6c 20 74 6f 20 74 6f 64 61 79 20 5b 63 68 61 6e 67 65 73 5d bd 98 12 00 0b 20 38 37 34 39 35 31 35 30 38 39 30 30 61 64 38 62 34 30 32 66 33 39 33 32 39 37 64 31 65 36 38 30 01 01 01 01 01 01 01 01 02 02 02 02 02 02 02 02 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 00 00 00 00 00 00 00 00")]
    [InlineData("Roxy's game", "123123",
        "1f 00 00 96 00 00 00 ff ff 00 00 00 00 00 00 0b 0b 52 6f 78 79 27 73 20 67 61 6d 65 0b 06 31 32 33 31 32 33 0b 25 55 53 41 4f 20 2d 20 4e 69 67 68 74 20 73 6b 79 20 5b 56 6f 72 74 65 78 27 73 20 49 6e 6e 65 72 20 4f 6e 69 5d 1f 93 0c 00 0b 20 32 36 66 39 64 38 63 63 62 38 62 66 39 38 61 66 65 62 61 62 37 39 38 36 35 63 36 37 66 65 63 33 01 01 01 01 01 01 01 01 02 02 02 02 02 02 02 02 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 00 00 00 00 00 00 00 00")]
    public void ValidMatchRequest_Should_DeserializeProperly(string roomName, string password, string bytes)
    {
        // Arrange
        var body = Helpers.FromByteString(bytes, true);

        // Act
        var match = PeppyConverter.Deserialize<Match>(body);
        var serialized = PeppyConverter.Serialize(match);

        // Assert
        match.RoomName.Should().Be(roomName);
        match.Password.Should().Be(password);
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
