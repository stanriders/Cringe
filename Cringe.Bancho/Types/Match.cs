using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Cringe.Bancho.Bancho;
using Cringe.Types.Enums;
using Cringe.Types.Enums.Multiplayer;

namespace Cringe.Bancho.Types;

public class Map
{
    [PeppyField]
    public string MapName { get; set; } = null!;

    [PeppyField]
    public int MapId { get; set; }

    [PeppyField]
    public string MapHash { get; set; } = null!;
}

public class Match : IDependant
{
    [PeppyField]
    public short Id { get; set; }

    [PeppyField]
    public bool InProgress { get; set; }

    [PeppyField]
    private byte _padding { get; set; }

    [PeppyField]
    [EnumType(typeof(int))]
    public Mods Mods { get; set; }

    [PeppyField]
    public string RoomName { get; set; } = null!;

    [PeppyField]
    [Secret]
    public string Password { get; set; }

    private List<Slot> _slots;

    public List<Slot> Slots
    {
        get
        {
            if (_slots is not null) return _slots;

            _slots = new List<Slot>();
            for (var i = 0; i < 16; i++)
            {
                _slots.Add(new Slot
                {
                    Status = (SlotStatus) _slotStatus[i],
                    Team = (MatchTeams) _slotTeams[i],
                    Mods = _slotMods.Length == 0 ? Mods.None : (Mods) _slotMods[i]
                });
            }

            return _slots;
        }
    }

    [PeppyField]
    public Map MapInfo { get; set; } = null!;

    [PeppyField]
    [ConstantSized(16)]
    private byte[] _slotStatus { get; } = Array.Empty<byte>();

    [PeppyField]
    [ConstantSized(16)]
    private byte[] _slotTeams { get; } = Array.Empty<byte>();

    [PeppyField]
    [DependentSized]
    private int[] _playerIds { get; set; } = Array.Empty<int>();

    [PeppyField]
    public int HostId { get; set; }

    [PeppyField]
    [EnumType(typeof(byte))]
    public GameModes Modes { get; set; }

    [PeppyField]
    [EnumType(typeof(byte))]
    public MatchWinConditions WinConditions { get; set; }

    [PeppyField]
    [EnumType(typeof(byte))]
    public MatchTeamTypes TeamType { get; set; }

    [PeppyField]
    private byte _freeModeByte { get; set; }

    public bool IsFreeMode
    {
        get => _freeModeByte == 1;
        set => _freeModeByte = value ? (byte) 1 : (byte) 0;
    }

    [PeppyField]
    [DependentSized]
    private byte[] _slotMods { get; set; } = null!;

    [PeppyField]
    public int Seed { get; set; }

    private int _skipCounter = 0;

    public int? Dependency(string propertyName)
    {
        return propertyName switch
        {
            nameof(_playerIds) => _slotStatus.Count(SlotIsNotEmpty),
            nameof(_slotMods) => IsFreeMode ? 16 : 0,
            _ => null
        };
    }

    #region Behaviour
    private void AssertHost(int playerId)
    {
        if (playerId != HostId)
        {
            throw new Exception("Forbidden action");
        }
    }

    public bool PlayerIsInTheMatch(int playerId)
    {
        return _playerIds.Contains(playerId);
    }

    public int PlayerPosition(int playerId)
    {
        for (var i = 0; i < _playerIds.Length; i++)
        {
            if (_playerIds[i] == playerId) return i;
        }

        throw new Exception($"Player {playerId} is not in the match");
    }

    private int PlayerSlotId(int playerId)
    {
        int? index = null;
        for (var i = 0; i < _playerIds.Length; i++)
        {
            if (_playerIds[i] != playerId) continue;

            index = i;

            break;
        }

        if (index is null)
        {
            throw new ArgumentOutOfRangeException(nameof(playerId), "Undefined player outside of the slots");
        }

        var skip = 0;
        for (var i = 0; i < _slotStatus.Length; i++)
        {
            if (SlotIsNotEmpty(_slotStatus[i])) continue;

            if (skip == index)
            {
                return i;
            }

            skip++;
        }

        throw new ArgumentOutOfRangeException(nameof(playerId), "XD");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool SlotIsNotEmpty(byte b) => (b & 124) != 0;

    private int FirstEmptySlot()
    {
        for (var i = 0; i < _slotStatus.Length; i++)
        {
            if (!SlotIsNotEmpty(_slotStatus[i])) return i;
        }

        throw new Exception("No empty slot was found");
    }

    // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Global
    public void AddPlayer(int playerId, string password)
    {
        InvalidateSlots();

        if (Password != "" && Password != password)
        {
            throw new Exception("Incorrect password");
        }

        if (_slotStatus.All(SlotIsNotEmpty))
        {
            throw new Exception("Lobby is full");
        }

        var firstAvailableSlot = FirstEmptySlot();
        _playerIds = _playerIds.Append(playerId).ToArray();
        _slotStatus[firstAvailableSlot] = (byte) SlotStatus.NotReady;
        if (TeamType is MatchTeamTypes.TeamVs or MatchTeamTypes.TagTeamVs)
        {
            _slotTeams[firstAvailableSlot] = (byte) MatchTeams.Red;
        }
    }

    private void RemovePlayerOnSlot(int slotId, int playerId)
    {
        _playerIds = _playerIds.Where(x => x != playerId).ToArray();
        _slotStatus[slotId] = (byte) SlotStatus.Open;
        _slotTeams[slotId] = (byte) MatchTeams.Neutral;
        _slotMods[slotId] = (byte) Mods.None;

        if (playerId != HostId) return;

        for (var i = 0; i < _slotStatus.Length; i++)
        {
            if (SlotIsNotEmpty(_slotStatus[i]))
            {
                TransferHost(i);
            }
        }
    }

    public void RemovePlayer(int playerId)
    {
        InvalidateSlots();

        var slotId = PlayerSlotId(playerId);
        RemovePlayerOnSlot(slotId, playerId);
    }

    private void InvalidateSlots()
    {
        _slots = null;
    }

    public void SetMods(int playerId, Mods mods)
    {
        InvalidateSlots();

        if (IsFreeMode)
        {
            if (HostId == playerId)
                Mods = mods & Mods.SpeedChangingMods;

            var playerSlot = PlayerSlotId(playerId);
            _slotMods[playerSlot] = (byte) (mods & ~Mods.SpeedChangingMods);

            return;
        }

        AssertHost(playerId);

        Mods = mods;
    }

    public void SetPassword(int playerId, string newPassword)
    {
        AssertHost(playerId);
        Password = newPassword;
    }

    public void ChangeSlot(int playerId, int slotId)
    {
        var slotStatus = (SlotStatus) _slotStatus[slotId];
        if (slotStatus != SlotStatus.Open)
        {
            throw new Exception("Слот закрыт нах");
        }

        var slots = Slots;

        var playerSlotId = PlayerSlotId(playerId);
        var playerSlot = slots[playerSlotId];
        slots[playerSlotId] = new Slot
        {
            PlayerId = null,
            Status = SlotStatus.Open,
            Team = MatchTeams.Neutral,
            Mods = Mods.None
        };
        slots[slotId] = playerSlot;
        for (var i = 0; i < slots.Count; i++)
        {
            _slotMods[i] = (byte) slots[i].Mods;
            _slotStatus[i] = (byte) slots[i].Status;
            _slotTeams[i] = (byte) slots[i].Team;
        }

        _playerIds = slots.Where(x => x.PlayerId is not null).Select(x => x.PlayerId.Value).ToArray();
    }

    public void ChangeSettings(int playerId, Match newMatch)
    {
        AssertHost(playerId);
        InvalidateSlots();

        if (newMatch.IsFreeMode != IsFreeMode)
        {
            IsFreeMode = newMatch.IsFreeMode;

            //If we converted from non-freemode to freemode
            if (newMatch.IsFreeMode)
            {
                for (var i = 0; i < _slotMods.Length; i++)
                {
                    if (!SlotIsNotEmpty(_slotStatus[i])) continue;

                    _slotMods[i] = (byte) (Mods & ~Mods.SpeedChangingMods);
                }

                Mods &= Mods.SpeedChangingMods;
            }
            else
            {
                var hostSlotId = PlayerSlotId(HostId);
                Mods &= Mods.SpeedChangingMods;
                Mods |= (Mods) _slotMods[hostSlotId];
                for (var i = 0; i < _slotMods.Length; i++)
                {
                    _slotMods[i] = 0;
                }
            }
        }

        if (newMatch.MapInfo.MapId != MapInfo.MapId)
        {
            if (newMatch.MapInfo.MapId == -1)
            {
                for (var i = 0; i < _slotStatus.Length; i++)
                {
                    if (!SlotIsNotEmpty(_slotStatus[i])) continue;

                    _slotStatus[i] = (byte) SlotStatus.NotReady;
                }
            }

            MapInfo = newMatch.MapInfo;
        }

        if (TeamType != newMatch.TeamType)
        {
            var team = newMatch.TeamType is MatchTeamTypes.HeadToHead or MatchTeamTypes.TagCoop
                ? MatchTeams.Neutral
                : MatchTeams.Red;

            for (var i = 0; i < _slotTeams.Length; i++)
            {
                if (!SlotIsNotEmpty(_slotStatus[i])) continue;

                _slotTeams[i] = (byte) team;
            }

            TeamType = newMatch.TeamType;
        }

        WinConditions = newMatch.WinConditions;
        RoomName = newMatch.RoomName;
    }

    public void ChangeTeam(int playerId)
    {
        if (TeamType is not (MatchTeamTypes.TeamVs or MatchTeamTypes.TagTeamVs))
        {
            throw new Exception("Match is not in team mode to change team");
        }

        InvalidateSlots();

        var slotId = PlayerSlotId(playerId);
        var prevTeam = (MatchTeams) _slotTeams[slotId];
        _slotTeams[slotId] = (byte) (prevTeam switch
        {
            MatchTeams.Red => MatchTeams.Blue,
            _ => MatchTeams.Blue
        });
    }

    public void Complete(int playerId)
    {
        _skipCounter = 0;
        var slotId = PlayerSlotId(playerId);
        _slotStatus[slotId] = (byte) SlotStatus.Complete;

        if (_slotStatus.Any(x => x == 32)) return;

        //If all players completed
        for (var i = 0; i < _slotStatus.Length; i++)
        {
            if (_slotStatus[i] != (byte) SlotStatus.Complete) continue;

            _slotStatus[i] = (byte) SlotStatus.NotReady;
        }

        //TODO: add events
        //player.Player.Queue.EnqueuePacket(new ResponsePackets.Match.MatchComplete());
    }

    private void ChangeStatus(int playerId, SlotStatus status)
    {
        var slotId = PlayerSlotId(playerId);
        _slotStatus[slotId] = (byte) status;
    }

    public void HasBeatmap(int playerId)
    {
        ChangeStatus(playerId, SlotStatus.NotReady);
    }

    public void NoBeatmap(int playerId)
    {
        ChangeStatus(playerId, SlotStatus.NoMap);
    }

    public void NotReady(int playerId)
    {
        ChangeStatus(playerId, SlotStatus.NotReady);
    }

    public void Ready(int playerId)
    {
        ChangeStatus(playerId, SlotStatus.Ready);
    }

    private int PlayerIdOnSlot(int slotId)
    {
        var playerIndex = 0;
        for (var i = 0; i < slotId; i++)
        {
            if (!SlotIsNotEmpty(_slotStatus[slotId])) continue;

            playerIndex++;
        }

        return playerIndex;
    }

    public void Start(int playerId)
    {
        AssertHost(playerId);

        InProgress = true;
        var playingPlayers = new List<int>();
        for (var i = 0; i < _slotStatus.Length; i++)
        {
            if (_slotStatus[i] != (byte) SlotStatus.Ready && _slotStatus[i] != (byte) SlotStatus.NotReady) continue;

            _slotStatus[i] = (byte) SlotStatus.Loading;
            playingPlayers.Add(PlayerIdOnSlot(i));
        }

        //TODO: domain event on playingPlayers MatchStart
    }

    public void LoadComplete(int playerId)
    {
        var playerSlot = PlayerSlotId(playerId);
        _slotStatus[playerSlot] = (byte) SlotStatus.Playing;
        if (_slotStatus.Any(x => x == (byte) SlotStatus.Loading))
        {
            return;
        }

        //TODO: MatchLoadComplete
        //player.Player.Queue.EnqueuePacket(new ResponsePackets.Match.MatchComplete());
    }

    public void LockSlot(int playerId, int slotId)
    {
        AssertHost(playerId);
        if (_slotStatus[slotId] == (byte) SlotStatus.Locked)
        {
            _slotStatus[slotId] = (byte) SlotStatus.Open;

            return;
        }

        if (!SlotIsNotEmpty(_slotStatus[slotId]))
        {
            _slotStatus[slotId] = (byte) SlotStatus.Locked;
            _slotTeams[slotId] = 0;
            _slotMods[slotId] = 0;

            return;
        }

        var playerIndex = PlayerIdOnSlot(slotId);
        var removedPlayerId = _playerIds[playerIndex];
        if (removedPlayerId == playerId)
        {
            throw new Exception("Unable to remove yourself with a lock xd");
        }

        RemovePlayerOnSlot(slotId, removedPlayerId);
    }

    public void Skip(int playerId)
    {
        _skipCounter++;
        //TODO: event PlayerSkipped

        var playingCount = _slotStatus.Count(x => x == (byte) SlotStatus.Playing);
        if (_skipCounter == playingCount)
        {
            //TODO: event Skip
        }
    }

    private void TransferHost(int slotId)
    {
        if (!SlotIsNotEmpty(_slotStatus[slotId]))
        {
            throw new Exception("No player on that slot");
        }

        HostId = PlayerIdOnSlot(slotId);

        //TODO: event next host id
    }

    public void TransferHost(int playerId, int nextHostSlotId)
    {
        AssertHost(playerId);
        TransferHost(nextHostSlotId);
    }
    #endregion
}
