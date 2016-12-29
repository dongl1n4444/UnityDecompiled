namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class KeyIdentifier
    {
        public EditorCurveBinding binding;
        public AnimationCurve curve;
        public int curveId;
        public int key;

        public KeyIdentifier(AnimationCurve _curve, int _curveId, int _keyIndex)
        {
            this.curve = _curve;
            this.curveId = _curveId;
            this.key = _keyIndex;
        }

        public KeyIdentifier(AnimationCurve _curve, int _curveId, int _keyIndex, EditorCurveBinding _binding)
        {
            this.curve = _curve;
            this.curveId = _curveId;
            this.key = _keyIndex;
            this.binding = _binding;
        }

        public Keyframe keyframe =>
            this.curve[this.key];
    }
}

