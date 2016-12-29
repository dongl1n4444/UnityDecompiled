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

    public abstract class CustomMarshalInfoWriter : MarshalableMarshalInfoWriter
    {
        private readonly MethodDefinition _defaultConstructor;
        private DefaultMarshalInfoWriter[] _fieldMarshalInfoWriters;
        private FieldDefinition[] _fields;
        private readonly bool _forFieldMarshaling;
        protected readonly string _marshalCleanupFunctionDeclaration;
        protected readonly string _marshalCleanupFunctionName;
        protected readonly string _marshaledDecoratedTypeName;
        protected readonly string _marshaledTypeName;
        private readonly MarshaledType[] _marshaledTypes;
        protected readonly string _marshalFromNativeFunctionDeclaration;
        protected readonly string _marshalFromNativeFunctionName;
        protected readonly string _marshalToNativeFunctionDeclaration;
        protected readonly string _marshalToNativeFunctionName;
        protected readonly MarshalType _marshalType;
        protected readonly TypeDefinition _type;
        [CompilerGenerated]
        private static Func<MethodDefinition, bool> <>f__am$cache0;

        protected CustomMarshalInfoWriter(TypeDefinition type, MarshalType marshalType, bool forFieldMarshaling) : base(type)
        {
            this._type = type;
            this._marshalType = marshalType;
            string str = DefaultMarshalInfoWriter.Naming.ForTypeNameOnly(type);
            string str2 = '_' + MarshalingUtils.MarshalTypeToString(marshalType);
            this._forFieldMarshaling = forFieldMarshaling;
            this._marshaledTypeName = GetMarshaledTypeName(type, marshalType);
            this._marshaledDecoratedTypeName = !this.TreatAsValueType() ? (this._marshaledTypeName + "*") : this._marshaledTypeName;
            this._marshaledTypes = new MarshaledType[] { new MarshaledType(this._marshaledTypeName, this._marshaledDecoratedTypeName) };
            this._marshalToNativeFunctionName = $"{str}_marshal{str2}";
            this._marshalFromNativeFunctionName = $"{str}_marshal{str2}_back";
            this._marshalCleanupFunctionName = $"{str}_marshal{str2}_cleanup";
            this._marshalToNativeFunctionDeclaration = $"extern "C" void {this.MarshalToNativeFunctionName}(const {str}& unmarshaled, {this._marshaledTypeName}& marshaled)";
            this._marshalFromNativeFunctionDeclaration = $"extern "C" void {this._marshalFromNativeFunctionName}(const {this._marshaledTypeName}& marshaled, {str}& unmarshaled)";
            this._marshalCleanupFunctionDeclaration = $"extern "C" void {this.MarshalCleanupFunctionName}({this._marshaledTypeName}& marshaled)";
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = new Func<MethodDefinition, bool>(null, (IntPtr) <CustomMarshalInfoWriter>m__0);
            }
            this._defaultConstructor = this._type.Methods.SingleOrDefault<MethodDefinition>(<>f__am$cache0);
        }

        [CompilerGenerated]
        private static bool <CustomMarshalInfoWriter>m__0(MethodDefinition ctor) => 
            ((ctor.Name == ".ctor") && (ctor.Parameters.Count == 0));

        public override string DecorateVariable(string unmarshaledParameterName, string marshaledVariableName)
        {
            if (!this.TreatAsValueType())
            {
                if (unmarshaledParameterName == null)
                {
                    throw new InvalidOperationException("CustomMarshalInfoWriter does not support decorating return value parameters.");
                }
                return string.Format("{0} != {2} ? &{1} : {2}", unmarshaledParameterName, marshaledVariableName, DefaultMarshalInfoWriter.Naming.Null);
            }
            return marshaledVariableName;
        }

        internal static void EmitCallToConstructor(CppCodeWriter writer, TypeDefinition typeDefinition, MethodDefinition defaultConstructor, string variableName, ManagedMarshalValue destinationVariable, Action writeMarshalFromNativeCode, bool emitNullCheck, IRuntimeMetadataAccess metadataAccess)
        {
            if (emitNullCheck)
            {
                object[] args = new object[] { destinationVariable.Load(), DefaultMarshalInfoWriter.Naming.Null };
                writer.WriteLine("if ({0} != {1})", args);
                writer.BeginBlock();
            }
            if (defaultConstructor != null)
            {
                if (MethodSignatureWriter.NeedsHiddenMethodInfo(defaultConstructor, MethodCallType.Normal, true))
                {
                    object[] objArray2 = new object[] { DefaultMarshalInfoWriter.Naming.ForMethodNameOnly(defaultConstructor), destinationVariable.Load(), metadataAccess.HiddenMethodInfo(defaultConstructor) };
                    writer.WriteLine("{0}({1}, {2});", objArray2);
                }
                else
                {
                    object[] objArray3 = new object[] { DefaultMarshalInfoWriter.Naming.ForMethodNameOnly(defaultConstructor), destinationVariable.Load() };
                    writer.WriteLine("{0}({1});", objArray3);
                }
            }
            else
            {
                writer.WriteStatement(Emit.RaiseManagedException($"il2cpp_codegen_get_missing_method_exception("A parameterless constructor is required for type '{typeDefinition.FullName}'.")"));
            }
            writeMarshalFromNativeCode.Invoke();
            if (emitNullCheck)
            {
                writer.EndBlock(false);
            }
        }

        internal static void EmitNewObject(CppCodeWriter writer, TypeReference typeReference, string unmarshaledVariableName, string marshaledVariableName, bool emitNullCheck, IRuntimeMetadataAccess metadataAccess)
        {
            if (emitNullCheck)
            {
                object[] args = new object[] { DefaultMarshalInfoWriter.Naming.ForVariable(typeReference), unmarshaledVariableName, marshaledVariableName, metadataAccess.TypeInfoFor(typeReference), DefaultMarshalInfoWriter.Naming.Null };
                writer.WriteLine("{0} {1} = ({2} != {4}) ? ({0})il2cpp_codegen_object_new({3}) : {4};", args);
            }
            else
            {
                object[] objArray2 = new object[] { DefaultMarshalInfoWriter.Naming.ForVariable(typeReference), unmarshaledVariableName, metadataAccess.TypeInfoFor(typeReference) };
                writer.WriteLine("{0} {1} = ({0})il2cpp_codegen_object_new({2});", objArray2);
            }
        }

        private static string GetMarshaledTypeName(TypeReference type, MarshalType marshalType) => 
            $"{DefaultMarshalInfoWriter.Naming.ForTypeNameOnly(type)}_marshaled_{MarshalingUtils.MarshalTypeToString(marshalType)}";

        protected static DefaultMarshalInfoWriter MarshalInfoWriterFor(TypeReference type, MarshalType marshalType, MarshalInfo marshalInfo, bool useUnicodeCharSet, bool forFieldMarshaling = false)
        {
            TypeReference reference = type;
            MarshalType type2 = marshalType;
            MarshalInfo info = marshalInfo;
            bool flag = useUnicodeCharSet;
            bool flag2 = forFieldMarshaling;
            return MarshalDataCollector.MarshalInfoWriterFor(reference, type2, info, flag, false, flag2, null);
        }

        private void PopulateFields()
        {
            this._fields = MarshalingUtils.GetMarshaledFields(this._type, this._marshalType).ToArray<FieldDefinition>();
            this._fieldMarshalInfoWriters = MarshalingUtils.GetFieldMarshalInfoWriters(this._type, this._marshalType).ToArray<DefaultMarshalInfoWriter>();
        }

        public override bool TreatAsValueType() => 
            (this._type.IsValueType || (((this._type.MetadataType == MetadataType.Class) && (this._marshalType == MarshalType.PInvoke)) && this._forFieldMarshaling));

        public override string UndecorateVariable(string variableName)
        {
            if (!this.TreatAsValueType())
            {
                return ("*" + variableName);
            }
            return variableName;
        }

        public override void WriteDeclareAndAllocateObject(CppCodeWriter writer, string unmarshaledVariableName, string marshaledVariableName, IRuntimeMetadataAccess metadataAccess)
        {
            if (this._type.IsValueType)
            {
                base.WriteDeclareAndAllocateObject(writer, unmarshaledVariableName, marshaledVariableName, metadataAccess);
            }
            else
            {
                EmitNewObject(writer, this._type, unmarshaledVariableName, marshaledVariableName, true, metadataAccess);
            }
        }

        private void WriteFieldWithExplicitLayout(CppCodeWriter writer, FieldDefinition field, bool forAlignmentOnly)
        {
            int num = TypeDefinitionWriter.AlignmentPackingSizeFor(this._type);
            bool flag = (!forAlignmentOnly && TypeDefinitionWriter.NeedsPackingForNative(this._type)) || ((num != -1) && (num != 0));
            string fieldNameSuffix = !forAlignmentOnly ? string.Empty : "_forAlignmentOnly";
            int offset = field.Offset;
            if (flag)
            {
                object[] args = new object[] { !forAlignmentOnly ? TypeDefinitionWriter.FieldLayoutPackingSizeFor(this._type) : num };
                writer.WriteLine("#pragma pack(push, tp, {0})", args);
            }
            writer.WriteLine("struct");
            writer.BeginBlock();
            if (offset > 0)
            {
                object[] objArray2 = new object[] { DefaultMarshalInfoWriter.Naming.ForFieldPadding(field) + fieldNameSuffix, offset };
                writer.WriteLine("char {0}[{1}];", objArray2);
            }
            MarshalInfoWriterFor(field.FieldType, this._marshalType, field.MarshalInfo, this._type.IsUnicodeClass, true).WriteFieldDeclaration(writer, field, fieldNameSuffix);
            writer.EndBlock(true);
            if (flag)
            {
                writer.WriteLine("#pragma pack(pop, tp)");
            }
        }

        public override void WriteIncludesForMarshaling(CppCodeWriter writer)
        {
            base.WriteIncludesForMarshaling(writer);
            writer.AddIncludeForMethodDeclarations(base._typeRef);
        }

        protected abstract void WriteMarshalCleanupFunction(CppCodeWriter writer);
        public override void WriteMarshalCleanupVariable(CppCodeWriter writer, string variableName, IRuntimeMetadataAccess metadataAccess, string managedVariableName = null)
        {
            if (this.TreatAsValueType())
            {
                object[] args = new object[] { this.MarshalCleanupFunctionName, variableName };
                writer.WriteLine("{0}({1});", args);
            }
            else if (!string.IsNullOrEmpty(managedVariableName))
            {
                object[] objArray2 = new object[] { managedVariableName, DefaultMarshalInfoWriter.Naming.Null, this.MarshalCleanupFunctionName, variableName };
                writer.WriteLine("if ({0} != {1}) {2}({3});", objArray2);
            }
            else
            {
                object[] objArray3 = new object[] { variableName, DefaultMarshalInfoWriter.Naming.Null, this.MarshalCleanupFunctionName, variableName };
                writer.WriteLine("if (&({0}) != {1}) {2}({3});", objArray3);
            }
        }

        public override string WriteMarshalEmptyVariableFromNative(CppCodeWriter writer, string variableName, IList<MarshaledParameter> methodParameters, IRuntimeMetadataAccess metadataAccess)
        {
            if (!this.TreatAsValueType())
            {
                string str = $"_{DefaultMarshalInfoWriter.CleanVariableName(variableName)}_empty";
                object[] args = new object[] { DefaultMarshalInfoWriter.Naming.ForVariable(this._type), str, DefaultMarshalInfoWriter.Naming.AddressOf(variableName), metadataAccess.TypeInfoFor(this._type), DefaultMarshalInfoWriter.Naming.Null };
                writer.WriteLine("{0} {1} = ({2} != {4}) ? ({0})il2cpp_codegen_object_new({3}) : {4};", args);
                return str;
            }
            return base.WriteMarshalEmptyVariableFromNative(writer, variableName, methodParameters, metadataAccess);
        }

        protected abstract void WriteMarshalFromNativeMethodDefinition(CppCodeWriter writer);
        public override void WriteMarshalFunctionDeclarations(CppCodeWriter writer)
        {
            writer.AddForwardDeclaration(this._type);
            writer.AddForwardDeclaration($"struct {this._marshaledTypeName}");
            writer.WriteLine();
            writer.WriteCommentedLine("Methods for marshaling");
            object[] args = new object[] { DefaultMarshalInfoWriter.Naming.ForTypeNameOnly(this._type) };
            writer.WriteLine("struct {0};", args);
            object[] objArray2 = new object[] { this._marshaledTypeName };
            writer.WriteLine("struct {0};", objArray2);
            writer.WriteLine();
            object[] objArray3 = new object[] { this._marshalToNativeFunctionDeclaration };
            writer.WriteLine("{0};", objArray3);
            object[] objArray4 = new object[] { this._marshalFromNativeFunctionDeclaration };
            writer.WriteLine("{0};", objArray4);
            object[] objArray5 = new object[] { this._marshalCleanupFunctionDeclaration };
            writer.WriteLine("{0};", objArray5);
        }

        public override void WriteMarshalFunctionDefinitions(CppCodeWriter writer, IMethodCollector methodCollector)
        {
            for (int i = 0; i < this.Fields.Length; i++)
            {
                this.FieldMarshalInfoWriters[i].WriteIncludesForMarshaling(writer);
            }
            object[] args = new object[] { this._type.FullName };
            writer.WriteLine("// Conversion methods for marshalling of: {0}", args);
            this.WriteMarshalToNativeMethodDefinition(writer);
            this.WriteMarshalFromNativeMethodDefinition(writer);
            object[] objArray2 = new object[] { this._type.FullName };
            writer.WriteLine("// Conversion method for clean up from marshalling of: {0}", objArray2);
            this.WriteMarshalCleanupFunction(writer);
            if (this._marshalType == MarshalType.PInvoke)
            {
                methodCollector.AddTypeMarshallingFunctions(this._type);
            }
        }

        protected abstract void WriteMarshalToNativeMethodDefinition(CppCodeWriter writer);
        public override void WriteMarshalVariableFromNative(CppCodeWriter writer, string variableName, ManagedMarshalValue destinationVariable, IList<MarshaledParameter> methodParameters, bool returnValue, bool forNativeWrapperOfManagedMethod, IRuntimeMetadataAccess metadataAccess)
        {
            <WriteMarshalVariableFromNative>c__AnonStorey0 storey = new <WriteMarshalVariableFromNative>c__AnonStorey0 {
                writer = writer,
                variableName = variableName,
                destinationVariable = destinationVariable,
                $this = this
            };
            if (this.TreatAsValueType())
            {
                if (this._type.MetadataType == MetadataType.Class)
                {
                    object[] args = new object[] { DefaultMarshalInfoWriter.Naming.ForVariable(this._type), metadataAccess.TypeInfoFor(this._type) };
                    storey.writer.WriteLine(storey.destinationVariable.Store("({0})il2cpp_codegen_object_new({1})", args));
                    Action writeMarshalFromNativeCode = new Action(storey, (IntPtr) this.<>m__0);
                    EmitCallToConstructor(storey.writer, this._type, this._defaultConstructor, storey.variableName, storey.destinationVariable, writeMarshalFromNativeCode, false, metadataAccess);
                }
                else
                {
                    object[] objArray2 = new object[] { this.MarshalFromNativeFunctionName, storey.variableName, storey.destinationVariable.Load() };
                    storey.writer.WriteLine("{0}({1}, {2});", objArray2);
                }
            }
            else
            {
                Action action2 = new Action(storey, (IntPtr) this.<>m__1);
                EmitCallToConstructor(storey.writer, this._type, this._defaultConstructor, storey.variableName, storey.destinationVariable, action2, true, metadataAccess);
            }
        }

        public override void WriteMarshalVariableToNative(CppCodeWriter writer, ManagedMarshalValue sourceVariable, string destinationVariable, string managedVariableName, IRuntimeMetadataAccess metadataAccess)
        {
            string block = !this._type.IsValueType ? "if ({1} != {3}) {0}(*{1}, {2});" : "{0}({1}, {2});";
            object[] args = new object[] { this.MarshalToNativeFunctionName, sourceVariable.Load(), destinationVariable, DefaultMarshalInfoWriter.Naming.Null };
            writer.WriteLine(block, args);
        }

        public override void WriteNativeStructDefinition(CppCodeWriter writer)
        {
            object[] args = new object[] { MarshalingUtils.MarshalTypeToNiceString(this._marshalType), this._type.FullName };
            writer.WriteLine("// Native definition for {0} marshalling of {1}", args);
            foreach (FieldDefinition definition in MarshalingUtils.NonStaticFieldsOf(this._type))
            {
                MarshalInfoWriterFor(definition.FieldType, this._marshalType, definition.MarshalInfo, this._type.IsUnicodeClass, true).WriteIncludesForFieldDeclaration(writer);
            }
            bool flag = TypeDefinitionWriter.NeedsPackingForNative(this._type) && !this._type.IsExplicitLayout;
            if (flag)
            {
                object[] objArray2 = new object[] { TypeDefinitionWriter.FieldLayoutPackingSizeFor(this._type) };
                writer.WriteLine("#pragma pack(push, tp, {0})", objArray2);
            }
            object[] objArray3 = new object[] { this._marshaledTypeName, (((this._type.BaseType == null) || this._type.BaseType.IsSpecialSystemBaseType()) || !MarshalDataCollector.MarshalInfoWriterFor(this._type.BaseType, this._marshalType, null, false, false, false, null).HasNativeStructDefinition) ? string.Empty : $" : public {GetMarshaledTypeName(this._type.BaseType, this._marshalType)}" };
            writer.WriteLine("struct {0}{1}", objArray3);
            writer.BeginBlock();
            using (new TypeDefinitionPaddingWriter(writer, this._type))
            {
                if (!this._type.IsExplicitLayout)
                {
                    foreach (FieldDefinition definition2 in MarshalingUtils.NonStaticFieldsOf(this._type))
                    {
                        MarshalInfoWriterFor(definition2.FieldType, this._marshalType, definition2.MarshalInfo, this._type.IsUnicodeClass, true).WriteFieldDeclaration(writer, definition2, null);
                    }
                }
                else
                {
                    writer.WriteLine("union");
                    writer.BeginBlock();
                    foreach (FieldDefinition definition3 in MarshalingUtils.NonStaticFieldsOf(this._type))
                    {
                        this.WriteFieldWithExplicitLayout(writer, definition3, false);
                        this.WriteFieldWithExplicitLayout(writer, definition3, true);
                    }
                    writer.EndBlock(true);
                }
            }
            writer.EndBlock(true);
            if (flag)
            {
                writer.WriteLine("#pragma pack(pop, tp)");
            }
        }

        protected DefaultMarshalInfoWriter[] FieldMarshalInfoWriters
        {
            get
            {
                if (this._fieldMarshalInfoWriters == null)
                {
                    this.PopulateFields();
                }
                return this._fieldMarshalInfoWriters;
            }
        }

        protected FieldDefinition[] Fields
        {
            get
            {
                if (this._fields == null)
                {
                    this.PopulateFields();
                }
                return this._fields;
            }
        }

        public sealed override bool HasNativeStructDefinition =>
            true;

        public sealed override string MarshalCleanupFunctionName =>
            this._marshalCleanupFunctionName;

        public override MarshaledType[] MarshaledTypes =>
            this._marshaledTypes;

        public sealed override string MarshalFromNativeFunctionName =>
            this._marshalFromNativeFunctionName;

        public sealed override string MarshalToNativeFunctionName =>
            this._marshalToNativeFunctionName;

        [CompilerGenerated]
        private sealed class <WriteMarshalVariableFromNative>c__AnonStorey0
        {
            internal CustomMarshalInfoWriter $this;
            internal ManagedMarshalValue destinationVariable;
            internal string variableName;
            internal CppCodeWriter writer;

            internal void <>m__0()
            {
                object[] args = new object[] { this.$this.MarshalFromNativeFunctionName, this.variableName, this.destinationVariable.Dereferenced.Load() };
                this.writer.WriteLine("{0}({1}, {2});", args);
            }

            internal void <>m__1()
            {
                object[] args = new object[] { this.$this.MarshalFromNativeFunctionName, this.variableName, this.destinationVariable.Load() };
                this.writer.WriteLine("{0}({1}, *{2});", args);
            }
        }
    }
}

