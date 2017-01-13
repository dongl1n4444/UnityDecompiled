namespace Unity.Serialization.Weaver
{
    using Mono.Cecil;
    using System;

    internal sealed class NamedSerializeMethodEmitter : SerializeMethodEmitterBase
    {
        private LocalVariable fieldNamesVariable;

        private NamedSerializeMethodEmitter(TypeDefinition typeDef, SerializationBridgeProvider serializationBridgeProvider) : base(typeDef, serializationBridgeProvider)
        {
            base.CreateMethodDef(this.SerializeMethodName);
        }

        protected override void CallSerializeMethodFor(string fieldName, TypeReference typeReference)
        {
            base.LoadStateInstance(this.SerializedStateWriterInterface);
            this.EmitCString(fieldName);
            base.Callvirt(this.SerializedStateWriterInterface, "BeginMetaGroup");
            base.CallSerializeMethodFor(fieldName, typeReference);
            base.LoadStateInstance(this.SerializedStateWriterInterface);
            base.Callvirt(this.SerializedStateWriterInterface, "EndMetaGroup");
        }

        protected override void CallWriterMethod(string methodName, string fieldName)
        {
            this.EmitCString(fieldName);
            base.Callvirt(this.SerializedStateWriterInterface, methodName);
        }

        private void EmitCString(string fieldName)
        {
            if (fieldName != null)
            {
                if (this.fieldNamesVariable == null)
                {
                    this.fieldNamesVariable = base.DefineLocal(new PinnedType(new ByReferenceType(base.Module.TypeSystem.Byte)));
                    CStringStore.StoreStoragePointerIntoLocalVariable(base.Processor, this.fieldNamesVariable, base.Module);
                }
                this.fieldNamesVariable.EmitLoad();
                base.Conv_I();
                int stringOffset = CStringStore.GetStringOffset(base.Module, fieldName);
                if (stringOffset != 0)
                {
                    base.Ldc_I4(stringOffset);
                    base.Add();
                }
            }
            else
            {
                base.Ldc_I4(0);
                base.Conv_I();
            }
        }

        protected override void FinishWritingSequence()
        {
            base.LoadStateInstance(this.SerializedStateWriterInterface);
            base.Callvirt(this.SerializedStateWriterInterface, "EndMetaGroup");
        }

        public static MethodDefinition SerializeMethodDefinitionFor(TypeDefinition typeDef, SerializationBridgeProvider serializationBridgeProvider) => 
            new NamedSerializeMethodEmitter(typeDef, serializationBridgeProvider).MethodDefinition;

        protected override void WriteSequenceLength(string fieldName, Action emitSequenceLength)
        {
            base.LoadStateInstance(this.SerializedStateWriterInterface);
            this.EmitCString(fieldName);
            emitSequenceLength();
            base.Callvirt(this.SerializedStateWriterInterface, "BeginSequenceGroup");
        }

        protected override TypeDefinition SerializedStateWriterInterface =>
            base._serializationBridgeProvider.SerializedNamedStateWriterInterface;

        protected override string SerializeMethodName =>
            "Unity_NamedSerialize";
    }
}

