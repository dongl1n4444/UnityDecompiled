namespace Unity.IL2CPP.Com
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
    using Unity.IL2CPP.Marshaling;
    using Unity.IL2CPP.Marshaling.BodyWriters.NativeToManaged;
    using Unity.IL2CPP.Metadata;

    public class CCWWriter
    {
        private readonly TypeReference[] _allInteropInterfaces;
        private readonly bool _canForwardMethodsToBaseClass;
        private readonly string _functionDeclaration;
        private readonly List<InterfaceMethodMapping> _interfaceMethodMappings;
        private readonly TypeReference[] _interfacesToForwardToBaseClass;
        private readonly List<TypeReference> _interfacesToImplement;
        private readonly TypeDefinition _type;
        private readonly string _typeName;
        [CompilerGenerated]
        private static Func<TypeDefinition, bool> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<InterfaceImplementationMapping, TypeReference> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<MethodReference, bool> <>f__am$cache2;
        [CompilerGenerated]
        private static Func<InterfaceImplementationMapping, TypeReference> <>f__am$cache3;
        [CompilerGenerated]
        private static Func<TypeReference, string> <>f__am$cache4;
        [CompilerGenerated]
        private static Func<TypeDefinition, IEnumerable<InterfaceImplementationMapping>> <>f__am$cache5;
        [CompilerGenerated]
        private static Func<CustomAttribute, bool> <>f__am$cache6;
        [CompilerGenerated]
        private static Func<InterfaceImplementation, bool> <>f__am$cache7;
        [CompilerGenerated]
        private static Func<InterfaceImplementationMapping, bool> <>f__mg$cache0;
        [Inject]
        public static INamingService Naming;

        public CCWWriter(TypeDefinition type)
        {
            this._type = type;
            this._typeName = Naming.ForTypeNameOnly(type) + "CCW";
            this._functionDeclaration = string.Format("extern \"C\" Il2CppIManagedObjectHolder* {0}(Il2CppObject* obj)", Naming.ForCreateComCallableWrapperFunction(type));
            this._interfaceMethodMappings = new List<InterfaceMethodMapping>();
            this._interfacesToImplement = new List<TypeReference>();
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = new Func<TypeDefinition, bool>(null, (IntPtr) <CCWWriter>m__0);
            }
            this._canForwardMethodsToBaseClass = Enumerable.Any<TypeDefinition>(Extensions.GetTypeHierarchy(type), <>f__am$cache0);
            IEnumerable<InterfaceImplementationMapping> allImplementedInteropInterfacesInTypeHierarchy = GetAllImplementedInteropInterfacesInTypeHierarchy(this._type);
            if (<>f__mg$cache0 == null)
            {
                <>f__mg$cache0 = new Func<InterfaceImplementationMapping, bool>(null, (IntPtr) CanImplementInCCW);
            }
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = new Func<InterfaceImplementationMapping, TypeReference>(null, (IntPtr) <CCWWriter>m__1);
            }
            IEnumerable<TypeReference> enumerable2 = Enumerable.Distinct<TypeReference>(Enumerable.Select<InterfaceImplementationMapping, TypeReference>(Enumerable.Where<InterfaceImplementationMapping>(allImplementedInteropInterfacesInTypeHierarchy, <>f__mg$cache0), <>f__am$cache1), new Unity.IL2CPP.Common.TypeReferenceEqualityComparer());
            VTable table = new VTableBuilder().VTableFor(type, null);
            foreach (TypeReference reference in enumerable2)
            {
                int num = table.InterfaceOffsets[reference];
                if (<>f__am$cache2 == null)
                {
                    <>f__am$cache2 = new Func<MethodReference, bool>(null, (IntPtr) <CCWWriter>m__2);
                }
                IEnumerable<MethodReference> enumerable3 = Enumerable.Where<MethodReference>(Extensions.GetMethods(reference), <>f__am$cache2);
                bool flag = false;
                List<InterfaceMethodMapping> collection = new List<InterfaceMethodMapping>();
                int num2 = 0;
                foreach (MethodReference reference2 in enumerable3)
                {
                    if (!Extensions.IsStripped(reference2))
                    {
                        MethodReference managedMethod = table.Slots[num + num2];
                        collection.Add(new InterfaceMethodMapping(reference2, managedMethod));
                        num2++;
                        if (!Extensions.IsComOrWindowsRuntimeType(managedMethod.DeclaringType.Resolve()))
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
                <>f__am$cache3 = new Func<InterfaceImplementationMapping, TypeReference>(null, (IntPtr) <CCWWriter>m__3);
            }
            this._allInteropInterfaces = Enumerable.ToArray<TypeReference>(Enumerable.Select<InterfaceImplementationMapping, TypeReference>(allImplementedInteropInterfacesInTypeHierarchy, <>f__am$cache3));
            this._interfacesToForwardToBaseClass = Enumerable.ToArray<TypeReference>(Enumerable.Except<TypeReference>(this._allInteropInterfaces, this._interfacesToImplement, new Unity.IL2CPP.Common.TypeReferenceEqualityComparer()));
        }

        [CompilerGenerated]
        private static bool <CCWWriter>m__0(TypeDefinition t)
        {
            return Extensions.IsComOrWindowsRuntimeType(t);
        }

        [CompilerGenerated]
        private static TypeReference <CCWWriter>m__1(InterfaceImplementationMapping i)
        {
            return i.InterfaceImplementation.InterfaceType;
        }

        [CompilerGenerated]
        private static bool <CCWWriter>m__2(MethodReference m)
        {
            return (m.HasThis && m.Resolve().IsVirtual);
        }

        [CompilerGenerated]
        private static TypeReference <CCWWriter>m__3(InterfaceImplementationMapping i)
        {
            return i.InterfaceImplementation.InterfaceType;
        }

        private static bool CanImplementInCCW(InterfaceImplementationMapping mapping)
        {
            if (!Extensions.IsComOrWindowsRuntimeType(mapping.DeclaringType))
            {
                return true;
            }
            if (<>f__am$cache6 == null)
            {
                <>f__am$cache6 = new Func<CustomAttribute, bool>(null, (IntPtr) <CanImplementInCCW>m__6);
            }
            return Enumerable.Any<CustomAttribute>(mapping.InterfaceImplementation.CustomAttributes, <>f__am$cache6);
        }

        private static IEnumerable<InterfaceImplementationMapping> GetAllImplementedInteropInterfacesInTypeHierarchy(TypeDefinition typeDefinition)
        {
            if (<>f__am$cache5 == null)
            {
                <>f__am$cache5 = new Func<TypeDefinition, IEnumerable<InterfaceImplementationMapping>>(null, (IntPtr) <GetAllImplementedInteropInterfacesInTypeHierarchy>m__5);
            }
            return Enumerable.SelectMany<TypeDefinition, InterfaceImplementationMapping>(Extensions.GetTypeHierarchy(typeDefinition), <>f__am$cache5);
        }

        private string GetBaseTypeName()
        {
            string str = "il2cpp::vm::ComObjectBase<" + this._typeName;
            bool flag = false;
            foreach (TypeReference reference in this._interfacesToImplement)
            {
                str = str + ", ";
                str = str + Naming.ForTypeNameOnly(reference);
                if (reference.Resolve().IsWindowsRuntime)
                {
                    flag = true;
                }
            }
            if (!flag)
            {
                str = str + ", Il2CppIInspectable";
            }
            return (str + '>');
        }

        public void WriteCreateCCWDeclaration(CppCodeWriter writer)
        {
            writer.WriteLine(this._functionDeclaration + ';');
        }

        public void WriteCreateCCWDefinition(CppCodeWriter writer)
        {
            writer.WriteLine(this._functionDeclaration);
            using (new BlockWriter(writer, false))
            {
                writer.WriteLine(string.Format("return {0}::__CreateInstance(obj);", this._typeName));
            }
        }

        private void WriteGetIidsDefinition(CppCodeWriter writer)
        {
            writer.WriteLine(string.Format("il2cpp_hresult_t STDCALL {0}::GetIids(uint32_t* iidCount, Il2CppGuid** iids)", this._typeName));
            using (new BlockWriter(writer, false))
            {
                int num = 0;
                foreach (TypeReference reference in this._allInteropInterfaces)
                {
                    if (reference.Resolve().IsWindowsRuntime)
                    {
                        num++;
                    }
                }
                writer.WriteLine(string.Format("Il2CppGuid* interfaceIds = il2cpp_codegen_marshal_allocate_array<Il2CppGuid>({0});", num));
                int num3 = 0;
                foreach (TypeReference reference2 in this._allInteropInterfaces)
                {
                    if (reference2.Resolve().IsWindowsRuntime)
                    {
                        string str = Naming.ForTypeNameOnly(reference2);
                        writer.WriteLine(string.Format("interfaceIds[{0}] = {1}::IID;", num3, str));
                        num3++;
                    }
                }
                writer.WriteLine();
                writer.WriteLine(string.Format("*iidCount = {0};", num));
                writer.WriteLine("*iids = interfaceIds;");
                writer.WriteLine("return IL2CPP_S_OK;");
            }
        }

        public void WriteMethodDefinitions(CppCodeWriter writer)
        {
            if (this._interfacesToForwardToBaseClass.Length > 0)
            {
                this.WriteQueryInterfaceDefinition(writer);
                this.WriteGetIidsDefinition(writer);
            }
            using (List<InterfaceMethodMapping>.Enumerator enumerator = this._interfaceMethodMappings.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    <WriteMethodDefinitions>c__AnonStorey0 storey = new <WriteMethodDefinitions>c__AnonStorey0 {
                        mapping = enumerator.Current,
                        $this = this
                    };
                    <WriteMethodDefinitions>c__AnonStorey1 storey2 = new <WriteMethodDefinitions>c__AnonStorey1 {
                        <>f__ref$0 = storey,
                        marshalType = !storey.mapping.InterfaceMethod.DeclaringType.Resolve().IsWindowsRuntime ? MarshalType.COM : MarshalType.WindowsRuntime
                    };
                    if (storey.mapping.ManagedMethod == null)
                    {
                    }
                    string methodSignature = ComInterfaceWriter.GetSignature(storey.mapping.InterfaceMethod, storey.mapping.InterfaceMethod, Unity.IL2CPP.ILPreProcessor.TypeResolver.For(this._type), this._typeName);
                    if (storey.mapping.ManagedMethod == null)
                    {
                    }
                    MethodWriter.WriteMethodWithMetadataInitialization(writer, methodSignature, storey.mapping.InterfaceMethod.FullName, new Action<CppCodeWriter, MetadataUsage, MethodUsage>(storey2, (IntPtr) this.<>m__0), Naming.ForMethod(storey.mapping.InterfaceMethod) + "_CCW_" + ((storey.mapping.ManagedMethod == null) ? this._typeName : Naming.ForMethod(storey.mapping.ManagedMethod)));
                }
            }
        }

        private void WriteQueryInterfaceDefinition(CppCodeWriter writer)
        {
            writer.WriteLine(string.Format("il2cpp_hresult_t STDCALL {0}::QueryInterface(const Il2CppGuid& iid, void** object)", this._typeName));
            using (new BlockWriter(writer, false))
            {
                if (<>f__am$cache4 == null)
                {
                    <>f__am$cache4 = new Func<TypeReference, string>(null, (IntPtr) <WriteQueryInterfaceDefinition>m__4);
                }
                string str = EnumerableExtensions.AggregateWith(Enumerable.Select<TypeReference, string>(this._interfacesToForwardToBaseClass, <>f__am$cache4), " || ");
                writer.WriteLine(string.Format("if ({0})", str));
                using (new BlockWriter(writer, false))
                {
                    string str2 = Naming.ForVariable(this._type);
                    string str3 = Naming.ForIl2CppComObjectIdentityField();
                    writer.WriteLine(string.Format("return (({0})GetManagedObjectInline())->{1}->QueryInterface(iid, object);", str2, str3));
                }
                writer.WriteLine(string.Format("return {0}::QueryInterface(iid, object);", this.GetBaseTypeName()));
            }
        }

        public void WriteTypeDefinition(CppCodeWriter writer)
        {
            foreach (TypeReference reference in this._interfacesToImplement)
            {
                writer.AddIncludeForTypeDefinition(reference);
            }
            string baseTypeName = this.GetBaseTypeName();
            writer.WriteLine(string.Format("struct {0} : {1}", this._typeName, baseTypeName));
            using (new BlockWriter(writer, true))
            {
                writer.WriteLine(string.Format("inline {0}(Il2CppObject* obj) : {1}(obj) {{}}", this._typeName, baseTypeName));
                if (this._interfacesToForwardToBaseClass.Length > 0)
                {
                    writer.WriteLine("virtual il2cpp_hresult_t STDCALL QueryInterface(const Il2CppGuid& iid, void** object) IL2CPP_OVERRIDE;");
                    writer.WriteLine("virtual il2cpp_hresult_t STDCALL GetIids(uint32_t* iidCount, Il2CppGuid** iids) IL2CPP_OVERRIDE;");
                }
                foreach (InterfaceMethodMapping mapping in this._interfaceMethodMappings)
                {
                    if (mapping.ManagedMethod == null)
                    {
                    }
                    writer.WriteLine(ComInterfaceWriter.GetSignature(mapping.InterfaceMethod, mapping.InterfaceMethod, Unity.IL2CPP.ILPreProcessor.TypeResolver.For(this._type), null) + " IL2CPP_OVERRIDE;");
                }
            }
        }

        [CompilerGenerated]
        private sealed class <GetAllImplementedInteropInterfacesInTypeHierarchy>c__AnonStorey2
        {
            internal TypeDefinition t;

            internal CCWWriter.InterfaceImplementationMapping <>m__0(InterfaceImplementation i)
            {
                return new CCWWriter.InterfaceImplementationMapping(this.t, i);
            }
        }

        [CompilerGenerated]
        private sealed class <WriteMethodDefinitions>c__AnonStorey0
        {
            internal CCWWriter $this;
            internal CCWWriter.InterfaceMethodMapping mapping;
        }

        [CompilerGenerated]
        private sealed class <WriteMethodDefinitions>c__AnonStorey1
        {
            internal CCWWriter.<WriteMethodDefinitions>c__AnonStorey0 <>f__ref$0;
            internal MarshalType marshalType;

            internal void <>m__0(CppCodeWriter bodyWriter, MetadataUsage metadataUsage, MethodUsage methodUsage)
            {
                if (Extensions.IsStripped(this.<>f__ref$0.mapping.InterfaceMethod))
                {
                    bodyWriter.WriteCommentedLine("Managed method has been stripped");
                    bodyWriter.WriteLine("return IL2CPP_E_ILLEGAL_METHOD_CALL;");
                }
                else if (!Extensions.IsComOrWindowsRuntimeType(this.<>f__ref$0.mapping.ManagedMethod.Resolve().DeclaringType))
                {
                    IRuntimeMetadataAccess metadataAccess = MethodWriter.GetDefaultRuntimeMetadataAccess(this.<>f__ref$0.mapping.ManagedMethod, metadataUsage, methodUsage);
                    new ComCallableWrapperMethodBodyWriter(this.<>f__ref$0.mapping.ManagedMethod, this.<>f__ref$0.mapping.InterfaceMethod, this.marshalType).WriteMethodBody(bodyWriter, metadataAccess);
                }
                else
                {
                    string str = CCWWriter.Naming.ForVariable(this.<>f__ref$0.$this._type);
                    string str2 = CCWWriter.Naming.ForComTypeInterfaceFieldGetter(this.<>f__ref$0.mapping.InterfaceMethod.DeclaringType);
                    string str3 = CCWWriter.Naming.ForMethod(this.<>f__ref$0.mapping.InterfaceMethod);
                    string str4 = ComInterfaceWriter.BuildMethodParameterList(this.<>f__ref$0.mapping.ManagedMethod, this.<>f__ref$0.mapping.InterfaceMethod, Unity.IL2CPP.ILPreProcessor.TypeResolver.For(this.<>f__ref$0.$this._type), this.marshalType, false);
                    bodyWriter.WriteLine(string.Format("return (({0})GetManagedObjectInline())->{1}()->{2}({3});", new object[] { str, str2, str3, str4 }));
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct InterfaceImplementationMapping
        {
            public readonly TypeDefinition DeclaringType;
            public readonly Mono.Cecil.InterfaceImplementation InterfaceImplementation;
            public InterfaceImplementationMapping(TypeDefinition declaringType, Mono.Cecil.InterfaceImplementation interfaceImplementation)
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

