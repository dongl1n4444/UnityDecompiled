namespace UnityEngine.CSSLayout
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate CSSSize CSSMeasureFunc(IntPtr node, float width, CSSMeasureMode widthMode, float height, CSSMeasureMode heightMode);
}

