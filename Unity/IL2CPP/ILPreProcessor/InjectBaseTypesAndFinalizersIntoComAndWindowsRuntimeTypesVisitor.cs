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
            if (!Unity.IL2CPP.Extensions.IsIl2CppComObject(type))
            {
                if (type.BaseType == null)
                {
                    throw new InvalidOperationException(string.Format("COM import type '{0}' has no base type.", type.FullName));
                }
                if (Unity.IL2CPP.Extensions.IsSystemObject(type.BaseType))
                {
                    type.BaseType = type.Module.ImportReference(TypeProvider.Il2CppComObjectTypeReference);
                }
            }
        }

        private static void InjectFinalizer(TypeDefinition type)
        {
            if (!Unity.IL2CPP.Extensions.IsAttribute(type))
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
            if (Unity.IL2CPP.Extensions.IsComOrWindowsRuntimeType(type) && !type.IsInterface)
            {
                InjectBaseTypeIfNeeded(type);
                InjectFinalizer(type);
            }
            base.Visit(type, context);
        }
    }
}

