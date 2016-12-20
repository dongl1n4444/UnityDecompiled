namespace Unity.IL2CPP.Marshaling.MarshalInfoWriters
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using Unity.IL2CPP;
    using Unity.IL2CPP.Marshaling;

    public sealed class UnmarshalableMarshalInfoWriter : MarshalableMarshalInfoWriter
    {
        private readonly MarshaledType[] _marshaledTypes;

        public UnmarshalableMarshalInfoWriter(TypeReference type) : base(type)
        {
            string str;
            if (base._typeRef is GenericParameter)
            {
                str = "void*";
            }
            else
            {
                str = DefaultMarshalInfoWriter.Naming.ForVariable(base._typeRef);
            }
            this._marshaledTypes = new MarshaledType[] { new MarshaledType(str, str) };
        }

        public override bool CanMarshalTypeToNative()
        {
            return false;
        }

        public override string GetMarshalingException()
        {
            return string.Format("il2cpp_codegen_get_marshal_directive_exception(\"Cannot marshal type '{0}'.\")", base._typeRef.FullName);
        }

        public override void WriteIncludesForFieldDeclaration(CppCodeWriter writer)
        {
        }

        public override void WriteIncludesForMarshaling(CppCodeWriter writer)
        {
        }

        public override void WriteMarshaledTypeForwardDeclaration(CppCodeWriter writer)
        {
            writer.AddForwardDeclaration(base._typeRef);
        }

        public override string WriteMarshalEmptyVariableFromNative(CppCodeWriter writer, string variableName, IList<MarshaledParameter> methodParameters, IRuntimeMetadataAccess metadataAccess)
        {
            throw new InvalidOperationException(string.Format("Cannot marshal {0} from native!", base._typeRef.FullName));
        }

        public override string WriteMarshalEmptyVariableToNative(CppCodeWriter writer, ManagedMarshalValue variableName, IList<MarshaledParameter> methodParameters)
        {
            throw new InvalidOperationException(string.Format("Cannot marshal {0} to native!", base._typeRef.FullName));
        }

        public override void WriteMarshalVariableFromNative(CppCodeWriter writer, string variableName, ManagedMarshalValue destinationVariable, IList<MarshaledParameter> methodParameters, bool returnValue, bool forNativeWrapperOfManagedMethod, IRuntimeMetadataAccess metadataAccess)
        {
            throw new InvalidOperationException(string.Format("Cannot marshal {0} from native!", base._typeRef.FullName));
        }

        public override void WriteMarshalVariableToNative(CppCodeWriter writer, ManagedMarshalValue sourceVariable, string destinationVariable, string managedVariableName, IRuntimeMetadataAccess metadataAccess)
        {
            throw new InvalidOperationException(string.Format("Cannot marshal {0} to native!", base._typeRef.FullName));
        }

        public override MarshaledType[] MarshaledTypes
        {
            get
            {
                return this._marshaledTypes;
            }
        }

        public override string NativeSize
        {
            get
            {
                return "-1";
            }
        }
    }
}

