namespace Unity.IL2CPP.ILPreProcessor
{
    using Mono.Cecil;
    using Mono.Cecil.Rocks;
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Unity.IL2CPP;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;

    internal class InjectBaseTypesAndFinalizersIntoComAndWindowsRuntimeTypesVisitor
    {
        [CompilerGenerated]
        private static Func<MethodDefinition, bool> <>f__am$cache0;
        [Inject]
        public static ITypeProviderService TypeProvider;

        private static void InjectBaseType(TypeDefinition type)
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

        private void InjectToStringMethod(TypeDefinition type)
        {
            MethodDefinition item = new MethodDefinition("ToString", MethodAttributes.CompilerControlled | MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.Virtual, type.Module.TypeSystem.String) {
                HasThis = true,
                ImplAttributes = MethodImplAttributes.CodeTypeMask
            };
            type.Methods.Add(item);
        }

        public void Process(AssemblyDefinition assembly)
        {
            foreach (TypeDefinition definition in assembly.MainModule.GetAllTypes())
            {
                if (definition.IsComOrWindowsRuntimeType() && !definition.IsInterface)
                {
                    if (definition.IsIl2CppComObject())
                    {
                        if (TypeProvider.IStringableType != null)
                        {
                            if (<>f__am$cache0 == null)
                            {
                                <>f__am$cache0 = m => m.Name == "ToString";
                            }
                            if (TypeProvider.IStringableType.Methods.Any<MethodDefinition>(<>f__am$cache0))
                            {
                                this.InjectToStringMethod(definition);
                            }
                        }
                    }
                    else
                    {
                        InjectBaseType(definition);
                    }
                    InjectFinalizer(definition);
                }
            }
        }
    }
}

