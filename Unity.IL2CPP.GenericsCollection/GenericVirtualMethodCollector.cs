using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.IL2CPP.Common;
using Unity.IL2CPP.Metadata;

namespace Unity.IL2CPP.GenericsCollection
{
	public class GenericVirtualMethodCollector
	{
		public void Collect(InflatedCollectionCollector generics, IEnumerable<TypeDefinition> types, VTableBuilder vTableBuilder)
		{
			GenericInstanceMethod[] array = (from m in generics.Methods.Items
			where m.Resolve().IsVirtual
			select m).ToArray<GenericInstanceMethod>();
			TypeReference[] array2 = (from t in (from t in types
			where !t.HasGenericParameters
			select t).Concat(generics.Types.Items)
			where GenericVirtualMethodCollector.TypeImplementsGenericVirtualMethods(t.Resolve())
			select t).ToArray<TypeReference>();
			TypeReference[] array3 = array2;
			for (int i = 0; i < array3.Length; i++)
			{
				TypeReference typeThatMightHaveAnOverrideingMethod = array3[i];
				GenericInstanceMethod[] array4 = array;
				for (int j = 0; j < array4.Length; j++)
				{
					GenericInstanceMethod potentiallyOverridenGenericInstanceMethod = array4[j];
					GenericInstanceMethod genericInstanceMethod = GenericVirtualMethodCollector.FindMethodInTypeThatOverrides(potentiallyOverridenGenericInstanceMethod, typeThatMightHaveAnOverrideingMethod, vTableBuilder);
					if (genericInstanceMethod != null)
					{
						GenericContextAwareVisitor.ProcessGenericMethod(genericInstanceMethod, generics);
					}
				}
			}
		}

		private static bool TypeImplementsGenericVirtualMethods(TypeDefinition typeDefinition)
		{
			bool arg_37_0;
			if (!typeDefinition.IsInterface)
			{
				arg_37_0 = typeDefinition.Methods.Any((MethodDefinition m) => m.HasGenericParameters && m.IsVirtual);
			}
			else
			{
				arg_37_0 = false;
			}
			return arg_37_0;
		}

		private static GenericInstanceMethod FindMethodInTypeThatOverrides(GenericInstanceMethod potentiallyOverridenGenericInstanceMethod, TypeReference typeThatMightHaveAnOverrideingMethod, VTableBuilder vTableBuilder)
		{
			MethodDefinition methodDefinition = potentiallyOverridenGenericInstanceMethod.Resolve();
			GenericInstanceMethod result;
			if (!typeThatMightHaveAnOverrideingMethod.DerivesFrom(potentiallyOverridenGenericInstanceMethod.DeclaringType, true))
			{
				result = null;
			}
			else
			{
				VTable vTable = vTableBuilder.VTableFor(typeThatMightHaveAnOverrideingMethod, null);
				int num = vTableBuilder.IndexFor(methodDefinition);
				if (methodDefinition.DeclaringType.IsInterface)
				{
					num += vTable.InterfaceOffsets[potentiallyOverridenGenericInstanceMethod.DeclaringType];
				}
				MethodReference methodReference = vTable.Slots[num];
				if (methodReference == null)
				{
					result = null;
				}
				else if (!TypeReferenceEqualityComparer.AreEqual(methodReference.DeclaringType, typeThatMightHaveAnOverrideingMethod, TypeComparisonMode.Exact))
				{
					result = null;
				}
				else
				{
					GenericInstanceMethod genericInstanceMethod = Inflater.InflateMethod(new GenericContext(typeThatMightHaveAnOverrideingMethod as GenericInstanceType, potentiallyOverridenGenericInstanceMethod), methodReference.Resolve());
					result = genericInstanceMethod;
				}
			}
			return result;
		}
	}
}
