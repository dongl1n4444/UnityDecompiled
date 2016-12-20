namespace UnityEditor
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    [StructLayout(LayoutKind.Sequential), RequiredByNativeCode]
    internal struct IMGUILayoutInstruction
    {
        public int level;
        public Rect unclippedRect;
        public int marginLeft;
        public int marginRight;
        public int marginTop;
        public int marginBottom;
        public GUIStyle style;
        public StackFrame[] stack;
        public int isGroup;
        public int isVertical;
    }
}

