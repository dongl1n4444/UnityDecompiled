namespace UnityEngine
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential, Size=1), Obsolete("iPhoneTouch struct is deprecated. Please use Touch instead (UnityUpgradable) -> Touch", true)]
    public struct iPhoneTouch
    {
        [Obsolete("positionDelta property is deprecated. Please use Touch.deltaPosition instead (UnityUpgradable) -> Touch.deltaPosition", true)]
        public Vector2 positionDelta =>
            new Vector2();
        [Obsolete("timeDelta property is deprecated. Please use Touch.deltaTime instead (UnityUpgradable) -> Touch.deltaTime", true)]
        public float timeDelta =>
            0f;
        public int fingerId =>
            0;
        public Vector2 position =>
            new Vector2();
        public Vector2 deltaPosition =>
            new Vector2();
        public float deltaTime =>
            0f;
        public int tapCount =>
            0;
        public iPhoneTouchPhase phase =>
            iPhoneTouchPhase.Began;
    }
}

