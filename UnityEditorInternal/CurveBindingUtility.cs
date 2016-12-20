namespace UnityEditorInternal
{
    using System;
    using UnityEditor;
    using UnityEngine;

    internal static class CurveBindingUtility
    {
        private static GameObject s_Root;

        public static object GetCurrentValue(AnimationWindowState state, AnimationWindowCurve curve)
        {
            if (AnimationMode.InAnimationMode() && (curve.rootGameObject != null))
            {
                return AnimationWindowUtility.GetCurrentValue(curve.rootGameObject, curve.binding);
            }
            return curve.Evaluate(state.currentTime - curve.timeOffset);
        }

        public static object GetCurrentValue(GameObject rootGameObject, EditorCurveBinding curveBinding)
        {
            if (rootGameObject != null)
            {
                return AnimationWindowUtility.GetCurrentValue(rootGameObject, curveBinding);
            }
            if (curveBinding.isPPtrCurve)
            {
                return null;
            }
            return 0f;
        }
    }
}

