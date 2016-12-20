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
            GenericInstanceMethod[] methodArray = Enumerable.ToArray<GenericInstanceMethod>(Enumerable.Where<GenericInstanceMethod>(generics.Methods.Items, <>f__am$cache0));
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = new Func<TypeDefinition, bool>(null, (IntPtr) <Collect>m__1);
            }
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = new Func<TypeReference, bool>(null, (IntPtr) <Collect>m__2);
            }
            TypeReference[] referenceArray = Enumerable.ToArray<TypeReference>(Enumerable.Where<TypeReference>(Enumerable.Concat<TypeReference>(Enumerable.Where<TypeDefinition>(types, <>f__am$cache1), generics.Types.Items), <>f__am$cache2));
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
            if (!Extensions.DerivesFrom(typeThatMightHaveAnOverrideingMethod, potentiallyOverridenGenericInstanceMethod.DeclaringType, true))
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
            return ((<>f__am$cache3 == null) && Enumerable.Any<MethodDefinition>(typeDefinition.Methods, <>f__am$cache3));
        }
    }
}

