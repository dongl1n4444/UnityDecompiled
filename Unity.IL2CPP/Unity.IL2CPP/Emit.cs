using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Unity.IL2CPP.IoC;
using Unity.IL2CPP.IoCServices;

namespace Unity.IL2CPP
{
	public class Emit
	{
		[Inject]
		public static INamingService Naming;

		public static string Arrow(string left, string right)
		{
			return string.Format("{0}->{1}", left, right);
		}

		public static string Assign(string left, string right)
		{
			return string.Format("{0} = {1}", left, right);
		}

		public static string Box(TypeReference type, string value, IRuntimeMetadataAccess metadataAccess)
		{
			string result;
			if (!type.IsValueType())
			{
				result = Emit.Cast(type, value);
			}
			else
			{
				result = Emit.Call("Box", metadataAccess.TypeInfoFor(type), "&" + value);
			}
			return result;
		}

		public static string Call(string method)
		{
			return Emit.Call(method, Enumerable.Empty<string>());
		}

		public static string Call(string method, string argument)
		{
			return Emit.Call(method, new string[]
			{
				argument
			});
		}

		public static string Call(string method, string argument1, string argument2)
		{
			return Emit.Call(method, new string[]
			{
				argument1,
				argument2
			});
		}

		public static string Call(string method, string argument1, string argument2, string argument3)
		{
			return Emit.Call(method, new string[]
			{
				argument1,
				argument2,
				argument3
			});
		}

		public static string Call(string method, IEnumerable<string> arguments)
		{
			return string.Format("{0}({1})", method, arguments.AggregateWithComma());
		}

		public static string Cast(TypeReference type, string value)
		{
			return string.Format("({0}){1}", Emit.Naming.ForVariable(type), value);
		}

		public static IEnumerable<string> CastEach(string targetTypeName, IEnumerable<string> values)
		{
			List<string> list = new List<string>();
			foreach (string current in values)
			{
				list.Add(string.Format("({0}){1}", targetTypeName, current));
			}
			return list;
		}

		public static string Cast(string type, string value)
		{
			return string.Format("({0}){1}", type, value);
		}

		public static string Dereference(string value)
		{
			return string.Format("(*{0})", value);
		}

		public static string Dot(string left, string right)
		{
			return string.Format("{0}.{1}", left, right);
		}

		public static string ArrayBoundsCheck(string array, string index)
		{
			return string.Format("IL2CPP_ARRAY_BOUNDS_CHECK({0}, {1});", array, index);
		}

		public static string LoadArrayElement(string array, string index)
		{
			return string.Format("({0})->{1}(static_cast<{2}>({3}))", new object[]
			{
				array,
				Emit.Naming.ForArrayItemGetter(),
				Emit.Naming.ForArrayIndexType(),
				index
			});
		}

		public static string LoadArrayElementAddress(string array, string index)
		{
			return string.Format("({0})->{1}(static_cast<{2}>({3}))", new object[]
			{
				array,
				Emit.Naming.ForArrayItemAddressGetter(),
				Emit.Naming.ForArrayIndexType(),
				index
			});
		}

		public static string StoreArrayElement(string array, string index, string value)
		{
			return string.Format("({0})->{1}(static_cast<{2}>({3}), {4})", new object[]
			{
				array,
				Emit.Naming.ForArrayItemSetter(),
				Emit.Naming.ForArrayIndexType(),
				index,
				value
			});
		}

		public static string NewSZArray(ArrayType arrayType, int length, IRuntimeMetadataAccess metadataAccess)
		{
			return Emit.NewSZArray(arrayType, length.ToString(CultureInfo.InvariantCulture), metadataAccess);
		}

		public static string NewSZArray(ArrayType arrayType, string length, IRuntimeMetadataAccess metadataAccess)
		{
			if (arrayType.Rank != 1)
			{
				throw new ArgumentException("Attempting for create a new sz array of invalid rank.", "arrayType");
			}
			return Emit.Cast(arrayType, Emit.Call("SZArrayNew", metadataAccess.TypeInfoFor(arrayType), length));
		}

		public static string NullCheck(TypeReference type, string name)
		{
			string result;
			if (type.IsValueType())
			{
				result = string.Empty;
			}
			else
			{
				result = Emit.NullCheck(name);
			}
			return result;
		}

		public static string NullCheck(string name)
		{
			return string.Format("NullCheck({0})", name);
		}

		public static string ArrayElementTypeCheck(string array, string value)
		{
			return string.Format("ArrayElementTypeCheck ({0}, {1});", array, value);
		}

		public static string DivideByZeroCheck(TypeReference type, string denominator)
		{
			string result;
			if (!type.IsIntegralType())
			{
				result = string.Empty;
			}
			else
			{
				result = string.Format("DivideByZeroCheck({0})", denominator);
			}
			return result;
		}

		public static string RaiseManagedException(string exception)
		{
			return string.Format("IL2CPP_RAISE_MANAGED_EXCEPTION({0})", exception);
		}

		public static string MemoryBarrier()
		{
			return "il2cpp_codegen_memory_barrier()";
		}
	}
}
