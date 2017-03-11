namespace UnityEditor.Scripting.Compilers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using UnityEditor;
    using UnityEditor.Scripting;
    using UnityEditor.Utils;
    using UnityEngine;

    internal class MonoCSharpCompiler : MonoScriptCompilerBase
    {
        [CompilerGenerated]
        private static Func<CompilerMessage, string> <>f__am$cache0;

        public MonoCSharpCompiler(MonoIsland island, bool runUpdater) : base(island, runUpdater)
        {
        }

        public static string[] Compile(string[] sources, string[] references, string[] defines, string outputFile)
        {
            MonoIsland island = new MonoIsland(BuildTarget.StandaloneWindows, ApiCompatibilityLevel.NET_2_0_Subset, sources, references, defines, outputFile);
            using (MonoCSharpCompiler compiler = new MonoCSharpCompiler(island, false))
            {
                compiler.BeginCompiling();
                while (!compiler.Poll())
                {
                    Thread.Sleep(50);
                }
                if (<>f__am$cache0 == null)
                {
                    <>f__am$cache0 = cm => cm.message;
                }
                return Enumerable.Select<CompilerMessage, string>(compiler.GetCompilerMessages(), <>f__am$cache0).ToArray<string>();
            }
        }

        protected override CompilerOutputParserBase CreateOutputParser() => 
            new MonoCSharpCompilerOutputParser();

        private string[] GetAdditionalReferences() => 
            new string[] { "System.Runtime.Serialization.dll", "System.Xml.Linq.dll", "UnityScript.dll", "UnityScript.Lang.dll", "Boo.Lang.dll" };

        private string GetCompilerPath(List<string> arguments)
        {
            string profileDirectory = MonoInstallationFinder.GetProfileDirectory("4.5", "MonoBleedingEdge");
            string path = Path.Combine(profileDirectory, "mcs.exe");
            if (!File.Exists(path))
            {
                throw new ApplicationException("Unable to find csharp compiler in " + profileDirectory);
            }
            arguments.Add("-sdk:" + ((this._island._api_compatibility_level != ApiCompatibilityLevel.NET_4_6) ? BuildPipeline.CompatibilityProfileToClassLibFolder(this._island._api_compatibility_level) : "4.6"));
            return path;
        }

        protected override Program StartCompiler()
        {
            List<string> arguments = new List<string> {
                "-debug",
                "-target:library",
                "-nowarn:0169",
                "-langversion:4",
                "-out:" + ScriptCompilerBase.PrepareFileName(this._island._output),
                "-unsafe"
            };
            if (!this._island._development_player && !this._island._editor)
            {
                arguments.Add("-optimize");
            }
            foreach (string str in this._island._references)
            {
                arguments.Add("-r:" + ScriptCompilerBase.PrepareFileName(str));
            }
            foreach (string str2 in this._island._defines.Distinct<string>())
            {
                arguments.Add("-define:" + str2);
            }
            foreach (string str3 in this._island._files)
            {
                arguments.Add(ScriptCompilerBase.PrepareFileName(str3));
            }
            string profile = (this._island._api_compatibility_level != ApiCompatibilityLevel.NET_2_0) ? base.GetMonoProfileLibDirectory() : "2.0-api";
            string profileDirectory = MonoInstallationFinder.GetProfileDirectory(profile, "MonoBleedingEdge");
            foreach (string str6 in this.GetAdditionalReferences())
            {
                string path = Path.Combine(profileDirectory, str6);
                if (File.Exists(path))
                {
                    arguments.Add("-r:" + ScriptCompilerBase.PrepareFileName(path));
                }
            }
            if (!base.AddCustomResponseFileIfPresent(arguments, "mcs.rsp"))
            {
                if ((this._island._api_compatibility_level == ApiCompatibilityLevel.NET_2_0_Subset) && base.AddCustomResponseFileIfPresent(arguments, "smcs.rsp"))
                {
                    Debug.LogWarning("Using obsolete custom response file 'smcs.rsp'. Please use 'mcs.rsp' instead.");
                }
                else if ((this._island._api_compatibility_level == ApiCompatibilityLevel.NET_2_0) && base.AddCustomResponseFileIfPresent(arguments, "gmcs.rsp"))
                {
                    Debug.LogWarning("Using obsolete custom response file 'gmcs.rsp'. Please use 'mcs.rsp' instead.");
                }
            }
            return base.StartCompiler(this._island._target, this.GetCompilerPath(arguments), arguments, false, MonoInstallationFinder.GetMonoInstallation("MonoBleedingEdge"));
        }
    }
}

