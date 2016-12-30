namespace Unity.IL2CPP.Marshaling.BodyWriters.ManagedToNative
{
    using Mono.Cecil;
    using System;
    using Unity.IL2CPP;
    using Unity.IL2CPP.Marshaling;
    using Unity.IL2CPP.Marshaling.BodyWriters;

    internal abstract class ComMethodBodyWriter : ManagedToNativeInteropMethodBodyWriter
    {
        protected readonly MethodReference _actualMethod;
        protected readonly TypeReference _interfaceType;
        protected readonly MarshalType _marshalType;

        public ComMethodBodyWriter(MethodReference actualMethod, MethodReference interfaceMethod) : base(interfaceMethod, actualMethod, GetMarshalType(interfaceMethod), true)
        {
            this._actualMethod = actualMethod;
            this._marshalType = GetMarshalType(interfaceMethod);
            this._interfaceType = interfaceMethod.DeclaringType;
        }

        private static MarshalType GetMarshalType(MethodReference interfaceMethod) => 
            (!interfaceMethod.DeclaringType.IsComInterface() ? MarshalType.WindowsRuntime : MarshalType.COM);

        private string GetMethodCallExpression(string[] localVariableNames)
        {
            string str = string.Empty;
            MethodReturnType methodReturnType = base.GetMethodReturnType();
            string functionCallParametersExpression = base.GetFunctionCallParametersExpression(localVariableNames);
            if (methodReturnType.ReturnType.MetadataType != MetadataType.Void)
            {
                if (functionCallParametersExpression.Length > 0)
                {
                    str = ", ";
                }
                str = str + '&' + InteropMethodInfo.Naming.ForInteropReturnValue();
            }
            string str3 = $"{InteropMethodInfo.Naming.ForInteropHResultVariable()} = {InteropMethodInfo.Naming.ForInteropInterfaceVariable(this._interfaceType)}->{this.GetMethodNameInGeneratedCode()}({functionCallParametersExpression}{str})";
            if (!this.UseQueryInterfaceToObtainInterfacePointer)
            {
                str3 = "const il2cpp_hresult_t " + str3;
            }
            return str3;
        }

        protected virtual void OnBeforeHResultCheck(CppCodeWriter writer)
        {
            if (this.UseQueryInterfaceToObtainInterfacePointer)
            {
                writer.WriteStatement(InteropMethodInfo.Naming.ForInteropInterfaceVariable(this._interfaceType) + "->Release()");
            }
        }

        protected override void WriteInteropCallStatement(CppCodeWriter writer, string[] localVariableNames, IRuntimeMetadataAccess metadataAccess)
        {
            MethodReturnType methodReturnType = base.GetMethodReturnType();
            if (methodReturnType.ReturnType.MetadataType != MetadataType.Void)
            {
                base.MarshalInfoWriterFor(methodReturnType).WriteNativeVariableDeclarationOfType(writer, InteropMethodInfo.Naming.ForInteropReturnValue());
            }
            writer.WriteStatement(this.GetMethodCallExpression(localVariableNames));
            this.OnBeforeHResultCheck(writer);
            writer.WriteLine();
            writer.WriteStatement(Emit.Call("il2cpp_codegen_com_raise_exception_if_failed", InteropMethodInfo.Naming.ForInteropHResultVariable(), (this._marshalType != MarshalType.COM) ? "false" : "true"));
        }

        protected override void WriteMethodPrologue(CppCodeWriter writer, IRuntimeMetadataAccess metadataAccess)
        {
            string str = InteropMethodInfo.Naming.ForTypeNameOnly(this._interfaceType);
            string str2 = InteropMethodInfo.Naming.ForInteropInterfaceVariable(this._interfaceType);
            if (this.UseQueryInterfaceToObtainInterfacePointer)
            {
                object[] args = new object[] { str, str2, InteropMethodInfo.Naming.Null };
                writer.WriteLine("{0}* {1} = {2};", args);
                object[] objArray2 = new object[] { InteropMethodInfo.Naming.ForInteropHResultVariable(), InteropMethodInfo.Naming.ForVariable(InteropMethodBodyWriter.TypeProvider.Il2CppComObjectTypeReference), InteropMethodInfo.Naming.ThisParameterName, InteropMethodInfo.Naming.ForIl2CppComObjectIdentityField(), str, str2 };
                writer.WriteLine("il2cpp_hresult_t {0} = static_cast<{1}>({2})->{3}->QueryInterface({4}::IID, reinterpret_cast<void**>(&{5}));", objArray2);
                writer.WriteStatement(Emit.Call("il2cpp_codegen_com_raise_exception_if_failed", InteropMethodInfo.Naming.ForInteropHResultVariable(), (this._marshalType != MarshalType.COM) ? "false" : "true"));
            }
            else
            {
                string str3 = !this._actualMethod.HasThis ? $"(({InteropMethodInfo.Naming.ForStaticFieldsStruct(this._actualMethod.DeclaringType)}*){metadataAccess.TypeInfoFor(this._actualMethod.DeclaringType)}->static_fields)" : $"{InteropMethodInfo.Naming.ThisParameterName}";
                object[] objArray3 = new object[] { InteropMethodInfo.Naming.ForTypeNameOnly(this._interfaceType), InteropMethodInfo.Naming.ForInteropInterfaceVariable(this._interfaceType), str3, InteropMethodInfo.Naming.ForComTypeInterfaceFieldGetter(this._interfaceType) };
                writer.WriteLine("{0}* {1} = {2}->{3}();", objArray3);
            }
            writer.WriteLine();
        }

        protected virtual bool UseQueryInterfaceToObtainInterfacePointer =>
            this._actualMethod.DeclaringType.IsInterface();
    }
}

