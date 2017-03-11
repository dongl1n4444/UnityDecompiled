namespace Unity.Bindings
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    [AttributeUsage(AttributeTargets.Struct)]
    internal class NativeStructAttribute : NativeMemberAttribute, IBindingsGenerateMarshallingTypeAttribute, IBindingsNameProviderAttribute, IBindingsAttribute
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private NativeStructGenerateOption <GenerateMarshallingType>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string <IntermediateScriptingStructName>k__BackingField;

        public NativeStructAttribute()
        {
            this.GenerateMarshallingType = NativeStructGenerateOption.Default;
        }

        public NativeStructAttribute(string name) : base(name)
        {
            this.GenerateMarshallingType = NativeStructGenerateOption.Default;
        }

        public NativeStructAttribute(NativeStructGenerateOption generateMarshallingType)
        {
            this.GenerateMarshallingType = generateMarshallingType;
        }

        public NativeStructAttribute(string name, string header) : base(name, header)
        {
            this.GenerateMarshallingType = NativeStructGenerateOption.Default;
        }

        public NativeStructAttribute(string name, NativeStructGenerateOption generateMarshallingType) : base(name)
        {
            this.GenerateMarshallingType = generateMarshallingType;
        }

        public NativeStructAttribute(string name, string header, NativeStructGenerateOption generateMarshallingType) : base(name, header)
        {
            this.GenerateMarshallingType = generateMarshallingType;
        }

        public NativeStructGenerateOption GenerateMarshallingType { get; set; }

        public string IntermediateScriptingStructName { get; set; }
    }
}

