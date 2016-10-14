using Mono.Cecil;
using System;
using Unity.IL2CPP.Common;

namespace Unity.IL2CPP.IoCServices
{
	public interface IIl2CppFieldReferenceCollectorReaderService
	{
		ReadOnlyDictionary<FieldReference, uint> Fields
		{
			get;
		}
	}
}
