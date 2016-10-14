using Mono.Cecil;
using System;
using System.Collections.Generic;
using Unity.IL2CPP.Common;

namespace Unity.IL2CPP.FileNaming
{
	public class UniqueShortNameGenerator
	{
		public const int kMaxFileNameLength = 60;

		private readonly Dictionary<string, TypeReference> _shortNameToType = new Dictionary<string, TypeReference>(StringComparer.OrdinalIgnoreCase);

		private readonly Dictionary<TypeReference, string> _typeToShortName = new Dictionary<TypeReference, string>(new TypeReferenceEqualityComparer());

		private readonly Dictionary<string, int> _nextShortNameSuffix = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

		public string GetUniqueShortName(string nonUniqueShortName, TypeReference type)
		{
			string text;
			string result;
			if (this._typeToShortName.TryGetValue(type, out text))
			{
				result = text;
			}
			else
			{
				result = this.CreateAndCacheShortUniqueName(nonUniqueShortName, type);
			}
			return result;
		}

		private string CreateAndCacheShortUniqueName(string nonUniqueShortName, TypeReference type)
		{
			string text = SemiUniqueStableTokenGenerator.GenerateFor(type).ToString();
			if (nonUniqueShortName.Length + text.Length > 60)
			{
				nonUniqueShortName = nonUniqueShortName.Substring(0, 60 - text.Length);
			}
			string text2 = nonUniqueShortName;
			text2 = string.Format("{0}{1}", text2, text);
			TypeReference a;
			if (this._shortNameToType.TryGetValue(text2, out a) && !TypeReferenceEqualityComparer.AreEqual(a, type, TypeComparisonMode.Exact))
			{
				text2 = this.MakeUniqueWithSuffix(nonUniqueShortName);
			}
			this._shortNameToType[text2] = type;
			this._typeToShortName[type] = text2;
			return text2;
		}

		private string MakeUniqueWithSuffix(string nonUniqueShortName)
		{
			int num;
			if (!this._nextShortNameSuffix.TryGetValue(nonUniqueShortName, out num))
			{
				num = 0;
			}
			this._nextShortNameSuffix[nonUniqueShortName] = num + 1;
			return string.Format("{0}_{1}", nonUniqueShortName, num);
		}
	}
}
