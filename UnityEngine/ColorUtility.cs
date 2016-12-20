namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    /// <summary>
    /// <para>A collection of common color functions.</para>
    /// </summary>
    public sealed class ColorUtility
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern bool DoTryParseHtmlColor(string htmlString, out Color32 color);
        /// <summary>
        /// <para>Returns the color as a hexadecimal string in the format "RRGGBB".</para>
        /// </summary>
        /// <param name="color">The color to be converted.</param>
        /// <returns>
        /// <para>Hexadecimal string representing the color.</para>
        /// </returns>
        public static string ToHtmlStringRGB(Color color)
        {
            Color32 color2 = new Color32((byte) Mathf.Clamp(Mathf.RoundToInt(color.r * 255f), 0, 0xff), (byte) Mathf.Clamp(Mathf.RoundToInt(color.g * 255f), 0, 0xff), (byte) Mathf.Clamp(Mathf.RoundToInt(color.b * 255f), 0, 0xff), 1);
            return string.Format("{0:X2}{1:X2}{2:X2}", color2.r, color2.g, color2.b);
        }

        /// <summary>
        /// <para>Returns the color as a hexadecimal string in the format "RRGGBBAA".</para>
        /// </summary>
        /// <param name="color">The color to be converted.</param>
        /// <returns>
        /// <para>Hexadecimal string representing the color.</para>
        /// </returns>
        public static string ToHtmlStringRGBA(Color color)
        {
            Color32 color2 = new Color32((byte) Mathf.Clamp(Mathf.RoundToInt(color.r * 255f), 0, 0xff), (byte) Mathf.Clamp(Mathf.RoundToInt(color.g * 255f), 0, 0xff), (byte) Mathf.Clamp(Mathf.RoundToInt(color.b * 255f), 0, 0xff), (byte) Mathf.Clamp(Mathf.RoundToInt(color.a * 255f), 0, 0xff));
            return string.Format("{0:X2}{1:X2}{2:X2}{3:X2}", new object[] { color2.r, color2.g, color2.b, color2.a });
        }

        public static bool TryParseHtmlString(string htmlString, out Color color)
        {
            Color32 color2;
            bool flag = DoTryParseHtmlColor(htmlString, out color2);
            color = (Color) color2;
            return flag;
        }
    }
}

