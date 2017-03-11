namespace UnityEditor.BuildReporting
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Scripting;

    internal sealed class BuildReport : UnityEngine.Object
    {
        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event Action<BuildReport> Changed;

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void AddAppendix(UnityEngine.Object obj);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void AddFile(string path, string role);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void AddFilesRecursive(string rootDir, string role);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void AddMessage(LogType messageType, string message);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void BeginBuildStep(string stepName);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void BeginBuildStepNoTiming(string stepName);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void DeleteFile(string path);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void DeleteFilesRecursive(string rootDir);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal extern UnityEngine.Object[] GetAllAppendices();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern UnityEngine.Object[] GetAppendices(System.Type type);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal extern UnityEngine.Object[] GetAppendicesByClassID(int classID);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern BuildReport GetLatestReport();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void RelocateFiles(string originalPathPrefix, string newPathPrefix);
        public void SendChanged()
        {
            if (this.Changed != null)
            {
                this.Changed(this);
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern string SummarizeErrors();

        public string buildGUID { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        public BuildOptions buildOptions { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        public BuildTarget buildTarget { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        public uint crc { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        public string outputPath { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        public bool succeeded { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        public int totalErrors { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        public long totalSize { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        public long totalTimeMS { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        public int totalWarnings { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }
    }
}

