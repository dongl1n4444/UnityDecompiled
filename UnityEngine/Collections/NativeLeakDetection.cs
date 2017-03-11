namespace UnityEngine.Collections
{
    using System;

    public static class NativeLeakDetection
    {
        private static int s_NativeLeakDetectionMode;

        public static NativeLeakDetectionMode Mode
        {
            get => 
                ((NativeLeakDetectionMode) s_NativeLeakDetectionMode);
            set
            {
                s_NativeLeakDetectionMode = (int) value;
            }
        }
    }
}

