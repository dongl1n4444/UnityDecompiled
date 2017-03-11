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
    using Unity.IL2CPP.Building;

    public class SimpleDirectoryProgramBuildDescription : ProgramBuildDescription, IHaveSourceDirectories
    {
        private readonly IEnumerable<string> _additionalCompilerFlags;
        private readonly IEnumerable<string> _additionalLinkerFlags;
        private readonly NPath _cacheDirectory;
        private readonly NPath _sourceDirectory;

        public SimpleDirectoryProgramBuildDescription(NPath sourceDir, NPath outputFile, NPath cacheDirectory, IEnumerable<string> additionalCompilerFlags = null, IEnumerable<string> additionalLinkerFlags = null)
        {
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
            this._cacheDirectory = cacheDirectory;
        }

        public override IEnumerable<string> AdditionalCompilerFlags =>
            this._additionalCompilerFlags;

        public override IEnumerable<string> AdditionalLinkerFlags =>
            this._additionalLinkerFlags;

        public override IEnumerable<CppCompilationInstruction> CppCompileInstructions =>
            this._sourceDirectory.Files("*.cpp", false).Select<NPath, CppCompilationInstruction>(delegate (NPath f) {
                CppCompilationInstruction instruction = new CppCompilationInstruction {
                    SourceFile=f,
                    CompilerFlags=this._additionalCompilerFlags
                };
                NPath[] pathArray1 = new NPath[] { this._sourceDirectory };
                instruction.IncludePaths = pathArray1;
                instruction.CacheDirectory = this._cacheDirectory;
                return instruction;
            });

        public override NPath GlobalCacheDirectory =>
            this._cacheDirectory;

        public IEnumerable<NPath> SourceDirectories =>
            new <>c__Iterator0 { 
                $this=this,
                $PC=-2
            };

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
            IEnumerator IEnumerable.GetEnumerator() => 
                this.System.Collections.Generic.IEnumerable<NiceIO.NPath>.GetEnumerator();

            NPath IEnumerator<NPath>.Current =>
                this.$current;

            object IEnumerator.Current =>
                this.$current;
        }
    }
}

