namespace UnityEditor.iOS.Xcode.PBX
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Text;

    internal class KnownSectionBase<T> : SectionBase where T: PBXObjectData, new()
    {
        private Dictionary<string, T> m_Entries;
        private string m_Name;

        public KnownSectionBase(string sectionName)
        {
            this.m_Entries = new Dictionary<string, T>();
            this.m_Name = sectionName;
        }

        public void AddEntry(T obj)
        {
            this.m_Entries[obj.guid] = obj;
        }

        public override void AddObject(string key, PBXElementDict value)
        {
            T local = Activator.CreateInstance<T>();
            local.guid = key;
            local.SetPropertiesWhenSerializing(value);
            local.UpdateVars();
            this.m_Entries[local.guid] = local;
        }

        public IEnumerable<KeyValuePair<string, T>> GetEntries() => 
            this.m_Entries;

        public IEnumerable<string> GetGuids() => 
            this.m_Entries.Keys;

        public IEnumerable<T> GetObjects() => 
            this.m_Entries.Values;

        public bool HasEntry(string guid) => 
            this.m_Entries.ContainsKey(guid);

        public void RemoveEntry(string guid)
        {
            if (this.m_Entries.ContainsKey(guid))
            {
                this.m_Entries.Remove(guid);
            }
        }

        public override void WriteSection(StringBuilder sb, GUIDToCommentMap comments)
        {
            if (this.m_Entries.Count != 0)
            {
                sb.AppendFormat("\n\n/* Begin {0} section */", this.m_Name);
                List<string> list = new List<string>(this.m_Entries.Keys);
                list.Sort(StringComparer.Ordinal);
                foreach (string str in list)
                {
                    T local = this.m_Entries[str];
                    local.UpdateProps();
                    sb.Append("\n\t\t");
                    comments.WriteStringBuilder(sb, local.guid);
                    sb.Append(" = ");
                    Serializer.WriteDict(sb, local.GetPropertiesWhenSerializing(), 2, local.shouldCompact, local.checker, comments);
                    sb.Append(";");
                }
                sb.AppendFormat("\n/* End {0} section */", this.m_Name);
            }
        }

        public T this[string guid]
        {
            get
            {
                if (this.m_Entries.ContainsKey(guid))
                {
                    return this.m_Entries[guid];
                }
                return null;
            }
        }
    }
}

