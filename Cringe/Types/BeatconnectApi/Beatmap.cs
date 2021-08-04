using System;
using Cringe.Types.Enums;
using Newtonsoft.Json;

namespace Cringe.Types.BeatconnectApi
{
    public class Beatmap
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("mode_int")]
        public long ModeInt { get; set; }

        [JsonProperty("difficulty")]
        public double StarRating { get; set; }

        [JsonProperty("version")]
        public string DifficultyName { get; set; }

        [JsonProperty("total_length")]
        public long TotalLength { get; set; }

        [JsonProperty("cs")]
        public double CircleSize { get; set; }

        [JsonProperty("drain")]
        public double DrainLength { get; set; }

        [JsonProperty("accuracy")]
        public double OverallDifficulty { get; set; }

        [JsonProperty("ar")]
        public double ApproachRate { get; set; }

        [JsonProperty("count_circles")]
        public long CountCircles { get; set; }

        [JsonProperty("count_sliders")]
        public long CountSliders { get; set; }

        [JsonProperty("count_spinners")]
        public long CountSpinners { get; set; }

        [JsonProperty("count_total")]
        public long? CountTotal { get; set; }

        [JsonProperty("status")]
        public RankedStatus Status { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
    }
}
