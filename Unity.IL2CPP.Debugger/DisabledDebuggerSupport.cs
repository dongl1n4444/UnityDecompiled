using Mono.Cecil;
using Mono.Cecil.Cil;
using NiceIO;
using System;
using System.Collections.Generic;
using Unity.IL2CPP.Metadata;

namespace Unity.IL2CPP.Debugger
{
	internal sealed class DisabledDebuggerSupport : IDebuggerSupport
	{
		public void WriteDebugMetadataIncludes(CppCodeWriter writer)
		{
		}

		public void Analyze(AssemblyDefinition assemblyDefinition)
		{
		}

		public void GenerateSupportFilesIfNeeded(NPath outputDir)
		{
		}

		public void ExtendTypeInfoInitializer(FieldInitializers initializer, TypeReference typeReference)
		{
		}

		public void WriteDebugTypeInfosDeclarationFor(CppCodeWriter writer, TypeReference typeReference)
		{
		}

		public void WriteDebugTypeInfosDefinitionFor(CppCodeWriter writer, TypeReference typeReference)
		{
		}

		public void ExtendMethodInfoInitializer(FieldInitializers initializers, MethodReference methodReference, bool isGeneric)
		{
		}

		public void WriteDebugMethodInfoDefinitionFor(CppCodeWriter writer, MethodReference methodReference)
		{
		}

		public void WriteCallStackInformation(CppCodeWriter writer, MethodReference methodReference, IEnumerable<KeyValuePair<string, TypeReference>> locals, IRuntimeMetadataAccess metadataAccess)
		{
		}

		public void WriteSequencePoint(CppCodeWriter writer, Instruction instruction, bool injected)
		{
		}

		public void WriteDebugIncludes(CppCodeWriter writer)
		{
		}

		public void WriteDebugBreak(CppCodeWriter writer)
		{
		}
	}
}
