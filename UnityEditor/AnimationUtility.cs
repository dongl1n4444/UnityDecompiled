namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Internal;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Editor utility functions for modifying animation clips.</para>
    /// </summary>
    public sealed class AnimationUtility
    {
        private static int kBrokenMask = 1;
        private static int kLeftTangentMask = 30;
        private static int kRightTangentMask = 480;
        /// <summary>
        /// <para>Triggered when an animation curve inside an animation clip has been modified.</para>
        /// </summary>
        public static OnCurveWasModified onCurveWasModified;

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern bool AmbiguousBinding(string path, int classID, Transform root);
        /// <summary>
        /// <para>Calculates path from root transform to target transform.</para>
        /// </summary>
        /// <param name="targetTransform"></param>
        /// <param name="root"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern string CalculateTransformPath(Transform targetTransform, Transform root);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void ConstrainToPolynomialCurve(AnimationCurve curve);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern bool CurveSupportsProcedural(AnimationCurve curve);
        /// <summary>
        /// <para>Retrieves all curves from a specific animation clip.</para>
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="includeCurveData"></param>
        [Obsolete("GetAllCurves is deprecated. Use GetCurveBindings and GetObjectReferenceCurveBindings instead."), ExcludeFromDocs]
        public static AnimationClipCurveData[] GetAllCurves(AnimationClip clip)
        {
            bool includeCurveData = true;
            return GetAllCurves(clip, includeCurveData);
        }

        /// <summary>
        /// <para>Retrieves all curves from a specific animation clip.</para>
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="includeCurveData"></param>
        [Obsolete("GetAllCurves is deprecated. Use GetCurveBindings and GetObjectReferenceCurveBindings instead.")]
        public static AnimationClipCurveData[] GetAllCurves(AnimationClip clip, [DefaultValue("true")] bool includeCurveData)
        {
            EditorCurveBinding[] curveBindings = GetCurveBindings(clip);
            AnimationClipCurveData[] dataArray = new AnimationClipCurveData[curveBindings.Length];
            for (int i = 0; i < dataArray.Length; i++)
            {
                dataArray[i] = new AnimationClipCurveData(curveBindings[i]);
                if (includeCurveData)
                {
                    dataArray[i].curve = GetEditorCurve(clip, curveBindings[i]);
                }
            }
            return dataArray;
        }

        /// <summary>
        /// <para>Returns all the animatable bindings that a specific game object has.</para>
        /// </summary>
        /// <param name="targetObject"></param>
        /// <param name="root"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern EditorCurveBinding[] GetAnimatableBindings(GameObject targetObject, GameObject root);
        /// <summary>
        /// <para>Returns the animated object that the binding is pointing to.</para>
        /// </summary>
        /// <param name="root"></param>
        /// <param name="binding"></param>
        public static Object GetAnimatedObject(GameObject root, EditorCurveBinding binding) => 
            INTERNAL_CALL_GetAnimatedObject(root, ref binding);

        /// <summary>
        /// <para>Returns the array of AnimationClips that are referenced in the Animation component.</para>
        /// </summary>
        /// <param name="component"></param>
        [Obsolete("GetAnimationClips(Animation) is deprecated. Use GetAnimationClips(GameObject) instead.")]
        public static AnimationClip[] GetAnimationClips(Animation component) => 
            GetAnimationClips(component.gameObject);

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern AnimationClip[] GetAnimationClips(GameObject gameObject);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern AnimationClipSettings GetAnimationClipSettings(AnimationClip clip);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern AnimationClipStats GetAnimationClipStats(AnimationClip clip);
        /// <summary>
        /// <para>Retrieves all animation events associated with the animation clip.</para>
        /// </summary>
        /// <param name="clip"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern AnimationEvent[] GetAnimationEvents(AnimationClip clip);
        internal static Vector3 GetClosestEuler(Quaternion q, Vector3 eulerHint, RotationOrder rotationOrder)
        {
            Vector3 vector;
            INTERNAL_CALL_GetClosestEuler(ref q, ref eulerHint, rotationOrder, out vector);
            return vector;
        }

        /// <summary>
        /// <para>Returns all the float curve bindings currently stored in the clip.</para>
        /// </summary>
        /// <param name="clip"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern EditorCurveBinding[] GetCurveBindings(AnimationClip clip);
        /// <summary>
        /// <para>Return the float curve that the binding is pointing to.</para>
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="relativePath"></param>
        /// <param name="type"></param>
        /// <param name="propertyName"></param>
        /// <param name="binding"></param>
        public static AnimationCurve GetEditorCurve(AnimationClip clip, EditorCurveBinding binding) => 
            INTERNAL_CALL_GetEditorCurve(clip, ref binding);

        /// <summary>
        /// <para>Return the float curve that the binding is pointing to.</para>
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="relativePath"></param>
        /// <param name="type"></param>
        /// <param name="propertyName"></param>
        /// <param name="binding"></param>
        [Obsolete("This overload is deprecated. Use the one with EditorCurveBinding instead.")]
        public static AnimationCurve GetEditorCurve(AnimationClip clip, string relativePath, Type type, string propertyName) => 
            GetEditorCurve(clip, EditorCurveBinding.FloatCurve(relativePath, type, propertyName));

        public static Type GetEditorCurveValueType(GameObject root, EditorCurveBinding binding) => 
            INTERNAL_CALL_GetEditorCurveValueType(root, ref binding);

        public static bool GetFloatValue(GameObject root, EditorCurveBinding binding, out float data) => 
            INTERNAL_CALL_GetFloatValue(root, ref binding, out data);

        [Obsolete("This overload is deprecated. Use the one with EditorCurveBinding instead.")]
        public static bool GetFloatValue(GameObject root, string relativePath, Type type, string propertyName, out float data) => 
            GetFloatValue(root, EditorCurveBinding.FloatCurve(relativePath, type, propertyName), out data);

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern bool GetGenerateMotionCurves(AnimationClip clip);
        internal static bool GetKeyBroken(Keyframe key) => 
            ((key.tangentMode & kBrokenMask) != 0);

        internal static TangentMode GetKeyLeftTangentMode(Keyframe key) => 
            ((TangentMode) ((key.tangentMode & kLeftTangentMask) >> 1));

        internal static TangentMode GetKeyRightTangentMode(Keyframe key) => 
            ((TangentMode) ((key.tangentMode & kRightTangentMask) >> 5));

        /// <summary>
        /// <para>Return the object reference curve that the binding is pointing to.</para>
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="binding"></param>
        public static ObjectReferenceKeyframe[] GetObjectReferenceCurve(AnimationClip clip, EditorCurveBinding binding) => 
            INTERNAL_CALL_GetObjectReferenceCurve(clip, ref binding);

        /// <summary>
        /// <para>Returns all the object reference curve bindings currently stored in the clip.</para>
        /// </summary>
        /// <param name="clip"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern EditorCurveBinding[] GetObjectReferenceCurveBindings(AnimationClip clip);
        public static bool GetObjectReferenceValue(GameObject root, EditorCurveBinding binding, out Object targetObject) => 
            INTERNAL_CALL_GetObjectReferenceValue(root, ref binding, out targetObject);

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern EditorCurveBinding[] GetScriptableObjectAnimatableBindings(ScriptableObject scriptableObject);
        internal static Type GetScriptableObjectEditorCurveValueType(ScriptableObject scriptableObject, EditorCurveBinding binding) => 
            INTERNAL_CALL_GetScriptableObjectEditorCurveValueType(scriptableObject, ref binding);

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern bool HasGenericRootTransform(AnimationClip clip);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern bool HasMotionCurves(AnimationClip clip);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern bool HasMotionFloatCurves(AnimationClip clip);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern bool HasRootCurves(AnimationClip clip);
        [Obsolete("Use AnimationMode.InAnimationMode instead")]
        public static bool InAnimationMode() => 
            AnimationMode.InAnimationMode();

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void Internal_CalculateAutoTangent(AnimationCurve curve, int index);
        private static float Internal_CalculateLinearTangent(AnimationCurve curve, int index, int toIndex)
        {
            Keyframe keyframe = curve[index];
            Keyframe keyframe2 = curve[toIndex];
            float a = keyframe.time - keyframe2.time;
            if (Mathf.Approximately(a, 0f))
            {
                return 0f;
            }
            Keyframe keyframe3 = curve[index];
            Keyframe keyframe4 = curve[toIndex];
            return ((keyframe3.value - keyframe4.value) / a);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern Object INTERNAL_CALL_GetAnimatedObject(GameObject root, ref EditorCurveBinding binding);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_GetClosestEuler(ref Quaternion q, ref Vector3 eulerHint, RotationOrder rotationOrder, out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern AnimationCurve INTERNAL_CALL_GetEditorCurve(AnimationClip clip, ref EditorCurveBinding binding);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern Type INTERNAL_CALL_GetEditorCurveValueType(GameObject root, ref EditorCurveBinding binding);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern bool INTERNAL_CALL_GetFloatValue(GameObject root, ref EditorCurveBinding binding, out float data);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern ObjectReferenceKeyframe[] INTERNAL_CALL_GetObjectReferenceCurve(AnimationClip clip, ref EditorCurveBinding binding);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern bool INTERNAL_CALL_GetObjectReferenceValue(GameObject root, ref EditorCurveBinding binding, out Object targetObject);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern Type INTERNAL_CALL_GetScriptableObjectEditorCurveValueType(ScriptableObject scriptableObject, ref EditorCurveBinding binding);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_Internal_SetEditorCurve(AnimationClip clip, ref EditorCurveBinding binding, AnimationCurve curve, bool syncEditorCurve);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_Internal_SetObjectReferenceCurve(AnimationClip clip, ref EditorCurveBinding binding, ObjectReferenceKeyframe[] keyframes);
        [RequiredByNativeCode]
        private static void Internal_CallAnimationClipAwake(AnimationClip clip)
        {
            if (onCurveWasModified != null)
            {
                onCurveWasModified(clip, new EditorCurveBinding(), CurveModifiedType.ClipModified);
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void Internal_SetAnimationEvents(AnimationClip clip, AnimationEvent[] events);
        private static void Internal_SetEditorCurve(AnimationClip clip, EditorCurveBinding binding, AnimationCurve curve, bool syncEditorCurve)
        {
            INTERNAL_CALL_Internal_SetEditorCurve(clip, ref binding, curve, syncEditorCurve);
        }

        private static void Internal_SetObjectReferenceCurve(AnimationClip clip, EditorCurveBinding binding, ObjectReferenceKeyframe[] keyframes)
        {
            INTERNAL_CALL_Internal_SetObjectReferenceCurve(clip, ref binding, keyframes);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void Internal_SyncEditorCurves(AnimationClip clip);
        private static void Internal_UpdateTangents(AnimationCurve curve, int index)
        {
            if ((index >= 0) && (index < curve.length))
            {
                Keyframe key = curve[index];
                if ((GetKeyLeftTangentMode(key) == TangentMode.Linear) && (index >= 1))
                {
                    key.inTangent = Internal_CalculateLinearTangent(curve, index, index - 1);
                    curve.MoveKey(index, key);
                }
                if ((GetKeyRightTangentMode(key) == TangentMode.Linear) && ((index + 1) < curve.length))
                {
                    key.outTangent = Internal_CalculateLinearTangent(curve, index, index + 1);
                    curve.MoveKey(index, key);
                }
                if ((GetKeyLeftTangentMode(key) == TangentMode.ClampedAuto) || (GetKeyRightTangentMode(key) == TangentMode.ClampedAuto))
                {
                    Internal_CalculateAutoTangent(curve, index);
                }
                if ((GetKeyLeftTangentMode(key) == TangentMode.Auto) || (GetKeyRightTangentMode(key) == TangentMode.Auto))
                {
                    curve.SmoothTangents(index, 0f);
                }
                if (((GetKeyLeftTangentMode(key) == TangentMode.Free) && (GetKeyRightTangentMode(key) == TangentMode.Free)) && !GetKeyBroken(key))
                {
                    key.outTangent = key.inTangent;
                    curve.MoveKey(index, key);
                }
                if (GetKeyLeftTangentMode(key) == TangentMode.Constant)
                {
                    key.inTangent = float.PositiveInfinity;
                    curve.MoveKey(index, key);
                }
                if (GetKeyRightTangentMode(key) == TangentMode.Constant)
                {
                    key.outTangent = float.PositiveInfinity;
                    curve.MoveKey(index, key);
                }
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern bool IsValidPolynomialCurve(AnimationCurve curve);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern Type PropertyModificationToEditorCurveBinding(PropertyModification modification, GameObject gameObject, out EditorCurveBinding binding);
        /// <summary>
        /// <para>Set the additive reference pose from referenceClip at time for animation clip clip.</para>
        /// </summary>
        /// <param name="clip">The animation clip to be used.</param>
        /// <param name="referenceClip">The animation clip containing the reference pose.</param>
        /// <param name="time">Time that defines the reference pose in referenceClip.</param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void SetAdditiveReferencePose(AnimationClip clip, AnimationClip referenceClip, float time);
        /// <summary>
        /// <para>Sets the array of AnimationClips to be referenced in the Animation component.</para>
        /// </summary>
        /// <param name="animation"></param>
        /// <param name="clips"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void SetAnimationClips(Animation animation, AnimationClip[] clips);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void SetAnimationClipSettings(AnimationClip clip, AnimationClipSettings srcClipInfo);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern void SetAnimationClipSettingsNoDirty(AnimationClip clip, AnimationClipSettings srcClipInfo);
        /// <summary>
        /// <para>Replaces all animation events in the animation clip.</para>
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="events"></param>
        public static void SetAnimationEvents(AnimationClip clip, AnimationEvent[] events)
        {
            if (clip == null)
            {
                throw new ArgumentNullException("clip");
            }
            if (events == null)
            {
                throw new ArgumentNullException("events");
            }
            Internal_SetAnimationEvents(clip, events);
            if (onCurveWasModified != null)
            {
                onCurveWasModified(clip, new EditorCurveBinding(), CurveModifiedType.ClipModified);
            }
        }

        [Obsolete("SetAnimationType is no longer supported", true)]
        public static void SetAnimationType(AnimationClip clip, ModelImporterAnimationType type)
        {
        }

        /// <summary>
        /// <para>Adds, modifies or removes an editor float curve in a given clip.</para>
        /// </summary>
        /// <param name="clip">The animation clip to which the curve will be added.</param>
        /// <param name="binding">The bindings which defines the path and the property of the curve.</param>
        /// <param name="curve">The curve to add. Setting this to null will remove the curve.</param>
        public static void SetEditorCurve(AnimationClip clip, EditorCurveBinding binding, AnimationCurve curve)
        {
            Internal_SetEditorCurve(clip, binding, curve, true);
            if (onCurveWasModified != null)
            {
                onCurveWasModified(clip, binding, (curve == null) ? CurveModifiedType.CurveDeleted : CurveModifiedType.CurveModified);
            }
        }

        [Obsolete("This overload is deprecated. Use the one with EditorCurveBinding instead.")]
        public static void SetEditorCurve(AnimationClip clip, string relativePath, Type type, string propertyName, AnimationCurve curve)
        {
            SetEditorCurve(clip, EditorCurveBinding.FloatCurve(relativePath, type, propertyName), curve);
        }

        internal static void SetEditorCurves(AnimationClip clip, EditorCurveBinding[] bindings, AnimationCurve[] curves)
        {
            if (clip == null)
            {
                throw new ArgumentNullException("clip");
            }
            if (curves == null)
            {
                throw new ArgumentNullException("curves");
            }
            if (bindings == null)
            {
                throw new ArgumentNullException("bindings");
            }
            if (bindings.Length != curves.Length)
            {
                throw new ArgumentException("bindings and curves array sizes do not match");
            }
            for (int i = 0; i < bindings.Length; i++)
            {
                Internal_SetEditorCurve(clip, bindings[i], curves[i], false);
                if (onCurveWasModified != null)
                {
                    onCurveWasModified(clip, bindings[i], (curves[i] == null) ? CurveModifiedType.CurveDeleted : CurveModifiedType.CurveModified);
                }
            }
            Internal_SyncEditorCurves(clip);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern void SetGenerateMotionCurves(AnimationClip clip, bool value);
        internal static void SetKeyBroken(ref Keyframe key, bool broken)
        {
            if (broken)
            {
                key.tangentMode |= kBrokenMask;
            }
            else
            {
                key.tangentMode &= ~kBrokenMask;
            }
        }

        /// <summary>
        /// <para>Change the specified keyframe broken tangent flag.</para>
        /// </summary>
        /// <param name="curve">The curve to modify.</param>
        /// <param name="index">Keyframe index.</param>
        /// <param name="broken">Broken flag.</param>
        public static void SetKeyBroken(AnimationCurve curve, int index, bool broken)
        {
            if (curve == null)
            {
                throw new ArgumentNullException("curve");
            }
            if ((index < 0) || (index >= curve.length))
            {
                throw new ArgumentException("Index out of bounds.");
            }
            Keyframe key = curve[index];
            SetKeyBroken(ref key, broken);
            curve.MoveKey(index, key);
            UpdateTangentsFromModeSurrounding(curve, index);
        }

        internal static void SetKeyLeftTangentMode(ref Keyframe key, TangentMode tangentMode)
        {
            key.tangentMode &= ~kLeftTangentMask;
            key.tangentMode |= ((int) tangentMode) << 1;
        }

        public static void SetKeyLeftTangentMode(AnimationCurve curve, int index, TangentMode tangentMode)
        {
            if (curve == null)
            {
                throw new ArgumentNullException("curve");
            }
            if ((index < 0) || (index >= curve.length))
            {
                throw new ArgumentException("Index out of bounds.");
            }
            Keyframe key = curve[index];
            if (tangentMode != TangentMode.Free)
            {
                SetKeyBroken(ref key, true);
            }
            SetKeyLeftTangentMode(ref key, tangentMode);
            curve.MoveKey(index, key);
            UpdateTangentsFromModeSurrounding(curve, index);
        }

        internal static void SetKeyRightTangentMode(ref Keyframe key, TangentMode tangentMode)
        {
            key.tangentMode &= ~kRightTangentMask;
            key.tangentMode |= ((int) tangentMode) << 5;
        }

        public static void SetKeyRightTangentMode(AnimationCurve curve, int index, TangentMode tangentMode)
        {
            if (curve == null)
            {
                throw new ArgumentNullException("curve");
            }
            if ((index < 0) || (index >= curve.length))
            {
                throw new ArgumentException("Index out of bounds.");
            }
            Keyframe key = curve[index];
            if (tangentMode != TangentMode.Free)
            {
                SetKeyBroken(ref key, true);
            }
            SetKeyRightTangentMode(ref key, tangentMode);
            curve.MoveKey(index, key);
            UpdateTangentsFromModeSurrounding(curve, index);
        }

        /// <summary>
        /// <para>Adds, modifies or removes an object reference curve in a given clip.</para>
        /// </summary>
        /// <param name="keyframes">Setting this to null will remove the curve.</param>
        /// <param name="clip"></param>
        /// <param name="binding"></param>
        public static void SetObjectReferenceCurve(AnimationClip clip, EditorCurveBinding binding, ObjectReferenceKeyframe[] keyframes)
        {
            Internal_SetObjectReferenceCurve(clip, binding, keyframes);
            if (onCurveWasModified != null)
            {
                onCurveWasModified(clip, binding, (keyframes == null) ? CurveModifiedType.CurveDeleted : CurveModifiedType.CurveModified);
            }
        }

        [Obsolete("Use AnimationMode.StartAnimationmode instead")]
        public static void StartAnimationMode(Object[] objects)
        {
            Debug.LogWarning("AnimationUtility.StartAnimationMode is deprecated. Use AnimationMode.StartAnimationMode with the new APIs. The objects passed to this function will no longer be reverted automatically. See AnimationMode.AddPropertyModification");
            AnimationMode.StartAnimationMode();
        }

        [Obsolete("Use AnimationMode.StopAnimationMode instead")]
        public static void StopAnimationMode()
        {
            AnimationMode.StopAnimationMode();
        }

        internal static void UpdateTangentsFromMode(AnimationCurve curve)
        {
            for (int i = 0; i < curve.length; i++)
            {
                Internal_UpdateTangents(curve, i);
            }
        }

        internal static void UpdateTangentsFromModeSurrounding(AnimationCurve curve, int index)
        {
            Internal_UpdateTangents(curve, index - 2);
            Internal_UpdateTangents(curve, index - 1);
            Internal_UpdateTangents(curve, index);
            Internal_UpdateTangents(curve, index + 1);
            Internal_UpdateTangents(curve, index + 2);
        }

        /// <summary>
        /// <para>Describes the type of modification that caused OnCurveWasModified to fire.</para>
        /// </summary>
        public enum CurveModifiedType
        {
            CurveDeleted,
            CurveModified,
            ClipModified
        }

        /// <summary>
        /// <para>Triggered when an animation curve inside an animation clip has been modified.</para>
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="binding"></param>
        /// <param name="deleted"></param>
        public delegate void OnCurveWasModified(AnimationClip clip, EditorCurveBinding binding, AnimationUtility.CurveModifiedType deleted);

        /// <summary>
        /// <para>Tangent constraints on Keyframe.</para>
        /// </summary>
        public enum TangentMode
        {
            Free,
            Auto,
            Linear,
            Constant,
            ClampedAuto
        }
    }
}

