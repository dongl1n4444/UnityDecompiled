namespace Unity.IL2CPP
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;

    public class MetadataUsage
    {
        private readonly HashSet<FieldReference> _fieldInfos = new HashSet<FieldReference>(new FieldReferenceComparer());
        private readonly HashSet<MethodReference> _inflatedMethods = new HashSet<MethodReference>(new MethodReferenceComparer());
        private readonly HashSet<string> _stringLiterals = new HashSet<string>();
        private readonly HashSet<TypeReference> _typeInfos = new HashSet<TypeReference>(new TypeReferenceEqualityComparer());
        private readonly HashSet<TypeReference> _types = new HashSet<TypeReference>(new TypeReferenceEqualityComparer());

        public void AddFieldInfo(FieldReference field)
        {
            this._fieldInfos.Add(field);
        }

        public void AddIl2CppType(TypeReference type)
        {
            this._types.Add(type);
        }

        public void AddInflatedMethod(MethodReference method)
        {
            this._inflatedMethods.Add(method);
        }

        public void AddStringLiteral(string literal)
        {
            this._stringLiterals.Add(literal);
        }

        public void AddTypeInfo(TypeReference type)
        {
            this._typeInfos.Add(type);
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

        public int UsageCount
        {
            get
            {
                return ((((this._types.Count + this._typeInfos.Count) + this._inflatedMethods.Count) + this._fieldInfos.Count) + this._stringLiterals.Count);
            }
        }

        public bool UsesMetadata
        {
            get
            {
                return ((((this._types.Count > 0) || (this._typeInfos.Count > 0)) || ((this._inflatedMethods.Count > 0) || (this._fieldInfos.Count > 0))) || (this._stringLiterals.Count > 0));
            }
        }
    }
}

