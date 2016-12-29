namespace UnityEditor.iOS.Xcode.PBX
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Text;

    internal class GUIDToCommentMap
    {
        private Dictionary<string, string> m_Dict = new Dictionary<string, string>();

        public void Add(string guid, string comment)
        {
            if (!this.m_Dict.ContainsKey(guid))
            {
                this.m_Dict.Add(guid, comment);
            }
        }

        public void Remove(string guid)
        {
            this.m_Dict.Remove(guid);
        }

        public string Write(string guid)
        {
            string str = this[guid];
            if (str == null)
            {
                return guid;
            }
            return $"{guid} /* {str} */";
        }

        public void WriteStringBuilder(StringBuilder sb, string guid)
        {
            string str = this[guid];
            if (str == null)
            {
                sb.Append(guid);
            }
            else
            {
                sb.Append(guid).Append(" /* ").Append(str).Append(" */");
            }
        }

        public string this[string guid]
        {
            get
            {
                if (this.m_Dict.ContainsKey(guid))
                {
                    return this.m_Dict[guid];
                }
                return null;
            }
        }
    }
}

