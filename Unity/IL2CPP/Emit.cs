namespace Unity.IL2CPP
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;

    public class Emit
    {
        [Inject]
        public static INamingService Naming;

        public static string ArrayBoundsCheck(string array, string index)
        {
            return MultiDimensionalArrayBoundsCheck(string.Format("(uint32_t)({0})->max_length", array), index);
        }

        public static string ArrayElementTypeCheck(string array, string value)
        {
            return string.Format("ArrayElementTypeCheck ({0}, {1});", array, value);
        }

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
            if (!Extensions.IsValueType(type))
            {
                return Cast(type, value);
            }
            return Call("Box", metadataAccess.TypeInfoFor(type), "&" + value);
        }

        public static string Call(string method)
        {
            return Call(method, Enumerable.Empty<string>());
        }

        public static string Call(string method, IEnumerable<string> arguments)
        {
            return string.Format("{0}({1})", method, EnumerableExtensions.AggregateWithComma(arguments));
        }

        public static string Call(string method, string argument)
        {
            string[] arguments = new string[] { argument };
            return Call(method, arguments);
        }

        public static string Call(string method, string argument1, string argument2)
        {
            string[] arguments = new string[] { argument1, argument2 };
            return Call(method, arguments);
        }

        public static string Call(string method, string argument1, string argument2, string argument3)
        {
            string[] arguments = new string[] { argument1, argument2, argument3 };
            return Call(method, arguments);
        }

        public static string Cast(TypeReference type, string value)
        {
            return string.Format("({0}){1}", Naming.ForVariable(type), value);
        }

        public static string Cast(string type, string value)
        {
            return string.Format("({0}){1}", type, value);
        }

        public static IEnumerable<string> CastEach(string targetTypeName, IEnumerable<string> values)
        {
            List<string> list = new List<string>();
            foreach (string str in values)
            {
                list.Add(string.Format("({0}){1}", targetTypeName, str));
            }
            return list;
        }

        public static string Dereference(string value)
        {
            return string.Format("(*{0})", value);
        }

        public static string DivideByZeroCheck(TypeReference type, string denominator)
        {
            if (!Extensions.IsIntegralType(type))
            {
                return string.Empty;
            }
            return string.Format("DivideByZeroCheck({0})", denominator);
        }

        public static string Dot(string left, string right)
        {
            return string.Format("{0}.{1}", left, right);
        }

        public static string LoadArrayElement(string array, string index, bool useArrayBoundsCheck)
        {
            return string.Format("({0})->{1}(static_cast<{2}>({3}))", new object[] { array, Naming.ForArrayItemGetter(useArrayBoundsCheck), Naming.ForArrayIndexType(), index });
        }

        public static string LoadArrayElementAddress(string array, string index, bool useArrayBoundsCheck)
        {
            return string.Format("({0})->{1}(static_cast<{2}>({3}))", new object[] { array, Naming.ForArrayItemAddressGetter(useArrayBoundsCheck), Naming.ForArrayIndexType(), index });
        }

        public static string MemoryBarrier()
        {
            return "il2cpp_codegen_memory_barrier()";
        }

        public static string MultiDimensionalArrayBoundsCheck(string length, string index)
        {
            return string.Format("IL2CPP_ARRAY_BOUNDS_CHECK({0}, {1});", index, length);
        }

        public static string NewObj(TypeReference type, IRuntimeMetadataAccess metadataAccess)
        {
            string str = Call("il2cpp_codegen_object_new", metadataAccess.TypeInfoFor(type));
            if (Extensions.IsValueType(type))
            {
                return str;
            }
            return Cast(Naming.ForTypeNameOnly(type) + "*", str);
        }

        public static string NewSZArray(ArrayType arrayType, int length, IRuntimeMetadataAccess metadataAccess)
        {
            return NewSZArray(arrayType, length.ToString(CultureInfo.InvariantCulture), metadataAccess);
        }

        public static string NewSZArray(ArrayType arrayType, string length, IRuntimeMetadataAccess metadataAccess)
        {
            if (arrayType.Rank != 1)
            {
                throw new ArgumentException("Attempting for create a new sz array of invalid rank.", "arrayType");
            }
            return Cast(arrayType, Call("SZArrayNew", metadataAccess.TypeInfoFor(arrayType), length));
        }

        public static string NullCheck(string name)
        {
            return string.Format("NullCheck({0})", name);
        }

        public static string NullCheck(TypeReference type, string name)
        {
            if (Extensions.IsValueType(type))
            {
                return string.Empty;
            }
            return NullCheck(name);
        }

        public static string RaiseManagedException(string exception)
        {
            return string.Format("IL2CPP_RAISE_MANAGED_EXCEPTION({0})", exception);
        }

        public static string RaiseManagedException(string format, params object[] arguments)
        {
            return string.Format("IL2CPP_RAISE_MANAGED_EXCEPTION({0})", string.Format(format, arguments));
        }

        public static string StoreArrayElement(string array, string index, string value, bool useArrayBoundsCheck)
        {
            return string.Format("({0})->{1}(static_cast<{2}>({3}), {4})", new object[] { array, Naming.ForArrayItemSetter(useArrayBoundsCheck), Naming.ForArrayIndexType(), index, value });
        }
    }
}

