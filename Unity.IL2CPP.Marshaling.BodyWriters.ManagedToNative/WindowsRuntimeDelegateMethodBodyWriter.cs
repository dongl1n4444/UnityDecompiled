using Mono.Cecil;
using System;

namespace Unity.IL2CPP.Marshaling.BodyWriters.ManagedToNative
{
	internal class WindowsRuntimeDelegateMethodBodyWriter : ComMethodBodyWriter
	{
		protected override bool UseQueryInterfaceToObtainInterfacePointer
		{
			get
			{
				return true;
			}
		}

		public WindowsRuntimeDelegateMethodBodyWriter(MethodReference invokeMethod) : base(invokeMethod, invokeMethod)
		{
		}

		protected override void WriteMethodPrologue(CppCodeWriter writer, IRuntimeMetadataAccess metadataAccess)
		{
			string text = InteropMethodBodyWriter.Naming.ForWindowsRuntimeDelegateComCallableWrapperInterface(this._interfaceType);
			string text2 = InteropMethodBodyWriter.Naming.ForInteropInterfaceVariable(this._interfaceType);
			writer.WriteLine("{0}* {1} = {2};", new object[]
			{
				text,
				text2,
				InteropMethodBodyWriter.Naming.Null
			});
			writer.WriteLine("il2cpp_hresult_t {0} = {1}->{2}->QueryInterface({3}::IID, reinterpret_cast<void**>(&{4}));", new object[]
			{
				InteropMethodBodyWriter.Naming.ForInteropHResultVariable(),
				InteropMethodBodyWriter.Naming.ThisParameterName,
				InteropMethodBodyWriter.Naming.ForIl2CppComObjectIdentityField(),
				text,
				text2
			});
			writer.WriteStatement(Emit.Call("il2cpp_codegen_com_raise_exception_if_failed", InteropMethodBodyWriter.Naming.ForInteropHResultVariable()));
			writer.WriteLine();
		}

		protected override string GetMethodNameInGeneratedCode()
		{
			return "Invoke";
		}
	}
}
