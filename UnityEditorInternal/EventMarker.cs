namespace UnityEditorInternal
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    [Serializable, StructLayout(LayoutKind.Sequential), RequiredByNativeCode]
    public struct EventMarker
    {
        public int objectInstanceId;
        public int nameOffset;
        public int frame;
    }
}

