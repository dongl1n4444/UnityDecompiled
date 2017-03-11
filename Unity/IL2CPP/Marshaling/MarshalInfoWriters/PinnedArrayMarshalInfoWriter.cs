namespace Unity.IL2CPP.Marshaling.MarshalInfoWriters
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using Unity.IL2CPP;
    using Unity.IL2CPP.Marshaling;

    internal class PinnedArrayMarshalInfoWriter : ArrayMarshalInfoWriter
    {
        public PinnedArrayMarshalInfoWriter(ArrayType arrayType, MarshalType marshalType, MarshalInfo marshalInfo) : base(arrayType, marshalType, marshalInfo)
        {
        }

        public override void WriteMarshalCleanupEmptyVariable(CppCodeWriter writer, string variableName, IRuntimeMetadataAccess metadataAccess, string managedVariableName = null)
        {
            object[] args = new object[] { variableName };
            writer.WriteLine("il2cpp_codegen_marshal_free({0});", args);
            object[] objArray2 = new object[] { variableName, DefaultMarshalInfoWriter.Naming.Null };
            writer.WriteLine("{0} = {1};", objArray2);
        }

        public override void WriteMarshalCleanupReturnValue(CppCodeWriter writer, string variableName, IRuntimeMetadataAccess metadataAccess)
        {
            object[] args = new object[] { variableName };
            writer.WriteLine("il2cpp_codegen_marshal_free({0});", args);
            object[] objArray2 = new object[] { variableName, DefaultMarshalInfoWriter.Naming.Null };
            writer.WriteLine("{0} = {1};", objArray2);
        }

        public override void WriteMarshalCleanupVariable(CppCodeWriter writer, string variableName, IRuntimeMetadataAccess metadataAccess, string managedVariableName = null)
        {
        }

        public override void WriteMarshalOutParameterFromNative(CppCodeWriter writer, string variableName, ManagedMarshalValue destinationVariable, IList<MarshaledParameter> methodParameters, bool returnValue, bool forNativeWrapperOfManagedMethod, bool isIn, IRuntimeMetadataAccess metadataAccess)
        {
            <WriteMarshalOutParameterFromNative>c__AnonStorey3 storey = new <WriteMarshalOutParameterFromNative>c__AnonStorey3 {
                destinationVariable = destinationVariable,
                variableName = variableName,
                $this = this
            };
            if (!isIn)
            {
                object[] args = new object[] { storey.variableName, DefaultMarshalInfoWriter.Naming.Null };
                writer.WriteLine("if ({0} != {1})", args);
                using (new BlockWriter(writer, false))
                {
                    base.WriteMarshalFromNativeLoop(writer, storey.variableName, storey.destinationVariable, methodParameters, returnValue, forNativeWrapperOfManagedMethod, metadataAccess, new Func<CppCodeWriter, string>(storey.<>m__0));
                }
            }
        }

        public override void WriteMarshalOutParameterToNative(CppCodeWriter writer, ManagedMarshalValue sourceVariable, string destinationVariable, string managedVariableName, IList<MarshaledParameter> methodParameters, IRuntimeMetadataAccess metadataAccess)
        {
            <WriteMarshalOutParameterToNative>c__AnonStorey0 storey = new <WriteMarshalOutParameterToNative>c__AnonStorey0 {
                sourceVariable = sourceVariable,
                destinationVariable = destinationVariable,
                $this = this
            };
            object[] args = new object[] { storey.sourceVariable.Load(), DefaultMarshalInfoWriter.Naming.Null };
            writer.WriteLine("if ({0} != {1})", args);
            using (new BlockWriter(writer, false))
            {
                base.WriteMarshalToNativeLoop(writer, storey.sourceVariable, storey.destinationVariable, managedVariableName, metadataAccess, new Func<CppCodeWriter, string>(storey.<>m__0));
            }
        }

        public override string WriteMarshalReturnValueToNative(CppCodeWriter writer, ManagedMarshalValue sourceVariable, IRuntimeMetadataAccess metadataAccess)
        {
            string variableName = $"_{sourceVariable.GetNiceName()}_marshaled";
            this.WriteNativeVariableDeclarationOfType(writer, variableName);
            object[] args = new object[] { sourceVariable.Load(), DefaultMarshalInfoWriter.Naming.Null };
            writer.WriteLine("if ({0} != {1})", args);
            using (new BlockWriter(writer, false))
            {
                <WriteMarshalReturnValueToNative>c__AnonStorey1 storey = new <WriteMarshalReturnValueToNative>c__AnonStorey1 {
                    arraySizeVariable = base.WriteArraySizeFromManagedArray(writer, sourceVariable, variableName)
                };
                base.AllocateAndStoreNativeArray(writer, variableName, storey.arraySizeVariable);
                base.WriteMarshalToNativeLoop(writer, sourceVariable, variableName, null, metadataAccess, new Func<CppCodeWriter, string>(storey.<>m__0));
            }
            return variableName;
        }

        public override void WriteMarshalVariableFromNative(CppCodeWriter writer, string variableName, ManagedMarshalValue destinationVariable, IList<MarshaledParameter> methodParameters, bool returnValue, bool forNativeWrapperOfManagedMethod, IRuntimeMetadataAccess metadataAccess)
        {
            object[] args = new object[] { variableName, DefaultMarshalInfoWriter.Naming.Null };
            writer.WriteLine("if ({0} != {1})", args);
            using (new BlockWriter(writer, false))
            {
                <WriteMarshalVariableFromNative>c__AnonStorey2 storey = new <WriteMarshalVariableFromNative>c__AnonStorey2 {
                    arraySize = base.MarshaledArraySizeFor(variableName, methodParameters)
                };
                base.AllocateAndStoreManagedArray(writer, destinationVariable, metadataAccess, storey.arraySize);
                base.WriteMarshalFromNativeLoop(writer, variableName, destinationVariable, methodParameters, returnValue, forNativeWrapperOfManagedMethod, metadataAccess, new Func<CppCodeWriter, string>(storey.<>m__0));
            }
        }

        public override void WriteMarshalVariableToNative(CppCodeWriter writer, ManagedMarshalValue sourceVariable, string destinationVariable, string managedVariableName, IRuntimeMetadataAccess metadataAccess)
        {
            object[] args = new object[] { sourceVariable.Load(), DefaultMarshalInfoWriter.Naming.Null };
            writer.WriteLine("if ({0} != {1})", args);
            using (new BlockWriter(writer, false))
            {
                if (base._arraySizeSelection == ArrayMarshalInfoWriter.ArraySizeOptions.UseFirstMarshaledType)
                {
                    object[] objArray2 = new object[] { destinationVariable, this.MarshaledTypes[0].VariableName, sourceVariable.Load() };
                    writer.WriteLine("{0}{1} = static_cast<uint32_t>(({2})->max_length);", objArray2);
                }
                object[] objArray3 = new object[] { destinationVariable, base._arrayMarshaledTypeName, sourceVariable.Load(), DefaultMarshalInfoWriter.Naming.ForArrayItemAddressGetter(false) };
                writer.WriteLine("{0} = reinterpret_cast<{1}>(({2})->{3}(0));", objArray3);
            }
        }

        [CompilerGenerated]
        private sealed class <WriteMarshalOutParameterFromNative>c__AnonStorey3
        {
            internal PinnedArrayMarshalInfoWriter $this;
            internal ManagedMarshalValue destinationVariable;
            internal string variableName;

            internal string <>m__0(CppCodeWriter bodyWriter) => 
                this.$this.WriteArraySizeFromManagedArray(bodyWriter, this.destinationVariable, this.variableName);
        }

        [CompilerGenerated]
        private sealed class <WriteMarshalOutParameterToNative>c__AnonStorey0
        {
            internal PinnedArrayMarshalInfoWriter $this;
            internal string destinationVariable;
            internal ManagedMarshalValue sourceVariable;

            internal string <>m__0(CppCodeWriter bodyWriter) => 
                this.$this.WriteArraySizeFromManagedArray(bodyWriter, this.sourceVariable, this.destinationVariable);
        }

        [CompilerGenerated]
        private sealed class <WriteMarshalReturnValueToNative>c__AnonStorey1
        {
            internal string arraySizeVariable;

            internal string <>m__0(CppCodeWriter bodyWriter) => 
                this.arraySizeVariable;
        }

        [CompilerGenerated]
        private sealed class <WriteMarshalVariableFromNative>c__AnonStorey2
        {
            internal string arraySize;

            internal string <>m__0(CppCodeWriter bodyWriter) => 
                this.arraySize;
        }
    }
}

