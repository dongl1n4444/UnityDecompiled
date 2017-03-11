namespace UnityEditor
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    internal class VisualStudioPath
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string <Edition>k__BackingField;
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private string <Path>k__BackingField;

        public VisualStudioPath(string path, string edition = "")
        {
            this.Path = path;
            this.Edition = edition;
        }

        public string Edition { get; set; }

        public string Path { get; set; }
    }
}

