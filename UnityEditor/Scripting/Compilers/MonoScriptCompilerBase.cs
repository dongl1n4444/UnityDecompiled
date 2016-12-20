namespace UnityEditor.Scripting.Compilers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using UnityEditor;
    using UnityEditor.Scripting;
    using UnityEditor.Utils;

    internal abstract class MonoScriptCompilerBase : ScriptCompilerBase
    {
        private readonly bool runUpdater;

        protected MonoScriptCompilerBase(MonoIsland island, bool runUpdater) : base(island)
        {
            this.runUpdater = runUpdater;
        }

        protected string GetProfileDirectory()
        {
            return MonoInstallationFinder.GetProfileDirectory(this._island._target, this._island._classlib_profile);
        }

        protected ManagedProgram StartCompiler(BuildTarget target, string compiler, List<string> arguments)
        {
            base.AddCustomResponseFileIfPresent(arguments, Path.GetFileNameWithoutExtension(compiler) + ".rsp");
            return this.StartCompiler(target, compiler, arguments, true, MonoInstallationFinder.GetMonoInstallation());
        }

        protected ManagedProgram StartCompiler(BuildTarget target, string compiler, List<string> arguments, bool setMonoEnvironmentVariables, string monodistro)
        {
            string responseFile = CommandLineFormatter.GenerateResponseFile(arguments);
            if (this.runUpdater)
            {
                APIUpdaterHelper.UpdateScripts(responseFile, this._island.GetExtensionOfSourceFiles());
            }
            ManagedProgram program = new ManagedProgram(monodistro, this._island._classlib_profile, compiler, " @" + responseFile, setMonoEnvironmentVariables, null);
            program.Start();
            return program;
        }
    }
}

