namespace Unity.IL2CPP.Marshaling.MarshalInfoWriters
{
    using Mono.Cecil;
    using System;
    using Unity.IL2CPP;
    using Unity.IL2CPP.Marshaling;

    public sealed class TypeDefinitionWithUnsupportedFieldMarshalInfoWriter : CustomMarshalInfoWriter
    {
        private readonly FieldDefinition _faultyField;

        public TypeDefinitionWithUnsupportedFieldMarshalInfoWriter(TypeDefinition type, MarshalType marshalType, FieldDefinition faultyField) : base(type, marshalType, false)
        {
            this._faultyField = faultyField;
        }

        public override bool CanMarshalTypeToNative() => 
            false;

        public override string GetMarshalingException()
        {
            if ((this._faultyField.FieldType.MetadataType == MetadataType.Class) || (this._faultyField.FieldType.IsArray && (((ArrayType) this._faultyField.FieldType).ElementType.MetadataType == MetadataType.Class)))
            {
                return $"il2cpp_codegen_get_marshal_directive_exception("Cannot marshal field '{this._faultyField.Name}' of type '{base._type.Name}': Reference type field marshaling is not supported.")";
            }
            return $"il2cpp_codegen_get_marshal_directive_exception("Cannot marshal field '{this._faultyField.Name}' of type '{base._type.Name}'.")";
        }

        protected override void WriteMarshalCleanupFunction(CppCodeWriter writer)
        {
            writer.WriteLine(base._marshalCleanupFunctionDeclaration);
            writer.WriteLine("{");
            writer.WriteLine("}");
        }

        protected override void WriteMarshalFromNativeMethodDefinition(CppCodeWriter writer)
        {
            writer.WriteLine(base._marshalFromNativeFunctionDeclaration);
            writer.BeginBlock();
            this.WriteThrowNotSupportedException(writer);
            writer.EndBlock(false);
        }

        protected override void WriteMarshalToNativeMethodDefinition(CppCodeWriter writer)
        {
            writer.WriteLine(base._marshalToNativeFunctionDeclaration);
            writer.BeginBlock();
            this.WriteThrowNotSupportedException(writer);
            writer.EndBlock(false);
        }

        private void WriteThrowNotSupportedException(CppCodeWriter writer)
        {
            string exception = DefaultMarshalInfoWriter.Naming.ForField(this._faultyField) + "Exception";
            object[] args = new object[] { exception, this.GetMarshalingException() };
            writer.WriteLine("Il2CppCodeGenException* {0} = {1};", args);
            writer.WriteStatement(Emit.RaiseManagedException(exception));
        }

        public override string NativeSize =>
            "-1";
    }
}

