namespace Unity.SerializationWeaver.Common
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;

    public class SerializationTypeProvider
    {
        private readonly AssemblyDefinition _unityEngineAssembly;

        public SerializationTypeProvider(AssemblyDefinition unityEngineAssembly)
        {
            this._unityEngineAssembly = unityEngineAssembly;
        }

        private TypeDefinition GetTypeNamed(string typeName)
        {
            <GetTypeNamed>c__AnonStorey0 storey = new <GetTypeNamed>c__AnonStorey0 {
                typeName = typeName
            };
            return this.Types.First<TypeDefinition>(new Func<TypeDefinition, bool>(storey.<>m__0));
        }

        public TypeDefinition PPtrRemapperInterface =>
            this.GetTypeNamed("UnityEngine.IPPtrRemapper");

        public TypeDefinition SerializedStateReaderInterface =>
            this.GetTypeNamed("UnityEngine.ISerializedStateReader");

        public TypeDefinition SerializedStateWriterInterface =>
            this.GetTypeNamed("UnityEngine.ISerializedStateWriter");

        private IEnumerable<TypeDefinition> Types =>
            this._unityEngineAssembly.MainModule.Types;

        public TypeDefinition UnityAssetsReferenceHolderInterface =>
            this.GetTypeNamed("UnityEngine.IUnityAssetsReferenceHolder");

        public TypeDefinition UnitySerializableInterface =>
            this.GetTypeNamed("UnityEngine.IUnitySerializable");

        [CompilerGenerated]
        private sealed class <GetTypeNamed>c__AnonStorey0
        {
            internal string typeName;

            internal bool <>m__0(TypeDefinition t) => 
                (t.FullName == this.typeName);
        }
    }
}

