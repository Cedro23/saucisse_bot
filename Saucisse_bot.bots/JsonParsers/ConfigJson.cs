using Newtonsoft.Json;

namespace Saucisse_bot.Bots.JsonParsers
{
    /// <summary>
    /// JSON class made to store config variables linked to the bot
    /// </summary>
    public class ConfigJson
    {
        [JsonProperty("token")]
        public string Token { get; private set; }
        [JsonProperty("prefix")]
        public string Prefix { get; private set; }
    }
}
