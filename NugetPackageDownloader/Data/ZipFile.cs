﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NugetPackageDownloader.Data
{
    public class ZipFile
    {
        public string Name { get; set; }
        public byte[] Content { get; set; }
    }
}
