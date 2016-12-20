namespace Unity.IL2CPP.Marshaling.BodyWriters.NativeToManaged
{
    using Mono.Cecil;
    using System;
    using Unity.IL2CPP;
    using Unity.IL2CPP.Marshaling;
    using Unity.IL2CPP.Marshaling.BodyWriters;

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
                object[] args = new object[] { InteropMethodBodyWriter.Naming.ForVariable(base._typeResolver.Resolve(methodReturnType.ReturnType)), InteropMethodBodyWriter.Naming.ForInteropReturnValue() };
                writer.WriteLine("{0} {1};", args);
            }
            writer.WriteLine("try");
            using (new BlockWriter(writer, false))
            {
                if (Extensions.IsValueType(base._managedMethod.DeclaringType))
                {
                    object[] objArray2 = new object[] { InteropMethodBodyWriter.Naming.ForTypeNameOnly(base._managedMethod.DeclaringType), InteropMethodBodyWriter.Naming.ThisParameterName, metadataAccess.TypeInfoFor(base._managedMethod.DeclaringType) };
                    writer.WriteLine("{0}* {1} = ({0}*)UnBox(GetManagedObjectInline(), {2});", objArray2);
                }
                else
                {
                    object[] objArray3 = new object[] { InteropMethodBodyWriter.Naming.ForVariable(base._managedMethod.DeclaringType), InteropMethodBodyWriter.Naming.ThisParameterName };
                    writer.WriteLine("{0} {1} = ({0})GetManagedObjectInline();", objArray3);
                }
                string block = base.GetMethodCallExpression(metadataAccess, InteropMethodBodyWriter.Naming.ThisParameterName, localVariableNames);
                if (methodReturnType.ReturnType.MetadataType != MetadataType.Void)
                {
                    object[] objArray4 = new object[] { InteropMethodBodyWriter.Naming.ForInteropReturnValue(), block };
                    writer.WriteLine("{0} = {1};", objArray4);
                }
                else
                {
                    writer.WriteStatement(block);
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
                object[] args = new object[] { InteropMethodBodyWriter.Naming.ForComInterfaceReturnParameterName(), unmarshaledReturnValueVariableName };
                writer.WriteLine("*{0} = {1};", args);
            }
            writer.WriteLine("return IL2CPP_S_OK;");
        }
    }
}

