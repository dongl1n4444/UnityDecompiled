namespace Unity.IL2CPP.Marshaling.MarshalInfoWriters
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using Unity.IL2CPP;
    using Unity.IL2CPP.Marshaling;

    public class BlittableByReferenceMarshalInfoWriter : DefaultMarshalInfoWriter
    {
        private readonly TypeReference _elementType;
        private readonly DefaultMarshalInfoWriter _elementTypeMarshalInfoWriter;
        private readonly MarshaledType[] _marshaledTypes;

        public BlittableByReferenceMarshalInfoWriter(ByReferenceType type, MarshalType marshalType, MarshalInfo marshalInfo) : base(type)
        {
            this._elementType = type.ElementType;
            this._elementTypeMarshalInfoWriter = MarshalDataCollector.MarshalInfoWriterFor(this._elementType, marshalType, marshalInfo, false, true, false, null);
            if (this._elementTypeMarshalInfoWriter.MarshaledTypes.Length > 1)
            {
                throw new InvalidOperationException($"BlittableByReferenceMarshalInfoWriter cannot marshal {type.ElementType.FullName}&.");
            }
            this._marshaledTypes = new MarshaledType[] { new MarshaledType(this._elementTypeMarshalInfoWriter.MarshaledTypes[0].Name + "*", this._elementTypeMarshalInfoWriter.MarshaledTypes[0].DecoratedName + "*") };
        }

        public override void WriteMarshaledTypeForwardDeclaration(CppCodeWriter writer)
        {
            this._elementTypeMarshalInfoWriter.WriteMarshaledTypeForwardDeclaration(writer);
        }

        public override string WriteMarshalEmptyVariableFromNative(CppCodeWriter writer, string variableName, IList<MarshaledParameter> methodParameters, IRuntimeMetadataAccess metadataAccess)
        {
            string name = $"_{DefaultMarshalInfoWriter.CleanVariableName(variableName)}_empty";
            writer.WriteVariable(this._elementType, name);
            return DefaultMarshalInfoWriter.Naming.AddressOf(name);
        }

        public override string WriteMarshalEmptyVariableToNative(CppCodeWriter writer, ManagedMarshalValue variableName, IList<MarshaledParameter> methodParameters)
        {
            string str = $"_{variableName.GetNiceName()}_empty";
            this._elementTypeMarshalInfoWriter.WriteNativeVariableDeclarationOfType(writer, str);
            return DefaultMarshalInfoWriter.Naming.AddressOf(str);
        }

        public override void WriteMarshalOutParameterFromNative(CppCodeWriter writer, string variableName, ManagedMarshalValue destinationVariable, IList<MarshaledParameter> methodParameters, bool returnValue, bool forNativeWrapperOfManagedMethod, IRuntimeMetadataAccess metadataAccess)
        {
            this._elementTypeMarshalInfoWriter.WriteMarshalVariableFromNative(writer, DefaultMarshalInfoWriter.Naming.Dereference(variableName), destinationVariable.Dereferenced, methodParameters, returnValue, forNativeWrapperOfManagedMethod, metadataAccess);
        }

        public override void WriteMarshalOutParameterToNative(CppCodeWriter writer, ManagedMarshalValue sourceVariable, string destinationVariable, string managedVariableName, IList<MarshaledParameter> methodParameters, IRuntimeMetadataAccess metadataAccess)
        {
            this._elementTypeMarshalInfoWriter.WriteMarshalVariableToNative(writer, sourceVariable.Dereferenced, DefaultMarshalInfoWriter.Naming.Dereference(this.UndecorateVariable(destinationVariable)), managedVariableName, metadataAccess);
        }

        public override string WriteMarshalVariableFromNative(CppCodeWriter writer, string variableName, IList<MarshaledParameter> methodParameters, bool returnValue, bool forNativeWrapperOfManagedMethod, IRuntimeMetadataAccess metadataAccess)
        {
            string str = DefaultMarshalInfoWriter.Naming.ForVariable(base._typeRef);
            if (str != this._marshaledTypes[0].DecoratedName)
            {
                return $"reinterpret_cast<{str}>({variableName})";
            }
            return variableName;
        }

        public override void WriteMarshalVariableFromNative(CppCodeWriter writer, string variableName, ManagedMarshalValue destinationVariable, IList<MarshaledParameter> methodParameters, bool returnValue, bool forNativeWrapperOfManagedMethod, IRuntimeMetadataAccess metadataAccess)
        {
            writer.WriteLine(destinationVariable.Store(this.WriteMarshalVariableFromNative(writer, variableName, methodParameters, returnValue, forNativeWrapperOfManagedMethod, metadataAccess)));
        }

        public override string WriteMarshalVariableToNative(CppCodeWriter writer, ManagedMarshalValue sourceVariable, string managedVariableName, IRuntimeMetadataAccess metadataAccess)
        {
            if (DefaultMarshalInfoWriter.Naming.ForVariable(base._typeRef) != this._marshaledTypes[0].DecoratedName)
            {
                return $"reinterpret_cast<{this._marshaledTypes[0].DecoratedName}>({sourceVariable.Load()})";
            }
            return sourceVariable.Load();
        }

        public override void WriteMarshalVariableToNative(CppCodeWriter writer, ManagedMarshalValue sourceVariable, string destinationVariable, string managedVariableName, IRuntimeMetadataAccess metadataAccess)
        {
            object[] args = new object[] { destinationVariable, this.WriteMarshalVariableToNative(writer, sourceVariable, managedVariableName, metadataAccess) };
            writer.WriteLine("{0} = {1};", args);
        }

        public override MarshaledType[] MarshaledTypes =>
            this._marshaledTypes;

        public override int NativeSizeWithoutPointers =>
            0;
    }
}

