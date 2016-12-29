namespace Unity.IL2CPP.Marshaling.MarshalInfoWriters
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using Unity.IL2CPP;
    using Unity.IL2CPP.Marshaling;

    public abstract class MarshalableMarshalInfoWriter : DefaultMarshalInfoWriter
    {
        protected MarshalableMarshalInfoWriter(TypeReference type) : base(type)
        {
        }

        public override string WriteMarshalEmptyVariableToNative(CppCodeWriter writer, ManagedMarshalValue variableName, IList<MarshaledParameter> methodParameters)
        {
            string str = $"_{variableName.GetNiceName()}_marshaled";
            this.WriteNativeVariableDeclarationOfType(writer, str);
            return str;
        }

        public override string WriteMarshalVariableFromNative(CppCodeWriter writer, string variableName, IList<MarshaledParameter> methodParameters, bool returnValue, bool forNativeWrapperOfManagedMethod, IRuntimeMetadataAccess metadataAccess)
        {
            string marshaledVariableName = variableName.Replace("*", "");
            string unmarshaledVariableName = $"_{DefaultMarshalInfoWriter.CleanVariableName(variableName)}_unmarshaled";
            this.WriteDeclareAndAllocateObject(writer, unmarshaledVariableName, marshaledVariableName, metadataAccess);
            this.WriteMarshalVariableFromNative(writer, variableName, new ManagedMarshalValue(unmarshaledVariableName), methodParameters, returnValue, forNativeWrapperOfManagedMethod, metadataAccess);
            return unmarshaledVariableName;
        }

        public sealed override string WriteMarshalVariableToNative(CppCodeWriter writer, ManagedMarshalValue sourceVariable, string managedVariableName, IRuntimeMetadataAccess metadataAccess)
        {
            string variableName = $"_{sourceVariable.GetNiceName()}_marshaled";
            this.WriteNativeVariableDeclarationOfType(writer, variableName);
            this.WriteMarshalVariableToNative(writer, sourceVariable, variableName, managedVariableName, metadataAccess);
            return variableName;
        }
    }
}

