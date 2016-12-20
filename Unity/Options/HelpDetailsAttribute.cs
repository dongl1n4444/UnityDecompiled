namespace Unity.Options
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    [AttributeUsage(AttributeTargets.Field)]
    public class HelpDetailsAttribute : Attribute
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string <CustomValueDescription>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string <Summary>k__BackingField;

        public HelpDetailsAttribute(string summary, [Optional, DefaultParameterValue(null)] string customValueDescription)
        {
            this.Summary = summary;
            this.CustomValueDescription = customValueDescription;
        }

        public string CustomValueDescription { get; set; }

        public string Summary { get; set; }
    }
}

