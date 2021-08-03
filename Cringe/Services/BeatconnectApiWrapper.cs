
using System.Net.Http;
using System.Threading.Tasks;
using Cringe.Types.BeatconnectApi;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Cringe.Services
{
    public class BeatconnectApiWrapper
    {
        private readonly HttpClient _client;
        private readonly string _token;

        public BeatconnectApiWrapper(HttpClient client, IConfiguration configuration)
        {
            _client = client;
            _token = configuration["BeatconnectApiKey"] ?? string.Empty;
        }

        public async Task<SearchResponse> BeatmapSearch(string search, string mode, string status)
        {
            var query = $"search/?token={_token}";
            if (!string.IsNullOrEmpty(search))
                query += $"&q={search}";

            if (!string.IsNullOrEmpty(mode))
                query += $"&m={mode}";

            if (!string.IsNullOrEmpty(status))
                query += $"&s={status}";

            var json = await _client.GetStringAsync(query);
            if (!string.IsNullOrEmpty(json))
                return JsonConvert.DeserializeObject<SearchResponse>(json);

            return null;
        }
    }
}
