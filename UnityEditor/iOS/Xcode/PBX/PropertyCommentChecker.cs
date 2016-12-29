namespace UnityEditor.iOS.Xcode.PBX
{
    using System;
    using System.Collections.Generic;

    internal class PropertyCommentChecker
    {
        private bool m_All;
        private int m_Level;
        private List<List<string>> m_Props;

        public PropertyCommentChecker()
        {
            this.m_Level = 0;
            this.m_All = false;
            this.m_Props = new List<List<string>>();
        }

        public PropertyCommentChecker(IEnumerable<string> props)
        {
            this.m_Level = 0;
            this.m_All = false;
            this.m_Props = new List<List<string>>();
            foreach (string str in props)
            {
                char[] separator = new char[] { '/' };
                this.m_Props.Add(new List<string>(str.Split(separator)));
            }
        }

        protected PropertyCommentChecker(int level, List<List<string>> props)
        {
            this.m_Level = level;
            this.m_All = false;
            this.m_Props = props;
        }

        private bool CheckContained(string prop)
        {
            if (this.m_All)
            {
                return true;
            }
            foreach (List<string> list in this.m_Props)
            {
                if (list.Count == (this.m_Level + 1))
                {
                    if (list[this.m_Level] == prop)
                    {
                        return true;
                    }
                    if (list[this.m_Level] == "*")
                    {
                        this.m_All = true;
                        return true;
                    }
                }
            }
            return false;
        }

        public bool CheckKeyInDict(string key) => 
            this.CheckContained(key);

        public bool CheckStringValueInArray(string value) => 
            this.CheckContained(value);

        public bool CheckStringValueInDict(string key, string value)
        {
            foreach (List<string> list in this.m_Props)
            {
                if ((list.Count == (this.m_Level + 2)) && ((((list[this.m_Level] == "*") || (list[this.m_Level] == key)) && (list[this.m_Level + 1] == "*")) || (list[this.m_Level + 1] == value)))
                {
                    return true;
                }
            }
            return false;
        }

        public PropertyCommentChecker NextLevel(string prop)
        {
            List<List<string>> props = new List<List<string>>();
            foreach (List<string> list2 in this.m_Props)
            {
                if ((list2.Count > (this.m_Level + 1)) && ((list2[this.m_Level] == "*") || (list2[this.m_Level] == prop)))
                {
                    props.Add(list2);
                }
            }
            return new PropertyCommentChecker(this.m_Level + 1, props);
        }
    }
}

