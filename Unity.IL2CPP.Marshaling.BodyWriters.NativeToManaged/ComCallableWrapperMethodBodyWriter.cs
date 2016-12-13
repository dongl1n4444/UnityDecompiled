using Mono.Cecil;
using System;

namespace Unity.IL2CPP.Marshaling.BodyWriters.NativeToManaged
{
	internal class ComCallableWrapperMethodBodyWriter : NativeToManagedInteropMethodBodyWriter
	{
		public ComCallableWrapperMethodBodyWriter(MethodReference managedMethod, MethodReference interfaceMethod, MarshalType marshalType) : base(managedMethod, interfaceMethod, marshalType, true)
		{
		}

		protected override void WriteInteropCallStatement(CppCodeWriter writer, string[] localVariableNames, IRuntimeMetadataAccess metadataAccess)
		{
			MethodReturnType methodReturnType = this.GetMethodReturnType();
			if (methodReturnType.ReturnType.MetadataType != MetadataType.Void)
			{
				writer.WriteLine("{0} {1};", new object[]
				{
					InteropMethodBodyWriter.Naming.ForVariable(this._typeResolver.Resolve(methodReturnType.ReturnType)),
					InteropMethodBodyWriter.Naming.ForInteropReturnValue()
				});
			}
			writer.WriteLine("try");
			using (new BlockWriter(writer, false))
			{
				if (this._managedMethod.DeclaringType.IsValueType())
				{
					writer.WriteLine("{0}* {1} = ({0}*)UnBox(GetManagedObjectInline(), {2});", new object[]
					{
						InteropMethodBodyWriter.Naming.ForTypeNameOnly(this._managedMethod.DeclaringType),
						InteropMethodBodyWriter.Naming.ThisParameterName,
						metadataAccess.TypeInfoFor(this._managedMethod.DeclaringType)
					});
				}
				else
				{
					writer.WriteLine("{0} {1} = ({0})GetManagedObjectInline();", new object[]
					{
						InteropMethodBodyWriter.Naming.ForVariable(this._managedMethod.DeclaringType),
						InteropMethodBodyWriter.Naming.ThisParameterName
					});
				}
				string methodCallExpression = base.GetMethodCallExpression(metadataAccess, InteropMethodBodyWriter.Naming.ThisParameterName, localVariableNames);
				if (methodReturnType.ReturnType.MetadataType != MetadataType.Void)
				{
					writer.WriteLine("{0} = {1};", new object[]
					{
						InteropMethodBodyWriter.Naming.ForInteropReturnValue(),
						methodCallExpression
					});
				}
				else
				{
					writer.WriteStatement(methodCallExpression);
				}
			}
			writer.WriteLine("catch (const Il2CppExceptionWrapper& ex)");
			using (new BlockWriter(writer, false))
			{
				writer.WriteLine("return ex.ex->hresult;");
			}
		}

		protected override void WriteReturnStatementEpilogue(CppCodeWriter writer, string unmarshaledReturnValueVariableName)
		{
			if (this.GetMethodReturnType().ReturnType.MetadataType != MetadataType.Void)
			{
				writer.WriteLine("*{0} = {1};", new object[]
				{
					InteropMethodBodyWriter.Naming.ForComInterfaceReturnParameterName(),
					unmarshaledReturnValueVariableName
				});
			}
			writer.WriteLine("return IL2CPP_S_OK;");
		}
	}
}
