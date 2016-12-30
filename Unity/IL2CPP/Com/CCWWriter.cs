namespace Unity.IL2CPP.Com
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;
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
        private readonly TypeReference[] _windowsRuntimeProjectedInterfaces;
        [CompilerGenerated]
        private static Func<TypeDefinition, bool> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<InterfaceImplementationMapping, TypeReference> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<MethodReference, bool> <>f__am$cache2;
        [CompilerGenerated]
        private static Func<InterfaceImplementationMapping, TypeReference> <>f__am$cache3;
        [CompilerGenerated]
        private static Func<MethodDefinition, bool> <>f__am$cache4;
        [CompilerGenerated]
        private static Func<TypeReference, bool> <>f__am$cache5;
        [CompilerGenerated]
        private static Func<CustomAttribute, bool> <>f__am$cache6;
        [CompilerGenerated]
        private static Func<InterfaceImplementationMapping, bool> <>f__mg$cache0;
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
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = new Func<TypeDefinition, bool>(CCWWriter.<CCWWriter>m__0);
            }
            this._canForwardMethodsToBaseClass = type.Resolve().GetTypeHierarchy().Any<TypeDefinition>(<>f__am$cache0);
            IEnumerable<InterfaceImplementationMapping> allImplementedInteropInterfacesInTypeHierarchy = GetAllImplementedInteropInterfacesInTypeHierarchy(base._type);
            if (<>f__mg$cache0 == null)
            {
                <>f__mg$cache0 = new Func<InterfaceImplementationMapping, bool>(CCWWriter.CanImplementInCCW);
            }
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = new Func<InterfaceImplementationMapping, TypeReference>(CCWWriter.<CCWWriter>m__1);
            }
            IEnumerable<TypeReference> enumerable2 = allImplementedInteropInterfacesInTypeHierarchy.Where<InterfaceImplementationMapping>(<>f__mg$cache0).Select<InterfaceImplementationMapping, TypeReference>(<>f__am$cache1).Distinct<TypeReference>(new Unity.IL2CPP.Common.TypeReferenceEqualityComparer());
            VTable table = new VTableBuilder().VTableFor(type, null);
            foreach (TypeReference reference in enumerable2)
            {
                int num = table.InterfaceOffsets[reference];
                if (<>f__am$cache2 == null)
                {
                    <>f__am$cache2 = new Func<MethodReference, bool>(CCWWriter.<CCWWriter>m__2);
                }
                IEnumerable<MethodReference> enumerable3 = reference.GetMethods().Where<MethodReference>(<>f__am$cache2);
                bool flag = false;
                List<InterfaceMethodMapping> collection = new List<InterfaceMethodMapping>();
                int num2 = 0;
                foreach (MethodReference reference2 in enumerable3)
                {
                    if (!reference2.IsStripped())
                    {
                        MethodReference managedMethod = table.Slots[num + num2];
                        collection.Add(new InterfaceMethodMapping(reference2, managedMethod));
                        num2++;
                        if (!managedMethod.DeclaringType.Resolve().IsComOrWindowsRuntimeType())
                        {
                            flag = true;
                        }
                    }
                    else
                    {
                        collection.Add(new InterfaceMethodMapping(reference2, null));
                    }
                }
                if (!this._canForwardMethodsToBaseClass || flag)
                {
                    this._interfacesToImplement.Add(reference);
                    this._interfaceMethodMappings.AddRange(collection);
                }
            }
            if (<>f__am$cache3 == null)
            {
                <>f__am$cache3 = new Func<InterfaceImplementationMapping, TypeReference>(CCWWriter.<CCWWriter>m__3);
            }
            this._allInteropInterfaces = allImplementedInteropInterfacesInTypeHierarchy.Select<InterfaceImplementationMapping, TypeReference>(<>f__am$cache3).ToArray<TypeReference>();
            this._interfacesToForwardToBaseClass = this._allInteropInterfaces.Except<TypeReference>(this._interfacesToImplement, new Unity.IL2CPP.Common.TypeReferenceEqualityComparer()).ToArray<TypeReference>();
            this._windowsRuntimeProjectedInterfaces = base._type.ImplementedWindowsRuntimeProjectedInterfaces().ToArray<TypeReference>();
            if (base._type.CanBoxToWindowsRuntime())
            {
                this._ireferenceOfType = new GenericInstanceType(CCWWriterBase.TypeProvider.IReferenceType);
                this._ireferenceOfType.GenericArguments.Add(base._type);
                this._interfacesToImplement.Add(this._ireferenceOfType);
                this._interfacesToImplement.Add(CCWWriterBase.TypeProvider.IPropertyValueType);
                if (<>f__am$cache4 == null)
                {
                    <>f__am$cache4 = new Func<MethodDefinition, bool>(CCWWriter.<CCWWriter>m__4);
                }
                MethodDefinition method = CCWWriterBase.TypeProvider.IReferenceType.Resolve().Methods.Single<MethodDefinition>(<>f__am$cache4);
                this._implementedIReferenceMethods.Add(Unity.IL2CPP.ILPreProcessor.TypeResolver.For(this._ireferenceOfType).Resolve(method));
                foreach (MethodDefinition definition3 in CCWWriterBase.TypeProvider.IPropertyValueType.Resolve().Methods)
                {
                    this._implementedIReferenceMethods.Add(definition3);
                }
            }
            if (this._windowsRuntimeProjectedInterfaces.Length <= 0)
            {
            }
            this._implementsAnyIInspectableInterfaces = (<>f__am$cache5 != null) || this._interfacesToImplement.Any<TypeReference>(<>f__am$cache5);
        }

        [CompilerGenerated]
        private static bool <CCWWriter>m__0(TypeDefinition t) => 
            t.IsComOrWindowsRuntimeType();

        [CompilerGenerated]
        private static TypeReference <CCWWriter>m__1(InterfaceImplementationMapping i) => 
            Unity.IL2CPP.ILPreProcessor.TypeResolver.For(i.DeclaringType).Resolve(i.InterfaceImplementation.InterfaceType);

        [CompilerGenerated]
        private static bool <CCWWriter>m__2(MethodReference m) => 
            (m.HasThis && m.Resolve().IsVirtual);

        [CompilerGenerated]
        private static TypeReference <CCWWriter>m__3(InterfaceImplementationMapping i) => 
            Unity.IL2CPP.ILPreProcessor.TypeResolver.For(i.DeclaringType).Resolve(i.InterfaceImplementation.InterfaceType);

        [CompilerGenerated]
        private static bool <CCWWriter>m__4(MethodDefinition m) => 
            (m.Name == "get_Value");

        [CompilerGenerated]
        private static bool <CCWWriter>m__5(TypeReference i) => 
            i.Resolve().IsWindowsRuntime;

        private static bool CanImplementInCCW(InterfaceImplementationMapping mapping)
        {
            if (!mapping.DeclaringType.Resolve().IsComOrWindowsRuntimeType())
            {
                return true;
            }
            if (<>f__am$cache6 == null)
            {
                <>f__am$cache6 = ca => ca.AttributeType.FullName == "Windows.Foundation.Metadata.OverridableAttribute";
            }
            return mapping.InterfaceImplementation.CustomAttributes.Any<CustomAttribute>(<>f__am$cache6);
        }

        private static IEnumerable<InterfaceImplementationMapping> GetAllImplementedInteropInterfacesInTypeHierarchy(TypeReference type)
        {
            List<InterfaceImplementationMapping> list = new List<InterfaceImplementationMapping>();
            while (type != null)
            {
                TypeDefinition definition = type.Resolve();
                Unity.IL2CPP.ILPreProcessor.TypeResolver resolver = Unity.IL2CPP.ILPreProcessor.TypeResolver.For(type);
                foreach (InterfaceImplementation implementation in definition.Interfaces)
                {
                    if (resolver.Resolve(implementation.InterfaceType).IsComOrWindowsRuntimeInterface())
                    {
                        list.Add(new InterfaceImplementationMapping(type, implementation));
                    }
                }
                type = resolver.Resolve(definition.BaseType);
            }
            return list;
        }

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
            foreach (TypeReference reference2 in this._windowsRuntimeProjectedInterfaces)
            {
                builder.Append(", ");
                builder.Append(CCWWriterBase.Naming.ForComCallableWrapperClass(reference2));
                builder.Append('<');
                builder.Append(this._typeName);
                builder.Append('>');
            }
            return builder.ToString();
        }

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
            <WriteImplementedIReferenceMethodDefinition>c__AnonStorey1 storey = new <WriteImplementedIReferenceMethodDefinition>c__AnonStorey1 {
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
            <WriteImplementedMethodDefinition>c__AnonStorey0 storey = new <WriteImplementedMethodDefinition>c__AnonStorey0 {
                mapping = mapping,
                writer = writer,
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
            this.WriteMethodDefinition(storey.writer, signature, storey.mapping.ManagedMethod, storey.mapping.InterfaceMethod, new Action<CppCodeWriter, IRuntimeMetadataAccess>(storey.<>m__0));
        }

        private void WriteMethodDefinition(CppCodeWriter writer, string signature, MethodReference managedMethod, MethodReference interfaceMethod, Action<CppCodeWriter, IRuntimeMetadataAccess> writeAction)
        {
            <WriteMethodDefinition>c__AnonStorey2 storey = new <WriteMethodDefinition>c__AnonStorey2 {
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
            this._interfacesToImplement.Concat<TypeReference>(this._windowsRuntimeProjectedInterfaces);

        protected override bool ImplementsAnyIInspectableInterfaces =>
            this._implementsAnyIInspectableInterfaces;

        protected override IList<TypeReference> InterfacesToForwardToBaseClass =>
            this._interfacesToForwardToBaseClass;

        [CompilerGenerated]
        private sealed class <WriteImplementedIReferenceMethodDefinition>c__AnonStorey1
        {
            internal CCWWriter $this;
            internal MethodReference method;

            internal void <>m__0(CppCodeWriter bodyWriter, IRuntimeMetadataAccess metadataAccess)
            {
                new IReferenceComCallableWrapperMethodBodyWriter(this.method, this.$this._type).WriteMethodBody(bodyWriter, metadataAccess);
            }
        }

        [CompilerGenerated]
        private sealed class <WriteImplementedMethodDefinition>c__AnonStorey0
        {
            internal CCWWriter $this;
            internal CCWWriter.InterfaceMethodMapping mapping;
            internal MarshalType marshalType;
            internal CppCodeWriter writer;

            internal void <>m__0(CppCodeWriter bodyWriter, IRuntimeMetadataAccess metadataAccess)
            {
                if (this.mapping.InterfaceMethod.IsStripped())
                {
                    bodyWriter.WriteCommentedLine("Managed method has been stripped");
                    bodyWriter.WriteLine("return IL2CPP_E_ILLEGAL_METHOD_CALL;");
                    CCWWriter.Stats.RecordStrippedComCallableWrapperMethod();
                }
                else if (!this.mapping.ManagedMethod.Resolve().DeclaringType.IsComOrWindowsRuntimeType())
                {
                    ComCallableWrapperMethodBodyWriter writer = new ComCallableWrapperMethodBodyWriter(this.mapping.ManagedMethod, this.mapping.InterfaceMethod, this.marshalType);
                    this.writer.AddIncludeForMethodDeclarations(this.mapping.ManagedMethod.DeclaringType);
                    writer.WriteMethodBody(bodyWriter, metadataAccess);
                    CCWWriter.Stats.RecordImplementedComCallableWrapperMethod();
                }
                else
                {
                    string str = CCWWriterBase.Naming.ForVariable(this.$this._type);
                    string str2 = CCWWriterBase.Naming.ForComTypeInterfaceFieldGetter(this.mapping.InterfaceMethod.DeclaringType);
                    string str3 = CCWWriterBase.Naming.ForMethod(this.mapping.InterfaceMethod);
                    string str4 = ComInterfaceWriter.BuildMethodParameterList(this.mapping.ManagedMethod, this.mapping.InterfaceMethod, Unity.IL2CPP.ILPreProcessor.TypeResolver.For(this.$this._type), this.marshalType, false);
                    bodyWriter.WriteLine($"return (({str})GetManagedObjectInline())->{str2}()->{str3}({str4});");
                    CCWWriter.Stats.RecordForwardedToBaseClassComCallableWrapperMethod();
                }
            }
        }

        [CompilerGenerated]
        private sealed class <WriteMethodDefinition>c__AnonStorey2
        {
            internal MethodReference interfaceMethod;
            internal MethodReference managedMethod;
            internal Action<CppCodeWriter, IRuntimeMetadataAccess> writeAction;

            internal void <>m__0(CppCodeWriter bodyWriter, MetadataUsage metadataUsage, MethodUsage methodUsage)
            {
                if (this.managedMethod == null)
                {
                }
                IRuntimeMetadataAccess access = MethodWriter.GetDefaultRuntimeMetadataAccess(this.interfaceMethod, metadataUsage, methodUsage);
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

