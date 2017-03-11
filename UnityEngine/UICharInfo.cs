namespace UnityEngine
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Class that specifies some information about a renderable character.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential), UsedByNativeCode]
    public struct UICharInfo
    {
        /// <summary>
        /// <para>Position of the character cursor in local (text generated) space.</para>
        /// </summary>
        public Vector2 cursorPos;
        /// <summary>
        /// <para>Character width.</para>
        /// </summary>
        public float charWidth;
    }
}

