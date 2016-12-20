namespace UnityEditorInternal
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    [Serializable, StructLayout(LayoutKind.Sequential), RequiredByNativeCode]
    public sealed class ObjectMemoryStackInfo
    {
        public bool expanded;
        public bool sorted;
        public int allocated;
        public int ownedAllocated;
        public ObjectMemoryStackInfo[] callerSites;
        public string name;
    }
}

