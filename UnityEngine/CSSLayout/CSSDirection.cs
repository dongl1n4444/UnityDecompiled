namespace UnityEngine.CSSLayout
{
    using System;

    internal enum CSSDirection
    {
        Inherit = 0,
        [Obsolete("Use LTR instead")]
        LeftToRight = 1,
        LTR = 1,
        [Obsolete("Use RTL instead")]
        RightToLeft = 2,
        RTL = 2
    }
}

