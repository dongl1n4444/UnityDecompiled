namespace Unity.Serialization.Weaver
{
    using Mono.Cecil;
    using System;

    internal sealed class SerializeMethodEmitter : SerializeMethodEmitterBase
    {
        private SerializeMethodEmitter(TypeDefinition typeDef, SerializationBridgeProvider serializationBridgeProvider) : base(typeDef, serializationBridgeProvider)
        {
            base.CreateMethodDef(this.SerializeMethodName);
        }

        protected override void CallWriterMethod(string methodName, string fieldName)
        {
            base.Callvirt(this.SerializedStateWriterInterface, methodName);
        }

        protected override void FinishWritingSequence()
        {
        }

        public static MethodDefinition SerializeMethodDefinitionFor(TypeDefinition typeDef, SerializationBridgeProvider serializationBridgeProvider) => 
            new SerializeMethodEmitter(typeDef, serializationBridgeProvider).MethodDefinition;

        protected override void WriteSequenceLength(string fieldName, Action emitSequenceLength)
        {
            base.LoadStateInstance(this.SerializedStateWriterInterface);
            emitSequenceLength();
            base.Callvirt(this.SerializedStateWriterInterface, base.WriteMethodNameFor(base.Import(typeof(int))));
        }

        protected override TypeDefinition SerializedStateWriterInterface =>
            base._serializationBridgeProvider.SerializedStateWriterInterface;

        protected override string SerializeMethodName =>
            "Unity_Serialize";
    }
}

