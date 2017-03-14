namespace Unity.IL2CPP.WindowsRuntime
{
    using Mono.Cecil;
    using Mono.Cecil.Cil;
    using Mono.Cecil.Rocks;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Unity.IL2CPP;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;

    internal class InterfaceNativeToManagedAdapterGenerator
    {
        [CompilerGenerated]
        private static Func<MethodDefinition, bool> <>f__am$cache0;
        [Inject]
        public static INamingService Naming;
        [Inject]
        public static IStatsService Stats;
        [Inject]
        public static ITypeProviderService TypeProvider;

        private static void CollectProjectedInterfaces(IEnumerable<KeyValuePair<TypeDefinition, TypeDefinition>> clrToWindowsRuntimeProjections, Dictionary<TypeDefinition, TypeDefinition> clrToWindowsRuntimeProjectedInterfaces)
        {
            foreach (KeyValuePair<TypeDefinition, TypeDefinition> pair in clrToWindowsRuntimeProjections)
            {
                if (pair.Key.IsInterface)
                {
                    clrToWindowsRuntimeProjectedInterfaces.Add(pair.Key, pair.Value);
                }
            }
            foreach (KeyValuePair<TypeDefinition, TypeDefinition> pair2 in clrToWindowsRuntimeProjectedInterfaces.ToArray<KeyValuePair<TypeDefinition, TypeDefinition>>())
            {
                CollectProjectedInterfacesRecursively(pair2.Key, clrToWindowsRuntimeProjectedInterfaces);
            }
        }

        private static void CollectProjectedInterfacesRecursively(TypeDefinition clrInterface, Dictionary<TypeDefinition, TypeDefinition> clrToWindowsRuntimeProjectedInterfaces)
        {
            foreach (InterfaceImplementation implementation in clrInterface.Interfaces)
            {
                TypeDefinition key = implementation.InterfaceType.Resolve();
                if (!clrToWindowsRuntimeProjectedInterfaces.ContainsKey(key))
                {
                    clrToWindowsRuntimeProjectedInterfaces.Add(key, null);
                    CollectProjectedInterfacesRecursively(key, clrToWindowsRuntimeProjectedInterfaces);
                }
            }
        }

        private static TypeDefinition CreateAdapterClass(TypeDefinition clrInterface, TypeDefinition windowsRuntimeInterface, Dictionary<MethodDefinition, InterfaceAdapterMethodBodyWriter> adapterMethodBodyWriters)
        {
            TypeDefinition item = new TypeDefinition("System.Runtime.InteropServices.WindowsRuntime", Naming.ForWindowsRuntimeAdapterTypeName(windowsRuntimeInterface, clrInterface), TypeAttributes.Abstract | TypeAttributes.BeforeFieldInit | TypeAttributes.Sealed, TypeProvider.ObjectTypeReference);
            TypeProvider.Corlib.MainModule.Types.Add(item);
            TypeReference reference = clrInterface;
            if (clrInterface.HasGenericParameters)
            {
                GenericInstanceType type = new GenericInstanceType(reference);
                foreach (GenericParameter parameter in clrInterface.GenericParameters)
                {
                    GenericParameter parameter2 = new GenericParameter(parameter.Name, item);
                    item.GenericParameters.Add(parameter2);
                    type.GenericArguments.Add(parameter2);
                }
                reference = type;
            }
            InterfaceUtilities.MakeImplementInterface(item, reference);
            MethodDefinition[] definitionArray = item.Methods.ToArray();
            foreach (MethodDefinition definition2 in definitionArray)
            {
                MethodDefinition key = definition2.Overrides[0].Resolve();
                if (key.DeclaringType == clrInterface)
                {
                    InterfaceAdapterMethodBodyWriter writer;
                    definition2.Attributes = (MethodAttributes) ((ushort) (definition2.Attributes & (MethodAttributes.Assembly | MethodAttributes.CheckAccessOnOverride | MethodAttributes.Family | MethodAttributes.Final | MethodAttributes.HasSecurity | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.PInvokeImpl | MethodAttributes.RequireSecObject | MethodAttributes.RTSpecialName | MethodAttributes.SpecialName | MethodAttributes.Static | MethodAttributes.UnmanagedExport | MethodAttributes.Virtual)));
                    if (adapterMethodBodyWriters.TryGetValue(key, out writer))
                    {
                        writer(definition2);
                    }
                    else
                    {
                        WriteThrowNotSupportedException(definition2.Body.GetILProcessor());
                    }
                    definition2.Body.OptimizeMacros();
                }
            }
            return item;
        }

        public static Dictionary<TypeDefinition, TypeDefinition> Generate(IEnumerable<KeyValuePair<TypeDefinition, TypeDefinition>> clrToWindowsRuntimeProjections, Dictionary<MethodDefinition, InterfaceAdapterMethodBodyWriter> adapterMethodBodyWriters)
        {
            Dictionary<TypeDefinition, TypeDefinition> dictionary = new Dictionary<TypeDefinition, TypeDefinition>();
            Dictionary<TypeDefinition, TypeDefinition> clrToWindowsRuntimeProjectedInterfaces = new Dictionary<TypeDefinition, TypeDefinition>();
            CollectProjectedInterfaces(clrToWindowsRuntimeProjections, clrToWindowsRuntimeProjectedInterfaces);
            foreach (KeyValuePair<TypeDefinition, TypeDefinition> pair in clrToWindowsRuntimeProjectedInterfaces)
            {
                TypeDefinition definition = CreateAdapterClass(pair.Key, pair.Value, adapterMethodBodyWriters);
                dictionary.Add(pair.Key, definition);
                Stats.RecordNativeToManagedInterfaceAdapter();
            }
            return dictionary;
        }

        private static void WriteThrowNotSupportedException(ILProcessor ilProcessor)
        {
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = m => (m.IsConstructor && (m.Parameters.Count == 1)) && (m.Parameters[0].ParameterType.MetadataType == MetadataType.String);
            }
            MethodDefinition method = TypeProvider.Corlib.MainModule.GetType("System", "NotSupportedException").Methods.Single<MethodDefinition>(<>f__am$cache0);
            string str = $"Cannot call method '{ilProcessor.Body.Method.FullName}'. IL2CPP does not yet support calling this projected method.";
            ilProcessor.Emit(OpCodes.Ldstr, str);
            ilProcessor.Emit(OpCodes.Newobj, method);
            ilProcessor.Emit(OpCodes.Throw);
        }
    }
}

