namespace UnityEngine.CSSLayout
{
    using System;

    internal class MeasureOutput
    {
        public static int GetHeight(long measureOutput) => 
            ((int) (0xffffffffL & ((ulong) measureOutput)));

        public static int GetWidth(long measureOutput) => 
            ((int) (0xffffffffL & ((ulong) (measureOutput >> 0x20))));

        public static long Make(double width, double height) => 
            Make((int) width, (int) height);

        public static long Make(int width, int height) => 
            ((width << 0x20) | ((long) ((ulong) height)));
    }
}

