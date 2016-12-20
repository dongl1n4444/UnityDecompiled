namespace Unity.IL2CPP.Marshaling.MarshalInfoWriters
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using Unity.IL2CPP;
    using Unity.IL2CPP.Marshaling;

    internal sealed class DelegateMarshalInfoWriter : MarshalableMarshalInfoWriter
    {
        public MarshaledType[] _marshaledTypes;

        public DelegateMarshalInfoWriter(TypeReference type) : base(type)
        {
            this._marshaledTypes = new MarshaledType[] { new MarshaledType("Il2CppMethodPointer", "Il2CppMethodPointer") };
        }

        public override void WriteMarshalCleanupVariable(CppCodeWriter writer, string variableName, IRuntimeMetadataAccess metadataAccess, [Optional, DefaultParameterValue(null)] string managedVariableName)
        {
        }

        public override void WriteMarshaledTypeForwardDeclaration(CppCodeWriter writer)
        {
        }

        public override void WriteMarshalVariableFromNative(CppCodeWriter writer, string variableName, ManagedMarshalValue destinationVariable, IList<MarshaledParameter> methodParameters, bool returnValue, bool forNativeWrapperOfManagedMethod, IRuntimeMetadataAccess metadataAccess)
        {
            object[] args = new object[] { DefaultMarshalInfoWriter.Naming.ForType(base._typeRef), variableName, metadataAccess.TypeInfoFor(base._typeRef) };
            writer.WriteLine(destinationVariable.Store("il2cpp_codegen_marshal_function_ptr_to_delegate<{0}>({1}, {2})", args));
        }

        public override void WriteMarshalVariableToNative(CppCodeWriter writer, ManagedMarshalValue sourceVariable, string destinationVariable, string managedVariableName, IRuntimeMetadataAccess metadataAccess)
        {
            object[] args = new object[] { destinationVariable, sourceVariable.Load() };
            writer.WriteLine("{0} = il2cpp_codegen_marshal_delegate(reinterpret_cast<Il2CppCodeGenMulticastDelegate*>({1}));", args);
        }

        public override void WriteNativeVariableDeclarationOfType(CppCodeWriter writer, string variableName)
        {
            object[] args = new object[] { this.MarshaledTypes[0].DecoratedName, variableName };
            writer.WriteLine("{0} {1} = NULL;", args);
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

