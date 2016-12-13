using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity.IL2CPP.IoC;
using Unity.IL2CPP.IoCServices;

namespace Unity.IL2CPP.Metadata
{
	public class ArrayTypeInfoWriter
	{
		[Inject]
		public static ITypeProviderService TypeProvider;

		internal static IEnumerable<MethodReference> InflateArrayMethods(ArrayType arrayType)
		{
			ModuleDefinition mainModule = ArrayTypeInfoWriter.TypeProvider.Corlib.MainModule;
			TypeDefinition type = mainModule.GetType("System.Array");
			TypeDefinition type2 = mainModule.GetType("System.Collections.Generic.ICollection`1");
			TypeDefinition type3 = mainModule.GetType("System.Collections.Generic.IList`1");
			TypeDefinition type4 = mainModule.GetType("System.Collections.Generic.IEnumerable`1");
			return Enumerable.Empty<MethodReference>().Concat(from m in ArrayTypeInfoWriter.GetArrayInterfaceMethods(type, type2, "InternalArray__ICollection_")
			select ArrayTypeInfoWriter.InflateArrayMethod(m, arrayType.ElementType)).Concat(from m in ArrayTypeInfoWriter.GetArrayInterfaceMethods(type, type3, "InternalArray__")
			select ArrayTypeInfoWriter.InflateArrayMethod(m, arrayType.ElementType)).Concat(from m in ArrayTypeInfoWriter.GetArrayInterfaceMethods(type, type4, "InternalArray__IEnumerable_")
			select ArrayTypeInfoWriter.InflateArrayMethod(m, arrayType.ElementType));
		}

		internal static MethodReference InflateArrayMethod(MethodDefinition method, TypeReference elementType)
		{
			MethodReference result;
			if (!method.HasGenericParameters)
			{
				result = method;
			}
			else
			{
				result = new GenericInstanceMethod(method)
				{
					GenericArguments = 
					{
						elementType
					}
				};
			}
			return result;
		}

		[DebuggerHidden]
		internal static IEnumerable<MethodDefinition> GetArrayInterfaceMethods(TypeDefinition arrayType, TypeDefinition interfaceType, string arrayMethodPrefix)
		{
			ArrayTypeInfoWriter.<GetArrayInterfaceMethods>c__Iterator0 <GetArrayInterfaceMethods>c__Iterator = new ArrayTypeInfoWriter.<GetArrayInterfaceMethods>c__Iterator0();
			<GetArrayInterfaceMethods>c__Iterator.interfaceType = interfaceType;
			<GetArrayInterfaceMethods>c__Iterator.arrayType = arrayType;
			<GetArrayInterfaceMethods>c__Iterator.arrayMethodPrefix = arrayMethodPrefix;
			ArrayTypeInfoWriter.<GetArrayInterfaceMethods>c__Iterator0 expr_1C = <GetArrayInterfaceMethods>c__Iterator;
			expr_1C.$PC = -2;
			return expr_1C;
		}

		internal static IEnumerable<TypeReference> TypeAndAllBaseAndInterfaceTypesFor(TypeReference type)
		{
			List<TypeReference> list = new List<TypeReference>();
			while (type != null)
			{
				list.Add(type);
				foreach (TypeReference current in type.GetInterfaces())
				{
					if (!ArrayTypeInfoWriter.IsGenericInstanceWithMoreThanOneGenericArgument(current) && !ArrayTypeInfoWriter.IsSpecialCollectionGenericInterface(current.FullName))
					{
						list.Add(current);
					}
				}
				type = type.GetBaseType();
			}
			return list;
		}

		private static bool IsGenericInstanceWithMoreThanOneGenericArgument(TypeReference type)
		{
			bool result;
			if (type.IsGenericInstance)
			{
				GenericInstanceType genericInstanceType = type as GenericInstanceType;
				if (genericInstanceType != null && genericInstanceType.HasGenericArguments && genericInstanceType.GenericArguments.Count > 1)
				{
					result = true;
					return result;
				}
			}
			result = false;
			return result;
		}

		private static bool IsSpecialCollectionGenericInterface(string typeFullName)
		{
			return typeFullName.Contains("System.Collections.Generic.ICollection`1") || typeFullName.Contains("System.Collections.Generic.IEnumerable`1") || typeFullName.Contains("System.Collections.Generic.IList`1");
		}
	}
}
