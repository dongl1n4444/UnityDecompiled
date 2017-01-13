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
            return this.Types.First<TypeDefinition>(new Func<TypeDefinition, bool>(storey.<>m__0));
        }

        public TypeDefinition PPtrRemapperInterface =>
            this.GetTypeNamed("UnityEngine.IPPtrRemapper");

        public TypeDefinition SerializedNamedStateReaderInterface =>
            this.GetTypeNamed("UnityEngine.ISerializedNamedStateReader");

        public TypeDefinition SerializedNamedStateWriterInterface =>
            this.GetTypeNamed("UnityEngine.ISerializedNamedStateWriter");

        public TypeDefinition SerializedStateReaderInterface =>
            this.GetTypeNamed("UnityEngine.ISerializedStateReader");

        public TypeDefinition SerializedStateWriterInterface =>
            this.GetTypeNamed("UnityEngine.ISerializedStateWriter");

        private IEnumerable<TypeDefinition> Types =>
            this.UnityEngineAssembly.MainModule.Types;

        public AssemblyDefinition UnityEngineAssembly { get; private set; }

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

