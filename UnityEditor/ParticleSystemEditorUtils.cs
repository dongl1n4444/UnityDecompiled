namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Internal;

    internal sealed class ParticleSystemEditorUtils
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern string CheckCircularReferences(ParticleSystem subEmitter);
        public static ParticleSystem GetRoot(ParticleSystem ps)
        {
            if (ps == null)
            {
                return null;
            }
            Transform parent = ps.transform;
            while ((parent.parent != null) && (parent.parent.gameObject.GetComponent<ParticleSystem>() != null))
            {
                parent = parent.parent;
            }
            return parent.gameObject.GetComponent<ParticleSystem>();
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void PerformCompleteResimulation();
        [ExcludeFromDocs]
        internal static void StopEffect()
        {
            bool clear = true;
            bool stop = true;
            StopEffect(stop, clear);
        }

        [ExcludeFromDocs]
        internal static void StopEffect(bool stop)
        {
            bool clear = true;
            StopEffect(stop, clear);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void StopEffect([DefaultValue("true")] bool stop, [DefaultValue("true")] bool clear);

        internal static bool editorIsPaused { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        internal static bool editorIsPlaying { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        internal static bool editorIsScrubbing { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        internal static float editorPlaybackTime { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        internal static bool editorResimulation { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        internal static float editorSimulationSpeed { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        internal static ParticleSystem lockedParticleSystem { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
    }
}

