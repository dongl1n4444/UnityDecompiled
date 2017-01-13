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
            MonoIsland island = new MonoIsland(BuildTarget.StandaloneWindows, "unity", sources, references, defines, outputFile);
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
            string str = MonoInstallationFinder.GetProfileDirectory(this._island._target, "4.5", "MonoBleedingEdge");
            string path = Path.Combine(str, "mcs.exe");
            if (!File.Exists(path))
            {
                throw new ApplicationException("Unable to find csharp compiler in " + str);
            }
            arguments.Add("-sdk:" + this._island._classlib_profile);
            return path;
        }

        protected override Program StartCompiler()
        {
            List<string> arguments = new List<string> {
                "-debug",
                "-target:library",
                "-nowarn:0169",
                "-langversion:4",
                "-out:" + ScriptCompilerBase.PrepareFileName(this._island._output)
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
            foreach (string str4 in this.GetAdditionalReferences())
            {
                string profile = this._island._classlib_profile;
                switch (profile)
                {
                    case "2":
                    case "2.0":
                        profile = "2.0-api";
                        break;

                    case "4":
                    case "4.0":
                        profile = "4.0-api";
                        break;

                    default:
                        if (profile == "4.5")
                        {
                            profile = "4.5-api";
                        }
                        break;
                }
                string path = Path.Combine(MonoInstallationFinder.GetProfileDirectory(this._island._target, profile, "MonoBleedingEdge"), str4);
                if (File.Exists(path))
                {
                    arguments.Add("-r:" + ScriptCompilerBase.PrepareFileName(path));
                }
            }
            if (!base.AddCustomResponseFileIfPresent(arguments, "mcs.rsp"))
            {
                if ((this._island._classlib_profile == "unity") && base.AddCustomResponseFileIfPresent(arguments, "smcs.rsp"))
                {
                    Debug.LogWarning("Using obsolete custom response file 'smcs.rsp'. Please use 'mcs.rsp' instead.");
                }
                else if ((this._island._classlib_profile == "2.0") && base.AddCustomResponseFileIfPresent(arguments, "gmcs.rsp"))
                {
                    Debug.LogWarning("Using obsolete custom response file 'gmcs.rsp'. Please use 'mcs.rsp' instead.");
                }
            }
            return base.StartCompiler(this._island._target, this.GetCompilerPath(arguments), arguments, false, MonoInstallationFinder.GetMonoInstallation("MonoBleedingEdge"));
        }
    }
}

