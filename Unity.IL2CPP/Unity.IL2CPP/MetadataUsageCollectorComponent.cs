using Mono.Cecil;
using System;
using System.Collections.Generic;
using Unity.IL2CPP.Common;
using Unity.IL2CPP.IoCServices;

namespace Unity.IL2CPP
{
	internal class MetadataUsageCollectorComponent : IMetadataUsageCollectorWriterService, IMetadataUsageCollectorReaderService, IDisposable
	{
		private readonly HashSet<TypeReference> _types = new HashSet<TypeReference>(new TypeReferenceEqualityComparer());

		private readonly HashSet<TypeReference> _typeInfos = new HashSet<TypeReference>(new TypeReferenceEqualityComparer());

		private readonly HashSet<MethodReference> _inflatedMethods = new HashSet<MethodReference>(new MethodReferenceComparer());

		private readonly HashSet<FieldReference> _fieldInfos = new HashSet<FieldReference>(new FieldReferenceComparer());

		private readonly HashSet<string> _stringLiterals = new HashSet<string>();

		private readonly Dictionary<string, MetadataUsage> _usages = new Dictionary<string, MetadataUsage>();

		public int UsageCount
		{
			get
			{
				return this._types.Count + this._typeInfos.Count + this._inflatedMethods.Count + this._fieldInfos.Count + this._stringLiterals.Count;
			}
		}

		public void Add(string identifier, MetadataUsage usage)
		{
			this._usages.Add(identifier, usage);
			foreach (TypeReference current in usage.GetIl2CppTypes())
			{
				this._types.Add(current);
			}
			foreach (TypeReference current2 in usage.GetTypeInfos())
			{
				this._typeInfos.Add(current2);
			}
			foreach (MethodReference current3 in usage.GetInflatedMethods())
			{
				this._inflatedMethods.Add(current3);
			}
			foreach (FieldReference current4 in usage.GetFieldInfos())
			{
				this._fieldInfos.Add(current4);
			}
			foreach (string current5 in usage.GetStringLiterals())
			{
				this._stringLiterals.Add(current5);
			}
		}

		public IEnumerable<TypeReference> GetTypeInfos()
		{
			return this._typeInfos;
		}

		public IEnumerable<TypeReference> GetIl2CppTypes()
		{
			return this._types;
		}

		public IEnumerable<MethodReference> GetInflatedMethods()
		{
			return this._inflatedMethods;
		}

		public IEnumerable<FieldReference> GetFieldInfos()
		{
			return this._fieldInfos;
		}

		public IEnumerable<string> GetStringLiterals()
		{
			return this._stringLiterals;
		}

		public IEnumerable<KeyValuePair<string, MetadataUsage>> GetUsages()
		{
			return this._usages;
		}

		public void Dispose()
		{
			this._types.Clear();
			this._typeInfos.Clear();
			this._inflatedMethods.Clear();
			this._fieldInfos.Clear();
			this._stringLiterals.Clear();
		}
	}
}
