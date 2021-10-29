using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saucisse_bot.Bots.JsonParser
{
    public struct UsersJson
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
    }
}
