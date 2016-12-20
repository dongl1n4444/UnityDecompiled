namespace UnityEngine
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>A UnityGUI event.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class Event
    {
        [NonSerialized]
        internal IntPtr m_Ptr;
        private static Event s_Current;
        private static Event s_MasterEvent;
        [CompilerGenerated]
        private static Dictionary<string, int> <>f__switch$map0;
        public Event()
        {
            this.Init(0);
        }

        public Event(int displayIndex)
        {
            this.Init(displayIndex);
        }

        public Event(Event other)
        {
            if (other == null)
            {
                throw new ArgumentException("Event to copy from is null.");
            }
            this.InitCopy(other);
        }

        private Event(IntPtr ptr)
        {
            this.InitPtr(ptr);
        }

        ~Event()
        {
            this.Cleanup();
        }

        internal static void CleanupRoots()
        {
            s_Current = null;
            s_MasterEvent = null;
        }

        /// <summary>
        /// <para>The mouse position.</para>
        /// </summary>
        public Vector2 mousePosition
        {
            get
            {
                Vector2 vector;
                this.Internal_GetMousePosition(out vector);
                return vector;
            }
            set
            {
                this.Internal_SetMousePosition(value);
            }
        }
        /// <summary>
        /// <para>The relative movement of the mouse compared to last event.</para>
        /// </summary>
        public Vector2 delta
        {
            get
            {
                Vector2 vector;
                this.Internal_GetMouseDelta(out vector);
                return vector;
            }
            set
            {
                this.Internal_SetMouseDelta(value);
            }
        }
        [Obsolete("Use HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);", true)]
        public Ray mouseRay
        {
            get
            {
                return new Ray(Vector3.up, Vector3.up);
            }
            set
            {
            }
        }
        /// <summary>
        /// <para>Is Shift held down? (Read Only)</para>
        /// </summary>
        public bool shift
        {
            get
            {
                return ((this.modifiers & EventModifiers.Shift) != EventModifiers.None);
            }
            set
            {
                if (!value)
                {
                    this.modifiers &= ~EventModifiers.Shift;
                }
                else
                {
                    this.modifiers |= EventModifiers.Shift;
                }
            }
        }
        /// <summary>
        /// <para>Is Control key held down? (Read Only)</para>
        /// </summary>
        public bool control
        {
            get
            {
                return ((this.modifiers & EventModifiers.Control) != EventModifiers.None);
            }
            set
            {
                if (!value)
                {
                    this.modifiers &= ~EventModifiers.Control;
                }
                else
                {
                    this.modifiers |= EventModifiers.Control;
                }
            }
        }
        /// <summary>
        /// <para>Is Alt/Option key held down? (Read Only)</para>
        /// </summary>
        public bool alt
        {
            get
            {
                return ((this.modifiers & EventModifiers.Alt) != EventModifiers.None);
            }
            set
            {
                if (!value)
                {
                    this.modifiers &= ~EventModifiers.Alt;
                }
                else
                {
                    this.modifiers |= EventModifiers.Alt;
                }
            }
        }
        /// <summary>
        /// <para>Is Command/Windows key held down? (Read Only)</para>
        /// </summary>
        public bool command
        {
            get
            {
                return ((this.modifiers & EventModifiers.Command) != EventModifiers.None);
            }
            set
            {
                if (!value)
                {
                    this.modifiers &= ~EventModifiers.Command;
                }
                else
                {
                    this.modifiers |= EventModifiers.Command;
                }
            }
        }
        /// <summary>
        /// <para>Is Caps Lock on? (Read Only)</para>
        /// </summary>
        public bool capsLock
        {
            get
            {
                return ((this.modifiers & EventModifiers.CapsLock) != EventModifiers.None);
            }
            set
            {
                if (!value)
                {
                    this.modifiers &= ~EventModifiers.CapsLock;
                }
                else
                {
                    this.modifiers |= EventModifiers.CapsLock;
                }
            }
        }
        /// <summary>
        /// <para>Is the current keypress on the numeric keyboard? (Read Only)</para>
        /// </summary>
        public bool numeric
        {
            get
            {
                return ((this.modifiers & EventModifiers.Numeric) != EventModifiers.None);
            }
            set
            {
                if (!value)
                {
                    this.modifiers &= ~EventModifiers.Shift;
                }
                else
                {
                    this.modifiers |= EventModifiers.Shift;
                }
            }
        }
        /// <summary>
        /// <para>Is the current keypress a function key? (Read Only)</para>
        /// </summary>
        public bool functionKey
        {
            get
            {
                return ((this.modifiers & EventModifiers.FunctionKey) != EventModifiers.None);
            }
        }
        /// <summary>
        /// <para>The current event that's being processed right now.</para>
        /// </summary>
        public static Event current
        {
            get
            {
                if (GUIUtility.Internal_GetGUIDepth() > 0)
                {
                    return s_Current;
                }
                return null;
            }
            set
            {
                if (value != null)
                {
                    s_Current = value;
                }
                else
                {
                    s_Current = s_MasterEvent;
                }
                Internal_SetNativeEvent(s_Current.m_Ptr);
            }
        }
        [RequiredByNativeCode]
        private static void Internal_MakeMasterEventCurrent(int displayIndex)
        {
            if (s_MasterEvent == null)
            {
                s_MasterEvent = new Event(displayIndex);
            }
            s_MasterEvent.displayIndex = displayIndex;
            s_Current = s_MasterEvent;
            Internal_SetNativeEvent(s_MasterEvent.m_Ptr);
        }

        /// <summary>
        /// <para>Is this event a keyboard event? (Read Only)</para>
        /// </summary>
        public bool isKey
        {
            get
            {
                EventType type = this.type;
                return ((type == EventType.KeyDown) || (type == EventType.KeyUp));
            }
        }
        /// <summary>
        /// <para>Is this event a mouse event? (Read Only)</para>
        /// </summary>
        public bool isMouse
        {
            get
            {
                EventType type = this.type;
                return ((((type == EventType.MouseMove) || (type == EventType.MouseDown)) || (type == EventType.MouseUp)) || (type == EventType.MouseDrag));
            }
        }
        /// <summary>
        /// <para>Create a keyboard event.</para>
        /// </summary>
        /// <param name="key"></param>
        public static Event KeyboardEvent(string key)
        {
            Event event2 = new Event(0) {
                type = EventType.KeyDown
            };
            if (!string.IsNullOrEmpty(key))
            {
                int startIndex = 0;
                bool flag = false;
                do
                {
                    flag = true;
                    if (startIndex >= key.Length)
                    {
                        flag = false;
                        break;
                    }
                    char ch = key[startIndex];
                    switch (ch)
                    {
                        case '#':
                            event2.modifiers |= EventModifiers.Shift;
                            startIndex++;
                            break;

                        case '%':
                            event2.modifiers |= EventModifiers.Command;
                            startIndex++;
                            break;

                        case '&':
                            event2.modifiers |= EventModifiers.Alt;
                            startIndex++;
                            break;

                        default:
                            if (ch == '^')
                            {
                                event2.modifiers |= EventModifiers.Control;
                                startIndex++;
                            }
                            else
                            {
                                flag = false;
                            }
                            break;
                    }
                }
                while (flag);
                string str = key.Substring(startIndex, key.Length - startIndex).ToLower();
                if (str != null)
                {
                    int num2;
                    if (<>f__switch$map0 == null)
                    {
                        Dictionary<string, int> dictionary = new Dictionary<string, int>(0x31) {
                            { 
                                "[0]",
                                0
                            },
                            { 
                                "[1]",
                                1
                            },
                            { 
                                "[2]",
                                2
                            },
                            { 
                                "[3]",
                                3
                            },
                            { 
                                "[4]",
                                4
                            },
                            { 
                                "[5]",
                                5
                            },
                            { 
                                "[6]",
                                6
                            },
                            { 
                                "[7]",
                                7
                            },
                            { 
                                "[8]",
                                8
                            },
                            { 
                                "[9]",
                                9
                            },
                            { 
                                "[.]",
                                10
                            },
                            { 
                                "[/]",
                                11
                            },
                            { 
                                "[-]",
                                12
                            },
                            { 
                                "[+]",
                                13
                            },
                            { 
                                "[=]",
                                14
                            },
                            { 
                                "[equals]",
                                15
                            },
                            { 
                                "[enter]",
                                0x10
                            },
                            { 
                                "up",
                                0x11
                            },
                            { 
                                "down",
                                0x12
                            },
                            { 
                                "left",
                                0x13
                            },
                            { 
                                "right",
                                20
                            },
                            { 
                                "insert",
                                0x15
                            },
                            { 
                                "home",
                                0x16
                            },
                            { 
                                "end",
                                0x17
                            },
                            { 
                                "pgup",
                                0x18
                            },
                            { 
                                "page up",
                                0x19
                            },
                            { 
                                "pgdown",
                                0x1a
                            },
                            { 
                                "page down",
                                0x1b
                            },
                            { 
                                "backspace",
                                0x1c
                            },
                            { 
                                "delete",
                                0x1d
                            },
                            { 
                                "tab",
                                30
                            },
                            { 
                                "f1",
                                0x1f
                            },
                            { 
                                "f2",
                                0x20
                            },
                            { 
                                "f3",
                                0x21
                            },
                            { 
                                "f4",
                                0x22
                            },
                            { 
                                "f5",
                                0x23
                            },
                            { 
                                "f6",
                                0x24
                            },
                            { 
                                "f7",
                                0x25
                            },
                            { 
                                "f8",
                                0x26
                            },
                            { 
                                "f9",
                                0x27
                            },
                            { 
                                "f10",
                                40
                            },
                            { 
                                "f11",
                                0x29
                            },
                            { 
                                "f12",
                                0x2a
                            },
                            { 
                                "f13",
                                0x2b
                            },
                            { 
                                "f14",
                                0x2c
                            },
                            { 
                                "f15",
                                0x2d
                            },
                            { 
                                "[esc]",
                                0x2e
                            },
                            { 
                                "return",
                                0x2f
                            },
                            { 
                                "space",
                                0x30
                            }
                        };
                        <>f__switch$map0 = dictionary;
                    }
                    if (<>f__switch$map0.TryGetValue(str, out num2))
                    {
                        switch (num2)
                        {
                            case 0:
                                event2.character = '0';
                                event2.keyCode = KeyCode.Keypad0;
                                return event2;

                            case 1:
                                event2.character = '1';
                                event2.keyCode = KeyCode.Keypad1;
                                return event2;

                            case 2:
                                event2.character = '2';
                                event2.keyCode = KeyCode.Keypad2;
                                return event2;

                            case 3:
                                event2.character = '3';
                                event2.keyCode = KeyCode.Keypad3;
                                return event2;

                            case 4:
                                event2.character = '4';
                                event2.keyCode = KeyCode.Keypad4;
                                return event2;

                            case 5:
                                event2.character = '5';
                                event2.keyCode = KeyCode.Keypad5;
                                return event2;

                            case 6:
                                event2.character = '6';
                                event2.keyCode = KeyCode.Keypad6;
                                return event2;

                            case 7:
                                event2.character = '7';
                                event2.keyCode = KeyCode.Keypad7;
                                return event2;

                            case 8:
                                event2.character = '8';
                                event2.keyCode = KeyCode.Keypad8;
                                return event2;

                            case 9:
                                event2.character = '9';
                                event2.keyCode = KeyCode.Keypad9;
                                return event2;

                            case 10:
                                event2.character = '.';
                                event2.keyCode = KeyCode.KeypadPeriod;
                                return event2;

                            case 11:
                                event2.character = '/';
                                event2.keyCode = KeyCode.KeypadDivide;
                                return event2;

                            case 12:
                                event2.character = '-';
                                event2.keyCode = KeyCode.KeypadMinus;
                                return event2;

                            case 13:
                                event2.character = '+';
                                event2.keyCode = KeyCode.KeypadPlus;
                                return event2;

                            case 14:
                                event2.character = '=';
                                event2.keyCode = KeyCode.KeypadEquals;
                                return event2;

                            case 15:
                                event2.character = '=';
                                event2.keyCode = KeyCode.KeypadEquals;
                                return event2;

                            case 0x10:
                                event2.character = '\n';
                                event2.keyCode = KeyCode.KeypadEnter;
                                return event2;

                            case 0x11:
                                event2.keyCode = KeyCode.UpArrow;
                                event2.modifiers |= EventModifiers.FunctionKey;
                                return event2;

                            case 0x12:
                                event2.keyCode = KeyCode.DownArrow;
                                event2.modifiers |= EventModifiers.FunctionKey;
                                return event2;

                            case 0x13:
                                event2.keyCode = KeyCode.LeftArrow;
                                event2.modifiers |= EventModifiers.FunctionKey;
                                return event2;

                            case 20:
                                event2.keyCode = KeyCode.RightArrow;
                                event2.modifiers |= EventModifiers.FunctionKey;
                                return event2;

                            case 0x15:
                                event2.keyCode = KeyCode.Insert;
                                event2.modifiers |= EventModifiers.FunctionKey;
                                return event2;

                            case 0x16:
                                event2.keyCode = KeyCode.Home;
                                event2.modifiers |= EventModifiers.FunctionKey;
                                return event2;

                            case 0x17:
                                event2.keyCode = KeyCode.End;
                                event2.modifiers |= EventModifiers.FunctionKey;
                                return event2;

                            case 0x18:
                                event2.keyCode = KeyCode.PageDown;
                                event2.modifiers |= EventModifiers.FunctionKey;
                                return event2;

                            case 0x19:
                                event2.keyCode = KeyCode.PageUp;
                                event2.modifiers |= EventModifiers.FunctionKey;
                                return event2;

                            case 0x1a:
                                event2.keyCode = KeyCode.PageUp;
                                event2.modifiers |= EventModifiers.FunctionKey;
                                return event2;

                            case 0x1b:
                                event2.keyCode = KeyCode.PageDown;
                                event2.modifiers |= EventModifiers.FunctionKey;
                                return event2;

                            case 0x1c:
                                event2.keyCode = KeyCode.Backspace;
                                event2.modifiers |= EventModifiers.FunctionKey;
                                return event2;

                            case 0x1d:
                                event2.keyCode = KeyCode.Delete;
                                event2.modifiers |= EventModifiers.FunctionKey;
                                return event2;

                            case 30:
                                event2.keyCode = KeyCode.Tab;
                                return event2;

                            case 0x1f:
                                event2.keyCode = KeyCode.F1;
                                event2.modifiers |= EventModifiers.FunctionKey;
                                return event2;

                            case 0x20:
                                event2.keyCode = KeyCode.F2;
                                event2.modifiers |= EventModifiers.FunctionKey;
                                return event2;

                            case 0x21:
                                event2.keyCode = KeyCode.F3;
                                event2.modifiers |= EventModifiers.FunctionKey;
                                return event2;

                            case 0x22:
                                event2.keyCode = KeyCode.F4;
                                event2.modifiers |= EventModifiers.FunctionKey;
                                return event2;

                            case 0x23:
                                event2.keyCode = KeyCode.F5;
                                event2.modifiers |= EventModifiers.FunctionKey;
                                return event2;

                            case 0x24:
                                event2.keyCode = KeyCode.F6;
                                event2.modifiers |= EventModifiers.FunctionKey;
                                return event2;

                            case 0x25:
                                event2.keyCode = KeyCode.F7;
                                event2.modifiers |= EventModifiers.FunctionKey;
                                return event2;

                            case 0x26:
                                event2.keyCode = KeyCode.F8;
                                event2.modifiers |= EventModifiers.FunctionKey;
                                return event2;

                            case 0x27:
                                event2.keyCode = KeyCode.F9;
                                event2.modifiers |= EventModifiers.FunctionKey;
                                return event2;

                            case 40:
                                event2.keyCode = KeyCode.F10;
                                event2.modifiers |= EventModifiers.FunctionKey;
                                return event2;

                            case 0x29:
                                event2.keyCode = KeyCode.F11;
                                event2.modifiers |= EventModifiers.FunctionKey;
                                return event2;

                            case 0x2a:
                                event2.keyCode = KeyCode.F12;
                                event2.modifiers |= EventModifiers.FunctionKey;
                                return event2;

                            case 0x2b:
                                event2.keyCode = KeyCode.F13;
                                event2.modifiers |= EventModifiers.FunctionKey;
                                return event2;

                            case 0x2c:
                                event2.keyCode = KeyCode.F14;
                                event2.modifiers |= EventModifiers.FunctionKey;
                                return event2;

                            case 0x2d:
                                event2.keyCode = KeyCode.F15;
                                event2.modifiers |= EventModifiers.FunctionKey;
                                return event2;

                            case 0x2e:
                                event2.keyCode = KeyCode.Escape;
                                return event2;

                            case 0x2f:
                                event2.character = '\n';
                                event2.keyCode = KeyCode.Return;
                                event2.modifiers &= ~EventModifiers.FunctionKey;
                                return event2;

                            case 0x30:
                                event2.keyCode = KeyCode.Space;
                                event2.character = ' ';
                                event2.modifiers &= ~EventModifiers.FunctionKey;
                                return event2;
                        }
                    }
                }
                if (str.Length != 1)
                {
                    try
                    {
                        event2.keyCode = (KeyCode) Enum.Parse(typeof(KeyCode), str, true);
                    }
                    catch (ArgumentException)
                    {
                        object[] args = new object[] { str };
                        Debug.LogError(UnityString.Format("Unable to find key name that matches '{0}'", args));
                    }
                    return event2;
                }
                event2.character = str.ToLower()[0];
                event2.keyCode = (KeyCode) event2.character;
                if (event2.modifiers != EventModifiers.None)
                {
                    event2.character = '\0';
                }
            }
            return event2;
        }

        public override int GetHashCode()
        {
            int keyCode = 1;
            if (this.isKey)
            {
                keyCode = (ushort) this.keyCode;
            }
            if (this.isMouse)
            {
                keyCode = this.mousePosition.GetHashCode();
            }
            return ((keyCode * 0x25) | this.modifiers);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (object.ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != base.GetType())
            {
                return false;
            }
            Event event2 = (Event) obj;
            if ((this.type != event2.type) || ((this.modifiers & ~EventModifiers.CapsLock) != (event2.modifiers & ~EventModifiers.CapsLock)))
            {
                return false;
            }
            if (this.isKey)
            {
                return (this.keyCode == event2.keyCode);
            }
            return (this.isMouse && (this.mousePosition == event2.mousePosition));
        }

        public override string ToString()
        {
            if (this.isKey)
            {
                if (this.character == '\0')
                {
                    object[] args = new object[] { this.type, this.modifiers, this.keyCode };
                    return UnityString.Format(@"Event:{0}   Character:\0   Modifiers:{1}   KeyCode:{2}", args);
                }
                object[] objArray2 = new object[] { "Event:", this.type, "   Character:", (int) this.character, "   Modifiers:", this.modifiers, "   KeyCode:", this.keyCode };
                return string.Concat(objArray2);
            }
            if (this.isMouse)
            {
                object[] objArray3 = new object[] { this.type, this.mousePosition, this.modifiers };
                return UnityString.Format("Event: {0}   Position: {1} Modifiers: {2}", objArray3);
            }
            if ((this.type == EventType.ExecuteCommand) || (this.type == EventType.ValidateCommand))
            {
                object[] objArray4 = new object[] { this.type, this.commandName };
                return UnityString.Format("Event: {0}  \"{1}\"", objArray4);
            }
            return ("" + this.type);
        }

        /// <summary>
        /// <para>Use this event.</para>
        /// </summary>
        public void Use()
        {
            if ((this.type == EventType.Repaint) || (this.type == EventType.Layout))
            {
                object[] args = new object[] { this.type };
                Debug.LogWarning(UnityString.Format("Event.Use() should not be called for events of type {0}", args));
            }
            this.Internal_Use();
        }

        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        private extern void Init(int displayIndex);
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        private extern void Cleanup();
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        private extern void InitCopy(Event other);
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        private extern void InitPtr(IntPtr ptr);
        public EventType rawType { [MethodImpl(MethodImplOptions.InternalCall)] get; }
        /// <summary>
        /// <para>The type of event.</para>
        /// </summary>
        public EventType type { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
        /// <summary>
        /// <para>Get a filtered event type for a given control ID.</para>
        /// </summary>
        /// <param name="controlID">The ID of the control you are querying from.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern EventType GetTypeForControl(int controlID);
        private void Internal_SetMousePosition(Vector2 value)
        {
            INTERNAL_CALL_Internal_SetMousePosition(this, ref value);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_Internal_SetMousePosition(Event self, ref Vector2 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void Internal_GetMousePosition(out Vector2 value);
        private void Internal_SetMouseDelta(Vector2 value)
        {
            INTERNAL_CALL_Internal_SetMouseDelta(this, ref value);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_Internal_SetMouseDelta(Event self, ref Vector2 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void Internal_GetMouseDelta(out Vector2 value);
        /// <summary>
        /// <para>Which mouse button was pressed.</para>
        /// </summary>
        public int button { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
        /// <summary>
        /// <para>Which modifier keys are held down.</para>
        /// </summary>
        public EventModifiers modifiers { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
        public float pressure { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
        /// <summary>
        /// <para>How many consecutive mouse clicks have we received.</para>
        /// </summary>
        public int clickCount { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
        /// <summary>
        /// <para>The character typed.</para>
        /// </summary>
        public char character { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
        /// <summary>
        /// <para>The name of an ExecuteCommand or ValidateCommand Event.</para>
        /// </summary>
        public string commandName { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
        /// <summary>
        /// <para>The raw key code for keyboard events.</para>
        /// </summary>
        public KeyCode keyCode { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void Internal_SetNativeEvent(IntPtr ptr);
        /// <summary>
        /// <para>Index of display that the event belongs to.</para>
        /// </summary>
        public int displayIndex { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void Internal_Use();
        /// <summary>
        /// <para>Get the next queued [Event] from the event system.</para>
        /// </summary>
        /// <param name="outEvent">Next Event.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern bool PopEvent(Event outEvent);
        /// <summary>
        /// <para>Returns the current number of events that are stored in the event queue.</para>
        /// </summary>
        /// <returns>
        /// <para>Current number of events currently in the event queue.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern int GetEventCount();
    }
}

