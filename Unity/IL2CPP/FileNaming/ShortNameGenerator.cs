namespace Unity.IL2CPP.FileNaming
{
    using Mono.Cecil;
    using System;
    using System.Globalization;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;

    public class ShortNameGenerator
    {
        [Inject]
        public static INamingService Naming;

        private static string NonUniqueShortNameFor(ArrayType type)
        {
            return (NonUniqueShortNameFor(type.ElementType) + "_arr" + ((type.Rank <= 1) ? "" : type.Rank.ToString(CultureInfo.InvariantCulture)));
        }

        private static string NonUniqueShortNameFor(GenericInstanceType type)
        {
            return (NonUniqueShortNameFor(type.ElementType) + "_gen");
        }

        internal static string NonUniqueShortNameFor(MethodReference method)
        {
            return Naming.ForMethodNameOnly(method);
        }

        private static string NonUniqueShortNameFor(TypeDefinition type)
        {
            return Naming.ForFile(type);
        }

        internal static string NonUniqueShortNameFor(TypeReference type)
        {
            GenericInstanceType type2 = type as GenericInstanceType;
            if (type2 != null)
            {
                return NonUniqueShortNameFor(type2);
            }
            ArrayType type3 = type as ArrayType;
            if (type3 != null)
            {
                return NonUniqueShortNameFor(type3);
            }
            if (type is TypeSpecification)
            {
                throw new Exception();
            }
            return NonUniqueShortNameFor(type.Resolve());
        }
    }
}

