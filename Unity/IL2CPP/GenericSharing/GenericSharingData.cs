namespace Unity.IL2CPP.GenericSharing
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Runtime.CompilerServices;
    using Unity.IL2CPP.Common;

    public class GenericSharingData
    {
        private readonly Dictionary<MethodDefinition, GenericContextUsage> _genericsSharings;
        private readonly List<RuntimeGenericData> _rgctxs;

        public GenericSharingData()
        {
            this._genericsSharings = new Dictionary<MethodDefinition, GenericContextUsage>();
            this._rgctxs = new List<RuntimeGenericData>();
        }

        private GenericSharingData(IDictionary<MethodDefinition, GenericContextUsage> genericSharings, IEnumerable<RuntimeGenericData> rgctxs)
        {
            this._genericsSharings = new Dictionary<MethodDefinition, GenericContextUsage>();
            this._rgctxs = new List<RuntimeGenericData>();
            this._genericsSharings = new Dictionary<MethodDefinition, GenericContextUsage>(genericSharings);
            this._rgctxs = new List<RuntimeGenericData>(rgctxs);
        }

        public void AddData(RuntimeGenericInflatedTypeData data)
        {
            <AddData>c__AnonStorey0 storey = new <AddData>c__AnonStorey0 {
                data = data
            };
            if (this._rgctxs.FindIndex(new Predicate<RuntimeGenericData>(storey.<>m__0)) == -1)
            {
                this._rgctxs.Add(storey.data);
            }
        }

        public void AddMethodUsage(MethodReference data, MethodReference genericMethod)
        {
            this._rgctxs.Add(new RuntimeGenericMethodData(RuntimeGenericContextInfo.Method, data, genericMethod));
        }

        public GenericSharingData Clone() => 
            new GenericSharingData(this._genericsSharings, this._rgctxs);

        public ReadOnlyCollection<RuntimeGenericData> RuntimeGenericDatas =>
            this._rgctxs.AsReadOnly();

        [CompilerGenerated]
        private sealed class <AddData>c__AnonStorey0
        {
            internal RuntimeGenericInflatedTypeData data;

            internal bool <>m__0(RuntimeGenericData d) => 
                ((d.InfoType == this.data.InfoType) && Unity.IL2CPP.Common.TypeReferenceEqualityComparer.AreEqual(((RuntimeGenericTypeData) d).GenericType, this.data.GenericType, TypeComparisonMode.Exact));
        }
    }
}

