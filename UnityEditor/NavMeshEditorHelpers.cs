namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class NavMeshEditorHelpers
    {
        public static void DrawAgentDiagram(Rect rect, float agentRadius, float agentHeight, float agentClimb, float agentSlope)
        {
            if (Event.current.type == EventType.Repaint)
            {
                float num = agentRadius;
                float b = agentHeight;
                float num3 = agentClimb;
                float num4 = 1f;
                float num5 = 0.35f;
                float num6 = 20f;
                float num7 = 10f;
                float num8 = rect.height - (num6 + num7);
                num4 = Mathf.Min((float) (num8 / (b + ((num * 2f) * num5))), (float) (num8 / (num * 2f)));
                b *= num4;
                num *= num4;
                num3 *= num4;
                float x = rect.xMin + (rect.width * 0.5f);
                float y = (rect.yMax - num7) - (num * num5);
                Vector3[] points = new Vector3[40];
                Vector3[] vectorArray2 = new Vector3[20];
                Vector3[] vectorArray3 = new Vector3[20];
                for (int i = 0; i < 20; i++)
                {
                    float f = (((float) i) / 19f) * 3.141593f;
                    float num13 = Mathf.Cos(f);
                    float num14 = Mathf.Sin(f);
                    points[i] = new Vector3(x + (num13 * num), (y - b) - ((num14 * num) * num5), 0f);
                    points[i + 20] = new Vector3(x - (num13 * num), y + ((num14 * num) * num5), 0f);
                    vectorArray2[i] = new Vector3(x - (num13 * num), (y - b) + ((num14 * num) * num5), 0f);
                    vectorArray3[i] = new Vector3(x - (num13 * num), (y - num3) + ((num14 * num) * num5), 0f);
                }
                Color color = Handles.color;
                float xMin = rect.xMin;
                float num16 = y - num3;
                float num17 = x - (num8 * 0.75f);
                float num18 = y;
                float num19 = x + (num8 * 0.75f);
                float num20 = y;
                float num21 = num19;
                float num22 = num20;
                float num23 = Mathf.Min(rect.xMax - num19, b);
                num21 += Mathf.Cos(agentSlope * 0.01745329f) * num23;
                num22 -= Mathf.Sin(agentSlope * 0.01745329f) * num23;
                Vector3[] vectorArray4 = new Vector3[] { new Vector3(xMin, y, 0f), new Vector3(num21, y, 0f) };
                Vector3[] vectorArray5 = new Vector3[] { new Vector3(xMin, num16, 0f), new Vector3(num17, num16, 0f), new Vector3(num17, num18, 0f), new Vector3(num19, num20, 0f), new Vector3(num21, num22, 0f) };
                Handles.color = !EditorGUIUtility.isProSkin ? new Color(1f, 1f, 1f, 0.5f) : new Color(0f, 0f, 0f, 0.5f);
                Handles.DrawAAPolyLine((float) 2f, vectorArray4);
                Handles.color = !EditorGUIUtility.isProSkin ? new Color(0f, 0f, 0f, 0.5f) : new Color(1f, 1f, 1f, 0.5f);
                Handles.DrawAAPolyLine((float) 3f, vectorArray5);
                Handles.color = Color.Lerp(new Color(0f, 0.75f, 1f, 1f), new Color(0.5f, 0.5f, 0.5f, 0.5f), 0.2f);
                Handles.DrawAAConvexPolygon(points);
                Handles.color = new Color(0f, 0f, 0f, 0.5f);
                Handles.DrawAAPolyLine((float) 2f, vectorArray3);
                Handles.color = new Color(1f, 1f, 1f, 0.4f);
                Handles.DrawAAPolyLine((float) 2f, vectorArray2);
                Vector3[] vectorArray6 = new Vector3[] { new Vector3(x, y - b, 0f), new Vector3(x + num, y - b, 0f) };
                Handles.color = new Color(0f, 0f, 0f, 0.5f);
                Handles.DrawAAPolyLine((float) 2f, vectorArray6);
                GUI.Label(new Rect((x + num) + 5f, (y - (b * 0.5f)) - 10f, 150f, 20f), string.Format("H = {0}", agentHeight));
                GUI.Label(new Rect(x, ((y - b) - (num * num5)) - 15f, 150f, 20f), string.Format("R = {0}", agentRadius));
                GUI.Label(new Rect(((xMin + num17) * 0.5f) - 20f, num16 - 15f, 150f, 20f), string.Format("{0}", agentClimb));
                GUI.Label(new Rect(num19 + 20f, num20 - 15f, 150f, 20f), string.Format("{0}\x00b0", agentSlope));
                Handles.color = color;
            }
        }
    }
}

