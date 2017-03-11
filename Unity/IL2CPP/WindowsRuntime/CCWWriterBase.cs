namespace Unity.IL2CPP.WindowsRuntime
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Unity.IL2CPP;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;

    public abstract class CCWWriterBase
    {
        protected readonly TypeReference _type;
        [CompilerGenerated]
        private static Func<TypeReference, string> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<TypeReference, bool> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<TypeReference, string> <>f__am$cache2;
        private static readonly Guid IID_IMarshal = new Guid(3, 0, 0, 0xc0, 0, 0, 0, 0, 0, 0, 70);
        [Inject]
        public static INamingService Naming;
        [Inject]
        public static ITypeProviderService TypeProvider;

        protected CCWWriterBase(TypeReference type)
        {
            this._type = type;
        }

        protected void AddIncludes(CppCodeWriter writer)
        {
            writer.AddIncludeForTypeDefinition(this._type);
            foreach (TypeReference reference in this.AllImplementedInterfaces)
            {
                writer.AddIncludeForTypeDefinition(reference);
            }
        }

        private void WriteAddRefDefinition(CppCodeWriter writer)
        {
            writer.WriteLine();
            writer.WriteLine("virtual uint32_t STDCALL AddRef() IL2CPP_OVERRIDE");
            using (new BlockWriter(writer, false))
            {
                writer.WriteLine("return AddRefImpl();");
            }
        }

        protected void WriteCommonInterfaceMethods(CppCodeWriter writer)
        {
            this.WriteQueryInterfaceDefinition(writer);
            this.WriteAddRefDefinition(writer);
            this.WriteReleaseDefinition(writer);
            this.WriteGetIidsDefinition(writer);
            if (this.ImplementsAnyIInspectableInterfaces)
            {
                this.WriteGetRuntimeClassNameDefinition(writer);
                this.WriteGetTrustLevelDefinition(writer);
            }
        }

        private void WriteGetIidsDefinition(CppCodeWriter writer)
        {
            writer.WriteLine();
            writer.WriteLine("virtual il2cpp_hresult_t STDCALL GetIids(uint32_t* iidCount, Il2CppGuid** iids) IL2CPP_OVERRIDE");
            using (new BlockWriter(writer, false))
            {
                int num = 0;
                foreach (TypeReference reference in this.AllInteropInterfaces)
                {
                    if (reference.Resolve().IsWindowsRuntime)
                    {
                        num++;
                    }
                }
                if (num > 0)
                {
                    writer.WriteLine($"Il2CppGuid* interfaceIds = il2cpp_codegen_marshal_allocate_array<Il2CppGuid>({num});");
                    int num2 = 0;
                    foreach (TypeReference reference2 in this.AllInteropInterfaces)
                    {
                        if (reference2.Resolve().IsWindowsRuntime)
                        {
                            string str = Naming.ForTypeNameOnly(reference2);
                            writer.WriteLine($"interfaceIds[{num2}] = {str}::IID;");
                            num2++;
                        }
                    }
                    writer.WriteLine();
                    writer.WriteLine($"*iidCount = {num};");
                    writer.WriteLine("*iids = interfaceIds;");
                }
                else
                {
                    writer.WriteLine("*iidCount = 0;");
                    writer.WriteLine($"*iids = {Naming.Null};");
                }
                writer.WriteLine("return IL2CPP_S_OK;");
            }
        }

        private void WriteGetRuntimeClassNameDefinition(CppCodeWriter writer)
        {
            writer.WriteLine();
            writer.WriteLine("virtual il2cpp_hresult_t STDCALL GetRuntimeClassName(Il2CppHString* className) IL2CPP_OVERRIDE");
            using (new BlockWriter(writer, false))
            {
                writer.WriteLine("return ComObjectBase::GetRuntimeClassName(className);");
            }
        }

        private void WriteGetTrustLevelDefinition(CppCodeWriter writer)
        {
            writer.WriteLine();
            writer.WriteLine("virtual il2cpp_hresult_t STDCALL GetTrustLevel(int32_t* trustLevel) IL2CPP_OVERRIDE");
            using (new BlockWriter(writer, false))
            {
                writer.WriteLine("return ComObjectBase::GetTrustLevel(trustLevel);");
            }
        }

        private void WriteQueryInterfaceDefinition(CppCodeWriter writer)
        {
            writer.WriteLine();
            writer.WriteLine("virtual il2cpp_hresult_t STDCALL QueryInterface(const Il2CppGuid& iid, void** object) IL2CPP_OVERRIDE");
            using (new BlockWriter(writer, false))
            {
                if (<>f__am$cache1 == null)
                {
                    <>f__am$cache1 = i => i.GetGuid() == IID_IMarshal;
                }
                bool flag = this.AllImplementedInterfaces.Any<TypeReference>(<>f__am$cache1);
                writer.WriteLine("if (::memcmp(&iid, &Il2CppIUnknown::IID, sizeof(Il2CppGuid)) == 0");
                writer.Write(" || ::memcmp(&iid, &Il2CppIInspectable::IID, sizeof(Il2CppGuid)) == 0");
                if (!flag)
                {
                    writer.WriteLine();
                    writer.WriteLine(" || ::memcmp(&iid, &Il2CppIAgileObject::IID, sizeof(Il2CppGuid)) == 0)");
                }
                else
                {
                    writer.WriteLine(")");
                }
                using (new BlockWriter(writer, false))
                {
                    writer.WriteLine("*object = GetIdentity();");
                    writer.WriteLine("AddRefImpl();");
                    writer.WriteLine("return IL2CPP_S_OK;");
                }
                writer.WriteLine();
                WriteQueryInterfaceForInterface(writer, "Il2CppIManagedObjectHolder");
                if (this.InterfacesToForwardToBaseClass.Count > 0)
                {
                    if (<>f__am$cache2 == null)
                    {
                        <>f__am$cache2 = i => $"::memcmp(&iid, &{Naming.ForTypeNameOnly(i)}::IID, sizeof(Il2CppGuid)) == 0";
                    }
                    string str = this.InterfacesToForwardToBaseClass.Select<TypeReference, string>(<>f__am$cache2).AggregateWith(" || ");
                    writer.WriteLine($"if ({str})");
                    using (new BlockWriter(writer, false))
                    {
                        string str2 = Naming.ForVariable(this._type);
                        string str3 = Naming.ForIl2CppComObjectIdentityField();
                        writer.WriteLine($"return (({str2})GetManagedObjectInline())->{str3}->QueryInterface(iid, object);");
                    }
                    writer.WriteLine();
                }
                foreach (string str4 in this.AllQueryableInterfaceNames)
                {
                    WriteQueryInterfaceForInterface(writer, str4);
                }
                if (!flag)
                {
                    WriteQueryInterfaceForInterface(writer, "Il2CppIMarshal");
                }
                writer.WriteLine("*object = NULL;");
                writer.WriteLine("return IL2CPP_E_NOINTERFACE;");
            }
        }

        private static void WriteQueryInterfaceForInterface(CppCodeWriter writer, string interfaceName)
        {
            writer.WriteLine($"if (::memcmp(&iid, &{interfaceName}::IID, sizeof(Il2CppGuid)) == 0)");
            using (new BlockWriter(writer, false))
            {
                writer.WriteLine($"*object = static_cast<{interfaceName}*>(this);");
                writer.WriteLine("AddRefImpl();");
                writer.WriteLine("return IL2CPP_S_OK;");
            }
            writer.WriteLine();
        }

        private void WriteReleaseDefinition(CppCodeWriter writer)
        {
            writer.WriteLine();
            writer.WriteLine("virtual uint32_t STDCALL Release() IL2CPP_OVERRIDE");
            using (new BlockWriter(writer, false))
            {
                writer.WriteLine("return ReleaseImpl();");
            }
        }

        protected abstract IEnumerable<TypeReference> AllImplementedInterfaces { get; }

        private IEnumerable<TypeReference> AllInteropInterfaces =>
            this.AllImplementedInterfaces.Concat<TypeReference>(this.InterfacesToForwardToBaseClass);

        protected virtual IEnumerable<string> AllQueryableInterfaceNames
        {
            get
            {
                if (<>f__am$cache0 == null)
                {
                    <>f__am$cache0 = t => Naming.ForTypeNameOnly(t);
                }
                return this.AllImplementedInterfaces.Select<TypeReference, string>(<>f__am$cache0);
            }
        }

        protected virtual bool ImplementsAnyIInspectableInterfaces =>
            false;

        protected virtual IList<TypeReference> InterfacesToForwardToBaseClass =>
            new TypeReference[0];
    }
}

