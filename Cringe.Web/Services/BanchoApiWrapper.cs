using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Cringe.Types;
using Newtonsoft.Json;

namespace Cringe.Web.Services
{
    public class BanchoApiWrapper
    {
        private readonly HttpClient _client;

        public BanchoApiWrapper(HttpClient client)
        {
            _client = client;
        }

        public Task SendNotification(int playerId, string text)
        {
            return _client.PostAsync("api/notification", new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("playerId", playerId.ToString()),
                new KeyValuePair<string, string>("text", text)
            }));
        }

        public async Task<int[]> GetOnlinePlayers()
        {
            var response = await _client.GetAsync("api/players/ids");

            return JsonConvert.DeserializeObject<int[]>(await response.Content.ReadAsStringAsync());
        }

        public async Task<MatchModel[]> GetActiveMatches()
        {
            var response = await _client.GetAsync("api/lobby/matches");

            var apiType = new[]
            {
                new
                {
                    match = new MatchModel()
                }
            };

            return JsonConvert.DeserializeAnonymousType(await response.Content.ReadAsStringAsync(), apiType)
                .Select(x => x.match).ToArray();
        }

        public Task UpdatePlayerStats(int playerId)
        {
            return _client.GetAsync($"api/players/{playerId}/stats/refresh");
        }
    }
}
