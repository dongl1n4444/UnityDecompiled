namespace UnityEditor.Scripting.ScriptCompilation
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using UnityEditor;

    internal class ScriptAssemblySettings
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private UnityEditor.ApiCompatibilityLevel <ApiCompatibilityLevel>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private UnityEditor.BuildTarget <BuildTarget>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private UnityEditor.BuildTargetGroup <BuildTargetGroup>k__BackingField;
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private string[] <Defines>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string <FilenameSuffix>k__BackingField;
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private string <OutputDirectory>k__BackingField;

        public ScriptAssemblySettings()
        {
            this.BuildTarget = UnityEditor.BuildTarget.NoTarget;
            this.BuildTargetGroup = UnityEditor.BuildTargetGroup.Unknown;
        }

        public UnityEditor.ApiCompatibilityLevel ApiCompatibilityLevel { get; set; }

        public UnityEditor.BuildTarget BuildTarget { get; set; }

        public UnityEditor.BuildTargetGroup BuildTargetGroup { get; set; }

        public string[] Defines { get; set; }

        public string FilenameSuffix { get; set; }

        public string OutputDirectory { get; set; }
    }
}

