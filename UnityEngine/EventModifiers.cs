namespace UnityEngine
{
    using System;

    /// <summary>
    /// <para>Types of modifier key that can be active during a keystroke event.</para>
    /// </summary>
    [Flags]
    public enum EventModifiers
    {
        /// <summary>
        /// <para>Alt key.</para>
        /// </summary>
        Alt = 4,
        /// <summary>
        /// <para>Caps lock key.</para>
        /// </summary>
        CapsLock = 0x20,
        /// <summary>
        /// <para>Command key (Mac).</para>
        /// </summary>
        Command = 8,
        /// <summary>
        /// <para>Control key.</para>
        /// </summary>
        Control = 2,
        /// <summary>
        /// <para>Function key.</para>
        /// </summary>
        FunctionKey = 0x40,
        /// <summary>
        /// <para>No modifier key pressed during a keystroke event.</para>
        /// </summary>
        None = 0,
        /// <summary>
        /// <para>Num lock key.</para>
        /// </summary>
        Numeric = 0x10,
        /// <summary>
        /// <para>Shift key.</para>
        /// </summary>
        Shift = 1
    }
}

