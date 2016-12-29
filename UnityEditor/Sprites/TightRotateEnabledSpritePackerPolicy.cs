namespace UnityEditor.Sprites
{
    using System;

    internal class TightRotateEnabledSpritePackerPolicy : DefaultPackerPolicy
    {
        protected override bool AllowRotationFlipping =>
            true;

        protected override bool AllowTightWhenTagged =>
            false;

        protected override string TagPrefix =>
            "[RECT]";
    }
}

