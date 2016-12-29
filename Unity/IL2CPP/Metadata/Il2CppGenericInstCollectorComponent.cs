namespace Unity.IL2CPP.Metadata
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Unity.IL2CPP;
    using Unity.IL2CPP.Common;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;

    public class Il2CppGenericInstCollectorComponent : IIl2CppGenericInstCollectorWriterService, IIl2CppGenericInstCollectorReaderService, IDisposable
    {
        private readonly Dictionary<TypeReference[], uint> _data = new Dictionary<TypeReference[], uint>(new Il2CppGenericInstComparer());
        [Inject]
        public static IIl2CppTypeCollectorWriterService Il2CppTypeCollector;

        public void Add(IList<TypeReference> types)
        {
            TypeReference[] key = types.ToArray<TypeReference>();
            object obj2 = this._data;
            lock (obj2)
            {
                if (this._data.ContainsKey(key) || !MetadataCacheWriter.TypesDoNotExceedMaximumRecursion(types))
                {
                    return;
                }
                this._data.Add(key, (uint) this._data.Count);
            }
            foreach (TypeReference reference in key)
            {
                Il2CppTypeCollector.Add(reference, 0);
            }
        }

        public void Dispose()
        {
            this._data.Clear();
        }

        public IDictionary<TypeReference[], uint> Items =>
            this._data;

        internal class Il2CppGenericInstComparer : EqualityComparer<TypeReference[]>
        {
            public override bool Equals(TypeReference[] x, TypeReference[] y)
            {
                if (x.Length != y.Length)
                {
                    return false;
                }
                for (int i = 0; i < x.Length; i++)
                {
                    if (!Unity.IL2CPP.Common.TypeReferenceEqualityComparer.AreEqual(x[i], y[i], TypeComparisonMode.Exact))
                    {
                        return false;
                    }
                }
                return true;
            }

            public override int GetHashCode(TypeReference[] obj)
            {
                int length = obj.Length;
                for (int i = 0; i < obj.Length; i++)
                {
                    length = (length * 0x1cfaa2db) + Unity.IL2CPP.Common.TypeReferenceEqualityComparer.GetHashCodeFor(obj[i]);
                }
                return length;
            }
        }
    }
}

