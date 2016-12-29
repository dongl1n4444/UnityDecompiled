namespace Unity.IL2CPP.Marshaling.MarshalInfoWriters
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using Unity.IL2CPP;
    using Unity.IL2CPP.Marshaling;

    internal sealed class FixedArrayMarshalInfoWriter : ArrayMarshalInfoWriter
    {
        public FixedArrayMarshalInfoWriter(ArrayType arrayType, MarshalType marshalType, MarshalInfo marshalInfo) : base(arrayType, marshalType, marshalInfo)
        {
        }

        public override void WriteFieldDeclaration(CppCodeWriter writer, FieldReference field, string fieldNameSuffix = null)
        {
            string str = DefaultMarshalInfoWriter.Naming.ForField(field) + fieldNameSuffix;
            object[] args = new object[] { base._elementTypeMarshalInfoWriter.MarshaledTypes[0].DecoratedName, str, base._arraySize };
            writer.WriteLine("{0} {1}[{2}];", args);
        }

        public override void WriteIncludesForFieldDeclaration(CppCodeWriter writer)
        {
            base.WriteIncludesForFieldDeclaration(writer);
            writer.AddIncludeForTypeDefinition(base._elementType);
        }

        public override void WriteMarshalCleanupVariable(CppCodeWriter writer, string variableName, IRuntimeMetadataAccess metadataAccess, string managedVariableName = null)
        {
            base.WriteCleanupLoop(writer, variableName, metadataAccess, new Func<CppCodeWriter, string>(this, (IntPtr) this.<WriteMarshalCleanupVariable>m__1));
        }

        public override void WriteMarshalOutParameterFromNative(CppCodeWriter writer, string variableName, ManagedMarshalValue destinationVariable, IList<MarshaledParameter> methodParameters, bool returnValue, bool forNativeWrapperOfManagedMethod, IRuntimeMetadataAccess metadataAccess)
        {
            <WriteMarshalOutParameterFromNative>c__AnonStorey1 storey = new <WriteMarshalOutParameterFromNative>c__AnonStorey1 {
                variableName = variableName,
                methodParameters = methodParameters,
                $this = this
            };
            base.WriteMarshalFromNativeLoop(writer, storey.variableName, destinationVariable, storey.methodParameters, returnValue, forNativeWrapperOfManagedMethod, metadataAccess, new Func<CppCodeWriter, string>(storey, (IntPtr) this.<>m__0));
        }

        public override void WriteMarshalVariableFromNative(CppCodeWriter writer, string variableName, ManagedMarshalValue destinationVariable, IList<MarshaledParameter> methodParameters, bool returnValue, bool forNativeWrapperOfManagedMethod, IRuntimeMetadataAccess metadataAccess)
        {
            <WriteMarshalVariableFromNative>c__AnonStorey0 storey = new <WriteMarshalVariableFromNative>c__AnonStorey0 {
                arraySize = base.MarshaledArraySizeFor(variableName, methodParameters)
            };
            base.AllocateAndStoreManagedArray(writer, destinationVariable, metadataAccess, storey.arraySize);
            base.WriteMarshalFromNativeLoop(writer, variableName, destinationVariable, methodParameters, returnValue, forNativeWrapperOfManagedMethod, metadataAccess, new Func<CppCodeWriter, string>(storey, (IntPtr) this.<>m__0));
        }

        public override void WriteMarshalVariableToNative(CppCodeWriter writer, ManagedMarshalValue sourceVariable, string destinationVariable, string managedVariableName, IRuntimeMetadataAccess metadataAccess)
        {
            object[] args = new object[] { sourceVariable.Load(), DefaultMarshalInfoWriter.Naming.Null };
            writer.WriteLine("if ({0} != {1})", args);
            using (new BlockWriter(writer, false))
            {
                object[] objArray2 = new object[] { base._arraySize, sourceVariable.Load() };
                writer.WriteLine("if ({0} > ({1})->max_length)", objArray2);
                using (new BlockWriter(writer, false))
                {
                    writer.WriteStatement(Emit.RaiseManagedException("il2cpp_codegen_get_argument_exception(\"\", \"Type could not be marshaled because the length of an embedded array instance does not match the declared length in the layout.\")"));
                }
                writer.WriteLine();
                base.WriteMarshalToNativeLoop(writer, sourceVariable, destinationVariable, managedVariableName, metadataAccess, new Func<CppCodeWriter, string>(this, (IntPtr) this.<WriteMarshalVariableToNative>m__0));
            }
        }

        [CompilerGenerated]
        private sealed class <WriteMarshalOutParameterFromNative>c__AnonStorey1
        {
            internal FixedArrayMarshalInfoWriter $this;
            internal IList<MarshaledParameter> methodParameters;
            internal string variableName;

            internal string <>m__0(CppCodeWriter bodyWriter) => 
                this.$this.MarshaledArraySizeFor(this.variableName, this.methodParameters);
        }

        [CompilerGenerated]
        private sealed class <WriteMarshalVariableFromNative>c__AnonStorey0
        {
            internal string arraySize;

            internal string <>m__0(CppCodeWriter bodyWriter) => 
                this.arraySize;
        }
    }
}

