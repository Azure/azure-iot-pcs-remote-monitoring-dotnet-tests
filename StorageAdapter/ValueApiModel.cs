using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace StorageAdapter
{
    public class ValueApiModel
    {
        [JsonProperty("Key")]
        public string Key { get; set; }

        [JsonProperty("Data")]
        public string Data { get; set; }

        [JsonProperty("ETag")]
        public string ETag { get; set; }

        [JsonProperty("$metadata")]
        public Dictionary<string, string> Metadata;
    }
}
