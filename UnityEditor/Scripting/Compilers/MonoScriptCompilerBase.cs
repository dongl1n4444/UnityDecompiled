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

        protected ManagedProgram StartCompiler(BuildTarget target, string compiler, List<string> arguments)
        {
            base.AddCustomResponseFileIfPresent(arguments, Path.GetFileNameWithoutExtension(compiler) + ".rsp");
            string monodistro = (this._island._api_compatibility_level != ApiCompatibilityLevel.NET_4_6) ? MonoInstallationFinder.GetMonoInstallation() : MonoInstallationFinder.GetMonoBleedingEdgeInstallation();
            return this.StartCompiler(target, compiler, arguments, true, monodistro);
        }

        protected ManagedProgram StartCompiler(BuildTarget target, string compiler, List<string> arguments, bool setMonoEnvironmentVariables, string monodistro)
        {
            string responseFile = CommandLineFormatter.GenerateResponseFile(arguments);
            if (this.runUpdater)
            {
                UnityEditor.Scripting.Compilers.APIUpdaterHelper.UpdateScripts(responseFile, this._island.GetExtensionOfSourceFiles());
            }
            ManagedProgram program = new ManagedProgram(monodistro, BuildPipeline.CompatibilityProfileToClassLibFolder(this._island._api_compatibility_level), compiler, " @" + responseFile, setMonoEnvironmentVariables, null);
            program.Start();
            return program;
        }
    }
}

