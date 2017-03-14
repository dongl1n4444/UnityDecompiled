namespace UnityEditor.Scripting.ScriptCompilation
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEditor;

    [StructLayout(LayoutKind.Sequential, Size=1)]
    internal struct CustomScriptAssemblyPlatform
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private string <Name>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private UnityEditor.BuildTarget <BuildTarget>k__BackingField;
        public CustomScriptAssemblyPlatform(string name, UnityEditor.BuildTarget buildTarget)
        {
            this = new CustomScriptAssemblyPlatform();
            this.Name = name;
            this.BuildTarget = buildTarget;
        }

        public string Name { get; set; }
        public UnityEditor.BuildTarget BuildTarget { get; set; }
    }
}

