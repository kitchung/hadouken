﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using Hadouken.SemVer;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Hadouken.Plugins.Metadata
{
    public sealed class Manifest : IManifest
    {
        public const string FileName = "manifest.json";

        private static readonly IDictionary<int, Func<IManifestReader>> ManifestReaders =
            new Dictionary<int, Func<IManifestReader>>();

        static Manifest()
        {
            ManifestReaders.Add(1, () => new ManifestV1Reader());
            ManifestReaders.Add(2, () => new ManifestV2Reader());
        }

        private readonly string _name;
        private readonly SemanticVersion _version;
        private readonly SemanticVersion _minimumHostVersion;
        private readonly IEnumerable<Dependency> _dependencies;
        private readonly IEnumerable<EventHandler> _eventHandlers;
        private readonly IUserInterface _userInterface;
        private readonly PermissionSet _permissions;

        public Manifest(string name, SemanticVersion version, IEnumerable<Dependency> dependencies)
            : this(name, version, "0.0", dependencies, null, null, null)
        {
        }

        public Manifest(string name, SemanticVersion version, SemanticVersion minimumHostVersion, IEnumerable<Dependency> dependencies, IEnumerable<EventHandler> eventHandlers, IUserInterface userInterface, PermissionSet permissions)
        {
            _name = name;
            _version = version;
            _minimumHostVersion = minimumHostVersion;
            _dependencies = dependencies ?? Enumerable.Empty<Dependency>();
            _eventHandlers = eventHandlers ?? Enumerable.Empty<EventHandler>();
            _userInterface = userInterface;
            _permissions = permissions;
        }

        public string Name
        {
            get { return _name; }
        }

        public SemanticVersion Version
        {
            get { return _version; }
        }

        public SemanticVersion MinimumHostVersion
        {
            get { return _minimumHostVersion; }
        }

        public IEnumerable<Dependency> Dependencies
        {
            get { return _dependencies; }
        }

        public IEnumerable<EventHandler> EventHandlers
        {
            get { return _eventHandlers; }
        } 

        public IUserInterface UserInterface
        {
            get { return _userInterface; }
        }

        public PermissionSet Permissions
        {
            get { return _permissions; }
        }

        public static bool TryParse(Stream json, out IManifest manifest)
        {
            Exception exception;
            return TryParse(json, out manifest, out exception);
        }

        public static bool TryParse(Stream json, out IManifest manifest, out Exception exception)
        {
            manifest = null;
            exception = null;

            try
            {
                using (var streamReader = new StreamReader(json))
                using (var jsonReader = new JsonTextReader(streamReader))
                {
                    var obj = JToken.ReadFrom(jsonReader) as JObject;

                    if (obj == null)
                        return false;

                    JToken manifestVersionToken;

                    if (!obj.TryGetValue("manifest_version", out manifestVersionToken))
                        return false;

                    if (manifestVersionToken.Type != JTokenType.Integer)
                        return false;

                    var manifestVersion = manifestVersionToken.Value<int>();

                    if (!ManifestReaders.ContainsKey(manifestVersion))
                        return false;

                    var manifestReader = ManifestReaders[manifestVersion]();
                    manifest = manifestReader.Read(obj);

                    return true;
                }
            }
            catch (Exception e)
            {
                exception = e;
                return false;
            }
        }
    }
}
