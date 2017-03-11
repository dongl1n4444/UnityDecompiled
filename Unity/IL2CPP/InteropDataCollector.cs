namespace Unity.IL2CPP
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;
    using Unity.IL2CPP.Metadata;
    using Unity.IL2CPP.Portability;

    public class InteropDataCollector : IInteropDataCollector, IInteropDataCollectorResults
    {
        private bool _complete;
        private readonly Dictionary<TypeReference, InteropData> _interopData = new Dictionary<TypeReference, InteropData>(new TypeReferenceEqualityComparer());
        private readonly Dictionary<MethodReference, int> _reversePInvokeWrappers = new Dictionary<MethodReference, int>();
        private readonly List<Tuple<TypeReference, string>> _windowsRuntimeTypesWithNames = new List<Tuple<TypeReference, string>>();
        [CompilerGenerated]
        private static Func<KeyValuePair<MethodReference, int>, int> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<KeyValuePair<MethodReference, int>, MethodReference> <>f__am$cache1;
        [Inject]
        public static IIl2CppTypeCollectorWriterService IIl2CppTypeCollector;
        [Inject]
        public static IStatsService Stats;

        [CompilerGenerated]
        private static int <GetReversePInvokeWrappers>m__0(KeyValuePair<MethodReference, int> kvp) => 
            kvp.Value;

        [CompilerGenerated]
        private static MethodReference <GetReversePInvokeWrappers>m__1(KeyValuePair<MethodReference, int> kvp) => 
            kvp.Key;

        private void AssertComplete()
        {
            if (!this._complete)
            {
                throw new InvalidOperationException("This method cannot be used until Complete() has been called.");
            }
        }

        private void AssertNotComplete()
        {
            if (this._complete)
            {
                throw new InvalidOperationException("Once Complete() has been called, items cannot be added");
            }
        }

        public void Complete(MetadataCollector metadataCollector)
        {
            this._complete = true;
            foreach (Tuple<TypeReference, string> tuple in this._windowsRuntimeTypesWithNames)
            {
                metadataCollector.AddString(tuple.Item2);
            }
        }

        void IInteropDataCollector.AddCCWMarshallingFunction(TypeReference type)
        {
            InteropData data;
            this.AssertNotComplete();
            IIl2CppTypeCollector.Add(type, 0);
            if (!this._interopData.TryGetValue(type, out data))
            {
                data = new InteropData(type, this._interopData.Count);
                this._interopData.Add(type, data);
            }
            data.HasCreateCCWFunction = true;
        }

        void IInteropDataCollector.AddGuid(TypeReference type)
        {
            InteropData data;
            this.AssertNotComplete();
            if (!this._interopData.TryGetValue(type, out data))
            {
                data = new InteropData(type, this._interopData.Count);
                this._interopData.Add(type, data);
            }
            data.HasGuid = true;
        }

        void IInteropDataCollector.AddReversePInvokeWrapper(MethodReference method)
        {
            this.AssertNotComplete();
            this._reversePInvokeWrappers.Add(method, this._reversePInvokeWrappers.Count);
        }

        void IInteropDataCollector.AddTypeMarshallingFunctions(TypeDefinition type)
        {
            InteropData data;
            this.AssertNotComplete();
            if (!this._interopData.TryGetValue(type, out data))
            {
                data = new InteropData(type, this._interopData.Count);
                this._interopData.Add(type, data);
            }
            data.HasPInvokeMarshalingFunctions = true;
        }

        void IInteropDataCollector.AddWindowsRuntimeTypeWithName(TypeReference type, string typeName)
        {
            this.AssertNotComplete();
            this._windowsRuntimeTypesWithNames.Add(new Tuple<TypeReference, string>(type, typeName));
            Stats.RecordWindowsRuntimeTypeWithName();
        }

        void IInteropDataCollector.AddWrapperForDelegateFromManagedToNative(MethodReference method)
        {
            InteropData data;
            this.AssertNotComplete();
            if (!this._interopData.TryGetValue(method.DeclaringType, out data))
            {
                data = new InteropData(method.DeclaringType, this._interopData.Count);
                this._interopData.Add(method.DeclaringType, data);
            }
            data.HasDelegatePInvokeWrapperMethod = true;
        }

        ReadOnlyCollection<InteropData> IInteropDataCollectorResults.GetInteropData()
        {
            this.AssertComplete();
            return this._interopData.Values.ToArray<InteropData>().AsReadOnlyPortable<InteropData>();
        }

        int IInteropDataCollectorResults.GetReversePInvokeWrapperIndex(MethodReference method)
        {
            int num;
            this.AssertComplete();
            if (this._reversePInvokeWrappers.TryGetValue(method, out num))
            {
                return num;
            }
            return -1;
        }

        ReadOnlyCollection<MethodReference> IInteropDataCollectorResults.GetReversePInvokeWrappers()
        {
            this.AssertComplete();
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = new Func<KeyValuePair<MethodReference, int>, int>(InteropDataCollector.<GetReversePInvokeWrappers>m__0);
            }
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = new Func<KeyValuePair<MethodReference, int>, MethodReference>(InteropDataCollector.<GetReversePInvokeWrappers>m__1);
            }
            return this._reversePInvokeWrappers.OrderBy<KeyValuePair<MethodReference, int>, int>(<>f__am$cache0).Select<KeyValuePair<MethodReference, int>, MethodReference>(<>f__am$cache1).ToArray<MethodReference>().AsReadOnlyPortable<MethodReference>();
        }

        ReadOnlyCollection<Tuple<TypeReference, string>> IInteropDataCollectorResults.GetWindowsRuntimeTypesWithNames()
        {
            this.AssertComplete();
            return this._windowsRuntimeTypesWithNames.AsReadOnly();
        }
    }
}

