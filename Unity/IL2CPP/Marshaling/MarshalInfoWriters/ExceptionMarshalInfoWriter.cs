namespace Unity.IL2CPP.Marshaling.MarshalInfoWriters
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using Unity.IL2CPP;
    using Unity.IL2CPP.Marshaling;

    internal class ExceptionMarshalInfoWriter : DefaultMarshalInfoWriter
    {
        private MarshaledType[] _marshaledTypes;

        public ExceptionMarshalInfoWriter(TypeReference type) : base(type)
        {
            string name = DefaultMarshalInfoWriter.Naming.ForVariable(DefaultMarshalInfoWriter.TypeProvider.Int32TypeReference);
            this._marshaledTypes = new MarshaledType[] { new MarshaledType(name, name) };
        }

        public override void WriteIncludesForFieldDeclaration(CppCodeWriter writer)
        {
        }

        public override void WriteIncludesForMarshaling(CppCodeWriter writer)
        {
            writer.AddIncludeForTypeDefinition(DefaultMarshalInfoWriter.TypeProvider.SystemException);
        }

        public override void WriteMarshaledTypeForwardDeclaration(CppCodeWriter writer)
        {
        }

        public override string WriteMarshalEmptyVariableFromNative(CppCodeWriter writer, string variableName, IList<MarshaledParameter> methodParameters, IRuntimeMetadataAccess metadataAccess) => 
            DefaultMarshalInfoWriter.Naming.Null;

        public override string WriteMarshalEmptyVariableToNative(CppCodeWriter writer, ManagedMarshalValue variableName, IList<MarshaledParameter> methodParameters) => 
            "0";

        public override string WriteMarshalVariableFromNative(CppCodeWriter writer, string variableName, IList<MarshaledParameter> methodParameters, bool returnValue, bool forNativeWrapperOfManagedMethod, IRuntimeMetadataAccess metadataAccess) => 
            $"({variableName} != IL2CPP_S_OK ? reinterpret_cast<{DefaultMarshalInfoWriter.Naming.ForVariable(DefaultMarshalInfoWriter.TypeProvider.SystemException)}>(il2cpp_codegen_com_get_exception({variableName}, false)) : {DefaultMarshalInfoWriter.Naming.Null})";

        public override void WriteMarshalVariableFromNative(CppCodeWriter writer, string variableName, ManagedMarshalValue destinationVariable, IList<MarshaledParameter> methodParameters, bool returnValue, bool forNativeWrapperOfManagedMethod, IRuntimeMetadataAccess metadataAccess)
        {
            writer.WriteLine(destinationVariable.Store(this.WriteMarshalVariableFromNative(writer, variableName, methodParameters, returnValue, forNativeWrapperOfManagedMethod, metadataAccess)));
        }

        public override string WriteMarshalVariableToNative(CppCodeWriter writer, ManagedMarshalValue sourceVariable, string managedVariableName, IRuntimeMetadataAccess metadataAccess) => 
            $"({sourceVariable.Load()} != {DefaultMarshalInfoWriter.Naming.Null} ? reinterpret_cast<Il2CppException*>({sourceVariable.Load()})->hresult : IL2CPP_S_OK)";

        public override void WriteMarshalVariableToNative(CppCodeWriter writer, ManagedMarshalValue sourceVariable, string destinationVariable, string managedVariableName, IRuntimeMetadataAccess metadataAccess)
        {
            writer.WriteLine($"{destinationVariable} = {this.WriteMarshalVariableToNative(writer, sourceVariable, managedVariableName, metadataAccess)};");
        }

        public override MarshaledType[] MarshaledTypes =>
            this._marshaledTypes;
    }
}

