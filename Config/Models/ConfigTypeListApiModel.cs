using System.Collections.Generic;
using Newtonsoft.Json;

namespace Config
{
    public class ConfigTypeListApiModel
    {
        [JsonProperty("Items")]
        public string[] configTypes { get; set; }

        [JsonProperty(PropertyName = "$metadata")]
        public Dictionary<string, string> Metadata { get; set; }
    }
}

