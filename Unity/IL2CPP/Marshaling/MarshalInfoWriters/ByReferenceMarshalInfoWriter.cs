namespace Unity.IL2CPP.Marshaling.MarshalInfoWriters
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using Unity.IL2CPP;
    using Unity.IL2CPP.Marshaling;

    internal sealed class ByReferenceMarshalInfoWriter : MarshalableMarshalInfoWriter
    {
        private readonly TypeReference _elementType;
        private readonly DefaultMarshalInfoWriter _elementTypeMarshalInfoWriter;
        private readonly MarshaledType[] _marshaledTypes;
        [CompilerGenerated]
        private static Func<MarshaledType, MarshaledType> <>f__am$cache0;

        public ByReferenceMarshalInfoWriter(ByReferenceType type, MarshalType marshalType, MarshalInfo marshalInfo) : base(type)
        {
            this._elementType = type.ElementType;
            this._elementTypeMarshalInfoWriter = MarshalDataCollector.MarshalInfoWriterFor(type.ElementType, marshalType, marshalInfo, false, true, false, null);
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = new Func<MarshaledType, MarshaledType>(ByReferenceMarshalInfoWriter.<ByReferenceMarshalInfoWriter>m__0);
            }
            this._marshaledTypes = this._elementTypeMarshalInfoWriter.MarshaledTypes.Select<MarshaledType, MarshaledType>(<>f__am$cache0).ToArray<MarshaledType>();
        }

        [CompilerGenerated]
        private static MarshaledType <ByReferenceMarshalInfoWriter>m__0(MarshaledType t) => 
            new MarshaledType(t.Name + "*", t.DecoratedName + "*", t.VariableName);

        public override bool CanMarshalTypeFromNative() => 
            this._elementTypeMarshalInfoWriter.CanMarshalTypeFromNative();

        public override bool CanMarshalTypeToNative() => 
            this._elementTypeMarshalInfoWriter.CanMarshalTypeToNative();

        public override string DecorateVariable(string unmarshaledParameterName, string marshaledVariableName) => 
            this._elementTypeMarshalInfoWriter.DecorateVariable(unmarshaledParameterName, marshaledVariableName);

        public override string GetMarshalingException() => 
            this._elementTypeMarshalInfoWriter.GetMarshalingException();

        public override string UndecorateVariable(string variableName) => 
            this._elementTypeMarshalInfoWriter.UndecorateVariable(variableName);

        public override void WriteDeclareAndAllocateObject(CppCodeWriter writer, string unmarshaledVariableName, string marshaledVariableName, IRuntimeMetadataAccess metadataAccess)
        {
            string str = unmarshaledVariableName + "_dereferenced";
            this._elementTypeMarshalInfoWriter.WriteDeclareAndAllocateObject(writer, str, DefaultMarshalInfoWriter.Naming.Dereference(marshaledVariableName), metadataAccess);
            object[] args = new object[] { DefaultMarshalInfoWriter.Naming.ForVariable(base._typeRef), unmarshaledVariableName, str };
            writer.WriteLine("{0} {1} = &{2};", args);
        }

        public override void WriteIncludesForFieldDeclaration(CppCodeWriter writer)
        {
            this._elementTypeMarshalInfoWriter.WriteMarshaledTypeForwardDeclaration(writer);
        }

        public override void WriteIncludesForMarshaling(CppCodeWriter writer)
        {
            this._elementTypeMarshalInfoWriter.WriteIncludesForMarshaling(writer);
        }

        public override void WriteMarshalCleanupEmptyVariable(CppCodeWriter writer, string variableName, IRuntimeMetadataAccess metadataAccess, string managedVariableName)
        {
            this._elementTypeMarshalInfoWriter.WriteMarshalCleanupEmptyVariable(writer, DefaultMarshalInfoWriter.Naming.Dereference(variableName), metadataAccess, DefaultMarshalInfoWriter.Naming.Dereference(managedVariableName));
        }

        public override void WriteMarshalCleanupVariable(CppCodeWriter writer, string variableName, IRuntimeMetadataAccess metadataAccess, string managedVariableName = null)
        {
            this._elementTypeMarshalInfoWriter.WriteMarshalCleanupVariable(writer, DefaultMarshalInfoWriter.Naming.Dereference(variableName), metadataAccess, DefaultMarshalInfoWriter.Naming.Dereference(managedVariableName));
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
            string str = $"_{variableName.GetNiceName()}_marshaled";
            if ((((((ByReferenceType) base._typeRef).ElementType.MetadataType == MetadataType.Class) && !(this._elementTypeMarshalInfoWriter is UnmarshalableMarshalInfoWriter)) && (!(this._elementTypeMarshalInfoWriter is SafeHandleMarshalInfoWriter) && !(this._elementTypeMarshalInfoWriter is ComObjectMarshalInfoWriter))) && ((!(this._elementTypeMarshalInfoWriter is DelegateMarshalInfoWriter) && !(this._elementTypeMarshalInfoWriter is WindowsRuntimeDelegateMarshalInfoWriter)) && !(this._elementTypeMarshalInfoWriter is StringMarshalInfoWriter)))
            {
                this.WriteNativeVariableDeclarationOfType(writer, str);
                return str;
            }
            string str2 = $"_{variableName.GetNiceName()}_empty";
            this._elementTypeMarshalInfoWriter.WriteNativeVariableDeclarationOfType(writer, str2);
            foreach (MarshaledType type in this.MarshaledTypes)
            {
                object[] args = new object[] { type.Name, str + type.VariableName, str2 + type.VariableName };
                writer.WriteLine("{0} {1} = &{2};", args);
            }
            return str;
        }

        public override void WriteMarshalOutParameterToNative(CppCodeWriter writer, ManagedMarshalValue sourceVariable, string destinationVariable, string managedVariableName, IList<MarshaledParameter> methodParameters, IRuntimeMetadataAccess metadataAccess)
        {
            this._elementTypeMarshalInfoWriter.WriteMarshalVariableToNative(writer, sourceVariable.Dereferenced, DefaultMarshalInfoWriter.Naming.Dereference(this.UndecorateVariable(destinationVariable)), managedVariableName, metadataAccess);
        }

        public override string WriteMarshalVariableFromNative(CppCodeWriter writer, string variableName, IList<MarshaledParameter> methodParameters, bool returnValue, bool forNativeWrapperOfManagedMethod, IRuntimeMetadataAccess metadataAccess)
        {
            string unmarshaledVariableName = $"_{DefaultMarshalInfoWriter.CleanVariableName(variableName)}_unmarshaled_dereferenced";
            this._elementTypeMarshalInfoWriter.WriteDeclareAndAllocateObject(writer, unmarshaledVariableName, variableName, metadataAccess);
            this._elementTypeMarshalInfoWriter.WriteMarshalVariableFromNative(writer, DefaultMarshalInfoWriter.Naming.Dereference(variableName), new ManagedMarshalValue(unmarshaledVariableName), methodParameters, returnValue, forNativeWrapperOfManagedMethod, metadataAccess);
            return DefaultMarshalInfoWriter.Naming.AddressOf(unmarshaledVariableName);
        }

        public override void WriteMarshalVariableFromNative(CppCodeWriter writer, string variableName, ManagedMarshalValue destinationVariable, IList<MarshaledParameter> methodParameters, bool returnValue, bool forNativeWrapperOfManagedMethod, IRuntimeMetadataAccess metadataAccess)
        {
            string unmarshaledVariableName = $"_{DefaultMarshalInfoWriter.CleanVariableName(variableName)}_unmarshaled_dereferenced";
            this._elementTypeMarshalInfoWriter.WriteDeclareAndAllocateObject(writer, unmarshaledVariableName, variableName, metadataAccess);
            this._elementTypeMarshalInfoWriter.WriteMarshalVariableFromNative(writer, DefaultMarshalInfoWriter.Naming.Dereference(variableName), new ManagedMarshalValue(unmarshaledVariableName), methodParameters, returnValue, forNativeWrapperOfManagedMethod, metadataAccess);
            writer.WriteLine(destinationVariable.Dereferenced.Store(unmarshaledVariableName));
        }

        public override void WriteMarshalVariableToNative(CppCodeWriter writer, ManagedMarshalValue sourceVariable, string destinationVariable, string managedVariableName, IRuntimeMetadataAccess metadataAccess)
        {
            string variableName = $"{DefaultMarshalInfoWriter.CleanVariableName(destinationVariable)}_dereferenced";
            this._elementTypeMarshalInfoWriter.WriteNativeVariableDeclarationOfType(writer, variableName);
            this._elementTypeMarshalInfoWriter.WriteMarshalVariableToNative(writer, sourceVariable.Dereferenced, variableName, managedVariableName, metadataAccess);
            object[] args = new object[] { destinationVariable, variableName };
            writer.WriteLine("{0} = &{1};", args);
        }

        public override MarshaledType[] MarshaledTypes =>
            this._marshaledTypes;
    }
}

