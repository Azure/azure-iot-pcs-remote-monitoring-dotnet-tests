// Copyright (c) Microsoft. All rights reserved.

using System.Collections.Generic;
using Newtonsoft.Json;

namespace Helpers.Models.TelemetryAlarms
{
    public class AlarmIdListApiModel
    {
        [JsonProperty(PropertyName = "Items")]
        public List<string> Items { get; set; }

        public AlarmIdListApiModel()
        {
            this.Items = null;
        }

        public AlarmIdListApiModel(List<string> items)
        {
            this.Items = items;
        }
    }
}
