using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public static class EnumerableExtensions
{
	[DebuggerHidden]
	public static IEnumerable<T> Append<T>(this IEnumerable<T> inputs, T extra)
	{
		EnumerableExtensions.<Append>c__Iterator0<T> <Append>c__Iterator = new EnumerableExtensions.<Append>c__Iterator0<T>();
		<Append>c__Iterator.inputs = inputs;
		<Append>c__Iterator.extra = extra;
		EnumerableExtensions.<Append>c__Iterator0<T> expr_15 = <Append>c__Iterator;
		expr_15.$PC = -2;
		return expr_15;
	}

	[DebuggerHidden]
	public static IEnumerable<T> Prepend<T>(this IEnumerable<T> inputs, T extra)
	{
		EnumerableExtensions.<Prepend>c__Iterator1<T> <Prepend>c__Iterator = new EnumerableExtensions.<Prepend>c__Iterator1<T>();
		<Prepend>c__Iterator.extra = extra;
		<Prepend>c__Iterator.inputs = inputs;
		EnumerableExtensions.<Prepend>c__Iterator1<T> expr_15 = <Prepend>c__Iterator;
		expr_15.$PC = -2;
		return expr_15;
	}

	public static string AggregateWithComma(this IEnumerable<string> elements)
	{
		return elements.AggregateWith(", ");
	}

	public static string AggregateWithSpace(this IEnumerable<string> elements)
	{
		return elements.AggregateWith(" ");
	}

	public static string AggregateWithUnderscore(this IEnumerable<string> elements)
	{
		return elements.AggregateWith("_");
	}

	public static string AggregateWith(this IEnumerable<string> elements, string separator)
	{
		string result;
		if (elements.Any<string>())
		{
			result = elements.Aggregate((string buff, string s) => buff + separator + s);
		}
		else
		{
			result = string.Empty;
		}
		return result;
	}
}
