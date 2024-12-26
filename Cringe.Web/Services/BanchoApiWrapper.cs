using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Cringe.Types;
using Cringe.Types.Database;
using Cringe.Types.Enums;
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
#if !DEBUG
            return _client.PostAsync("api/players/{playerId}/notification", new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("text", text)
            }));
#else
            return Task.CompletedTask;
#endif
        }

        public async Task<int[]> GetOnlinePlayers()
        {
#if !DEBUG
            var response = await _client.GetAsync("api/players/all/online");

            return JsonConvert.DeserializeObject<int[]>(await response.Content.ReadAsStringAsync());
#else
            return new []{ 1, 2, 3 };
#endif
        }

        public async Task<MatchModel[]> GetActiveMatches()
        {
#if !DEBUG
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
#else
            return new []{ new MatchModel
            {
                Id = 1,
                FreeMode = true,
                Host = 1,
                InProgress = true,
                MapId = 1,
                MapName = "Not A Map",
                Mode = GameModes.Osu,
                Name = "Not A Lobby",
                Mods = Mods.HardRock,
                Players = new [] { new Player
                {
                    Id = 1,
                    Username = "Not A Player"
                }}
            }};
#endif
        }

        public Task UpdatePlayerStats(int playerId)
        {
#if !DEBUG
            return _client.GetAsync($"api/players/{playerId}/stats/refresh");
#else
            return Task.CompletedTask;
#endif
        }
    }
}
