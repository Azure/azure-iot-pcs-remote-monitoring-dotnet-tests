// Copyright (c) Microsoft. All rights reserved.

using Newtonsoft.Json;

namespace Helpers.Models.TelemetryRules
{
    public class ConditionApiModel
    {
        [JsonProperty(PropertyName = "Field")]
        public string Field { get; set; } = string.Empty;

        [JsonProperty(PropertyName = "Operator")]
        public string Operator { get; set; } = string.Empty;

        [JsonProperty(PropertyName = "Value")]
        public string Value { get; set; } = string.Empty;

        public ConditionApiModel() { }
    }
}
