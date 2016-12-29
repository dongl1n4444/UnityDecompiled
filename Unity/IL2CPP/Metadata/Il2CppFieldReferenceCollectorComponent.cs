namespace Unity.IL2CPP.Metadata
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using Unity.IL2CPP.Common;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;

    public class Il2CppFieldReferenceCollectorComponent : IIl2CppFieldReferenceCollectorWriterService, IIl2CppFieldReferenceCollectorReaderService, IDisposable
    {
        private readonly Dictionary<FieldReference, uint> _fields = new Dictionary<FieldReference, uint>();
        [Inject]
        public static IIl2CppTypeCollectorWriterService Il2CppTypeCollector;

        public void Dispose()
        {
            this._fields.Clear();
        }

        public uint GetOrCreateIndex(FieldReference field)
        {
            uint count;
            if (!this._fields.TryGetValue(field, out count))
            {
                count = (uint) this._fields.Count;
                this._fields.Add(field, count);
                Il2CppTypeCollector.Add(field.DeclaringType, 0);
            }
            return count;
        }

        public ReadOnlyDictionary<FieldReference, uint> Fields =>
            this._fields.AsReadOnly<FieldReference, uint>();
    }
}

