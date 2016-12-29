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

        public static string ArrayBoundsCheck(string array, string index) => 
            MultiDimensionalArrayBoundsCheck($"(uint32_t)({array})->max_length", index);

        public static string ArrayElementTypeCheck(string array, string value) => 
            $"ArrayElementTypeCheck ({array}, {value});";

        public static string Arrow(string left, string right) => 
            $"{left}->{right}";

        public static string Assign(string left, string right) => 
            $"{left} = {right}";

        public static string Box(TypeReference type, string value, IRuntimeMetadataAccess metadataAccess)
        {
            if (!type.IsValueType())
            {
                return Cast(type, value);
            }
            return Call("Box", metadataAccess.TypeInfoFor(type), "&" + value);
        }

        public static string Call(string method) => 
            Call(method, Enumerable.Empty<string>());

        public static string Call(string method, IEnumerable<string> arguments) => 
            $"{method}({arguments.AggregateWithComma()})";

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

        public static string Cast(TypeReference type, string value) => 
            $"({Naming.ForVariable(type)}){value}";

        public static string Cast(string type, string value) => 
            $"({type}){value}";

        public static IEnumerable<string> CastEach(string targetTypeName, IEnumerable<string> values)
        {
            List<string> list = new List<string>();
            foreach (string str in values)
            {
                list.Add($"({targetTypeName}){str}");
            }
            return list;
        }

        public static string Dereference(string value) => 
            $"(*{value})";

        public static string DivideByZeroCheck(TypeReference type, string denominator)
        {
            if (!type.IsIntegralType())
            {
                return string.Empty;
            }
            return $"DivideByZeroCheck({denominator})";
        }

        public static string Dot(string left, string right) => 
            $"{left}.{right}";

        public static string LoadArrayElement(string array, string index, bool useArrayBoundsCheck) => 
            $"({array})->{Naming.ForArrayItemGetter(useArrayBoundsCheck)}(static_cast<{Naming.ForArrayIndexType()}>({index}))";

        public static string LoadArrayElementAddress(string array, string index, bool useArrayBoundsCheck) => 
            $"({array})->{Naming.ForArrayItemAddressGetter(useArrayBoundsCheck)}(static_cast<{Naming.ForArrayIndexType()}>({index}))";

        public static string MemoryBarrier() => 
            "il2cpp_codegen_memory_barrier()";

        public static string MultiDimensionalArrayBoundsCheck(string length, string index) => 
            $"IL2CPP_ARRAY_BOUNDS_CHECK({index}, {length});";

        public static string NewObj(TypeReference type, IRuntimeMetadataAccess metadataAccess)
        {
            string str = Call("il2cpp_codegen_object_new", metadataAccess.TypeInfoFor(type));
            if (type.IsValueType())
            {
                return str;
            }
            return Cast(Naming.ForTypeNameOnly(type) + "*", str);
        }

        public static string NewSZArray(ArrayType arrayType, int length, IRuntimeMetadataAccess metadataAccess) => 
            NewSZArray(arrayType, length.ToString(CultureInfo.InvariantCulture), metadataAccess);

        public static string NewSZArray(ArrayType arrayType, string length, IRuntimeMetadataAccess metadataAccess)
        {
            if (arrayType.Rank != 1)
            {
                throw new ArgumentException("Attempting for create a new sz array of invalid rank.", "arrayType");
            }
            return Cast(arrayType, Call("SZArrayNew", metadataAccess.TypeInfoFor(arrayType), length));
        }

        public static string NullCheck(string name) => 
            $"NullCheck({name})";

        public static string NullCheck(TypeReference type, string name)
        {
            if (type.IsValueType())
            {
                return string.Empty;
            }
            return NullCheck(name);
        }

        public static string RaiseManagedException(string exception) => 
            $"IL2CPP_RAISE_MANAGED_EXCEPTION({exception})";

        public static string RaiseManagedException(string format, params object[] arguments) => 
            $"IL2CPP_RAISE_MANAGED_EXCEPTION({string.Format(format, arguments)})";

        public static string StoreArrayElement(string array, string index, string value, bool useArrayBoundsCheck) => 
            $"({array})->{Naming.ForArrayItemSetter(useArrayBoundsCheck)}(static_cast<{Naming.ForArrayIndexType()}>({index}), {value})";
    }
}

