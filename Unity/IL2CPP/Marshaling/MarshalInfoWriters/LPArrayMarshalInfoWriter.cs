namespace Unity.IL2CPP.Marshaling.MarshalInfoWriters
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using Unity.IL2CPP;
    using Unity.IL2CPP.Marshaling;

    internal sealed class LPArrayMarshalInfoWriter : ArrayMarshalInfoWriter
    {
        public LPArrayMarshalInfoWriter(ArrayType arrayType, MarshalType marshalType, MarshalInfo marshalInfo) : base(arrayType, marshalType, marshalInfo)
        {
        }

        public override void WriteMarshalCleanupVariable(CppCodeWriter writer, string variableName, IRuntimeMetadataAccess metadataAccess, string managedVariableName = null)
        {
            <WriteMarshalCleanupVariable>c__AnonStorey3 storey = new <WriteMarshalCleanupVariable>c__AnonStorey3 {
                variableName = variableName,
                managedVariableName = managedVariableName,
                $this = this
            };
            object[] args = new object[] { storey.variableName, DefaultMarshalInfoWriter.Naming.Null };
            writer.WriteLine("if ({0} != {1})", args);
            using (new BlockWriter(writer, false))
            {
                base.WriteCleanupLoop(writer, storey.variableName, metadataAccess, new Func<CppCodeWriter, string>(storey.<>m__0));
                object[] objArray2 = new object[] { storey.variableName };
                writer.WriteLine("il2cpp_codegen_marshal_free({0});", objArray2);
                object[] objArray3 = new object[] { storey.variableName, DefaultMarshalInfoWriter.Naming.Null };
                writer.WriteLine("{0} = {1};", objArray3);
            }
        }

        public override void WriteMarshalOutParameterFromNative(CppCodeWriter writer, string variableName, ManagedMarshalValue destinationVariable, IList<MarshaledParameter> methodParameters, bool returnValue, bool forNativeWrapperOfManagedMethod, IRuntimeMetadataAccess metadataAccess)
        {
            <WriteMarshalOutParameterFromNative>c__AnonStorey2 storey = new <WriteMarshalOutParameterFromNative>c__AnonStorey2 {
                destinationVariable = destinationVariable,
                variableName = variableName,
                $this = this
            };
            object[] args = new object[] { storey.variableName, DefaultMarshalInfoWriter.Naming.Null };
            writer.WriteLine("if ({0} != {1})", args);
            using (new BlockWriter(writer, false))
            {
                base.WriteMarshalFromNativeLoop(writer, storey.variableName, storey.destinationVariable, methodParameters, returnValue, forNativeWrapperOfManagedMethod, metadataAccess, new Func<CppCodeWriter, string>(storey.<>m__0));
            }
        }

        public override void WriteMarshalVariableFromNative(CppCodeWriter writer, string variableName, ManagedMarshalValue destinationVariable, IList<MarshaledParameter> methodParameters, bool returnValue, bool forNativeWrapperOfManagedMethod, IRuntimeMetadataAccess metadataAccess)
        {
            <WriteMarshalVariableFromNative>c__AnonStorey1 storey = new <WriteMarshalVariableFromNative>c__AnonStorey1 {
                writer = writer,
                destinationVariable = destinationVariable
            };
            object[] args = new object[] { variableName, DefaultMarshalInfoWriter.Naming.Null };
            storey.writer.WriteLine("if ({0} != {1})", args);
            using (new BlockWriter(storey.writer, false))
            {
                object[] objArray2 = new object[] { storey.destinationVariable.Load(), DefaultMarshalInfoWriter.Naming.Null };
                storey.writer.WriteLine("if ({0} == {1})", objArray2);
                using (new BlockWriter(storey.writer, false))
                {
                    base.AllocateAndStoreManagedArray(storey.writer, storey.destinationVariable, metadataAccess, base.MarshaledArraySizeFor(variableName, methodParameters));
                }
                base.WriteMarshalFromNativeLoop(storey.writer, variableName, storey.destinationVariable, methodParameters, returnValue, forNativeWrapperOfManagedMethod, metadataAccess, new Func<CppCodeWriter, string>(storey.<>m__0));
            }
        }

        public override void WriteMarshalVariableToNative(CppCodeWriter writer, ManagedMarshalValue sourceVariable, string destinationVariable, string managedVariableName, IRuntimeMetadataAccess metadataAccess)
        {
            object[] args = new object[] { sourceVariable.Load(), DefaultMarshalInfoWriter.Naming.Null };
            writer.WriteLine("if ({0} != {1})", args);
            using (new BlockWriter(writer, false))
            {
                <WriteMarshalVariableToNative>c__AnonStorey0 storey = new <WriteMarshalVariableToNative>c__AnonStorey0 {
                    arraySizeVariable = base.WriteArraySizeFromManagedArray(writer, sourceVariable, destinationVariable)
                };
                base.AllocateAndStoreNativeArray(writer, destinationVariable, storey.arraySizeVariable);
                base.WriteMarshalToNativeLoop(writer, sourceVariable, destinationVariable, managedVariableName, metadataAccess, new Func<CppCodeWriter, string>(storey.<>m__0));
            }
            writer.WriteLine("else");
            using (new BlockWriter(writer, false))
            {
                base.WriteAssignNullArray(writer, destinationVariable);
            }
        }

        [CompilerGenerated]
        private sealed class <WriteMarshalCleanupVariable>c__AnonStorey3
        {
            internal LPArrayMarshalInfoWriter $this;
            internal string managedVariableName;
            internal string variableName;

            internal string <>m__0(CppCodeWriter bodyWriter)
            {
                string str = DefaultMarshalInfoWriter.CleanVariableName(this.variableName);
                string str2 = $"{str}_CleanupLoopCount";
                string str3 = (this.managedVariableName != null) ? string.Format("({0} != {1}) ? ({0})->max_length : 0", this.managedVariableName, DefaultMarshalInfoWriter.Naming.Null) : $"{this.$this._arraySize}";
                object[] args = new object[] { str2, str3 };
                bodyWriter.WriteLine("const int32_t {0} = {1};", args);
                return str2;
            }
        }

        [CompilerGenerated]
        private sealed class <WriteMarshalOutParameterFromNative>c__AnonStorey2
        {
            internal LPArrayMarshalInfoWriter $this;
            internal ManagedMarshalValue destinationVariable;
            internal string variableName;

            internal string <>m__0(CppCodeWriter bodyWriter) => 
                this.$this.WriteArraySizeFromManagedArray(bodyWriter, this.destinationVariable, this.variableName);
        }

        [CompilerGenerated]
        private sealed class <WriteMarshalVariableFromNative>c__AnonStorey1
        {
            internal ManagedMarshalValue destinationVariable;
            internal CppCodeWriter writer;

            internal string <>m__0(CppCodeWriter bodyWriter)
            {
                object[] args = new object[] { "_arrayLength", this.destinationVariable.Load() };
                this.writer.WriteLine("int32_t {0} = ({1})->max_length;", args);
                return "_arrayLength";
            }
        }

        [CompilerGenerated]
        private sealed class <WriteMarshalVariableToNative>c__AnonStorey0
        {
            internal string arraySizeVariable;

            internal string <>m__0(CppCodeWriter bodyWriter) => 
                this.arraySizeVariable;
        }
    }
}

