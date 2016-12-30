namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Offsets for rectangles, borders, etc.</para>
    /// </summary>
    [Serializable, StructLayout(LayoutKind.Sequential), UsedByNativeCode]
    public sealed class RectOffset
    {
        [NonSerialized]
        internal IntPtr m_Ptr;
        private readonly object m_SourceStyle;
        /// <summary>
        /// <para>Creates a new rectangle with offsets.</para>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="top"></param>
        /// <param name="bottom"></param>
        public RectOffset()
        {
            this.Init();
        }

        internal RectOffset(object sourceStyle, IntPtr source)
        {
            this.m_SourceStyle = sourceStyle;
            this.m_Ptr = source;
        }

        /// <summary>
        /// <para>Creates a new rectangle with offsets.</para>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="top"></param>
        /// <param name="bottom"></param>
        public RectOffset(int left, int right, int top, int bottom)
        {
            this.Init();
            this.left = left;
            this.right = right;
            this.top = top;
            this.bottom = bottom;
        }

        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe, GeneratedByOldBindingsGenerator]
        private extern void Init();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator, ThreadAndSerializationSafe]
        private extern void Cleanup();
        /// <summary>
        /// <para>Left edge size.</para>
        /// </summary>
        public int left { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }
        /// <summary>
        /// <para>Right edge size.</para>
        /// </summary>
        public int right { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }
        /// <summary>
        /// <para>Top edge size.</para>
        /// </summary>
        public int top { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }
        /// <summary>
        /// <para>Bottom edge size.</para>
        /// </summary>
        public int bottom { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }
        /// <summary>
        /// <para>Shortcut for left + right. (Read Only)</para>
        /// </summary>
        public int horizontal { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }
        /// <summary>
        /// <para>Shortcut for top + bottom. (Read Only)</para>
        /// </summary>
        public int vertical { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }
        /// <summary>
        /// <para>Add the border offsets to a rect.</para>
        /// </summary>
        /// <param name="rect"></param>
        public Rect Add(Rect rect)
        {
            Rect rect2;
            INTERNAL_CALL_Add(this, ref rect, out rect2);
            return rect2;
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_Add(RectOffset self, ref Rect rect, out Rect value);
        /// <summary>
        /// <para>Remove the border offsets from a rect.</para>
        /// </summary>
        /// <param name="rect"></param>
        public Rect Remove(Rect rect)
        {
            Rect rect2;
            INTERNAL_CALL_Remove(this, ref rect, out rect2);
            return rect2;
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_Remove(RectOffset self, ref Rect rect, out Rect value);
        ~RectOffset()
        {
            if (this.m_SourceStyle == null)
            {
                this.Cleanup();
            }
        }

        public override string ToString()
        {
            object[] args = new object[] { this.left, this.right, this.top, this.bottom };
            return UnityString.Format("RectOffset (l:{0} r:{1} t:{2} b:{3})", args);
        }
    }
}

