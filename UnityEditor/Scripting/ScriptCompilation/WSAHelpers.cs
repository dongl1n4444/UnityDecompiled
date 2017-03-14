namespace UnityEditor.Scripting.ScriptCompilation
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEditor.Scripting;

    internal static class WSAHelpers
    {
        [CompilerGenerated]
        private static Func<EditorBuildRules.TargetAssembly, bool> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<EditorBuildRules.TargetAssembly, bool> <>f__am$cache1;

        public static bool IsCSharpAssembly(string assemblyName, EditorBuildRules.TargetAssembly[] customTargetAssemblies)
        {
            <IsCSharpAssembly>c__AnonStorey0 storey = new <IsCSharpAssembly>c__AnonStorey0 {
                assemblyName = assemblyName
            };
            if (storey.assemblyName.ToLower().Contains("firstpass"))
            {
                return false;
            }
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = a => a.Flags != AssemblyFlags.EditorOnly;
            }
            return Enumerable.Any<EditorBuildRules.TargetAssembly>(Enumerable.Where<EditorBuildRules.TargetAssembly>(EditorBuildRules.GetTargetAssemblies(ScriptCompilers.CSharpSupportedLanguage, customTargetAssemblies), <>f__am$cache0), new Func<EditorBuildRules.TargetAssembly, bool>(storey.<>m__0));
        }

        public static bool IsCSharpFirstPassAssembly(string assemblyName, EditorBuildRules.TargetAssembly[] customTargetAssemblies)
        {
            <IsCSharpFirstPassAssembly>c__AnonStorey1 storey = new <IsCSharpFirstPassAssembly>c__AnonStorey1 {
                assemblyName = assemblyName
            };
            if (!storey.assemblyName.ToLower().Contains("firstpass"))
            {
                return false;
            }
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = a => a.Flags != AssemblyFlags.EditorOnly;
            }
            return Enumerable.Any<EditorBuildRules.TargetAssembly>(Enumerable.Where<EditorBuildRules.TargetAssembly>(EditorBuildRules.GetTargetAssemblies(ScriptCompilers.CSharpSupportedLanguage, customTargetAssemblies), <>f__am$cache1), new Func<EditorBuildRules.TargetAssembly, bool>(storey.<>m__0));
        }

        public static bool UseDotNetCore(string path, BuildTarget buildTarget, EditorBuildRules.TargetAssembly[] customTargetAssemblies)
        {
            PlayerSettings.WSACompilationOverrides compilationOverrides = PlayerSettings.WSA.compilationOverrides;
            bool flag = (buildTarget == BuildTarget.WSAPlayer) && (compilationOverrides != PlayerSettings.WSACompilationOverrides.None);
            string fileName = Path.GetFileName(path);
            return (flag && (IsCSharpAssembly(path, customTargetAssemblies) || ((compilationOverrides != PlayerSettings.WSACompilationOverrides.UseNetCorePartially) && IsCSharpFirstPassAssembly(fileName, customTargetAssemblies))));
        }

        [CompilerGenerated]
        private sealed class <IsCSharpAssembly>c__AnonStorey0
        {
            internal string assemblyName;

            internal bool <>m__0(EditorBuildRules.TargetAssembly a) => 
                (a.Filename == this.assemblyName);
        }

        [CompilerGenerated]
        private sealed class <IsCSharpFirstPassAssembly>c__AnonStorey1
        {
            internal string assemblyName;

            internal bool <>m__0(EditorBuildRules.TargetAssembly a) => 
                (a.Filename == this.assemblyName);
        }
    }
}

