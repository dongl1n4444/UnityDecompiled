using Mono.Cecil;
using System;
using System.Collections.Generic;
using Unity.IL2CPP.ILPreProcessor;
using Unity.IL2CPP.Metadata;

namespace Unity.IL2CPP.Marshaling.BodyWriters.NativeToManaged
{
	internal abstract class NativeToManagedInteropMethodBodyWriter : InteropMethodBodyWriter
	{
		protected readonly MethodReference _managedMethod;

		public NativeToManagedInteropMethodBodyWriter(MethodReference managedMethod, MethodReference interopMethod, MarshalType marshalType, bool useUnicodeCharset) : base(interopMethod, managedMethod, new NativeToManagedMarshaler(TypeResolver.For(interopMethod.DeclaringType, interopMethod), marshalType, useUnicodeCharset))
		{
			this._managedMethod = managedMethod;
		}

		protected override void WriteMethodPrologue(CppCodeWriter writer, IRuntimeMetadataAccess metadataAccess)
		{
			writer.WriteLine("il2cpp_native_wrapper_vm_thread_attacher _vmThreadHelper;");
			writer.WriteLine();
		}

		protected string GetMethodCallExpression(IRuntimeMetadataAccess metadataAccess, string thisArgument, IEnumerable<string> localVariableNames)
		{
			List<string> list = new List<string>();
			list.Add(thisArgument);
			list.AddRange(localVariableNames);
			if (MethodSignatureWriter.NeedsHiddenMethodInfo(this._managedMethod, MethodCallType.Normal, false))
			{
				list.Add(metadataAccess.HiddenMethodInfo(this._managedMethod));
			}
			return "::" + MethodBodyWriter.GetMethodCallExpression(this._managedMethod, this._managedMethod, this._managedMethod, this._typeResolver, MethodCallType.Normal, metadataAccess, new VTableBuilder(), list, false, null);
		}

		protected sealed override void WriteReturnStatement(CppCodeWriter writer, string unmarshaledReturnValueVariableName, IRuntimeMetadataAccess metadataAccess)
		{
			MarshaledType[] marshaledTypes = base.MarshalInfoWriterFor(this.GetMethodReturnType()).MarshaledTypes;
			for (int i = 0; i < marshaledTypes.Length - 1; i++)
			{
				writer.WriteLine("*{0}{2} = {1}{2};", new object[]
				{
					InteropMethodBodyWriter.Naming.ForComInterfaceReturnParameterName(),
					unmarshaledReturnValueVariableName,
					marshaledTypes[i].VariableName
				});
			}
			this.WriteReturnStatementEpilogue(writer, unmarshaledReturnValueVariableName);
		}

		protected abstract void WriteReturnStatementEpilogue(CppCodeWriter writer, string unmarshaleedReturnValueVariableName);
	}
}
