namespace UnityEngine
{
    using System;

    internal class GUIUtilitySystem : IGUIUtility
    {
        public int GetControlID(int hint, FocusType focus) => 
            GUIUtility.GetControlID(hint, focus);

        public int GetPermanentControlID() => 
            GUIUtility.GetPermanentControlID();

        public int hotControl
        {
            get => 
                GUIUtility.hotControl;
            set
            {
                GUIUtility.hotControl = value;
            }
        }

        public int keyboardControl
        {
            get => 
                GUIUtility.keyboardControl;
            set
            {
                GUIUtility.keyboardControl = value;
            }
        }
    }
}

