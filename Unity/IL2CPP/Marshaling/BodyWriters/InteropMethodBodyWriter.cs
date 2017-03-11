namespace Unity.IL2CPP.Marshaling.BodyWriters
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using Unity.IL2CPP;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;
    using Unity.IL2CPP.Marshaling;
    using Unity.IL2CPP.Marshaling.MarshalInfoWriters;

    public abstract class InteropMethodBodyWriter : InteropMethodInfo
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool <AreParametersMarshaled>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool <IsReturnValueMarshaled>k__BackingField;
        [Inject]
        public static ITypeProviderService TypeProvider;

        protected InteropMethodBodyWriter(MethodReference interopMethod, MethodReference methodForParameterNames, InteropMarshaler marshaler) : base(interopMethod, methodForParameterNames, marshaler)
        {
            this.<AreParametersMarshaled>k__BackingField = true;
            this.<IsReturnValueMarshaled>k__BackingField = true;
        }

        protected DefaultMarshalInfoWriter FirstOrDefaultUnmarshalableMarshalInfoWriter()
        {
            foreach (MarshaledParameter parameter in base.Parameters)
            {
                if (!base._marshaler.CanMarshalAsInputParameter(parameter))
                {
                    return base._marshaler.MarshalInfoWriterFor(parameter);
                }
                if (this.IsOutParameter(parameter) && !base._marshaler.CanMarshalAsOutputParameter(parameter))
                {
                    return base._marshaler.MarshalInfoWriterFor(parameter);
                }
            }
            MethodReturnType methodReturnType = this.GetMethodReturnType();
            if ((methodReturnType.ReturnType.MetadataType != MetadataType.Void) && !base._marshaler.CanMarshalAsOutputParameter(methodReturnType))
            {
                return base._marshaler.MarshalInfoWriterFor(methodReturnType);
            }
            return null;
        }

        protected IList<CustomAttribute> GetCustomMethodAttributes() => 
            this.InteropMethod.Resolve().CustomAttributes;

        protected string GetMethodName() => 
            this.InteropMethod.Name;

        protected virtual string GetMethodNameInGeneratedCode() => 
            InteropMethodInfo.Naming.ForMethodNameOnly(this.InteropMethod);

        protected MethodReturnType GetMethodReturnType() => 
            this.InteropMethod.MethodReturnType;

        protected bool IsInParameter(MarshaledParameter parameter)
        {
            TypeReference parameterType = parameter.ParameterType;
            return ((!parameter.IsOut || parameter.IsIn) || (parameterType.IsValueType() && !parameterType.IsByReference));
        }

        protected bool IsOutParameter(MarshaledParameter parameter)
        {
            TypeReference parameterType = parameter.ParameterType;
            if (parameter.IsOut && !parameterType.IsValueType())
            {
                return true;
            }
            if (parameter.IsIn && !parameter.IsOut)
            {
                return false;
            }
            return (parameter.ParameterType.IsByReference || MarshalingUtils.IsStringBuilder(parameterType));
        }

        protected DefaultMarshalInfoWriter MarshalInfoWriterFor(MethodReturnType methodReturnType) => 
            base._marshaler.MarshalInfoWriterFor(methodReturnType);

        protected DefaultMarshalInfoWriter MarshalInfoWriterFor(MarshaledParameter parameter) => 
            base._marshaler.MarshalInfoWriterFor(parameter);

        private bool ParameterRequiresCleanup(MarshaledParameter parameter) => 
            (this.IsInParameter(parameter) || parameter.IsOut);

        private void WriteCleanupParameter(CppCodeWriter writer, string valueName, MarshaledParameter parameter, IRuntimeMetadataAccess metadataAccess)
        {
            if (this.ParameterRequiresCleanup(parameter))
            {
                if (this.IsInParameter(parameter))
                {
                    base._marshaler.WriteMarshalCleanupParameter(writer, valueName, parameter, metadataAccess);
                }
                else
                {
                    base._marshaler.WriteMarshalCleanupEmptyParameter(writer, valueName, parameter, metadataAccess);
                }
            }
        }

        protected abstract void WriteInteropCallStatement(CppCodeWriter writer, string[] localVariableNames, IRuntimeMetadataAccess metadataAccess);
        private string WriteMarshalInputParameter(CppCodeWriter writer, MarshaledParameter parameter, IRuntimeMetadataAccess metadataAccess)
        {
            if (this.IsInParameter(parameter))
            {
                return base._marshaler.WriteMarshalInputParameter(writer, parameter, base.Parameters, metadataAccess);
            }
            return base._marshaler.WriteMarshalEmptyInputParameter(writer, parameter, base.Parameters, metadataAccess);
        }

        private string[] WriteMarshalInputParameters(CppCodeWriter writer, IRuntimeMetadataAccess metadataAccess)
        {
            string[] strArray = new string[base.Parameters.Length];
            for (int i = 0; i < base.Parameters.Length; i++)
            {
                strArray[i] = this.WriteMarshalInputParameter(writer, base.Parameters[i], metadataAccess);
            }
            return strArray;
        }

        private void WriteMarshalOutputParameter(CppCodeWriter writer, string valueName, MarshaledParameter parameter, IRuntimeMetadataAccess metadataAccess)
        {
            if (this.IsOutParameter(parameter))
            {
                base._marshaler.WriteMarshalOutputParameter(writer, valueName, parameter, base.Parameters, metadataAccess);
            }
        }

        private void WriteMarshalOutputParameters(CppCodeWriter writer, string[] localVariableNames, IRuntimeMetadataAccess metadataAccess)
        {
            for (int i = 0; i < base.Parameters.Length; i++)
            {
                this.WriteMarshalOutputParameter(writer, localVariableNames[i], base.Parameters[i], metadataAccess);
                this.WriteCleanupParameter(writer, localVariableNames[i], base.Parameters[i], metadataAccess);
            }
        }

        public void WriteMethodBody(CppCodeWriter writer, IRuntimeMetadataAccess metadataAccess)
        {
            DefaultMarshalInfoWriter writer2 = this.FirstOrDefaultUnmarshalableMarshalInfoWriter();
            if (writer2 != null)
            {
                writer.WriteStatement(Emit.RaiseManagedException(writer2.GetMarshalingException()));
            }
            else
            {
                foreach (MarshaledParameter parameter in base.Parameters)
                {
                    this.MarshalInfoWriterFor(parameter).WriteIncludesForMarshaling(writer);
                }
                this.MarshalInfoWriterFor(this.GetMethodReturnType()).WriteIncludesForMarshaling(writer);
                this.WriteMethodBodyImpl(writer, metadataAccess);
            }
        }

        private void WriteMethodBodyImpl(CppCodeWriter writer, IRuntimeMetadataAccess metadataAccess)
        {
            this.WriteMethodPrologue(writer, metadataAccess);
            string[] localVariableNames = !this.AreParametersMarshaled ? null : this.WriteMarshalInputParameters(writer, metadataAccess);
            string unmarshaledReturnValueVariableName = null;
            object[] args = new object[] { base._marshaler.GetPrettyCalleeName() };
            writer.WriteLine("// {0} invocation", args);
            this.WriteInteropCallStatement(writer, localVariableNames, metadataAccess);
            writer.WriteLine();
            MethodReturnType methodReturnType = this.GetMethodReturnType();
            if (methodReturnType.ReturnType.MetadataType != MetadataType.Void)
            {
                if (this.IsReturnValueMarshaled)
                {
                    unmarshaledReturnValueVariableName = base._marshaler.WriteMarshalReturnValue(writer, methodReturnType, base.Parameters, metadataAccess);
                    base._marshaler.WriteMarshalCleanupReturnValue(writer, methodReturnType, metadataAccess);
                }
                else
                {
                    unmarshaledReturnValueVariableName = InteropMethodInfo.Naming.ForInteropReturnValue();
                }
            }
            if (this.AreParametersMarshaled)
            {
                this.WriteMarshalOutputParameters(writer, localVariableNames, metadataAccess);
            }
            this.WriteMethodEpilogue(writer, metadataAccess);
            this.WriteReturnStatement(writer, unmarshaledReturnValueVariableName, metadataAccess);
        }

        protected virtual void WriteMethodEpilogue(CppCodeWriter writer, IRuntimeMetadataAccess metadataAccess)
        {
        }

        protected virtual void WriteMethodPrologue(CppCodeWriter writer, IRuntimeMetadataAccess metadataAccess)
        {
        }

        protected virtual void WriteReturnStatement(CppCodeWriter writer, string unmarshaledReturnValueVariableName, IRuntimeMetadataAccess metadataAccess)
        {
            if (this.GetMethodReturnType().ReturnType.MetadataType != MetadataType.Void)
            {
                object[] args = new object[] { unmarshaledReturnValueVariableName };
                writer.WriteLine("return {0};", args);
            }
        }

        protected virtual bool AreParametersMarshaled =>
            this.<AreParametersMarshaled>k__BackingField;

        protected sealed override MethodReference InteropMethod =>
            base.InteropMethod;

        protected virtual bool IsReturnValueMarshaled =>
            this.<IsReturnValueMarshaled>k__BackingField;
    }
}

