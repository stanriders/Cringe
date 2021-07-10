
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Cringe.Services
{
    public class OsuApiWrapper
    {
        private readonly HttpClient _client;
        private readonly IConfiguration _configuration;

        public OsuApiWrapper(HttpClient client, IConfiguration configuration)
        {
            _client = client;
            _configuration = configuration;
        }

        public async Task DownloadBeatmap(int beatmapId)
        {
            var cachePath = _configuration["BeatmapCachePath"];
            if (!Directory.Exists(cachePath))
                Directory.CreateDirectory(cachePath);

            var beatmapBytes = await _client.GetByteArrayAsync($"https://osu.ppy.sh/osu/{beatmapId}");
            await File.WriteAllBytesAsync($"{cachePath}/{beatmapId}.osu", beatmapBytes);
        }
    }
}
