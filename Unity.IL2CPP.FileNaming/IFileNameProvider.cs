using Mono.Cecil;
using System;

namespace Unity.IL2CPP.FileNaming
{
	public interface IFileNameProvider
	{
		string ForModule(ModuleDefinition module);

		string ForTypeDefinition(TypeReference type);

		string ForMethodDeclarations(TypeReference type);
	}
}
