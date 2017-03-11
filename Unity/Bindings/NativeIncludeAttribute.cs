namespace Unity.Bindings
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Struct | AttributeTargets.Class, AllowMultiple=true)]
    internal class NativeIncludeAttribute : Attribute, IBindingsHeaderProviderAttribute, IBindingsAttribute
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string <Header>k__BackingField;

        public NativeIncludeAttribute()
        {
        }

        public NativeIncludeAttribute(string header)
        {
            if (header == null)
            {
                throw new ArgumentNullException("header");
            }
            if (header == "")
            {
                throw new ArgumentException("header cannot be empty", "header");
            }
            this.Header = header;
        }

        public string Header { get; set; }
    }
}

