namespace UnityEngine.Advertisements.Editor
{
    using SimpleJson;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    internal sealed class Configuration
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string <defaultPlacement>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool <enabled>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Dictionary<string, bool> <placements>k__BackingField;

        public Configuration(string configurationResponse)
        {
            IDictionary<string, object> dictionary = (IDictionary<string, object>) SimpleJson.SimpleJson.DeserializeObject(configurationResponse);
            this.enabled = (bool) dictionary["enabled"];
            this.placements = new Dictionary<string, bool>();
            foreach (IDictionary<string, object> dictionary2 in (IList<object>) dictionary["placements"])
            {
                string key = (string) dictionary2["id"];
                bool flag = (bool) dictionary2["allowSkip"];
                if ((bool) dictionary2["default"])
                {
                    this.defaultPlacement = key;
                }
                this.placements.Add(key, flag);
            }
        }

        public string defaultPlacement { get; private set; }

        public bool enabled { get; private set; }

        public Dictionary<string, bool> placements { get; private set; }
    }
}

