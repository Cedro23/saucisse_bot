using Newtonsoft.Json;
using System.Collections.Generic;

namespace Saucisse_bot.Bots.JsonParsers
{
    /// <summary>
    /// JSON class made to store Discord's user IDs
    /// </summary>
    public class UsersJson
    {
        [JsonProperty("Pau")]
        public ulong Pau { get; private set; }
        [JsonProperty("Roro")]
        public ulong Roro { get; private set; }
        [JsonProperty("Fares")]
        public ulong Fares { get; private set; }
        [JsonProperty("Basile")]
        public ulong Basile { get; private set; }
        [JsonProperty("Giacomo")]
        public ulong Giacomo { get; private set; }
        [JsonProperty("Ele")]
        public ulong Ele { get; private set; }
        [JsonProperty("Ced")]
        public ulong Ced { get; private set; }
        [JsonProperty("Matbot")]
        public ulong Matbot { get; private set; }
        [JsonProperty("Ozhy")]
        public ulong Ozhy { get; private set; }
        [JsonProperty("Vincent")]
        public ulong Vincent { get; private set; }
        [JsonProperty("LCBot")]
        public ulong LCBot { get; private set; }
    }
}
