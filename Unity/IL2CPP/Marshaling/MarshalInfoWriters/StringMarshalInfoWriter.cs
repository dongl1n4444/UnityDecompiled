namespace Unity.IL2CPP.Marshaling.MarshalInfoWriters
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using Unity.IL2CPP;
    using Unity.IL2CPP.Marshaling;

    internal sealed class StringMarshalInfoWriter : MarshalableMarshalInfoWriter
    {
        private readonly bool _canReferenceOriginalManagedString;
        private readonly bool _isStringBuilder;
        private readonly string _marshaledTypeName;
        private readonly MarshaledType[] _marshaledTypes;
        private readonly MarshalInfo _marshalInfo;
        private readonly Mono.Cecil.NativeType _nativeType;
        private readonly bool _useUnicodeCharSet;
        [CompilerGenerated]
        private static Func<FieldDefinition, bool> <>f__am$cache0;
        private const Mono.Cecil.NativeType kNativeTypeHString = (Mono.Cecil.NativeType.Error | Mono.Cecil.NativeType.Boolean);

        public StringMarshalInfoWriter(TypeReference type, MarshalType marshalType, MarshalInfo marshalInfo, bool useUnicodeCharSet, bool forByReferenceType, bool forFieldMarshaling) : base(type)
        {
            this._isStringBuilder = MarshalingUtils.IsStringBuilder(type);
            this._useUnicodeCharSet = useUnicodeCharSet;
            this._nativeType = DetermineNativeTypeFor(marshalType, marshalInfo, this._useUnicodeCharSet, this._isStringBuilder);
            if (this._nativeType == (Mono.Cecil.NativeType.Error | Mono.Cecil.NativeType.Boolean))
            {
                this._marshaledTypeName = "Il2CppHString";
            }
            else if (this.IsWideString)
            {
                this._marshaledTypeName = "Il2CppChar*";
            }
            else
            {
                this._marshaledTypeName = "char*";
            }
            this._marshaledTypes = new MarshaledType[] { new MarshaledType(this._marshaledTypeName, this._marshaledTypeName) };
            this._marshalInfo = marshalInfo;
            this._canReferenceOriginalManagedString = ((!this._isStringBuilder && !forByReferenceType) && !forFieldMarshaling) && ((this._nativeType == Mono.Cecil.NativeType.LPWStr) || (this._nativeType == (Mono.Cecil.NativeType.Error | Mono.Cecil.NativeType.Boolean)));
        }

        private static Mono.Cecil.NativeType DetermineNativeTypeFor(MarshalType marshalType, MarshalInfo marshalInfo, bool useUnicodeCharset, bool isStringBuilder)
        {
            Mono.Cecil.NativeType nativeType;
            if (marshalInfo != null)
            {
                nativeType = marshalInfo.NativeType;
            }
            else if (marshalType == MarshalType.PInvoke)
            {
                nativeType = !useUnicodeCharset ? Mono.Cecil.NativeType.LPStr : Mono.Cecil.NativeType.LPWStr;
            }
            else
            {
                nativeType = Mono.Cecil.NativeType.None;
            }
            bool flag = false;
            switch (nativeType)
            {
                case Mono.Cecil.NativeType.BStr:
                case Mono.Cecil.NativeType.LPStr:
                case Mono.Cecil.NativeType.LPWStr:
                case Mono.Cecil.NativeType.FixedSysString:
                case (Mono.Cecil.NativeType.Error | Mono.Cecil.NativeType.Boolean):
                    flag = true;
                    break;
            }
            if (flag && ((!isStringBuilder || (nativeType == Mono.Cecil.NativeType.LPStr)) || (nativeType == Mono.Cecil.NativeType.LPWStr)))
            {
                return nativeType;
            }
            if (marshalType != MarshalType.PInvoke)
            {
                if (marshalType != MarshalType.COM)
                {
                    if (marshalType != MarshalType.WindowsRuntime)
                    {
                        return nativeType;
                    }
                    return (Mono.Cecil.NativeType.Error | Mono.Cecil.NativeType.Boolean);
                }
            }
            else
            {
                return Mono.Cecil.NativeType.LPStr;
            }
            return Mono.Cecil.NativeType.BStr;
        }

        private void FreeMarshaledString(CppCodeWriter writer, string variableName)
        {
            if (!this.IsFixedSizeString)
            {
                Mono.Cecil.NativeType type = this._nativeType;
                if (type == Mono.Cecil.NativeType.BStr)
                {
                    object[] objArray1 = new object[] { variableName };
                    writer.WriteLine("il2cpp_codegen_marshal_free_bstring({0});", objArray1);
                }
                else if (type == (Mono.Cecil.NativeType.Error | Mono.Cecil.NativeType.Boolean))
                {
                    object[] objArray2 = new object[] { variableName };
                    writer.WriteLine("il2cpp_codegen_marshal_free_hstring({0});", objArray2);
                }
                else
                {
                    object[] objArray3 = new object[] { variableName };
                    writer.WriteLine("il2cpp_codegen_marshal_free({0});", objArray3);
                }
                object[] args = new object[] { variableName, DefaultMarshalInfoWriter.Naming.Null };
                writer.WriteLine("{0} = {1};", args);
            }
        }

        public override void WriteFieldDeclaration(CppCodeWriter writer, FieldReference field, string fieldNameSuffix = null)
        {
            if (this.IsFixedSizeString)
            {
                string str = DefaultMarshalInfoWriter.Naming.ForField(field) + fieldNameSuffix;
                object[] args = new object[] { this._marshaledTypeName.Replace("*", ""), str, ((FixedSysStringMarshalInfo) this._marshalInfo).Size };
                writer.WriteLine("{0} {1}[{2}];", args);
            }
            else
            {
                base.WriteFieldDeclaration(writer, field, fieldNameSuffix);
            }
        }

        public override void WriteMarshalCleanupReturnValue(CppCodeWriter writer, string variableName, IRuntimeMetadataAccess metadataAccess)
        {
            this.FreeMarshaledString(writer, variableName);
        }

        public override void WriteMarshalCleanupVariable(CppCodeWriter writer, string variableName, IRuntimeMetadataAccess metadataAccess, string managedVariableName = null)
        {
            if (!this._canReferenceOriginalManagedString)
            {
                this.FreeMarshaledString(writer, variableName);
            }
        }

        public override void WriteMarshaledTypeForwardDeclaration(CppCodeWriter writer)
        {
        }

        public override string WriteMarshalEmptyVariableFromNative(CppCodeWriter writer, string variableName, IList<MarshaledParameter> methodParameters, IRuntimeMetadataAccess metadataAccess) => 
            DefaultMarshalInfoWriter.Naming.Null;

        public override string WriteMarshalReturnValueToNative(CppCodeWriter writer, ManagedMarshalValue sourceVariable, IRuntimeMetadataAccess metadataAccess)
        {
            string variableName = $"_{sourceVariable.GetNiceName()}_marshaled";
            this.WriteNativeVariableDeclarationOfType(writer, variableName);
            this.WriteMarshalVariableToNative(writer, sourceVariable, variableName, null, metadataAccess, true);
            return variableName;
        }

        public override void WriteMarshalVariableFromNative(CppCodeWriter writer, string variableName, ManagedMarshalValue destinationVariable, IList<MarshaledParameter> methodParameters, bool returnValue, bool forNativeWrapperOfManagedMethod, IRuntimeMetadataAccess metadataAccess)
        {
            string str;
            if (this._isStringBuilder)
            {
                str = !this.IsWideString ? "il2cpp_codegen_marshal_string_builder_result" : "il2cpp_codegen_marshal_wstring_builder_result";
                object[] args = new object[] { str, destinationVariable.Load(), variableName };
                writer.WriteLine("{0}({1}, {2});", args);
            }
            else
            {
                Mono.Cecil.NativeType type = this._nativeType;
                if (type == Mono.Cecil.NativeType.BStr)
                {
                    str = "il2cpp_codegen_marshal_bstring_result";
                }
                else if (type == (Mono.Cecil.NativeType.Error | Mono.Cecil.NativeType.Boolean))
                {
                    str = "il2cpp_codegen_marshal_hstring_result";
                }
                else
                {
                    str = !this.IsWideString ? "il2cpp_codegen_marshal_string_result" : "il2cpp_codegen_marshal_wstring_result";
                }
                object[] objArray2 = new object[] { str, variableName };
                writer.WriteLine(destinationVariable.Store("{0}({1})", objArray2));
            }
        }

        public override void WriteMarshalVariableToNative(CppCodeWriter writer, ManagedMarshalValue sourceVariable, string destinationVariable, string managedVariableName, IRuntimeMetadataAccess metadataAccess)
        {
            this.WriteMarshalVariableToNative(writer, sourceVariable, destinationVariable, managedVariableName, metadataAccess, false);
        }

        private void WriteMarshalVariableToNative(CppCodeWriter writer, ManagedMarshalValue sourceVariable, string destinationVariable, string managedVariableName, IRuntimeMetadataAccess metadataAccess, bool isMarshalingReturnValue)
        {
            string str;
            if (this._nativeType == (Mono.Cecil.NativeType.Error | Mono.Cecil.NativeType.Boolean))
            {
                object[] args = new object[] { sourceVariable.Load(), DefaultMarshalInfoWriter.Naming.Null };
                writer.WriteLine("if ({0} == {1})", args);
                using (new BlockWriter(writer, false))
                {
                    object[] arguments = new object[] { !string.IsNullOrEmpty(managedVariableName) ? managedVariableName : sourceVariable.GetNiceName() };
                    writer.WriteStatement(Emit.RaiseManagedException("il2cpp_codegen_get_argument_null_exception(\"{0}\")", arguments));
                }
            }
            if (this.IsFixedSizeString)
            {
                str = !this.IsWideString ? "il2cpp_codegen_marshal_string_fixed" : "il2cpp_codegen_marshal_wstring_fixed";
                object[] objArray3 = new object[] { str, sourceVariable.Load(), this._marshaledTypeName, destinationVariable, ((FixedSysStringMarshalInfo) this._marshalInfo).Size };
                writer.WriteLine("{0}({1}, ({2})&{3}, {4});", objArray3);
            }
            else if (this._canReferenceOriginalManagedString && !isMarshalingReturnValue)
            {
                if (this._nativeType != Mono.Cecil.NativeType.LPWStr)
                {
                    if (this._nativeType != (Mono.Cecil.NativeType.Error | Mono.Cecil.NativeType.Boolean))
                    {
                        throw new InvalidOperationException($"StringMarshalInfoWriter doesn't know how to marshal {this._nativeType} while maintaining reference to original managed string.");
                    }
                    string niceName = sourceVariable.GetNiceName();
                    string str4 = niceName + "NativeView";
                    string str5 = niceName + "HStringReference";
                    writer.WriteLine();
                    object[] objArray6 = new object[] { str4, sourceVariable.Load() };
                    writer.WriteLine("DECLARE_IL2CPP_STRING_AS_STRING_VIEW_OF_NATIVE_CHARS({0}, {1});", objArray6);
                    object[] objArray7 = new object[] { str5, str4 };
                    writer.WriteLine("il2cpp::utils::Il2CppHStringReference {0}({1});", objArray7);
                    object[] objArray8 = new object[] { destinationVariable, str5 };
                    writer.WriteLine("{0} = {1};", objArray8);
                }
                else
                {
                    string str2 = sourceVariable.Load();
                    object[] objArray4 = new object[] { str2, DefaultMarshalInfoWriter.Naming.Null };
                    writer.WriteLine("if ({0} != {1})", objArray4);
                    using (new BlockWriter(writer, false))
                    {
                        if (<>f__am$cache0 == null)
                        {
                            <>f__am$cache0 = new Func<FieldDefinition, bool>(null, (IntPtr) <WriteMarshalVariableToNative>m__0);
                        }
                        FieldDefinition field = DefaultMarshalInfoWriter.TypeProvider.SystemString.Fields.Single<FieldDefinition>(<>f__am$cache0);
                        object[] objArray5 = new object[] { destinationVariable, sourceVariable.Load(), DefaultMarshalInfoWriter.Naming.ForFieldAddressGetter(field) };
                        writer.WriteLine("{0} = {1}->{2}();", objArray5);
                    }
                }
            }
            else
            {
                if (this._isStringBuilder)
                {
                    str = !this.IsWideString ? "il2cpp_codegen_marshal_string_builder" : "il2cpp_codegen_marshal_wstring_builder";
                }
                else if (this._nativeType == Mono.Cecil.NativeType.BStr)
                {
                    str = "il2cpp_codegen_marshal_bstring";
                }
                else if (this._nativeType == (Mono.Cecil.NativeType.Error | Mono.Cecil.NativeType.Boolean))
                {
                    str = "il2cpp_codegen_create_hstring";
                }
                else
                {
                    str = !this.IsWideString ? "il2cpp_codegen_marshal_string" : "il2cpp_codegen_marshal_wstring";
                }
                object[] objArray9 = new object[] { destinationVariable, str, sourceVariable.Load() };
                writer.WriteLine("{0} = {1}({2});", objArray9);
            }
        }

        private int BytesPerCharacter =>
            (!this.IsWideString ? 1 : 2);

        private bool IsFixedSizeString =>
            (this._nativeType == Mono.Cecil.NativeType.FixedSysString);

        private bool IsWideString =>
            ((((this._nativeType == Mono.Cecil.NativeType.LPWStr) || (this._nativeType == Mono.Cecil.NativeType.BStr)) || (this._nativeType == (Mono.Cecil.NativeType.Error | Mono.Cecil.NativeType.Boolean))) || (this.IsFixedSizeString && this._useUnicodeCharSet));

        public override MarshaledType[] MarshaledTypes =>
            this._marshaledTypes;

        public override int NativeSizeWithoutPointers
        {
            get
            {
                if (this.IsFixedSizeString)
                {
                    return (((FixedSysStringMarshalInfo) this._marshalInfo).Size * this.BytesPerCharacter);
                }
                return base.NativeSizeWithoutPointers;
            }
        }

        public Mono.Cecil.NativeType NativeType =>
            this._nativeType;
    }
}

