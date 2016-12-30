namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Interface into SamsungTV specific functionality.</para>
    /// </summary>
    public sealed class SamsungTV
    {
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern TouchPadMode GetTouchPadMode();
        /// <summary>
        /// <para>Set the system language that is returned by Application.SystemLanguage.</para>
        /// </summary>
        /// <param name="language"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void SetSystemLanguage(SystemLanguage language);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern bool SetTouchPadMode(TouchPadMode value);

        /// <summary>
        /// <para>Returns true if there is an air mouse available.</para>
        /// </summary>
        public static bool airMouseConnected { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Changes the type of input the gamepad produces.</para>
        /// </summary>
        public static GamePadMode gamePadMode { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Changes the type of input the gesture camera produces.</para>
        /// </summary>
        public static GestureMode gestureMode { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Returns true if the camera sees a hand.</para>
        /// </summary>
        public static bool gestureWorking { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>The type of input the remote's touch pad produces.</para>
        /// </summary>
        public static TouchPadMode touchPadMode
        {
            get => 
                GetTouchPadMode();
            set
            {
                if (!SetTouchPadMode(value))
                {
                    throw new ArgumentException("Fail to set touchPadMode.");
                }
            }
        }

        /// <summary>
        /// <para>Types of input the gamepad can produce.</para>
        /// </summary>
        public enum GamePadMode
        {
            Default,
            Mouse
        }

        /// <summary>
        /// <para>Types of input the gesture camera can produce.</para>
        /// </summary>
        public enum GestureMode
        {
            Off,
            Mouse,
            Joystick
        }

        /// <summary>
        /// <para>Access to TV specific information.</para>
        /// </summary>
        public sealed class OpenAPI
        {
            public static string dUid { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

            /// <summary>
            /// <para>The server type. Possible values:
            /// Developing, Development, Invalid, Operating.</para>
            /// </summary>
            public static OpenAPIServerType serverType { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

            /// <summary>
            /// <para>Get local time on TV.</para>
            /// </summary>
            public static string timeOnTV { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

            /// <summary>
            /// <para>Get UID from TV.</para>
            /// </summary>
            public static string uid { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

            public enum OpenAPIServerType
            {
                Operating,
                Development,
                Developing,
                Invalid
            }
        }

        /// <summary>
        /// <para>Types of input the remote's touchpad can produce.</para>
        /// </summary>
        public enum TouchPadMode
        {
            Dpad,
            Joystick,
            Mouse
        }
    }
}

