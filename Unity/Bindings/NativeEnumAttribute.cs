namespace Unity.Bindings
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    [AttributeUsage(AttributeTargets.Enum)]
    internal class NativeEnumAttribute : NativeMemberAttribute
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool <GenerateNativeType>k__BackingField;

        public NativeEnumAttribute()
        {
            this.GenerateNativeType = false;
        }

        public NativeEnumAttribute(bool generateNativeType)
        {
            this.GenerateNativeType = generateNativeType;
        }

        public NativeEnumAttribute(string name) : base(name)
        {
            this.GenerateNativeType = false;
        }

        public NativeEnumAttribute(string name, bool generateNativeType) : base(name)
        {
            this.GenerateNativeType = generateNativeType;
        }

        public NativeEnumAttribute(string name, string header) : base(name, header)
        {
            this.GenerateNativeType = false;
        }

        public NativeEnumAttribute(string name, string header, bool generateNativeType) : base(name, header)
        {
            this.GenerateNativeType = generateNativeType;
        }

        public bool GenerateNativeType { get; set; }
    }
}

