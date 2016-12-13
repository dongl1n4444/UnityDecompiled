using Mono.Cecil;
using Mono.Cecil.Cil;
using NiceIO;
using System;
using System.Collections.Generic;
using Unity.IL2CPP.Metadata;

namespace Unity.IL2CPP.Debugger
{
	public interface IDebuggerSupport
	{
		void WriteDebugMetadataIncludes(CppCodeWriter writer);

		void Analyze(AssemblyDefinition assemblyDefinition);

		void GenerateSupportFilesIfNeeded(NPath outputDir);

		void ExtendTypeInfoInitializer(FieldInitializers initializer, TypeReference typeReference);

		void WriteDebugTypeInfosDeclarationFor(CppCodeWriter writer, TypeReference typeRe);

		void WriteDebugTypeInfosDefinitionFor(CppCodeWriter writer, TypeReference typeReference);

		void ExtendMethodInfoInitializer(FieldInitializers initializers, MethodReference methodReference, bool isGeneric);

		void WriteDebugMethodInfoDefinitionFor(CppCodeWriter writer, MethodReference methodReference);

		void WriteCallStackInformation(CppCodeWriter writer, MethodReference methodReference, IEnumerable<KeyValuePair<string, TypeReference>> locals, IRuntimeMetadataAccess metadataAccess);

		void WriteSequencePoint(CppCodeWriter writer, Instruction instruction, bool injected);

		void WriteDebugIncludes(CppCodeWriter writer);

		void WriteDebugBreak(CppCodeWriter writer);
	}
}
