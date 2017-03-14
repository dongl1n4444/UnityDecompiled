namespace UnityEditor
{
    using System;
    using System.Globalization;
    using UnityEngine;

    internal class PrefColor : IPrefType
    {
        private UnityEngine.Color m_Color;
        private UnityEngine.Color m_DefaultColor;
        private bool m_Loaded;
        private string m_Name;
        private UnityEngine.Color m_OptionalDarkColor;
        private UnityEngine.Color m_OptionalDarkDefaultColor;
        private bool m_SeparateColors;

        public PrefColor()
        {
            this.m_Loaded = true;
        }

        public PrefColor(string name, float defaultRed, float defaultGreen, float defaultBlue, float defaultAlpha)
        {
            this.m_Name = name;
            this.m_Color = this.m_DefaultColor = new UnityEngine.Color(defaultRed, defaultGreen, defaultBlue, defaultAlpha);
            this.m_SeparateColors = false;
            this.m_OptionalDarkColor = this.m_OptionalDarkDefaultColor = UnityEngine.Color.clear;
            Settings.Add(this);
            this.m_Loaded = false;
        }

        public PrefColor(string name, float defaultRed, float defaultGreen, float defaultBlue, float defaultAlpha, float defaultRed2, float defaultGreen2, float defaultBlue2, float defaultAlpha2)
        {
            this.m_Name = name;
            this.m_Color = this.m_DefaultColor = new UnityEngine.Color(defaultRed, defaultGreen, defaultBlue, defaultAlpha);
            this.m_SeparateColors = true;
            this.m_OptionalDarkColor = this.m_OptionalDarkDefaultColor = new UnityEngine.Color(defaultRed2, defaultGreen2, defaultBlue2, defaultAlpha2);
            Settings.Add(this);
            this.m_Loaded = false;
        }

        public void FromUniqueString(string s)
        {
            this.Load();
            char[] separator = new char[] { ';' };
            string[] strArray = s.Split(separator);
            if ((strArray.Length != 5) && (strArray.Length != 9))
            {
                Debug.LogError("Parsing PrefColor failed");
            }
            else
            {
                float num;
                float num2;
                float num3;
                float num4;
                this.m_Name = strArray[0];
                strArray[1] = strArray[1].Replace(',', '.');
                strArray[2] = strArray[2].Replace(',', '.');
                strArray[3] = strArray[3].Replace(',', '.');
                strArray[4] = strArray[4].Replace(',', '.');
                bool flag = float.TryParse(strArray[1], NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat, out num) & float.TryParse(strArray[2], NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat, out num2);
                flag &= float.TryParse(strArray[3], NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat, out num3);
                if (flag & float.TryParse(strArray[4], NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat, out num4))
                {
                    this.m_Color = new UnityEngine.Color(num, num2, num3, num4);
                }
                else
                {
                    Debug.LogError("Parsing PrefColor failed");
                }
                if (strArray.Length == 9)
                {
                    this.m_SeparateColors = true;
                    strArray[5] = strArray[5].Replace(',', '.');
                    strArray[6] = strArray[6].Replace(',', '.');
                    strArray[7] = strArray[7].Replace(',', '.');
                    strArray[8] = strArray[8].Replace(',', '.');
                    flag = float.TryParse(strArray[5], NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat, out num) & float.TryParse(strArray[6], NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat, out num2);
                    flag &= float.TryParse(strArray[7], NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat, out num3);
                    if (flag & float.TryParse(strArray[8], NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat, out num4))
                    {
                        this.m_OptionalDarkColor = new UnityEngine.Color(num, num2, num3, num4);
                    }
                    else
                    {
                        Debug.LogError("Parsing PrefColor failed");
                    }
                }
                else
                {
                    this.m_SeparateColors = false;
                    this.m_OptionalDarkColor = UnityEngine.Color.clear;
                }
            }
        }

        public void Load()
        {
            if (!this.m_Loaded)
            {
                this.m_Loaded = true;
                PrefColor color = Settings.Get<PrefColor>(this.m_Name, this);
                this.m_Name = color.m_Name;
                this.m_Color = color.m_Color;
                this.m_SeparateColors = color.m_SeparateColors;
                this.m_OptionalDarkColor = color.m_OptionalDarkColor;
            }
        }

        public static implicit operator UnityEngine.Color(PrefColor pcolor) => 
            pcolor.Color;

        internal void ResetToDefault()
        {
            this.Load();
            this.m_Color = this.m_DefaultColor;
            this.m_OptionalDarkColor = this.m_OptionalDarkDefaultColor;
        }

        public string ToUniqueString()
        {
            this.Load();
            if (this.m_SeparateColors)
            {
                object[] objArray1 = new object[] { this.m_Name, this.m_Color.r, this.m_Color.g, this.m_Color.b, this.m_Color.a, this.m_OptionalDarkColor.r, this.m_OptionalDarkColor.g, this.m_OptionalDarkColor.b, this.m_OptionalDarkColor.a };
                return string.Format(CultureInfo.InvariantCulture, "{0};{1};{2};{3};{4};{5};{6};{7};{8}", objArray1);
            }
            object[] args = new object[] { this.m_Name, this.m_Color.r, this.m_Color.g, this.m_Color.b, this.m_Color.a };
            return string.Format(CultureInfo.InvariantCulture, "{0};{1};{2};{3};{4}", args);
        }

        public UnityEngine.Color Color
        {
            get
            {
                this.Load();
                if (this.m_SeparateColors && EditorGUIUtility.isProSkin)
                {
                    return this.m_OptionalDarkColor;
                }
                return this.m_Color;
            }
            set
            {
                this.Load();
                if (this.m_SeparateColors && EditorGUIUtility.isProSkin)
                {
                    this.m_OptionalDarkColor = value;
                }
                else
                {
                    this.m_Color = value;
                }
            }
        }

        public string Name
        {
            get
            {
                this.Load();
                return this.m_Name;
            }
        }
    }
}

