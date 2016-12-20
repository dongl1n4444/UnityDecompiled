namespace Unity.IL2CPP.Building.BuildDescriptions
{
    using NiceIO;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;
    using Unity.IL2CPP.Common;

    public class SimpleDirectoryProgramBuildDescription : ProgramBuildDescription, IHaveSourceDirectories
    {
        private readonly IEnumerable<string> _additionalCompilerFlags;
        private readonly IEnumerable<string> _additionalLinkerFlags;
        private readonly NPath _cacheDirectory;
        private readonly NPath _sourceDirectory;

        public SimpleDirectoryProgramBuildDescription(NPath sourceDir, NPath outputFile, string programName, [Optional, DefaultParameterValue(null)] IEnumerable<string> additionalCompilerFlags, [Optional, DefaultParameterValue(null)] IEnumerable<string> additionalLinkerFlags, [Optional, DefaultParameterValue(null)] NPath rootCacheDirectory)
        {
            if (programName == null)
            {
                throw new ArgumentNullException("programName");
            }
            this._sourceDirectory = sourceDir;
            base._outputFile = outputFile;
            if (additionalCompilerFlags == null)
            {
            }
            this._additionalCompilerFlags = Enumerable.Empty<string>();
            if (additionalLinkerFlags == null)
            {
            }
            this._additionalLinkerFlags = Enumerable.Empty<string>();
            this._cacheDirectory = (rootCacheDirectory != null) ? rootCacheDirectory.Combine(new string[] { programName }) : TempDir.Il2CppTemporaryDirectoryRoot.Combine(new string[] { programName });
        }

        public override IEnumerable<string> AdditionalCompilerFlags
        {
            get
            {
                return this._additionalCompilerFlags;
            }
        }

        public override IEnumerable<string> AdditionalLinkerFlags
        {
            get
            {
                return this._additionalLinkerFlags;
            }
        }

        public override IEnumerable<CppCompilationInstruction> CppCompileInstructions
        {
            get
            {
                return Enumerable.Select<NPath, CppCompilationInstruction>(this._sourceDirectory.Files("*.cpp", false), new Func<NPath, CppCompilationInstruction>(this, (IntPtr) this.<get_CppCompileInstructions>m__0));
            }
        }

        public override NPath GlobalCacheDirectory
        {
            get
            {
                return this._cacheDirectory;
            }
        }

        public IEnumerable<NPath> SourceDirectories
        {
            get
            {
                return new <>c__Iterator0 { 
                    $this = this,
                    $PC = -2
                };
            }
        }

        [CompilerGenerated]
        private sealed class <>c__Iterator0 : IEnumerable, IEnumerable<NPath>, IEnumerator, IDisposable, IEnumerator<NPath>
        {
            internal NPath $current;
            internal bool $disposing;
            internal int $PC;
            internal SimpleDirectoryProgramBuildDescription $this;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$disposing = true;
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        this.$current = this.$this._sourceDirectory;
                        if (!this.$disposing)
                        {
                            this.$PC = 1;
                        }
                        return true;

                    case 1:
                        this.$PC = -1;
                        break;
                }
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            [DebuggerHidden]
            IEnumerator<NPath> IEnumerable<NPath>.GetEnumerator()
            {
                if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
                {
                    return this;
                }
                return new SimpleDirectoryProgramBuildDescription.<>c__Iterator0 { $this = this.$this };
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.System.Collections.Generic.IEnumerable<NiceIO.NPath>.GetEnumerator();
            }

            NPath IEnumerator<NPath>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }
    }
}

