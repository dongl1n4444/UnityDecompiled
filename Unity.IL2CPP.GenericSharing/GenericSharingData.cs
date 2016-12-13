using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Unity.IL2CPP.Common;

namespace Unity.IL2CPP.GenericSharing
{
	public class GenericSharingData
	{
		private readonly Dictionary<MethodDefinition, GenericContextUsage> _genericsSharings = new Dictionary<MethodDefinition, GenericContextUsage>();

		private readonly List<RuntimeGenericData> _rgctxs = new List<RuntimeGenericData>();

		public ReadOnlyCollection<RuntimeGenericData> RuntimeGenericDatas
		{
			get
			{
				return this._rgctxs.AsReadOnly();
			}
		}

		public GenericSharingData()
		{
		}

		private GenericSharingData(IDictionary<MethodDefinition, GenericContextUsage> genericSharings, IEnumerable<RuntimeGenericData> rgctxs)
		{
			this._genericsSharings = new Dictionary<MethodDefinition, GenericContextUsage>(genericSharings);
			this._rgctxs = new List<RuntimeGenericData>(rgctxs);
		}

		public void AddData(RuntimeGenericInflatedTypeData data)
		{
			if (this._rgctxs.FindIndex((RuntimeGenericData d) => d.InfoType == data.InfoType && TypeReferenceEqualityComparer.AreEqual(((RuntimeGenericTypeData)d).GenericType, data.GenericType, TypeComparisonMode.Exact)) == -1)
			{
				this._rgctxs.Add(data);
			}
		}

		public void AddMethodUsage(MethodReference data, MethodReference genericMethod)
		{
			this._rgctxs.Add(new RuntimeGenericMethodData(RuntimeGenericContextInfo.Method, data, genericMethod));
		}

		public GenericSharingData Clone()
		{
			return new GenericSharingData(this._genericsSharings, this._rgctxs);
		}
	}
}
