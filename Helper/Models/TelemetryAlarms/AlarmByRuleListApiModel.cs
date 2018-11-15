// Copyright (c) Microsoft. All rights reserved.

using System.Collections.Generic;
using Newtonsoft.Json;

namespace Helpers.Models.TelemetryAlarms
{
    public class AlarmByRuleListApiModel
    {
        private readonly List<AlarmByRuleApiModel> items;

        [JsonProperty(PropertyName = "Items")]
        public List<AlarmByRuleApiModel> Items
        {
            get { return this.items; }

            private set { }
        }

        [JsonProperty(PropertyName = "$metadata", Order = 1000)]
        public Dictionary<string, string> Metadata => new Dictionary<string, string>
        {
            { "$type", $"AlarmByRuleList;1"},
            { "$uri", "/v1/alarmsbyrule" }
        };

        public AlarmByRuleListApiModel()
        {
            this.items = new List<AlarmByRuleApiModel>();
        }
    }
}
