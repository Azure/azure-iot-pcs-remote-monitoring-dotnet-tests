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
        private const string APPLIED_METRICS_KEY = "appliedCount";
        private const string TARGETED_METRICS_KEY = "targetedCount";
        private const string SUCCESSFUL_METRICS_KEY = "successfullCount";
        private const string FAILED_METRICS_KEY = "failedCount";
        private const string PENDING_METRICS_KEY = "pendingCount";

        [JsonProperty(PropertyName = "SystemMetrics")]
        public IDictionary<string, long> SystemMetrics { get; set; }

        [JsonProperty(PropertyName = "CustomMetrics")]
        public IDictionary<string, long> CustomMetrics { get; set; }

        [JsonProperty(PropertyName = "DeviceStatuses")]
        public IDictionary<string, DeploymentStatus> DeviceStatuses { get; set; }

    }
}
