namespace Unity.IL2CPP.WindowsRuntime
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Unity.IL2CPP;
    using Unity.IL2CPP.ILPreProcessor;
    using Unity.IL2CPP.Marshaling;
    using Unity.IL2CPP.Marshaling.BodyWriters.NativeToManaged;

    internal class DelegateCCWWriter : CCWWriterBase
    {
        private readonly TypeReference[] _implementedInterfaces;
        private readonly string[] _queryableInterfaces;
        [CompilerGenerated]
        private static Func<MethodDefinition, bool> <>f__am$cache0;

        public DelegateCCWWriter(TypeReference type) : base(type)
        {
            this._implementedInterfaces = new TypeReference[0];
            this._queryableInterfaces = new string[] { CCWWriterBase.Naming.ForWindowsRuntimeDelegateComCallableWrapperInterface(base._type) };
        }

        public void Write(CppCodeWriter writer)
        {
            <Write>c__AnonStorey0 storey = new <Write>c__AnonStorey0();
            Unity.IL2CPP.ILPreProcessor.TypeResolver typeResolver = Unity.IL2CPP.ILPreProcessor.TypeResolver.For(base._type);
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = m => m.Name == "Invoke";
            }
            storey.invokeMethod = typeResolver.Resolve(base._type.Resolve().Methods.Single<MethodDefinition>(<>f__am$cache0));
            string str = MethodSignatureWriter.FormatComMethodParameterList(storey.invokeMethod, storey.invokeMethod, typeResolver, MarshalType.WindowsRuntime, true);
            string str2 = CCWWriterBase.Naming.ForWindowsRuntimeDelegateComCallableWrapperInterface(base._type);
            string str3 = CCWWriterBase.Naming.ForComCallableWrapperClass(base._type);
            writer.AddInclude("vm/CachedCCWBase.h");
            base.AddIncludes(writer);
            writer.WriteLine();
            writer.WriteCommentedLine($"COM Callable Wrapper class definition for {base._type.FullName}");
            writer.WriteLine($"struct {str3} IL2CPP_FINAL : il2cpp::vm::CachedCCWBase<{str3}>, {str2}");
            using (new BlockWriter(writer, true))
            {
                object[] args = new object[] { str3, CCWWriterBase.Naming.ForVariable(CCWWriterBase.TypeProvider.ObjectTypeReference) };
                writer.WriteLine("inline {0}({1} obj) : ", args);
                writer.Indent(1);
                writer.WriteLine($"il2cpp::vm::CachedCCWBase<{str3}>(obj)");
                writer.Dedent(1);
                using (new BlockWriter(writer, false))
                {
                }
                base.WriteCommonInterfaceMethods(writer);
                writer.WriteLine();
                writer.WriteCommentedLine($"COM Callable invoker for {base._type.FullName}");
                string methodSignature = $"virtual il2cpp_hresult_t STDCALL Invoke({str})";
                MethodWriter.WriteMethodWithMetadataInitialization(writer, methodSignature, storey.invokeMethod.FullName, new Action<CppCodeWriter, MetadataUsage, MethodUsage>(storey.<>m__0), CCWWriterBase.Naming.ForMethod(storey.invokeMethod) + "_WindowsRuntimeManagedInvoker");
            }
        }

        protected override IEnumerable<TypeReference> AllImplementedInterfaces =>
            this._implementedInterfaces;

        protected override IEnumerable<string> AllQueryableInterfaceNames =>
            this._queryableInterfaces;

        [CompilerGenerated]
        private sealed class <Write>c__AnonStorey0
        {
            internal MethodReference invokeMethod;

            internal void <>m__0(CppCodeWriter bodyWriter, MetadataUsage metadataUsage, MethodUsage methodUsage)
            {
                IRuntimeMetadataAccess metadataAccess = MethodWriter.GetDefaultRuntimeMetadataAccess(this.invokeMethod, metadataUsage, methodUsage, null);
                new ComCallableWrapperMethodBodyWriter(this.invokeMethod, this.invokeMethod, MarshalType.WindowsRuntime).WriteMethodBody(bodyWriter, metadataAccess);
            }
        }
    }
}

