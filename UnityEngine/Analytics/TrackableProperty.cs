namespace UnityEngine.Analytics
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [Serializable]
    internal class TrackableProperty
    {
        public const int kMaxParams = 10;
        [SerializeField]
        private List<FieldWithTarget> m_Fields;

        public override int GetHashCode()
        {
            int num = 0x11;
            foreach (FieldWithTarget target in this.m_Fields)
            {
                num = (num * 0x17) + target.paramName.GetHashCode();
            }
            return num;
        }

        public List<FieldWithTarget> fields
        {
            get
            {
                return this.m_Fields;
            }
            set
            {
                this.m_Fields = value;
            }
        }

        [Serializable]
        internal class FieldWithTarget
        {
            [SerializeField]
            private bool m_DoStatic;
            [SerializeField]
            private string m_FieldPath;
            [SerializeField]
            private string m_ParamName;
            [SerializeField]
            private string m_StaticString;
            [SerializeField]
            private UnityEngine.Object m_Target;
            [SerializeField]
            private string m_TypeString;

            public object GetValue()
            {
                if (this.m_DoStatic)
                {
                    return this.m_StaticString;
                }
                object target = this.m_Target;
                char[] separator = new char[] { '.' };
                foreach (string str in this.m_FieldPath.Split(separator))
                {
                    try
                    {
                        target = target.GetType().GetProperty(str).GetValue(target, null);
                    }
                    catch
                    {
                        target = target.GetType().GetField(str).GetValue(target);
                    }
                }
                return target;
            }

            public bool doStatic
            {
                get
                {
                    return this.m_DoStatic;
                }
                set
                {
                    this.m_DoStatic = value;
                }
            }

            public string fieldPath
            {
                get
                {
                    return this.m_FieldPath;
                }
                set
                {
                    this.m_FieldPath = value;
                }
            }

            public string paramName
            {
                get
                {
                    return this.m_ParamName;
                }
                set
                {
                    this.m_ParamName = value;
                }
            }

            public string staticString
            {
                get
                {
                    return this.m_StaticString;
                }
                set
                {
                    this.m_StaticString = value;
                }
            }

            public UnityEngine.Object target
            {
                get
                {
                    return this.m_Target;
                }
                set
                {
                    this.m_Target = value;
                }
            }

            public string typeString
            {
                get
                {
                    return this.m_TypeString;
                }
                set
                {
                    this.m_TypeString = value;
                }
            }
        }
    }
}

