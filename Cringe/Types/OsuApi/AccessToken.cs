using System;
using Newtonsoft.Json;

namespace Cringe.Types.OsuApi
{
    public class AccessToken
    {
        private DateTime expireDate;

        [JsonProperty("token_type")]
        public string Type { get; set; }

        [JsonProperty("expires_in")]
        public long ExpiresIn
        {
            get => expireDate.Ticks;
            set => expireDate = DateTime.Now.AddSeconds(value);
        }

        public bool Expired => expireDate < DateTime.Now;

        [JsonProperty("access_token")]
        public string Token { get; set; }
    }
}
