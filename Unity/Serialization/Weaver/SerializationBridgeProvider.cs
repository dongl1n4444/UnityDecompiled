namespace Unity.Serialization.Weaver
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;

    public class SerializationBridgeProvider
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private AssemblyDefinition <UnityEngineAssembly>k__BackingField;

        public SerializationBridgeProvider(AssemblyDefinition serializationInferfaceProvider)
        {
            this.UnityEngineAssembly = serializationInferfaceProvider;
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

        public TypeDefinition SerializedNamedStateReaderInterface
        {
            get
            {
                return this.GetTypeNamed("UnityEngine.ISerializedNamedStateReader");
            }
        }

        public TypeDefinition SerializedNamedStateWriterInterface
        {
            get
            {
                return this.GetTypeNamed("UnityEngine.ISerializedNamedStateWriter");
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
                return this.UnityEngineAssembly.MainModule.Types;
            }
        }

        public AssemblyDefinition UnityEngineAssembly { get; private set; }

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

