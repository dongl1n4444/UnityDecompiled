namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    internal class DuckVolumeGUI : IAudioEffectPluginGUI
    {
        private static DragType dragtype = DragType.None;
        public static string kAttackTimeName = "Attack Time";
        public static string kKneeName = "Knee";
        public static string kMakeupGainName = "Make-up Gain";
        public static string kRatioName = "Ratio";
        public static string kReleaseTimeName = "Release Time";
        public static string kThresholdName = "Threshold";
        public static GUIStyle textStyle10 = BuildGUIStyleForLabel(Color.grey, 10, false, FontStyle.Normal, TextAnchor.MiddleLeft);

        public static GUIStyle BuildGUIStyleForLabel(Color color, int fontSize, bool wrapText, FontStyle fontstyle, TextAnchor anchor)
        {
            GUIStyle style = new GUIStyle();
            style.focused.background = style.onNormal.background;
            style.focused.textColor = color;
            style.alignment = anchor;
            style.fontSize = fontSize;
            style.fontStyle = fontstyle;
            style.wordWrap = wrapText;
            style.clipping = TextClipping.Overflow;
            style.normal.textColor = color;
            return style;
        }

        private static bool CurveDisplay(IAudioEffectPlugin plugin, Rect r0, ref float threshold, ref float ratio, ref float makeupGain, ref float attackTime, ref float releaseTime, ref float knee, float sidechainLevel, float outputLevel, float blend)
        {
            float num3;
            float num4;
            float num5;
            float num6;
            float num7;
            float num8;
            float num9;
            float num10;
            float num11;
            float num12;
            float num13;
            float num14;
            <CurveDisplay>c__AnonStorey0 storey = new <CurveDisplay>c__AnonStorey0 {
                blend = blend
            };
            Event current = Event.current;
            int controlID = GUIUtility.GetControlID(FocusType.Passive);
            Rect r = AudioCurveRendering.BeginCurveFrame(r0);
            float num2 = 10f;
            plugin.GetFloatParameterInfo(kThresholdName, out num3, out num4, out num5);
            plugin.GetFloatParameterInfo(kRatioName, out num6, out num7, out num8);
            plugin.GetFloatParameterInfo(kMakeupGainName, out num9, out num10, out num11);
            plugin.GetFloatParameterInfo(kKneeName, out num12, out num13, out num14);
            storey.dbRange = 100f;
            storey.dbMin = -80f;
            float num15 = (r.width * (threshold - storey.dbMin)) / storey.dbRange;
            bool flag = false;
            switch (current.GetTypeForControl(controlID))
            {
                case EventType.MouseDown:
                    if (r.Contains(Event.current.mousePosition) && (current.button == 0))
                    {
                        dragtype = DragType.None;
                        GUIUtility.hotControl = controlID;
                        EditorGUIUtility.SetWantsMouseJumping(1);
                        current.Use();
                        if (Mathf.Abs((float) ((r.x + num15) - current.mousePosition.x)) >= 10f)
                        {
                            dragtype = (current.mousePosition.x >= (r.x + num15)) ? DragType.Ratio : DragType.MakeupGain;
                        }
                        else
                        {
                            dragtype = DragType.ThresholdAndKnee;
                        }
                    }
                    break;

                case EventType.MouseUp:
                    if ((GUIUtility.hotControl == controlID) && (current.button == 0))
                    {
                        dragtype = DragType.None;
                        GUIUtility.hotControl = 0;
                        EditorGUIUtility.SetWantsMouseJumping(0);
                        current.Use();
                    }
                    break;

                case EventType.MouseDrag:
                    if (GUIUtility.hotControl == controlID)
                    {
                        float num16 = !current.alt ? 1f : 0.25f;
                        if (dragtype == DragType.ThresholdAndKnee)
                        {
                            if (Mathf.Abs(current.delta.x) < Mathf.Abs(current.delta.y))
                            {
                                knee = Mathf.Clamp(knee + ((current.delta.y * 0.5f) * num16), num12, num13);
                            }
                            else
                            {
                                threshold = Mathf.Clamp(threshold + ((current.delta.x * 0.1f) * num16), num3, num4);
                            }
                        }
                        else if (dragtype == DragType.Ratio)
                        {
                            ratio = Mathf.Clamp(ratio + ((current.delta.y * ((ratio <= 1f) ? 0.003f : 0.05f)) * num16), num6, num7);
                        }
                        else if (dragtype == DragType.MakeupGain)
                        {
                            makeupGain = Mathf.Clamp(makeupGain - ((current.delta.y * 0.5f) * num16), num9, num10);
                        }
                        else
                        {
                            Debug.LogError("Drag: Unhandled enum");
                        }
                        flag = true;
                        current.Use();
                    }
                    break;
            }
            if (current.type == EventType.Repaint)
            {
                <CurveDisplay>c__AnonStorey1 storey2 = new <CurveDisplay>c__AnonStorey1 {
                    <>f__ref$0 = storey
                };
                HandleUtility.ApplyWireMaterial();
                float num17 = r.height * (1f - (((threshold - storey.dbMin) + makeupGain) / storey.dbRange));
                Color col = new Color(0.7f, 0.7f, 0.7f);
                Color black = Color.black;
                storey2.duckGradient = 1f / ratio;
                storey2.duckThreshold = threshold;
                storey2.duckSidechainLevel = sidechainLevel;
                storey2.duckMakeupGain = makeupGain;
                storey2.duckKnee = knee;
                storey2.duckKneeC1 = (knee <= 0f) ? 0f : ((storey2.duckGradient - 1f) / (4f * knee));
                storey2.duckKneeC2 = storey2.duckThreshold - knee;
                AudioCurveRendering.DrawFilledCurve(r, new AudioCurveRendering.AudioCurveAndColorEvaluator(storey2.<>m__0));
                if (dragtype == DragType.MakeupGain)
                {
                    AudioCurveRendering.DrawCurve(r, new AudioCurveRendering.AudioCurveEvaluator(storey2.<>m__1), Color.white);
                }
                textStyle10.normal.textColor = ScaleAlpha(col, storey.blend);
                EditorGUI.DrawRect(new Rect(r.x + num15, r.y, 1f, r.height), textStyle10.normal.textColor);
                DrawText((r.x + num15) + 4f, r.y + 6f, $"Threshold: {(float) threshold:F1} dB");
                textStyle10.normal.textColor = ScaleAlpha(black, storey.blend);
                DrawText(r.x + 4f, (r.y + r.height) - 10f, (sidechainLevel >= -80f) ? $"Input: {sidechainLevel:F1} dB" : "Input: None");
                if (dragtype == DragType.Ratio)
                {
                    float num18 = r.height / r.width;
                    Color[] colors = new Color[] { Color.black, Color.black };
                    Vector3[] points = new Vector3[] { new Vector3((r.x + num15) + r.width, (r.y + num17) - (num18 * r.width), 0f), new Vector3((r.x + num15) - r.width, (r.y + num17) + (num18 * r.width), 0f) };
                    Handles.DrawAAPolyLine(2f, colors, points);
                    Color[] colorArray2 = new Color[] { Color.white, Color.white };
                    Vector3[] vectorArray2 = new Vector3[] { new Vector3((r.x + num15) + r.width, (r.y + num17) - ((num18 * storey2.duckGradient) * r.width), 0f), new Vector3((r.x + num15) - r.width, (r.y + num17) + ((num18 * storey2.duckGradient) * r.width), 0f) };
                    Handles.DrawAAPolyLine(3f, colorArray2, vectorArray2);
                }
                else if (dragtype == DragType.ThresholdAndKnee)
                {
                    float x = ((threshold - knee) - storey.dbMin) / storey.dbRange;
                    float num20 = ((threshold + knee) - storey.dbMin) / storey.dbRange;
                    float num21 = EvaluateDuckingVolume(x, ratio, threshold, makeupGain, knee, storey.dbRange, storey.dbMin);
                    float num22 = EvaluateDuckingVolume(num20, ratio, threshold, makeupGain, knee, storey.dbRange, storey.dbMin);
                    float y = r.yMax - (((num21 + 1f) * 0.5f) * r.height);
                    float num24 = r.yMax - (((num22 + 1f) * 0.5f) * r.height);
                    EditorGUI.DrawRect(new Rect(r.x + (x * r.width), y, 1f, r.height - y), new Color(0f, 0f, 0f, 0.5f));
                    EditorGUI.DrawRect(new Rect((r.x + (num20 * r.width)) - 1f, num24, 1f, r.height - num24), new Color(0f, 0f, 0f, 0.5f));
                    EditorGUI.DrawRect(new Rect((r.x + num15) - 1f, r.y, 3f, r.height), Color.white);
                }
                outputLevel = (Mathf.Clamp(outputLevel - makeupGain, storey.dbMin, storey.dbMin + storey.dbRange) - storey.dbMin) / storey.dbRange;
                if (EditorApplication.isPlaying)
                {
                    Rect rect2 = new Rect(((r.x + r.width) - num2) + 2f, r.y + 2f, num2 - 4f, r.height - 4f);
                    DrawVU(rect2, outputLevel, storey.blend, true);
                }
            }
            AudioCurveRendering.EndCurveFrame();
            return flag;
        }

        public static void DrawLine(float x1, float y1, float x2, float y2, Color col)
        {
            Handles.color = col;
            Handles.DrawLine(new Vector3(x1, y1, 0f), new Vector3(x2, y2, 0f));
        }

        public static void DrawText(float x, float y, string text)
        {
            GUI.Label(new Rect(x, y - 5f, 200f, 10f), new GUIContent(text, ""), textStyle10);
        }

        protected static void DrawVU(Rect r, float level, float blend, bool topdown)
        {
            level = 1f - level;
            Rect rect = new Rect(r.x + 1f, (r.y + 1f) + (!topdown ? (level * r.height) : 0f), r.width - 2f, (r.y - 2f) + (!topdown ? (r.height - (level * r.height)) : (level * r.height)));
            AudioMixerDrawUtils.DrawRect(r, new Color(0.1f, 0.1f, 0.1f));
            AudioMixerDrawUtils.DrawRect(rect, new Color(0.6f, 0.2f, 0.2f));
        }

        private static float EvaluateDuckingVolume(float x, float ratio, float threshold, float makeupGain, float knee, float dbRange, float dbMin)
        {
            float num = 1f / ratio;
            float num2 = threshold;
            float num3 = makeupGain;
            float num4 = knee;
            float num5 = (knee <= 0f) ? 0f : ((num - 1f) / (4f * knee));
            float num6 = num2 - knee;
            float num7 = (x * dbRange) + dbMin;
            float num8 = num7;
            float num9 = num7 - num2;
            if ((num9 > -num4) && (num9 < num4))
            {
                num9 += num4;
                num8 = (num9 * ((num5 * num9) + 1f)) + num6;
            }
            else if (num9 > 0f)
            {
                num8 = num2 + (num * num9);
            }
            return (((2f * ((num8 + num3) - dbMin)) / dbRange) - 1f);
        }

        public override bool OnGUI(IAudioEffectPlugin plugin)
        {
            float num2;
            float num3;
            float num4;
            float num5;
            float num6;
            float num7;
            float[] numArray;
            float blend = !plugin.IsPluginEditableAndEnabled() ? 0.5f : 1f;
            plugin.GetFloatParameter(kThresholdName, out num2);
            plugin.GetFloatParameter(kRatioName, out num3);
            plugin.GetFloatParameter(kMakeupGainName, out num4);
            plugin.GetFloatParameter(kAttackTimeName, out num5);
            plugin.GetFloatParameter(kReleaseTimeName, out num6);
            plugin.GetFloatParameter(kKneeName, out num7);
            plugin.GetFloatBuffer("Metering", out numArray, 2);
            GUILayout.Space(5f);
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandWidth(true) };
            Rect rect = GUILayoutUtility.GetRect((float) 200f, (float) 160f, options);
            if (CurveDisplay(plugin, rect, ref num2, ref num3, ref num4, ref num5, ref num6, ref num7, numArray[0], numArray[1], blend))
            {
                plugin.SetFloatParameter(kThresholdName, num2);
                plugin.SetFloatParameter(kRatioName, num3);
                plugin.SetFloatParameter(kMakeupGainName, num4);
                plugin.SetFloatParameter(kAttackTimeName, num5);
                plugin.SetFloatParameter(kReleaseTimeName, num6);
                plugin.SetFloatParameter(kKneeName, num7);
            }
            return true;
        }

        protected static Color ScaleAlpha(Color col, float blend) => 
            new Color(col.r, col.g, col.b, col.a * blend);

        public override string Description =>
            "Volume Ducking";

        public override string Name =>
            "Duck Volume";

        public override string Vendor =>
            "Unity Technologies";

        [CompilerGenerated]
        private sealed class <CurveDisplay>c__AnonStorey0
        {
            internal float blend;
            internal float dbMin;
            internal float dbRange;
        }

        [CompilerGenerated]
        private sealed class <CurveDisplay>c__AnonStorey1
        {
            internal DuckVolumeGUI.<CurveDisplay>c__AnonStorey0 <>f__ref$0;
            internal float duckGradient;
            internal float duckKnee;
            internal float duckKneeC1;
            internal float duckKneeC2;
            internal float duckMakeupGain;
            internal float duckSidechainLevel;
            internal float duckThreshold;

            internal float <>m__0(float x, out Color col)
            {
                float num = (x * this.<>f__ref$0.dbRange) + this.<>f__ref$0.dbMin;
                float num2 = num;
                float num3 = num - this.duckThreshold;
                col = DuckVolumeGUI.ScaleAlpha((this.duckSidechainLevel <= num) ? Color.grey : AudioCurveRendering.kAudioOrange, this.<>f__ref$0.blend);
                if ((num3 > -this.duckKnee) && (num3 < this.duckKnee))
                {
                    num3 += this.duckKnee;
                    num2 = (num3 * ((this.duckKneeC1 * num3) + 1f)) + this.duckKneeC2;
                    if (DuckVolumeGUI.dragtype == DuckVolumeGUI.DragType.ThresholdAndKnee)
                    {
                        col = new Color(col.r * 1.2f, col.g * 1.2f, col.b * 1.2f);
                    }
                }
                else if (num3 > 0f)
                {
                    num2 = this.duckThreshold + (this.duckGradient * num3);
                }
                return (((2f * ((num2 + this.duckMakeupGain) - this.<>f__ref$0.dbMin)) / this.<>f__ref$0.dbRange) - 1f);
            }

            internal float <>m__1(float x)
            {
                float num = (x * this.<>f__ref$0.dbRange) + this.<>f__ref$0.dbMin;
                float num2 = num;
                float num3 = num - this.duckThreshold;
                if ((num3 > -this.duckKnee) && (num3 < this.duckKnee))
                {
                    num3 += this.duckKnee;
                    num2 = (num3 * ((this.duckKneeC1 * num3) + 1f)) + this.duckKneeC2;
                }
                else if (num3 > 0f)
                {
                    num2 = this.duckThreshold + (this.duckGradient * num3);
                }
                return (((2f * ((num2 + this.duckMakeupGain) - this.<>f__ref$0.dbMin)) / this.<>f__ref$0.dbRange) - 1f);
            }
        }

        public enum DragType
        {
            None,
            ThresholdAndKnee,
            Ratio,
            MakeupGain
        }
    }
}

