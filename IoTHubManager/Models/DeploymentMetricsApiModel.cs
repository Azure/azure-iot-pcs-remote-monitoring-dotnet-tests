// Copyright (c) Microsoft. All rights reserved.

using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace IoTHubManager
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum DeploymentStatus
    {
        Pending, Successful, Failed
    }

    public class DeploymentMetricsApiModel
    {
        [JsonProperty(PropertyName = "AppliedCount")]
        public long AppliedCount { get; set; }
        
        [JsonProperty(PropertyName = "FailedCount")]
        public long FailedCount { get; set; }

        [JsonProperty(PropertyName = "SucceededCount")]
        public long SucceededCount { get; set; }

        [JsonProperty(PropertyName = "TargetedCount")]
        public long TargetedCount { get; set; }

        [JsonProperty(PropertyName = "DeviceStatuses")]
        public IDictionary<string, DeploymentStatus> DeviceStatuses { get; set; }

        public DeploymentMetricsApiModel()
        {
        }
    }
}
