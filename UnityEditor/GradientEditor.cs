namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using UnityEditorInternal;
    using UnityEngine;

    internal class GradientEditor
    {
        private const int k_MaxNumKeys = 8;
        private List<Swatch> m_AlphaSwatches;
        private Gradient m_Gradient;
        private GradientMode m_GradientMode;
        private bool m_HDR;
        private int m_NumSteps;
        private List<Swatch> m_RGBSwatches;
        [NonSerialized]
        private Swatch m_SelectedSwatch;
        private static Texture2D s_BackgroundTexture;
        private static Styles s_Styles;

        private void AssignBack()
        {
            this.m_RGBSwatches.Sort((Comparison<Swatch>) ((a, b) => this.SwatchSort(a, b)));
            GradientColorKey[] keyArray = new GradientColorKey[this.m_RGBSwatches.Count];
            for (int i = 0; i < this.m_RGBSwatches.Count; i++)
            {
                keyArray[i].color = this.m_RGBSwatches[i].m_Value;
                keyArray[i].time = this.m_RGBSwatches[i].m_Time;
            }
            this.m_AlphaSwatches.Sort((Comparison<Swatch>) ((a, b) => this.SwatchSort(a, b)));
            GradientAlphaKey[] keyArray2 = new GradientAlphaKey[this.m_AlphaSwatches.Count];
            for (int j = 0; j < this.m_AlphaSwatches.Count; j++)
            {
                keyArray2[j].alpha = this.m_AlphaSwatches[j].m_Value.r;
                keyArray2[j].time = this.m_AlphaSwatches[j].m_Time;
            }
            this.m_Gradient.colorKeys = keyArray;
            this.m_Gradient.alphaKeys = keyArray2;
            this.m_Gradient.mode = this.m_GradientMode;
            GUI.changed = true;
        }

        private void BuildArrays()
        {
            if (this.m_Gradient != null)
            {
                GradientColorKey[] colorKeys = this.m_Gradient.colorKeys;
                this.m_RGBSwatches = new List<Swatch>(colorKeys.Length);
                for (int i = 0; i < colorKeys.Length; i++)
                {
                    Color color = colorKeys[i].color;
                    color.a = 1f;
                    this.m_RGBSwatches.Add(new Swatch(colorKeys[i].time, color, false));
                }
                GradientAlphaKey[] alphaKeys = this.m_Gradient.alphaKeys;
                this.m_AlphaSwatches = new List<Swatch>(alphaKeys.Length);
                for (int j = 0; j < alphaKeys.Length; j++)
                {
                    float alpha = alphaKeys[j].alpha;
                    this.m_AlphaSwatches.Add(new Swatch(alphaKeys[j].time, new Color(alpha, alpha, alpha, 1f), true));
                }
                this.m_GradientMode = this.m_Gradient.mode;
            }
        }

        private Rect CalcSwatchRect(Rect totalRect, Swatch s)
        {
            float time = s.m_Time;
            return new Rect((totalRect.x + Mathf.Round(totalRect.width * time)) - 5f, totalRect.y, 10f, totalRect.height);
        }

        public static Texture2D CreateCheckerTexture(int numCols, int numRows, int cellPixelWidth, Color col1, Color col2)
        {
            int height = numRows * cellPixelWidth;
            int width = numCols * cellPixelWidth;
            Texture2D textured = new Texture2D(width, height, TextureFormat.RGBA32, false) {
                hideFlags = HideFlags.HideAndDontSave
            };
            Color[] colors = new Color[width * height];
            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numCols; j++)
                {
                    for (int k = 0; k < cellPixelWidth; k++)
                    {
                        for (int m = 0; m < cellPixelWidth; m++)
                        {
                            colors[((((i * cellPixelWidth) + k) * width) + (j * cellPixelWidth)) + m] = (((i + j) % 2) != 0) ? col2 : col1;
                        }
                    }
                }
            }
            textured.SetPixels(colors);
            textured.Apply();
            return textured;
        }

        public static void DrawGradientSwatch(Rect position, SerializedProperty property, Color bgColor)
        {
            DrawGradientSwatchInternal(position, null, property, bgColor);
        }

        public static void DrawGradientSwatch(Rect position, Gradient gradient, Color bgColor)
        {
            DrawGradientSwatchInternal(position, gradient, null, bgColor);
        }

        private static void DrawGradientSwatchInternal(Rect position, Gradient gradient, SerializedProperty property, Color bgColor)
        {
            if (Event.current.type == EventType.Repaint)
            {
                if (EditorGUI.showMixedValue)
                {
                    Color color = GUI.color;
                    float a = !GUI.enabled ? ((float) 2) : ((float) 1);
                    GUI.color = new Color(0.82f, 0.82f, 0.82f, a) * bgColor;
                    GUIStyle whiteTextureStyle = EditorGUIUtility.whiteTextureStyle;
                    whiteTextureStyle.Draw(position, false, false, false, false);
                    EditorGUI.BeginHandleMixedValueContentColor();
                    whiteTextureStyle.Draw(position, EditorGUI.mixedValueContent, false, false, false, false);
                    EditorGUI.EndHandleMixedValueContentColor();
                    GUI.color = color;
                }
                else
                {
                    float maxColorComponent;
                    Texture2D backgroundTexture = GetBackgroundTexture();
                    if (backgroundTexture != null)
                    {
                        Color color2 = GUI.color;
                        GUI.color = bgColor;
                        EditorGUIUtility.GetBasicTextureStyle(backgroundTexture).Draw(position, false, false, false, false);
                        GUI.color = color2;
                    }
                    Texture2D tex = null;
                    if (property != null)
                    {
                        tex = GradientPreviewCache.GetPropertyPreview(property);
                        maxColorComponent = GetMaxColorComponent(property.gradientValue);
                    }
                    else
                    {
                        tex = GradientPreviewCache.GetGradientPreview(gradient);
                        maxColorComponent = GetMaxColorComponent(gradient);
                    }
                    if (tex == null)
                    {
                        Debug.Log("Warning: Could not create preview for gradient");
                    }
                    else
                    {
                        EditorGUIUtility.GetBasicTextureStyle(tex).Draw(position, false, false, false, false);
                        if (maxColorComponent > 1f)
                        {
                            GUI.Label(new Rect(position.x, position.y - 1f, position.width - 3f, position.height + 2f), "HDR", EditorStyles.centeredGreyMiniLabel);
                        }
                    }
                }
            }
        }

        public static void DrawGradientWithBackground(Rect position, Gradient gradient)
        {
            Texture2D gradientPreview = GradientPreviewCache.GetGradientPreview(gradient);
            Rect rect = new Rect(position.x + 1f, position.y + 1f, position.width - 2f, position.height - 2f);
            Texture2D backgroundTexture = GetBackgroundTexture();
            Rect texCoords = new Rect(0f, 0f, rect.width / ((float) backgroundTexture.width), rect.height / ((float) backgroundTexture.height));
            GUI.DrawTextureWithTexCoords(rect, backgroundTexture, texCoords, false);
            if (gradientPreview != null)
            {
                GUI.DrawTexture(rect, gradientPreview, ScaleMode.StretchToFill, true);
            }
            GUI.Label(position, GUIContent.none, EditorStyles.colorPickerBox);
            if (GetMaxColorComponent(gradient) > 1f)
            {
                GUI.Label(new Rect(position.x, position.y, position.width - 3f, position.height), "HDR", EditorStyles.centeredGreyMiniLabel);
            }
        }

        private void DrawSwatch(Rect totalPos, Swatch s, bool upwards)
        {
            Color backgroundColor = GUI.backgroundColor;
            Rect position = this.CalcSwatchRect(totalPos, s);
            GUI.backgroundColor = s.m_Value;
            GUIStyle style = !upwards ? s_Styles.downSwatch : s_Styles.upSwatch;
            GUIStyle style2 = !upwards ? s_Styles.downSwatchOverlay : s_Styles.upSwatchOverlay;
            style.Draw(position, false, false, this.m_SelectedSwatch == s, false);
            GUI.backgroundColor = backgroundColor;
            style2.Draw(position, false, false, this.m_SelectedSwatch == s, false);
        }

        public static Texture2D GetBackgroundTexture()
        {
            if (s_BackgroundTexture == null)
            {
                s_BackgroundTexture = CreateCheckerTexture(0x20, 4, 4, Color.white, new Color(0.7f, 0.7f, 0.7f));
            }
            return s_BackgroundTexture;
        }

        private static float GetMaxColorComponent(Gradient gradient)
        {
            float a = 0f;
            GradientColorKey[] colorKeys = gradient.colorKeys;
            for (int i = 0; i < colorKeys.Length; i++)
            {
                a = Mathf.Max(a, colorKeys[i].color.maxColorComponent);
            }
            return a;
        }

        private float GetTime(float actualTime)
        {
            actualTime = Mathf.Clamp01(actualTime);
            if (this.m_NumSteps > 1)
            {
                float num = 1f / ((float) (this.m_NumSteps - 1));
                return (((float) Mathf.RoundToInt(actualTime / num)) / ((float) (this.m_NumSteps - 1)));
            }
            return actualTime;
        }

        public void Init(Gradient gradient, int numSteps, bool hdr)
        {
            this.m_Gradient = gradient;
            this.m_NumSteps = numSteps;
            this.m_HDR = hdr;
            this.BuildArrays();
            if (this.m_RGBSwatches.Count > 0)
            {
                this.m_SelectedSwatch = this.m_RGBSwatches[0];
            }
        }

        public void OnGUI(Rect position)
        {
            if (s_Styles == null)
            {
                s_Styles = new Styles();
            }
            float num = 24f;
            float num2 = 16f;
            float num3 = 26f;
            float num4 = ((position.height - (2f * num2)) - num3) - num;
            position.height = num;
            this.m_GradientMode = (GradientMode) EditorGUI.EnumPopup(position, s_Styles.modeText, this.m_GradientMode);
            if (this.m_GradientMode != this.m_Gradient.mode)
            {
                this.AssignBack();
            }
            position.y += num;
            position.height = num2;
            this.ShowSwatchArray(position, this.m_AlphaSwatches, true);
            position.y += num2;
            if (Event.current.type == EventType.Repaint)
            {
                position.height = num4;
                DrawGradientWithBackground(position, this.m_Gradient);
            }
            position.y += num4;
            position.height = num2;
            this.ShowSwatchArray(position, this.m_RGBSwatches, false);
            if (this.m_SelectedSwatch != null)
            {
                position.y += num2;
                position.height = num3;
                position.y += 10f;
                float num5 = 45f;
                float num6 = 60f;
                float num7 = 20f;
                float num8 = 50f;
                float num9 = ((num6 + num7) + num6) + num5;
                Rect rect = position;
                rect.height = 18f;
                rect.x += 17f;
                rect.width -= num9;
                EditorGUIUtility.labelWidth = num8;
                if (this.m_SelectedSwatch.m_IsAlpha)
                {
                    EditorGUIUtility.fieldWidth = 30f;
                    EditorGUI.BeginChangeCheck();
                    float num10 = ((float) EditorGUI.IntSlider(rect, s_Styles.alphaText, (int) (this.m_SelectedSwatch.m_Value.r * 255f), 0, 0xff)) / 255f;
                    if (EditorGUI.EndChangeCheck())
                    {
                        num10 = Mathf.Clamp01(num10);
                        this.m_SelectedSwatch.m_Value.r = this.m_SelectedSwatch.m_Value.g = this.m_SelectedSwatch.m_Value.b = num10;
                        this.AssignBack();
                        HandleUtility.Repaint();
                    }
                }
                else
                {
                    EditorGUI.BeginChangeCheck();
                    this.m_SelectedSwatch.m_Value = EditorGUI.ColorField(rect, s_Styles.colorText, this.m_SelectedSwatch.m_Value, true, false, this.m_HDR, ColorPicker.defaultHDRConfig);
                    if (EditorGUI.EndChangeCheck())
                    {
                        this.AssignBack();
                        HandleUtility.Repaint();
                    }
                }
                rect.x += rect.width + num7;
                rect.width = num5 + num6;
                EditorGUIUtility.labelWidth = num6;
                string kFloatFieldFormatString = EditorGUI.kFloatFieldFormatString;
                EditorGUI.kFloatFieldFormatString = "f1";
                EditorGUI.BeginChangeCheck();
                float num12 = EditorGUI.FloatField(rect, s_Styles.locationText, this.m_SelectedSwatch.m_Time * 100f) / 100f;
                if (EditorGUI.EndChangeCheck())
                {
                    this.m_SelectedSwatch.m_Time = Mathf.Clamp(num12, 0f, 1f);
                    this.AssignBack();
                }
                EditorGUI.kFloatFieldFormatString = kFloatFieldFormatString;
                rect.x += rect.width;
                rect.width = 20f;
                GUI.Label(rect, s_Styles.percentText);
            }
        }

        private void RemoveDuplicateOverlappingSwatches()
        {
            bool flag = false;
            for (int i = 1; i < this.m_RGBSwatches.Count; i++)
            {
                if (Mathf.Approximately(this.m_RGBSwatches[i - 1].m_Time, this.m_RGBSwatches[i].m_Time))
                {
                    this.m_RGBSwatches.RemoveAt(i);
                    i--;
                    flag = true;
                }
            }
            for (int j = 1; j < this.m_AlphaSwatches.Count; j++)
            {
                if (Mathf.Approximately(this.m_AlphaSwatches[j - 1].m_Time, this.m_AlphaSwatches[j].m_Time))
                {
                    this.m_AlphaSwatches.RemoveAt(j);
                    j--;
                    flag = true;
                }
            }
            if (flag)
            {
                this.AssignBack();
            }
        }

        private void ShowSwatchArray(Rect position, List<Swatch> swatches, bool isAlpha)
        {
            bool flag2;
            int controlID = GUIUtility.GetControlID(0x26e20929, FocusType.Passive);
            Event current = Event.current;
            float time = this.GetTime((Event.current.mousePosition.x - position.x) / position.width);
            Vector2 point = new Vector3(position.x + (time * position.width), Event.current.mousePosition.y);
            EventType typeForControl = current.GetTypeForControl(controlID);
            switch (typeForControl)
            {
                case EventType.MouseDown:
                {
                    Rect rect = position;
                    rect.xMin -= 10f;
                    rect.xMax += 10f;
                    if (rect.Contains(current.mousePosition))
                    {
                        GUIUtility.hotControl = controlID;
                        current.Use();
                        if ((!swatches.Contains(this.m_SelectedSwatch) || this.m_SelectedSwatch.m_IsAlpha) || !this.CalcSwatchRect(position, this.m_SelectedSwatch).Contains(current.mousePosition))
                        {
                            flag2 = false;
                            foreach (Swatch swatch2 in swatches)
                            {
                                if (this.CalcSwatchRect(position, swatch2).Contains(point))
                                {
                                    flag2 = true;
                                    this.m_SelectedSwatch = swatch2;
                                    break;
                                }
                            }
                            break;
                        }
                        if (current.clickCount == 2)
                        {
                            GUIUtility.keyboardControl = controlID;
                            ColorPicker.Show(GUIView.current, this.m_SelectedSwatch.m_Value, false, this.m_HDR, ColorPicker.defaultHDRConfig);
                            GUIUtility.ExitGUI();
                        }
                    }
                    return;
                }
                case EventType.MouseUp:
                    if (GUIUtility.hotControl == controlID)
                    {
                        GUIUtility.hotControl = 0;
                        current.Use();
                        if (!swatches.Contains(this.m_SelectedSwatch))
                        {
                            this.m_SelectedSwatch = null;
                        }
                        this.RemoveDuplicateOverlappingSwatches();
                    }
                    return;

                case EventType.MouseDrag:
                    if ((GUIUtility.hotControl == controlID) && (this.m_SelectedSwatch != null))
                    {
                        current.Use();
                        if (((current.mousePosition.y + 5f) >= position.y) && ((current.mousePosition.y - 5f) <= position.yMax))
                        {
                            if (!swatches.Contains(this.m_SelectedSwatch))
                            {
                                swatches.Add(this.m_SelectedSwatch);
                            }
                        }
                        else if (swatches.Count > 1)
                        {
                            swatches.Remove(this.m_SelectedSwatch);
                            this.AssignBack();
                            return;
                        }
                        this.m_SelectedSwatch.m_Time = time;
                        this.AssignBack();
                    }
                    return;

                case EventType.KeyDown:
                    if (current.keyCode == KeyCode.Delete)
                    {
                        if (this.m_SelectedSwatch != null)
                        {
                            List<Swatch> rGBSwatches;
                            if (!this.m_SelectedSwatch.m_IsAlpha)
                            {
                                rGBSwatches = this.m_RGBSwatches;
                            }
                            else
                            {
                                rGBSwatches = this.m_AlphaSwatches;
                            }
                            if (rGBSwatches.Count > 1)
                            {
                                rGBSwatches.Remove(this.m_SelectedSwatch);
                                this.AssignBack();
                                HandleUtility.Repaint();
                            }
                        }
                        current.Use();
                    }
                    return;

                case EventType.Repaint:
                {
                    bool flag = false;
                    foreach (Swatch swatch in swatches)
                    {
                        if (this.m_SelectedSwatch == swatch)
                        {
                            flag = true;
                        }
                        else
                        {
                            this.DrawSwatch(position, swatch, !isAlpha);
                        }
                    }
                    if (flag && (this.m_SelectedSwatch != null))
                    {
                        this.DrawSwatch(position, this.m_SelectedSwatch, !isAlpha);
                    }
                    return;
                }
                default:
                    switch (typeForControl)
                    {
                        case EventType.ValidateCommand:
                            if (current.commandName == "Delete")
                            {
                                Event.current.Use();
                            }
                            return;

                        case EventType.ExecuteCommand:
                            if (current.commandName == "ColorPickerChanged")
                            {
                                GUI.changed = true;
                                this.m_SelectedSwatch.m_Value = ColorPicker.color;
                                this.AssignBack();
                                HandleUtility.Repaint();
                            }
                            else if ((current.commandName == "Delete") && (swatches.Count > 1))
                            {
                                swatches.Remove(this.m_SelectedSwatch);
                                this.AssignBack();
                                HandleUtility.Repaint();
                            }
                            return;

                        default:
                            return;
                    }
                    break;
            }
            if (!flag2)
            {
                if (swatches.Count < 8)
                {
                    Color color = this.m_Gradient.Evaluate(time);
                    if (isAlpha)
                    {
                        color = new Color(color.a, color.a, color.a, 1f);
                    }
                    else
                    {
                        color.a = 1f;
                    }
                    this.m_SelectedSwatch = new Swatch(time, color, isAlpha);
                    swatches.Add(this.m_SelectedSwatch);
                    this.AssignBack();
                }
                else
                {
                    Debug.LogWarning(string.Concat(new object[] { "Max ", 8, " color keys and ", 8, " alpha keys are allowed in a gradient." }));
                }
            }
        }

        private int SwatchSort(Swatch lhs, Swatch rhs)
        {
            if ((lhs.m_Time == rhs.m_Time) && (lhs == this.m_SelectedSwatch))
            {
                return -1;
            }
            if ((lhs.m_Time == rhs.m_Time) && (rhs == this.m_SelectedSwatch))
            {
                return 1;
            }
            return lhs.m_Time.CompareTo(rhs.m_Time);
        }

        public Gradient target =>
            this.m_Gradient;

        private class Styles
        {
            public GUIContent alphaText = new GUIContent("Alpha");
            public GUIContent colorText = new GUIContent("Color");
            public GUIStyle downSwatch = "Grad Down Swatch";
            public GUIStyle downSwatchOverlay = "Grad Down Swatch Overlay";
            public GUIContent locationText = new GUIContent("Location");
            public GUIContent modeText = new GUIContent("Mode");
            public GUIContent percentText = new GUIContent("%");
            public GUIStyle upSwatch = "Grad Up Swatch";
            public GUIStyle upSwatchOverlay = "Grad Up Swatch Overlay";

            private static GUIStyle GetStyle(string name)
            {
                GUISkin skin = (GUISkin) EditorGUIUtility.LoadRequired("GradientEditor.GUISkin");
                return skin.GetStyle(name);
            }
        }

        public class Swatch
        {
            public bool m_IsAlpha;
            public float m_Time;
            public Color m_Value;

            public Swatch(float time, Color value, bool isAlpha)
            {
                this.m_Time = time;
                this.m_Value = value;
                this.m_IsAlpha = isAlpha;
            }
        }
    }
}

