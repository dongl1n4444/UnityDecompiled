namespace Unity.IL2CPP.ILPreProcessor
{
    using Mono.Cecil;
    using System;
    using Unity.Cecil.Visitor;
    using Unity.IL2CPP;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;

    internal class InjectBaseTypesAndFinalizersIntoComAndWindowsRuntimeTypesVisitor : Unity.Cecil.Visitor.Visitor
    {
        [Inject]
        public static ITypeProviderService TypeProvider;

        private static void InjectBaseTypeIfNeeded(TypeDefinition type)
        {
            if (!type.IsIl2CppComObject())
            {
                if (type.BaseType == null)
                {
                    throw new InvalidOperationException($"COM import type '{type.FullName}' has no base type.");
                }
                if (type.BaseType.IsSystemObject())
                {
                    type.BaseType = type.Module.ImportReference(TypeProvider.Il2CppComObjectTypeReference);
                }
            }
        }

        private static void InjectFinalizer(TypeDefinition type)
        {
            if (!type.IsAttribute())
            {
                MethodDefinition item = new MethodDefinition("Finalize", MethodAttributes.CompilerControlled | MethodAttributes.Family | MethodAttributes.HideBySig | MethodAttributes.Virtual, type.Module.TypeSystem.Void) {
                    HasThis = true,
                    ImplAttributes = MethodImplAttributes.CodeTypeMask
                };
                type.Methods.Add(item);
            }
        }

        protected override void Visit(TypeDefinition type, Unity.Cecil.Visitor.Context context)
        {
            if (type.IsComOrWindowsRuntimeType() && !type.IsInterface)
            {
                InjectBaseTypeIfNeeded(type);
                InjectFinalizer(type);
            }
            base.Visit(type, context);
        }
    }
}

