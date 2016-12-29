namespace Unity.IL2CPP.Marshaling.BodyWriters.ManagedToNative
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

    internal class ManagedToNativeMarshaler : InteropMarshaler
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
        [CompilerGenerated]
        private static Action<CppCodeWriter> <>f__am$cache5;
        [CompilerGenerated]
        private static Action<CppCodeWriter> <>f__am$cache6;
        [CompilerGenerated]
        private static Action<CppCodeWriter> <>f__am$cache7;
        [CompilerGenerated]
        private static Action<CppCodeWriter> <>f__am$cache8;

        public ManagedToNativeMarshaler(Unity.IL2CPP.ILPreProcessor.TypeResolver typeResolver, MarshalType marshalType, bool useUnicodeCharset) : base(typeResolver, marshalType, useUnicodeCharset)
        {
        }

        public override bool CanMarshalAsInputParameter(MarshaledParameter parameter) => 
            base.MarshalInfoWriterFor(parameter).CanMarshalTypeToNative();

        public override bool CanMarshalAsOutputParameter(MethodReturnType methodReturnType) => 
            base.MarshalInfoWriterFor(methodReturnType).CanMarshalTypeFromNative();

        public override bool CanMarshalAsOutputParameter(MarshaledParameter parameter) => 
            base.MarshalInfoWriterFor(parameter).CanMarshalTypeFromNative();

        public override string GetPrettyCalleeName() => 
            "Native function";

        public override void WriteMarshalCleanupEmptyParameter(CppCodeWriter writer, string valueName, MarshaledParameter parameter, IRuntimeMetadataAccess metadataAccess)
        {
            <WriteMarshalCleanupEmptyParameter>c__AnonStorey4 storey = new <WriteMarshalCleanupEmptyParameter>c__AnonStorey4 {
                parameter = parameter,
                valueName = valueName,
                metadataAccess = metadataAccess,
                $this = this
            };
            if (<>f__am$cache5 == null)
            {
                <>f__am$cache5 = bodyWriter => bodyWriter.WriteLine();
            }
            writer.WriteIfNotEmpty(new Action<CppCodeWriter>(storey.<>m__0), new Action<CppCodeWriter>(storey.<>m__1), <>f__am$cache5);
        }

        public override void WriteMarshalCleanupParameter(CppCodeWriter writer, string valueName, MarshaledParameter parameter, IRuntimeMetadataAccess metadataAccess)
        {
            <WriteMarshalCleanupParameter>c__AnonStorey5 storey = new <WriteMarshalCleanupParameter>c__AnonStorey5 {
                parameter = parameter,
                valueName = valueName,
                metadataAccess = metadataAccess,
                $this = this
            };
            if (<>f__am$cache6 == null)
            {
                <>f__am$cache6 = bodyWriter => bodyWriter.WriteLine();
            }
            writer.WriteIfNotEmpty(new Action<CppCodeWriter>(storey.<>m__0), new Action<CppCodeWriter>(storey.<>m__1), <>f__am$cache6);
        }

        public override void WriteMarshalCleanupReturnValue(CppCodeWriter writer, MethodReturnType methodReturnType, IRuntimeMetadataAccess metadataAccess)
        {
            <WriteMarshalCleanupReturnValue>c__AnonStorey6 storey = new <WriteMarshalCleanupReturnValue>c__AnonStorey6 {
                methodReturnType = methodReturnType,
                metadataAccess = metadataAccess,
                $this = this
            };
            if (<>f__am$cache7 == null)
            {
                <>f__am$cache7 = bodyWriter => bodyWriter.WriteCommentedLine("Marshaling cleanup of return value native representation");
            }
            if (<>f__am$cache8 == null)
            {
                <>f__am$cache8 = bodyWriter => bodyWriter.WriteLine();
            }
            writer.WriteIfNotEmpty(<>f__am$cache7, new Action<CppCodeWriter>(storey.<>m__0), <>f__am$cache8);
        }

        public override string WriteMarshalEmptyInputParameter(CppCodeWriter writer, MarshaledParameter parameter, IList<MarshaledParameter> parameters, IRuntimeMetadataAccess metadataAccess)
        {
            <WriteMarshalEmptyInputParameter>c__AnonStorey0 storey = new <WriteMarshalEmptyInputParameter>c__AnonStorey0 {
                parameter = parameter,
                parameters = parameters,
                $this = this
            };
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = delegate (CppCodeWriter bodyWriter) {
                    bodyWriter.WriteLine();
                };
            }
            return writer.WriteIfNotEmpty<string>(new Action<CppCodeWriter>(storey.<>m__0), new Func<CppCodeWriter, string>(storey, (IntPtr) this.<>m__1), <>f__am$cache0);
        }

        public override string WriteMarshalInputParameter(CppCodeWriter writer, MarshaledParameter parameter, IList<MarshaledParameter> parameters, IRuntimeMetadataAccess metadataAccess)
        {
            <WriteMarshalInputParameter>c__AnonStorey1 storey = new <WriteMarshalInputParameter>c__AnonStorey1 {
                parameter = parameter,
                metadataAccess = metadataAccess,
                $this = this
            };
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = delegate (CppCodeWriter bodyWriter) {
                    bodyWriter.WriteLine();
                };
            }
            return writer.WriteIfNotEmpty<string>(new Action<CppCodeWriter>(storey.<>m__0), new Func<CppCodeWriter, string>(storey, (IntPtr) this.<>m__1), <>f__am$cache1);
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
                parameters = parameters,
                metadataAccess = metadataAccess,
                $this = this
            };
            if (<>f__am$cache3 == null)
            {
                <>f__am$cache3 = delegate (CppCodeWriter bodyWriter) {
                    bodyWriter.WriteCommentedLine("Marshaling of return value back from native representation");
                };
            }
            if (<>f__am$cache4 == null)
            {
                <>f__am$cache4 = delegate (CppCodeWriter bodyWriter) {
                    bodyWriter.WriteLine();
                };
            }
            return writer.WriteIfNotEmpty<string>(<>f__am$cache3, new Func<CppCodeWriter, string>(storey, (IntPtr) this.<>m__0), <>f__am$cache4);
        }

        [CompilerGenerated]
        private sealed class <WriteMarshalCleanupEmptyParameter>c__AnonStorey4
        {
            internal ManagedToNativeMarshaler $this;
            internal IRuntimeMetadataAccess metadataAccess;
            internal MarshaledParameter parameter;
            internal string valueName;

            internal void <>m__0(CppCodeWriter bodyWriter)
            {
                object[] parameters = new object[] { this.parameter.NameInGeneratedCode };
                bodyWriter.WriteCommentedLine("Marshaling cleanup of parameter '{0}' native representation", parameters);
            }

            internal void <>m__1(CppCodeWriter bodyWriter)
            {
                this.$this.MarshalInfoWriterFor(this.parameter).WriteMarshalCleanupEmptyVariable(bodyWriter, this.valueName, this.metadataAccess, this.parameter.NameInGeneratedCode);
            }
        }

        [CompilerGenerated]
        private sealed class <WriteMarshalCleanupParameter>c__AnonStorey5
        {
            internal ManagedToNativeMarshaler $this;
            internal IRuntimeMetadataAccess metadataAccess;
            internal MarshaledParameter parameter;
            internal string valueName;

            internal void <>m__0(CppCodeWriter bodyWriter)
            {
                object[] parameters = new object[] { this.parameter.NameInGeneratedCode };
                bodyWriter.WriteCommentedLine("Marshaling cleanup of parameter '{0}' native representation", parameters);
            }

            internal void <>m__1(CppCodeWriter bodyWriter)
            {
                this.$this.MarshalInfoWriterFor(this.parameter).WriteMarshalCleanupVariable(bodyWriter, this.valueName, this.metadataAccess, this.parameter.NameInGeneratedCode);
            }
        }

        [CompilerGenerated]
        private sealed class <WriteMarshalCleanupReturnValue>c__AnonStorey6
        {
            internal ManagedToNativeMarshaler $this;
            internal IRuntimeMetadataAccess metadataAccess;
            internal MethodReturnType methodReturnType;

            internal void <>m__0(CppCodeWriter bodyWriter)
            {
                DefaultMarshalInfoWriter writer = this.$this.MarshalInfoWriterFor(this.methodReturnType);
                string variableName = writer.UndecorateVariable(InteropMarshaler.Naming.ForInteropReturnValue());
                writer.WriteMarshalCleanupReturnValue(bodyWriter, variableName, this.metadataAccess);
            }
        }

        [CompilerGenerated]
        private sealed class <WriteMarshalEmptyInputParameter>c__AnonStorey0
        {
            internal ManagedToNativeMarshaler $this;
            internal MarshaledParameter parameter;
            internal IList<MarshaledParameter> parameters;

            internal void <>m__0(CppCodeWriter bodyWriter)
            {
                object[] parameters = new object[] { this.parameter.NameInGeneratedCode };
                bodyWriter.WriteCommentedLine("Marshaling of parameter '{0}' to native representation", parameters);
            }

            internal string <>m__1(CppCodeWriter bodyWriter) => 
                this.$this.MarshalInfoWriterFor(this.parameter).WriteMarshalEmptyVariableToNative(bodyWriter, new ManagedMarshalValue(this.parameter.NameInGeneratedCode), this.parameters);
        }

        [CompilerGenerated]
        private sealed class <WriteMarshalInputParameter>c__AnonStorey1
        {
            internal ManagedToNativeMarshaler $this;
            internal IRuntimeMetadataAccess metadataAccess;
            internal MarshaledParameter parameter;

            internal void <>m__0(CppCodeWriter bodyWriter)
            {
                object[] parameters = new object[] { this.parameter.NameInGeneratedCode };
                bodyWriter.WriteCommentedLine("Marshaling of parameter '{0}' to native representation", parameters);
            }

            internal string <>m__1(CppCodeWriter bodyWriter) => 
                this.$this.MarshalInfoWriterFor(this.parameter).WriteMarshalVariableToNative(bodyWriter, new ManagedMarshalValue(this.parameter.NameInGeneratedCode), this.parameter.Name, this.metadataAccess);
        }

        [CompilerGenerated]
        private sealed class <WriteMarshalOutputParameter>c__AnonStorey2
        {
            internal ManagedToNativeMarshaler $this;
            internal IRuntimeMetadataAccess metadataAccess;
            internal MarshaledParameter parameter;
            internal IList<MarshaledParameter> parameters;
            internal string valueName;

            internal void <>m__0(CppCodeWriter bodyWriter)
            {
                object[] parameters = new object[] { this.parameter.NameInGeneratedCode };
                bodyWriter.WriteCommentedLine("Marshaling of parameter '{0}' back from native representation", parameters);
            }

            internal void <>m__1(CppCodeWriter bodyWriter)
            {
                DefaultMarshalInfoWriter writer = this.$this.MarshalInfoWriterFor(this.parameter);
                ManagedMarshalValue destinationVariable = new ManagedMarshalValue(this.parameter.NameInGeneratedCode);
                if (this.parameter.IsOut)
                {
                    writer.WriteMarshalOutParameterFromNative(bodyWriter, this.valueName, destinationVariable, this.parameters, false, false, this.metadataAccess);
                }
                else
                {
                    writer.WriteMarshalVariableFromNative(bodyWriter, this.valueName, destinationVariable, this.parameters, false, false, this.metadataAccess);
                }
            }
        }

        [CompilerGenerated]
        private sealed class <WriteMarshalReturnValue>c__AnonStorey3
        {
            internal ManagedToNativeMarshaler $this;
            internal IRuntimeMetadataAccess metadataAccess;
            internal MethodReturnType methodReturnType;
            internal IList<MarshaledParameter> parameters;

            internal string <>m__0(CppCodeWriter bodyWriter)
            {
                DefaultMarshalInfoWriter writer = this.$this.MarshalInfoWriterFor(this.methodReturnType);
                string variableName = writer.UndecorateVariable(InteropMarshaler.Naming.ForInteropReturnValue());
                return writer.WriteMarshalVariableFromNative(bodyWriter, variableName, this.parameters, true, false, this.metadataAccess);
            }
        }
    }
}

