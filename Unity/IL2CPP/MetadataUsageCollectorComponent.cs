namespace Unity.IL2CPP
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using Unity.IL2CPP.IoCServices;

    internal class MetadataUsageCollectorComponent : IMetadataUsageCollectorWriterService, IMetadataUsageCollectorReaderService, IDisposable
    {
        private readonly HashSet<FieldReference> _fieldInfos = new HashSet<FieldReference>(new FieldReferenceComparer());
        private readonly HashSet<MethodReference> _inflatedMethods = new HashSet<MethodReference>(new MethodReferenceComparer());
        private readonly HashSet<string> _stringLiterals = new HashSet<string>();
        private readonly HashSet<TypeReference> _typeInfos = new HashSet<TypeReference>(new TypeReferenceEqualityComparer());
        private readonly HashSet<TypeReference> _types = new HashSet<TypeReference>(new TypeReferenceEqualityComparer());
        private readonly Dictionary<string, MetadataUsage> _usages = new Dictionary<string, MetadataUsage>();

        public void Add(string identifier, MetadataUsage usage)
        {
            this._usages.Add(identifier, usage);
            foreach (TypeReference reference in usage.GetIl2CppTypes())
            {
                this._types.Add(reference);
            }
            foreach (TypeReference reference2 in usage.GetTypeInfos())
            {
                this._typeInfos.Add(reference2);
            }
            foreach (MethodReference reference3 in usage.GetInflatedMethods())
            {
                this._inflatedMethods.Add(reference3);
            }
            foreach (FieldReference reference4 in usage.GetFieldInfos())
            {
                this._fieldInfos.Add(reference4);
            }
            foreach (string str in usage.GetStringLiterals())
            {
                this._stringLiterals.Add(str);
            }
        }

        public void Dispose()
        {
            this._types.Clear();
            this._typeInfos.Clear();
            this._inflatedMethods.Clear();
            this._fieldInfos.Clear();
            this._stringLiterals.Clear();
        }

        public IEnumerable<FieldReference> GetFieldInfos()
        {
            return this._fieldInfos;
        }

        public IEnumerable<TypeReference> GetIl2CppTypes()
        {
            return this._types;
        }

        public IEnumerable<MethodReference> GetInflatedMethods()
        {
            return this._inflatedMethods;
        }

        public IEnumerable<string> GetStringLiterals()
        {
            return this._stringLiterals;
        }

        public IEnumerable<TypeReference> GetTypeInfos()
        {
            return this._typeInfos;
        }

        public IEnumerable<KeyValuePair<string, MetadataUsage>> GetUsages()
        {
            return this._usages;
        }

        public int UsageCount
        {
            get
            {
                return ((((this._types.Count + this._typeInfos.Count) + this._inflatedMethods.Count) + this._fieldInfos.Count) + this._stringLiterals.Count);
            }
        }
    }
}

