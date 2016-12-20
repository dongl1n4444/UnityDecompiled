namespace Unity.IL2CPP.Debugger
{
    using Mono.Cecil;
    using Mono.Cecil.Cil;
    using NiceIO;
    using System;
    using System.Collections.Generic;
    using Unity.IL2CPP;
    using Unity.IL2CPP.Metadata;

    internal sealed class DisabledDebuggerSupport : IDebuggerSupport
    {
        public void Analyze(AssemblyDefinition assemblyDefinition)
        {
        }

        public void ExtendMethodInfoInitializer(FieldInitializers initializers, MethodReference methodReference, bool isGeneric)
        {
        }

        public void ExtendTypeInfoInitializer(FieldInitializers initializer, TypeReference typeReference)
        {
        }

        public void GenerateSupportFilesIfNeeded(NPath outputDir)
        {
        }

        public void WriteCallStackInformation(CppCodeWriter writer, MethodReference methodReference, IEnumerable<KeyValuePair<string, TypeReference>> locals, IRuntimeMetadataAccess metadataAccess)
        {
        }

        public void WriteDebugBreak(CppCodeWriter writer)
        {
        }

        public void WriteDebugIncludes(CppCodeWriter writer)
        {
        }

        public void WriteDebugMetadataIncludes(CppCodeWriter writer)
        {
        }

        public void WriteDebugMethodInfoDefinitionFor(CppCodeWriter writer, MethodReference methodReference)
        {
        }

        public void WriteDebugTypeInfosDeclarationFor(CppCodeWriter writer, TypeReference typeReference)
        {
        }

        public void WriteDebugTypeInfosDefinitionFor(CppCodeWriter writer, TypeReference typeReference)
        {
        }

        public void WriteSequencePoint(CppCodeWriter writer, Instruction instruction, bool injected)
        {
        }
    }
}

