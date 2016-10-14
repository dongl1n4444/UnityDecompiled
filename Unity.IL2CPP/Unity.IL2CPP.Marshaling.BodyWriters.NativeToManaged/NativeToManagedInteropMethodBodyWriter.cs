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

		public NativeToManagedInteropMethodBodyWriter(MethodReference managedMethod, MethodReference interopMethod, MarshalType marshalType, bool useUnicodeCharset) : base(interopMethod, new NativeToManagedMarshaler(TypeResolver.For(interopMethod.DeclaringType, interopMethod), marshalType, useUnicodeCharset))
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
			MethodCallType callType = (this._managedMethod.HasThis && !MethodSignatureWriter.CanDevirtualizeMethodCall(this._managedMethod.Resolve())) ? MethodCallType.Virtual : MethodCallType.Normal;
			List<string> list = new List<string>();
			list.Add(thisArgument);
			list.AddRange(localVariableNames);
			if (MethodSignatureWriter.NeedsHiddenMethodInfo(this._managedMethod, callType, false))
			{
				list.Add(metadataAccess.HiddenMethodInfo(this._managedMethod));
			}
			return MethodBodyWriter.GetMethodCallExpression(this._managedMethod, this._managedMethod, this._managedMethod, this._typeResolver, callType, metadataAccess, new VTableBuilder(), list, null);
		}
	}
}
