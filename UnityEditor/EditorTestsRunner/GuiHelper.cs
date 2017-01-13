namespace UnityEditor.EditorTestsRunner
{
    using Mono.Cecil;
    using Mono.Cecil.Cil;
    using Mono.Cecil.Mdb;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text.RegularExpressions;
    using UnityEditor;
    using UnityEditor.EditorTests;
    using UnityEngine;

    internal static class GuiHelper
    {
        [CompilerGenerated]
        private static Func<Instruction, bool> <>f__am$cache0;
        private const string k_StacktraceParserRegexp = @".* in (?'path'.*):(?'line'\d+)";

        private static IEnumerable<TypeDefinition> CollectTypeDefinitions(IEnumerable<TypeDefinition> collection)
        {
            List<TypeDefinition> list = collection.ToList<TypeDefinition>();
            foreach (TypeDefinition definition in collection)
            {
                if (definition.HasNestedTypes)
                {
                    list.AddRange(CollectTypeDefinitions(definition.NestedTypes));
                }
            }
            return list;
        }

        private static void CopyStream(Stream srcStream, Stream destStream)
        {
            byte[] buffer = new byte[0x80];
            for (int i = srcStream.Read(buffer, 0, 0x80); i > 0; i = srcStream.Read(buffer, 0, 0x80))
            {
                destStream.Write(buffer, 0, i);
            }
        }

        private static int ExtractSourceFileLine(string stackTrace)
        {
            int num = 0;
            if (!string.IsNullOrEmpty(stackTrace))
            {
                MatchCollection matchs = new Regex(@".* in (?'path'.*):(?'line'\d+)").Matches(stackTrace);
                for (int i = 0; i < matchs.Count; i++)
                {
                    num = int.Parse(matchs[i].Groups["line"].Value);
                    if (num != 0)
                    {
                        return num;
                    }
                }
            }
            return num;
        }

        private static string ExtractSourceFilePath(string stackTrace)
        {
            string str = "";
            if (!string.IsNullOrEmpty(stackTrace))
            {
                MatchCollection matchs = new Regex(@".* in (?'path'.*):(?'line'\d+)").Matches(stackTrace);
                for (int i = 0; i < matchs.Count; i++)
                {
                    str = matchs[i].Groups["path"].Value;
                    if (str != "<filename unknown>")
                    {
                        return str;
                    }
                }
            }
            return str;
        }

        public static Texture GetIconForResult(TestResultState? resultState)
        {
            if (resultState.HasValue)
            {
                switch (resultState.Value)
                {
                    case TestResultState.Inconclusive:
                    case TestResultState.NotRunnable:
                    case TestResultState.Cancelled:
                        return Icons.s_InconclusiveImg;

                    case TestResultState.Skipped:
                    case TestResultState.Ignored:
                        return Icons.s_IgnoreImg;

                    case TestResultState.Success:
                        return Icons.s_SuccessImg;

                    case TestResultState.Failure:
                    case TestResultState.Error:
                        return Icons.s_FailImg;
                }
                return Icons.s_UnknownImg;
            }
            return Icons.s_UnknownImg;
        }

        private static SequencePoint GetSequencePointOfTest(EditorTestResult test)
        {
            <GetSequencePointOfTest>c__AnonStorey0 storey = new <GetSequencePointOfTest>c__AnonStorey0 {
                test = test
            };
            ReaderParameters parameters = new ReaderParameters {
                ReadSymbols = true,
                SymbolReaderProvider = new MdbReaderProvider(),
                ReadingMode = ReadingMode.Deferred
            };
            AssemblyDefinition definition = AssemblyDefinition.ReadAssembly(storey.test.test.assemblyPath, parameters);
            storey.testName = storey.test.test.fullClassName.Replace("+", "/");
            TypeDefinition baseType = Enumerable.Single<TypeDefinition>(CollectTypeDefinitions(definition.MainModule.Types), new Func<TypeDefinition, bool>(storey.<>m__0));
            MethodDefinition definition3 = null;
            while (baseType.BaseType != null)
            {
                if (Enumerable.Any<MethodDefinition>(baseType.Methods, new Func<MethodDefinition, bool>(storey.<>m__1)))
                {
                    definition3 = Enumerable.First<MethodDefinition>(baseType.Methods, new Func<MethodDefinition, bool>(storey.<>m__2));
                    break;
                }
                baseType = baseType.BaseType as TypeDefinition;
            }
            if (definition3 != null)
            {
                if (<>f__am$cache0 == null)
                {
                    <>f__am$cache0 = i => i.SequencePoint != null;
                }
                return Enumerable.First<Instruction>(definition3.Body.Instructions, <>f__am$cache0).SequencePoint;
            }
            return null;
        }

        public static void OpenInEditor(EditorTestResult test, bool openError)
        {
            string url = ExtractSourceFilePath(test.stackTrace);
            int line = ExtractSourceFileLine(test.stackTrace);
            if ((!openError || (line == 0)) || string.IsNullOrEmpty(url))
            {
                SequencePoint sequencePointOfTest = GetSequencePointOfTest(test);
                if (sequencePointOfTest != null)
                {
                    line = sequencePointOfTest.StartLine;
                    url = sequencePointOfTest.Document.Url;
                }
            }
            OpenInEditorInternal(url, line);
        }

        private static void OpenInEditorInternal(string filename, int line)
        {
            AssetDatabase.OpenAsset(AssetDatabase.LoadMainAssetAtPath(filename.Substring((Application.dataPath.Length - "Assets/".Length) + 1)), line);
        }

        [CompilerGenerated]
        private sealed class <GetSequencePointOfTest>c__AnonStorey0
        {
            internal EditorTestResult test;
            internal string testName;

            internal bool <>m__0(TypeDefinition t) => 
                (t.FullName == this.testName);

            internal bool <>m__1(MethodDefinition t) => 
                (t.Name == this.test.test.methodName);

            internal bool <>m__2(MethodDefinition t) => 
                (t.Name == this.test.test.methodName);
        }
    }
}

