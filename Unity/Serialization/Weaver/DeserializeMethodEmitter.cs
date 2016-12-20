namespace Unity.Serialization.Weaver
{
    using Mono.Cecil;
    using System;

    internal sealed class DeserializeMethodEmitter : DeserializeMethodEmitterBase
    {
        private DeserializeMethodEmitter(TypeDefinition typeDef, SerializationBridgeProvider serializationBridgeProvider) : base(typeDef, serializationBridgeProvider)
        {
            base.CreateMethodDef(this.DeserializeMethodName);
        }

        protected override void CallReaderMethod(string methodName, string fieldName)
        {
            base.Callvirt(this.SerializedStateReaderInterface, methodName);
        }

        public static MethodDefinition DeserializeMethodDefinitionFor(TypeDefinition typeDef, SerializationBridgeProvider serializationBridgeProvider)
        {
            return new DeserializeMethodEmitter(typeDef, serializationBridgeProvider).MethodDefinition;
        }

        protected override void FinishReadingSequence()
        {
        }

        protected override void ReadSequenceLength(string fieldName)
        {
            base.LoadStateInstance(this.SerializedStateReaderInterface);
            base.Callvirt(this.SerializedStateReaderInterface, base.ReadMethodNameFor(base.Import(typeof(int))));
        }

        protected override string DeserializeMethodName
        {
            get
            {
                return "Unity_Deserialize";
            }
        }

        protected override TypeDefinition SerializedStateReaderInterface
        {
            get
            {
                return base._serializationBridgeProvider.SerializedStateReaderInterface;
            }
        }
    }
}

