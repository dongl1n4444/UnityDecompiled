namespace UnityEditor.Scripting.ScriptCompilation
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using UnityEditor.Scripting;
    using UnityEditor.Scripting.Compilers;

    internal class CompilationTask
    {
        [CompilerGenerated]
        private static Func<CompilerMessage, bool> <>f__am$cache0;
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private bool <CompileErrors>k__BackingField;
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private bool <Stopped>k__BackingField;
        private BuildFlags buildFlags;
        private string buildOutputDirectory;
        private int compilePhase = 0;
        private Dictionary<ScriptAssembly, ScriptCompilerBase> compilerTasks = new Dictionary<ScriptAssembly, ScriptCompilerBase>();
        private HashSet<ScriptAssembly> pendingAssemblies;
        private Dictionary<ScriptAssembly, CompilerMessage[]> processedAssemblies = new Dictionary<ScriptAssembly, CompilerMessage[]>();

        [field: DebuggerBrowsable(0), CompilerGenerated]
        public event OnCompilationFinishedDelegate OnCompilationFinished;

        [field: DebuggerBrowsable(0), CompilerGenerated]
        public event OnCompilationStartedDelegate OnCompilationStarted;

        public CompilationTask(ScriptAssembly[] scriptAssemblies, string buildOutputDirectory, BuildFlags buildFlags)
        {
            this.pendingAssemblies = new HashSet<ScriptAssembly>(scriptAssemblies);
            this.CompileErrors = false;
            this.buildOutputDirectory = buildOutputDirectory;
            this.buildFlags = buildFlags;
        }

        ~CompilationTask()
        {
            this.Stop();
        }

        public bool Poll()
        {
            if (this.Stopped)
            {
                return true;
            }
            Dictionary<ScriptAssembly, ScriptCompilerBase> dictionary = null;
            foreach (KeyValuePair<ScriptAssembly, ScriptCompilerBase> pair in this.compilerTasks)
            {
                ScriptCompilerBase base2 = pair.Value;
                if (base2.Poll())
                {
                    if (dictionary == null)
                    {
                        dictionary = new Dictionary<ScriptAssembly, ScriptCompilerBase>();
                    }
                    ScriptAssembly key = pair.Key;
                    dictionary.Add(key, base2);
                }
            }
            if (dictionary != null)
            {
                foreach (KeyValuePair<ScriptAssembly, ScriptCompilerBase> pair2 in dictionary)
                {
                    ScriptAssembly assembly = pair2.Key;
                    ScriptCompilerBase base3 = pair2.Value;
                    CompilerMessage[] compilerMessages = base3.GetCompilerMessages();
                    if (this.OnCompilationFinished != null)
                    {
                        this.OnCompilationFinished(assembly, compilerMessages);
                    }
                    this.processedAssemblies.Add(assembly, compilerMessages);
                    if (!this.CompileErrors)
                    {
                        if (<>f__am$cache0 == null)
                        {
                            <>f__am$cache0 = m => m.type == CompilerMessageType.Error;
                        }
                        this.CompileErrors = Enumerable.Any<CompilerMessage>(compilerMessages, <>f__am$cache0);
                    }
                    this.compilerTasks.Remove(assembly);
                    base3.Dispose();
                }
            }
            if (this.CompileErrors)
            {
                if (this.pendingAssemblies.Count > 0)
                {
                    foreach (ScriptAssembly assembly3 in this.pendingAssemblies)
                    {
                        this.processedAssemblies.Add(assembly3, new CompilerMessage[0]);
                    }
                    this.pendingAssemblies.Clear();
                }
                return (this.compilerTasks.Count == 0);
            }
            if ((this.compilerTasks.Count == 0) || ((dictionary != null) && (dictionary.Count > 0)))
            {
                this.QueuePendingAssemblies();
            }
            return ((this.pendingAssemblies.Count == 0) && (this.compilerTasks.Count == 0));
        }

        private void QueuePendingAssemblies()
        {
            if (this.pendingAssemblies.Count != 0)
            {
                List<ScriptAssembly> list = null;
                foreach (ScriptAssembly assembly in this.pendingAssemblies)
                {
                    bool flag = true;
                    foreach (ScriptAssembly assembly2 in assembly.ScriptAssemblyReferences)
                    {
                        if (!this.processedAssemblies.ContainsKey(assembly2))
                        {
                            flag = false;
                            break;
                        }
                    }
                    if (flag)
                    {
                        if (list == null)
                        {
                            list = new List<ScriptAssembly>();
                        }
                        list.Add(assembly);
                    }
                }
                if (list != null)
                {
                    bool buildingForEditor = (this.buildFlags & BuildFlags.BuildingForEditor) == BuildFlags.BuildingForEditor;
                    foreach (ScriptAssembly assembly3 in list)
                    {
                        this.pendingAssemblies.Remove(assembly3);
                        MonoIsland island = assembly3.ToMonoIsland(this.buildFlags, this.buildOutputDirectory);
                        ScriptCompilerBase base2 = ScriptCompilers.CreateCompilerInstance(island, buildingForEditor, island._target, assembly3.RunUpdater);
                        this.compilerTasks.Add(assembly3, base2);
                        base2.BeginCompiling();
                        if (this.OnCompilationStarted != null)
                        {
                            this.OnCompilationStarted(assembly3, this.compilePhase);
                        }
                    }
                    this.compilePhase++;
                }
            }
        }

        public void Stop()
        {
            if (!this.Stopped)
            {
                foreach (KeyValuePair<ScriptAssembly, ScriptCompilerBase> pair in this.compilerTasks)
                {
                    pair.Value.Dispose();
                }
                this.compilerTasks.Clear();
                this.Stopped = true;
            }
        }

        public bool CompileErrors { get; private set; }

        public Dictionary<ScriptAssembly, CompilerMessage[]> CompilerMessages =>
            this.processedAssemblies;

        public bool IsCompiling =>
            ((this.pendingAssemblies.Count > 0) || (this.compilerTasks.Count > 0));

        public bool Stopped { get; private set; }

        public delegate void OnCompilationFinishedDelegate(ScriptAssembly assembly, CompilerMessage[] messages);

        public delegate void OnCompilationStartedDelegate(ScriptAssembly assembly, int phase);
    }
}

