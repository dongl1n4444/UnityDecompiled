namespace Unity.Bindings
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    internal abstract class NativeMemberAttribute : Attribute, IBindingsHeaderProviderAttribute, IBindingsNameProviderAttribute, IBindingsAttribute
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string <Header>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string <Name>k__BackingField;

        protected NativeMemberAttribute()
        {
        }

        protected NativeMemberAttribute(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (name == "")
            {
                throw new ArgumentException("name cannot be empty", "name");
            }
            this.Name = name;
        }

        protected NativeMemberAttribute(string name, string header)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (name == "")
            {
                throw new ArgumentException("name cannot be empty", "name");
            }
            if (header == null)
            {
                throw new ArgumentNullException("header");
            }
            if (header == "")
            {
                throw new ArgumentException("header cannot be empty", "header");
            }
            this.Name = name;
            this.Header = header;
        }

        public string Header { get; set; }

        public string Name { get; set; }
    }
}

