namespace Unity.IL2CPP.Debugger
{
    using Mono.Cecil;
    using Mono.Cecil.Cil;
    using NiceIO;
    using System;
    using System.Collections.Generic;
    using Unity.IL2CPP;
    using Unity.IL2CPP.Metadata;

    public interface IDebuggerSupport
    {
        void Analyze(AssemblyDefinition assemblyDefinition);
        void ExtendMethodInfoInitializer(FieldInitializers initializers, MethodReference methodReference, bool isGeneric);
        void ExtendTypeInfoInitializer(FieldInitializers initializer, TypeReference typeReference);
        void GenerateSupportFilesIfNeeded(NPath outputDir);
        void WriteCallStackInformation(CppCodeWriter writer, MethodReference methodReference, IEnumerable<KeyValuePair<string, TypeReference>> locals, IRuntimeMetadataAccess metadataAccess);
        void WriteDebugBreak(CppCodeWriter writer);
        void WriteDebugIncludes(CppCodeWriter writer);
        void WriteDebugMetadataIncludes(CppCodeWriter writer);
        void WriteDebugMethodInfoDefinitionFor(CppCodeWriter writer, MethodReference methodReference);
        void WriteDebugTypeInfosDeclarationFor(CppCodeWriter writer, TypeReference typeRe);
        void WriteDebugTypeInfosDefinitionFor(CppCodeWriter writer, TypeReference typeReference);
        void WriteSequencePoint(CppCodeWriter writer, Instruction instruction, bool injected);
    }
}

