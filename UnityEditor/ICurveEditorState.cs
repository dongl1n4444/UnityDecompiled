namespace UnityEditor
{
    using System;

    internal interface ICurveEditorState
    {
        float currentTime { set; }

        bool syncTimeDuringDrag { get; }

        TimeArea.TimeFormat timeFormat { get; }
    }
}

