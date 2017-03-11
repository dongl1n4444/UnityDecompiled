namespace Unity.IL2CPP
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Unity.IL2CPP.Common;
    using Unity.IL2CPP.ILPreProcessor;

    internal static class InterfaceUtilities
    {
        [CompilerGenerated]
        private static Func<MethodDefinition, bool> <>f__am$cache0;

        public static void MakeImplementInterface(TypeDefinition type, TypeReference interfaceType)
        {
            <MakeImplementInterface>c__AnonStorey0 storey = new <MakeImplementInterface>c__AnonStorey0 {
                interfaceType = interfaceType
            };
            ModuleDefinition module = type.Module;
            if (!type.Interfaces.Any<InterfaceImplementation>(new Func<InterfaceImplementation, bool>(storey.<>m__0)))
            {
                Unity.IL2CPP.ILPreProcessor.TypeResolver resolver = Unity.IL2CPP.ILPreProcessor.TypeResolver.For(storey.interfaceType);
                foreach (InterfaceImplementation implementation in storey.interfaceType.Resolve().Interfaces)
                {
                    MakeImplementInterface(type, resolver.Resolve(implementation.InterfaceType));
                }
                type.Interfaces.Add(new InterfaceImplementation(module.ImportReference(storey.interfaceType, type)));
                Dictionary<MethodDefinition, MethodDefinition> dictionary = new Dictionary<MethodDefinition, MethodDefinition>();
                TypeDefinition definition2 = storey.interfaceType.Resolve();
                if (<>f__am$cache0 == null)
                {
                    <>f__am$cache0 = m => !m.IsStripped();
                }
                foreach (MethodDefinition definition3 in definition2.Methods.Where<MethodDefinition>(<>f__am$cache0))
                {
                    MethodReference item = resolver.Resolve(definition3);
                    TypeReference reference2 = resolver.Resolve(item.ReturnType);
                    MethodDefinition definition4 = new MethodDefinition($"{definition2.FullName}.{item.Name}", MethodAttributes.Abstract | MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.Final | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual, module.ImportReference(reference2, type));
                    type.Methods.Add(definition4);
                    definition4.Overrides.Add(item);
                    dictionary.Add(definition3, definition4);
                    foreach (ParameterDefinition definition5 in item.Parameters)
                    {
                        TypeReference reference3 = resolver.Resolve(definition5.ParameterType);
                        definition4.Parameters.Add(new ParameterDefinition(definition5.Name, definition5.Attributes, module.ImportReference(reference3, type)));
                    }
                }
                foreach (PropertyDefinition definition6 in definition2.Properties)
                {
                    TypeReference reference4 = resolver.Resolve(definition6.PropertyType);
                    PropertyDefinition definition7 = new PropertyDefinition($"{storey.interfaceType.FullName}.{definition6.Name}", definition6.Attributes, module.ImportReference(reference4, type));
                    type.Properties.Add(definition7);
                    if (definition6.GetMethod != null)
                    {
                        definition7.GetMethod = dictionary[definition6.GetMethod];
                    }
                    if (definition6.SetMethod != null)
                    {
                        definition7.SetMethod = dictionary[definition6.SetMethod];
                    }
                    foreach (MethodDefinition definition8 in definition6.OtherMethods)
                    {
                        definition7.OtherMethods.Add(dictionary[definition8]);
                    }
                }
                foreach (EventDefinition definition9 in definition2.Events)
                {
                    TypeReference reference5 = resolver.Resolve(definition9.EventType);
                    EventDefinition definition10 = new EventDefinition($"{storey.interfaceType.FullName}.{definition9.Name}", definition9.Attributes, module.ImportReference(reference5, type));
                    type.Events.Add(definition10);
                    if (definition9.AddMethod != null)
                    {
                        definition10.AddMethod = dictionary[definition9.AddMethod];
                    }
                    if (definition9.RemoveMethod != null)
                    {
                        definition10.RemoveMethod = dictionary[definition9.RemoveMethod];
                    }
                    if (definition9.InvokeMethod != null)
                    {
                        definition10.InvokeMethod = dictionary[definition9.InvokeMethod];
                    }
                    foreach (MethodDefinition definition11 in definition9.OtherMethods)
                    {
                        definition10.OtherMethods.Add(dictionary[definition11]);
                    }
                }
            }
        }

        [CompilerGenerated]
        private sealed class <MakeImplementInterface>c__AnonStorey0
        {
            internal TypeReference interfaceType;

            internal bool <>m__0(InterfaceImplementation i) => 
                Unity.IL2CPP.Common.TypeReferenceEqualityComparer.AreEqual(i.InterfaceType, this.interfaceType, TypeComparisonMode.Exact);
        }
    }
}

