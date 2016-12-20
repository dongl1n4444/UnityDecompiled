namespace Unity.IL2CPP.Marshaling.MarshalInfoWriters
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using Unity.IL2CPP;
    using Unity.IL2CPP.Marshaling;

    public class LPStructMarshalInfoWriter : DefaultMarshalInfoWriter
    {
        private readonly MarshaledType[] _marshaledTypes;

        public LPStructMarshalInfoWriter(TypeReference type, MarshalType marshalType) : base(type)
        {
            string name = DefaultMarshalInfoWriter.Naming.ForVariable(base._typeRef);
            this._marshaledTypes = new MarshaledType[] { new MarshaledType(name, name + "*") };
        }

        public override string WriteMarshalVariableFromNative(CppCodeWriter writer, string variableName, IList<MarshaledParameter> methodParameters, bool returnValue, bool forNativeWrapperOfManagedMethod, IRuntimeMetadataAccess metadataAccess)
        {
            return DefaultMarshalInfoWriter.Naming.Dereference(variableName);
        }

        public override string WriteMarshalVariableToNative(CppCodeWriter writer, ManagedMarshalValue sourceVariable, string managedVariableName, IRuntimeMetadataAccess metadataAccess)
        {
            return sourceVariable.LoadAddress();
        }

        public override MarshaledType[] MarshaledTypes
        {
            get
            {
                return this._marshaledTypes;
            }
        }
    }
}

