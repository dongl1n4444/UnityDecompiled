namespace Unity.Bindings
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    [AttributeUsage(AttributeTargets.Method)]
    internal class NativeSetterAttribute : Attribute, IBindingsNameProviderAttribute, IBindingsAttribute
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string <Name>k__BackingField;

        public NativeSetterAttribute()
        {
        }

        public NativeSetterAttribute(string name)
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

