namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>AnimationMode is used by the AnimationWindow to store properties modified
    /// by the AnimationClip playback.</para>
    /// </summary>
    public sealed class AnimationMode
    {
        internal static AnimationModeChangedCallback animationModeChangedCallback;
        private static Color s_AnimatedPropertyColorDark = new Color(1f, 0.55f, 0.5f, 1f);
        private static Color s_AnimatedPropertyColorLight = new Color(1f, 0.65f, 0.6f, 1f);
        private static bool s_InAnimationPlaybackMode = false;

        /// <summary>
        /// <para>Marks a property as currently being animated.</para>
        /// </summary>
        /// <param name="binding">Description of the animation clip curve being modified.</param>
        /// <param name="modification">Object property being modified.</param>
        /// <param name="keepPrefabOverride">Indicates whether to retain modifications when the targeted object is an instance of prefab.</param>
        public static void AddPropertyModification(EditorCurveBinding binding, PropertyModification modification, bool keepPrefabOverride)
        {
            INTERNAL_CALL_AddPropertyModification(ref binding, modification, keepPrefabOverride);
        }

        /// <summary>
        /// <para>Initialise the start of the animation clip sampling.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void BeginSampling();
        /// <summary>
        /// <para>Finish the sampling of the animation clip.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void EndSampling();
        /// <summary>
        /// <para>Are we currently in AnimationMode?</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern bool InAnimationMode();
        internal static bool InAnimationPlaybackMode() => 
            s_InAnimationPlaybackMode;

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern void InitializePropertyModificationForGameObject(GameObject gameObject, AnimationClip clip);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern void InitializePropertyModificationForObject(UnityEngine.Object target, AnimationClip clip);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_AddPropertyModification(ref EditorCurveBinding binding, PropertyModification modification, bool keepPrefabOverride);
        private static void InternalAnimationModeChanged(bool newValue)
        {
            if (animationModeChangedCallback != null)
            {
                animationModeChangedCallback(newValue);
            }
        }

        /// <summary>
        /// <para>Is the specified property currently in animation mode and being animated?</para>
        /// </summary>
        /// <param name="target">The object to determine if it contained the animation.</param>
        /// <param name="propertyPath">The name of the animation to search for.</param>
        /// <returns>
        /// <para>Whether the property search is found or not.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern bool IsPropertyAnimated(UnityEngine.Object target, string propertyPath);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern void RevertPropertyModificationsForGameObject(GameObject gameObject);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern void RevertPropertyModificationsForObject(UnityEngine.Object target);
        /// <summary>
        /// <para>Samples an AnimationClip on the object and also records any modified
        /// properties in AnimationMode.</para>
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="clip"></param>
        /// <param name="time"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void SampleAnimationClip(GameObject gameObject, AnimationClip clip, float time);
        /// <summary>
        /// <para>Starts the animation mode.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void StartAnimationMode();
        internal static void StartAnimationPlaybackMode()
        {
            s_InAnimationPlaybackMode = true;
        }

        /// <summary>
        /// <para>Stops Animation mode, reverts all properties that were animated in animation mode.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void StopAnimationMode();
        internal static void StopAnimationPlaybackMode()
        {
            s_InAnimationPlaybackMode = false;
        }

        /// <summary>
        /// <para>The color used to show that a property is currently being animated.</para>
        /// </summary>
        public static Color animatedPropertyColor =>
            (!EditorGUIUtility.isProSkin ? s_AnimatedPropertyColorLight : s_AnimatedPropertyColorDark);

        internal delegate void AnimationModeChangedCallback(bool newValue);
    }
}

