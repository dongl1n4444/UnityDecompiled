using Mono.Cecil;
using System;
using Unity.Cecil.Visitor;
using Unity.IL2CPP.IoC;
using Unity.IL2CPP.IoCServices;

namespace Unity.IL2CPP.ILPreProcessor
{
	internal class InjectBaseTypesAndFinalizersIntoComAndWindowsRuntimeTypesVisitor : Visitor
	{
		[Inject]
		public static ITypeProviderService TypeProvider;

		protected override void Visit(TypeDefinition type, Context context)
		{
			if (type.IsComOrWindowsRuntimeType() && !type.IsInterface)
			{
				InjectBaseTypesAndFinalizersIntoComAndWindowsRuntimeTypesVisitor.InjectBaseTypeIfNeeded(type);
				InjectBaseTypesAndFinalizersIntoComAndWindowsRuntimeTypesVisitor.InjectFinalizer(type);
			}
			base.Visit(type, context);
		}

		private static void InjectBaseTypeIfNeeded(TypeDefinition type)
		{
			if (!type.IsIl2CppComObject())
			{
				if (type.BaseType == null)
				{
					throw new InvalidOperationException(string.Format("COM import type '{0}' has no base type.", type.FullName));
				}
				if (type.BaseType.IsSystemObject())
				{
					type.BaseType = type.Module.ImportReference(InjectBaseTypesAndFinalizersIntoComAndWindowsRuntimeTypesVisitor.TypeProvider.Il2CppComObjectTypeReference);
				}
			}
		}

		private static void InjectFinalizer(TypeDefinition type)
		{
			if (!type.IsAttribute())
			{
				MethodDefinition methodDefinition = new MethodDefinition("Finalize", MethodAttributes.Family | MethodAttributes.Virtual | MethodAttributes.HideBySig, type.Module.TypeSystem.Void);
				methodDefinition.HasThis = true;
				methodDefinition.ImplAttributes = MethodImplAttributes.CodeTypeMask;
				type.Methods.Add(methodDefinition);
			}
		}
	}
}
