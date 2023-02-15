using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Cringe.Bancho.Bancho;
using Cringe.Bancho.Events.Multiplayer;
using Cringe.Types.Common;
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

public class Match : BaseEntity, IDependant
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

    public List<Slot> Slots
    {
        get
        {
            var slots = new List<Slot>();

            for (var i = 0; i < 16; i++)
            {
                slots.Add(new Slot
                {
                    Status = (SlotStatus) _slotStatus[i],
                    Team = (MatchTeams) _slotTeams[i],
                    Mods = _slotMods.Length == 0 ? Mods.None : (Mods) _slotMods[i]
                });
            }

            return slots;
        }
    }

    [PeppyField]
    public Map MapInfo { get; set; } = null!;

    [PeppyField]
    [ConstantSized(16)]
    private byte[] _slotStatus { get; set; } = Array.Empty<byte>();

    [PeppyField]
    [ConstantSized(16)]
    private byte[] _slotTeams { get; set; } = Array.Empty<byte>();

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
    public bool FreeMode { get; private set; }


    [PeppyField]
    [DependentSized]
    private byte[] _slotMods { get; set; } = null!;

    [PeppyField]
    public int Seed { get; set; }

    private int _skipCounter = 0;
    private int _loadingCounter = 0;

    public int? Dependency(string propertyName)
    {
        return propertyName switch
        {
            nameof(_playerIds) => _slotStatus.Count(SlotIsNotEmpty),
            nameof(_slotMods) => FreeMode ? 16 : 0,
            _ => null
        };
    }

    public List<int> Players => _playerIds.ToList();

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

    private void AddMatchUpdatedEvent()
    {
        AddEvent(new MatchUpdatedEvent(this));
    }

    #region Behaviour
    // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Global
    public void AddPlayer(int playerId, string password)
    {
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

        AddMatchUpdatedEvent();
    }

    private void RemovePlayerOnSlot(int slotId, int playerId)
    {
        _playerIds = _playerIds.Where(x => x != playerId).ToArray();
        _slotStatus[slotId] = (byte) SlotStatus.Open;
        _slotTeams[slotId] = (byte) MatchTeams.Neutral;
        _slotMods[slotId] = (byte) Mods.None;


        if (playerId != HostId) return;

        if (_playerIds.Length == 0)
        {
            AddEvent(new MatchDisbandedEvent(Id));

            return;
        }

        for (var i = 0; i < _slotStatus.Length; i++)
        {
            if (SlotIsNotEmpty(_slotStatus[i]))
            {
                TransferHost(i);
            }
        }

        AddMatchUpdatedEvent();
    }

    public void RemovePlayer(int playerId)
    {
        var slotId = PlayerSlotId(playerId);
        RemovePlayerOnSlot(slotId, playerId);
    }

    public void SetMods(int playerId, Mods mods)
    {
        if (FreeMode)
        {
            if (HostId == playerId)
                Mods = mods & Mods.SpeedChangingMods;

            var playerSlot = PlayerSlotId(playerId);
            _slotMods[playerSlot] = (byte) (mods & ~Mods.SpeedChangingMods);

            AddMatchUpdatedEvent();

            return;
        }

        AssertHost(playerId);
        Mods = mods;

        AddMatchUpdatedEvent();
    }

    public void SetPassword(int playerId, string newPassword)
    {
        AssertHost(playerId);
        Password = newPassword;
        AddMatchUpdatedEvent();
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
        AddMatchUpdatedEvent();
    }

    public void ChangeSettings(int playerId, Match newMatch)
    {
        AssertHost(playerId);

        if (newMatch.FreeMode != FreeMode)
        {
            FreeMode = newMatch.FreeMode;

            //If we converted from non-freemode to freemode
            if (newMatch.FreeMode)
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
        AddMatchUpdatedEvent();
    }

    public void ChangeTeam(int playerId)
    {
        if (TeamType is not (MatchTeamTypes.TeamVs or MatchTeamTypes.TagTeamVs))
        {
            throw new Exception("Match is not in team mode to change team");
        }

        var slotId = PlayerSlotId(playerId);
        var prevTeam = (MatchTeams) _slotTeams[slotId];
        _slotTeams[slotId] = (byte) (prevTeam switch
        {
            MatchTeams.Red => MatchTeams.Blue,
            _ => MatchTeams.Blue
        });
        AddMatchUpdatedEvent();
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

        AddEvent(new MatchCompletedEvent(_playerIds.ToList()));
        AddMatchUpdatedEvent();
    }

    private void ChangeStatus(int playerId, SlotStatus status)
    {
        var slotId = PlayerSlotId(playerId);
        _slotStatus[slotId] = (byte) status;
        AddMatchUpdatedEvent();
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

    public void Start(int playerId)
    {
        AssertHost(playerId);

        InProgress = true;
        _skipCounter = 0;
        _loadingCounter = 0;

        var playingPlayers = new List<int>();
        for (var i = 0; i < _slotStatus.Length; i++)
        {
            if (_slotStatus[i] != (byte) SlotStatus.Ready && _slotStatus[i] != (byte) SlotStatus.NotReady) continue;

            _slotStatus[i] = (byte) SlotStatus.Playing;

            _skipCounter++;
            _loadingCounter++;
            playingPlayers.Add(PlayerIdOnSlot(i));
        }


        AddEvent(new MatchStartEvent(playingPlayers, this));
        AddMatchUpdatedEvent();
    }

    public void LoadComplete(int playerId)
    {
        _loadingCounter--;
        if (_loadingCounter != 0)
        {
            return;
        }

        //TODO: MatchLoadComplete
        AddEvent(new MatchLoadComplete(_playerIds.ToList()));
        //player.Player.Queue.EnqueuePacket(new ResponsePackets.Match.MatchComplete());
    }

    public void LockSlot(int playerId, int slotId)
    {
        AssertHost(playerId);
        if (_slotStatus[slotId] == (byte) SlotStatus.Locked)
        {
            _slotStatus[slotId] = (byte) SlotStatus.Open;
            AddMatchUpdatedEvent();

            return;
        }

        if (!SlotIsNotEmpty(_slotStatus[slotId]))
        {
            _slotStatus[slotId] = (byte) SlotStatus.Locked;
            _slotTeams[slotId] = 0;
            _slotMods[slotId] = 0;
            AddMatchUpdatedEvent();

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
        _skipCounter--;
        //TODO: event PlayerSkipped
        AddEvent(new MatchPlayerSkippedEvent(_playerIds.ToList(), PlayerSlotId(playerId)));

        if (_skipCounter == 0)
        {
            //TODO: event Skip
            AddEvent(new MatchSkippedEvent(_playerIds.ToList()));
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
        AddEvent(new MatchHostTransferredEvent(_playerIds.ToList()));
    }

    public void TransferHost(int playerId, int nextHostSlotId)
    {
        AssertHost(playerId);
        TransferHost(nextHostSlotId);
    }
    #endregion
}
