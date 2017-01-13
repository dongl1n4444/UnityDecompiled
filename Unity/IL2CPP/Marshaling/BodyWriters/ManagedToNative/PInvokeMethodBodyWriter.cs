namespace Unity.IL2CPP.Marshaling.BodyWriters.ManagedToNative
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using Unity.IL2CPP;
    using Unity.IL2CPP.Marshaling;
    using Unity.IL2CPP.Marshaling.BodyWriters;
    using Unity.IL2CPP.Marshaling.MarshalInfoWriters;

    internal class PInvokeMethodBodyWriter : ManagedToNativeInteropMethodBodyWriter
    {
        private static readonly Dictionary<string, string[]> _internalizedMethods;
        protected readonly MethodDefinition _methodDefinition;
        protected readonly PInvokeInfo _pinvokeInfo;
        [CompilerGenerated]
        private static Func<MarshaledType, string> <>f__am$cache0;

        static PInvokeMethodBodyWriter()
        {
            Dictionary<string, string[]> dictionary = new Dictionary<string, string[]>();
            string[] textArray1 = new string[] { "CreateZStream", "CloseZStream", "Flush", "ReadZStream", "WriteZStream" };
            dictionary.Add("MonoPosixHelper", textArray1);
            _internalizedMethods = dictionary;
        }

        public PInvokeMethodBodyWriter(MethodReference interopMethod) : base(interopMethod, interopMethod, MarshalType.PInvoke, MarshalingUtils.UseUnicodeAsDefaultMarshalingForStringParameters(interopMethod))
        {
            this._methodDefinition = interopMethod.Resolve();
            this._pinvokeInfo = this._methodDefinition.PInvokeInfo;
        }

        private string CalculateParameterSize()
        {
            MarshaledParameter[] parameterArray = base._parameters;
            if (parameterArray.Length == 0)
            {
                return "0";
            }
            StringBuilder builder = new StringBuilder(this.GetParameterSize(parameterArray[0]));
            for (int i = 1; i < parameterArray.Length; i++)
            {
                builder.AppendFormat(" + {0}", this.GetParameterSize(parameterArray[i]));
            }
            return builder.ToString();
        }

        protected string FormatParametersForTypedef()
        {
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = parameterType => parameterType.DecoratedName;
            }
            return base._marshaledParameterTypes.Select<MarshaledType, string>(<>f__am$cache0).AggregateWithComma();
        }

        private string GetCallingConvention()
        {
            if (this._pinvokeInfo.IsCallConvStdCall)
            {
                return "STDCALL";
            }
            if (this._pinvokeInfo.IsCallConvCdecl)
            {
                return "CDECL";
            }
            return "DEFAULT_CALL";
        }

        private string GetIl2CppCallConvention()
        {
            if (this._pinvokeInfo.IsCallConvStdCall)
            {
                return "IL2CPP_CALL_STDCALL";
            }
            if (this._pinvokeInfo.IsCallConvCdecl)
            {
                return "IL2CPP_CALL_C";
            }
            return "IL2CPP_CALL_DEFAULT";
        }

        private string GetMethodCallExpression(string[] localVariableNames)
        {
            string functionCallParametersExpression = base.GetFunctionCallParametersExpression(localVariableNames);
            if (this.ShouldInternalizeMethod())
            {
                return $"reinterpret_cast<{InteropMethodBodyWriter.Naming.ForPInvokeFunctionPointerTypedef()}>({this._pinvokeInfo.EntryPoint})({functionCallParametersExpression})";
            }
            return $"{InteropMethodBodyWriter.Naming.ForPInvokeFunctionPointerVariable()}({functionCallParametersExpression})";
        }

        private string GetParameterSize(MarshaledParameter parameter)
        {
            DefaultMarshalInfoWriter writer = base.MarshalInfoWriterFor(parameter);
            if ((writer.NativeSize == "-1") && (parameter.ParameterType.MetadataType != MetadataType.Array))
            {
                throw new NotSupportedException($"Cannot marshal parameter {parameter.NameInGeneratedCode} of type {parameter.ParameterType.FullName} for P/Invoke.");
            }
            switch (parameter.ParameterType.MetadataType)
            {
                case MetadataType.Class:
                case MetadataType.Array:
                    return "sizeof(void*)";
            }
            int num = 4 - (writer.NativeSizeWithoutPointers % 4);
            if (num != 4)
            {
                return (writer.NativeSize + " + " + num);
            }
            return writer.NativeSize;
        }

        private string GetPInvokeMethodVariable() => 
            $"{base._marshaledReturnType.DecoratedName} ({this.GetCallingConvention()} *{InteropMethodBodyWriter.Naming.ForPInvokeFunctionPointerTypedef()}) ({this.FormatParametersForTypedef()})";

        private bool ShouldInternalizeMethod()
        {
            string[] strArray;
            if (this._pinvokeInfo == null)
            {
                return false;
            }
            string name = this._pinvokeInfo.Module.Name;
            return ((name == "__Internal") || (_internalizedMethods.TryGetValue(name, out strArray) && strArray.Any<string>(a => (a == this._methodDefinition.Name))));
        }

        private bool UseUnicodeCharSetForPInvokeInWindowsCallResolution() => 
            !this._pinvokeInfo.IsCharSetAnsi;

        public void WriteExternMethodeDeclarationForInternalPInvoke(CppCodeWriter writer)
        {
            if (this.ShouldInternalizeMethod())
            {
                writer.WriteInternalPInvokeDeclaration(this._pinvokeInfo.EntryPoint, $"extern "C" {base._marshaledReturnType.DecoratedName} {this.GetCallingConvention()} {this._pinvokeInfo.EntryPoint}({this.FormatParametersForTypedef()});");
            }
        }

        protected override void WriteInteropCallStatement(CppCodeWriter writer, string[] localVariableNames, IRuntimeMetadataAccess metadataAccess)
        {
            if (this.GetMethodReturnType().ReturnType.MetadataType != MetadataType.Void)
            {
                object[] args = new object[] { base._marshaledReturnType.DecoratedName, InteropMethodBodyWriter.Naming.ForInteropReturnValue(), this.GetMethodCallExpression(localVariableNames) };
                writer.WriteLine("{0} {1} = {2};", args);
            }
            else
            {
                writer.WriteStatement(this.GetMethodCallExpression(localVariableNames));
            }
            if ((this._pinvokeInfo != null) && this._pinvokeInfo.SupportsLastError)
            {
                writer.WriteLine("il2cpp_codegen_marshal_store_last_error();");
            }
        }

        protected override void WriteMethodPrologue(CppCodeWriter writer, IRuntimeMetadataAccess metadataAccess)
        {
            object[] args = new object[] { this.GetPInvokeMethodVariable() };
            writer.WriteLine("typedef {0};", args);
            if (this.ShouldInternalizeMethod())
            {
                writer.WriteLine();
            }
            else
            {
                object[] objArray2 = new object[] { InteropMethodBodyWriter.Naming.ForPInvokeFunctionPointerTypedef(), InteropMethodBodyWriter.Naming.ForPInvokeFunctionPointerVariable() };
                writer.WriteLine("static {0} {1};", objArray2);
                object[] objArray3 = new object[] { InteropMethodBodyWriter.Naming.ForPInvokeFunctionPointerVariable() };
                writer.WriteLine("if ({0} == NULL)", objArray3);
                writer.BeginBlock();
                string str = "parameterSize";
                object[] objArray4 = new object[] { str, this.CalculateParameterSize() };
                writer.WriteLine("int {0} = {1};", objArray4);
                string name = this._pinvokeInfo.Module.Name;
                string entryPoint = this._pinvokeInfo.EntryPoint;
                object[] objArray5 = new object[] { InteropMethodBodyWriter.Naming.ForPInvokeFunctionPointerVariable(), InteropMethodBodyWriter.Naming.ForPInvokeFunctionPointerTypedef(), name, entryPoint, this.GetIl2CppCallConvention(), !this.UseUnicodeCharSetForPInvokeInWindowsCallResolution() ? "CHARSET_ANSI" : "CHARSET_UNICODE", str, !this._pinvokeInfo.IsNoMangle ? "false" : "true" };
                writer.WriteLine("{0} = il2cpp_codegen_resolve_pinvoke<{1}>(IL2CPP_NATIVE_STRING(\"{2}\"), \"{3}\", {4}, {5}, {6}, {7});", objArray5);
                writer.WriteLine();
                object[] objArray6 = new object[] { InteropMethodBodyWriter.Naming.ForPInvokeFunctionPointerVariable() };
                writer.WriteLine("if ({0} == NULL)", objArray6);
                writer.BeginBlock();
                writer.WriteStatement(Emit.RaiseManagedException($"il2cpp_codegen_get_not_supported_exception("Unable to find method for p/invoke: '{base.GetMethodName()}'")"));
                writer.EndBlock(false);
                writer.EndBlock(false);
                writer.WriteLine();
            }
        }
    }
}

