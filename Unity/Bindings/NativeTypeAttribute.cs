namespace Unity.Bindings
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    [AttributeUsage(AttributeTargets.Class)]
    internal class NativeTypeAttribute : NativeMemberAttribute, IBindingsGenerateMarshallingTypeAttribute, IBindingsNameProviderAttribute, IBindingsAttribute
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private NativeStructGenerateOption <GenerateMarshallingType>k__BackingField;

        public NativeTypeAttribute()
        {
            this.GenerateMarshallingType = NativeStructGenerateOption.UseCustomStruct;
        }

        public NativeTypeAttribute(string name) : base(name)
        {
            this.GenerateMarshallingType = NativeStructGenerateOption.UseCustomStruct;
        }

        public NativeTypeAttribute(NativeStructGenerateOption generateMarshallingType)
        {
            this.GenerateMarshallingType = generateMarshallingType;
        }

        public NativeTypeAttribute(string name, string header) : base(name, header)
        {
            this.GenerateMarshallingType = NativeStructGenerateOption.UseCustomStruct;
        }

        public NativeTypeAttribute(string name, NativeStructGenerateOption generateMarshallingType) : base(name)
        {
            this.GenerateMarshallingType = generateMarshallingType;
        }

        public NativeTypeAttribute(string name, string header, NativeStructGenerateOption generateMarshallingType) : base(name, header)
        {
            this.GenerateMarshallingType = generateMarshallingType;
        }

        public NativeStructGenerateOption GenerateMarshallingType { get; set; }
    }
}

