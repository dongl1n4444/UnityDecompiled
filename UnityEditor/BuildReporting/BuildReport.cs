namespace UnityEditor.BuildReporting
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using UnityEditor;
    using UnityEngine;

    internal sealed class BuildReport : Object
    {
        public event Action<BuildReport> Changed;

        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void AddAppendix(Object obj);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void AddFile(string path, string role);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void AddFilesRecursive(string rootDir, string role);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void AddMessage(LogType messageType, string message);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void BeginBuildStep(string stepName);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void BeginBuildStepNoTiming(string stepName);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void DeleteFile(string path);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void DeleteFilesRecursive(string rootDir);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern Object[] GetAllAppendices();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern Object[] GetAppendices(Type type);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern Object[] GetAppendicesByClassID(int classID);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern BuildReport GetLatestReport();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void RelocateFiles(string originalPathPrefix, string newPathPrefix);
        public void SendChanged()
        {
            if (this.Changed != null)
            {
                this.Changed(this);
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern string SummarizeErrors();

        public BuildOptions buildOptions { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        public BuildTarget buildTarget { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        public uint crc { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        public string outputPath { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        public bool succeeded { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        public int totalErrors { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        public long totalSize { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        public long totalTimeMS { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        public int totalWarnings { [MethodImpl(MethodImplOptions.InternalCall)] get; }
    }
}

