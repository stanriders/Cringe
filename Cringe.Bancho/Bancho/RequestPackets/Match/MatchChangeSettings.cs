using System;
using System.Threading.Tasks;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;
using Cringe.Types.Enums.Multiplayer;
using Microsoft.Extensions.Logging;

namespace Cringe.Bancho.Bancho.RequestPackets.Match
{
    public class MatchChangeSettings : MatchChanged
    {
        public MatchChangeSettings(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override ClientPacketType Type => ClientPacketType.MatchChangeSettings;

        public override Task Execute(PlayerSession session, byte[] data)
        {
            if (session.MatchSession is null)
            {
                Logger.LogError("{Token} | User tries to change match's settings while his MatchSession is null",
                    session.Token);

                return Task.CompletedTask;
            }

            if (session.MatchSession.Match.Host != session.Id)
            {
                Logger.LogError("{Token} | User tries to change match's settings while not being a host",
                    session.Token);

                return Task.CompletedTask;
            }

            var sessionMatch = session.MatchSession.Match;
            var match = Types.Match.Parse(data);

            if (match.FreeMode != sessionMatch.FreeMode)
            {
                sessionMatch.FreeMode = match.FreeMode;
                if (match.FreeMode)
                {
                    foreach (var slot in sessionMatch.OccupiedSlots)
                        slot.Mods = sessionMatch.Mods & ~Mods.SpeedChangingMods;

                    sessionMatch.Mods &= Mods.SpeedChangingMods;
                }
                else
                {
                    var host = sessionMatch.GetHost();
                    if (host is null)
                    {
                        Logger.LogCritical("{Token} | No host in match wtf. Match info: {@Match}", session.Token,
                            sessionMatch);

                        return Task.CompletedTask;
                    }

                    sessionMatch.Mods &= Mods.SpeedChangingMods;
                    sessionMatch.Mods |= host.Mods;
                    foreach (var player in sessionMatch.OccupiedSlots)
                        player.Mods = Mods.None;
                }
            }

            if (match.MapId == -1)
            {
                foreach (var player in sessionMatch.OccupiedSlots)
                {
                    player.Status = SlotStatus.NotReady;
                    sessionMatch.MapId = -1;
                    sessionMatch.MapMd5 = "";
                    sessionMatch.Name = "";
                }
            }
            else
            {
                if (sessionMatch.MapId == -1)
                {
                    Logger.LogDebug("{Token} | User selected a map {MapId}|{MapName}", session.Token, match.MapId,
                        match.MapName);
                    sessionMatch.MapId = match.MapId;
                    sessionMatch.MapMd5 = match.MapMd5;
                    sessionMatch.MapName = match.MapName;
                    sessionMatch.Mode = match.Mode;
                }
            }

            if (match.TeamTypes != sessionMatch.TeamTypes)
            {
                var team = match.TeamTypes is MatchTeamTypes.HeadToHead or MatchTeamTypes.TagCoop
                    ? MatchTeams.Neutral
                    : MatchTeams.Red;
                foreach (var player in sessionMatch.OccupiedSlots)
                    player.Team = team;

                sessionMatch.TeamTypes = match.TeamTypes;
            }

            sessionMatch.WinConditions = match.WinConditions;
            sessionMatch.Name = match.Name;
            session.MatchSession.OnUpdateMatch(true);

            Logger.LogDebug("{Token} | User changes the settings of the match to {@Match}", session.Token, match);

            return Task.CompletedTask;
        }
    }
}
