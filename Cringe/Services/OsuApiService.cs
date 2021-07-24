using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Cringe.Types.OsuApi;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Cringe.Services
{
    public class OsuApiWrapper
    {
        private static AccessToken AccessToken;
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

        public Task<Beatmap> GetBeatmapInfo(int beatmapId)
        {
            return MakeApiRequest<Beatmap>($"beatmaps/{beatmapId}");
        }

        private async Task<T> MakeApiRequest<T>(string request)
        {
            if (AccessToken == null || AccessToken.Expired)
            {
                var authRequest = new
                {
                    client_id = _configuration["osuAPIClientId"],
                    client_secret = _configuration["osuAPIClientSecret"],
                    grant_type = "client_credentials",
                    scope = "public"
                };

                var authJson = await _client.PostAsync("https://osu.ppy.sh/oauth/token", new StringContent(JsonConvert.SerializeObject(authRequest), Encoding.UTF8, "application/json"));
                if (authJson.IsSuccessStatusCode)
                {
                    var response = await authJson.Content.ReadAsStringAsync();
                    AccessToken = JsonConvert.DeserializeObject<AccessToken>(response);
                }
            }

            if (AccessToken != null)
                try
                {
                    var json = await DownloadString($"https://osu.ppy.sh/api/v2/{request}", AccessToken.Token);

                    return JsonConvert.DeserializeObject<T>(json,
                        new JsonSerializerSettings {DateTimeZoneHandling = DateTimeZoneHandling.Utc});
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

            return default;
        }

        private async Task<string> DownloadString(string address, string bearer)
        {
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(address),
                Method = HttpMethod.Get,
                Headers =
                {
                    {HttpRequestHeader.Authorization.ToString(), $"Bearer {bearer}"}
                }
            };

            var response = await _client.SendAsync(request);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadAsStringAsync();

            if (response.StatusCode == HttpStatusCode.Found || response.StatusCode == HttpStatusCode.Unauthorized)
            {
                request = new HttpRequestMessage
                {
                    RequestUri = new Uri(response.RequestMessage.RequestUri.ToString()),
                    Method = HttpMethod.Get,
                    Headers =
                    {
                        {HttpRequestHeader.Authorization.ToString(), $"Bearer {bearer}"}
                    }
                };

                response = await _client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                    return await response.Content.ReadAsStringAsync();
            }

            return string.Empty;
        }
    }
}