using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace StorageAdapter
{
    public class ValueListApiModel
    {
        [JsonProperty("Items")]
        public readonly IEnumerable<ValueApiModel> Items;

        [JsonProperty("$metadata")]
        public Dictionary<string, string> Metadata;
    }
}
