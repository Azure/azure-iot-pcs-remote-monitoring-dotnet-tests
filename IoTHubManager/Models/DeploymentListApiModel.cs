// Copyright (c) Microsoft. All rights reserved.

using System.Collections.Generic;
using Newtonsoft.Json;

namespace IoTHubManager.Models
{
    public class DeploymentListApiModel
    {
        [JsonProperty(PropertyName = "Items")]
        public List<DeploymentApiModel> Items { get; set; }
    }
}