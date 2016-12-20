namespace Unity.IL2CPP.Building
{
    using NiceIO;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;

    public class CppCompilationInstruction
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private NPath <CacheDirectory>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private IEnumerable<string> <CompilerFlags>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private IEnumerable<string> <Defines>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private IEnumerable<NPath> <IncludePaths>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private NPath <SourceFile>k__BackingField;

        public CppCompilationInstruction()
        {
            this.Defines = Enumerable.Empty<string>();
            this.IncludePaths = Enumerable.Empty<NPath>();
            this.CompilerFlags = Enumerable.Empty<string>();
        }

        public NPath CacheDirectory { get; set; }

        public IEnumerable<string> CompilerFlags { get; set; }

        public IEnumerable<string> Defines { get; set; }

        public IEnumerable<NPath> IncludePaths { get; set; }

        public NPath SourceFile { get; set; }
    }
}

