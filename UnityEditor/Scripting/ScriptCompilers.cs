namespace UnityEditor.Scripting
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEditor.Scripting.Compilers;

    internal static class ScriptCompilers
    {
        private static List<SupportedLanguage> _supportedLanguages = new List<SupportedLanguage>();
        [CompilerGenerated]
        private static Func<SupportedLanguage, SupportedLanguageStruct> <>f__am$cache0;

        static ScriptCompilers()
        {
            List<Type> list = new List<Type> {
                typeof(CSharpLanguage),
                typeof(BooLanguage),
                typeof(UnityScriptLanguage)
            };
            foreach (Type type in list)
            {
                _supportedLanguages.Add((SupportedLanguage) Activator.CreateInstance(type));
            }
        }

        internal static ScriptCompilerBase CreateCompilerInstance(MonoIsland island, bool buildingForEditor, BuildTarget targetPlatform, bool runUpdater)
        {
            if (island._files.Length == 0)
            {
                throw new ArgumentException("Cannot compile MonoIsland with no files");
            }
            foreach (SupportedLanguage language in _supportedLanguages)
            {
                if (language.GetExtensionICanCompile() == island.GetExtensionOfSourceFiles())
                {
                    return language.CreateCompiler(island, buildingForEditor, targetPlatform, runUpdater);
                }
            }
            throw new ApplicationException($"Unable to find a suitable compiler for sources with extension '{island.GetExtensionOfSourceFiles()}' (Output assembly: {island._output})");
        }

        public static string GetExtensionOfSourceFile(string file) => 
            Path.GetExtension(file).ToLower().Substring(1);

        internal static string GetNamespace(string file, string definedSymbols)
        {
            if (string.IsNullOrEmpty(file))
            {
                throw new ArgumentException("Invalid file");
            }
            string extensionOfSourceFile = GetExtensionOfSourceFile(file);
            foreach (SupportedLanguage language in _supportedLanguages)
            {
                if (language.GetExtensionICanCompile() == extensionOfSourceFile)
                {
                    return language.GetNamespace(file, definedSymbols);
                }
            }
            throw new ApplicationException("Unable to find a suitable compiler");
        }

        internal static SupportedLanguageStruct[] GetSupportedLanguageStructs()
        {
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = new Func<SupportedLanguage, SupportedLanguageStruct>(null, (IntPtr) <GetSupportedLanguageStructs>m__0);
            }
            return Enumerable.Select<SupportedLanguage, SupportedLanguageStruct>(_supportedLanguages, <>f__am$cache0).ToArray<SupportedLanguageStruct>();
        }
    }
}

