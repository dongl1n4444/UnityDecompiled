namespace UnityEditor.Scripting.Compilers
{
    using ICSharpCode.NRefactory;
    using ICSharpCode.NRefactory.Ast;
    using ICSharpCode.NRefactory.Visitors;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using UnityEditor;
    using UnityEditor.Modules;
    using UnityEditor.Scripting;

    internal class CSharpLanguage : SupportedLanguage
    {
        public override ScriptCompilerBase CreateCompiler(MonoIsland island, bool buildingForEditor, BuildTarget targetPlatform, bool runUpdater)
        {
            switch (GetCSharpCompiler(targetPlatform, buildingForEditor, island._output))
            {
                case CSharpCompiler.Microsoft:
                    return new MicrosoftCSharpCompiler(island, runUpdater);
            }
            return new MonoCSharpCompiler(island, runUpdater);
        }

        internal static CSharpCompiler GetCSharpCompiler(BuildTarget targetPlatform, bool buildingForEditor, string assemblyName)
        {
            return ModuleManager.GetCompilationExtension(ModuleManager.GetTargetStringFromBuildTarget(targetPlatform)).GetCsCompiler(buildingForEditor, assemblyName);
        }

        public override string GetExtensionICanCompile()
        {
            return "cs";
        }

        public override string GetLanguageName()
        {
            return "CSharp";
        }

        public override string GetNamespace(string fileName, string definedSymbols)
        {
            using (IParser parser = ParserFactory.CreateParser(fileName))
            {
                char[] separator = new char[] { ',' };
                HashSet<string> set = new HashSet<string>(definedSymbols.Split(separator, StringSplitOptions.RemoveEmptyEntries));
                foreach (string str in set)
                {
                    parser.Lexer.ConditionalCompilationSymbols.Add(str, string.Empty);
                }
                parser.Lexer.EvaluateConditionalCompilation = true;
                parser.Parse();
                try
                {
                    NamespaceVisitor visitor = new NamespaceVisitor();
                    VisitorData data = new VisitorData {
                        TargetClassName = Path.GetFileNameWithoutExtension(fileName)
                    };
                    parser.CompilationUnit.AcceptVisitor(visitor, data);
                    return (!string.IsNullOrEmpty(data.DiscoveredNamespace) ? data.DiscoveredNamespace : string.Empty);
                }
                catch
                {
                }
            }
            return string.Empty;
        }

        private class NamespaceVisitor : AbstractAstVisitor
        {
            public override object VisitNamespaceDeclaration(NamespaceDeclaration namespaceDeclaration, object data)
            {
                CSharpLanguage.VisitorData data2 = (CSharpLanguage.VisitorData) data;
                data2.CurrentNamespaces.Push(namespaceDeclaration.Name);
                namespaceDeclaration.AcceptChildren(this, data2);
                data2.CurrentNamespaces.Pop();
                return null;
            }

            public override object VisitTypeDeclaration(TypeDeclaration typeDeclaration, object data)
            {
                CSharpLanguage.VisitorData data2 = (CSharpLanguage.VisitorData) data;
                if (typeDeclaration.Name == data2.TargetClassName)
                {
                    string str = string.Empty;
                    foreach (string str2 in data2.CurrentNamespaces)
                    {
                        if (str == string.Empty)
                        {
                            str = str2;
                        }
                        else
                        {
                            str = str2 + "." + str;
                        }
                    }
                    data2.DiscoveredNamespace = str;
                }
                return null;
            }
        }

        private class VisitorData
        {
            public Stack<string> CurrentNamespaces = new Stack<string>();
            public string DiscoveredNamespace;
            public string TargetClassName;
        }
    }
}

