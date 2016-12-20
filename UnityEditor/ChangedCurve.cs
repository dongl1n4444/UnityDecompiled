namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class ChangedCurve
    {
        public EditorCurveBinding binding;
        public AnimationCurve curve;
        public int curveId;

        public ChangedCurve(AnimationCurve curve, int curveId, EditorCurveBinding binding)
        {
            this.curve = curve;
            this.curveId = curveId;
            this.binding = binding;
        }

        public override int GetHashCode()
        {
            int hashCode = 0;
            hashCode = this.curve.GetHashCode();
            return ((0x21 * hashCode) + this.binding.GetHashCode());
        }
    }
}

