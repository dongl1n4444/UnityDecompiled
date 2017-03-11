namespace JetBrains.Annotations
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    [AttributeUsage(AttributeTargets.Parameter)]
    public class PathReferenceAttribute : Attribute
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string <BasePath>k__BackingField;

        public PathReferenceAttribute()
        {
        }

        public PathReferenceAttribute([PathReference] string basePath)
        {
            this.BasePath = basePath;
        }

        [NotNull]
        public string BasePath { get; private set; }
    }
}

