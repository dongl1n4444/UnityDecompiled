namespace Unity.IL2CPP
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;

    public class Il2CppTypeCollectorComponent : IIl2CppTypeCollectorReaderService, IIl2CppTypeCollectorWriterService, IDisposable
    {
        private readonly Dictionary<Il2CppTypeData, int> _data = new Dictionary<Il2CppTypeData, int>(new Il2CppTypeDataComparer());
        [Inject]
        public static IIl2CppGenericInstCollectorWriterService Il2CppGenericInstCollector;
        [Inject]
        public static INamingService Naming;

        public void Add(TypeReference type, [Optional, DefaultParameterValue(0)] int attrs)
        {
            type = Naming.RemoveModifiers(type);
            Il2CppTypeData key = new Il2CppTypeData(type, attrs);
            object obj2 = this._data;
            lock (obj2)
            {
                if (this._data.ContainsKey(key))
                {
                    return;
                }
                this._data.Add(key, this._data.Count);
            }
            GenericInstanceType type2 = type as GenericInstanceType;
            if (type2 != null)
            {
                Il2CppGenericInstCollector.Add(type2.GenericArguments);
            }
            ArrayType type3 = type as ArrayType;
            if (type3 != null)
            {
                this.Add(type3.ElementType, 0);
            }
            ByReferenceType type4 = type as ByReferenceType;
            if (type4 != null)
            {
                this.Add(type4.ElementType, 0);
            }
        }

        public void Dispose()
        {
            this._data.Clear();
        }

        public int GetIndex(TypeReference type, [Optional, DefaultParameterValue(0)] int attrs)
        {
            int num;
            Il2CppTypeData key = new Il2CppTypeData(Naming.RemoveModifiers(type), attrs);
            if (!this._data.TryGetValue(key, out num))
            {
                throw new InvalidOperationException(string.Format("Il2CppTypeIndexFor type {0} does not exist.", type.FullName));
            }
            return num;
        }

        public int GetOrCreateIndex(TypeReference type, [Optional, DefaultParameterValue(0)] int attrs)
        {
            int num;
            Il2CppTypeData key = new Il2CppTypeData(Naming.RemoveModifiers(type), attrs);
            if (this._data.TryGetValue(key, out num))
            {
                return num;
            }
            this.Add(type, attrs);
            return this._data[key];
        }

        public IDictionary<Il2CppTypeData, int> Items
        {
            get
            {
                return this._data;
            }
        }
    }
}

