namespace Unity.IL2CPP.Metadata
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using Unity.IL2CPP;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;

    public class Il2CppGenericMethodCollectorComponent : IIl2CppGenericMethodCollectorWriterService, IIl2CppGenericMethodCollectorReaderService, IDisposable
    {
        private readonly Dictionary<MethodReference, uint> _data = new Dictionary<MethodReference, uint>(new MethodReferenceComparer());
        [Inject]
        public static IIl2CppGenericInstCollectorWriterService Il2CppGenericInstCollector;

        public void Add(MethodReference method)
        {
            object obj2 = this._data;
            lock (obj2)
            {
                if ((this._data.ContainsKey(method) || !MetadataCacheWriter.TypeDoesNotExceedMaximumRecursion(method.DeclaringType)) || (method.IsGenericInstance && !MetadataCacheWriter.TypesDoNotExceedMaximumRecursion(((GenericInstanceMethod) method).GenericArguments)))
                {
                    return;
                }
                this._data.Add(method, (uint) this._data.Count);
            }
            if (method.DeclaringType.IsGenericInstance)
            {
                GenericInstanceType declaringType = (GenericInstanceType) method.DeclaringType;
                Il2CppGenericInstCollector.Add(declaringType.GenericArguments);
            }
            if (method.IsGenericInstance)
            {
                GenericInstanceMethod method2 = (GenericInstanceMethod) method;
                Il2CppGenericInstCollector.Add(method2.GenericArguments);
            }
        }

        public void Dispose()
        {
            this._data.Clear();
        }

        public uint GetIndex(MethodReference method)
        {
            return this._data[method];
        }

        public bool HasIndex(MethodReference method)
        {
            return this._data.ContainsKey(method);
        }

        public IDictionary<MethodReference, uint> Items
        {
            get
            {
                return this._data;
            }
        }
    }
}

