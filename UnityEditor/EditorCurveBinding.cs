namespace UnityEditor
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// <para>Defines how a curve is attached to an object that it controls.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct EditorCurveBinding
    {
        /// <summary>
        /// <para>The transform path of the object that is animated.</para>
        /// </summary>
        public string path;
        private Type m_type;
        /// <summary>
        /// <para>The property of the object that is animated.</para>
        /// </summary>
        public string propertyName;
        private int m_isPPtrCurve;
        private int m_isPhantom;
        internal int m_ClassID;
        internal int m_ScriptInstanceID;
        public bool isPPtrCurve
        {
            get
            {
                return (this.m_isPPtrCurve != 0);
            }
        }
        internal bool isPhantom
        {
            get
            {
                return (this.m_isPhantom != 0);
            }
            set
            {
                this.m_isPhantom = !value ? 0 : 1;
            }
        }
        public static bool operator ==(EditorCurveBinding lhs, EditorCurveBinding rhs)
        {
            if (((lhs.m_ClassID != 0) && (rhs.m_ClassID != 0)) && ((lhs.m_ClassID != rhs.m_ClassID) || (lhs.m_ScriptInstanceID != rhs.m_ScriptInstanceID)))
            {
                return false;
            }
            return ((((lhs.path == rhs.path) && (lhs.type == rhs.type)) && (lhs.propertyName == rhs.propertyName)) && (lhs.m_isPPtrCurve == rhs.m_isPPtrCurve));
        }

        public static bool operator !=(EditorCurveBinding lhs, EditorCurveBinding rhs)
        {
            return !(lhs == rhs);
        }

        public override int GetHashCode()
        {
            object[] objArray1 = new object[] { this.path, ':', this.type.Name, ':', this.propertyName };
            return string.Concat(objArray1).GetHashCode();
        }

        public override bool Equals(object other)
        {
            if (!(other is EditorCurveBinding))
            {
                return false;
            }
            EditorCurveBinding binding = (EditorCurveBinding) other;
            return (this == binding);
        }

        public Type type
        {
            get
            {
                return this.m_type;
            }
            set
            {
                this.m_type = value;
                this.m_ClassID = 0;
                this.m_ScriptInstanceID = 0;
            }
        }
        public static EditorCurveBinding FloatCurve(string inPath, Type inType, string inPropertyName)
        {
            return new EditorCurveBinding { 
                path = inPath,
                type = inType,
                propertyName = inPropertyName,
                m_isPPtrCurve = 0,
                m_isPhantom = 0
            };
        }

        public static EditorCurveBinding PPtrCurve(string inPath, Type inType, string inPropertyName)
        {
            return new EditorCurveBinding { 
                path = inPath,
                type = inType,
                propertyName = inPropertyName,
                m_isPPtrCurve = 1,
                m_isPhantom = 0
            };
        }
    }
}

