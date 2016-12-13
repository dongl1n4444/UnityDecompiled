using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity.CecilTools.Extensions;

namespace Unity.CecilTools
{
	public static class CecilUtils
	{
		public static MethodDefinition FindInTypeExplicitImplementationFor(MethodDefinition interfaceMethod, TypeDefinition typeDefinition)
		{
			return typeDefinition.Methods.SingleOrDefault((MethodDefinition m) => m.Overrides.Any((MethodReference o) => o.CheckedResolve().SameAs(interfaceMethod)));
		}

		public static IEnumerable<TypeDefinition> AllInterfacesImplementedBy(TypeDefinition typeDefinition)
		{
			return (from i in CecilUtils.TypeAndBaseTypesOf(typeDefinition).SelectMany((TypeDefinition t) => t.Interfaces)
			select i.InterfaceType.CheckedResolve()).Distinct<TypeDefinition>();
		}

		[DebuggerHidden]
		public static IEnumerable<TypeDefinition> TypeAndBaseTypesOf(TypeReference typeReference)
		{
			CecilUtils.<TypeAndBaseTypesOf>c__Iterator0 <TypeAndBaseTypesOf>c__Iterator = new CecilUtils.<TypeAndBaseTypesOf>c__Iterator0();
			<TypeAndBaseTypesOf>c__Iterator.typeReference = typeReference;
			<TypeAndBaseTypesOf>c__Iterator.<$>typeReference = typeReference;
			CecilUtils.<TypeAndBaseTypesOf>c__Iterator0 expr_15 = <TypeAndBaseTypesOf>c__Iterator;
			expr_15.$PC = -2;
			return expr_15;
		}

		public static IEnumerable<TypeDefinition> BaseTypesOf(TypeReference typeReference)
		{
			return CecilUtils.TypeAndBaseTypesOf(typeReference).Skip(1);
		}

		public static bool IsGenericList(TypeReference type)
		{
			return type.Name == "List`1" && type.SafeNamespace() == "System.Collections.Generic";
		}

		public static bool IsGenericDictionary(TypeReference type)
		{
			if (type is GenericInstanceType)
			{
				type = ((GenericInstanceType)type).ElementType;
			}
			return type.Name == "Dictionary`2" && type.SafeNamespace() == "System.Collections.Generic";
		}

		public static TypeReference ElementTypeOfCollection(TypeReference type)
		{
			ArrayType arrayType = type as ArrayType;
			TypeReference result;
			if (arrayType != null)
			{
				result = arrayType.ElementType;
			}
			else
			{
				if (!CecilUtils.IsGenericList(type))
				{
					throw new ArgumentException();
				}
				result = ((GenericInstanceType)type).GenericArguments.Single<TypeReference>();
			}
			return result;
		}
	}
}
