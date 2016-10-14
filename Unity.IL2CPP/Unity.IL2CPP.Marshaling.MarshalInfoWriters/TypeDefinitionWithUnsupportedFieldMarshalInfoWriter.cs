using Mono.Cecil;
using System;

namespace Unity.IL2CPP.Marshaling.MarshalInfoWriters
{
	public sealed class TypeDefinitionWithUnsupportedFieldMarshalInfoWriter : CustomMarshalInfoWriter
	{
		private readonly FieldDefinition _faultyField;

		public override string NativeSize
		{
			get
			{
				return "-1";
			}
		}

		public TypeDefinitionWithUnsupportedFieldMarshalInfoWriter(TypeDefinition type, MarshalType marshalType, FieldDefinition faultyField) : base(type, marshalType)
		{
			this._faultyField = faultyField;
		}

		private void WriteThrowNotSupportedException(CppCodeWriter writer)
		{
			string text = DefaultMarshalInfoWriter.Naming.ForField(this._faultyField) + "Exception";
			writer.WriteLine("Il2CppCodeGenException* {0} = {1};", new object[]
			{
				text,
				this.GetMarshalingException()
			});
			writer.WriteStatement(Emit.RaiseManagedException(text));
		}

		protected override void WriteMarshalToNativeMethodDefinition(CppCodeWriter writer)
		{
			writer.WriteLine(this._marshalToNativeFunctionDeclaration);
			writer.BeginBlock();
			this.WriteThrowNotSupportedException(writer);
			writer.EndBlock(false);
		}

		protected override void WriteMarshalFromNativeMethodDefinition(CppCodeWriter writer)
		{
			writer.WriteLine(this._marshalFromNativeFunctionDeclaration);
			writer.BeginBlock();
			this.WriteThrowNotSupportedException(writer);
			writer.EndBlock(false);
		}

		protected override void WriteMarshalCleanupFunction(CppCodeWriter writer)
		{
			writer.WriteLine(this._marshalCleanupFunctionDeclaration);
			writer.WriteLine("{");
			writer.WriteLine("}");
		}

		public override bool CanMarshalTypeToNative()
		{
			return false;
		}

		public override string GetMarshalingException()
		{
			string result;
			if (this._faultyField.FieldType.MetadataType == MetadataType.Class || (this._faultyField.FieldType.IsArray && ((ArrayType)this._faultyField.FieldType).ElementType.MetadataType == MetadataType.Class))
			{
				result = string.Format("il2cpp_codegen_get_marshal_directive_exception(\"Cannot marshal field '{0}' of type '{1}': Reference type field marshaling is not supported.\")", this._faultyField.Name, this._type.Name);
			}
			else
			{
				result = string.Format("il2cpp_codegen_get_marshal_directive_exception(\"Cannot marshal field '{0}' of type '{1}'.\")", this._faultyField.Name, this._type.Name);
			}
			return result;
		}
	}
}
