namespace Unity.IL2CPP.Metadata
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using Unity.IL2CPP;
    using Unity.IL2CPP.Common;
    using Unity.IL2CPP.ILPreProcessor;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;

    public class VTableBuilder
    {
        private readonly Dictionary<MethodReference, int> _methodSlots = new Dictionary<MethodReference, int>(new Unity.IL2CPP.Common.MethodReferenceComparer());
        private readonly Dictionary<TypeReference, VTable> _vtables = new Dictionary<TypeReference, VTable>(new Unity.IL2CPP.Common.TypeReferenceEqualityComparer());
        [CompilerGenerated]
        private static Func<MethodDefinition, bool> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<MethodDefinition, bool> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<MethodDefinition, bool> <>f__am$cache2;
        [CompilerGenerated]
        private static Func<MethodDefinition, bool> <>f__am$cache3;
        [CompilerGenerated]
        private static Func<MethodDefinition, bool> <>f__am$cache4;
        [CompilerGenerated]
        private static Func<MethodDefinition, bool> <>f__am$cache5;
        [Inject]
        public static IGenericSharingAnalysisService GenericSharingAnalysis;

        private static bool CheckInterfaceMethodOverride(MethodReference itfMethod, MethodReference virtualMethod, bool requireNewslot, bool interfaceIsExplicitlyImplementedByClass, bool slotIsEmpty)
        {
            if (itfMethod.Name == virtualMethod.Name)
            {
                if (!virtualMethod.Resolve().IsPublic)
                {
                    return false;
                }
                if (!slotIsEmpty && requireNewslot)
                {
                    if (!interfaceIsExplicitlyImplementedByClass)
                    {
                        return false;
                    }
                    if (!virtualMethod.Resolve().IsNewSlot)
                    {
                        return false;
                    }
                }
                return VirtualMethodResolution.MethodSignaturesMatch(itfMethod, virtualMethod);
            }
            return false;
        }

        internal static MethodReference CloneMethodReference(GenericInstanceType genericInstanceType, MethodReference method)
        {
            MethodReference owner = new MethodReference(method.Name, method.ReturnType, genericInstanceType) {
                HasThis = method.HasThis,
                ExplicitThis = method.ExplicitThis,
                CallingConvention = method.CallingConvention
            };
            foreach (ParameterDefinition definition in method.Parameters)
            {
                owner.Parameters.Add(new ParameterDefinition(definition.Name, definition.Attributes, definition.ParameterType));
            }
            foreach (GenericParameter parameter in method.GenericParameters)
            {
                owner.GenericParameters.Add(new GenericParameter(parameter.Name, owner));
            }
            return owner;
        }

        private static Dictionary<MethodReference, MethodDefinition> CollectOverrides(TypeDefinition typeDefinition)
        {
            Dictionary<MethodReference, MethodDefinition> dictionary = new Dictionary<MethodReference, MethodDefinition>(new Unity.IL2CPP.Common.MethodReferenceComparer());
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = new Func<MethodDefinition, bool>(null, (IntPtr) <CollectOverrides>m__2);
            }
            foreach (MethodDefinition definition in Enumerable.Where<MethodDefinition>(typeDefinition.Methods, <>f__am$cache2))
            {
                foreach (MethodReference reference in definition.Overrides)
                {
                    dictionary.Add(reference, definition);
                }
            }
            return dictionary;
        }

        private int GetSlot(MethodReference method)
        {
            return this._methodSlots[method];
        }

        internal MethodReference GetVirtualMethodTargetMethodForConstrainedCallOnValueType(TypeReference type, MethodReference method)
        {
            MethodDefinition definition = method.Resolve();
            if (!definition.IsVirtual)
            {
                return method;
            }
            int num = this.IndexFor(definition);
            VTable table = this.VTableFor(type, null);
            if (Extensions.IsInterface(method.DeclaringType))
            {
                int num2;
                if (table.InterfaceOffsets.TryGetValue(method.DeclaringType, out num2))
                {
                    return table.Slots[num2 + num];
                }
                return null;
            }
            return table.Slots[num];
        }

        public int IndexFor(MethodDefinition method)
        {
            if (method.DeclaringType.IsInterface)
            {
                this.SetupMethodSlotsForInterface(method.DeclaringType);
                return this.GetSlot(method);
            }
            this.VTableFor(method.DeclaringType, null);
            if (!method.IsVirtual)
            {
                return 0xffff;
            }
            return this._methodSlots[method];
        }

        internal static MethodReference InflatedMethodFromMethodDefinition(GenericInstanceType genericInstanceType, MethodDefinition method)
        {
            if (!method.DeclaringType.HasGenericParameters)
            {
                throw new Exception("Declaring type should be generic");
            }
            if (method.DeclaringType.Resolve() != genericInstanceType.Resolve())
            {
                throw new NotSupportedException();
            }
            return CloneMethodReference(genericInstanceType, method);
        }

        private static bool InterfaceIsExplicitlyImplementedByClass(TypeDefinition typeDefinition, TypeReference itf)
        {
            <InterfaceIsExplicitlyImplementedByClass>c__AnonStorey1 storey = new <InterfaceIsExplicitlyImplementedByClass>c__AnonStorey1 {
                itf = itf
            };
            return ((typeDefinition.BaseType == null) || Enumerable.Any<InterfaceImplementation>(typeDefinition.Interfaces, new Func<InterfaceImplementation, bool>(storey, (IntPtr) this.<>m__0)));
        }

        private void OverrideInterfaceMethods(Dictionary<TypeReference, int> interfaceOffsets, List<MethodReference> slots, Dictionary<MethodReference, MethodDefinition> overrides, Dictionary<MethodReference, MethodReference> overrideMap)
        {
            foreach (KeyValuePair<MethodReference, MethodDefinition> pair in overrides)
            {
                MethodReference key = pair.Key;
                if (key.DeclaringType.Resolve().IsInterface)
                {
                    int slot = this.GetSlot(key) + interfaceOffsets[pair.Key.DeclaringType];
                    slots[slot] = pair.Value;
                    this.SetSlot(pair.Value, slot);
                    overrideMap.Add(pair.Key, pair.Value);
                }
            }
        }

        private void OverrideNonInterfaceMethods(Dictionary<MethodReference, MethodDefinition> overrides, List<MethodReference> slots, Dictionary<MethodReference, MethodReference> overrideMap)
        {
            foreach (KeyValuePair<MethodReference, MethodDefinition> pair in overrides)
            {
                MethodReference key = pair.Key;
                MethodDefinition method = pair.Value;
                TypeReference declaringType = key.DeclaringType;
                if (!declaringType.Resolve().IsInterface)
                {
                    MethodReference reference3;
                    int slot = this.GetSlot(key);
                    slots[slot] = method;
                    this.SetSlot(method, slot);
                    overrideMap.TryGetValue(key, out reference3);
                    if (reference3 != null)
                    {
                        if (!Unity.IL2CPP.Common.MethodReferenceComparer.AreEqual(reference3, method))
                        {
                            throw new InvalidOperationException(string.Format("Error while creating VTable for {0}. The base method {1} is implemented both by {2} and {3}.", new object[] { declaringType, key, reference3, method }));
                        }
                    }
                    else
                    {
                        overrideMap.Add(key, method);
                    }
                }
            }
        }

        private static void ReplaceOverridenMethods(Dictionary<MethodReference, MethodReference> overrideMap, List<MethodReference> slots)
        {
            if (overrideMap.Count > 0)
            {
                for (int i = 0; i < slots.Count; i++)
                {
                    MethodReference reference;
                    if ((slots[i] != null) && overrideMap.TryGetValue(slots[i], out reference))
                    {
                        slots[i] = reference;
                    }
                }
            }
        }

        private void SetSlot(MethodReference method, int slot)
        {
            this._methodSlots[method] = slot;
        }

        private void SetupClassMethods(List<MethodReference> slots, TypeDefinition typeDefinition, Dictionary<MethodReference, MethodReference> overrideMap)
        {
            if (<>f__am$cache5 == null)
            {
                <>f__am$cache5 = new Func<MethodDefinition, bool>(null, (IntPtr) <SetupClassMethods>m__5);
            }
            foreach (MethodDefinition definition in Enumerable.Where<MethodDefinition>(typeDefinition.Methods, <>f__am$cache5))
            {
                if (!definition.IsNewSlot)
                {
                    int num = -1;
                    for (TypeReference reference = Extensions.GetBaseType(typeDefinition); reference != null; reference = Extensions.GetBaseType(reference))
                    {
                        foreach (MethodReference reference2 in Extensions.GetVirtualMethods(reference))
                        {
                            if ((definition.Name == reference2.Name) && VirtualMethodResolution.MethodSignaturesMatch(definition, reference2))
                            {
                                num = this.GetSlot(reference2);
                                overrideMap.Add(reference2, definition);
                                break;
                            }
                        }
                        if (num >= 0)
                        {
                            break;
                        }
                    }
                    if (num >= 0)
                    {
                        this.SetSlot(definition, num);
                    }
                }
                if ((definition.IsNewSlot && !definition.IsFinal) && this._methodSlots.ContainsKey(definition))
                {
                    this._methodSlots.Remove(definition);
                }
                if (!this._methodSlots.ContainsKey(definition))
                {
                    int count = slots.Count;
                    slots.Add(null);
                    this.SetSlot(definition, count);
                }
                int slot = this.GetSlot(definition);
                slots[slot] = (!definition.IsAbstract || Extensions.IsComOrWindowsRuntimeInterface(typeDefinition)) ? definition : null;
            }
        }

        private void SetupInterfaceMethods(TypeDefinition typeDefinition, Dictionary<TypeReference, int> interfaceOffsets, Dictionary<MethodReference, MethodReference> overrideMap, List<MethodReference> slots)
        {
            foreach (KeyValuePair<TypeReference, int> pair in interfaceOffsets)
            {
                <SetupInterfaceMethods>c__AnonStorey0 storey = new <SetupInterfaceMethods>c__AnonStorey0 {
                    itf = pair.Key
                };
                int num = pair.Value;
                TypeDefinition definition = storey.itf.Resolve();
                this.SetupMethodSlotsForInterface(storey.itf);
                bool interfaceIsExplicitlyImplementedByClass = InterfaceIsExplicitlyImplementedByClass(typeDefinition, storey.itf);
                if (<>f__am$cache3 == null)
                {
                    <>f__am$cache3 = new Func<MethodDefinition, bool>(null, (IntPtr) <SetupInterfaceMethods>m__3);
                }
                foreach (MethodReference reference in Enumerable.Select<MethodDefinition, MethodReference>(Enumerable.Where<MethodDefinition>(definition.Methods, <>f__am$cache3), new Func<MethodDefinition, MethodReference>(storey, (IntPtr) this.<>m__0)))
                {
                    MethodReference reference2;
                    int slot = num + this.GetSlot(reference);
                    if (!overrideMap.TryGetValue(reference, out reference2))
                    {
                        foreach (MethodReference reference3 in Extensions.GetVirtualMethods(typeDefinition))
                        {
                            if (CheckInterfaceMethodOverride(reference, reference3, true, interfaceIsExplicitlyImplementedByClass, slots[slot] == null))
                            {
                                slots[slot] = reference3;
                                this.SetSlot(reference3, slot);
                            }
                        }
                        if ((slots[slot] == null) && (typeDefinition.BaseType != null))
                        {
                            VTable table = this.VTableFor(typeDefinition.BaseType, null);
                            for (int i = table.Slots.Count - 1; i >= 0; i--)
                            {
                                MethodReference virtualMethod = table.Slots[i];
                                if ((virtualMethod != null) && CheckInterfaceMethodOverride(reference, virtualMethod, false, false, true))
                                {
                                    slots[slot] = virtualMethod;
                                    if (!this._methodSlots.ContainsKey(virtualMethod))
                                    {
                                        this.SetSlot(virtualMethod, slot);
                                    }
                                }
                            }
                        }
                    }
                    else if (slots[slot] != reference2)
                    {
                        throw new Exception();
                    }
                }
            }
        }

        private Dictionary<TypeReference, int> SetupInterfaceOffsets(TypeReference type, ref int currentSlot)
        {
            Dictionary<TypeReference, int> dictionary = new Dictionary<TypeReference, int>(new Unity.IL2CPP.Common.TypeReferenceEqualityComparer());
            for (TypeReference reference = Extensions.GetBaseType(type); reference != null; reference = Extensions.GetBaseType(reference))
            {
                VTable table = this.VTableFor(reference, null);
                foreach (TypeReference reference2 in Extensions.GetInterfaces(reference))
                {
                    this.SetupMethodSlotsForInterface(reference2);
                    int num = table.InterfaceOffsets[reference2];
                    dictionary[reference2] = num;
                }
            }
            foreach (TypeReference reference3 in Extensions.GetInterfaces(type))
            {
                if (!dictionary.ContainsKey(reference3))
                {
                    this.SetupMethodSlotsForInterface(reference3);
                    dictionary.Add(reference3, currentSlot);
                    currentSlot += VirtualMethodCount(reference3);
                }
            }
            return dictionary;
        }

        private void SetupMethodSlotsForInterface(TypeReference typeReference)
        {
            if (!typeReference.Resolve().IsInterface)
            {
                throw new Exception();
            }
            int num = 0;
            TypeDefinition definition = typeReference.Resolve();
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = new Func<MethodDefinition, bool>(null, (IntPtr) <SetupMethodSlotsForInterface>m__1);
            }
            foreach (MethodDefinition definition2 in Enumerable.Where<MethodDefinition>(definition.Methods, <>f__am$cache1))
            {
                this.SetSlot(Unity.IL2CPP.ILPreProcessor.TypeResolver.For(typeReference).Resolve(definition2), num++);
            }
        }

        private static void ValidateAllMethodSlots(TypeDefinition typeDefinition, IEnumerable<MethodReference> slots)
        {
            if (!typeDefinition.IsAbstract)
            {
                foreach (MethodReference reference in slots)
                {
                    if (((reference == null) || (reference.Resolve().IsAbstract && !Extensions.IsComOrWindowsRuntimeInterface(reference.Resolve().DeclaringType))) || reference.Resolve().IsStatic)
                    {
                        throw new Exception(string.Format("Invalid method '{0}' found in vtable for '{1}'", (reference != null) ? reference.FullName : "null", typeDefinition.FullName));
                    }
                }
            }
        }

        private void ValidateInterfaceMethodSlots(TypeDefinition typeDefinition, Dictionary<TypeReference, int> interfaceOffsets, List<MethodReference> slots)
        {
            if (!typeDefinition.IsAbstract)
            {
                foreach (KeyValuePair<TypeReference, int> pair in interfaceOffsets)
                {
                    TypeReference key = pair.Key;
                    int num = pair.Value;
                    Unity.IL2CPP.ILPreProcessor.TypeResolver resolver = Unity.IL2CPP.ILPreProcessor.TypeResolver.For(key);
                    if (<>f__am$cache4 == null)
                    {
                        <>f__am$cache4 = new Func<MethodDefinition, bool>(null, (IntPtr) <ValidateInterfaceMethodSlots>m__4);
                    }
                    foreach (MethodReference reference2 in Enumerable.Select<MethodDefinition, MethodReference>(Enumerable.Where<MethodDefinition>(key.Resolve().Methods, <>f__am$cache4), new Func<MethodDefinition, MethodReference>(resolver, (IntPtr) this.Resolve)))
                    {
                        int num2 = num + this.GetSlot(reference2);
                        if (slots[num2] == null)
                        {
                            throw new Exception(string.Format("Interface {0} method {1} not implemented on non-abstract class {2}", key.FullName, reference2.FullName, typeDefinition.FullName));
                        }
                    }
                }
            }
        }

        private static int VirtualMethodCount(TypeReference type)
        {
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = new Func<MethodDefinition, bool>(null, (IntPtr) <VirtualMethodCount>m__0);
            }
            return Enumerable.Count<MethodDefinition>(type.Resolve().Methods, <>f__am$cache0);
        }

        public VTable VTableFor(TypeReference typeReference, [Optional, DefaultParameterValue(null)] Unity.IL2CPP.ILPreProcessor.TypeResolver resolver)
        {
            VTable table;
            if (this._vtables.TryGetValue(typeReference, out table))
            {
                return table;
            }
            TypeDefinition definition = typeReference.Resolve();
            if (definition.IsInterface && !Extensions.IsComOrWindowsRuntimeType(definition))
            {
                throw new Exception();
            }
            int currentSlot = (definition.BaseType != null) ? this.VTableFor(Extensions.GetBaseType(typeReference), null).Slots.Count : 0;
            Dictionary<TypeReference, int> interfaceOffsets = this.SetupInterfaceOffsets(typeReference, ref currentSlot);
            GenericInstanceType genericInstanceType = typeReference as GenericInstanceType;
            ErrorInformation.CurrentlyProcessing.Type = definition;
            return ((genericInstanceType == null) ? this.VTableForType(definition, interfaceOffsets, currentSlot) : this.VTableForGenericInstance(genericInstanceType, interfaceOffsets));
        }

        private VTable VTableForGenericInstance(GenericInstanceType genericInstanceType, Dictionary<TypeReference, int> offsets)
        {
            TypeDefinition typeReference = genericInstanceType.Resolve();
            List<MethodReference> list = new List<MethodReference>(this.VTableFor(typeReference, null).Slots);
            Unity.IL2CPP.ILPreProcessor.TypeResolver resolver = new Unity.IL2CPP.ILPreProcessor.TypeResolver(genericInstanceType);
            for (int i = 0; i < list.Count; i++)
            {
                MethodReference method = list[i];
                if (method != null)
                {
                    MethodReference reference2 = resolver.Resolve(method);
                    list[i] = reference2;
                    this.SetSlot(reference2, this.GetSlot(method));
                }
            }
            for (int j = 0; j < typeReference.Methods.Count; j++)
            {
                MethodDefinition definition2 = typeReference.Methods[j];
                if (definition2.IsVirtual)
                {
                    MethodReference key = resolver.Resolve(definition2);
                    if (!this._methodSlots.ContainsKey(key))
                    {
                        int slot = this.GetSlot(definition2);
                        this.SetSlot(key, slot);
                    }
                }
            }
            VTable table2 = new VTable(list.AsReadOnly(), offsets);
            this._vtables[genericInstanceType] = table2;
            return table2;
        }

        private VTable VTableForType(TypeDefinition typeDefinition, Dictionary<TypeReference, int> interfaceOffsets, int currentSlot)
        {
            TypeReference baseType = typeDefinition.BaseType;
            List<MethodReference> slots = (baseType == null) ? new List<MethodReference>() : new List<MethodReference>(this.VTableFor(baseType, null).Slots);
            if (currentSlot > slots.Count)
            {
                slots.AddRange(new MethodReference[currentSlot - slots.Count]);
            }
            Dictionary<MethodReference, MethodDefinition> overrides = CollectOverrides(typeDefinition);
            Dictionary<MethodReference, MethodReference> overrideMap = new Dictionary<MethodReference, MethodReference>(new Unity.IL2CPP.Common.MethodReferenceComparer());
            this.OverrideInterfaceMethods(interfaceOffsets, slots, overrides, overrideMap);
            this.SetupInterfaceMethods(typeDefinition, interfaceOffsets, overrideMap, slots);
            this.ValidateInterfaceMethodSlots(typeDefinition, interfaceOffsets, slots);
            this.SetupClassMethods(slots, typeDefinition, overrideMap);
            this.OverrideNonInterfaceMethods(overrides, slots, overrideMap);
            ReplaceOverridenMethods(overrideMap, slots);
            ValidateAllMethodSlots(typeDefinition, slots);
            VTable table = new VTable(slots.AsReadOnly(), (!typeDefinition.IsInterface || Extensions.IsComOrWindowsRuntimeInterface(typeDefinition)) ? interfaceOffsets : null);
            this._vtables[typeDefinition] = table;
            return table;
        }

        [CompilerGenerated]
        private sealed class <InterfaceIsExplicitlyImplementedByClass>c__AnonStorey1
        {
            internal TypeReference itf;

            internal bool <>m__0(InterfaceImplementation classItf)
            {
                return Unity.IL2CPP.Common.TypeReferenceEqualityComparer.AreEqual(this.itf, classItf.InterfaceType, TypeComparisonMode.Exact);
            }
        }

        [CompilerGenerated]
        private sealed class <SetupInterfaceMethods>c__AnonStorey0
        {
            internal TypeReference itf;

            internal MethodReference <>m__0(MethodDefinition m)
            {
                return Unity.IL2CPP.ILPreProcessor.TypeResolver.For(this.itf).Resolve(m);
            }
        }
    }
}

