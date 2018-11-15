// Copyright (c) Microsoft. All rights reserved.

using System.Collections.Generic;
using Newtonsoft.Json;

namespace Helpers.Models.TelemetryAlarms
{
    public class AlarmListByRuleApiModel : AlarmListApiModel
    {
        public AlarmListByRuleApiModel() { }

        [JsonProperty(PropertyName = "$metadata", Order = 1000)]
        public new Dictionary<string, string> Metadata => new Dictionary<string, string>
        {
            { "$type", $"AlarmsByRule;1"},
            { "$uri", "/v1/alarmsbyrule" }
        };
    }
}
