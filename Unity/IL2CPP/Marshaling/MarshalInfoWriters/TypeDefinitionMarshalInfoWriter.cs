namespace Unity.IL2CPP.Marshaling.MarshalInfoWriters
{
    using Mono.Cecil;
    using System;
    using Unity.IL2CPP;
    using Unity.IL2CPP.Marshaling;

    internal sealed class TypeDefinitionMarshalInfoWriter : CustomMarshalInfoWriter
    {
        private readonly int _nativeSizeWithoutPointers;

        public TypeDefinitionMarshalInfoWriter(TypeDefinition type, MarshalType marshalType, bool forFieldMarshaling) : base(type, marshalType, forFieldMarshaling)
        {
            this._nativeSizeWithoutPointers = this.CalculateNativeSizeWithoutPointers();
        }

        internal int CalculateNativeSizeWithoutPointers()
        {
            int num = 0;
            foreach (DefaultMarshalInfoWriter writer in base.FieldMarshalInfoWriters)
            {
                num += writer.NativeSizeWithoutPointers;
            }
            return num;
        }

        protected override void WriteMarshalCleanupFunction(CppCodeWriter writer)
        {
            string uniqueIdentifier = $"{DefaultMarshalInfoWriter.Naming.ForType(base._type)}_{MarshalingUtils.MarshalTypeToString(base._marshalType)}_MarshalCleanupMethodDefinition";
            MethodWriter.WriteMethodWithMetadataInitialization(writer, base._marshalCleanupFunctionDeclaration, base._marshalToNativeFunctionName, delegate (CppCodeWriter bodyWriter, MetadataUsage metadataUsage, MethodUsage methodUsage) {
                DefaultRuntimeMetadataAccess metadataAccess = new DefaultRuntimeMetadataAccess(null, metadataUsage, methodUsage, null);
                for (int j = 0; j < base.Fields.Length; j++)
                {
                    base.FieldMarshalInfoWriters[j].WriteMarshalCleanupVariable(bodyWriter, base.FieldMarshalInfoWriters[j].UndecorateVariable($"marshaled.{DefaultMarshalInfoWriter.Naming.ForField(base.Fields[j])}"), metadataAccess, null);
                }
            }, uniqueIdentifier);
        }

        protected override void WriteMarshalFromNativeMethodDefinition(CppCodeWriter writer)
        {
            string uniqueIdentifier = $"{DefaultMarshalInfoWriter.Naming.ForType(base._type)}_{MarshalingUtils.MarshalTypeToString(base._marshalType)}_FromNativeMethodDefinition";
            MethodWriter.WriteMethodWithMetadataInitialization(writer, base._marshalFromNativeFunctionDeclaration, base._marshalFromNativeFunctionName, delegate (CppCodeWriter bodyWriter, MetadataUsage metadataUsage, MethodUsage methodUsage) {
                DefaultRuntimeMetadataAccess metadataAccess = new DefaultRuntimeMetadataAccess(null, metadataUsage, methodUsage, null);
                for (int j = 0; j < base.Fields.Length; j++)
                {
                    FieldDefinition field = base.Fields[j];
                    ManagedMarshalValue destinationVariable = new ManagedMarshalValue("unmarshaled", field);
                    if (!field.FieldType.IsValueType())
                    {
                        base.FieldMarshalInfoWriters[j].WriteMarshalVariableFromNative(bodyWriter, base.FieldMarshalInfoWriters[j].UndecorateVariable($"marshaled.{DefaultMarshalInfoWriter.Naming.ForField(field)}"), destinationVariable, null, false, false, metadataAccess);
                    }
                    else
                    {
                        string name = destinationVariable.GetNiceName() + "_temp_" + j;
                        bodyWriter.WriteVariable(field.FieldType, name);
                        base.FieldMarshalInfoWriters[j].WriteMarshalVariableFromNative(bodyWriter, $"marshaled.{DefaultMarshalInfoWriter.Naming.ForField(field)}", new ManagedMarshalValue(name), null, false, false, metadataAccess);
                        bodyWriter.WriteLine(destinationVariable.Store(name));
                    }
                }
            }, uniqueIdentifier);
        }

        protected override void WriteMarshalToNativeMethodDefinition(CppCodeWriter writer)
        {
            string uniqueIdentifier = $"{DefaultMarshalInfoWriter.Naming.ForType(base._type)}_{MarshalingUtils.MarshalTypeToString(base._marshalType)}_ToNativeMethodDefinition";
            MethodWriter.WriteMethodWithMetadataInitialization(writer, base._marshalToNativeFunctionDeclaration, base._marshalToNativeFunctionName, delegate (CppCodeWriter bodyWriter, MetadataUsage metadataUsage, MethodUsage methodUsage) {
                DefaultRuntimeMetadataAccess metadataAccess = new DefaultRuntimeMetadataAccess(null, metadataUsage, methodUsage, null);
                for (int j = 0; j < base.Fields.Length; j++)
                {
                    base.FieldMarshalInfoWriters[j].WriteMarshalVariableToNative(bodyWriter, new ManagedMarshalValue("unmarshaled", base.Fields[j]), base.FieldMarshalInfoWriters[j].UndecorateVariable($"marshaled.{DefaultMarshalInfoWriter.Naming.ForField(base.Fields[j])}"), null, metadataAccess);
                }
            }, uniqueIdentifier);
        }

        public override int NativeSizeWithoutPointers =>
            this._nativeSizeWithoutPointers;
    }
}

