namespace UnityEditor
{
    using System;
    using System.Collections.Generic;

    internal interface CurveUpdater
    {
        void UpdateCurves(List<ChangedCurve> curve, string undoText);
    }
}

