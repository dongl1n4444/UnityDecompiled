namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    /// <summary>
    /// <para>Antialiased curve rendering functionality used by audio tools in the editor.</para>
    /// </summary>
    public class AudioCurveRendering
    {
        public static readonly Color kAudioOrange = new Color(1f, 0.6588235f, 0.02745098f);
        private static float pixelEpsilon = 0.005f;
        private static Vector3[] s_PointCache;

        public static Rect BeginCurveFrame(Rect r)
        {
            DrawCurveBackground(r);
            r = DrawCurveFrame(r);
            GUI.BeginGroup(r);
            return new Rect(0f, 0f, r.width, r.height);
        }

        public static void DrawCurve(Rect r, AudioCurveEvaluator eval, Color curveColor)
        {
            if (Event.current.type == EventType.Repaint)
            {
                HandleUtility.ApplyWireMaterial();
                int numPoints = (int) Mathf.Ceil(r.width);
                float num2 = r.height * 0.5f;
                float num3 = 1f / ((float) (numPoints - 1));
                Vector3[] pointCache = GetPointCache(numPoints);
                for (int i = 0; i < numPoints; i++)
                {
                    pointCache[i].x = i + r.x;
                    pointCache[i].y = (num2 - (num2 * eval(i * num3))) + r.y;
                    pointCache[i].z = 0f;
                }
                GUI.BeginClip(r);
                Handles.color = curveColor;
                Handles.DrawAAPolyLine(3f, numPoints, pointCache);
                GUI.EndClip();
            }
        }

        public static void DrawCurveBackground(Rect r)
        {
            EditorGUI.DrawRect(r, new Color(0.3f, 0.3f, 0.3f));
        }

        public static Rect DrawCurveFrame(Rect r)
        {
            if (Event.current.type == EventType.Repaint)
            {
                EditorStyles.colorPickerBox.Draw(r, false, false, false, false);
                r.x++;
                r.y++;
                r.width -= 2f;
                r.height -= 2f;
            }
            return r;
        }

        public static void DrawFilledCurve(Rect r, AudioCurveAndColorEvaluator eval)
        {
            if (Event.current.type == EventType.Repaint)
            {
                Color color;
                HandleUtility.ApplyWireMaterial();
                GL.Begin(1);
                float pixelsPerPoint = EditorGUIUtility.pixelsPerPoint;
                float num2 = 1f / pixelsPerPoint;
                float num3 = 0.5f * num2;
                float num4 = Mathf.Ceil(r.width) * pixelsPerPoint;
                float num5 = Mathf.Floor(r.x) + pixelEpsilon;
                float num6 = 1f / (num4 - 1f);
                float max = r.height * 0.5f;
                float num8 = r.y + (0.5f * r.height);
                float y = r.y + r.height;
                float b = Mathf.Clamp(max * eval(0f, out color), -max, max);
                for (int i = 0; i < num4; i++)
                {
                    float x = num5 + (i * num2);
                    float a = Mathf.Clamp(max * eval(i * num6, out color), -max, max);
                    float num14 = Mathf.Min(a, b) - num3;
                    float num15 = Mathf.Max(a, b) + num3;
                    GL.Color(new Color(color.r, color.g, color.b, 0f));
                    AudioMixerDrawUtils.Vertex(x, num8 - num15);
                    GL.Color(color);
                    AudioMixerDrawUtils.Vertex(x, num8 - num14);
                    AudioMixerDrawUtils.Vertex(x, num8 - num14);
                    AudioMixerDrawUtils.Vertex(x, y);
                    b = a;
                }
                GL.End();
            }
        }

        public static void DrawFilledCurve(Rect r, AudioCurveEvaluator eval, Color curveColor)
        {
            <DrawFilledCurve>c__AnonStorey0 storey = new <DrawFilledCurve>c__AnonStorey0 {
                curveColor = curveColor,
                eval = eval
            };
            DrawFilledCurve(r, new AudioCurveAndColorEvaluator(storey.<>m__0));
        }

        public static void DrawGradientRect(Rect r, Color c1, Color c2, float blend, bool horizontal)
        {
            if (Event.current.type == EventType.Repaint)
            {
                HandleUtility.ApplyWireMaterial();
                GL.Begin(7);
                if (horizontal)
                {
                    GL.Color(new Color(c1.r, c1.g, c1.b, c1.a * blend));
                    GL.Vertex3(r.x, r.y, 0f);
                    GL.Vertex3(r.x + r.width, r.y, 0f);
                    GL.Color(new Color(c2.r, c2.g, c2.b, c2.a * blend));
                    GL.Vertex3(r.x + r.width, r.y + r.height, 0f);
                    GL.Vertex3(r.x, r.y + r.height, 0f);
                }
                else
                {
                    GL.Color(new Color(c1.r, c1.g, c1.b, c1.a * blend));
                    GL.Vertex3(r.x, r.y + r.height, 0f);
                    GL.Vertex3(r.x, r.y, 0f);
                    GL.Color(new Color(c2.r, c2.g, c2.b, c2.a * blend));
                    GL.Vertex3(r.x + r.width, r.y, 0f);
                    GL.Vertex3(r.x + r.width, r.y + r.height, 0f);
                }
                GL.End();
            }
        }

        public static void DrawMinMaxFilledCurve(Rect r, AudioMinMaxCurveAndColorEvaluator eval)
        {
            float num9;
            float num10;
            Color color;
            HandleUtility.ApplyWireMaterial();
            GL.Begin(1);
            float pixelsPerPoint = EditorGUIUtility.pixelsPerPoint;
            float num2 = 1f / pixelsPerPoint;
            float num3 = 0.5f * num2;
            float num4 = Mathf.Ceil(r.width) * pixelsPerPoint;
            float num5 = Mathf.Floor(r.x) + pixelEpsilon;
            float num6 = 1f / (num4 - 1f);
            float num7 = r.height * 0.5f;
            float num8 = r.y + (0.5f * r.height);
            eval(0.0001f, out color, out num9, out num10);
            Sort2(ref num9, ref num10);
            float b = num8 - (num7 * Mathf.Clamp(num10, -1f, 1f));
            float num12 = num8 - (num7 * Mathf.Clamp(num9, -1f, 1f));
            float y = r.y;
            float max = r.y + r.height;
            for (int i = 0; i < num4; i++)
            {
                float x = num5 + (i * num2);
                eval(i * num6, out color, out num9, out num10);
                Sort2(ref num9, ref num10);
                Color c = new Color(color.r, color.g, color.b, 0f);
                float a = num8 - (num7 * Mathf.Clamp(num10, -1f, 1f));
                float num18 = num8 - (num7 * Mathf.Clamp(num9, -1f, 1f));
                float minValue = Mathf.Clamp(Mathf.Min(a, b) - num3, y, max);
                float num20 = Mathf.Clamp(Mathf.Max(a, b) + num3, y, max);
                float maxValue = Mathf.Clamp(Mathf.Min(num18, num12) - num3, y, max);
                float num22 = Mathf.Clamp(Mathf.Max(num18, num12) + num3, y, max);
                Sort2(ref minValue, ref maxValue);
                Sort2(ref num20, ref num22);
                Sort2(ref minValue, ref num20);
                Sort2(ref maxValue, ref num22);
                Sort2(ref num20, ref maxValue);
                GL.Color(c);
                AudioMixerDrawUtils.Vertex(x, minValue);
                GL.Color(color);
                AudioMixerDrawUtils.Vertex(x, num20);
                AudioMixerDrawUtils.Vertex(x, num20);
                AudioMixerDrawUtils.Vertex(x, maxValue);
                AudioMixerDrawUtils.Vertex(x, maxValue);
                GL.Color(c);
                AudioMixerDrawUtils.Vertex(x, num22);
                num12 = num18;
                b = a;
            }
            GL.End();
        }

        public static void DrawSymmetricFilledCurve(Rect r, AudioCurveAndColorEvaluator eval)
        {
            if (Event.current.type == EventType.Repaint)
            {
                Color color;
                HandleUtility.ApplyWireMaterial();
                GL.Begin(1);
                float pixelsPerPoint = EditorGUIUtility.pixelsPerPoint;
                float num2 = 1f / pixelsPerPoint;
                float num3 = 0.5f * num2;
                float num4 = Mathf.Ceil(r.width) * pixelsPerPoint;
                float num5 = Mathf.Floor(r.x) + pixelEpsilon;
                float num6 = 1f / (num4 - 1f);
                float max = r.height * 0.5f;
                float num8 = r.y + (0.5f * r.height);
                float b = Mathf.Clamp(max * eval(0.0001f, out color), 0f, max);
                for (int i = 0; i < num4; i++)
                {
                    float x = num5 + (i * num2);
                    float a = Mathf.Clamp(max * eval(i * num6, out color), 0f, max);
                    float num13 = Mathf.Max((float) (Mathf.Min(a, b) - num3), (float) 0f);
                    float num14 = Mathf.Min(Mathf.Max(a, b) + num3, max);
                    Color c = new Color(color.r, color.g, color.b, 0f);
                    GL.Color(c);
                    AudioMixerDrawUtils.Vertex(x, num8 - num14);
                    GL.Color(color);
                    AudioMixerDrawUtils.Vertex(x, num8 - num13);
                    AudioMixerDrawUtils.Vertex(x, num8 - num13);
                    AudioMixerDrawUtils.Vertex(x, num8 + num13);
                    AudioMixerDrawUtils.Vertex(x, num8 + num13);
                    GL.Color(c);
                    AudioMixerDrawUtils.Vertex(x, num8 + num14);
                    b = a;
                }
                GL.End();
            }
        }

        public static void EndCurveFrame()
        {
            GUI.EndGroup();
        }

        private static Vector3[] GetPointCache(int numPoints)
        {
            if ((s_PointCache == null) || (s_PointCache.Length != numPoints))
            {
                s_PointCache = new Vector3[numPoints];
            }
            return s_PointCache;
        }

        private static void Sort2(ref float minValue, ref float maxValue)
        {
            if (minValue > maxValue)
            {
                float num = minValue;
                minValue = maxValue;
                maxValue = num;
            }
        }

        [CompilerGenerated]
        private sealed class <DrawFilledCurve>c__AnonStorey0
        {
            internal Color curveColor;
            internal AudioCurveRendering.AudioCurveEvaluator eval;

            internal float <>m__0(float x, out Color color)
            {
                color = this.curveColor;
                return this.eval(x);
            }
        }

        /// <summary>
        /// <para>Curve evaluation function that allows simultaneous evaluation of the curve y-value and a color of the curve at that point.</para>
        /// </summary>
        /// <param name="x">Normalized x-position in the range [0; 1] at which the curve should be evaluated.</param>
        /// <param name="col">Color of the curve at the evaluated point.</param>
        public delegate float AudioCurveAndColorEvaluator(float x, out Color col);

        /// <summary>
        /// <para>Curve evaluation function used to evaluate the curve y-value and at the specified point.</para>
        /// </summary>
        /// <param name="x">Normalized x-position in the range [0; 1] at which the curve should be evaluated.</param>
        public delegate float AudioCurveEvaluator(float x);

        /// <summary>
        /// <para>Curve evaluation function that allows simultaneous evaluation of the min- and max-curves. The returned minValue and maxValue values are expected to be in the range [-1; 1] and a value of 0 corresponds to the vertical center of the rectangle that is drawn into. Values outside of this range will be clamped. Additionally the color of the curve at this point is evaluated.</para>
        /// </summary>
        /// <param name="x">Normalized x-position in the range [0; 1] at which the min- and max-curves should be evaluated.</param>
        /// <param name="col">Color of the curve at the specified evaluation point.</param>
        /// <param name="minValue">Returned value of the minimum curve. Clamped to [-1; 1].</param>
        /// <param name="maxValue">Returned value of the maximum curve. Clamped to [-1; 1].</param>
        public delegate void AudioMinMaxCurveAndColorEvaluator(float x, out Color col, out float minValue, out float maxValue);
    }
}

