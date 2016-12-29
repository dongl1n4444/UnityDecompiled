namespace UnityEditor.iOS.Xcode.PBX
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    internal class BuildConfigEntryData
    {
        public string name;
        public List<string> val = new List<string>();

        public void AddValue(string value)
        {
            if (!this.val.Contains(value))
            {
                this.val.Add(value);
            }
        }

        public static string ExtractValue(string src)
        {
            char[] trimChars = new char[] { ',' };
            return PBXStream.UnquoteString(src.Trim().TrimEnd(trimChars));
        }

        public static BuildConfigEntryData FromNameValue(string name, string value)
        {
            BuildConfigEntryData data = new BuildConfigEntryData {
                name = name
            };
            data.AddValue(value);
            return data;
        }

        public void RemoveValue(string value)
        {
            <RemoveValue>c__AnonStorey0 storey = new <RemoveValue>c__AnonStorey0 {
                value = value
            };
            this.val.RemoveAll(new Predicate<string>(storey.<>m__0));
        }

        public void RemoveValueList(IEnumerable<string> values)
        {
            List<string> list = new List<string>(values);
            if (list.Count != 0)
            {
                for (int i = 0; i < (this.val.Count - list.Count); i++)
                {
                    bool flag = true;
                    for (int j = 0; j < list.Count; j++)
                    {
                        if (this.val[i + j] != list[j])
                        {
                            flag = false;
                            break;
                        }
                    }
                    if (flag)
                    {
                        this.val.RemoveRange(i, list.Count);
                        break;
                    }
                }
            }
        }

        [CompilerGenerated]
        private sealed class <RemoveValue>c__AnonStorey0
        {
            internal string value;

            internal bool <>m__0(string v) => 
                (v == this.value);
        }
    }
}

