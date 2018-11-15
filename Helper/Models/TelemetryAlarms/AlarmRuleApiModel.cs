// Copyright (c) Microsoft. All rights reserved.

using System.Collections.Generic;
using Newtonsoft.Json;

namespace Helpers.Models.TelemetryAlarms
{
    public class AlarmRuleApiModel
    {
        [JsonProperty(PropertyName = "Id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "Severity")]
        public string Severity { get; set; }

        [JsonProperty(PropertyName = "Description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "$metadata", Order = 1000)]
        public Dictionary<string, string> Metadata;

        public AlarmRuleApiModel(
            string id,
            string severity,
            string description)
        {
            this.Id = id;
            this.Severity = severity;
            this.Description = description;

            this.Metadata = new Dictionary<string, string>
            {
                { "$type", $"Rule;1" },
                { "$uri", "/v1/rules/" + id }
            };
        }

        public AlarmRuleApiModel() { }
    }
}
