using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Unity.IL2CPP.Portability;

namespace Unity.IL2CPP.StringLiterals
{
	public class StringLiteralCollection : IStringLiteralProvider, IStringLiteralCollection, IDisposable
	{
		private readonly Dictionary<string, int> _stringLiterals;

		public StringLiteralCollection()
		{
			this._stringLiterals = new Dictionary<string, int>();
		}

		public void Dispose()
		{
			this._stringLiterals.Clear();
		}

		public int Add(string literal)
		{
			int count;
			int result;
			if (this._stringLiterals.TryGetValue(literal, out count))
			{
				result = count;
			}
			else
			{
				count = this._stringLiterals.Count;
				this._stringLiterals.Add(literal, count);
				result = count;
			}
			return result;
		}

		public ReadOnlyCollection<string> GetStringLiterals()
		{
			return (from kvp in this._stringLiterals
			orderby kvp.Value
			select kvp.Key).ToArray<string>().AsReadOnlyPortable<string>();
		}

		public int GetIndex(string literal)
		{
			return this._stringLiterals[literal];
		}
	}
}
