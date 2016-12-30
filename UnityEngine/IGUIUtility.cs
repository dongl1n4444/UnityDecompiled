namespace UnityEngine
{
    using System;

    internal interface IGUIUtility
    {
        int GetControlID(int hint, FocusType focus);
        int GetPermanentControlID();

        int hotControl { get; set; }

        int keyboardControl { get; set; }
    }
}

