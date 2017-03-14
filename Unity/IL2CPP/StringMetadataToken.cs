namespace Unity.IL2CPP
{
    using Mono.Cecil;
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    public class StringMetadataToken
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private AssemblyDefinition <Assembly>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string <Literal>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private MetadataToken <Token>k__BackingField;

        public StringMetadataToken(string literal, AssemblyDefinition assembly, MetadataToken token)
        {
            this.Literal = literal;
            this.Assembly = assembly;
            this.Token = token;
        }

        public AssemblyDefinition Assembly { get; private set; }

        public string Literal { get; private set; }

        public MetadataToken Token { get; private set; }
    }
}

