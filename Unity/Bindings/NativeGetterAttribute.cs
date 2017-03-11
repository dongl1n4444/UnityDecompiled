namespace Unity.Bindings
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    [AttributeUsage(AttributeTargets.Method)]
    internal class NativeGetterAttribute : Attribute, IBindingsNameProviderAttribute, IBindingsAttribute
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string <Name>k__BackingField;

        public NativeGetterAttribute()
        {
        }

        public NativeGetterAttribute(string name)
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

        public string Name { get; set; }
    }
}

