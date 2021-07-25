using System;
using Newtonsoft.Json;

namespace Cringe.Types.OsuApi
{
    public class UserShort
    {
        [JsonProperty("id")]
        public uint Id { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("country")]
        public Country Country { get; set; }

        [JsonProperty("country_code")]
        public string CountryCode { get; set; }

        [JsonProperty("avatar_url")]
        public string AvatarUrl { get; set; }

        [JsonProperty("is_active")]
        public bool IsActive { get; set; }

        [JsonProperty("is_bot")]
        public bool IsBot { get; set; }

        [JsonProperty("is_online")]
        public bool IsOnline { get; set; }

        [JsonProperty("is_supporter")]
        public bool IsSupporter { get; set; }

        [JsonProperty("last_visit")]
        public DateTime? LastVisit { get; set; }
    }

    public class User : UserShort
    {
        [JsonProperty("join_date")]
        public DateTime JoinDate { get; set; }

        [JsonProperty("statistics")]
        public UserStatistics Statistics { get; set; }

        [JsonProperty("cover_url")]
        public string CoverUrl { get; set; }

        [JsonProperty("previous_usernames")]
        public string[] PreviousUsernames { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("badges")]
        public TournamentBadge[] Badges { get; set; }

        [JsonProperty("follower_count")]
        public uint Followers { get; set; }

        [JsonProperty("monthly_playcounts")]
        public TimeframeCount[] MonthlyPlays { get; set; }

        [JsonProperty("replays_watched_counts")]
        public TimeframeCount[] ReplaysWatched { get; set; }

        [JsonProperty("rankHistory")]
        public RankHistory Ranks { get; set; }

        public class UserCover
        {
            [JsonProperty("custom_url")]
            public string CustomUrl { get; set; }

            [JsonProperty("url")]
            public string Url { get; set; }

            [JsonProperty("id")]
            public int? Id { get; set; }
        }

        public class TournamentBadge
        {
            [JsonProperty("awarded_at")]
            public DateTime AwardedAt { get; set; }

            [JsonProperty("description")]
            public string Description { get; set; }

            [JsonProperty("image_url")]
            public string ImageUrl { get; set; }

            [JsonProperty("url")]
            public string Url { get; set; }
        }

        public class RankHistory
        {
            [JsonProperty("mode")]
            public string Mode { get; set; }

            [JsonProperty("data")]
            public int[] Ranks { get; set; }
        }
    }

    public class UserStatistics
    {
        [JsonProperty("play_count")]
        public uint Playcount { get; set; }

        [JsonProperty("play_time")]
        public uint PlaytimeSeconds { get; set; }

        [JsonProperty("pp")]
        public double Pp { get; set; }

        [JsonProperty("global_rank")]
        public uint GlobalRank { get; set; }

        [JsonProperty("rank")]
        public UserRank Rank { get; set; }

        [JsonProperty("pp_country_rank")]
        public uint CountryRank { get; set; }

        [JsonProperty("hit_accuracy")]
        public double Accuracy { get; set; }

        [JsonProperty("level")]
        public UserLevel Level { get; set; }

        [JsonProperty("ranked_score")]
        public ulong RankedScore { get; set; }

        [JsonProperty("total_score")]
        public ulong TotalScore { get; set; }

        [JsonProperty("total_hits")]
        public ulong TotalHits { get; set; }

        [JsonProperty("maximum_combo")]
        public uint MaximumCombo { get; set; }

        [JsonProperty("replays_watched_by_others")]
        public uint TeplaysWatched { get; set; }

        [JsonProperty("grade_counts")]
        public UserGradeCounts GradeCounts { get; set; }

        [JsonProperty("is_ranked")]
        public bool HasRank { get; set; }

        public class UserRank
        {
            [JsonProperty("country")]
            public uint Country { get; set; }
        }

        public class UserLevel
        {
            [JsonProperty("current")]
            public uint Current { get; set; }

            [JsonProperty("progress")]
            public uint Progress { get; set; }
        }

        public class UserGradeCounts
        {
            [JsonProperty("ss")]
            public uint SS { get; set; }

            [JsonProperty("s")]
            public uint S { get; set; }

            [JsonProperty("a")]
            public uint A { get; set; }
        }
    }
}
