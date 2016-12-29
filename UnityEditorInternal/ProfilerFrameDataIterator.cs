namespace UnityEditorInternal
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public sealed class ProfilerFrameDataIterator
    {
        private IntPtr m_Ptr;

        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern ProfilerFrameDataIterator();
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public extern void Dispose();
        ~ProfilerFrameDataIterator()
        {
            this.Dispose();
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern bool GetAllocationCallstack(ref string resolvedStack);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern double GetFrameStartS(int frame);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern int GetGroupCount(int frame);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern string GetGroupName();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern bool GetIsNativeAllocation();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern int GetThreadCount(int frame);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern string GetThreadName();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern bool Next(bool enterChildren);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void SetRoot(int frame, int threadIdx);

        public int depth { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        public float durationMS { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        public float frameTimeMS { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        public int group { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        [Obsolete("Use instanceId instead (UnityUpgradable) -> instanceId")]
        public int id =>
            this.instanceId;

        public int instanceId { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        public string name { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        public string path { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        public int sampleId { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        public float startTimeMS { [MethodImpl(MethodImplOptions.InternalCall)] get; }
    }
}

