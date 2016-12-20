namespace UnityEngine
{
    using System;

    [Flags]
    internal enum TextGenerationError
    {
        CustomSizeOnNonDynamicFont = 1,
        CustomStyleOnNonDynamicFont = 2,
        NoFont = 4,
        None = 0
    }
}

