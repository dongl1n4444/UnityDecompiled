namespace Unity.Bindings
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    [AttributeUsage(AttributeTargets.Property)]
    internal class NativePropertyAttribute : Attribute, IBindingsNameProviderAttribute, IBindingsIsThreadSafeProviderAttribute, IBindingsAttribute
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool <IsThreadSafe>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string <Name>k__BackingField;

        public NativePropertyAttribute()
        {
        }

        public NativePropertyAttribute(bool isThreadSafe)
        {
            this.IsThreadSafe = isThreadSafe;
        }

        public NativePropertyAttribute(string name)
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

        public NativePropertyAttribute(string name, bool isThreadSafe) : this(name)
        {
            this.IsThreadSafe = isThreadSafe;
        }

        public bool IsThreadSafe { get; set; }

        public string Name { get; set; }
    }
}

