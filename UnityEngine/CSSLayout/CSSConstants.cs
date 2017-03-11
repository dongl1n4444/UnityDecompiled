namespace UnityEngine.CSSLayout
{
    using System;

    internal static class CSSConstants
    {
        public const float Undefined = float.NaN;

        public static bool IsUndefined(float value) => 
            float.IsNaN(value);
    }
}

