namespace UnityEditor.Sprites
{
    using System;

    internal class TightPackerPolicy : DefaultPackerPolicy
    {
        protected override bool AllowRotationFlipping =>
            false;

        protected override bool AllowTightWhenTagged =>
            false;

        protected override string TagPrefix =>
            "[RECT]";
    }
}

