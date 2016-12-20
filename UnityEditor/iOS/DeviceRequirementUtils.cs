namespace UnityEditor.iOS
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEditor.iOS.Xcode;

    internal class DeviceRequirementUtils
    {
        private static readonly Dictionary<string, string> m_DeviceTypeToString;
        private static readonly Dictionary<string, string> m_GraphicsToString;
        private static readonly Dictionary<string, RequirementDesc> m_KeyDescriptions;
        private static readonly Dictionary<string, string> m_MemoryToString;

        static DeviceRequirementUtils()
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string> {
                { 
                    UnityEditor.iOS.DeviceTypeRequirement.Any,
                    "Any"
                },
                { 
                    UnityEditor.iOS.DeviceTypeRequirement.iPhone,
                    "iPhone"
                },
                { 
                    UnityEditor.iOS.DeviceTypeRequirement.iPad,
                    "iPad"
                },
                { 
                    UnityEditor.iOS.DeviceTypeRequirement.iWatch,
                    "iWatch"
                }
            };
            m_DeviceTypeToString = dictionary;
            dictionary = new Dictionary<string, string> {
                { 
                    UnityEditor.iOS.MemoryRequirement.Any,
                    "Any"
                },
                { 
                    UnityEditor.iOS.MemoryRequirement.Mem1GB,
                    "1GB"
                },
                { 
                    UnityEditor.iOS.MemoryRequirement.Mem2GB,
                    "2GB"
                }
            };
            m_MemoryToString = dictionary;
            dictionary = new Dictionary<string, string> {
                { 
                    UnityEditor.iOS.GraphicsRequirement.Any,
                    "Any"
                },
                { 
                    UnityEditor.iOS.GraphicsRequirement.Metal1v2,
                    "Metal1v2"
                },
                { 
                    UnityEditor.iOS.GraphicsRequirement.Metal2v2,
                    "Metal2v2"
                }
            };
            m_GraphicsToString = dictionary;
            Dictionary<string, RequirementDesc> dictionary2 = new Dictionary<string, RequirementDesc> {
                { 
                    UnityEditor.iOS.DeviceTypeRequirement.Key,
                    new RequirementDesc(LocalizationDatabase.MarkForTranslation("Device"), UnityEditor.iOS.DeviceTypeRequirement.Any, m_DeviceTypeToString)
                },
                { 
                    UnityEditor.iOS.MemoryRequirement.Key,
                    new RequirementDesc(LocalizationDatabase.MarkForTranslation("Memory"), UnityEditor.iOS.MemoryRequirement.Any, m_MemoryToString)
                },
                { 
                    UnityEditor.iOS.GraphicsRequirement.Key,
                    new RequirementDesc(LocalizationDatabase.MarkForTranslation("Graphics"), UnityEditor.iOS.GraphicsRequirement.Any, m_GraphicsToString)
                }
            };
            m_KeyDescriptions = dictionary2;
        }

        public static string GetDefaultValueForKey(string key)
        {
            return m_KeyDescriptions[key].defaultValue;
        }

        public static string GetKeyDescription(string key)
        {
            if (m_KeyDescriptions.ContainsKey(key))
            {
                return LocalizationDatabase.GetLocalizedString(m_KeyDescriptions[key].name);
            }
            return (LocalizationDatabase.GetLocalizedString("(Custom)") + " " + key);
        }

        public static string[] GetKnownKeys()
        {
            return Enumerable.ToArray<string>(m_KeyDescriptions.Keys);
        }

        public static string[] GetKnownValuesForKey(string key)
        {
            return Enumerable.ToArray<string>(m_KeyDescriptions[key].valueNames.Keys);
        }

        public static string GetValueDescription(string key, string value)
        {
            if (m_KeyDescriptions.ContainsKey(key))
            {
                RequirementDesc desc = m_KeyDescriptions[key];
                if (desc.valueNames.ContainsKey(value))
                {
                    return desc.valueNames[value];
                }
            }
            return (LocalizationDatabase.GetLocalizedString("(custom) ") + value);
        }

        internal static bool IsKnownValueForKey(string key, string value)
        {
            <IsKnownValueForKey>c__AnonStorey0 storey = new <IsKnownValueForKey>c__AnonStorey0 {
                value = value
            };
            return Array.Exists<string>(GetKnownValuesForKey(key), new Predicate<string>(storey.<>m__0));
        }

        public static string RequirementToReadableString(iOSDeviceRequirement requirement)
        {
            List<string> list = new List<string>();
            string[] knownKeys = GetKnownKeys();
            foreach (string str in knownKeys)
            {
                if (requirement.values.ContainsKey(str))
                {
                    string str2 = requirement.values[str];
                    if (str2 != GetDefaultValueForKey(str))
                    {
                        if (IsKnownValueForKey(str, str2))
                        {
                            list.Add(GetValueDescription(str, str2));
                        }
                        else
                        {
                            list.Add(GetKeyDescription(str) + ":" + GetValueDescription(str, str2));
                        }
                    }
                }
            }
            using (IEnumerator<string> enumerator = requirement.values.Keys.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    <RequirementToReadableString>c__AnonStorey1 storey = new <RequirementToReadableString>c__AnonStorey1 {
                        key = enumerator.Current
                    };
                    if (!Array.Exists<string>(knownKeys, new Predicate<string>(storey.<>m__0)))
                    {
                        list.Add(GetKeyDescription(storey.key) + ":" + requirement.values[storey.key]);
                    }
                }
            }
            if (list.Count == 0)
            {
                return LocalizationDatabase.GetLocalizedString("No requirements");
            }
            return string.Join(" ; ", list.ToArray());
        }

        public static DeviceRequirement ToXcodeRequirement(iOSDeviceRequirement requirement)
        {
            DeviceRequirement requirement2 = new DeviceRequirement();
            foreach (string str in requirement.values.Keys)
            {
                requirement2.AddCustom(str, requirement.values[str]);
            }
            return requirement2;
        }

        [CompilerGenerated]
        private sealed class <IsKnownValueForKey>c__AnonStorey0
        {
            internal string value;

            internal bool <>m__0(string v)
            {
                return (v == this.value);
            }
        }

        [CompilerGenerated]
        private sealed class <RequirementToReadableString>c__AnonStorey1
        {
            internal string key;

            internal bool <>m__0(string s)
            {
                return (s == this.key);
            }
        }

        private class RequirementDesc
        {
            public string defaultValue;
            public string name;
            public Dictionary<string, string> valueNames;

            public RequirementDesc(string name, string defaultValue, Dictionary<string, string> valueNames)
            {
                this.name = name;
                this.defaultValue = defaultValue;
                this.valueNames = valueNames;
            }
        }
    }
}

