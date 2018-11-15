// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Helpers.Models.TelemetryRules
{
    public class RuleApiModel
    {
        private const string DATE_FORMAT = "yyyy-MM-dd'T'HH:mm:sszzz";

        [JsonProperty(PropertyName = "ETag")]
        public string ETag { get; set; } = string.Empty;

        [JsonProperty(PropertyName = "Id")]
        public string Id { get; set; } = string.Empty;

        [JsonProperty(PropertyName = "Name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty(PropertyName = "DateCreated")]
        public string DateCreated { get; set; } = DateTimeOffset.UtcNow.ToString(DATE_FORMAT);

        [JsonProperty(PropertyName = "DateModified")]
        public string DateModified { get; set; } = DateTimeOffset.UtcNow.ToString(DATE_FORMAT);

        [JsonProperty(PropertyName = "Enabled")]
        public bool Enabled { get; set; } = false;

        [JsonProperty(PropertyName = "Description")]
        public string Description { get; set; } = string.Empty;

        [JsonProperty(PropertyName = "GroupId")]
        public string GroupId { get; set; } = string.Empty;

        [JsonProperty(PropertyName = "Severity")]
        public string Severity { get; set; } = string.Empty;

        [JsonProperty(PropertyName = "Conditions")]
        public List<ConditionApiModel> Conditions { get; set; } = new List<ConditionApiModel>();

        // Possible values -["average", "instant"]
        [JsonProperty(PropertyName = "Calculation")]
        public string Calculation { get; set; } = string.Empty;

        // Possible values -["60000", "300000", "600000"] in milliseconds
        [JsonProperty(PropertyName = "TimePeriod")]
        public string TimePeriod { get; set; } = "0";

        [JsonProperty(PropertyName = "Actions", NullValueHandling = NullValueHandling.Ignore)]
        public List<ActionApiModel> Actions { get; set; }

        [JsonProperty(PropertyName = "$metadata")]
        public IDictionary<string, string> Metadata => new Dictionary<string, string>
        {
            { "$type", "Rule;1"},
            { "$uri", "/v1/rules/" + this.Id }
        };

        [JsonProperty(PropertyName = "Deleted")]
        public bool? Deleted { get; set; }

        public RuleApiModel() { }
    }
}
