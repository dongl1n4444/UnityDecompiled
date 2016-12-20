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

    internal abstract class InteropMarshaler
    {
        protected readonly MarshalType _marshalType;
        protected readonly Unity.IL2CPP.ILPreProcessor.TypeResolver _typeResolver;
        protected readonly bool _useUnicodeCharset;
        [Inject]
        public static INamingService Naming;

        public InteropMarshaler(Unity.IL2CPP.ILPreProcessor.TypeResolver typeResolver, MarshalType marshalType, bool useUnicodeCharset)
        {
            this._typeResolver = typeResolver;
            this._marshalType = marshalType;
            this._useUnicodeCharset = useUnicodeCharset;
        }

        public abstract bool CanMarshalAsInputParameter(MarshaledParameter parameter);
        public abstract bool CanMarshalAsOutputParameter(MethodReturnType methodReturnType);
        public abstract bool CanMarshalAsOutputParameter(MarshaledParameter parameter);
        public abstract string GetPrettyCalleeName();
        public DefaultMarshalInfoWriter MarshalInfoWriterFor(MethodReturnType methodReturnType)
        {
            return MarshalDataCollector.MarshalInfoWriterFor(this._typeResolver.Resolve(methodReturnType.ReturnType), this._marshalType, methodReturnType.MarshalInfo, this._useUnicodeCharset, false, false, null);
        }

        public DefaultMarshalInfoWriter MarshalInfoWriterFor(MarshaledParameter parameter)
        {
            return MarshalDataCollector.MarshalInfoWriterFor(parameter.ParameterType, this._marshalType, parameter.MarshalInfo, this._useUnicodeCharset, false, false, null);
        }

        public abstract void WriteMarshalCleanupEmptyParameter(CppCodeWriter writer, string valueName, MarshaledParameter parameter, IRuntimeMetadataAccess metadataAccess);
        public abstract void WriteMarshalCleanupParameter(CppCodeWriter writer, string valueName, MarshaledParameter parameter, IRuntimeMetadataAccess metadataAccess);
        public abstract void WriteMarshalCleanupReturnValue(CppCodeWriter writer, MethodReturnType methodReturnType, IRuntimeMetadataAccess metadataAccess);
        public abstract string WriteMarshalEmptyInputParameter(CppCodeWriter writer, MarshaledParameter parameter, IList<MarshaledParameter> parameters, IRuntimeMetadataAccess metadataAccess);
        public abstract string WriteMarshalInputParameter(CppCodeWriter writer, MarshaledParameter parameter, IList<MarshaledParameter> parameters, IRuntimeMetadataAccess metadataAccess);
        public abstract void WriteMarshalOutputParameter(CppCodeWriter writer, string valueName, MarshaledParameter parameter, IList<MarshaledParameter> parameters, IRuntimeMetadataAccess metadataAccess);
        public abstract string WriteMarshalReturnValue(CppCodeWriter writer, MethodReturnType methodReturnType, IList<MarshaledParameter> parameters, IRuntimeMetadataAccess metadataAccess);
    }
}

