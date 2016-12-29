namespace UnityEditor.Scripting.Compilers
{
    using Boo.Lang.Parser;
    using System;
    using System.Linq;
    using UnityEditor;
    using UnityEditor.Scripting;

    internal class BooLanguage : SupportedLanguage
    {
        public override ScriptCompilerBase CreateCompiler(MonoIsland island, bool buildingForEditor, BuildTarget targetPlatform, bool runUpdater) => 
            new BooCompiler(island, runUpdater);

        public override string GetExtensionICanCompile() => 
            "boo";

        public override string GetLanguageName() => 
            "Boo";

        public override string GetNamespace(string fileName, string definedSymbols)
        {
            try
            {
                return BooParser.ParseFile(fileName).get_Modules().First<Module>().get_Namespace().get_Name();
            }
            catch
            {
            }
            return base.GetNamespace(fileName, definedSymbols);
        }
    }
}

