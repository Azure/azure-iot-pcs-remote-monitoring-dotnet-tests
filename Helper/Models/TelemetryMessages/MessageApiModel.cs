// Copyright (c) Microsoft. All rights reserved.

using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Helpers.Models.TelemetryMessages
{
    public class MessageApiModel
    {
        private const string DATE_FORMAT = "yyyy-MM-dd'T'HH:mm:sszzz";
        private DateTimeOffset time;

        [JsonProperty(PropertyName = "DeviceId")]
        public string DeviceId { get; set; }

        [JsonProperty(PropertyName = "Time")]
        public string Time => this.time.ToString(DATE_FORMAT);

        [JsonProperty(PropertyName = "Data")]
        public JObject Data { get; set; }

        public MessageApiModel(
            string deviceId,
            DateTimeOffset time,
            JObject data)
        {
            this.DeviceId = deviceId;
            this.time = time;
            this.Data = data;
        }

        public MessageApiModel() { }
    }
}
