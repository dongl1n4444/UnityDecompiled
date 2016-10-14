using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.IL2CPP.IoC;
using Unity.IL2CPP.IoCServices;

namespace Unity.IL2CPP
{
	public sealed class MethodTables
	{
		public Dictionary<string, int> MethodPointers;

		[Inject]
		public static IRuntimeInvokerCollectorAdderService RuntimeInvokerCollectorAdder;

		[Inject]
		public static INamingService Naming;

		[Inject]
		public static IGenericSharingAnalysisService GenericSharingAnalysis;

		internal static MethodTables CollectMethodTables(IEnumerable<MethodReference> genericMethods)
		{
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			dictionary.Add(MethodTables.Naming.Null, 0);
			foreach (MethodReference current in from m in genericMethods
			where !m.HasGenericParameters && !m.DeclaringType.HasGenericParameters && !m.ContainsGenericParameters()
			select m)
			{
				string key = MethodTables.MethodPointerFor(current);
				if (!dictionary.ContainsKey(key))
				{
					dictionary.Add(key, dictionary.Count);
				}
				MethodTables.RuntimeInvokerCollectorAdder.Add(current);
			}
			return new MethodTables
			{
				MethodPointers = dictionary
			};
		}

		internal static string MethodPointerFor(MethodReference method)
		{
			string text = MethodTables.MethodPointerNameFor(method);
			if (text != MethodTables.Naming.Null)
			{
				text = "(Il2CppMethodPointer)&" + text;
			}
			return text;
		}

		internal static string MethodPointerNameFor(MethodReference method)
		{
			string result;
			if (MethodWriter.IsGetOrSetGenericValueImplOnArray(method))
			{
				result = MethodTables.Naming.Null;
			}
			else if (GenericsUtilities.IsGenericInstanceOfCompareExchange(method))
			{
				result = MethodTables.Naming.Null;
			}
			else if (GenericsUtilities.IsGenericInstanceOfExchange(method))
			{
				result = MethodTables.Naming.Null;
			}
			else if (!MethodWriter.MethodCanBeDirectlyCalled(method.Resolve()))
			{
				result = MethodTables.Naming.Null;
			}
			else if (MethodTables.GenericSharingAnalysis.CanShareMethod(method))
			{
				method = MethodTables.GenericSharingAnalysis.GetSharedMethod(method);
				if (method.HasThis && method.DeclaringType.IsValueType())
				{
					result = MethodTables.Naming.ForMethodAdjustorThunk(method);
				}
				else
				{
					result = MethodTables.Naming.ForMethod(method) + "_gshared";
				}
			}
			else if (method.HasThis && method.DeclaringType.IsValueType())
			{
				result = MethodTables.Naming.ForMethodAdjustorThunk(method);
			}
			else
			{
				result = MethodTables.Naming.ForMethod(method);
			}
			return result;
		}
	}
}
