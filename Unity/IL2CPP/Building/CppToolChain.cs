namespace Unity.IL2CPP.Building
{
    using NiceIO;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using Unity.IL2CPP;
    using Unity.IL2CPP.Building.BuildDescriptions;
    using Unity.IL2CPP.Building.ToolChains;
    using Unity.IL2CPP.Common;

    public abstract class CppToolChain
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Unity.IL2CPP.Common.Architecture <Architecture>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Unity.IL2CPP.Building.BuildConfiguration <BuildConfiguration>k__BackingField;

        protected CppToolChain(Unity.IL2CPP.Common.Architecture architecture, Unity.IL2CPP.Building.BuildConfiguration buildConfiguration)
        {
            this.Architecture = architecture;
            this.BuildConfiguration = buildConfiguration;
        }

        public abstract bool CanBuildInCurrentEnvironment();
        protected IEnumerable<string> ChooseCompilerFlags(CppCompilationInstruction cppCompilationInstruction, Func<CppCompilationInstruction, IEnumerable<string>> defaultCompilerFlags) => 
            defaultCompilerFlags(cppCompilationInstruction).Concat<string>(cppCompilationInstruction.CompilerFlags);

        protected IEnumerable<string> ChooseLinkerFlags(IEnumerable<NPath> staticLibraries, IEnumerable<NPath> dynamicLibraries, NPath outputFile, IEnumerable<string> specifiedLinkerFlags, Func<IEnumerable<NPath>, IEnumerable<NPath>, NPath, IEnumerable<string>> defaultLinkerFlags) => 
            defaultLinkerFlags(staticLibraries, dynamicLibraries, outputFile).Concat<string>(specifiedLinkerFlags);

        public abstract NPath CompilerExecutableFor(NPath sourceFile);
        public abstract IEnumerable<string> CompilerFlagsFor(CppCompilationInstruction cppCompilationInstruction);
        public virtual CppToolChainContext CreateToolChainContext() => 
            new CppToolChainContext();

        public virtual bool DynamicLibrariesHaveToSitNextToExecutable() => 
            false;

        public virtual Dictionary<string, string> EnvVars() => 
            null;

        public abstract string ExecutableExtension();
        protected virtual string GetInterestingOutputFromCompilationShellResult(Shell.ExecuteResult shellResult) => 
            shellResult.StdOut;

        protected virtual string GetInterestingOutputFromLinkerShellResult(Shell.ExecuteResult shellResult) => 
            shellResult.StdOut;

        public virtual NPath GetLibraryFileName(NPath library) => 
            library;

        public abstract CppProgramBuilder.LinkerInvocation MakeLinkerInvocation(IEnumerable<NPath> objectFiles, NPath outputFile, IEnumerable<NPath> staticLibraries, IEnumerable<NPath> dynamicLibraries, IEnumerable<string> specifiedLinkerFlags, CppToolChainContext toolChainContext);
        public abstract string ObjectExtension();
        public virtual void OnAfterLink(NPath outputFile, CppToolChainContext toolChainContext, bool forceRebuild, bool verbose)
        {
        }

        public virtual void OnBeforeCompile(ProgramBuildDescription programBuildDescription, CppToolChainContext toolChainContext, NPath workingDirectory, bool forceRebuild, bool verbose)
        {
        }

        public virtual void OnBeforeLink(ProgramBuildDescription programBuildDescription, NPath workingDirectory, IEnumerable<NPath> objectFiles, CppToolChainContext toolChainContext, bool forceRebuild, bool verbose)
        {
        }

        public abstract IEnumerable<string> OutputArgumentFor(NPath objectFile);
        public virtual CompilationResult ShellResultToCompilationResult(Shell.ExecuteResult shellResult) => 
            new CompilationResult { 
                Duration = shellResult.Duration,
                Success = shellResult.ExitCode == 0,
                InterestingOutput = this.GetInterestingOutputFromCompilationShellResult(shellResult)
            };

        public virtual LinkerResult ShellResultToLinkerResult(Shell.ExecuteResult shellResult) => 
            new LinkerResult { 
                Duration = shellResult.Duration,
                Success = shellResult.ExitCode == 0,
                InterestingOutput = this.GetInterestingOutputFromLinkerShellResult(shellResult)
            };

        public virtual IEnumerable<Type> SupportedArchitectures() => 
            new Type[] { typeof(x86Architecture), typeof(x64Architecture) };

        public abstract IEnumerable<string> ToolChainDefines();
        public abstract IEnumerable<NPath> ToolChainIncludePaths();
        [DebuggerHidden]
        public virtual IEnumerable<NPath> ToolChainLibraryPaths() => 
            new <ToolChainLibraryPaths>c__Iterator0 { $PC = -2 };

        [DebuggerHidden]
        public virtual IEnumerable<string> ToolChainStaticLibraries() => 
            new <ToolChainStaticLibraries>c__Iterator1 { $PC = -2 };

        public Unity.IL2CPP.Common.Architecture Architecture { get; private set; }

        public Unity.IL2CPP.Building.BuildConfiguration BuildConfiguration { get; private set; }

        public abstract string DynamicLibraryExtension { get; }

        public virtual string MapFileParserFormat
        {
            get
            {
                throw new NotImplementedException("The base class version of this  property should never be called. Does the derived class need to override it?");
            }
        }

        public virtual bool SupportsMapFileParser =>
            false;

        [CompilerGenerated]
        private sealed class <ToolChainLibraryPaths>c__Iterator0 : IEnumerable, IEnumerable<NPath>, IEnumerator, IDisposable, IEnumerator<NPath>
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
                return new CppToolChain.<ToolChainLibraryPaths>c__Iterator0();
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator() => 
                this.System.Collections.Generic.IEnumerable<NiceIO.NPath>.GetEnumerator();

            NPath IEnumerator<NPath>.Current =>
                this.$current;

            object IEnumerator.Current =>
                this.$current;
        }

        [CompilerGenerated]
        private sealed class <ToolChainStaticLibraries>c__Iterator1 : IEnumerable, IEnumerable<string>, IEnumerator, IDisposable, IEnumerator<string>
        {
            internal string $current;
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
            IEnumerator<string> IEnumerable<string>.GetEnumerator()
            {
                if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
                {
                    return this;
                }
                return new CppToolChain.<ToolChainStaticLibraries>c__Iterator1();
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator() => 
                this.System.Collections.Generic.IEnumerable<string>.GetEnumerator();

            string IEnumerator<string>.Current =>
                this.$current;

            object IEnumerator.Current =>
                this.$current;
        }
    }
}

