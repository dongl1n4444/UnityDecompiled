using Mono.Cecil;
using System;

namespace Unity.IL2CPP.Marshaling.BodyWriters.ManagedToNative
{
	internal class DelegatePInvokeMethodBodyWriter : PInvokeMethodBodyWriter
	{
		public DelegatePInvokeMethodBodyWriter(MethodReference interopMethod) : base(interopMethod)
		{
		}

		protected override void WriteMethodPrologue(CppCodeWriter writer, IRuntimeMetadataAccess metadataAccess)
		{
			string decoratedName = this._marshaledReturnType.DecoratedName;
			writer.WriteLine("typedef {0} (STDCALL *{1})({2});", new object[]
			{
				decoratedName,
				InteropMethodBodyWriter.Naming.ForPInvokeFunctionPointerTypedef(),
				base.FormatParametersForTypedef()
			});
			writer.WriteLine("{0} {1} = reinterpret_cast<{0}>(((Il2CppDelegate*){2})->method->methodPointer);", new object[]
			{
				InteropMethodBodyWriter.Naming.ForPInvokeFunctionPointerTypedef(),
				InteropMethodBodyWriter.Naming.ForPInvokeFunctionPointerVariable(),
				InteropMethodBodyWriter.Naming.ThisParameterName
			});
			writer.WriteLine();
		}

		public bool IsDelegatePInvokeWrapperNecessary()
		{
			bool result;
			if (this._methodDefinition.Name != "Invoke")
			{
				result = false;
			}
			else
			{
				TypeDefinition declaringType = this._methodDefinition.DeclaringType;
				result = (declaringType.IsDelegate() && !declaringType.HasGenericParameters && !this._methodDefinition.HasGenericParameters && !this._methodDefinition.ReturnType.IsGenericParameter && this._methodDefinition.IsRuntime && base.FirstOrDefaultUnmarshalableMarshalInfoWriter() == null);
			}
			return result;
		}
	}
}
