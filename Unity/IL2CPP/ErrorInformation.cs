namespace Unity.IL2CPP
{
    using Mono.Cecil;
    using Mono.Cecil.Cil;
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    public sealed class ErrorInformation
    {
        private EventDefinition _event;
        private FieldDefinition _field;
        private static readonly ErrorInformation _instance = new ErrorInformation();
        private MethodDefinition _method;
        private PropertyDefinition _property;
        private TypeDefinition _type;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Mono.Cecil.Cil.Instruction <Instruction>k__BackingField;

        private void ClearAllTypeChildren()
        {
            this._method = null;
            this._field = null;
            this._property = null;
            this._event = null;
            this.Instruction = null;
        }

        public static ErrorInformation CurrentlyProcessing =>
            _instance;

        public EventDefinition Event
        {
            get => 
                this._event;
            set
            {
                if (this._event != value)
                {
                    this.ClearAllTypeChildren();
                    this._event = value;
                }
            }
        }

        public FieldDefinition Field
        {
            get => 
                this._field;
            set
            {
                if (this._field != value)
                {
                    this.ClearAllTypeChildren();
                    this._field = value;
                }
            }
        }

        public Mono.Cecil.Cil.Instruction Instruction { get; set; }

        public MethodDefinition Method
        {
            get => 
                this._method;
            set
            {
                if (this._method != value)
                {
                    this.ClearAllTypeChildren();
                    this._method = value;
                }
            }
        }

        public PropertyDefinition Property
        {
            get => 
                this._property;
            set
            {
                if (this._property != value)
                {
                    this.ClearAllTypeChildren();
                    this._property = value;
                }
            }
        }

        public TypeDefinition Type
        {
            get => 
                this._type;
            set
            {
                if (this._type != value)
                {
                    this.ClearAllTypeChildren();
                    this._type = value;
                }
            }
        }
    }
}

