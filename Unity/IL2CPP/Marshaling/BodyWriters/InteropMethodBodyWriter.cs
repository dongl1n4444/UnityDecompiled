namespace Unity.IL2CPP.Marshaling.BodyWriters
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using Unity.IL2CPP;
    using Unity.IL2CPP.ILPreProcessor;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;
    using Unity.IL2CPP.Marshaling;
    using Unity.IL2CPP.Marshaling.MarshalInfoWriters;

    internal abstract class InteropMethodBodyWriter
    {
        private readonly MethodReference _interopMethod;
        protected readonly MarshaledType[] _marshaledParameterTypes;
        protected readonly MarshaledType _marshaledReturnType;
        private readonly InteropMarshaler _marshaler;
        protected readonly MarshaledParameter[] _parameters;
        protected readonly Unity.IL2CPP.ILPreProcessor.TypeResolver _typeResolver;
        [Inject]
        public static INamingService Naming;
        [Inject]
        public static ITypeProviderService TypeProvider;

        protected InteropMethodBodyWriter(MethodReference interopMethod, MethodReference methodForParameterNames, InteropMarshaler marshaler)
        {
            this._typeResolver = Unity.IL2CPP.ILPreProcessor.TypeResolver.For(interopMethod.DeclaringType, interopMethod);
            this._interopMethod = interopMethod;
            this._marshaler = marshaler;
            MethodDefinition definition = this._interopMethod.Resolve();
            this._parameters = new MarshaledParameter[definition.Parameters.Count];
            for (int i = 0; i < definition.Parameters.Count; i++)
            {
                ParameterDefinition definition2 = definition.Parameters[i];
                TypeReference parameterType = this._typeResolver.Resolve(definition2.ParameterType);
                this._parameters[i] = new MarshaledParameter(methodForParameterNames.Parameters[i].Name, Naming.ForParameterName(methodForParameterNames.Parameters[i]), parameterType, definition2.MarshalInfo, definition2.IsIn, definition2.IsOut);
            }
            List<MarshaledType> list = new List<MarshaledType>();
            foreach (MarshaledParameter parameter in this._parameters)
            {
                foreach (MarshaledType type in this.MarshalInfoWriterFor(parameter).MarshaledTypes)
                {
                    list.Add(new MarshaledType(type.Name, type.DecoratedName, parameter.NameInGeneratedCode + type.VariableName));
                }
            }
            MarshaledType[] marshaledTypes = this.MarshalInfoWriterFor(this.GetMethodReturnType()).MarshaledTypes;
            for (int j = 0; j < (marshaledTypes.Length - 1); j++)
            {
                MarshaledType type2 = marshaledTypes[j];
                list.Add(new MarshaledType(type2.Name + "*", type2.DecoratedName + "*", Naming.ForComInterfaceReturnParameterName() + type2.VariableName));
            }
            this._marshaledParameterTypes = list.ToArray();
            this._marshaledReturnType = marshaledTypes[marshaledTypes.Length - 1];
        }

        protected DefaultMarshalInfoWriter FirstOrDefaultUnmarshalableMarshalInfoWriter()
        {
            foreach (MarshaledParameter parameter in this._parameters)
            {
                if (!this._marshaler.CanMarshalAsInputParameter(parameter))
                {
                    return this._marshaler.MarshalInfoWriterFor(parameter);
                }
                if (this.IsOutParameter(parameter) && !this._marshaler.CanMarshalAsOutputParameter(parameter))
                {
                    return this._marshaler.MarshalInfoWriterFor(parameter);
                }
            }
            MethodReturnType methodReturnType = this.GetMethodReturnType();
            if ((methodReturnType.ReturnType.MetadataType != MetadataType.Void) && !this._marshaler.CanMarshalAsOutputParameter(methodReturnType))
            {
                return this._marshaler.MarshalInfoWriterFor(methodReturnType);
            }
            return null;
        }

        protected IList<CustomAttribute> GetCustomMethodAttributes() => 
            this._interopMethod.Resolve().CustomAttributes;

        protected string GetMethodName() => 
            this._interopMethod.Name;

        protected virtual string GetMethodNameInGeneratedCode() => 
            Naming.ForMethodNameOnly(this._interopMethod);

        protected virtual MethodReturnType GetMethodReturnType() => 
            this._interopMethod.MethodReturnType;

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
            this._marshaler.MarshalInfoWriterFor(methodReturnType);

        protected DefaultMarshalInfoWriter MarshalInfoWriterFor(MarshaledParameter parameter) => 
            this._marshaler.MarshalInfoWriterFor(parameter);

        private bool ParameterRequiresCleanup(MarshaledParameter parameter) => 
            (this.IsInParameter(parameter) || parameter.IsOut);

        private void WriteCleanupParameter(CppCodeWriter writer, string valueName, MarshaledParameter parameter, IRuntimeMetadataAccess metadataAccess)
        {
            if (this.ParameterRequiresCleanup(parameter))
            {
                if (this.IsInParameter(parameter))
                {
                    this._marshaler.WriteMarshalCleanupParameter(writer, valueName, parameter, metadataAccess);
                }
                else
                {
                    this._marshaler.WriteMarshalCleanupEmptyParameter(writer, valueName, parameter, metadataAccess);
                }
            }
        }

        protected abstract void WriteInteropCallStatement(CppCodeWriter writer, string[] localVariableNames, IRuntimeMetadataAccess metadataAccess);
        private string WriteMarshalInputParameter(CppCodeWriter writer, MarshaledParameter parameter, IRuntimeMetadataAccess metadataAccess)
        {
            if (this.IsInParameter(parameter))
            {
                return this._marshaler.WriteMarshalInputParameter(writer, parameter, this._parameters, metadataAccess);
            }
            return this._marshaler.WriteMarshalEmptyInputParameter(writer, parameter, this._parameters, metadataAccess);
        }

        private string[] WriteMarshalInputParameters(CppCodeWriter writer, IRuntimeMetadataAccess metadataAccess)
        {
            string[] strArray = new string[this._parameters.Length];
            for (int i = 0; i < this._parameters.Length; i++)
            {
                strArray[i] = this.WriteMarshalInputParameter(writer, this._parameters[i], metadataAccess);
            }
            return strArray;
        }

        private void WriteMarshalOutputParameter(CppCodeWriter writer, string valueName, MarshaledParameter parameter, IRuntimeMetadataAccess metadataAccess)
        {
            if (this.IsOutParameter(parameter))
            {
                this._marshaler.WriteMarshalOutputParameter(writer, valueName, parameter, this._parameters, metadataAccess);
            }
        }

        private void WriteMarshalOutputParameters(CppCodeWriter writer, string[] localVariableNames, IRuntimeMetadataAccess metadataAccess)
        {
            for (int i = 0; i < this._parameters.Length; i++)
            {
                this.WriteMarshalOutputParameter(writer, localVariableNames[i], this._parameters[i], metadataAccess);
                this.WriteCleanupParameter(writer, localVariableNames[i], this._parameters[i], metadataAccess);
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
                foreach (MarshaledParameter parameter in this._parameters)
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
            string[] localVariableNames = this.WriteMarshalInputParameters(writer, metadataAccess);
            string unmarshaledReturnValueVariableName = null;
            object[] args = new object[] { this._marshaler.GetPrettyCalleeName() };
            writer.WriteLine("// {0} invocation", args);
            this.WriteInteropCallStatement(writer, localVariableNames, metadataAccess);
            writer.WriteLine();
            MethodReturnType methodReturnType = this.GetMethodReturnType();
            if (methodReturnType.ReturnType.MetadataType != MetadataType.Void)
            {
                unmarshaledReturnValueVariableName = this._marshaler.WriteMarshalReturnValue(writer, methodReturnType, this._parameters, metadataAccess);
                this._marshaler.WriteMarshalCleanupReturnValue(writer, methodReturnType, metadataAccess);
            }
            this.WriteMarshalOutputParameters(writer, localVariableNames, metadataAccess);
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
    }
}

