using Cringe.Types.Enums;
using Newtonsoft.Json;

namespace Cringe.Types.OsuApi
{
    public class BeatmapShort
    {
        [JsonProperty("id")]
        public uint Id { get; set; }

        [JsonProperty("checksum")]
        public string Md5 { get; set; }

        [JsonProperty("beatmapset_id")]
        public int BeatmapSetId { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("mode_int")]
        public GameModes Mode { get; set; }

        [JsonProperty("mode")]
        public string ModeName { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("ar")]
        public double AR { get; set; }

        [JsonProperty("accuracy")]
        public double OD { get; set; }

        [JsonProperty("cs")]
        public double CS { get; set; }

        [JsonProperty("drain")]
        public double HP { get; set; }

        [JsonProperty("bpm")]
        public double BPM { get; set; }

        [JsonProperty("count_circles")]
        public uint? Circles { get; set; }

        [JsonProperty("count_sliders")]
        public uint? Sliders { get; set; }

        [JsonProperty("count_spinners")]
        public uint? Spinners { get; set; }

        [JsonProperty("total_length")]
        public uint Length { get; set; }

        [JsonProperty("hit_length")]
        public uint DrainLength { get; set; }

        //[JsonProperty("status")]
        //[JsonConverter(typeof(StringEnumConverter), typeof(SnakeCaseNamingStrategy))]
        //public RankedStatus Status { get; set; }

        [JsonProperty("difficulty_rating")]
        public double StarRating { get; set; }

        [JsonProperty("ranked")]
        public bool Ranked { get; set; }

        [JsonProperty("beatmapset")]
        public BeatmapSetShort BeatmapSet { get; set; }

        /*
            "convert": false,
            "deleted_at": null,
            "is_scoreable": true,
            "last_updated": "2019-10-04T00:29:36+00:00",
            "passcount": 277434,
            "playcount": 1058211,
        */

        public virtual string Thumbnail => "https://assets.ppy.sh/beatmaps/" + BeatmapSetId + "/covers/card@2x.jpg";
    }

    public class Beatmap : BeatmapShort
    {
        [JsonProperty("max_combo")]
        public uint? MaxCombo { get; set; }

        public override string Thumbnail => BeatmapSet?.Covers?.Cover2X;
    }
}
