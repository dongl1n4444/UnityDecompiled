namespace UnityEditor.Scripting.ScriptCompilation
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEditor.Scripting;

    internal class ScriptAssembly
    {
        [CompilerGenerated]
        private static Func<ScriptAssembly, string> <>f__am$cache0;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private UnityEditor.ApiCompatibilityLevel <ApiCompatibilityLevel>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private UnityEditor.BuildTarget <BuildTarget>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string[] <Defines>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string <Filename>k__BackingField;
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private string[] <Files>k__BackingField;
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private string <OutputDirectory>k__BackingField;
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private string[] <References>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool <RunUpdater>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ScriptAssembly[] <ScriptAssemblyReferences>k__BackingField;

        public string GetExtensionOfSourceFiles() => 
            ((this.Files.Length <= 0) ? "NA" : Path.GetExtension(this.Files[0]).ToLower().Substring(1));

        public MonoIsland ToMonoIsland(BuildFlags buildFlags, string buildOutputDirectory)
        {
            bool editor = (buildFlags & BuildFlags.BuildingForEditor) == BuildFlags.BuildingForEditor;
            bool flag2 = (buildFlags & BuildFlags.BuildingDevelopmentBuild) == BuildFlags.BuildingDevelopmentBuild;
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = a => Path.Combine(a.OutputDirectory, a.Filename);
            }
            string[] references = Enumerable.Select<ScriptAssembly, string>(this.ScriptAssemblyReferences, <>f__am$cache0).Concat<string>(this.References).ToArray<string>();
            return new MonoIsland(this.BuildTarget, editor, flag2, this.ApiCompatibilityLevel, this.Files, references, this.Defines, Path.Combine(buildOutputDirectory, this.Filename));
        }

        public UnityEditor.ApiCompatibilityLevel ApiCompatibilityLevel { get; set; }

        public UnityEditor.BuildTarget BuildTarget { get; set; }

        public string[] Defines { get; set; }

        public string Filename { get; set; }

        public string[] Files { get; set; }

        public string FullPath =>
            Path.Combine(this.OutputDirectory, this.Filename);

        public string OutputDirectory { get; set; }

        public string[] References { get; set; }

        public bool RunUpdater { get; set; }

        public ScriptAssembly[] ScriptAssemblyReferences { get; set; }
    }
}

