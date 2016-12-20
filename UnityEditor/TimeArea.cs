namespace UnityEditor
{
    using System;
    using UnityEngine;

    [Serializable]
    internal class TimeArea : ZoomableArea
    {
        internal const int kTickRulerDistFull = 80;
        internal const int kTickRulerDistLabel = 40;
        internal const int kTickRulerDistMin = 3;
        internal const float kTickRulerFatThreshold = 0.5f;
        internal const float kTickRulerHeightMax = 0.7f;
        [SerializeField]
        private TickHandler m_HTicks;
        [SerializeField]
        private TickHandler m_VTicks;
        private static float s_OriginalTime;
        private static float s_PickOffset;
        private static Styles2 styles;

        public TimeArea(bool minimalGUI) : base(minimalGUI)
        {
            float[] tickModulos = new float[] { 
                1E-07f, 5E-07f, 1E-06f, 5E-06f, 1E-05f, 5E-05f, 0.0001f, 0.0005f, 0.001f, 0.005f, 0.01f, 0.05f, 0.1f, 0.5f, 1f, 5f,
                10f, 50f, 100f, 500f, 1000f, 5000f, 10000f, 50000f, 100000f, 500000f, 1000000f, 5000000f, 1E+07f
            };
            this.hTicks = new TickHandler();
            this.hTicks.SetTickModulos(tickModulos);
            this.vTicks = new TickHandler();
            this.vTicks.SetTickModulos(tickModulos);
        }

        public TimeRulerDragMode BrowseRuler(Rect position, ref float time, float frameRate, bool pickAnywhere, GUIStyle thumbStyle)
        {
            int controlID = GUIUtility.GetControlID(0x2fb605, FocusType.Passive);
            return this.BrowseRuler(position, controlID, ref time, frameRate, pickAnywhere, thumbStyle);
        }

        public TimeRulerDragMode BrowseRuler(Rect position, int id, ref float time, float frameRate, bool pickAnywhere, GUIStyle thumbStyle)
        {
            Event current = Event.current;
            Rect rect = position;
            if (time != -1f)
            {
                rect.x = Mathf.Round(base.TimeToPixel(time, position)) - thumbStyle.overflow.left;
                rect.width = thumbStyle.fixedWidth + thumbStyle.overflow.horizontal;
            }
            switch (current.GetTypeForControl(id))
            {
                case EventType.MouseDown:
                    if (!rect.Contains(current.mousePosition))
                    {
                        if (pickAnywhere && position.Contains(current.mousePosition))
                        {
                            GUIUtility.hotControl = id;
                            float num = this.SnapTimeToWholeFPS(base.PixelToTime(current.mousePosition.x, position), frameRate);
                            s_OriginalTime = time;
                            if (num != time)
                            {
                                GUI.changed = true;
                            }
                            time = num;
                            s_PickOffset = 0f;
                            current.Use();
                            return TimeRulerDragMode.Start;
                        }
                        break;
                    }
                    GUIUtility.hotControl = id;
                    s_PickOffset = current.mousePosition.x - base.TimeToPixel(time, position);
                    current.Use();
                    return TimeRulerDragMode.Start;

                case EventType.MouseUp:
                    if (GUIUtility.hotControl != id)
                    {
                        break;
                    }
                    GUIUtility.hotControl = 0;
                    current.Use();
                    return TimeRulerDragMode.End;

                case EventType.MouseDrag:
                {
                    if (GUIUtility.hotControl != id)
                    {
                        break;
                    }
                    float num2 = this.SnapTimeToWholeFPS(base.PixelToTime(current.mousePosition.x - s_PickOffset, position), frameRate);
                    if (num2 != time)
                    {
                        GUI.changed = true;
                    }
                    time = num2;
                    current.Use();
                    return TimeRulerDragMode.Dragging;
                }
                case EventType.KeyDown:
                    if ((GUIUtility.hotControl != id) || (current.keyCode != KeyCode.Escape))
                    {
                        break;
                    }
                    if (time != s_OriginalTime)
                    {
                        GUI.changed = true;
                    }
                    time = s_OriginalTime;
                    GUIUtility.hotControl = 0;
                    current.Use();
                    return TimeRulerDragMode.Cancel;

                case EventType.Repaint:
                    if (time != -1f)
                    {
                        bool flag = position.Contains(current.mousePosition);
                        rect.x += thumbStyle.overflow.left;
                        thumbStyle.Draw(rect, id == GUIUtility.hotControl, flag || (id == GUIUtility.hotControl), false, false);
                    }
                    break;
            }
            return TimeRulerDragMode.None;
        }

        private void DrawLine(Vector2 lhs, Vector2 rhs)
        {
            GL.Vertex(base.DrawingToViewTransformPoint(new Vector3(lhs.x, lhs.y, 0f)));
            GL.Vertex(base.DrawingToViewTransformPoint(new Vector3(rhs.x, rhs.y, 0f)));
        }

        public void DrawMajorTicks(Rect position, float frameRate)
        {
            Color color = Handles.color;
            GUI.BeginGroup(position);
            if (Event.current.type != EventType.Repaint)
            {
                GUI.EndGroup();
            }
            else
            {
                InitStyles();
                this.SetTickMarkerRanges();
                this.hTicks.SetTickStrengths(3f, 80f, true);
                Color textColor = styles.TimelineTick.normal.textColor;
                textColor.a = 0.1f;
                Handles.color = textColor;
                Rect shownArea = base.shownArea;
                for (int i = 0; i < this.hTicks.tickLevels; i++)
                {
                    float num2 = this.hTicks.GetStrengthOfLevel(i) * 0.9f;
                    if (num2 > 0.5f)
                    {
                        float[] ticksAtLevel = this.hTicks.GetTicksAtLevel(i, true);
                        for (int j = 0; j < ticksAtLevel.Length; j++)
                        {
                            if (ticksAtLevel[j] >= 0f)
                            {
                                int num4 = Mathf.RoundToInt(ticksAtLevel[j] * frameRate);
                                float x = this.FrameToPixel((float) num4, frameRate, position, shownArea);
                                Handles.DrawLine(new Vector3(x, 0f, 0f), new Vector3(x, position.height, 0f));
                            }
                        }
                    }
                }
                GUI.EndGroup();
                Handles.color = color;
            }
        }

        public static void DrawVerticalLine(float x, float minY, float maxY, Color color)
        {
            if (Event.current.type == EventType.Repaint)
            {
                HandleUtility.ApplyWireMaterial();
                if (Application.platform == RuntimePlatform.WindowsEditor)
                {
                    GL.Begin(7);
                }
                else
                {
                    GL.Begin(1);
                }
                DrawVerticalLineFast(x, minY, maxY, color);
                GL.End();
            }
        }

        public static void DrawVerticalLineFast(float x, float minY, float maxY, Color color)
        {
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                GL.Color(color);
                GL.Vertex(new Vector3(x - 0.5f, minY, 0f));
                GL.Vertex(new Vector3(x + 0.5f, minY, 0f));
                GL.Vertex(new Vector3(x + 0.5f, maxY, 0f));
                GL.Vertex(new Vector3(x - 0.5f, maxY, 0f));
            }
            else
            {
                GL.Color(color);
                GL.Vertex(new Vector3(x, minY, 0f));
                GL.Vertex(new Vector3(x, maxY, 0f));
            }
        }

        public string FormatTime(float time, float frameRate, TimeFormat timeFormat)
        {
            if (timeFormat == TimeFormat.None)
            {
                int numberOfDecimalsForMinimumDifference;
                if (frameRate != 0f)
                {
                    numberOfDecimalsForMinimumDifference = MathUtils.GetNumberOfDecimalsForMinimumDifference((float) (1f / frameRate));
                }
                else
                {
                    numberOfDecimalsForMinimumDifference = MathUtils.GetNumberOfDecimalsForMinimumDifference((float) (base.shownArea.width / base.drawRect.width));
                }
                return time.ToString("N" + numberOfDecimalsForMinimumDifference);
            }
            int num2 = Mathf.RoundToInt(time * frameRate);
            if (timeFormat == TimeFormat.TimeFrame)
            {
                int num4 = (int) frameRate;
                int length = num4.ToString().Length;
                string str2 = string.Empty;
                if (num2 < 0)
                {
                    str2 = "-";
                    num2 = -num2;
                }
                int num5 = num2 / ((int) frameRate);
                float num6 = ((float) num2) % frameRate;
                return (str2 + num5.ToString() + ":" + num6.ToString().PadLeft(length, '0'));
            }
            return num2.ToString();
        }

        public string FormatValue(float value)
        {
            int numberOfDecimalsForMinimumDifference = MathUtils.GetNumberOfDecimalsForMinimumDifference((float) (base.shownArea.height / base.drawRect.height));
            return value.ToString("N" + numberOfDecimalsForMinimumDifference);
        }

        public float FrameToPixel(float i, float frameRate, Rect rect)
        {
            return this.FrameToPixel(i, frameRate, rect, base.shownArea);
        }

        private float FrameToPixel(float i, float frameRate, Rect rect, Rect theShownArea)
        {
            return (((i - (theShownArea.xMin * frameRate)) * rect.width) / (theShownArea.width * frameRate));
        }

        private static void InitStyles()
        {
            if (styles == null)
            {
                styles = new Styles2();
            }
        }

        public void SetTickMarkerRanges()
        {
            this.hTicks.SetRanges(base.shownArea.xMin, base.shownArea.xMax, base.drawRect.xMin, base.drawRect.xMax);
            this.vTicks.SetRanges(base.shownArea.yMin, base.shownArea.yMax, base.drawRect.yMin, base.drawRect.yMax);
        }

        public float SnapTimeToWholeFPS(float time, float frameRate)
        {
            if (frameRate == 0f)
            {
                return time;
            }
            return (Mathf.Round(time * frameRate) / frameRate);
        }

        public float TimeField(Rect rect, int id, float time, float frameRate, TimeFormat timeFormat)
        {
            bool flag;
            if (timeFormat == TimeFormat.None)
            {
                float num = EditorGUI.DoFloatField(EditorGUI.s_RecycledEditor, rect, new Rect(0f, 0f, 0f, 0f), id, time, EditorGUI.kFloatFieldFormatString, EditorStyles.numberField, false);
                return this.SnapTimeToWholeFPS(num, frameRate);
            }
            if (timeFormat == TimeFormat.Frame)
            {
                int num3 = Mathf.RoundToInt(time * frameRate);
                return (((float) EditorGUI.DoIntField(EditorGUI.s_RecycledEditor, rect, new Rect(0f, 0f, 0f, 0f), id, num3, EditorGUI.kIntFieldFormatString, EditorStyles.numberField, false, 0f)) / frameRate);
            }
            string text = this.FormatTime(time, frameRate, TimeFormat.TimeFrame);
            string allowedletters = "0123456789.,:";
            text = EditorGUI.DoTextField(EditorGUI.s_RecycledEditor, id, rect, text, EditorStyles.numberField, allowedletters, out flag, false, false, false);
            if (flag && (GUIUtility.keyboardControl == id))
            {
                float num9;
                GUI.changed = true;
                text = text.Replace(',', '.');
                int index = text.IndexOf(':');
                if (index >= 0)
                {
                    int num6;
                    int num7;
                    string s = text.Substring(0, index);
                    string str4 = text.Substring(index + 1);
                    if (int.TryParse(s, out num6) && int.TryParse(str4, out num7))
                    {
                        return (num6 + (((float) num7) / frameRate));
                    }
                    return time;
                }
                if (float.TryParse(text, out num9))
                {
                    return this.SnapTimeToWholeFPS(num9, frameRate);
                }
            }
            return time;
        }

        public void TimeRuler(Rect position, float frameRate)
        {
            this.TimeRuler(position, frameRate, true, false, 1f, TimeFormat.TimeFrame);
        }

        public void TimeRuler(Rect position, float frameRate, bool labels, bool useEntireHeight, float alpha)
        {
            this.TimeRuler(position, frameRate, labels, useEntireHeight, alpha, TimeFormat.TimeFrame);
        }

        public void TimeRuler(Rect position, float frameRate, bool labels, bool useEntireHeight, float alpha, TimeFormat timeFormat)
        {
            Color color = GUI.color;
            GUI.BeginGroup(position);
            InitStyles();
            HandleUtility.ApplyWireMaterial();
            Color backgroundColor = GUI.backgroundColor;
            this.SetTickMarkerRanges();
            this.hTicks.SetTickStrengths(3f, 80f, true);
            Color textColor = styles.TimelineTick.normal.textColor;
            textColor.a = 0.75f * alpha;
            if (Event.current.type == EventType.Repaint)
            {
                if (Application.platform == RuntimePlatform.WindowsEditor)
                {
                    GL.Begin(7);
                }
                else
                {
                    GL.Begin(1);
                }
                Rect shownArea = base.shownArea;
                for (int i = 0; i < this.hTicks.tickLevels; i++)
                {
                    float b = this.hTicks.GetStrengthOfLevel(i) * 0.9f;
                    float[] ticksAtLevel = this.hTicks.GetTicksAtLevel(i, true);
                    for (int j = 0; j < ticksAtLevel.Length; j++)
                    {
                        if ((ticksAtLevel[j] >= base.hRangeMin) && (ticksAtLevel[j] <= base.hRangeMax))
                        {
                            int num4 = Mathf.RoundToInt(ticksAtLevel[j] * frameRate);
                            float num5 = !useEntireHeight ? ((position.height * Mathf.Min(1f, b)) * 0.7f) : position.height;
                            DrawVerticalLineFast(this.FrameToPixel((float) num4, frameRate, position, shownArea), (position.height - num5) + 0.5f, position.height - 0.5f, new Color(1f, 1f, 1f, b / 0.5f) * textColor);
                        }
                    }
                }
                GL.End();
            }
            if (labels)
            {
                int levelWithMinSeparation = this.hTicks.GetLevelWithMinSeparation(40f);
                float[] numArray2 = this.hTicks.GetTicksAtLevel(levelWithMinSeparation, false);
                for (int k = 0; k < numArray2.Length; k++)
                {
                    if ((numArray2[k] >= base.hRangeMin) && (numArray2[k] <= base.hRangeMax))
                    {
                        int num9 = Mathf.RoundToInt(numArray2[k] * frameRate);
                        float num10 = Mathf.Floor(this.FrameToPixel((float) num9, frameRate, position));
                        string text = this.FormatTime(numArray2[k], frameRate, timeFormat);
                        GUI.Label(new Rect(num10 + 3f, -3f, 40f, 20f), text, styles.TimelineTick);
                    }
                }
            }
            GUI.EndGroup();
            GUI.backgroundColor = backgroundColor;
            GUI.color = color;
        }

        public float ValueField(Rect rect, int id, float value)
        {
            return EditorGUI.DoFloatField(EditorGUI.s_RecycledEditor, rect, new Rect(0f, 0f, 0f, 0f), id, value, EditorGUI.kFloatFieldFormatString, EditorStyles.numberField, false);
        }

        public TickHandler hTicks
        {
            get
            {
                return this.m_HTicks;
            }
            set
            {
                this.m_HTicks = value;
            }
        }

        public TickHandler vTicks
        {
            get
            {
                return this.m_VTicks;
            }
            set
            {
                this.m_VTicks = value;
            }
        }

        private class Styles2
        {
            public GUIStyle labelTickMarks = "CurveEditorLabelTickMarks";
            public GUIStyle TimelineTick = "AnimationTimelineTick";
        }

        public enum TimeFormat
        {
            None,
            TimeFrame,
            Frame
        }

        public enum TimeRulerDragMode
        {
            None,
            Start,
            End,
            Dragging,
            Cancel
        }
    }
}

