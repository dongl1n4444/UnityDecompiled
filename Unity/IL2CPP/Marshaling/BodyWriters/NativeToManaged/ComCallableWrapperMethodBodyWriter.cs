namespace Unity.IL2CPP.Marshaling.BodyWriters.NativeToManaged
{
    using Mono.Cecil;
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Unity.IL2CPP;
    using Unity.IL2CPP.ILPreProcessor;
    using Unity.IL2CPP.Marshaling;
    using Unity.IL2CPP.Marshaling.BodyWriters;
    using Unity.IL2CPP.Metadata;

    internal class ComCallableWrapperMethodBodyWriter : NativeToManagedInteropMethodBodyWriter
    {
        [CompilerGenerated]
        private static Func<MethodDefinition, bool> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<FieldDefinition, bool> <>f__am$cache1;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string <ManagedObjectExpression>k__BackingField;

        public ComCallableWrapperMethodBodyWriter(MethodReference managedMethod, MethodReference interfaceMethod, MarshalType marshalType) : base(managedMethod, interfaceMethod, marshalType, true)
        {
            this.<ManagedObjectExpression>k__BackingField = Emit.Call("GetManagedObjectInline");
        }

        protected virtual void WriteExceptionReturnStatement(CppCodeWriter writer)
        {
            writer.WriteStatement("return ex.ex->hresult");
        }

        protected sealed override void WriteInteropCallStatement(CppCodeWriter writer, string[] localVariableNames, IRuntimeMetadataAccess metadataAccess)
        {
            MethodReturnType methodReturnType = base.GetMethodReturnType();
            if (methodReturnType.ReturnType.MetadataType != MetadataType.Void)
            {
                object[] args = new object[] { InteropMethodInfo.Naming.ForVariable(base._typeResolver.Resolve(methodReturnType.ReturnType)), InteropMethodInfo.Naming.ForInteropReturnValue() };
                writer.WriteLine("{0} {1};", args);
            }
            writer.WriteLine("try");
            using (new BlockWriter(writer, false))
            {
                this.WriteInteropCallStatementWithinTryBlock(writer, localVariableNames, metadataAccess);
            }
            writer.WriteLine("catch (const Il2CppExceptionWrapper& ex)");
            using (new BlockWriter(writer, false))
            {
                if (methodReturnType.ReturnType.MetadataType != MetadataType.Void)
                {
                    string defaultCppValue = base.MarshalInfoWriterFor(methodReturnType).GetDefaultCppValue(base.MarshaledReturnType);
                    writer.WriteStatement(Emit.Assign('*' + InteropMethodInfo.Naming.ForComInterfaceReturnParameterName(), defaultCppValue));
                }
                Unity.IL2CPP.ILPreProcessor.TypeResolver typeResolverForMethodToCall = new Unity.IL2CPP.ILPreProcessor.TypeResolver();
                writer.AddIncludeForTypeDefinition(InteropMethodBodyWriter.TypeProvider.SystemString);
                writer.WriteLine($"{InteropMethodInfo.Naming.ForVariable(InteropMethodBodyWriter.TypeProvider.SystemString)} exceptionStr = {InteropMethodInfo.Naming.Null};");
                writer.WriteLine("try");
                using (new BlockWriter(writer, false))
                {
                    string[] argumentArray = new string[] { "ex.ex" };
                    if (<>f__am$cache0 == null)
                    {
                        <>f__am$cache0 = m => m.Name == "ToString";
                    }
                    MethodDefinition methodToCall = InteropMethodBodyWriter.TypeProvider.SystemObject.Methods.Single<MethodDefinition>(<>f__am$cache0);
                    string str2 = MethodBodyWriter.GetMethodCallExpression(base._managedMethod, methodToCall, methodToCall, typeResolverForMethodToCall, MethodCallType.Virtual, metadataAccess, new VTableBuilder(), argumentArray, false, null);
                    writer.WriteLine($"exceptionStr = {str2};");
                }
                writer.WriteLine("catch (const Il2CppExceptionWrapper&)");
                using (new BlockWriter(writer, false))
                {
                    if (<>f__am$cache1 == null)
                    {
                        <>f__am$cache1 = f => f.Name == "Empty";
                    }
                    FieldDefinition fieldReference = InteropMethodBodyWriter.TypeProvider.SystemString.Fields.Single<FieldDefinition>(<>f__am$cache1);
                    string str3 = MethodBodyWriter.TypeStaticsExpressionFor(fieldReference, typeResolverForMethodToCall, metadataAccess);
                    writer.WriteLine($"exceptionStr = {str3}{InteropMethodInfo.Naming.ForFieldGetter(fieldReference)}();");
                }
                writer.WriteLine("il2cpp_codegen_store_exception_info(ex.ex, exceptionStr);");
                this.WriteExceptionReturnStatement(writer);
            }
        }

        protected virtual void WriteInteropCallStatementWithinTryBlock(CppCodeWriter writer, string[] localVariableNames, IRuntimeMetadataAccess metadataAccess)
        {
            if (base._managedMethod.DeclaringType.IsValueType())
            {
                object[] args = new object[] { InteropMethodInfo.Naming.ForTypeNameOnly(base._managedMethod.DeclaringType), InteropMethodInfo.Naming.ThisParameterName, metadataAccess.TypeInfoFor(base._managedMethod.DeclaringType), this.ManagedObjectExpression };
                writer.WriteLine("{0}* {1} = ({0}*)UnBox({3}, {2});", args);
            }
            else
            {
                object[] objArray2 = new object[] { InteropMethodInfo.Naming.ForVariable(base._managedMethod.DeclaringType), InteropMethodInfo.Naming.ThisParameterName, this.ManagedObjectExpression };
                writer.WriteLine("{0} {1} = ({0}){2};", objArray2);
            }
            string block = base.GetMethodCallExpression(metadataAccess, InteropMethodInfo.Naming.ThisParameterName, localVariableNames);
            if (base.GetMethodReturnType().ReturnType.MetadataType != MetadataType.Void)
            {
                object[] objArray3 = new object[] { InteropMethodInfo.Naming.ForInteropReturnValue(), block };
                writer.WriteLine("{0} = {1};", objArray3);
            }
            else
            {
                writer.WriteStatement(block);
            }
        }

        protected override void WriteReturnStatementEpilogue(CppCodeWriter writer, string unmarshaledReturnValueVariableName)
        {
            if (base.GetMethodReturnType().ReturnType.MetadataType != MetadataType.Void)
            {
                object[] args = new object[] { InteropMethodInfo.Naming.ForComInterfaceReturnParameterName(), unmarshaledReturnValueVariableName };
                writer.WriteLine("*{0} = {1};", args);
            }
            writer.WriteLine("return IL2CPP_S_OK;");
        }

        protected virtual string ManagedObjectExpression =>
            this.<ManagedObjectExpression>k__BackingField;
    }
}

