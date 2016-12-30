namespace UnityEngine
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    [StructLayout(LayoutKind.Sequential), UsedByNativeCode]
    public class TrackedReference
    {
        internal IntPtr m_Ptr;
        protected TrackedReference()
        {
        }

        public static bool operator ==(TrackedReference x, TrackedReference y)
        {
            object obj2 = x;
            object obj3 = y;
            if ((obj3 == null) && (obj2 == null))
            {
                return true;
            }
            if (obj3 == null)
            {
                return (x.m_Ptr == IntPtr.Zero);
            }
            if (obj2 == null)
            {
                return (y.m_Ptr == IntPtr.Zero);
            }
            return (x.m_Ptr == y.m_Ptr);
        }

        public static bool operator !=(TrackedReference x, TrackedReference y) => 
            !(x == y);

        public override bool Equals(object o) => 
            ((o as TrackedReference) == this);

        public override int GetHashCode() => 
            ((int) this.m_Ptr);

        public static implicit operator bool(TrackedReference exists) => 
            (exists != null);
    }
}

