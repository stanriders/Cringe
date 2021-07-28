
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

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
            return _client.PostAsync("api/notification", new FormUrlEncodedContent(new []{ new KeyValuePair<string, string>(playerId.ToString(), text)}));
        }
    }
}
