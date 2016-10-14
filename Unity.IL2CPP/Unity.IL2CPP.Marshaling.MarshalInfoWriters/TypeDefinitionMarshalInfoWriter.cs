using Mono.Cecil;
using System;

namespace Unity.IL2CPP.Marshaling.MarshalInfoWriters
{
	internal sealed class TypeDefinitionMarshalInfoWriter : CustomMarshalInfoWriter
	{
		private readonly int _nativeSizeWithoutPointers;

		public override int NativeSizeWithoutPointers
		{
			get
			{
				return this._nativeSizeWithoutPointers;
			}
		}

		public TypeDefinitionMarshalInfoWriter(TypeDefinition type, MarshalType marshalType) : base(type, marshalType)
		{
			this._nativeSizeWithoutPointers = this.CalculateNativeSizeWithoutPointers();
		}

		protected override void WriteMarshalToNativeMethodDefinition(CppCodeWriter writer)
		{
			string uniqueIdentifier = string.Format("{0}_{1}_ToNativeMethodDefinition", DefaultMarshalInfoWriter.Naming.ForType(this._type), MarshalingUtils.MarshalTypeToString(this._marshalType));
			MethodWriter.WriteMethodWithMetadataInitialization(writer, this._marshalToNativeFunctionDeclaration, this._marshalToNativeFunctionName, delegate(CppCodeWriter bodyWriter, MetadataUsage metadataUsage, MethodUsage methodUsage)
			{
				DefaultRuntimeMetadataAccess metadataAccess = new DefaultRuntimeMetadataAccess(null, metadataUsage, methodUsage);
				for (int i = 0; i < base.Fields.Length; i++)
				{
					base.FieldMarshalInfoWriters[i].WriteMarshalVariableToNative(bodyWriter, new ManagedMarshalValue("unmarshaled", base.Fields[i]), string.Format("marshaled.{0}", DefaultMarshalInfoWriter.Naming.ForField(base.Fields[i])), null, metadataAccess);
				}
			}, uniqueIdentifier);
		}

		protected override void WriteMarshalFromNativeMethodDefinition(CppCodeWriter writer)
		{
			string uniqueIdentifier = string.Format("{0}_{1}_FromNativeMethodDefinition", DefaultMarshalInfoWriter.Naming.ForType(this._type), MarshalingUtils.MarshalTypeToString(this._marshalType));
			MethodWriter.WriteMethodWithMetadataInitialization(writer, this._marshalFromNativeFunctionDeclaration, this._marshalFromNativeFunctionName, delegate(CppCodeWriter bodyWriter, MetadataUsage metadataUsage, MethodUsage methodUsage)
			{
				DefaultRuntimeMetadataAccess metadataAccess = new DefaultRuntimeMetadataAccess(null, metadataUsage, methodUsage);
				for (int i = 0; i < base.Fields.Length; i++)
				{
					FieldDefinition fieldDefinition = base.Fields[i];
					ManagedMarshalValue destinationVariable = new ManagedMarshalValue("unmarshaled", fieldDefinition);
					if (!fieldDefinition.FieldType.IsValueType())
					{
						base.FieldMarshalInfoWriters[i].WriteMarshalVariableFromNative(bodyWriter, string.Format("marshaled.{0}", DefaultMarshalInfoWriter.Naming.ForField(fieldDefinition)), destinationVariable, null, false, false, metadataAccess);
					}
					else
					{
						string text = destinationVariable.GetNiceName() + "_temp_" + i;
						bodyWriter.WriteVariable(fieldDefinition.FieldType, text);
						base.FieldMarshalInfoWriters[i].WriteMarshalVariableFromNative(bodyWriter, string.Format("marshaled.{0}", DefaultMarshalInfoWriter.Naming.ForField(fieldDefinition)), new ManagedMarshalValue(text), null, false, false, metadataAccess);
						bodyWriter.WriteLine(destinationVariable.Store(text));
					}
				}
			}, uniqueIdentifier);
		}

		protected override void WriteMarshalCleanupFunction(CppCodeWriter writer)
		{
			string uniqueIdentifier = string.Format("{0}_{1}_MarshalCleanupMethodDefinition", DefaultMarshalInfoWriter.Naming.ForType(this._type), MarshalingUtils.MarshalTypeToString(this._marshalType));
			MethodWriter.WriteMethodWithMetadataInitialization(writer, this._marshalCleanupFunctionDeclaration, this._marshalToNativeFunctionName, delegate(CppCodeWriter bodyWriter, MetadataUsage metadataUsage, MethodUsage methodUsage)
			{
				DefaultRuntimeMetadataAccess metadataAccess = new DefaultRuntimeMetadataAccess(null, metadataUsage, methodUsage);
				for (int i = 0; i < base.Fields.Length; i++)
				{
					base.FieldMarshalInfoWriters[i].WriteMarshalCleanupVariable(bodyWriter, string.Format("marshaled.{0}", DefaultMarshalInfoWriter.Naming.ForField(base.Fields[i])), metadataAccess, null);
				}
			}, uniqueIdentifier);
		}

		internal int CalculateNativeSizeWithoutPointers()
		{
			int num = 0;
			DefaultMarshalInfoWriter[] fieldMarshalInfoWriters = base.FieldMarshalInfoWriters;
			for (int i = 0; i < fieldMarshalInfoWriters.Length; i++)
			{
				DefaultMarshalInfoWriter defaultMarshalInfoWriter = fieldMarshalInfoWriters[i];
				num += defaultMarshalInfoWriter.NativeSizeWithoutPointers;
			}
			return num;
		}
	}
}
