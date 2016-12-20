namespace Unity.IL2CPP.Metadata
{
    using Mono.Cecil;
    using System;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;

    public class Il2CppMethodReferenceCollectorComponent : IIl2CppMethodReferenceCollectorWriterService, IIl2CppMethodReferenceCollectorReaderService, IDisposable
    {
        [Inject]
        public static IIl2CppGenericMethodCollectorReaderService Il2CppGenericMethodCollectorRead;
        [Inject]
        public static IIl2CppGenericMethodCollectorWriterService Il2CppGenericMethodCollectorWrite;

        public void Dispose()
        {
        }

        public uint GetOrCreateIndex(MethodReference method, IMetadataCollection metadataCollection)
        {
            if (method == null)
            {
                return 0;
            }
            if (method.IsGenericInstance || method.DeclaringType.IsGenericInstance)
            {
                Il2CppGenericMethodCollectorWrite.Add(method);
                if (Il2CppGenericMethodCollectorRead.HasIndex(method))
                {
                    return (Il2CppGenericMethodCollectorRead.GetIndex(method) | 0xc0000000);
                }
                return 0xc0000000;
            }
            return (uint) (metadataCollection.GetMethodIndex(method.Resolve()) | 0x60000000);
        }
    }
}

