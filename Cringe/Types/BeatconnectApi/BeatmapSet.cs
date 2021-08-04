using System;
using Cringe.Types.Enums;
using Newtonsoft.Json;

namespace Cringe.Types.BeatconnectApi
{
    public class BeatmapSet
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("artist")]
        public string Artist { get; set; }

        [JsonProperty("has_favourited")]
        public bool HasFavourited { get; set; }

        [JsonProperty("submitted_date")]
        public DateTime SubmittedDate { get; set; }

        [JsonProperty("ranked_date")]
        public DateTime RankedDate { get; set; }

        [JsonProperty("creator")]
        public string Creator { get; set; }

        [JsonProperty("user_id")]
        public long UserId { get; set; }

        [JsonProperty("average_length")]
        public long AverageLength { get; set; }

        [JsonProperty("bpm")]
        public double Bpm { get; set; }

        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("covers_id")]
        public long CoversId { get; set; }

        [JsonProperty("tags")]
        public string Tags { get; set; }

        [JsonProperty("video")]
        public bool HasVideo { get; set; }

        [JsonProperty("storyboard")]
        public bool HasStoryboard { get; set; }

        [JsonProperty("has_scores")]
        public bool HasScores { get; set; }

        [JsonProperty("discussion_enabled")]
        public bool HasDiscussionEnabled { get; set; }

        [JsonProperty("ranked")]
        public string Ranked { get; set; }

        [JsonProperty("status")]
        public RankedStatus Status { get; set; }

        [JsonProperty("legacy_thread_url")]
        public string legacy_thread_url { get; set; }

        [JsonProperty("preview_url")]
        public string preview_url { get; set; }

        [JsonProperty("mode_std")]
        public bool ModeStd { get; set; }

        [JsonProperty("mode_mania")]
        public bool ModeMania { get; set; }

        [JsonProperty("mode_taiko")]
        public bool ModeTaiko { get; set; }

        [JsonProperty("mode_ctb")]
        public bool ModeCtb { get; set; }

        [JsonProperty("unique_id")]
        public string UniqueId { get; set; }

        [JsonProperty("map_not_found")]
        public bool MapNotFound { get; set; }

        [JsonProperty("beatmaps")]
        public Beatmap[] Beatmaps { get; set; }
    }
}
