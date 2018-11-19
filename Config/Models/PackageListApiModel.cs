// Copyright (c) Microsoft. All rights reserved.

using System.Collections.Generic;

namespace Config.Models
{
    public class PackageListApiModel
    {
        public IEnumerable<PackageApiModel> Items { get; set; }
    }
}