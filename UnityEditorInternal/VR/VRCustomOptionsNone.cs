namespace UnityEditorInternal.VR
{
    using System;
    using UnityEngine;

    internal class VRCustomOptionsNone : VRCustomOptions
    {
        public override void Draw(Rect rect)
        {
        }

        public override float GetHeight() => 
            0f;
    }
}

