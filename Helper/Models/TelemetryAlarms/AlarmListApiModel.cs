// Copyright (c) Microsoft. All rights reserved.

using System.Collections.Generic;
using Newtonsoft.Json;

namespace Helpers.Models.TelemetryAlarms
{
    public class AlarmListApiModel
    {
        [JsonProperty(PropertyName = "Items")]
        public List<AlarmApiModel> Items { get; set; }

        [JsonProperty(PropertyName = "$metadata", Order = 1000)]
        public Dictionary<string, string> Metadata => new Dictionary<string, string>
        {
            { "$type", $"Alarms;1"},
            { "$uri", "/v1/alarms" }
        };

        public AlarmListApiModel() { }
    }
}
