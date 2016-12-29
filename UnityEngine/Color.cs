namespace UnityEngine
{
    using System;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Representation of RGBA colors.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential), UsedByNativeCode]
    public struct Color
    {
        /// <summary>
        /// <para>Red component of the color.</para>
        /// </summary>
        public float r;
        /// <summary>
        /// <para>Green component of the color.</para>
        /// </summary>
        public float g;
        /// <summary>
        /// <para>Blue component of the color.</para>
        /// </summary>
        public float b;
        /// <summary>
        /// <para>Alpha component of the color.</para>
        /// </summary>
        public float a;
        /// <summary>
        /// <para>Constructs a new Color with given r,g,b,a components.</para>
        /// </summary>
        /// <param name="r">Red component.</param>
        /// <param name="g">Green component.</param>
        /// <param name="b">Blue component.</param>
        /// <param name="a">Alpha component.</param>
        public Color(float r, float g, float b, float a)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }

        /// <summary>
        /// <para>Constructs a new Color with given r,g,b components and sets a to 1.</para>
        /// </summary>
        /// <param name="r">Red component.</param>
        /// <param name="g">Green component.</param>
        /// <param name="b">Blue component.</param>
        public Color(float r, float g, float b)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = 1f;
        }

        /// <summary>
        /// <para>Returns a nicely formatted string of this color.</para>
        /// </summary>
        /// <param name="format"></param>
        public override string ToString()
        {
            object[] args = new object[] { this.r, this.g, this.b, this.a };
            return UnityString.Format("RGBA({0:F3}, {1:F3}, {2:F3}, {3:F3})", args);
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

        public override unsafe int GetHashCode()
        {
            Vector4 vector = *((Vector4*) this);
            return vector.GetHashCode();
        }

        public override bool Equals(object other)
        {
            if (!(other is Color))
            {
                return false;
            }
            Color color = (Color) other;
            return (((this.r.Equals(color.r) && this.g.Equals(color.g)) && this.b.Equals(color.b)) && this.a.Equals(color.a));
        }

        public static Color operator +(Color a, Color b) => 
            new Color(a.r + b.r, a.g + b.g, a.b + b.b, a.a + b.a);

        public static Color operator -(Color a, Color b) => 
            new Color(a.r - b.r, a.g - b.g, a.b - b.b, a.a - b.a);

        public static Color operator *(Color a, Color b) => 
            new Color(a.r * b.r, a.g * b.g, a.b * b.b, a.a * b.a);

        public static Color operator *(Color a, float b) => 
            new Color(a.r * b, a.g * b, a.b * b, a.a * b);

        public static Color operator *(float b, Color a) => 
            new Color(a.r * b, a.g * b, a.b * b, a.a * b);

        public static Color operator /(Color a, float b) => 
            new Color(a.r / b, a.g / b, a.b / b, a.a / b);

        public static bool operator ==(Color lhs, Color rhs) => 
            (lhs == rhs);

        public static bool operator !=(Color lhs, Color rhs) => 
            !(lhs == rhs);

        /// <summary>
        /// <para>Linearly interpolates between colors a and b by t.</para>
        /// </summary>
        /// <param name="a">Color a</param>
        /// <param name="b">Color b</param>
        /// <param name="t">Float for combining a and b</param>
        public static Color Lerp(Color a, Color b, float t)
        {
            t = Mathf.Clamp01(t);
            return new Color(a.r + ((b.r - a.r) * t), a.g + ((b.g - a.g) * t), a.b + ((b.b - a.b) * t), a.a + ((b.a - a.a) * t));
        }

        /// <summary>
        /// <para>Linearly interpolates between colors a and b by t.</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="t"></param>
        public static Color LerpUnclamped(Color a, Color b, float t) => 
            new Color(a.r + ((b.r - a.r) * t), a.g + ((b.g - a.g) * t), a.b + ((b.b - a.b) * t), a.a + ((b.a - a.a) * t));

        internal Color RGBMultiplied(float multiplier) => 
            new Color(this.r * multiplier, this.g * multiplier, this.b * multiplier, this.a);

        internal Color AlphaMultiplied(float multiplier) => 
            new Color(this.r, this.g, this.b, this.a * multiplier);

        internal Color RGBMultiplied(Color multiplier) => 
            new Color(this.r * multiplier.r, this.g * multiplier.g, this.b * multiplier.b, this.a);

        /// <summary>
        /// <para>Solid red. RGBA is (1, 0, 0, 1).</para>
        /// </summary>
        public static Color red =>
            new Color(1f, 0f, 0f, 1f);
        /// <summary>
        /// <para>Solid green. RGBA is (0, 1, 0, 1).</para>
        /// </summary>
        public static Color green =>
            new Color(0f, 1f, 0f, 1f);
        /// <summary>
        /// <para>Solid blue. RGBA is (0, 0, 1, 1).</para>
        /// </summary>
        public static Color blue =>
            new Color(0f, 0f, 1f, 1f);
        /// <summary>
        /// <para>Solid white. RGBA is (1, 1, 1, 1).</para>
        /// </summary>
        public static Color white =>
            new Color(1f, 1f, 1f, 1f);
        /// <summary>
        /// <para>Solid black. RGBA is (0, 0, 0, 1).</para>
        /// </summary>
        public static Color black =>
            new Color(0f, 0f, 0f, 1f);
        /// <summary>
        /// <para>Yellow. RGBA is (1, 0.92, 0.016, 1), but the color is nice to look at!</para>
        /// </summary>
        public static Color yellow =>
            new Color(1f, 0.9215686f, 0.01568628f, 1f);
        /// <summary>
        /// <para>Cyan. RGBA is (0, 1, 1, 1).</para>
        /// </summary>
        public static Color cyan =>
            new Color(0f, 1f, 1f, 1f);
        /// <summary>
        /// <para>Magenta. RGBA is (1, 0, 1, 1).</para>
        /// </summary>
        public static Color magenta =>
            new Color(1f, 0f, 1f, 1f);
        /// <summary>
        /// <para>Gray. RGBA is (0.5, 0.5, 0.5, 1).</para>
        /// </summary>
        public static Color gray =>
            new Color(0.5f, 0.5f, 0.5f, 1f);
        /// <summary>
        /// <para>English spelling for gray. RGBA is the same (0.5, 0.5, 0.5, 1).</para>
        /// </summary>
        public static Color grey =>
            new Color(0.5f, 0.5f, 0.5f, 1f);
        /// <summary>
        /// <para>Completely transparent. RGBA is (0, 0, 0, 0).</para>
        /// </summary>
        public static Color clear =>
            new Color(0f, 0f, 0f, 0f);
        /// <summary>
        /// <para>The grayscale value of the color. (Read Only)</para>
        /// </summary>
        public float grayscale =>
            (((0.299f * this.r) + (0.587f * this.g)) + (0.114f * this.b));
        /// <summary>
        /// <para>A linear value of an sRGB color.</para>
        /// </summary>
        public Color linear =>
            new Color(Mathf.GammaToLinearSpace(this.r), Mathf.GammaToLinearSpace(this.g), Mathf.GammaToLinearSpace(this.b), this.a);
        /// <summary>
        /// <para>A version of the color that has had the gamma curve applied.</para>
        /// </summary>
        public Color gamma =>
            new Color(Mathf.LinearToGammaSpace(this.r), Mathf.LinearToGammaSpace(this.g), Mathf.LinearToGammaSpace(this.b), this.a);
        /// <summary>
        /// <para>Returns the maximum color component value: Max(r,g,b).</para>
        /// </summary>
        public float maxColorComponent =>
            Mathf.Max(Mathf.Max(this.r, this.g), this.b);
        public static implicit operator Vector4(Color c) => 
            new Vector4(c.r, c.g, c.b, c.a);

        public static implicit operator Color(Vector4 v) => 
            new Color(v.x, v.y, v.z, v.w);

        public float this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return this.r;

                    case 1:
                        return this.g;

                    case 2:
                        return this.b;

                    case 3:
                        return this.a;
                }
                throw new IndexOutOfRangeException("Invalid Vector3 index!");
            }
            set
            {
                switch (index)
                {
                    case 0:
                        this.r = value;
                        break;

                    case 1:
                        this.g = value;
                        break;

                    case 2:
                        this.b = value;
                        break;

                    case 3:
                        this.a = value;
                        break;

                    default:
                        throw new IndexOutOfRangeException("Invalid Vector3 index!");
                }
            }
        }
        public static void RGBToHSV(Color rgbColor, out float H, out float S, out float V)
        {
            if ((rgbColor.b > rgbColor.g) && (rgbColor.b > rgbColor.r))
            {
                RGBToHSVHelper(4f, rgbColor.b, rgbColor.r, rgbColor.g, out H, out S, out V);
            }
            else if (rgbColor.g > rgbColor.r)
            {
                RGBToHSVHelper(2f, rgbColor.g, rgbColor.b, rgbColor.r, out H, out S, out V);
            }
            else
            {
                RGBToHSVHelper(0f, rgbColor.r, rgbColor.g, rgbColor.b, out H, out S, out V);
            }
        }

        private static void RGBToHSVHelper(float offset, float dominantcolor, float colorone, float colortwo, out float H, out float S, out float V)
        {
            V = dominantcolor;
            if (V != 0f)
            {
                float num = 0f;
                if (colorone > colortwo)
                {
                    num = colortwo;
                }
                else
                {
                    num = colorone;
                }
                float num2 = V - num;
                if (num2 != 0f)
                {
                    S = num2 / V;
                    H = offset + ((colorone - colortwo) / num2);
                }
                else
                {
                    S = 0f;
                    H = offset + (colorone - colortwo);
                }
                H /= 6f;
                if (H < 0f)
                {
                    H++;
                }
            }
            else
            {
                S = 0f;
                H = 0f;
            }
        }

        /// <summary>
        /// <para>Creates an RGB colour from HSV input.</para>
        /// </summary>
        /// <param name="H">Hue [0..1].</param>
        /// <param name="S">Saturation [0..1].</param>
        /// <param name="V">Value [0..1].</param>
        /// <param name="hdr">Output HDR colours. If true, the returned colour will not be clamped to [0..1].</param>
        /// <returns>
        /// <para>An opaque colour with HSV matching the input.</para>
        /// </returns>
        public static Color HSVToRGB(float H, float S, float V) => 
            HSVToRGB(H, S, V, true);

        /// <summary>
        /// <para>Creates an RGB colour from HSV input.</para>
        /// </summary>
        /// <param name="H">Hue [0..1].</param>
        /// <param name="S">Saturation [0..1].</param>
        /// <param name="V">Value [0..1].</param>
        /// <param name="hdr">Output HDR colours. If true, the returned colour will not be clamped to [0..1].</param>
        /// <returns>
        /// <para>An opaque colour with HSV matching the input.</para>
        /// </returns>
        public static Color HSVToRGB(float H, float S, float V, bool hdr)
        {
            Color white = Color.white;
            if (S == 0f)
            {
                white.r = V;
                white.g = V;
                white.b = V;
                return white;
            }
            if (V == 0f)
            {
                white.r = 0f;
                white.g = 0f;
                white.b = 0f;
                return white;
            }
            white.r = 0f;
            white.g = 0f;
            white.b = 0f;
            float num = S;
            float num2 = V;
            float f = H * 6f;
            int num4 = (int) Mathf.Floor(f);
            float num5 = f - num4;
            float num6 = num2 * (1f - num);
            float num7 = num2 * (1f - (num * num5));
            float num8 = num2 * (1f - (num * (1f - num5)));
            switch ((num4 + 1))
            {
                case 0:
                    white.r = num2;
                    white.g = num6;
                    white.b = num7;
                    break;

                case 1:
                    white.r = num2;
                    white.g = num8;
                    white.b = num6;
                    break;

                case 2:
                    white.r = num7;
                    white.g = num2;
                    white.b = num6;
                    break;

                case 3:
                    white.r = num6;
                    white.g = num2;
                    white.b = num8;
                    break;

                case 4:
                    white.r = num6;
                    white.g = num7;
                    white.b = num2;
                    break;

                case 5:
                    white.r = num8;
                    white.g = num6;
                    white.b = num2;
                    break;

                case 6:
                    white.r = num2;
                    white.g = num6;
                    white.b = num7;
                    break;

                case 7:
                    white.r = num2;
                    white.g = num8;
                    white.b = num6;
                    break;
            }
            if (!hdr)
            {
                white.r = Mathf.Clamp(white.r, 0f, 1f);
                white.g = Mathf.Clamp(white.g, 0f, 1f);
                white.b = Mathf.Clamp(white.b, 0f, 1f);
            }
            return white;
        }
    }
}

