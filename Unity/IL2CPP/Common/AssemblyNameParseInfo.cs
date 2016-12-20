namespace Unity.IL2CPP.Common
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    public class AssemblyNameParseInfo
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ushort <Build>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string <Culture>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private uint <Flags>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private uint <HashAlgorithm>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private uint <HashLength>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string <HashValue>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ushort <Major>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ushort <Minor>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string <Name>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string <PublicKey>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private char[] <PublicKeyToken>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ushort <Revision>k__BackingField;
        internal const int PublicKeyTokenLength = 0x11;

        public AssemblyNameParseInfo()
        {
            this.PublicKeyToken = new char[0x11];
        }

        public ushort Build { get; internal set; }

        public string Culture { get; internal set; }

        public uint Flags { get; internal set; }

        public uint HashAlgorithm { get; internal set; }

        public uint HashLength { get; internal set; }

        public string HashValue { get; internal set; }

        public ushort Major { get; internal set; }

        public ushort Minor { get; internal set; }

        public string Name { get; internal set; }

        public string PublicKey { get; internal set; }

        public char[] PublicKeyToken { get; internal set; }

        public ushort Revision { get; internal set; }
    }
}

