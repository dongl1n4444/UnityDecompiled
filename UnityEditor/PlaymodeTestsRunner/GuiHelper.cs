namespace UnityEditor.PlaymodeTestsRunner
{
    using Mono.Cecil;
    using Mono.Cecil.Cil;
    using Mono.Cecil.Mdb;
    using Mono.Collections.Generic;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEngine;

    internal static class GuiHelper
    {
        [CompilerGenerated]
        private static Func<MethodDefinition, bool> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<Instruction, bool> <>f__am$cache1;
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

        private static SequencePoint GetSequencePointOfTest(System.Type type, MethodInfo m)
        {
            <GetSequencePointOfTest>c__AnonStorey0 storey = new <GetSequencePointOfTest>c__AnonStorey0 {
                type = type,
                m = m
            };
            ReaderParameters parameters = new ReaderParameters {
                ReadSymbols = true,
                SymbolReaderProvider = new MdbReaderProvider(),
                ReadingMode = ReadingMode.Immediate
            };
            TypeDefinition baseType = Enumerable.Single<TypeDefinition>(CollectTypeDefinitions(AssemblyDefinition.ReadAssembly(storey.type.Assembly.Location, parameters).MainModule.Types), new Func<TypeDefinition, bool>(storey, (IntPtr) this.<>m__0));
            MethodDefinition definition3 = null;
            while ((baseType != null) && (baseType.BaseType != null))
            {
                Collection<MethodDefinition> methods = baseType.Methods;
                if (storey.m != null)
                {
                    if (Enumerable.Any<MethodDefinition>(methods, new Func<MethodDefinition, bool>(storey, (IntPtr) this.<>m__1)))
                    {
                        definition3 = Enumerable.First<MethodDefinition>(baseType.Methods, new Func<MethodDefinition, bool>(storey, (IntPtr) this.<>m__2));
                        break;
                    }
                }
                else
                {
                    if (<>f__am$cache0 == null)
                    {
                        <>f__am$cache0 = new Func<MethodDefinition, bool>(null, (IntPtr) <GetSequencePointOfTest>m__0);
                    }
                    foreach (MethodDefinition definition4 in Enumerable.Where<MethodDefinition>(baseType.Methods, <>f__am$cache0))
                    {
                        foreach (Instruction instruction in definition4.Body.Instructions)
                        {
                            if (instruction.SequencePoint != null)
                            {
                                definition3 = definition4;
                                break;
                            }
                        }
                    }
                }
                baseType = baseType.BaseType as TypeDefinition;
            }
            if (definition3 != null)
            {
                if (<>f__am$cache1 == null)
                {
                    <>f__am$cache1 = new Func<Instruction, bool>(null, (IntPtr) <GetSequencePointOfTest>m__1);
                }
                return Enumerable.First<Instruction>(definition3.Body.Instructions, <>f__am$cache1).SequencePoint;
            }
            return null;
        }

        public static void OpenInEditor(System.Type type, MethodInfo method)
        {
            string filename = "";
            int line = -1;
            SequencePoint sequencePointOfTest = GetSequencePointOfTest(type, method);
            if (sequencePointOfTest != null)
            {
                line = sequencePointOfTest.StartLine;
                filename = sequencePointOfTest.Document.Url;
            }
            OpenInEditorInternal(filename, line);
        }

        private static void OpenInEditorInternal(string filename, int line)
        {
            AssetDatabase.OpenAsset(AssetDatabase.LoadMainAssetAtPath(filename.Substring((Application.dataPath.Length - "Assets/".Length) + 1)), line);
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

