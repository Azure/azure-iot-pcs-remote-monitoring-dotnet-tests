// Copyright (c) Microsoft. All rights reserved.

using System.Collections.Generic;
using Newtonsoft.Json;

namespace Helpers.Models.TelemetryMessages
{
    public class MessageListApiModel
    {
        private readonly List<MessageApiModel> items = new List<MessageApiModel>();
        private readonly List<string> properties = new List<string>();

        [JsonProperty(PropertyName = "Items")]
        public List<MessageApiModel> Items
        {
            get { return this.items; }
        }

        [JsonProperty(PropertyName = "Properties")]
        public List<string> Properties
        {
            get { return this.properties; }
        }

        [JsonProperty(PropertyName = "$metadata", Order = 1000)]
        public IDictionary<string, string> Metadata => new Dictionary<string, string>
        {
            { "$type", "MessageList;1" },
            { "$uri", "/v1/messages" },
        };

        public MessageListApiModel() { }
    }
}
