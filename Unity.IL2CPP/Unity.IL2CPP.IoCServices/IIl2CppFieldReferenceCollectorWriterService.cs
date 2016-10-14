using Mono.Cecil;
using System;

namespace Unity.IL2CPP.IoCServices
{
	public interface IIl2CppFieldReferenceCollectorWriterService
	{
		uint GetOrCreateIndex(FieldReference field);
	}
}
