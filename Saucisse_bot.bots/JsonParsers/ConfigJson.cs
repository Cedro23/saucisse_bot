using Newtonsoft.Json;

namespace Saucisse_bot.Bots.JsonParsers
{
    class ConfigJson
    {
        [JsonProperty("token")]
        public string Token { get; private set; }
        [JsonProperty("prefix")]
        public string Prefix { get; private set; }
    }
}
