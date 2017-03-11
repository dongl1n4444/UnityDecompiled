namespace Unity.IL2CPP.Marshaling.BodyWriters.ManagedToNative
{
    using Mono.Cecil;
    using System;
    using Unity.IL2CPP;
    using Unity.IL2CPP.Marshaling.BodyWriters;

    internal class WindowsRuntimeDelegateMethodBodyWriter : ComMethodBodyWriter
    {
        public WindowsRuntimeDelegateMethodBodyWriter(MethodReference invokeMethod) : base(invokeMethod, invokeMethod)
        {
        }

        protected override string GetMethodNameInGeneratedCode() => 
            "Invoke";

        protected override void WriteMethodPrologue(CppCodeWriter writer, IRuntimeMetadataAccess metadataAccess)
        {
            string str = InteropMethodInfo.Naming.ForWindowsRuntimeDelegateComCallableWrapperInterface(base._interfaceType);
            string str2 = InteropMethodInfo.Naming.ForInteropInterfaceVariable(base._interfaceType);
            object[] args = new object[] { str, str2, InteropMethodInfo.Naming.Null };
            writer.WriteLine("{0}* {1} = {2};", args);
            object[] objArray2 = new object[] { InteropMethodInfo.Naming.ForInteropHResultVariable(), InteropMethodInfo.Naming.ThisParameterName, InteropMethodInfo.Naming.ForIl2CppComObjectIdentityField(), str, str2 };
            writer.WriteLine("il2cpp_hresult_t {0} = {1}->{2}->QueryInterface({3}::IID, reinterpret_cast<void**>(&{4}));", objArray2);
            writer.WriteStatement(Emit.Call("il2cpp_codegen_com_raise_exception_if_failed", InteropMethodInfo.Naming.ForInteropHResultVariable(), "false"));
            writer.WriteLine();
        }

        protected override bool UseQueryInterfaceToObtainInterfacePointer =>
            true;
    }
}

