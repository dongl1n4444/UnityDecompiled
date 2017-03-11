namespace Unity.Options
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    [AttributeUsage(AttributeTargets.Field, AllowMultiple=true)]
    public class OptionAliasAttribute : Attribute
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string <Name>k__BackingField;

        public OptionAliasAttribute(string name)
        {
            this.Name = name;
        }

        public string Name { get; set; }
    }
}

