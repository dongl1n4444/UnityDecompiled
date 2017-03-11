namespace UnityEditorInternal.VR
{
    using System;
    using UnityEngine;

    internal class VRCustomOptionsNone : VRCustomOptions
    {
        public override Rect Draw(Rect rect) => 
            rect;

        public override float GetHeight() => 
            0f;
    }
}

