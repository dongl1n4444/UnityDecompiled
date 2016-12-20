namespace Unity.Serialization.Weaver
{
    using Mono.Cecil;
    using System;

    internal sealed class NamedDeserializeMethodEmitter : DeserializeMethodEmitterBase
    {
        private LocalVariable fieldNamesVariable;

        private NamedDeserializeMethodEmitter(TypeDefinition typeDef, SerializationBridgeProvider serializationBridgeProvider) : base(typeDef, serializationBridgeProvider)
        {
            base.CreateMethodDef(this.DeserializeMethodName);
        }

        protected override void CallDeserializeMethodFor(string fieldName, TypeReference typeReference)
        {
            base.LoadStateInstance(this.SerializedStateReaderInterface);
            this.EmitCString(fieldName);
            base.Callvirt(this.SerializedStateReaderInterface, "BeginMetaGroup");
            base.CallDeserializeMethodFor(fieldName, typeReference);
            base.LoadStateInstance(this.SerializedStateReaderInterface);
            base.Callvirt(this.SerializedStateReaderInterface, "EndMetaGroup");
        }

        protected override void CallReaderMethod(string methodName, string fieldName)
        {
            this.EmitCString(fieldName);
            base.Callvirt(this.SerializedStateReaderInterface, methodName);
        }

        public static MethodDefinition DeserializeMethodDefinitionFor(TypeDefinition typeDef, SerializationBridgeProvider serializationBridgeProvider)
        {
            return new NamedDeserializeMethodEmitter(typeDef, serializationBridgeProvider).MethodDefinition;
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

        protected override void FinishReadingSequence()
        {
            base.LoadStateInstance(this.SerializedStateReaderInterface);
            base.Callvirt(this.SerializedStateReaderInterface, "EndMetaGroup");
        }

        protected override void ReadSequenceLength(string fieldName)
        {
            base.LoadStateInstance(this.SerializedStateReaderInterface);
            this.EmitCString(fieldName);
            base.Callvirt(this.SerializedStateReaderInterface, "BeginSequenceGroup");
        }

        protected override string DeserializeMethodName
        {
            get
            {
                return "Unity_NamedDeserialize";
            }
        }

        protected override TypeDefinition SerializedStateReaderInterface
        {
            get
            {
                return base._serializationBridgeProvider.SerializedNamedStateReaderInterface;
            }
        }
    }
}

