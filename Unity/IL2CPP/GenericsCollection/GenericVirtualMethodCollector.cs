namespace Unity.IL2CPP.GenericsCollection
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Unity.IL2CPP;
    using Unity.IL2CPP.Common;
    using Unity.IL2CPP.Metadata;

    public class GenericVirtualMethodCollector
    {
        [CompilerGenerated]
        private static Func<GenericInstanceMethod, bool> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<TypeDefinition, bool> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<TypeReference, bool> <>f__am$cache2;
        [CompilerGenerated]
        private static Func<MethodDefinition, bool> <>f__am$cache3;

        public void Collect(InflatedCollectionCollector generics, IEnumerable<TypeDefinition> types, VTableBuilder vTableBuilder)
        {
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = new Func<GenericInstanceMethod, bool>(null, (IntPtr) <Collect>m__0);
            }
            GenericInstanceMethod[] methodArray = generics.Methods.Items.Where<GenericInstanceMethod>(<>f__am$cache0).ToArray<GenericInstanceMethod>();
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = new Func<TypeDefinition, bool>(null, (IntPtr) <Collect>m__1);
            }
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = new Func<TypeReference, bool>(null, (IntPtr) <Collect>m__2);
            }
            TypeReference[] referenceArray = types.Where<TypeDefinition>(<>f__am$cache1).Concat<TypeReference>(generics.Types.Items).Where<TypeReference>(<>f__am$cache2).ToArray<TypeReference>();
            foreach (TypeReference reference in referenceArray)
            {
                foreach (GenericInstanceMethod method in methodArray)
                {
                    GenericInstanceMethod method2 = FindMethodInTypeThatOverrides(method, reference, vTableBuilder);
                    if (method2 != null)
                    {
                        GenericContextAwareVisitor.ProcessGenericMethod(method2, generics);
                    }
                }
            }
        }

        private static GenericInstanceMethod FindMethodInTypeThatOverrides(GenericInstanceMethod potentiallyOverridenGenericInstanceMethod, TypeReference typeThatMightHaveAnOverrideingMethod, VTableBuilder vTableBuilder)
        {
            MethodDefinition method = potentiallyOverridenGenericInstanceMethod.Resolve();
            if (!typeThatMightHaveAnOverrideingMethod.DerivesFrom(potentiallyOverridenGenericInstanceMethod.DeclaringType, true))
            {
                return null;
            }
            VTable table = vTableBuilder.VTableFor(typeThatMightHaveAnOverrideingMethod, null);
            int num = vTableBuilder.IndexFor(method);
            if (method.DeclaringType.IsInterface)
            {
                num += table.InterfaceOffsets[potentiallyOverridenGenericInstanceMethod.DeclaringType];
            }
            MethodReference reference = table.Slots[num];
            if (reference == null)
            {
                return null;
            }
            if (!Unity.IL2CPP.Common.TypeReferenceEqualityComparer.AreEqual(reference.DeclaringType, typeThatMightHaveAnOverrideingMethod, TypeComparisonMode.Exact))
            {
                return null;
            }
            return Inflater.InflateMethod(new GenericContext(typeThatMightHaveAnOverrideingMethod as GenericInstanceType, potentiallyOverridenGenericInstanceMethod), reference.Resolve());
        }

        private static bool TypeImplementsGenericVirtualMethods(TypeDefinition typeDefinition)
        {
            if (!typeDefinition.IsInterface)
            {
            }
            return ((<>f__am$cache3 == null) && typeDefinition.Methods.Any<MethodDefinition>(<>f__am$cache3));
        }
    }
}

