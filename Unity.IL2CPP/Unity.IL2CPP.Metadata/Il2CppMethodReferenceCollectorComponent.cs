using Mono.Cecil;
using System;
using Unity.IL2CPP.IoC;
using Unity.IL2CPP.IoCServices;

namespace Unity.IL2CPP.Metadata
{
	public class Il2CppMethodReferenceCollectorComponent : IIl2CppMethodReferenceCollectorWriterService, IIl2CppMethodReferenceCollectorReaderService, IDisposable
	{
		[Inject]
		public static IIl2CppGenericMethodCollectorWriterService Il2CppGenericMethodCollectorWrite;

		[Inject]
		public static IIl2CppGenericMethodCollectorReaderService Il2CppGenericMethodCollectorRead;

		public uint GetOrCreateIndex(MethodReference method, IMetadataCollection metadataCollection)
		{
			uint result;
			if (method == null)
			{
				result = 0u;
			}
			else if (method.IsGenericInstance || method.DeclaringType.IsGenericInstance)
			{
				Il2CppMethodReferenceCollectorComponent.Il2CppGenericMethodCollectorWrite.Add(method);
				if (Il2CppMethodReferenceCollectorComponent.Il2CppGenericMethodCollectorRead.HasIndex(method))
				{
					uint num = Il2CppMethodReferenceCollectorComponent.Il2CppGenericMethodCollectorRead.GetIndex(method);
					num |= 3221225472u;
					result = num;
				}
				else
				{
					result = 3221225472u;
				}
			}
			else
			{
				result = (uint)(metadataCollection.GetMethodIndex(method.Resolve()) | 1610612736);
			}
			return result;
		}

		public void Dispose()
		{
		}
	}
}
