namespace Unity.IL2CPP.WindowsRuntime
{
    using Mono.Cecil;
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Unity.IL2CPP;
    using Unity.IL2CPP.ILPreProcessor;
    using Unity.IL2CPP.Marshaling.BodyWriters.NativeToManaged;

    internal class KeyValuePairCCWWriter : IProjectedComCallableWrapperMethodWriter
    {
        private readonly TypeDefinition _keyValuePairTypeDef;

        public KeyValuePairCCWWriter(TypeDefinition keyValuePairTypeDef)
        {
            this._keyValuePairTypeDef = keyValuePairTypeDef;
        }

        public ComCallableWrapperMethodBodyWriter GetBodyWriter(MethodReference interfaceMethod)
        {
            <GetBodyWriter>c__AnonStorey0 storey = new <GetBodyWriter>c__AnonStorey0 {
                interfaceMethod = interfaceMethod
            };
            GenericInstanceType declaringType = (GenericInstanceType) storey.interfaceMethod.DeclaringType;
            GenericInstanceType typeReference = new GenericInstanceType(this._keyValuePairTypeDef) {
                GenericArguments = { 
                    declaringType.GenericArguments[0],
                    declaringType.GenericArguments[1]
                }
            };
            MethodDefinition method = this._keyValuePairTypeDef.Methods.Single<MethodDefinition>(new Func<MethodDefinition, bool>(storey.<>m__0));
            return new ProjectedMethodBodyWriter(Unity.IL2CPP.ILPreProcessor.TypeResolver.For(typeReference).Resolve(method), storey.interfaceMethod);
        }

        public void WriteDependenciesFor(CppCodeWriter writer, TypeReference interfaceType)
        {
        }

        [CompilerGenerated]
        private sealed class <GetBodyWriter>c__AnonStorey0
        {
            internal MethodReference interfaceMethod;

            internal bool <>m__0(MethodDefinition m) => 
                (m.Name == this.interfaceMethod.Name);
        }
    }
}

