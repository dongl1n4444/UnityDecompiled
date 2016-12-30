namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Interface into the Input system.</para>
    /// </summary>
    public sealed class Input
    {
        private static Compass compassInstance;
        private static LocationService locationServiceInstance;
        private static Gyroscope m_MainGyro = null;

        /// <summary>
        /// <para>Returns specific acceleration measurement which occurred during last frame. (Does not allocate temporary variables).</para>
        /// </summary>
        /// <param name="index"></param>
        public static AccelerationEvent GetAccelerationEvent(int index)
        {
            AccelerationEvent event2;
            INTERNAL_CALL_GetAccelerationEvent(index, out event2);
            return event2;
        }

        /// <summary>
        /// <para>Returns the value of the virtual axis identified by axisName.</para>
        /// </summary>
        /// <param name="axisName"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern float GetAxis(string axisName);
        /// <summary>
        /// <para>Returns the value of the virtual axis identified by axisName with no smoothing filtering applied.</para>
        /// </summary>
        /// <param name="axisName"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern float GetAxisRaw(string axisName);
        /// <summary>
        /// <para>Returns true while the virtual button identified by buttonName is held down.</para>
        /// </summary>
        /// <param name="buttonName"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern bool GetButton(string buttonName);
        /// <summary>
        /// <para>Returns true during the frame the user pressed down the virtual button identified by buttonName.</para>
        /// </summary>
        /// <param name="buttonName"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern bool GetButtonDown(string buttonName);
        /// <summary>
        /// <para>Returns true the first frame the user releases the virtual button identified by buttonName.</para>
        /// </summary>
        /// <param name="buttonName"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern bool GetButtonUp(string buttonName);
        /// <summary>
        /// <para>Returns an array of strings describing the connected joysticks.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern string[] GetJoystickNames();
        /// <summary>
        /// <para>Returns true while the user holds down the key identified by name. Think auto fire.</para>
        /// </summary>
        /// <param name="name"></param>
        public static bool GetKey(string name) => 
            GetKeyString(name);

        /// <summary>
        /// <para>Returns true while the user holds down the key identified by the key KeyCode enum parameter.</para>
        /// </summary>
        /// <param name="key"></param>
        public static bool GetKey(KeyCode key) => 
            GetKeyInt((int) key);

        /// <summary>
        /// <para>Returns true during the frame the user starts pressing down the key identified by name.</para>
        /// </summary>
        /// <param name="name"></param>
        public static bool GetKeyDown(string name) => 
            GetKeyDownString(name);

        /// <summary>
        /// <para>Returns true during the frame the user starts pressing down the key identified by the key KeyCode enum parameter.</para>
        /// </summary>
        /// <param name="key"></param>
        public static bool GetKeyDown(KeyCode key) => 
            GetKeyDownInt((int) key);

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern bool GetKeyDownInt(int key);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern bool GetKeyDownString(string name);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern bool GetKeyInt(int key);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern bool GetKeyString(string name);
        /// <summary>
        /// <para>Returns true during the frame the user releases the key identified by name.</para>
        /// </summary>
        /// <param name="name"></param>
        public static bool GetKeyUp(string name) => 
            GetKeyUpString(name);

        /// <summary>
        /// <para>Returns true during the frame the user releases the key identified by the key KeyCode enum parameter.</para>
        /// </summary>
        /// <param name="key"></param>
        public static bool GetKeyUp(KeyCode key) => 
            GetKeyUpInt((int) key);

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern bool GetKeyUpInt(int key);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern bool GetKeyUpString(string name);
        /// <summary>
        /// <para>Returns whether the given mouse button is held down.</para>
        /// </summary>
        /// <param name="button"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern bool GetMouseButton(int button);
        /// <summary>
        /// <para>Returns true during the frame the user pressed the given mouse button.</para>
        /// </summary>
        /// <param name="button"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern bool GetMouseButtonDown(int button);
        /// <summary>
        /// <para>Returns true during the frame the user releases the given mouse button.</para>
        /// </summary>
        /// <param name="button"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern bool GetMouseButtonUp(int button);
        /// <summary>
        /// <para>Returns object representing status of a specific touch. (Does not allocate temporary variables).</para>
        /// </summary>
        /// <param name="index"></param>
        public static Touch GetTouch(int index)
        {
            Touch touch;
            INTERNAL_CALL_GetTouch(index, out touch);
            return touch;
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_GetAccelerationEvent(int index, out AccelerationEvent value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_GetTouch(int index, out Touch value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_get_acceleration(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_get_compositionCursorPos(out Vector2 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_get_mousePosition(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_get_mouseScrollDelta(out Vector2 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_set_compositionCursorPos(ref Vector2 value);
        /// <summary>
        /// <para>Determine whether a particular joystick model has been preconfigured by Unity. (Linux-only).</para>
        /// </summary>
        /// <param name="joystickName">The name of the joystick to check (returned by Input.GetJoystickNames).</param>
        /// <returns>
        /// <para>True if the joystick layout has been preconfigured; false otherwise.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern bool IsJoystickPreconfigured(string joystickName);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern int mainGyroIndex_Internal();
        /// <summary>
        /// <para>Resets all input. After ResetInputAxes all axes return to 0 and all buttons return to 0 for one frame.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void ResetInputAxes();

        /// <summary>
        /// <para>Last measured linear acceleration of a device in three-dimensional space. (Read Only)</para>
        /// </summary>
        public static Vector3 acceleration
        {
            get
            {
                Vector3 vector;
                INTERNAL_get_acceleration(out vector);
                return vector;
            }
        }

        /// <summary>
        /// <para>Number of acceleration measurements which occurred during last frame.</para>
        /// </summary>
        public static int accelerationEventCount { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Returns list of acceleration measurements which occurred during the last frame. (Read Only) (Allocates temporary variables).</para>
        /// </summary>
        public static AccelerationEvent[] accelerationEvents
        {
            get
            {
                int accelerationEventCount = Input.accelerationEventCount;
                AccelerationEvent[] eventArray = new AccelerationEvent[accelerationEventCount];
                for (int i = 0; i < accelerationEventCount; i++)
                {
                    eventArray[i] = GetAccelerationEvent(i);
                }
                return eventArray;
            }
        }

        /// <summary>
        /// <para>Is any key or mouse button currently held down? (Read Only)</para>
        /// </summary>
        public static bool anyKey { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Returns true the first frame the user hits any key or mouse button. (Read Only)</para>
        /// </summary>
        public static bool anyKeyDown { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Should  Back button quit the application?
        /// 
        /// Only usable on Android, Windows Phone or Windows Tablets.</para>
        /// </summary>
        public static bool backButtonLeavesApp { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Property for accessing compass (handheld devices only). (Read Only)</para>
        /// </summary>
        public static Compass compass
        {
            get
            {
                if (compassInstance == null)
                {
                    compassInstance = new Compass();
                }
                return compassInstance;
            }
        }

        /// <summary>
        /// <para>This property controls if input sensors should be compensated for screen orientation.</para>
        /// </summary>
        public static bool compensateSensors { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The current text input position used by IMEs to open windows.</para>
        /// </summary>
        public static Vector2 compositionCursorPos
        {
            get
            {
                Vector2 vector;
                INTERNAL_get_compositionCursorPos(out vector);
                return vector;
            }
            set
            {
                INTERNAL_set_compositionCursorPos(ref value);
            }
        }

        /// <summary>
        /// <para>The current IME composition string being typed by the user.</para>
        /// </summary>
        public static string compositionString { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Device physical orientation as reported by OS. (Read Only)</para>
        /// </summary>
        public static DeviceOrientation deviceOrientation { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Property indicating whether keypresses are eaten by a textinput if it has focus (default true).</para>
        /// </summary>
        [Obsolete("eatKeyPressOnTextFieldFocus property is deprecated, and only provided to support legacy behavior.")]
        public static bool eatKeyPressOnTextFieldFocus { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Returns default gyroscope.</para>
        /// </summary>
        public static Gyroscope gyro
        {
            get
            {
                if (m_MainGyro == null)
                {
                    m_MainGyro = new Gyroscope(mainGyroIndex_Internal());
                }
                return m_MainGyro;
            }
        }

        /// <summary>
        /// <para>Controls enabling and disabling of IME input composition.</para>
        /// </summary>
        public static IMECompositionMode imeCompositionMode { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Does the user have an IME keyboard input source selected?</para>
        /// </summary>
        public static bool imeIsSelected { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Returns the keyboard input entered this frame. (Read Only)</para>
        /// </summary>
        public static string inputString { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        [Obsolete("isGyroAvailable property is deprecated. Please use SystemInfo.supportsGyroscope instead.")]
        public static bool isGyroAvailable { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Property for accessing device location (handheld devices only). (Read Only)</para>
        /// </summary>
        public static LocationService location
        {
            get
            {
                if (locationServiceInstance == null)
                {
                    locationServiceInstance = new LocationService();
                }
                return locationServiceInstance;
            }
        }

        /// <summary>
        /// <para>The current mouse position in pixel coordinates. (Read Only)</para>
        /// </summary>
        public static Vector3 mousePosition
        {
            get
            {
                Vector3 vector;
                INTERNAL_get_mousePosition(out vector);
                return vector;
            }
        }

        /// <summary>
        /// <para>Indicates if a mouse device is detected.</para>
        /// </summary>
        public static bool mousePresent { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>The current mouse scroll delta. (Read Only)</para>
        /// </summary>
        public static Vector2 mouseScrollDelta
        {
            get
            {
                Vector2 vector;
                INTERNAL_get_mouseScrollDelta(out vector);
                return vector;
            }
        }

        /// <summary>
        /// <para>Property indicating whether the system handles multiple touches.</para>
        /// </summary>
        public static bool multiTouchEnabled { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Enables/Disables mouse simulation with touches. By default this option is enabled.</para>
        /// </summary>
        public static bool simulateMouseWithTouches { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Returns true when Stylus Touch is supported by a device or platform.</para>
        /// </summary>
        public static bool stylusTouchSupported { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Number of touches. Guaranteed not to change throughout the frame. (Read Only)</para>
        /// </summary>
        public static int touchCount { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Returns list of objects representing status of all touches during last frame. (Read Only) (Allocates temporary variables).</para>
        /// </summary>
        public static Touch[] touches
        {
            get
            {
                int touchCount = Input.touchCount;
                Touch[] touchArray = new Touch[touchCount];
                for (int i = 0; i < touchCount; i++)
                {
                    touchArray[i] = GetTouch(i);
                }
                return touchArray;
            }
        }

        /// <summary>
        /// <para>Bool value which let's users check if touch pressure is supported.</para>
        /// </summary>
        public static bool touchPressureSupported { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Returns whether the device on which application is currently running supports touch input.</para>
        /// </summary>
        public static bool touchSupported { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }
    }
}

