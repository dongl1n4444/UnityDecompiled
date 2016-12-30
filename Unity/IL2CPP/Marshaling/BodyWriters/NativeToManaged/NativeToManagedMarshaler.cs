namespace Unity.IL2CPP.Marshaling.BodyWriters.NativeToManaged
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using Unity.IL2CPP;
    using Unity.IL2CPP.ILPreProcessor;
    using Unity.IL2CPP.Marshaling;
    using Unity.IL2CPP.Marshaling.BodyWriters;
    using Unity.IL2CPP.Marshaling.MarshalInfoWriters;

    internal class NativeToManagedMarshaler : InteropMarshaler
    {
        [CompilerGenerated]
        private static Action<CppCodeWriter> <>f__am$cache0;
        [CompilerGenerated]
        private static Action<CppCodeWriter> <>f__am$cache1;
        [CompilerGenerated]
        private static Action<CppCodeWriter> <>f__am$cache2;
        [CompilerGenerated]
        private static Action<CppCodeWriter> <>f__am$cache3;
        [CompilerGenerated]
        private static Action<CppCodeWriter> <>f__am$cache4;

        public NativeToManagedMarshaler(Unity.IL2CPP.ILPreProcessor.TypeResolver typeResolver, MarshalType marshalType, bool useUnicodeCharset) : base(typeResolver, marshalType, useUnicodeCharset)
        {
        }

        public override bool CanMarshalAsInputParameter(MarshaledParameter parameter) => 
            base.MarshalInfoWriterFor(parameter).CanMarshalTypeFromNative();

        public override bool CanMarshalAsOutputParameter(MethodReturnType methodReturnType) => 
            base.MarshalInfoWriterFor(methodReturnType).CanMarshalTypeToNative();

        public override bool CanMarshalAsOutputParameter(MarshaledParameter parameter) => 
            base.MarshalInfoWriterFor(parameter).CanMarshalTypeToNative();

        public override string GetPrettyCalleeName() => 
            "Managed method";

        public override void WriteMarshalCleanupEmptyParameter(CppCodeWriter writer, string valueName, MarshaledParameter parameter, IRuntimeMetadataAccess metadataAccess)
        {
        }

        public override void WriteMarshalCleanupParameter(CppCodeWriter writer, string valueName, MarshaledParameter parameter, IRuntimeMetadataAccess metadataAccess)
        {
        }

        public override void WriteMarshalCleanupReturnValue(CppCodeWriter writer, MethodReturnType methodReturnType, IRuntimeMetadataAccess metadataAccess)
        {
        }

        public override string WriteMarshalEmptyInputParameter(CppCodeWriter writer, MarshaledParameter parameter, IList<MarshaledParameter> parameters, IRuntimeMetadataAccess metadataAccess)
        {
            <WriteMarshalEmptyInputParameter>c__AnonStorey0 storey = new <WriteMarshalEmptyInputParameter>c__AnonStorey0 {
                parameter = parameter,
                parameters = parameters,
                metadataAccess = metadataAccess,
                $this = this
            };
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = delegate (CppCodeWriter bodyWriter) {
                    bodyWriter.WriteLine();
                };
            }
            return writer.WriteIfNotEmpty<string>(new Action<CppCodeWriter>(storey.<>m__0), new Func<CppCodeWriter, string>(storey.<>m__1), <>f__am$cache0);
        }

        public override string WriteMarshalInputParameter(CppCodeWriter writer, MarshaledParameter parameter, IList<MarshaledParameter> parameters, IRuntimeMetadataAccess metadataAccess)
        {
            <WriteMarshalInputParameter>c__AnonStorey1 storey = new <WriteMarshalInputParameter>c__AnonStorey1 {
                parameter = parameter,
                parameters = parameters,
                metadataAccess = metadataAccess,
                $this = this
            };
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = delegate (CppCodeWriter bodyWriter) {
                    bodyWriter.WriteLine();
                };
            }
            return writer.WriteIfNotEmpty<string>(new Action<CppCodeWriter>(storey.<>m__0), new Func<CppCodeWriter, string>(storey.<>m__1), <>f__am$cache1);
        }

        public override void WriteMarshalOutputParameter(CppCodeWriter writer, string valueName, MarshaledParameter parameter, IList<MarshaledParameter> parameters, IRuntimeMetadataAccess metadataAccess)
        {
            <WriteMarshalOutputParameter>c__AnonStorey2 storey = new <WriteMarshalOutputParameter>c__AnonStorey2 {
                parameter = parameter,
                valueName = valueName,
                parameters = parameters,
                metadataAccess = metadataAccess,
                $this = this
            };
            if (storey.valueName != storey.parameter.NameInGeneratedCode)
            {
                if (<>f__am$cache2 == null)
                {
                    <>f__am$cache2 = bodyWriter => bodyWriter.WriteLine();
                }
                writer.WriteIfNotEmpty(new Action<CppCodeWriter>(storey.<>m__0), new Action<CppCodeWriter>(storey.<>m__1), <>f__am$cache2);
            }
        }

        public override string WriteMarshalReturnValue(CppCodeWriter writer, MethodReturnType methodReturnType, IList<MarshaledParameter> parameters, IRuntimeMetadataAccess metadataAccess)
        {
            <WriteMarshalReturnValue>c__AnonStorey3 storey = new <WriteMarshalReturnValue>c__AnonStorey3 {
                methodReturnType = methodReturnType,
                metadataAccess = metadataAccess,
                $this = this
            };
            if (<>f__am$cache3 == null)
            {
                <>f__am$cache3 = delegate (CppCodeWriter bodyWriter) {
                    bodyWriter.WriteCommentedLine("Marshaling of return value back from managed representation");
                };
            }
            if (<>f__am$cache4 == null)
            {
                <>f__am$cache4 = delegate (CppCodeWriter bodyWriter) {
                    bodyWriter.WriteLine();
                };
            }
            return writer.WriteIfNotEmpty<string>(<>f__am$cache3, new Func<CppCodeWriter, string>(storey.<>m__0), <>f__am$cache4);
        }

        [CompilerGenerated]
        private sealed class <WriteMarshalEmptyInputParameter>c__AnonStorey0
        {
            internal NativeToManagedMarshaler $this;
            internal IRuntimeMetadataAccess metadataAccess;
            internal MarshaledParameter parameter;
            internal IList<MarshaledParameter> parameters;

            internal void <>m__0(CppCodeWriter bodyWriter)
            {
                object[] parameters = new object[] { this.parameter.NameInGeneratedCode };
                bodyWriter.WriteCommentedLine("Marshaling of parameter '{0}' to managed representation", parameters);
            }

            internal string <>m__1(CppCodeWriter bodyWriter)
            {
                DefaultMarshalInfoWriter writer = this.$this.MarshalInfoWriterFor(this.parameter);
                return writer.WriteMarshalEmptyVariableFromNative(bodyWriter, writer.UndecorateVariable(this.parameter.NameInGeneratedCode), this.parameters, this.metadataAccess);
            }
        }

        [CompilerGenerated]
        private sealed class <WriteMarshalInputParameter>c__AnonStorey1
        {
            internal NativeToManagedMarshaler $this;
            internal IRuntimeMetadataAccess metadataAccess;
            internal MarshaledParameter parameter;
            internal IList<MarshaledParameter> parameters;

            internal void <>m__0(CppCodeWriter bodyWriter)
            {
                object[] parameters = new object[] { this.parameter.NameInGeneratedCode };
                bodyWriter.WriteCommentedLine("Marshaling of parameter '{0}' to managed representation", parameters);
            }

            internal string <>m__1(CppCodeWriter bodyWriter)
            {
                DefaultMarshalInfoWriter writer = this.$this.MarshalInfoWriterFor(this.parameter);
                return writer.WriteMarshalVariableFromNative(bodyWriter, writer.UndecorateVariable(this.parameter.NameInGeneratedCode), this.parameters, false, true, this.metadataAccess);
            }
        }

        [CompilerGenerated]
        private sealed class <WriteMarshalOutputParameter>c__AnonStorey2
        {
            internal NativeToManagedMarshaler $this;
            internal IRuntimeMetadataAccess metadataAccess;
            internal MarshaledParameter parameter;
            internal IList<MarshaledParameter> parameters;
            internal string valueName;

            internal void <>m__0(CppCodeWriter bodyWriter)
            {
                object[] parameters = new object[] { this.parameter.NameInGeneratedCode };
                bodyWriter.WriteCommentedLine("Marshaling of parameter '{0}' back from managed representation", parameters);
            }

            internal void <>m__1(CppCodeWriter bodyWriter)
            {
                this.$this.MarshalInfoWriterFor(this.parameter).WriteMarshalOutParameterToNative(bodyWriter, new ManagedMarshalValue(this.valueName), this.parameter.NameInGeneratedCode, this.parameter.Name, this.parameters, this.metadataAccess);
            }
        }

        [CompilerGenerated]
        private sealed class <WriteMarshalReturnValue>c__AnonStorey3
        {
            internal NativeToManagedMarshaler $this;
            internal IRuntimeMetadataAccess metadataAccess;
            internal MethodReturnType methodReturnType;

            internal string <>m__0(CppCodeWriter bodyWriter)
            {
                DefaultMarshalInfoWriter writer = this.$this.MarshalInfoWriterFor(this.methodReturnType);
                string variableName = InteropMarshaler.Naming.ForInteropReturnValue();
                string objectVariableName = writer.UndecorateVariable(variableName);
                return writer.WriteMarshalReturnValueToNative(bodyWriter, new ManagedMarshalValue(objectVariableName), this.metadataAccess);
            }
        }
    }
}

