namespace Unity.IL2CPP.FileNaming
{
    using Mono.Cecil;
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;

    internal class UniqueLongNameGenerator
    {
        [CompilerGenerated]
        private static Func<TypeReference, string> <>f__mg$cache0;
        [CompilerGenerated]
        private static Func<TypeReference, string> <>f__mg$cache1;

        private static string NameFor(ArrayType type)
        {
            return string.Format("{0} {1}", type.Resolve().Module.Assembly.Name.Name, type.FullName);
        }

        private static string NameFor(GenericInstanceType type)
        {
            if (<>f__mg$cache1 == null)
            {
                <>f__mg$cache1 = new Func<TypeReference, string>(null, (IntPtr) NameFor);
            }
            return string.Format("{0} {1}", NameFor(type.ElementType), "[" + EnumerableExtensions.AggregateWithComma(Enumerable.Select<TypeReference, string>(type.GenericArguments, <>f__mg$cache1)) + "]");
        }

        internal static string NameFor(MethodReference method)
        {
            string str = string.Format("{0} - {1}", NameFor(method.DeclaringType), method.Resolve().FullName);
            GenericInstanceMethod method2 = method as GenericInstanceMethod;
            if (method2 == null)
            {
                return str;
            }
            if (<>f__mg$cache0 == null)
            {
                <>f__mg$cache0 = new Func<TypeReference, string>(null, (IntPtr) NameFor);
            }
            return (str + "[" + EnumerableExtensions.AggregateWithComma(Enumerable.Select<TypeReference, string>(method2.GenericArguments, <>f__mg$cache0)) + "]");
        }

        private static string NameFor(PointerType type)
        {
            return string.Format("{0} {1}", type.Resolve().Module.Assembly.Name.Name, type.FullName);
        }

        private static string NameFor(TypeDefinition type)
        {
            return string.Format("{0} {1}", type.Resolve().Module.Assembly.Name.Name, type.FullName);
        }

        internal static string NameFor(TypeReference type)
        {
            GenericInstanceType type2 = type as GenericInstanceType;
            if (type2 != null)
            {
                return NameFor(type2);
            }
            ArrayType type3 = type as ArrayType;
            if (type3 != null)
            {
                return NameFor(type3);
            }
            PointerType type4 = type as PointerType;
            if (type4 != null)
            {
                return NameFor(type4);
            }
            if (type is TypeSpecification)
            {
                throw new Exception();
            }
            return NameFor(type.Resolve());
        }
    }
}

