namespace UnityEditor.Scripting.Compilers
{
    using System;
    using UnityEditor;
    using UnityEditor.Scripting;

    internal class UnityScriptLanguage : SupportedLanguage
    {
        public override ScriptCompilerBase CreateCompiler(MonoIsland island, bool buildingForEditor, BuildTarget targetPlatform, bool runUpdater) => 
            new UnityScriptCompiler(island, runUpdater);

        public override string GetExtensionICanCompile() => 
            "js";

        public override string GetLanguageName() => 
            "UnityScript";
    }
}

