
using Newtonsoft.Json;

namespace Cringe.Types.OsuApi
{
	public class Country
	{
		[JsonProperty("code")]
		public string Code { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }
	}
}
