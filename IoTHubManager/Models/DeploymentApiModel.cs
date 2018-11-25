// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace IoTHubManager.Models
{
    public class DeploymentApiModel
    {

        [JsonProperty(PropertyName = "Id")]
        public string DeploymentId { get; set; }

        [JsonProperty(PropertyName = "Name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "CreatedDateTimeUtc")]
        public DateTime CreatedDateTimeUtc { get; set; }

        [JsonProperty(PropertyName = "DeviceGroupId")]
        public string DeviceGroupId { get; set; }

        [JsonProperty(PropertyName = "DeviceGroupName")]
        public string DeviceGroupName { get; set; }

        [JsonProperty(PropertyName = "DeviceGroupQuery")]
        public string DeviceGroupQuery { get; set; }

        [JsonProperty(PropertyName = "PackageContent")]
        public string PackageContent { get; set; }

        [JsonProperty(PropertyName = "PackageName")]
        public string PackageName { get; set; }

        [JsonProperty(PropertyName = "Priority")]
        public int Priority { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty(PropertyName = "PackageType")]
        public PackageType PackageType { get; set; }

        [JsonProperty(PropertyName = "ConfigType")]
        public string ConfigType { get; set; }

        [JsonProperty(PropertyName = "Metrics", NullValueHandling = NullValueHandling.Ignore)]
        public DeploymentMetricsApiModel Metrics { get; set; }

        [JsonProperty(PropertyName = "$metadata")]
        public Dictionary<string, string> Metadata { get; set; }
    }

    public enum PackageType
    {
        EdgeManifest,
        DeviceConfiguration
    }

    public enum ConfigType
    {
        FirmwareUpdate
    }
}
