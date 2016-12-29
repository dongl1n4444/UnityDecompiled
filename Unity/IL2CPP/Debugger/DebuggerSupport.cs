namespace Unity.IL2CPP.Debugger
{
    using Mono.Cecil;
    using Mono.Cecil.Cil;
    using NiceIO;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Unity.Cecil.Visitor;
    using Unity.IL2CPP;
    using Unity.IL2CPP.ILPreProcessor;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;
    using Unity.IL2CPP.Metadata;

    public class DebuggerSupport : IDebuggerSupport
    {
        private readonly HashSet<string> _documents = new HashSet<string>();
        private readonly CollectSourceDocumentsVisitor _documentsCollector;
        private readonly Dictionary<string, string> _documentToVarName = new Dictionary<string, string>();
        private int _sequencePointUid;
        private readonly Dictionary<string, string> _varNameToDocument = new Dictionary<string, string>();
        [CompilerGenerated]
        private static Func<ParameterDefinition, string> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<KeyValuePair<string, TypeReference>, string> <>f__am$cache1;
        [Inject]
        public static INamingService Naming;

        public DebuggerSupport()
        {
            this._documentsCollector = new CollectSourceDocumentsVisitor(this._documents);
        }

        public void Analyze(AssemblyDefinition assembly)
        {
            if (DebuggerOptions.Enabled)
            {
                assembly.Accept(this._documentsCollector);
            }
        }

        private string DocumentAddressFor(MethodDefinition methodDefinition)
        {
            foreach (string str in this.DocumentsFor(methodDefinition))
            {
                return ("&" + this.UniqueDocumentVarNameFor(str));
            }
            return "0";
        }

        private string DocumentAddressFor(TypeDefinition typeDefinition)
        {
            foreach (string str in DocumentsFor(typeDefinition))
            {
                return ("&" + this.UniqueDocumentVarNameFor(str));
            }
            return "0";
        }

        private IEnumerable<string> DocumentsFor(MethodDefinition methodDefinition)
        {
            CollectSourceDocumentsVisitor visitor = new CollectSourceDocumentsVisitor();
            methodDefinition.Accept(visitor);
            return visitor.Documents;
        }

        private static IEnumerable<string> DocumentsFor(TypeDefinition typeDefinition)
        {
            CollectSourceDocumentsVisitor visitor = new CollectSourceDocumentsVisitor();
            typeDefinition.Accept(visitor);
            return visitor.Documents;
        }

        public void ExtendMethodInfoInitializer(FieldInitializers initializers, MethodReference methodReference, bool isGeneric)
        {
            initializers.Add("debug_info", !isGeneric ? ("&" + Naming.ForDebugMethodInfo(methodReference)) : "0");
        }

        public void ExtendTypeInfoInitializer(FieldInitializers initializer, TypeReference typeReference)
        {
            initializer.Add("debug_info", "&" + Naming.ForDebugTypeInfos(typeReference));
        }

        public void GenerateSupportFilesIfNeeded(NPath outputDir)
        {
            string[] append = new string[] { "document-debug-info.h" };
            using (SourceCodeWriter writer = new SourceCodeWriter(outputDir.Combine(append)))
            {
                this.WriteDocumentDebugInfoHeader(writer);
            }
            string[] textArray2 = new string[] { "document-debug-info.cpp" };
            using (SourceCodeWriter writer2 = new SourceCodeWriter(outputDir.Combine(textArray2)))
            {
                this.WriteDocumentDebugInfoSource(writer2);
            }
        }

        private static string LocalsAddressArrayFor(MethodDefinition methodDefinition)
        {
            if (!methodDefinition.HasBody || (methodDefinition.Body.Variables.Count == 0))
            {
                return "0";
            }
            return "__dbg_locals";
        }

        private static string ParamsAddressArrayFor(MethodDefinition methodDefinition) => 
            (methodDefinition.HasParameters ? "__dbg_params" : "0");

        private static string Quote(object val) => 
            $""{val}"";

        private static string StringifyPath(string path)
        {
            string str = path.Replace(@"\", @"\\");
            if (!str.EndsWith(@"\"))
            {
                str = str + @"\\";
            }
            return str;
        }

        private static string ThisAddressFor(MethodDefinition methodDefinition) => 
            (!methodDefinition.IsStaticConstructor() ? "&__this" : "&__dummy_this");

        private string UniqueDocumentVarNameFor(string document)
        {
            string str;
            if (!this._documentToVarName.TryGetValue(document, out str))
            {
                int num = 0;
                string str2 = Path.GetFileNameWithoutExtension(document).ToLower();
                do
                {
                    str = $"{str2}_{num++}_dbgdoc";
                }
                while (this._varNameToDocument.ContainsKey(str));
                this._documentToVarName[document] = str;
                this._varNameToDocument[str] = document;
            }
            return str;
        }

        public void WriteCallStackInformation(CppCodeWriter writer, MethodReference methodReference, IEnumerable<KeyValuePair<string, TypeReference>> locals, IRuntimeMetadataAccess metadataAccess)
        {
            this._sequencePointUid = 0;
            KeyValuePair<string, TypeReference>[] localvars = locals.ToArray<KeyValuePair<string, TypeReference>>();
            MethodDefinition methodDefinition = methodReference.Resolve();
            WriteDummyNullThisIfNeeded(writer, methodDefinition);
            WriteParamsAddressArray(writer, methodDefinition);
            WriteLocalsAddressArray(writer, methodDefinition, localvars);
            object[] args = new object[] { metadataAccess.MethodInfo(methodReference), ThisAddressFor(methodDefinition), ParamsAddressArrayFor(methodDefinition), methodDefinition.Parameters.Count, LocalsAddressArrayFor(methodDefinition), localvars.Length };
            writer.WriteLine("StackTraceSentry _stackTraceSentry(&{0}, {1}, {2}, {3}, {4}, {5});", args);
        }

        public void WriteDebugBreak(CppCodeWriter writer)
        {
            writer.WriteLine("il2cpp_debugger_notify_user_break();");
        }

        private void WriteDebugDocumentDeclarationFor(CppCodeWriter writer, string document)
        {
            object[] args = new object[] { this.UniqueDocumentVarNameFor(document) };
            writer.WriteLine("Il2CppDebugDocument {0} = ", args);
            object[] values = new object[] { Quote(StringifyPath(Path.GetDirectoryName(document))), Quote(Path.GetFileName(document)) };
            writer.WriteArrayInitializer(values);
        }

        private void WriteDebugDocumentDefinitionFor(Unity.IL2CPP.CodeWriter writer, string document)
        {
            object[] args = new object[] { this.UniqueDocumentVarNameFor(document), document };
            writer.WriteLine("extern Il2CppDebugDocument {0}; // {1}", args);
        }

        public void WriteDebugIncludes(CppCodeWriter writer)
        {
            writer.AddInclude("il2cpp-debugger.h");
        }

        private void WriteDebugLocalAndMethodInfoFor(CppCodeWriter writer, MethodReference methodReference, MethodDefinition methodDefinition)
        {
            WriteOffsetTable(writer, methodReference, methodDefinition);
            WriteDebugLocalInfos(writer, methodReference, methodDefinition);
            List<SequencePointInfo> sequencePoints = new List<SequencePointInfo>();
            string variableName = $"{Naming.ForDebugMethodInfo(methodReference)}_sp";
            WriteSequencePointsTableIfNeeded(writer, variableName, methodDefinition, sequencePoints);
            this.WriteDebugMethodInfos(writer, variableName, sequencePoints, methodReference, methodDefinition);
        }

        private static void WriteDebugLocalInfos(CppCodeWriter writer, MethodReference methodReference, MethodDefinition methodDefinition)
        {
            <WriteDebugLocalInfos>c__AnonStorey1 storey = new <WriteDebugLocalInfos>c__AnonStorey1 {
                methodReference = methodReference
            };
            foreach (VariableDefinition definition in methodDefinition.Body.Variables)
            {
                for (TypeSpecification specification = Unity.IL2CPP.ILPreProcessor.TypeResolver.For(storey.methodReference.DeclaringType).ResolveVariableType(storey.methodReference, definition) as TypeSpecification; specification != null; specification = specification.ElementType as TypeSpecification)
                {
                }
                object[] args = new object[] { Naming.ForDebugMethodLocalInfo(definition, storey.methodReference) };
                writer.WriteLine("Il2CppDebugLocalsInfo {0} = ", args);
                object[] values = new object[4];
                values[0] = Naming.Null;
                if (definition.Name == null)
                {
                }
                values[1] = Quote(definition.Index.ToString());
                values[2] = 0;
                values[3] = 0;
                writer.WriteArrayInitializer(values);
            }
            writer.WriteArrayInitializer("const Il2CppDebugLocalsInfo*", Naming.ForDebugLocalInfo(storey.methodReference), methodDefinition.Body.Variables.Select<VariableDefinition, string>(new Func<VariableDefinition, string>(storey, (IntPtr) this.<>m__0)), true);
        }

        public void WriteDebugMetadataIncludes(CppCodeWriter writer)
        {
            writer.AddInclude("document-debug-info.h");
        }

        public void WriteDebugMethodInfoDefinitionFor(CppCodeWriter writer, MethodReference methodReference)
        {
            MethodDefinition methodDefinition = methodReference.Resolve();
            if (methodDefinition.HasBody)
            {
                this.WriteDebugLocalAndMethodInfoFor(writer, methodReference, methodDefinition);
            }
            else
            {
                this.WriteEmptyDebugMethodInfoFor(writer, methodReference, methodDefinition);
            }
        }

        private void WriteDebugMethodInfos(CppCodeWriter writer, string variableName, ICollection sequencePoints, MethodReference methodReference, MethodDefinition methodDefinition)
        {
            object[] args = new object[] { Naming.ForDebugMethodInfo(methodReference) };
            writer.WriteLine("Il2CppDebugMethodInfo {0} = ", args);
            object[] values = new object[] { this.DocumentAddressFor(methodDefinition), Naming.ForDebugMethodInfoOffsetTable(methodReference), methodDefinition.Body.CodeSize, Naming.ForDebugLocalInfo(methodReference), sequencePoints.Count, 0, (sequencePoints.Count != 0) ? variableName : "0" };
            writer.WriteArrayInitializer(values);
        }

        public void WriteDebugTypeInfosDeclarationFor(CppCodeWriter writer, TypeReference typeRe)
        {
            object[] args = new object[] { Naming.ForDebugTypeInfos(typeRe) };
            writer.Write("extern Il2CppDebugTypeInfo {0};", args);
        }

        public void WriteDebugTypeInfosDefinitionFor(CppCodeWriter writer, TypeReference typeReference)
        {
            TypeDefinition typeDefinition = typeReference.Resolve();
            object[] args = new object[] { Naming.ForDebugTypeInfos(typeReference), this.DocumentAddressFor(typeDefinition) };
            writer.WriteLine("Il2CppDebugTypeInfo {0} = {{ {1} }};", args);
        }

        private void WriteDocumentDebugInfoHeader(CppCodeWriter writer)
        {
            writer.WriteLine("#pragma once");
            writer.WriteLine();
            writer.AddInclude("il2cpp-config.h");
            writer.AddInclude("class-internals.h");
            writer.WriteLine();
            foreach (string str in this._documents)
            {
                this.WriteDebugDocumentDefinitionFor(writer, str);
            }
        }

        private void WriteDocumentDebugInfoSource(CppCodeWriter writer)
        {
            writer.AddInclude("document-debug-info.h");
            writer.WriteLine();
            foreach (string str in this._documents)
            {
                this.WriteDebugDocumentDeclarationFor(writer, str);
            }
        }

        private static void WriteDummyNullThisIfNeeded(CppCodeWriter writer, MethodDefinition methodDefinition)
        {
            if (methodDefinition.IsStaticConstructor())
            {
                writer.WriteLine("void *__dummy_this = 0;");
            }
        }

        private void WriteEmptyDebugMethodInfoFor(CppCodeWriter writer, MethodReference methodReference, MethodDefinition methodDefinition)
        {
            object[] args = new object[] { Naming.ForDebugMethodInfo(methodReference) };
            writer.WriteLine("Il2CppDebugMethodInfo {0} = ", args);
            object[] values = new object[] { this.DocumentAddressFor(methodDefinition), 0, 0, 0 };
            writer.WriteArrayInitializer(values);
        }

        private static void WriteLocalsAddressArray(CppCodeWriter writer, MethodDefinition methodDefinition, IEnumerable<KeyValuePair<string, TypeReference>> localvars)
        {
            if (methodDefinition.HasBody && (methodDefinition.Body.Variables.Count != 0))
            {
                if (<>f__am$cache1 == null)
                {
                    <>f__am$cache1 = new Func<KeyValuePair<string, TypeReference>, string>(null, (IntPtr) <WriteLocalsAddressArray>m__1);
                }
                writer.WriteArrayInitializer("void*", "__dbg_locals", localvars.Select<KeyValuePair<string, TypeReference>, string>(<>f__am$cache1), true);
            }
        }

        private static void WriteOffsetTable(CppCodeWriter writer, MethodReference methodReference, MethodDefinition methodDefinition)
        {
            <WriteOffsetTable>c__AnonStorey2 storey = new <WriteOffsetTable>c__AnonStorey2 {
                writer = writer
            };
            object[] args = new object[] { Naming.ForDebugMethodInfoOffsetTable(methodReference) };
            storey.writer.WriteLine("const int32_t {0}[] = ", args);
            storey.writer.BeginBlock();
            methodDefinition.Accept(new SequencePointsMappingVisitor(new Action<Instruction, SequencePoint>(storey, (IntPtr) this.<>m__0)));
            storey.writer.WriteLine("-1, -1");
            storey.writer.EndBlock(true);
        }

        private static void WriteParamsAddressArray(CppCodeWriter writer, IMethodSignature methodDefinition)
        {
            if (methodDefinition.HasParameters)
            {
                if (<>f__am$cache0 == null)
                {
                    <>f__am$cache0 = new Func<ParameterDefinition, string>(null, (IntPtr) <WriteParamsAddressArray>m__0);
                }
                writer.WriteArrayInitializer("void*", "__dbg_params", methodDefinition.Parameters.Select<ParameterDefinition, string>(<>f__am$cache0), true);
            }
        }

        public void WriteSequencePoint(CppCodeWriter writer, Instruction instruction, bool injected)
        {
            object[] args = new object[] { !injected ? this._sequencePointUid++ : -1, instruction.Offset };
            writer.WriteLine("il2cpp_debugger_sequence_point_hit({0}, {1});", args);
        }

        private static void WriteSequencePointsTableIfNeeded(CppCodeWriter writer, string variableName, MethodDefinition methodDefinition, ICollection<SequencePointInfo> sequencePoints)
        {
            <WriteSequencePointsTableIfNeeded>c__AnonStorey0 storey = new <WriteSequencePointsTableIfNeeded>c__AnonStorey0 {
                sequencePoints = sequencePoints,
                methodDefinition = methodDefinition
            };
            storey.methodDefinition.Accept(new SequencePointsMappingVisitor(new Action<Instruction, SequencePoint>(storey, (IntPtr) this.<>m__0)));
            if (storey.sequencePoints.Count != 0)
            {
                object[] args = new object[] { variableName };
                writer.WriteLine("static SequencePointRecord {0}[] = ", args);
                writer.WriteArrayInitializer(storey.sequencePoints.Select<SequencePointInfo, string>(new Func<SequencePointInfo, string>(storey, (IntPtr) this.<>m__1)).ToArray<string>());
            }
        }

        [CompilerGenerated]
        private sealed class <WriteDebugLocalInfos>c__AnonStorey1
        {
            internal MethodReference methodReference;

            internal string <>m__0(VariableDefinition v) => 
                DebuggerSupport.Naming.AddressOf(DebuggerSupport.Naming.ForDebugMethodLocalInfo(v, this.methodReference));
        }

        [CompilerGenerated]
        private sealed class <WriteOffsetTable>c__AnonStorey2
        {
            internal CppCodeWriter writer;

            internal void <>m__0(Instruction i, SequencePoint s)
            {
                object[] args = new object[] { i.Offset, s.StartLine };
                this.writer.WriteLine("{0}, {1}, ", args);
            }
        }

        [CompilerGenerated]
        private sealed class <WriteSequencePointsTableIfNeeded>c__AnonStorey0
        {
            internal MethodDefinition methodDefinition;
            internal ICollection<SequencePointInfo> sequencePoints;

            internal void <>m__0(Instruction i, SequencePoint s)
            {
                SequencePointInfo item = new SequencePointInfo {
                    Instruction = i,
                    SequencePoint = s
                };
                SequencePointInfo info3 = this.sequencePoints.LastOrDefault<SequencePointInfo>();
                if (info3 != null)
                {
                    info3.NextSequencePoint = item;
                }
                this.sequencePoints.Add(item);
            }

            internal string <>m__1(SequencePointInfo info) => 
                $"{{ {info.Instruction.Offset}, {((info.NextSequencePoint != null) ? (info.NextSequencePoint.Instruction.Offset - 1) : this.methodDefinition.Body.CodeSize)}, NULL }}";
        }
    }
}

