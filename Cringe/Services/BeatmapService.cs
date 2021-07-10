
using System.IO;
using System.Threading.Tasks;
using Cringe.Database;
using Microsoft.Extensions.Configuration;

namespace Cringe.Services
{
    public class BeatmapService
    {
        private readonly OsuApiWrapper _apiService;
        private readonly IConfiguration _configuration;

        public BeatmapService(OsuApiWrapper apiService, IConfiguration configuration)
        {
            _apiService = apiService;
            _configuration = configuration;
        }

        public async Task<byte[]> GetBeatmapBytes(int beatmapId)
        {
            var cachePath = _configuration["BeatmapCachePath"];
            var beatmapPath = $"{cachePath}/{beatmapId}.osu";

            if (!File.Exists(beatmapPath))
                await _apiService.DownloadBeatmap(beatmapId);

            return await File.ReadAllBytesAsync(beatmapPath);
        }
    }
}
