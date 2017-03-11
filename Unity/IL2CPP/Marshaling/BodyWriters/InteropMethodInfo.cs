namespace Unity.IL2CPP.Marshaling.BodyWriters
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using Unity.IL2CPP.ILPreProcessor;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;
    using Unity.IL2CPP.Marshaling;
    using Unity.IL2CPP.Marshaling.BodyWriters.NativeToManaged;

    public class InteropMethodInfo
    {
        protected readonly InteropMarshaler _marshaler;
        protected readonly Unity.IL2CPP.ILPreProcessor.TypeResolver _typeResolver;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly MethodReference <InteropMethod>k__BackingField;
        public readonly MarshaledType[] MarshaledParameterTypes;
        public readonly MarshaledType MarshaledReturnType;
        [Inject]
        public static INamingService Naming;
        public readonly MarshaledParameter[] Parameters;

        protected InteropMethodInfo(MethodReference interopMethod, MethodReference methodForParameterNames, InteropMarshaler marshaler)
        {
            this.<InteropMethod>k__BackingField = interopMethod;
            this._marshaler = marshaler;
            this._typeResolver = Unity.IL2CPP.ILPreProcessor.TypeResolver.For(interopMethod.DeclaringType, interopMethod);
            MethodDefinition definition = interopMethod.Resolve();
            this.Parameters = new MarshaledParameter[definition.Parameters.Count];
            for (int i = 0; i < definition.Parameters.Count; i++)
            {
                ParameterDefinition parameterReference = methodForParameterNames.Parameters[i];
                ParameterDefinition definition3 = definition.Parameters[i];
                TypeReference parameterType = this._typeResolver.Resolve(definition3.ParameterType);
                this.Parameters[i] = new MarshaledParameter(parameterReference.Name, Naming.ForParameterName(parameterReference), parameterType, definition3.MarshalInfo, definition3.IsIn, definition3.IsOut);
            }
            List<MarshaledType> list = new List<MarshaledType>();
            foreach (MarshaledParameter parameter in this.Parameters)
            {
                foreach (MarshaledType type in marshaler.MarshalInfoWriterFor(parameter).MarshaledTypes)
                {
                    list.Add(new MarshaledType(type.Name, type.DecoratedName, parameter.NameInGeneratedCode + type.VariableName));
                }
            }
            MarshaledType[] marshaledTypes = marshaler.MarshalInfoWriterFor(interopMethod.MethodReturnType).MarshaledTypes;
            for (int j = 0; j < (marshaledTypes.Length - 1); j++)
            {
                MarshaledType type2 = marshaledTypes[j];
                list.Add(new MarshaledType(type2.Name + '*', type2.DecoratedName + '*', Naming.ForComInterfaceReturnParameterName() + type2.VariableName));
            }
            this.MarshaledParameterTypes = list.ToArray();
            this.MarshaledReturnType = marshaledTypes[marshaledTypes.Length - 1];
        }

        public static InteropMethodInfo ForComCallableWrapper(MethodReference managedMethod, MethodReference interfaceMethod, MarshalType marshalType) => 
            ForNativeToManaged(managedMethod, interfaceMethod, marshalType, true);

        public static InteropMethodInfo ForNativeToManaged(MethodReference managedMethod, MethodReference interopMethod, MarshalType marshalType, bool useUnicodeCharset) => 
            new InteropMethodInfo(interopMethod, managedMethod, new NativeToManagedMarshaler(Unity.IL2CPP.ILPreProcessor.TypeResolver.For(interopMethod.DeclaringType, interopMethod), marshalType, useUnicodeCharset));

        public string GetDefaultReturnCppValue() => 
            this._marshaler.MarshalInfoWriterFor(this.InteropMethod.MethodReturnType).GetDefaultCppValue(this.MarshaledReturnType);

        protected virtual MethodReference InteropMethod =>
            this.<InteropMethod>k__BackingField;
    }
}

