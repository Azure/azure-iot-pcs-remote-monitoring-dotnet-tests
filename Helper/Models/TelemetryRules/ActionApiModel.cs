// Copyright (c) Microsoft. All rights reserved.

using System.Collections.Generic;
using Newtonsoft.Json;

namespace Helpers.Models.TelemetryRules
{
    public class ActionApiModel
    {
        [JsonProperty(PropertyName = "Type")]
        public string Type { get; set; }

        // Note: Parameters dictionary should always be initialized as case-insensitive.
        [JsonProperty(PropertyName = "Parameters")]
        public IDictionary<string, object> Parameters { get; set; }
    }
}
