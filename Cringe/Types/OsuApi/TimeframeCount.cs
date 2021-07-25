using System;
using Newtonsoft.Json;

namespace Cringe.Types.OsuApi
{
    public class TimeframeCount
    {
        [JsonProperty("start_date")]
        public DateTime StartDate { get; set; }

        [JsonProperty("count")]
        public uint Count { get; set; }
    }
}
