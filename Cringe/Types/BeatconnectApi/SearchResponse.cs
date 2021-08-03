
using Newtonsoft.Json;

namespace Cringe.Types.BeatconnectApi
{
    public class SearchResponse
    {
        [JsonProperty("beatmaps")]
        public BeatmapSet[] Beatmaps { get; set; }

        [JsonProperty("max_page")]
        public int MaxPage { get; set; }
    }
}
