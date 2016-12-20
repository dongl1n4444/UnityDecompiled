namespace Unity.IL2CPP.Building.BuildDescriptions
{
    using NiceIO;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using Unity.IL2CPP.Building;
    using Unity.IL2CPP.Building.ToolChains;
    using Unity.IL2CPP.Common;

    public abstract class ProgramBuildDescription
    {
        protected NPath _outputFile;

        protected ProgramBuildDescription()
        {
        }

        public virtual IEnumerable<string> AdditionalDefinesFor(NPath path)
        {
            return Enumerable.Empty<string>();
        }

        public virtual IEnumerable<NPath> AdditionalIncludePathsFor(NPath path)
        {
            return Enumerable.Empty<NPath>();
        }

        public virtual void FinalizeBuild(CppToolChain toolChain)
        {
        }

        public virtual void OnBeforeLink(NPath workingDirectory, IEnumerable<NPath> objectFiles, CppToolChainContext toolChainContext, bool forceRebuild, bool verbose)
        {
        }

        public virtual IEnumerable<string> AdditionalCompilerFlags
        {
            get
            {
                return Enumerable.Empty<string>();
            }
        }

        public virtual IEnumerable<string> AdditionalLinkerFlags
        {
            get
            {
                return Enumerable.Empty<string>();
            }
        }

        public abstract IEnumerable<CppCompilationInstruction> CppCompileInstructions { get; }

        public virtual IEnumerable<NPath> DynamicLibraries
        {
            get
            {
                return new <>c__Iterator0 { $PC = -2 };
            }
        }

        public abstract NPath GlobalCacheDirectory { get; }

        public virtual NPath OutputFile
        {
            get
            {
                if (this._outputFile == null)
                {
                    if (this.GlobalCacheDirectory != null)
                    {
                        string[] textArray1 = new string[] { "program" };
                        this._outputFile = this.GlobalCacheDirectory.Combine(textArray1);
                        return this._outputFile;
                    }
                    string[] append = new string[] { "program" };
                    this._outputFile = TempDir.Root.Combine(append);
                }
                return this._outputFile;
            }
        }

        public virtual IEnumerable<NPath> StaticLibraries
        {
            get
            {
                return Enumerable.Empty<NPath>();
            }
        }

        [CompilerGenerated]
        private sealed class <>c__Iterator0 : IEnumerable, IEnumerable<NPath>, IEnumerator, IDisposable, IEnumerator<NPath>
        {
            internal NPath $current;
            internal bool $disposing;
            internal int $PC;

            [DebuggerHidden]
            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                this.$PC = -1;
                if (this.$PC == 0)
                {
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
                return new ProgramBuildDescription.<>c__Iterator0();
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

