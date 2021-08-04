
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Cringe.Types.BeatconnectApi;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Cringe.Services
{
    public class BeatconnectApiWrapper
    {
        private readonly HttpClient _client;
        private readonly ILogger<BeatconnectApiWrapper> _logger;

        private readonly string _token;

        public BeatconnectApiWrapper(HttpClient client, IConfiguration configuration, ILogger<BeatconnectApiWrapper> logger)
        {
            _client = client;
            _logger = logger;
            _token = configuration["BeatconnectApiKey"] ?? string.Empty;
        }

        public async Task<SearchResponse> BeatmapSearch(string search, string mode, string status)
        {
            if (string.IsNullOrEmpty(_token))
            {
                _logger.LogWarning("Beatconnect API key is not set!");
                return null;
            }

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

        public async Task<BeatmapSet> GetBeatmapSet(int id)
        {
            if (string.IsNullOrEmpty(_token))
            {
                _logger.LogWarning("Beatconnect API key is not set!");
                return null;
            }

            var json = await _client.GetStringAsync($"beatmap/{id}/?token={_token}");
            if (!string.IsNullOrEmpty(json))
                return JsonConvert.DeserializeObject<BeatmapSet>(json);

            return null;
        }

        public Task<Stream> DownloadBeatmapSet(int id, string uniqueId)
        {
            return _client.GetStreamAsync($"https://beatconnect.io/b/{id}/{uniqueId}/");
        }
    }
}
