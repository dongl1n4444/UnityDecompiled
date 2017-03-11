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

        private static string NameFor(ArrayType type) => 
            $"{type.Resolve().Module.Assembly.Name.Name} {type.FullName}";

        private static string NameFor(GenericInstanceType type)
        {
            if (<>f__mg$cache1 == null)
            {
                <>f__mg$cache1 = new Func<TypeReference, string>(UniqueLongNameGenerator.NameFor);
            }
            return $"{NameFor(type.ElementType)} {("[" + type.GenericArguments.Select<TypeReference, string>(<>f__mg$cache1).AggregateWithComma() + "]")}";
        }

        internal static string NameFor(MethodReference method)
        {
            string str = $"{NameFor(method.DeclaringType)} - {method.Resolve().FullName}";
            GenericInstanceMethod method2 = method as GenericInstanceMethod;
            if (method2 == null)
            {
                return str;
            }
            if (<>f__mg$cache0 == null)
            {
                <>f__mg$cache0 = new Func<TypeReference, string>(UniqueLongNameGenerator.NameFor);
            }
            return (str + "[" + method2.GenericArguments.Select<TypeReference, string>(<>f__mg$cache0).AggregateWithComma() + "]");
        }

        private static string NameFor(PointerType type) => 
            $"{type.Resolve().Module.Assembly.Name.Name} {type.FullName}";

        private static string NameFor(TypeDefinition type) => 
            $"{type.Resolve().Module.Assembly.Name.Name} {type.FullName}";

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

