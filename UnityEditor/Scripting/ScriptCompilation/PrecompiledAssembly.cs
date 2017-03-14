namespace UnityEditor.Scripting.ScriptCompilation
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct PrecompiledAssembly
    {
        public string Path;
        public AssemblyFlags Flags;
    }
}

