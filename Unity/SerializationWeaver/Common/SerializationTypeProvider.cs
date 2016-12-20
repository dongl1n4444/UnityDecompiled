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
            return Enumerable.First<TypeDefinition>(this.Types, new Func<TypeDefinition, bool>(storey, (IntPtr) this.<>m__0));
        }

        public TypeDefinition PPtrRemapperInterface
        {
            get
            {
                return this.GetTypeNamed("UnityEngine.IPPtrRemapper");
            }
        }

        public TypeDefinition SerializedStateReaderInterface
        {
            get
            {
                return this.GetTypeNamed("UnityEngine.ISerializedStateReader");
            }
        }

        public TypeDefinition SerializedStateWriterInterface
        {
            get
            {
                return this.GetTypeNamed("UnityEngine.ISerializedStateWriter");
            }
        }

        private IEnumerable<TypeDefinition> Types
        {
            get
            {
                return this._unityEngineAssembly.MainModule.Types;
            }
        }

        public TypeDefinition UnityAssetsReferenceHolderInterface
        {
            get
            {
                return this.GetTypeNamed("UnityEngine.IUnityAssetsReferenceHolder");
            }
        }

        public TypeDefinition UnitySerializableInterface
        {
            get
            {
                return this.GetTypeNamed("UnityEngine.IUnitySerializable");
            }
        }

        [CompilerGenerated]
        private sealed class <GetTypeNamed>c__AnonStorey0
        {
            internal string typeName;

            internal bool <>m__0(TypeDefinition t)
            {
                return (t.FullName == this.typeName);
            }
        }
    }
}

