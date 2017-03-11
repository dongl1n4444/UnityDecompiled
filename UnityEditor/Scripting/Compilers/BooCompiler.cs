namespace UnityEditor.Scripting.Compilers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using UnityEditor.Scripting;
    using UnityEditor.Utils;
    using UnityEngineInternal;

    internal class BooCompiler : MonoScriptCompilerBase
    {
        public BooCompiler(MonoIsland island, bool runUpdater) : base(island, runUpdater)
        {
        }

        protected override CompilerOutputParserBase CreateOutputParser() => 
            new BooCompilerOutputParser();

        protected override Program StartCompiler()
        {
            List<string> arguments = new List<string> {
                "-debug",
                "-target:library",
                "-out:" + this._island._output,
                "-x-type-inference-rule-attribute:" + typeof(TypeInferenceRuleAttribute)
            };
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
            string compiler = Path.Combine(base.GetMonoProfileLibDirectory(), "booc.exe");
            return base.StartCompiler(this._island._target, compiler, arguments);
        }
    }
}

