namespace UnityEditor.iOS.Xcode.PBX
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class XCBuildConfigurationData : PBXObjectData
    {
        public string baseConfigurationReference;
        protected SortedDictionary<string, BuildConfigEntryData> entries = new SortedDictionary<string, BuildConfigEntryData>();

        public void AddProperty(string name, string value)
        {
            if (this.entries.ContainsKey(name))
            {
                this.entries[name].AddValue(EscapeWithQuotesIfNeeded(name, value));
            }
            else
            {
                this.SetProperty(name, value);
            }
        }

        public static XCBuildConfigurationData Create(string name)
        {
            XCBuildConfigurationData data = new XCBuildConfigurationData {
                guid = PBXGUID.Generate()
            };
            data.SetPropertyString("isa", "XCBuildConfiguration");
            data.SetPropertyString("name", name);
            return data;
        }

        private static string EscapeWithQuotesIfNeeded(string name, string value)
        {
            if (name != "LIBRARY_SEARCH_PATHS")
            {
                return value;
            }
            if (!value.Contains(" "))
            {
                return value;
            }
            if ((value.First<char>() == '"') && (value.Last<char>() == '"'))
            {
                return value;
            }
            return ("\"" + value + "\"");
        }

        public void RemoveProperty(string name)
        {
            if (this.entries.ContainsKey(name))
            {
                this.entries.Remove(name);
            }
        }

        public void RemovePropertyValue(string name, string value)
        {
            if (this.entries.ContainsKey(name))
            {
                this.entries[name].RemoveValue(EscapeWithQuotesIfNeeded(name, value));
            }
        }

        public void RemovePropertyValueList(string name, IEnumerable<string> valueList)
        {
            if (this.entries.ContainsKey(name))
            {
                this.entries[name].RemoveValueList(valueList);
            }
        }

        public void SetProperty(string name, string value)
        {
            this.entries[name] = BuildConfigEntryData.FromNameValue(name, EscapeWithQuotesIfNeeded(name, value));
        }

        public override void UpdateProps()
        {
            base.SetPropertyString("baseConfigurationReference", this.baseConfigurationReference);
            PBXElementDict dict = base.m_Properties.CreateDict("buildSettings");
            foreach (KeyValuePair<string, BuildConfigEntryData> pair in this.entries)
            {
                if (pair.Value.val.Count != 0)
                {
                    if (pair.Value.val.Count == 1)
                    {
                        dict.SetString(pair.Key, pair.Value.val[0]);
                    }
                    else
                    {
                        PBXElementArray array = dict.CreateArray(pair.Key);
                        foreach (string str in pair.Value.val)
                        {
                            array.AddString(str);
                        }
                    }
                }
            }
        }

        public override void UpdateVars()
        {
            this.baseConfigurationReference = base.GetPropertyString("baseConfigurationReference");
            this.entries = new SortedDictionary<string, BuildConfigEntryData>();
            if (base.m_Properties.Contains("buildSettings"))
            {
                PBXElementDict dict = base.m_Properties["buildSettings"].AsDict();
                foreach (string str in dict.values.Keys)
                {
                    PBXElement element = dict[str];
                    if (element is PBXElementString)
                    {
                        if (this.entries.ContainsKey(str))
                        {
                            this.entries[str].val.Add(element.AsString());
                        }
                        else
                        {
                            this.entries.Add(str, BuildConfigEntryData.FromNameValue(str, element.AsString()));
                        }
                    }
                    else if (element is PBXElementArray)
                    {
                        foreach (PBXElement element2 in element.AsArray().values)
                        {
                            if (element2 is PBXElementString)
                            {
                                if (this.entries.ContainsKey(str))
                                {
                                    this.entries[str].val.Add(element2.AsString());
                                }
                                else
                                {
                                    this.entries.Add(str, BuildConfigEntryData.FromNameValue(str, element2.AsString()));
                                }
                            }
                        }
                    }
                }
            }
        }

        public string name =>
            base.GetPropertyString("name");
    }
}

