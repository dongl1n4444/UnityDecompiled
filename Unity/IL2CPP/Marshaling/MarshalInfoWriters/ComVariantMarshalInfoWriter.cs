namespace Unity.IL2CPP.Marshaling.MarshalInfoWriters
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using Unity.IL2CPP;
    using Unity.IL2CPP.Marshaling;

    public class ComVariantMarshalInfoWriter : MarshalableMarshalInfoWriter
    {
        private readonly MarshaledType[] _marshaledTypes;

        public ComVariantMarshalInfoWriter(TypeReference type) : base(type)
        {
            this._marshaledTypes = new MarshaledType[] { new MarshaledType("Il2CppVariant", "Il2CppVariant") };
        }

        public override void WriteIncludesForFieldDeclaration(CppCodeWriter writer)
        {
        }

        public override void WriteMarshalCleanupVariable(CppCodeWriter writer, string variableName, IRuntimeMetadataAccess metadataAccess, string managedVariableName = null)
        {
            object[] args = new object[] { variableName };
            writer.WriteLine("il2cpp_codegen_com_destroy_variant(&({0}));", args);
        }

        public override void WriteMarshaledTypeForwardDeclaration(CppCodeWriter writer)
        {
        }

        public override void WriteMarshalVariableFromNative(CppCodeWriter writer, string variableName, ManagedMarshalValue destinationVariable, IList<MarshaledParameter> methodParameters, bool returnValue, bool forNativeWrapperOfManagedMethod, IRuntimeMetadataAccess metadataAccess)
        {
            object[] args = new object[] { variableName };
            writer.WriteLine(destinationVariable.Store("(Il2CppObject*)il2cpp_codegen_com_marshal_variant_result(&({0}))", args));
        }

        public override void WriteMarshalVariableToNative(CppCodeWriter writer, ManagedMarshalValue sourceVariable, string destinationVariable, string managedVariableName, IRuntimeMetadataAccess metadataAccess)
        {
            object[] args = new object[] { sourceVariable.Load(), destinationVariable };
            writer.WriteLine("il2cpp_codegen_com_marshal_variant((Il2CppCodeGenObject*)({0}), &({1}));", args);
        }

        public override MarshaledType[] MarshaledTypes =>
            this._marshaledTypes;
    }
}

