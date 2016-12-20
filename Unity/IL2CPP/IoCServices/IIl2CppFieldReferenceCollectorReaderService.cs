namespace Unity.IL2CPP.IoCServices
{
    using Unity.IL2CPP.Common;

    public interface IIl2CppFieldReferenceCollectorReaderService
    {
        ReadOnlyDictionary<FieldReference, uint> Fields { get; }
    }
}

