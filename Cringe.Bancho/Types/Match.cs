using System;
using System.Collections.Generic;
using System.Linq;
using Cringe.Bancho.Bancho;
using Cringe.Bancho.Events.Lobby;
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

    private readonly Slot[] _slots = Enumerable.Range(0, 16).Select(x => new Slot(x)).ToArray();
    private IEnumerable<Slot> _playerSlots => _slots.Where(x => x.HasPlayer());

    public IReadOnlyList<int> CurrentlyPlayingPlayers =>
        _playerSlots.Where(x => x.Status == SlotStatus.Playing).Select(x => x.PlayerId!.Value).ToList().AsReadOnly();


    [PeppyField]
    public Map MapInfo { get; set; } = null!;

    [PeppyField]
    [ConstantSized(16)]
    private byte[] _slotStatus
    {
        get => _slots.Select(x => (byte) x.Status).ToArray();
        // ReSharper disable once ValueParameterNotUsed
        // ReSharper disable once UnusedMember.Local
        set { } //Ignore any changes
    }

    [PeppyField]
    [ConstantSized(16)]
    private byte[] _slotTeams
    {
        get => _slots.Select(x => (byte) x.Team).ToArray();
        // ReSharper disable once ValueParameterNotUsed
        // ReSharper disable once UnusedMember.Local
        set { } //Ignore any changes
    }

    [PeppyField]
    [DependentSized]
    private int[] _playersIds
    {
        get => _slots.Where(x => x.PlayerId.HasValue).Select(x => x.PlayerId.Value).ToArray();
        // ReSharper disable once ValueParameterNotUsed
        // ReSharper disable once UnusedMember.Local
        set { } //Ignore any changes
    }

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
    private byte[] _playersMods { get; set; } = null!;

    [PeppyField]
    public int Seed { get; set; }

    private int _skipCounter = 0;
    private int _loadingCounter = 0;

    public int? Dependency(string propertyName)
    {
        return propertyName switch
        {
            nameof(_playersIds) => _playerSlots.Count(),
            nameof(_playersMods) => FreeMode ? 16 : 0,
            _ => null
        };
    }

    public List<int> Players => _playersIds.ToList();

    private void AssertHost(int playerId)
    {
        if (playerId != HostId)
        {
            throw new Exception("Forbidden action");
        }
    }

    private void AddMatchUpdatedEvent(bool updateLobby = false)
    {
        AddEvent(new LocalMatchUpdatedEvent(this));

        if (updateLobby)
        {
            AddEvent(new UpdateLobbyEvent(this));
        }
    }

    private Slot PlayerSlot(int playerId)
    {
        var slot = _slots.FirstOrDefault(x => x.PlayerId == playerId);
        if (slot is null)
        {
            throw new Exception($"Player {playerId} is not found in the match");
        }

        return slot;
    }

    public int PlayerSlotIndex(int playerId)
    {
        for (int i = 0; i < _slots.Length; i++)
        {
            if (_slots[i].PlayerId == playerId) return i;
        }

        throw new Exception($"Player {playerId} is not found in the match");
    }

    #region Behaviour
    // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Global
    public void AddPlayer(int playerId, string password)
    {
        if (Password != "" && Password != password)
        {
            throw new Exception("Incorrect password");
        }


        var slot = _slots.FirstOrDefault(x => x.Status == SlotStatus.Open && x.PlayerId is null);
        if (slot is null)
        {
            throw new Exception("Lobby is full");
        }

        slot.PlayerId = playerId;
        slot.Status = SlotStatus.NotReady;
        if (TeamType is MatchTeamTypes.TeamVs or MatchTeamTypes.TagTeamVs)
        {
            slot.Team = MatchTeams.Red;
        }

        AddEvent(new MatchPlayerJoinedEvent(playerId, Id));
        AddMatchUpdatedEvent(true);
    }

    public void RemovePlayer(int playerId)
    {
        var slot = _slots.FirstOrDefault(x => x.PlayerId == playerId);
        if (slot is null)
        {
            throw new Exception($"Player {playerId} is not found");
        }

        AddEvent(new MatchPlayerLeftEvent(playerId, Id));

        slot.Reset();

        if (playerId != HostId) return;

        if (_playersIds.Length == 0)
        {
            AddEvent(new MatchDisbandedEvent(Id));

            return;
        }

        for (var i = 0; i < _slots.Length; i++)
        {
            if (_slots[i].HasPlayer())
            {
                TransferHost(i);
            }
        }

        AddMatchUpdatedEvent(true);
    }

    public void SetMods(int playerId, Mods mods)
    {
        if (FreeMode)
        {
            if (HostId == playerId)
                Mods = mods & Mods.SpeedChangingMods;

            var slot = PlayerSlot(playerId);
            slot.Mods = mods & ~Mods.SpeedChangingMods;

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
        if (_slots[slotId].Status != SlotStatus.Open)
        {
            throw new Exception("Slot zakryt nah :D");
        }

        for (var i = 0; i < _slots.Length; i++)
        {
            if (_slots[i].PlayerId != playerId) continue;

            _slots[slotId] = _slots[i];
            _slots[i] = new Slot();
            AddMatchUpdatedEvent(true);

            return;
        }

        throw new Exception($"Player {playerId} is not found");
    }

    //TODO: split this
    public void ChangeSettings(int playerId, Match newMatch)
    {
        AssertHost(playerId);

        if (newMatch.FreeMode != FreeMode)
        {
            FreeMode = newMatch.FreeMode;

            //If we converted from non-freemode to freemode
            if (newMatch.FreeMode)
            {
                foreach (var modSlot in _playerSlots)
                {
                    modSlot.Mods &= ~Mods.SpeedChangingMods;
                }

                Mods &= Mods.SpeedChangingMods;
            }
            else
            {
                var hostMods = _slots.First(x => x.PlayerId == HostId).Mods;

                Mods &= Mods.SpeedChangingMods;
                Mods |= hostMods;

                foreach (var s in _slots)
                {
                    s.Mods = Mods.None;
                }
            }
        }

        if (newMatch.MapInfo.MapId != MapInfo.MapId)
        {
            if (newMatch.MapInfo.MapId == -1)
            {
                foreach (var slot in _playerSlots)
                {
                    slot.Status = SlotStatus.NotReady;
                }
            }

            MapInfo = newMatch.MapInfo;
        }

        if (TeamType != newMatch.TeamType)
        {
            var team = newMatch.TeamType is MatchTeamTypes.HeadToHead or MatchTeamTypes.TagCoop
                ? MatchTeams.Neutral
                : MatchTeams.Red;

            foreach (var slot in _playerSlots)
            {
                slot.Team = team;
            }

            TeamType = newMatch.TeamType;
        }

        WinConditions = newMatch.WinConditions;
        RoomName = newMatch.RoomName;
        AddMatchUpdatedEvent(true);
    }

    public void ChangeTeam(int playerId)
    {
        if (TeamType is not (MatchTeamTypes.TeamVs or MatchTeamTypes.TagTeamVs))
        {
            throw new Exception("Match is not in team mode to change team");
        }

        var slot = _slots.First(x => x.PlayerId == playerId);
        slot.Team = slot.Team switch
        {
            MatchTeams.Red => MatchTeams.Blue,
            _ => MatchTeams.Blue
        };

        AddMatchUpdatedEvent();
    }

    public void Complete(int playerId)
    {
        _slots.First(x => x.PlayerId == playerId).Status = SlotStatus.Complete;

        if (_slots.Any(x => x.Status == SlotStatus.Playing)) return;

        //If all players completed
        foreach (var slot in _playerSlots.Where(x => x.Status == SlotStatus.Complete))
        {
            slot.Status = SlotStatus.NotReady;
        }

        AddEvent(new MatchCompletedEvent(_playersIds.ToList()));
        AddMatchUpdatedEvent(true);
    }

    private void ChangeStatus(int playerId, SlotStatus status)
    {
        _slots.First(x => x.PlayerId == playerId).Status = status;

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

    public void Failed(int playerId)
    {
        var slot = PlayerSlotIndex(playerId);
        AddEvent(new MatchFailedEvent(_playersIds, slot));
    }

    public void Start(int playerId)
    {
        AssertHost(playerId);

        InProgress = true;
        _skipCounter = 0;
        _loadingCounter = 0;

        var playingPlayers = _playerSlots
            .Where(x => x.HasPlayer())
            .Where(x => x.Status is SlotStatus.Ready or SlotStatus.NotReady)
            .ToList();

        foreach (var player in playingPlayers)
        {
            player.Status = SlotStatus.Playing;
            _skipCounter++;
            _loadingCounter++;
        }

        AddEvent(new MatchStartEvent(playingPlayers.Select(x => x.PlayerId!.Value).ToList(), this));
        AddMatchUpdatedEvent(true);
    }

    public void LoadComplete(int playerId)
    {
        _loadingCounter--;
        if (_loadingCounter != 0)
        {
            return;
        }

        AddEvent(new MatchLoadComplete(_playerSlots
            .Where(x => x.Status == SlotStatus.Playing)
            .Select(x => x.PlayerId!.Value)
            .ToList()));
    }

    public void LockSlot(int playerId, int slotId)
    {
        AssertHost(playerId);
        if (_slots[slotId].Status == SlotStatus.Locked)
        {
            _slots[slotId].Status = SlotStatus.Open;
            AddMatchUpdatedEvent(true);

            return;
        }

        if (!_slots[slotId].HasPlayer())
        {
            _slots[slotId].Status = SlotStatus.Locked;
            AddMatchUpdatedEvent(true);

            return;
        }

        if (PlayerSlot(playerId) == _slots[slotId])
        {
            throw new Exception("Unable to remove yourself with a lock xd");
        }

        RemovePlayer(_slots[slotId].PlayerId!.Value);
    }

    public void Skip(int playerId)
    {
        _skipCounter--;
        AddEvent(new MatchPlayerSkippedEvent(_playersIds.ToList(), PlayerSlotIndex(playerId)));

        if (_skipCounter == 0)
        {
            AddEvent(new MatchSkippedEvent(_playersIds.ToList()));
        }
    }

    private void TransferHost(int slotId)
    {
        var slot = _slots[slotId];
        if (!slot.HasPlayer())
        {
            throw new Exception("No player on that slot");
        }

        HostId = slot.PlayerId!.Value;

        //TODO: event next host id
        AddEvent(new MatchHostTransferredEvent(_playersIds.ToList()));
    }

    public void TransferHost(int playerId, int nextHostSlotId)
    {
        AssertHost(playerId);
        TransferHost(nextHostSlotId);
    }
    #endregion
}
