// Copyright (c) Microsoft. All rights reserved.

using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Config.Models
{
    public class PackageApiModel
    {
        private const string DATE_FORMAT = "yyyy-MM-dd'T'HH:mm:sszzz";

        [JsonProperty("Id")]
        public string Id;

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("PackageType")]
        [JsonConverter(typeof(StringEnumConverter))]
        public PackageType packageType { get; set; }

        [JsonProperty("ConfigType")]
        public string ConfigType { get; set; }

        [JsonProperty(PropertyName = "DateCreated")]
        public string DateCreated { get; set; }

        [JsonProperty("Content")]
        public string Content { get; set; }

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
