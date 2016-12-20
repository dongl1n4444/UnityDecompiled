namespace Unity.Options
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    public class ProgramOptionsAttribute : Attribute
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string <CollectionSeparator>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string <Group>k__BackingField;

        public string CollectionSeparator { get; set; }

        public string Group { get; set; }
    }
}

