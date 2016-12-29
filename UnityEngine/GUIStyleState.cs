namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    /// <summary>
    /// <para>Specialized values for the given states used by GUIStyle objects.</para>
    /// </summary>
    [Serializable, StructLayout(LayoutKind.Sequential)]
    public sealed class GUIStyleState
    {
        [NonSerialized]
        internal IntPtr m_Ptr;
        private readonly GUIStyle m_SourceStyle;
        [NonSerialized]
        private Texture2D m_Background;
        [NonSerialized]
        private Texture2D[] m_ScaledBackgrounds;
        public GUIStyleState()
        {
            this.Init();
        }

        private GUIStyleState(GUIStyle sourceStyle, IntPtr source)
        {
            this.m_SourceStyle = sourceStyle;
            this.m_Ptr = source;
        }

        internal static GUIStyleState ProduceGUIStyleStateFromDeserialization(GUIStyle sourceStyle, IntPtr source)
        {
            GUIStyleState state = new GUIStyleState(sourceStyle, source);
            state.m_Background = state.GetBackgroundInternalFromDeserialization();
            state.m_ScaledBackgrounds = state.GetScaledBackgroundsInternalFromDeserialization();
            return state;
        }

        internal static GUIStyleState GetGUIStyleState(GUIStyle sourceStyle, IntPtr source)
        {
            GUIStyleState state = new GUIStyleState(sourceStyle, source);
            state.m_Background = state.GetBackgroundInternal();
            state.m_ScaledBackgrounds = state.GetScaledBackgroundsInternalFromDeserialization();
            return state;
        }

        ~GUIStyleState()
        {
            if (this.m_SourceStyle == null)
            {
                this.Cleanup();
            }
        }

        /// <summary>
        /// <para>The background image used by GUI elements in this given state.</para>
        /// </summary>
        public Texture2D background
        {
            get => 
                this.GetBackgroundInternal();
            set
            {
                this.SetBackgroundInternal(value);
                this.m_Background = value;
            }
        }
        /// <summary>
        /// <para>Background images used by this state when on a high-resolution screen. It should either be left empty, or contain a single image that is exactly twice the resolution of background. This is only used by the editor. The field is not copied to player data, and is not accessible from player code.</para>
        /// </summary>
        public Texture2D[] scaledBackgrounds
        {
            get => 
                this.GetScaledBackgroundsInternal();
            set
            {
                this.SetScaledBackgroundsInternal(value);
                this.m_ScaledBackgrounds = value;
            }
        }
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        private extern void Init();
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        private extern void Cleanup();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void SetBackgroundInternal(Texture2D value);
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        private extern Texture2D GetBackgroundInternalFromDeserialization();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern Texture2D GetBackgroundInternal();
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        private extern Texture2D[] GetScaledBackgroundsInternalFromDeserialization();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern Texture2D[] GetScaledBackgroundsInternal();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void SetScaledBackgroundsInternal(Texture2D[] newValue);
        /// <summary>
        /// <para>The text color used by GUI elements in this state.</para>
        /// </summary>
        public Color textColor
        {
            get
            {
                Color color;
                this.INTERNAL_get_textColor(out color);
                return color;
            }
            set
            {
                this.INTERNAL_set_textColor(ref value);
            }
        }
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_get_textColor(out Color value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_set_textColor(ref Color value);
    }
}

