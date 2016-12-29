namespace Unity.IL2CPP.FileNaming
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using Unity.IL2CPP;
    using Unity.IL2CPP.Common;

    public class UniqueShortNameGenerator
    {
        private readonly Dictionary<string, int> _nextShortNameSuffix = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, TypeReference> _shortNameToType = new Dictionary<string, TypeReference>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<TypeReference, string> _typeToShortName = new Dictionary<TypeReference, string>(new Unity.IL2CPP.Common.TypeReferenceEqualityComparer());
        public const int kMaxFileNameLength = 60;

        private string CreateAndCacheShortUniqueName(string nonUniqueShortName, TypeReference type)
        {
            TypeReference reference;
            string str = SemiUniqueStableTokenGenerator.GenerateFor(type).ToString();
            if ((nonUniqueShortName.Length + str.Length) > 60)
            {
                nonUniqueShortName = nonUniqueShortName.Substring(0, 60 - str.Length);
            }
            string str2 = nonUniqueShortName;
            str2 = $"{str2}{str}";
            if (this._shortNameToType.TryGetValue(str2, out reference) && !Unity.IL2CPP.Common.TypeReferenceEqualityComparer.AreEqual(reference, type, TypeComparisonMode.Exact))
            {
                str2 = this.MakeUniqueWithSuffix(nonUniqueShortName);
            }
            this._shortNameToType[str2] = type;
            this._typeToShortName[type] = str2;
            return str2;
        }

        public string GetUniqueShortName(string nonUniqueShortName, TypeReference type)
        {
            string str;
            if (this._typeToShortName.TryGetValue(type, out str))
            {
                return str;
            }
            return this.CreateAndCacheShortUniqueName(nonUniqueShortName, type);
        }

        private string MakeUniqueWithSuffix(string nonUniqueShortName)
        {
            int num;
            if (!this._nextShortNameSuffix.TryGetValue(nonUniqueShortName, out num))
            {
                num = 0;
            }
            this._nextShortNameSuffix[nonUniqueShortName] = num + 1;
            return $"{nonUniqueShortName}_{num}";
        }
    }
}

