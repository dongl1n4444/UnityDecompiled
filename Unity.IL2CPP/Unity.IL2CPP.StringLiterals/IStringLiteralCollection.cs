using System;
using System.Collections.ObjectModel;

namespace Unity.IL2CPP.StringLiterals
{
	public interface IStringLiteralCollection
	{
		ReadOnlyCollection<string> GetStringLiterals();

		int GetIndex(string literal);
	}
}
