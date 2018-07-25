// Copyright (c) Microsoft. All rights reserved.

using System.Collections.Generic;
using Newtonsoft.Json;

namespace Helpers.Models.TelemetryRules
{
    public class RuleListApiModel
    {
        private List<RuleApiModel> items;

        [JsonProperty(PropertyName = "Items")]
        public List<RuleApiModel> Items
        {
            get { return this.items; }
        }

        [JsonProperty(PropertyName = "$metadata")]
        public IDictionary<string, string> Metadata => new Dictionary<string, string>
        {
            { "$type", "RuleList;1" },
            { "$uri", "/v1/rules" },
        };

        public RuleListApiModel()
        {
            this.items = new List<RuleApiModel>();
        }
    }
}
