namespace UnityEditor.TestTools.TestRunner.GUI
{
    using Mono.Cecil;
    using Mono.Cecil.Cil;
    using Mono.Cecil.Mdb;
    using Mono.Collections.Generic;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Text.RegularExpressions;
    using UnityEditor;
    using UnityEditor.Utils;
    using UnityEngine;

    internal static class GuiHelper
    {
        [CompilerGenerated]
        private static Func<MethodDefinition, bool> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<Instruction, bool> <>f__am$cache1;
        private const string k_LineGroup = "line";
        private const string k_PathGroup = "path";
        private const string k_StacktraceParserRegexp = @"\(at (?<path>.*):(?<line>\d+)\)";

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

        private static SequencePoint GetSequencePointOfTest(System.Type type, MethodInfo m)
        {
            <GetSequencePointOfTest>c__AnonStorey0 storey = new <GetSequencePointOfTest>c__AnonStorey0 {
                type = type,
                m = m
            };
            ReaderParameters parameters = new ReaderParameters {
                ReadSymbols = true,
                SymbolReaderProvider = new MdbReaderProvider(),
                ReadingMode = ReadingMode.Deferred
            };
            TypeDefinition baseType = Enumerable.Single<TypeDefinition>(CollectTypeDefinitions(AssemblyDefinition.ReadAssembly(storey.type.Assembly.Location, parameters).MainModule.Types), new Func<TypeDefinition, bool>(storey.<>m__0));
            MethodDefinition definition3 = null;
            while ((baseType != null) && (baseType.BaseType != null))
            {
                Collection<MethodDefinition> methods = baseType.Methods;
                if (storey.m != null)
                {
                    if (Enumerable.Any<MethodDefinition>(methods, new Func<MethodDefinition, bool>(storey.<>m__1)))
                    {
                        definition3 = Enumerable.First<MethodDefinition>(baseType.Methods, new Func<MethodDefinition, bool>(storey.<>m__2));
                        break;
                    }
                }
                else
                {
                    if (<>f__am$cache0 == null)
                    {
                        <>f__am$cache0 = x => x.HasBody;
                    }
                    foreach (MethodDefinition definition4 in Enumerable.Where<MethodDefinition>(baseType.Methods, <>f__am$cache0))
                    {
                        foreach (Instruction instruction in definition4.Body.Instructions)
                        {
                            if (instruction.get_SequencePoint() != null)
                            {
                                definition3 = definition4;
                                break;
                            }
                        }
                    }
                }
                baseType = baseType.BaseType as TypeDefinition;
            }
            if ((definition3 != null) && definition3.HasBody)
            {
                if (<>f__am$cache1 == null)
                {
                    <>f__am$cache1 = i => i.get_SequencePoint() != null;
                }
                return Enumerable.First<Instruction>(definition3.Body.Instructions, <>f__am$cache1).get_SequencePoint();
            }
            return null;
        }

        public static bool OpenInEditor(string stacktrace)
        {
            if (!string.IsNullOrEmpty(stacktrace))
            {
                char[] separator = new char[] { '\n' };
                string[] strArray = stacktrace.Split(separator);
                Regex regex = new Regex(@"\(at (?<path>.*):(?<line>\d+)\)");
                foreach (string str in strArray)
                {
                    Match match = regex.Match(str);
                    string str2 = match.Groups["path"].Value;
                    string str3 = match.Groups["line"].Value;
                    if ((!string.IsNullOrEmpty(str2) && !string.IsNullOrEmpty(str3)) && File.Exists(str2))
                    {
                        OpenInEditorInternal(str2, int.Parse(str3));
                        return true;
                    }
                }
            }
            return false;
        }

        public static void OpenInEditor(System.Type type, MethodInfo method)
        {
            string path = "";
            int line = -1;
            SequencePoint sequencePointOfTest = GetSequencePointOfTest(type, method);
            if (sequencePointOfTest != null)
            {
                line = sequencePointOfTest.StartLine;
                path = sequencePointOfTest.Document.Url;
            }
            OpenInEditorInternal(Paths.UnifyDirectorySeparator(path).Substring(Paths.UnifyDirectorySeparator(Application.dataPath).Length - "Assets".Length), line);
        }

        private static void OpenInEditorInternal(string assetPath, int line)
        {
            AssetDatabase.OpenAsset(AssetDatabase.LoadMainAssetAtPath(assetPath), line);
        }

        [CompilerGenerated]
        private sealed class <GetSequencePointOfTest>c__AnonStorey0
        {
            internal MethodInfo m;
            internal System.Type type;

            internal bool <>m__0(TypeDefinition t) => 
                (t.FullName == this.type.FullName);

            internal bool <>m__1(MethodDefinition t) => 
                (t.Name == this.m.Name);

            internal bool <>m__2(MethodDefinition t) => 
                (t.Name == this.m.Name);
        }
    }
}

