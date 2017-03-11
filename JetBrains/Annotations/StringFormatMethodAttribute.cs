namespace JetBrains.Annotations
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor, AllowMultiple=false, Inherited=true)]
    public sealed class StringFormatMethodAttribute : Attribute
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string <FormatParameterName>k__BackingField;

        public StringFormatMethodAttribute(string formatParameterName)
        {
            this.FormatParameterName = formatParameterName;
        }

        public string FormatParameterName { get; private set; }
    }
}

