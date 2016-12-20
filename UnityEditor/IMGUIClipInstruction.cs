namespace UnityEditor
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    [StructLayout(LayoutKind.Sequential), RequiredByNativeCode]
    internal struct IMGUIClipInstruction
    {
        public Rect screenRect;
        public Rect unclippedScreenRect;
        public Vector2 scrollOffset;
        public Vector2 renderOffset;
        public bool resetOffset;
        public int level;
        public StackFrame[] pushStacktrace;
        public StackFrame[] popStacktrace;
    }
}

