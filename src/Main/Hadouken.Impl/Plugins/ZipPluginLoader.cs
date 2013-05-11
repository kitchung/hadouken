﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hadouken.Plugins;
using Hadouken.IO;
using System.Reflection;
using Ionic.Zip;
using System.IO;
using Hadouken.Reflection;

namespace Hadouken.Impl.Plugins
{
    [Component]
    public class ZipPluginLoader : IPluginLoader
    {
        private IFileSystem _fileSystem;

        public ZipPluginLoader(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public bool CanLoad(string path)
        {
            if (_fileSystem.IsDirectory(path))
                return false;

            // TODO: check file header as well
            byte[] data = _fileSystem.ReadAllBytes(path);

            int header = (data[0] << 24) | (data[1] << 16) | (data[2] << 8) | data[3];

            return (header == 0x04034b50 && !_fileSystem.IsDirectory(path) && path.EndsWith(".zip"));
        }

        public IEnumerable<byte[]> Load(string path)
        {
            List<byte[]> assemblies = new List<byte[]>();

            using (ZipFile file = ZipFile.Read(_fileSystem.OpenRead(path)))
            {
                foreach (ZipEntry entry in file.Entries.Where(e => e.FileName.EndsWith(".dll")))
                {
                    using (var ms = new MemoryStream())
                    {
                        entry.Extract(ms);
                        assemblies.Add(ms.ToArray());
                    }
                }
            }

            return assemblies;
        }
    }
}
