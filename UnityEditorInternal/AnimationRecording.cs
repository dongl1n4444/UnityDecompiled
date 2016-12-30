namespace UnityEditorInternal
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEditor;
    using UnityEngine;

    internal class AnimationRecording
    {
        [CompilerGenerated]
        private static Comparison<AnimationWindowKeyframe> <>f__am$cache0;
        private const string kLocalEulerAnglesHint = "m_LocalEulerAnglesHint";
        private const string kLocalRotation = "m_LocalRotation";

        private static void AddKey(IAnimationRecordingState state, EditorCurveBinding binding, Type type, PropertyModification modification)
        {
            GameObject activeRootGameObject = state.activeRootGameObject;
            AnimationClip activeAnimationClip = state.activeAnimationClip;
            if ((activeAnimationClip.hideFlags & HideFlags.NotEditable) == HideFlags.None)
            {
                AnimationWindowCurve curve = new AnimationWindowCurve(activeAnimationClip, binding, type);
                object currentValue = CurveBindingUtility.GetCurrentValue(activeRootGameObject, binding);
                if (curve.length == 0)
                {
                    object outObject = null;
                    if (!ValueFromPropertyModification(modification, binding, out outObject))
                    {
                        outObject = currentValue;
                    }
                    if ((state.currentFrame != 0) && state.addZeroFrame)
                    {
                        AnimationWindowUtility.AddKeyframeToCurve(curve, outObject, type, AnimationKeyTime.Frame(0, activeAnimationClip.frameRate));
                    }
                }
                AnimationWindowUtility.AddKeyframeToCurve(curve, currentValue, type, AnimationKeyTime.Frame(state.currentFrame, activeAnimationClip.frameRate));
                state.SaveCurve(curve);
            }
        }

        private static void AddRotationKey(IAnimationRecordingState state, EditorCurveBinding binding, Type type, Vector3 previousEulerAngles, Vector3 currentEulerAngles)
        {
            AnimationClip activeAnimationClip = state.activeAnimationClip;
            if ((activeAnimationClip.hideFlags & HideFlags.NotEditable) == HideFlags.None)
            {
                EditorCurveBinding[] bindingArray = RotationCurveInterpolation.RemapAnimationBindingForRotationAddKey(binding, activeAnimationClip);
                for (int i = 0; i < 3; i++)
                {
                    AnimationWindowCurve curve = new AnimationWindowCurve(activeAnimationClip, bindingArray[i], type);
                    if ((curve.length == 0) && (state.currentFrame != 0))
                    {
                        AnimationWindowUtility.AddKeyframeToCurve(curve, previousEulerAngles[i], type, AnimationKeyTime.Frame(0, activeAnimationClip.frameRate));
                    }
                    AnimationWindowUtility.AddKeyframeToCurve(curve, currentEulerAngles[i], type, AnimationKeyTime.Frame(state.currentFrame, activeAnimationClip.frameRate));
                    state.SaveCurve(curve);
                }
            }
        }

        private static void AddRotationPropertyModification(IAnimationRecordingState state, EditorCurveBinding baseBinding, UndoPropertyModification modification)
        {
            if (modification.previousValue != null)
            {
                EditorCurveBinding binding = baseBinding;
                binding.propertyName = modification.previousValue.propertyPath;
                AnimationMode.AddPropertyModification(binding, modification.previousValue, modification.keepPrefabOverride);
            }
        }

        private static void CollectRotationModifications(IAnimationRecordingState state, ref UndoPropertyModification[] modifications, ref Dictionary<object, RotationModification> rotationModifications)
        {
            List<UndoPropertyModification> list = new List<UndoPropertyModification>();
            foreach (UndoPropertyModification modification in modifications)
            {
                EditorCurveBinding binding = new EditorCurveBinding();
                PropertyModification previousValue = modification.previousValue;
                AnimationUtility.PropertyModificationToEditorCurveBinding(previousValue, state.activeRootGameObject, out binding);
                if (binding.propertyName.StartsWith("m_LocalRotation"))
                {
                    RotationModification modification3;
                    if (!rotationModifications.TryGetValue(previousValue.target, out modification3))
                    {
                        modification3 = new RotationModification();
                        rotationModifications[previousValue.target] = modification3;
                    }
                    if (binding.propertyName.EndsWith("x"))
                    {
                        modification3.x = modification;
                    }
                    else if (binding.propertyName.EndsWith("y"))
                    {
                        modification3.y = modification;
                    }
                    else if (binding.propertyName.EndsWith("z"))
                    {
                        modification3.z = modification;
                    }
                    else if (binding.propertyName.EndsWith("w"))
                    {
                        modification3.w = modification;
                    }
                    modification3.lastQuatModification = modification;
                }
                else if (previousValue.propertyPath.StartsWith("m_LocalEulerAnglesHint"))
                {
                    RotationModification modification4;
                    if (!rotationModifications.TryGetValue(previousValue.target, out modification4))
                    {
                        modification4 = new RotationModification();
                        rotationModifications[previousValue.target] = modification4;
                    }
                    modification4.useEuler = true;
                    if (previousValue.propertyPath.EndsWith("x"))
                    {
                        modification4.eulerX = modification;
                    }
                    else if (previousValue.propertyPath.EndsWith("y"))
                    {
                        modification4.eulerY = modification;
                    }
                    else if (previousValue.propertyPath.EndsWith("z"))
                    {
                        modification4.eulerZ = modification;
                    }
                }
                else
                {
                    list.Add(modification);
                }
            }
            if (rotationModifications.Count > 0)
            {
                modifications = list.ToArray();
            }
        }

        private static PropertyModification FindPropertyModification(GameObject root, UndoPropertyModification[] modifications, EditorCurveBinding binding)
        {
            for (int i = 0; i < modifications.Length; i++)
            {
                EditorCurveBinding binding2;
                AnimationUtility.PropertyModificationToEditorCurveBinding(modifications[i].previousValue, root, out binding2);
                if (binding2 == binding)
                {
                    return modifications[i].previousValue;
                }
            }
            return null;
        }

        private static bool HasAnyRecordableModifications(GameObject root, UndoPropertyModification[] modifications)
        {
            for (int i = 0; i < modifications.Length; i++)
            {
                EditorCurveBinding binding;
                if (AnimationUtility.PropertyModificationToEditorCurveBinding(modifications[i].previousValue, root, out binding) != null)
                {
                    return true;
                }
            }
            return false;
        }

        public static UndoPropertyModification[] Process(IAnimationRecordingState state, UndoPropertyModification[] modifications)
        {
            GameObject activeRootGameObject = state.activeRootGameObject;
            if (activeRootGameObject == null)
            {
                return modifications;
            }
            AnimationClip activeAnimationClip = state.activeAnimationClip;
            Animator component = activeRootGameObject.GetComponent<Animator>();
            if (!HasAnyRecordableModifications(activeRootGameObject, modifications))
            {
                return modifications;
            }
            ProcessRotationModifications(state, ref modifications);
            List<UndoPropertyModification> list = new List<UndoPropertyModification>();
            for (int i = 0; i < modifications.Length; i++)
            {
                EditorCurveBinding binding = new EditorCurveBinding();
                PropertyModification previousValue = modifications[i].previousValue;
                Type type = AnimationUtility.PropertyModificationToEditorCurveBinding(previousValue, activeRootGameObject, out binding);
                if (type != null)
                {
                    if (((component != null) && component.isHuman) && ((binding.type == typeof(Transform)) && component.IsBoneTransform(previousValue.target as Transform)))
                    {
                        Debug.LogWarning("Keyframing for humanoid rig is not supported!", previousValue.target as Transform);
                    }
                    else
                    {
                        AnimationMode.AddPropertyModification(binding, previousValue, modifications[i].keepPrefabOverride);
                        EditorCurveBinding[] bindingArray = RotationCurveInterpolation.RemapAnimationBindingForAddKey(binding, activeAnimationClip);
                        if (bindingArray != null)
                        {
                            for (int j = 0; j < bindingArray.Length; j++)
                            {
                                AddKey(state, bindingArray[j], type, FindPropertyModification(activeRootGameObject, modifications, bindingArray[j]));
                            }
                        }
                        else
                        {
                            AddKey(state, binding, type, previousValue);
                        }
                    }
                }
                else
                {
                    list.Add(modifications[i]);
                }
            }
            return list.ToArray();
        }

        private static void ProcessRotationModifications(IAnimationRecordingState state, ref UndoPropertyModification[] modifications)
        {
            Dictionary<object, RotationModification> rotationModifications = new Dictionary<object, RotationModification>();
            CollectRotationModifications(state, ref modifications, ref rotationModifications);
            foreach (KeyValuePair<object, RotationModification> pair in rotationModifications)
            {
                RotationModification modification = pair.Value;
                Transform key = pair.Key as Transform;
                if (key != null)
                {
                    EditorCurveBinding binding = new EditorCurveBinding();
                    Type type = AnimationUtility.PropertyModificationToEditorCurveBinding(modification.lastQuatModification.currentValue, state.activeRootGameObject, out binding);
                    if (type != null)
                    {
                        object obj2;
                        object obj3;
                        object obj4;
                        object obj5;
                        AddRotationPropertyModification(state, binding, modification.x);
                        AddRotationPropertyModification(state, binding, modification.y);
                        AddRotationPropertyModification(state, binding, modification.z);
                        AddRotationPropertyModification(state, binding, modification.w);
                        Quaternion localRotation = key.localRotation;
                        Quaternion q = key.localRotation;
                        if (ValueFromPropertyModification(modification.x.previousValue, binding, out obj2))
                        {
                            localRotation.x = (float) obj2;
                        }
                        if (ValueFromPropertyModification(modification.y.previousValue, binding, out obj3))
                        {
                            localRotation.y = (float) obj3;
                        }
                        if (ValueFromPropertyModification(modification.z.previousValue, binding, out obj4))
                        {
                            localRotation.z = (float) obj4;
                        }
                        if (ValueFromPropertyModification(modification.w.previousValue, binding, out obj5))
                        {
                            localRotation.w = (float) obj5;
                        }
                        if (ValueFromPropertyModification(modification.x.currentValue, binding, out obj2))
                        {
                            q.x = (float) obj2;
                        }
                        if (ValueFromPropertyModification(modification.y.currentValue, binding, out obj3))
                        {
                            q.y = (float) obj3;
                        }
                        if (ValueFromPropertyModification(modification.z.currentValue, binding, out obj4))
                        {
                            q.z = (float) obj4;
                        }
                        if (ValueFromPropertyModification(modification.w.currentValue, binding, out obj5))
                        {
                            q.w = (float) obj5;
                        }
                        if (modification.useEuler)
                        {
                            object obj6;
                            object obj7;
                            object obj8;
                            AddRotationPropertyModification(state, binding, modification.eulerX);
                            AddRotationPropertyModification(state, binding, modification.eulerY);
                            AddRotationPropertyModification(state, binding, modification.eulerZ);
                            Vector3 localEulerAngles = key.GetLocalEulerAngles(RotationOrder.OrderZXY);
                            Vector3 eulerHint = localEulerAngles;
                            if (ValueFromPropertyModification(modification.eulerX.previousValue, binding, out obj6))
                            {
                                localEulerAngles.x = (float) obj6;
                            }
                            if (ValueFromPropertyModification(modification.eulerY.previousValue, binding, out obj7))
                            {
                                localEulerAngles.y = (float) obj7;
                            }
                            if (ValueFromPropertyModification(modification.eulerZ.previousValue, binding, out obj8))
                            {
                                localEulerAngles.z = (float) obj8;
                            }
                            if (ValueFromPropertyModification(modification.eulerX.currentValue, binding, out obj6))
                            {
                                eulerHint.x = (float) obj6;
                            }
                            if (ValueFromPropertyModification(modification.eulerY.currentValue, binding, out obj7))
                            {
                                eulerHint.y = (float) obj7;
                            }
                            if (ValueFromPropertyModification(modification.eulerZ.currentValue, binding, out obj8))
                            {
                                eulerHint.z = (float) obj8;
                            }
                            localEulerAngles = AnimationUtility.GetClosestEuler(localRotation, localEulerAngles, RotationOrder.OrderZXY);
                            eulerHint = AnimationUtility.GetClosestEuler(q, eulerHint, RotationOrder.OrderZXY);
                            AddRotationKey(state, binding, type, localEulerAngles, eulerHint);
                        }
                        else
                        {
                            AddRotationKey(state, binding, type, localRotation.eulerAngles, q.eulerAngles);
                        }
                    }
                }
            }
        }

        public static void SaveModifiedCurve(AnimationWindowCurve curve, AnimationClip clip)
        {
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = (a, b) => a.time.CompareTo(b.time);
            }
            curve.m_Keyframes.Sort(<>f__am$cache0);
            if (curve.isPPtrCurve)
            {
                ObjectReferenceKeyframe[] keyframes = curve.ToObjectCurve();
                if (keyframes.Length == 0)
                {
                    keyframes = null;
                }
                AnimationUtility.SetObjectReferenceCurve(clip, curve.binding, keyframes);
            }
            else
            {
                AnimationCurve curve2 = curve.ToAnimationCurve();
                if (curve2.keys.Length == 0)
                {
                    curve2 = null;
                }
                else
                {
                    QuaternionCurveTangentCalculation.UpdateTangentsFromMode(curve2, clip, curve.binding);
                }
                AnimationUtility.SetEditorCurve(clip, curve.binding, curve2);
            }
        }

        private static bool ValueFromPropertyModification(PropertyModification modification, EditorCurveBinding binding, out object outObject)
        {
            float num;
            if (modification == null)
            {
                outObject = null;
                return false;
            }
            if (binding.isPPtrCurve)
            {
                outObject = modification.objectReference;
                return true;
            }
            if (float.TryParse(modification.value, out num))
            {
                outObject = num;
                return true;
            }
            outObject = null;
            return false;
        }

        internal class RotationModification
        {
            public UndoPropertyModification eulerX;
            public UndoPropertyModification eulerY;
            public UndoPropertyModification eulerZ;
            public UndoPropertyModification lastQuatModification;
            public bool useEuler = false;
            public UndoPropertyModification w;
            public UndoPropertyModification x;
            public UndoPropertyModification y;
            public UndoPropertyModification z;
        }
    }
}

