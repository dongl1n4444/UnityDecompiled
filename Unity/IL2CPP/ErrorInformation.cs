namespace Unity.IL2CPP
{
    using Mono.Cecil;
    using Mono.Cecil.Cil;
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    public sealed class ErrorInformation
    {
        private static readonly ErrorInformation _instance = new ErrorInformation();
        private MethodDefinition _method;
        private TypeDefinition _type;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Mono.Cecil.Cil.Instruction <Instruction>k__BackingField;

        public static ErrorInformation CurrentlyProcessing
        {
            get
            {
                return _instance;
            }
        }

        public Mono.Cecil.Cil.Instruction Instruction { get; set; }

        public MethodDefinition Method
        {
            get
            {
                return this._method;
            }
            set
            {
                this.Instruction = null;
                this._method = value;
            }
        }

        public TypeDefinition Type
        {
            get
            {
                return this._type;
            }
            set
            {
                this.Method = null;
                this.Instruction = null;
                this._type = value;
            }
        }
    }
}

