namespace Unity.IL2CPP.Com
{
    using Mono.Cecil;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading;
    using Unity.IL2CPP;
    using Unity.IL2CPP.Common;
    using Unity.IL2CPP.ILPreProcessor;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;
    using Unity.IL2CPP.Marshaling;
    using Unity.IL2CPP.Marshaling.BodyWriters.NativeToManaged;
    using Unity.IL2CPP.Metadata;
    using Unity.IL2CPP.WindowsRuntime;

    public class CCWWriter : CCWWriterBase
    {
        private readonly TypeReference[] _allInteropInterfaces;
        private readonly bool _canForwardMethodsToBaseClass;
        private readonly List<MethodReference> _implementedIReferenceMethods;
        private readonly bool _implementsAnyIInspectableInterfaces;
        private readonly List<InterfaceMethodMapping> _interfaceMethodMappings;
        private readonly TypeReference[] _interfacesToForwardToBaseClass;
        private readonly List<TypeReference> _interfacesToImplement;
        private readonly GenericInstanceType _ireferenceOfType;
        private readonly string _typeName;
        [CompilerGenerated]
        private static Func<TypeDefinition, bool> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<MethodReference, bool> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<MethodDefinition, bool> <>f__am$cache2;
        [CompilerGenerated]
        private static Func<TypeReference, bool> <>f__am$cache3;
        [CompilerGenerated]
        private static Func<CustomAttribute, bool> <>f__am$cache4;
        [Inject]
        public static IStatsService Stats;
        [Inject]
        public static IWindowsRuntimeProjections WindowsRuntimeProjections;

        public CCWWriter(TypeReference type) : base(type)
        {
            this._typeName = CCWWriterBase.Naming.ForComCallableWrapperClass(type);
            this._interfaceMethodMappings = new List<InterfaceMethodMapping>();
            this._interfacesToImplement = new List<TypeReference>();
            this._implementedIReferenceMethods = new List<MethodReference>();
            if (!type.IsArray)
            {
            }
            this._canForwardMethodsToBaseClass = (<>f__am$cache0 == null) && type.Resolve().GetTypeHierarchy().Any<TypeDefinition>(<>f__am$cache0);
            this._allInteropInterfaces = type.GetInterfacesImplementedByComCallableWrapper().ToArray<TypeReference>();
            VTable table = type.IsArray ? null : new VTableBuilder().VTableFor(type, null);
            foreach (TypeReference reference in GetInterfacesToPotentiallyImplement(this._allInteropInterfaces, this.GetInterfacesToNotImplement(type)))
            {
                int num = 0;
                bool flag = type.IsArray || !table.InterfaceOffsets.TryGetValue(reference, out num);
                bool flag2 = false;
                List<InterfaceMethodMapping> collection = new List<InterfaceMethodMapping>();
                int num2 = 0;
                if (<>f__am$cache1 == null)
                {
                    <>f__am$cache1 = new Func<MethodReference, bool>(CCWWriter.<CCWWriter>m__1);
                }
                foreach (MethodReference reference2 in reference.GetMethods().Where<MethodReference>(<>f__am$cache1))
                {
                    if (!reference2.IsStripped() && !flag)
                    {
                        MethodReference managedMethod = table.Slots[num + num2];
                        collection.Add(new InterfaceMethodMapping(reference2, managedMethod));
                        num2++;
                        if (!managedMethod.DeclaringType.Resolve().IsComOrWindowsRuntimeType())
                        {
                            flag2 = true;
                        }
                    }
                    else
                    {
                        collection.Add(new InterfaceMethodMapping(reference2, null));
                    }
                }
                if ((!this._canForwardMethodsToBaseClass || flag2) || flag)
                {
                    this._interfacesToImplement.Add(reference);
                    this._interfaceMethodMappings.AddRange(collection);
                }
            }
            this._interfacesToForwardToBaseClass = this._allInteropInterfaces.Except<TypeReference>(this._interfacesToImplement, new Unity.IL2CPP.Common.TypeReferenceEqualityComparer()).ToArray<TypeReference>();
            if (base._type.CanBoxToWindowsRuntime())
            {
                this._ireferenceOfType = new GenericInstanceType(CCWWriterBase.TypeProvider.IReferenceType);
                this._ireferenceOfType.GenericArguments.Add(base._type);
                this._interfacesToImplement.Add(this._ireferenceOfType);
                this._interfacesToImplement.Add(CCWWriterBase.TypeProvider.IPropertyValueType);
                if (<>f__am$cache2 == null)
                {
                    <>f__am$cache2 = new Func<MethodDefinition, bool>(CCWWriter.<CCWWriter>m__2);
                }
                MethodDefinition method = CCWWriterBase.TypeProvider.IReferenceType.Resolve().Methods.Single<MethodDefinition>(<>f__am$cache2);
                this._implementedIReferenceMethods.Add(Unity.IL2CPP.ILPreProcessor.TypeResolver.For(this._ireferenceOfType).Resolve(method));
                foreach (MethodDefinition definition3 in CCWWriterBase.TypeProvider.IPropertyValueType.Resolve().Methods)
                {
                    this._implementedIReferenceMethods.Add(definition3);
                }
            }
            if (<>f__am$cache3 == null)
            {
                <>f__am$cache3 = new Func<TypeReference, bool>(CCWWriter.<CCWWriter>m__3);
            }
            this._implementsAnyIInspectableInterfaces = this._interfacesToImplement.Any<TypeReference>(<>f__am$cache3);
        }

        [CompilerGenerated]
        private static bool <CCWWriter>m__0(TypeDefinition t) => 
            t.IsComOrWindowsRuntimeType();

        [CompilerGenerated]
        private static bool <CCWWriter>m__1(MethodReference m) => 
            (m.HasThis && m.Resolve().IsVirtual);

        [CompilerGenerated]
        private static bool <CCWWriter>m__2(MethodDefinition m) => 
            (m.Name == "get_Value");

        [CompilerGenerated]
        private static bool <CCWWriter>m__3(TypeReference i) => 
            i.Resolve().IsWindowsRuntime;

        private string GetBaseTypeName()
        {
            StringBuilder builder = new StringBuilder("il2cpp::vm::CachedCCWBase<");
            builder.Append(this._typeName);
            builder.Append('>');
            foreach (TypeReference reference in this._interfacesToImplement)
            {
                builder.Append(", ");
                builder.Append(CCWWriterBase.Naming.ForTypeNameOnly(reference));
            }
            return builder.ToString();
        }

        private HashSet<TypeReference> GetInterfacesToNotImplement(TypeReference type)
        {
            HashSet<TypeReference> set = new HashSet<TypeReference>(new Unity.IL2CPP.Common.TypeReferenceEqualityComparer());
            if (!type.IsArray)
            {
                do
                {
                    TypeDefinition definition = type.Resolve();
                    Unity.IL2CPP.ILPreProcessor.TypeResolver resolver = Unity.IL2CPP.ILPreProcessor.TypeResolver.For(type);
                    if (definition.IsComOrWindowsRuntimeType())
                    {
                        foreach (InterfaceImplementation implementation in definition.Interfaces)
                        {
                            if (<>f__am$cache4 == null)
                            {
                                <>f__am$cache4 = ca => ca.AttributeType.FullName == "Windows.Foundation.Metadata.OverridableAttribute";
                            }
                            if (!implementation.CustomAttributes.Any<CustomAttribute>(<>f__am$cache4))
                            {
                                set.Add(resolver.Resolve(implementation.InterfaceType));
                            }
                        }
                    }
                    type = resolver.Resolve(definition.BaseType);
                }
                while (type != null);
            }
            return set;
        }

        [DebuggerHidden]
        private static IEnumerable<TypeReference> GetInterfacesToPotentiallyImplement(IEnumerable<TypeReference> allInteropInterfaces, HashSet<TypeReference> interfacesToNotImplement) => 
            new <GetInterfacesToPotentiallyImplement>c__Iterator0 { 
                allInteropInterfaces = allInteropInterfaces,
                interfacesToNotImplement = interfacesToNotImplement,
                $PC = -2
            };

        public void Write(CppCodeWriter writer)
        {
            writer.AddInclude("vm/CachedCCWBase.h");
            base.AddIncludes(writer);
            string baseTypeName = this.GetBaseTypeName();
            writer.WriteLine();
            writer.WriteCommentedLine("COM Callable Wrapper for " + base._type.FullName);
            writer.WriteLine($"struct {this._typeName} IL2CPP_FINAL : {baseTypeName}");
            using (new BlockWriter(writer, true))
            {
                writer.WriteLine($"inline {this._typeName}(Il2CppObject* obj) : il2cpp::vm::CachedCCWBase<{this._typeName}>(obj) {{}}");
                base.WriteCommonInterfaceMethods(writer);
                foreach (InterfaceMethodMapping mapping in this._interfaceMethodMappings)
                {
                    this.WriteImplementedMethodDefinition(writer, mapping);
                }
                foreach (MethodReference reference in this._implementedIReferenceMethods)
                {
                    this.WriteImplementedIReferenceMethodDefinition(writer, reference);
                }
            }
            Stats.RecordComCallableWrapper();
        }

        private void WriteImplementedIReferenceMethodDefinition(CppCodeWriter writer, MethodReference method)
        {
            <WriteImplementedIReferenceMethodDefinition>c__AnonStorey2 storey = new <WriteImplementedIReferenceMethodDefinition>c__AnonStorey2 {
                method = method,
                $this = this
            };
            string signature = ComInterfaceWriter.GetSignature(storey.method, storey.method, Unity.IL2CPP.ILPreProcessor.TypeResolver.For(this._ireferenceOfType), null, true);
            this.WriteMethodDefinition(writer, signature, null, storey.method, new Action<CppCodeWriter, IRuntimeMetadataAccess>(storey.<>m__0));
            Stats.RecordImplementedComCallableWrapperMethod();
        }

        private void WriteImplementedMethodDefinition(CppCodeWriter writer, InterfaceMethodMapping mapping)
        {
            MethodReference managedMethod;
            <WriteImplementedMethodDefinition>c__AnonStorey1 storey = new <WriteImplementedMethodDefinition>c__AnonStorey1 {
                mapping = mapping,
                $this = this
            };
            storey.marshalType = !storey.mapping.InterfaceMethod.DeclaringType.Resolve().IsWindowsRuntime ? MarshalType.COM : MarshalType.WindowsRuntime;
            if (storey.mapping.ManagedMethod != null)
            {
                managedMethod = storey.mapping.ManagedMethod;
            }
            else
            {
                managedMethod = storey.mapping.InterfaceMethod;
            }
            string signature = ComInterfaceWriter.GetSignature(managedMethod, storey.mapping.InterfaceMethod, Unity.IL2CPP.ILPreProcessor.TypeResolver.For(managedMethod.DeclaringType), null, true);
            this.WriteMethodDefinition(writer, signature, storey.mapping.ManagedMethod, storey.mapping.InterfaceMethod, new Action<CppCodeWriter, IRuntimeMetadataAccess>(storey.<>m__0));
        }

        private void WriteMethodDefinition(CppCodeWriter writer, string signature, MethodReference managedMethod, MethodReference interfaceMethod, Action<CppCodeWriter, IRuntimeMetadataAccess> writeAction)
        {
            <WriteMethodDefinition>c__AnonStorey3 storey = new <WriteMethodDefinition>c__AnonStorey3 {
                managedMethod = managedMethod,
                interfaceMethod = interfaceMethod,
                writeAction = writeAction
            };
            writer.WriteLine();
            if (storey.managedMethod == null)
            {
            }
            MethodWriter.WriteMethodWithMetadataInitialization(writer, signature, storey.interfaceMethod.FullName, new Action<CppCodeWriter, MetadataUsage, MethodUsage>(storey.<>m__0), CCWWriterBase.Naming.ForMethod(storey.interfaceMethod) + "_CCW_" + ((storey.managedMethod == null) ? this._typeName : (this._typeName + "_" + CCWWriterBase.Naming.ForMethod(storey.managedMethod))));
        }

        protected override IEnumerable<TypeReference> AllImplementedInterfaces =>
            this._interfacesToImplement;

        protected override bool ImplementsAnyIInspectableInterfaces =>
            this._implementsAnyIInspectableInterfaces;

        protected override IList<TypeReference> InterfacesToForwardToBaseClass =>
            this._interfacesToForwardToBaseClass;

        [CompilerGenerated]
        private sealed class <GetInterfacesToPotentiallyImplement>c__Iterator0 : IEnumerable, IEnumerable<TypeReference>, IEnumerator, IDisposable, IEnumerator<TypeReference>
        {
            internal TypeReference $current;
            internal bool $disposing;
            internal IEnumerator<TypeReference> $locvar0;
            internal int $PC;
            internal TypeReference <interfaceType>__1;
            internal IEnumerable<TypeReference> allInteropInterfaces;
            internal HashSet<TypeReference> interfacesToNotImplement;

            [DebuggerHidden]
            public void Dispose()
            {
                uint num = (uint) this.$PC;
                this.$disposing = true;
                this.$PC = -1;
                switch (num)
                {
                    case 1:
                        try
                        {
                        }
                        finally
                        {
                            if (this.$locvar0 != null)
                            {
                                this.$locvar0.Dispose();
                            }
                        }
                        break;
                }
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                bool flag = false;
                switch (num)
                {
                    case 0:
                        this.$locvar0 = this.allInteropInterfaces.GetEnumerator();
                        num = 0xfffffffd;
                        break;

                    case 1:
                        break;

                    default:
                        goto Label_00CC;
                }
                try
                {
                    switch (num)
                    {
                        case 1:
                            goto Label_0094;
                    }
                    while (this.$locvar0.MoveNext())
                    {
                        this.<interfaceType>__1 = this.$locvar0.Current;
                        if (!this.interfacesToNotImplement.Contains(this.<interfaceType>__1))
                        {
                            this.$current = this.<interfaceType>__1;
                            if (!this.$disposing)
                            {
                                this.$PC = 1;
                            }
                            flag = true;
                            return true;
                        }
                    Label_0094:;
                    }
                }
                finally
                {
                    if (!flag)
                    {
                    }
                    if (this.$locvar0 != null)
                    {
                        this.$locvar0.Dispose();
                    }
                }
                this.$PC = -1;
            Label_00CC:
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            [DebuggerHidden]
            IEnumerator<TypeReference> IEnumerable<TypeReference>.GetEnumerator()
            {
                if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
                {
                    return this;
                }
                return new CCWWriter.<GetInterfacesToPotentiallyImplement>c__Iterator0 { 
                    allInteropInterfaces = this.allInteropInterfaces,
                    interfacesToNotImplement = this.interfacesToNotImplement
                };
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator() => 
                this.System.Collections.Generic.IEnumerable<Mono.Cecil.TypeReference>.GetEnumerator();

            TypeReference IEnumerator<TypeReference>.Current =>
                this.$current;

            object IEnumerator.Current =>
                this.$current;
        }

        [CompilerGenerated]
        private sealed class <WriteImplementedIReferenceMethodDefinition>c__AnonStorey2
        {
            internal CCWWriter $this;
            internal MethodReference method;

            internal void <>m__0(CppCodeWriter bodyWriter, IRuntimeMetadataAccess metadataAccess)
            {
                new IReferenceComCallableWrapperMethodBodyWriter(this.method, this.$this._type).WriteMethodBody(bodyWriter, metadataAccess);
            }
        }

        [CompilerGenerated]
        private sealed class <WriteImplementedMethodDefinition>c__AnonStorey1
        {
            internal CCWWriter $this;
            internal CCWWriter.InterfaceMethodMapping mapping;
            internal MarshalType marshalType;

            internal void <>m__0(CppCodeWriter bodyWriter, IRuntimeMetadataAccess metadataAccess)
            {
                if (this.mapping.InterfaceMethod.IsStripped())
                {
                    bodyWriter.WriteCommentedLine("Managed method has been stripped");
                    bodyWriter.WriteLine("return IL2CPP_E_ILLEGAL_METHOD_CALL;");
                    CCWWriter.Stats.RecordStrippedComCallableWrapperMethod();
                }
                else if (this.mapping.ManagedMethod == null)
                {
                    Unity.IL2CPP.ILPreProcessor.TypeResolver typeResolver = Unity.IL2CPP.ILPreProcessor.TypeResolver.For(this.mapping.InterfaceMethod.DeclaringType);
                    string str = CCWWriterBase.Naming.ForComCallableWrapperProjectedMethod(this.mapping.InterfaceMethod);
                    string str2 = MethodSignatureWriter.FormatComMethodParameterList(this.mapping.InterfaceMethod, this.mapping.InterfaceMethod, typeResolver, this.marshalType, false);
                    bodyWriter.AddMethodForwardDeclaration(MethodSignatureWriter.FormatProjectedComCallableWrapperMethodDeclaration(this.mapping.InterfaceMethod, typeResolver, this.marshalType));
                    if (string.IsNullOrEmpty(str2))
                    {
                        bodyWriter.WriteLine($"return {str}(GetManagedObjectInline());");
                    }
                    else
                    {
                        bodyWriter.WriteLine($"return {str}(GetManagedObjectInline(), {str2});");
                    }
                }
                else if (!this.mapping.ManagedMethod.Resolve().DeclaringType.IsComOrWindowsRuntimeType())
                {
                    new ComCallableWrapperMethodBodyWriter(this.mapping.ManagedMethod, this.mapping.InterfaceMethod, this.marshalType).WriteMethodBody(bodyWriter, metadataAccess);
                    CCWWriter.Stats.RecordImplementedComCallableWrapperMethod();
                }
                else
                {
                    string str3 = CCWWriterBase.Naming.ForVariable(this.$this._type);
                    string str4 = CCWWriterBase.Naming.ForComTypeInterfaceFieldGetter(this.mapping.InterfaceMethod.DeclaringType);
                    string str5 = CCWWriterBase.Naming.ForMethod(this.mapping.InterfaceMethod);
                    string str6 = MethodSignatureWriter.FormatComMethodParameterList(this.mapping.ManagedMethod, this.mapping.InterfaceMethod, Unity.IL2CPP.ILPreProcessor.TypeResolver.For(this.$this._type), this.marshalType, false);
                    bodyWriter.WriteLine($"return (({str3})GetManagedObjectInline())->{str4}()->{str5}({str6});");
                    CCWWriter.Stats.RecordForwardedToBaseClassComCallableWrapperMethod();
                }
            }
        }

        [CompilerGenerated]
        private sealed class <WriteMethodDefinition>c__AnonStorey3
        {
            internal MethodReference interfaceMethod;
            internal MethodReference managedMethod;
            internal Action<CppCodeWriter, IRuntimeMetadataAccess> writeAction;

            internal void <>m__0(CppCodeWriter bodyWriter, MetadataUsage metadataUsage, MethodUsage methodUsage)
            {
                if (this.managedMethod == null)
                {
                }
                IRuntimeMetadataAccess access = MethodWriter.GetDefaultRuntimeMetadataAccess(this.interfaceMethod, metadataUsage, methodUsage, null);
                this.writeAction(bodyWriter, access);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct InterfaceImplementationMapping
        {
            public readonly TypeReference DeclaringType;
            public readonly Mono.Cecil.InterfaceImplementation InterfaceImplementation;
            public InterfaceImplementationMapping(TypeReference declaringType, Mono.Cecil.InterfaceImplementation interfaceImplementation)
            {
                this.DeclaringType = declaringType;
                this.InterfaceImplementation = interfaceImplementation;
            }
        }

        private sealed class InterfaceMethodMapping
        {
            public readonly MethodReference InterfaceMethod;
            public MethodReference ManagedMethod;

            public InterfaceMethodMapping(MethodReference interfaceMethod, MethodReference managedMethod)
            {
                this.InterfaceMethod = interfaceMethod;
                this.ManagedMethod = managedMethod;
            }
        }
    }
}

