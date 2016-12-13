using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unity.IL2CPP.Common
{
	public static class ExtensionMethods
	{
		public static string SeparateWithSpaces(this IEnumerable<string> inputs)
		{
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = true;
			foreach (string current in inputs)
			{
				if (!flag)
				{
					stringBuilder.Append(" ");
				}
				flag = false;
				stringBuilder.Append(current);
			}
			return stringBuilder.ToString();
		}

		public static IEnumerable<string> PrefixedWith(this IEnumerable<string> inputs, string prefix)
		{
			return from input in inputs
			select prefix + input;
		}

		public static IEnumerable<string> InQuotes(this IEnumerable<string> inputs)
		{
			return from input in inputs
			select input.InQuotes();
		}

		public static string InQuotes(this string input)
		{
			return "\"" + input + "\"";
		}

		public static ReadOnlyHashSet<T> AsReadOnly<T>(this HashSet<T> set)
		{
			return new ReadOnlyHashSet<T>(set);
		}

		public static ReadOnlyDictionary<T, K> AsReadOnly<T, K>(this IDictionary<T, K> dictionary)
		{
			return new ReadOnlyDictionary<T, K>(dictionary);
		}
	}
}
