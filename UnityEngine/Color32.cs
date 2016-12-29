namespace UnityEngine
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Representation of RGBA colors in 32 bit format.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential), UsedByNativeCode, IL2CPPStructAlignment(Align=4)]
    public struct Color32
    {
        /// <summary>
        /// <para>Red component of the color.</para>
        /// </summary>
        public byte r;
        /// <summary>
        /// <para>Green component of the color.</para>
        /// </summary>
        public byte g;
        /// <summary>
        /// <para>Blue component of the color.</para>
        /// </summary>
        public byte b;
        /// <summary>
        /// <para>Alpha component of the color.</para>
        /// </summary>
        public byte a;
        /// <summary>
        /// <para>Constructs a new Color32 with given r, g, b, a components.</para>
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <param name="a"></param>
        public Color32(byte r, byte g, byte b, byte a)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }

        public static implicit operator Color32(Color c) => 
            new Color32((byte) (Mathf.Clamp01(c.r) * 255f), (byte) (Mathf.Clamp01(c.g) * 255f), (byte) (Mathf.Clamp01(c.b) * 255f), (byte) (Mathf.Clamp01(c.a) * 255f));

        public static implicit operator Color(Color32 c) => 
            new Color(((float) c.r) / 255f, ((float) c.g) / 255f, ((float) c.b) / 255f, ((float) c.a) / 255f);

        /// <summary>
        /// <para>Linearly interpolates between colors a and b by t.</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="t"></param>
        public static Color32 Lerp(Color32 a, Color32 b, float t)
        {
            t = Mathf.Clamp01(t);
            return new Color32((byte) (a.r + ((b.r - a.r) * t)), (byte) (a.g + ((b.g - a.g) * t)), (byte) (a.b + ((b.b - a.b) * t)), (byte) (a.a + ((b.a - a.a) * t)));
        }

        /// <summary>
        /// <para>Linearly interpolates between colors a and b by t.</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="t"></param>
        public static Color32 LerpUnclamped(Color32 a, Color32 b, float t) => 
            new Color32((byte) (a.r + ((b.r - a.r) * t)), (byte) (a.g + ((b.g - a.g) * t)), (byte) (a.b + ((b.b - a.b) * t)), (byte) (a.a + ((b.a - a.a) * t)));

        /// <summary>
        /// <para>Returns a nicely formatted string of this color.</para>
        /// </summary>
        /// <param name="format"></param>
        public override string ToString()
        {
            object[] args = new object[] { this.r, this.g, this.b, this.a };
            return UnityString.Format("RGBA({0}, {1}, {2}, {3})", args);
        }

        /// <summary>
        /// <para>Returns a nicely formatted string of this color.</para>
        /// </summary>
        /// <param name="format"></param>
        public string ToString(string format)
        {
            object[] args = new object[] { this.r.ToString(format), this.g.ToString(format), this.b.ToString(format), this.a.ToString(format) };
            return UnityString.Format("RGBA({0}, {1}, {2}, {3})", args);
        }
    }
}

