namespace UnityEngine
{
    using System;

    [Obsolete("iPhoneKeyboard class is deprecated. Please use TouchScreenKeyboard instead (UnityUpgradable) -> TouchScreenKeyboard", true)]
    public class iPhoneKeyboard
    {
        public bool active
        {
            get => 
                false;
            set
            {
            }
        }

        public static Rect area =>
            new Rect();

        public bool done =>
            false;

        public static bool hideInput
        {
            get => 
                false;
            set
            {
            }
        }

        public string text
        {
            get => 
                string.Empty;
            set
            {
            }
        }

        public static bool visible =>
            false;
    }
}

