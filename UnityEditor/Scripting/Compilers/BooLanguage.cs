namespace UnityEditor.Scripting.Compilers
{
    using Boo.Lang.Parser;
    using System;
    using System.Linq;
    using UnityEditor;
    using UnityEditor.Scripting;

    internal class BooLanguage : SupportedLanguage
    {
        public override ScriptCompilerBase CreateCompiler(MonoIsland island, bool buildingForEditor, BuildTarget targetPlatform, bool runUpdater)
        {
            return new BooCompiler(island, runUpdater);
        }

        public override string GetExtensionICanCompile()
        {
            return "boo";
        }

        public override string GetLanguageName()
        {
            return "Boo";
        }

        public override string GetNamespace(string fileName, string definedSymbols)
        {
            try
            {
                return Enumerable.First<Module>(BooParser.ParseFile(fileName).get_Modules()).get_Namespace().get_Name();
            }
            catch
            {
            }
            return base.GetNamespace(fileName, definedSymbols);
        }
    }
}

