namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>AnimationMode is used by the AnimationWindow to store properties modifed by the AnimationClip playback.</para>
    /// </summary>
    public sealed class AnimationMode
    {
        private static PrefColor s_AnimatedPropertyColor = new PrefColor("Animation/Property Animated", 0.82f, 0.97f, 1f, 1f, 0.54f, 0.85f, 1f, 1f);
        private static PrefColor s_CandidatePropertyColor = new PrefColor("Animation/Property Candidate", 1f, 0.7f, 0.6f, 1f, 1f, 0.67f, 0.43f, 1f);
        private static AnimationModeDriver s_DummyDriver;
        private static bool s_InAnimationPlaybackMode = false;
        private static bool s_InAnimationRecordMode = false;
        private static PrefColor s_RecordedPropertyColor = new PrefColor("Animation/Property Recorded", 1f, 0.6f, 0.6f, 1f, 1f, 0.5f, 0.5f, 1f);

        internal static void AddCandidate(EditorCurveBinding binding, PropertyModification modification, bool keepPrefabOverride)
        {
            if (!IsRecordingCandidates())
            {
                throw new InvalidOperationException("AnimationMode.AddCandidate may only be called when recording candidates.  See AnimationMode.StartCandidateRecording.");
            }
            Internal_AddCandidate(binding, modification, keepPrefabOverride);
        }

        public static void AddPropertyModification(EditorCurveBinding binding, PropertyModification modification, bool keepPrefabOverride)
        {
            if (!InAnimationMode())
            {
                throw new InvalidOperationException("AnimationMode.AddPropertyModification may only be called in animation mode.  See AnimationMode.StartAnimationMode.");
            }
            Internal_AddPropertyModification(binding, modification, keepPrefabOverride);
        }

        public static void BeginSampling()
        {
            if (!InAnimationMode())
            {
                throw new InvalidOperationException("AnimationMode.BeginSampling may only be called in animation mode.  See AnimationMode.StartAnimationMode.");
            }
            Internal_BeginSampling();
        }

        private static AnimationModeDriver DummyDriver()
        {
            if (s_DummyDriver == null)
            {
                s_DummyDriver = ScriptableObject.CreateInstance<AnimationModeDriver>();
                s_DummyDriver.name = "DummyDriver";
            }
            return s_DummyDriver;
        }

        public static void EndSampling()
        {
            if (!InAnimationMode())
            {
                throw new InvalidOperationException("AnimationMode.EndSampling may only be called in animation mode.  See AnimationMode.StartAnimationMode.");
            }
            Internal_EndSampling();
        }

        /// <summary>
        /// <para>Are we currently in AnimationMode.</para>
        /// </summary>
        public static bool InAnimationMode() => 
            Internal_InAnimationModeNoDriver();

        internal static bool InAnimationMode(UnityEngine.Object driver) => 
            Internal_InAnimationMode(driver);

        internal static bool InAnimationPlaybackMode() => 
            s_InAnimationPlaybackMode;

        internal static bool InAnimationRecording() => 
            s_InAnimationRecordMode;

        internal static void InitializePropertyModificationForGameObject(GameObject gameObject, AnimationClip clip)
        {
            if (!InAnimationMode())
            {
                throw new InvalidOperationException("AnimationMode.InitializePropertyModificationForGameObject may only be called in animation mode.  See AnimationMode.StartAnimationMode.");
            }
            Internal_InitializePropertyModificationForGameObject(gameObject, clip);
        }

        internal static void InitializePropertyModificationForObject(UnityEngine.Object target, AnimationClip clip)
        {
            if (!InAnimationMode())
            {
                throw new InvalidOperationException("AnimationMode.InitializePropertyModificationForObject may only be called in animation mode.  See AnimationMode.StartAnimationMode.");
            }
            Internal_InitializePropertyModificationForObject(target, clip);
        }

        private static void Internal_AddCandidate(EditorCurveBinding binding, PropertyModification modification, bool keepPrefabOverride)
        {
            INTERNAL_CALL_Internal_AddCandidate(ref binding, modification, keepPrefabOverride);
        }

        private static void Internal_AddPropertyModification(EditorCurveBinding binding, PropertyModification modification, bool keepPrefabOverride)
        {
            INTERNAL_CALL_Internal_AddPropertyModification(ref binding, modification, keepPrefabOverride);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void Internal_BeginSampling();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_Internal_AddCandidate(ref EditorCurveBinding binding, PropertyModification modification, bool keepPrefabOverride);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_Internal_AddPropertyModification(ref EditorCurveBinding binding, PropertyModification modification, bool keepPrefabOverride);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void Internal_EndSampling();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern bool Internal_InAnimationMode(UnityEngine.Object driver);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern bool Internal_InAnimationModeNoDriver();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void Internal_InitializePropertyModificationForGameObject(GameObject gameObject, AnimationClip clip);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void Internal_InitializePropertyModificationForObject(UnityEngine.Object target, AnimationClip clip);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void Internal_RevertPropertyModificationsForGameObject(GameObject gameObject);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void Internal_RevertPropertyModificationsForObject(UnityEngine.Object target);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void Internal_SampleAnimationClip(GameObject gameObject, AnimationClip clip, float time);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void Internal_SampleCandidateClip(GameObject gameObject, AnimationClip clip, float time);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void Internal_StartCandidateRecording(UnityEngine.Object driver);
        /// <summary>
        /// <para>Is the specified property currently in animation mode and being animated?</para>
        /// </summary>
        /// <param name="target"></param>
        /// <param name="propertyPath"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern bool IsPropertyAnimated(UnityEngine.Object target, string propertyPath);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern bool IsPropertyCandidate(UnityEngine.Object target, string propertyPath);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern bool IsRecordingCandidates();
        internal static void RevertPropertyModificationsForGameObject(GameObject gameObject)
        {
            if (!InAnimationMode())
            {
                throw new InvalidOperationException("AnimationMode.RevertPropertyModificationsForGameObject may only be called in animation mode.  See AnimationMode.StartAnimationMode.");
            }
            Internal_RevertPropertyModificationsForGameObject(gameObject);
        }

        internal static void RevertPropertyModificationsForObject(UnityEngine.Object target)
        {
            if (!InAnimationMode())
            {
                throw new InvalidOperationException("AnimationMode.RevertPropertyModificationsForObject may only be called in animation mode.  See AnimationMode.StartAnimationMode.");
            }
            Internal_RevertPropertyModificationsForObject(target);
        }

        /// <summary>
        /// <para>Samples an AnimationClip on the object and also records any modified properties in AnimationMode.</para>
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="clip"></param>
        /// <param name="time"></param>
        public static void SampleAnimationClip(GameObject gameObject, AnimationClip clip, float time)
        {
            if (!InAnimationMode())
            {
                throw new InvalidOperationException("AnimationMode.SampleAnimationClip may only be called in animation mode.  See AnimationMode.StartAnimationMode.");
            }
            Internal_SampleAnimationClip(gameObject, clip, time);
        }

        internal static void SampleCandidateClip(GameObject gameObject, AnimationClip clip, float time)
        {
            if (!IsRecordingCandidates())
            {
                throw new InvalidOperationException("AnimationMode.SampleCandidateClip may only be called when recording candidates.  See AnimationMode.StartAnimationMode.");
            }
            Internal_SampleCandidateClip(gameObject, clip, time);
        }

        /// <summary>
        /// <para>Starts the animation mode.</para>
        /// </summary>
        public static void StartAnimationMode()
        {
            StartAnimationMode(DummyDriver());
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern void StartAnimationMode(UnityEngine.Object driver);
        internal static void StartAnimationPlaybackMode()
        {
            s_InAnimationPlaybackMode = true;
        }

        internal static void StartAnimationRecording()
        {
            s_InAnimationRecordMode = true;
        }

        internal static void StartCandidateRecording(UnityEngine.Object driver)
        {
            if (!InAnimationMode())
            {
                throw new InvalidOperationException("AnimationMode.StartCandidateRecording may only be called in animation mode.  See AnimationMode.StartAnimationMode.");
            }
            Internal_StartCandidateRecording(driver);
        }

        /// <summary>
        /// <para>Stops Animation mode, reverts all properties that were animated in animation mode.</para>
        /// </summary>
        public static void StopAnimationMode()
        {
            StopAnimationMode(DummyDriver());
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern void StopAnimationMode(UnityEngine.Object driver);
        internal static void StopAnimationPlaybackMode()
        {
            s_InAnimationPlaybackMode = false;
        }

        internal static void StopAnimationRecording()
        {
            s_InAnimationRecordMode = false;
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern void StopCandidateRecording();

        /// <summary>
        /// <para>The color used to show that a property is currently being animated.</para>
        /// </summary>
        public static Color animatedPropertyColor =>
            ((Color) s_AnimatedPropertyColor);

        /// <summary>
        /// <para>The color used to show that an animated property has been modified.</para>
        /// </summary>
        public static Color candidatePropertyColor =>
            ((Color) s_CandidatePropertyColor);

        /// <summary>
        /// <para>The color used to show that an animated property automatically records changes in the animation clip.</para>
        /// </summary>
        public static Color recordedPropertyColor =>
            ((Color) s_RecordedPropertyColor);
    }
}

