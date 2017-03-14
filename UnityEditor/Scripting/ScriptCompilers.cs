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
        [CompilerGenerated]
        private static Func<SupportedLanguage, SupportedLanguageStruct> <>f__am$cache0;
        internal static readonly SupportedLanguage CSharpSupportedLanguage;
        internal static readonly List<SupportedLanguage> SupportedLanguages = new List<SupportedLanguage>();

        static ScriptCompilers()
        {
            List<System.Type> list = new List<System.Type> {
                typeof(CSharpLanguage),
                typeof(BooLanguage),
                typeof(UnityScriptLanguage)
            };
            foreach (System.Type type in list)
            {
                SupportedLanguages.Add((SupportedLanguage) Activator.CreateInstance(type));
            }
            CSharpSupportedLanguage = Enumerable.Single<SupportedLanguage>(SupportedLanguages, new Func<SupportedLanguage, bool>(ScriptCompilers.<ScriptCompilers>m__0));
        }

        [CompilerGenerated]
        private static bool <ScriptCompilers>m__0(SupportedLanguage l) => 
            (l.GetType() == typeof(CSharpLanguage));

        internal static ScriptCompilerBase CreateCompilerInstance(MonoIsland island, bool buildingForEditor, BuildTarget targetPlatform, bool runUpdater)
        {
            if (island._files.Length == 0)
            {
                throw new ArgumentException("Cannot compile MonoIsland with no files");
            }
            foreach (SupportedLanguage language in SupportedLanguages)
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

        internal static SupportedLanguage GetLanguageFromName(string name)
        {
            foreach (SupportedLanguage language in SupportedLanguages)
            {
                if (string.Equals(name, language.GetLanguageName(), StringComparison.OrdinalIgnoreCase))
                {
                    return language;
                }
            }
            throw new ApplicationException($"Script language '{name}' is not supported");
        }

        internal static string GetNamespace(string file, string definedSymbols)
        {
            if (string.IsNullOrEmpty(file))
            {
                throw new ArgumentException("Invalid file");
            }
            string extensionOfSourceFile = GetExtensionOfSourceFile(file);
            foreach (SupportedLanguage language in SupportedLanguages)
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
                <>f__am$cache0 = lang => new SupportedLanguageStruct { 
                    extension = lang.GetExtensionICanCompile(),
                    languageName = lang.GetLanguageName()
                };
            }
            return Enumerable.Select<SupportedLanguage, SupportedLanguageStruct>(SupportedLanguages, <>f__am$cache0).ToArray<SupportedLanguageStruct>();
        }
    }
}

