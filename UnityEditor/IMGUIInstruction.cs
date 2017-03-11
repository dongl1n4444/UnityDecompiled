namespace UnityEditor
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    [StructLayout(LayoutKind.Sequential), RequiredByNativeCode]
    internal struct IMGUIInstruction
    {
        public UnityEditor.InstructionType type;
        public int level;
        public Rect unclippedRect;
        public StackFrame[] stack;
        public int typeInstructionIndex;
    }
}

