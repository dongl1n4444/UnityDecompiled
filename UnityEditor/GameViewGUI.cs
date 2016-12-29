namespace UnityEditor
{
    using System;
    using System.Text;
    using UnityEngine;
    using UnityEngine.Scripting;

    internal class GameViewGUI
    {
        private static float m_ClientFrameTime = 0f;
        private static float m_ClientTimeAccumulator = 0f;
        private static int m_FrameCounter = 0;
        private static float m_MaxFrameTime = 0f;
        private static float m_MaxTimeAccumulator = 0f;
        private static float m_RenderFrameTime = 0f;
        private static float m_RenderTimeAccumulator = 0f;
        private static GUIStyle s_LabelStyle;
        private static GUIStyle s_SectionHeaderStyle;

        private static string FormatDb(float vol)
        {
            if (vol == 0f)
            {
                return "-∞ dB";
            }
            return $"{(20f * Mathf.Log10(vol)):F1} dB";
        }

        private static string FormatNumber(int num)
        {
            if (num < 0x3e8)
            {
                return num.ToString();
            }
            if (num < 0xf4240)
            {
                double num2 = num * 0.001;
                return (num2.ToString("f1") + "k");
            }
            double num3 = num * 1E-06;
            return (num3.ToString("f1") + "M");
        }

        [RequiredByNativeCode]
        public static void GameViewStatsGUI()
        {
            GUI.skin = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Scene);
            GUI.color = new Color(1f, 1f, 1f, 0.75f);
            float width = 300f;
            float height = 204f;
            int length = Network.connections.Length;
            if (length != 0)
            {
                height += 220f;
            }
            GUILayout.BeginArea(new Rect((GUIView.current.position.width - width) - 10f, 27f, width, height), "Statistics", GUI.skin.window);
            GUILayout.Label("Audio:", sectionHeaderStyle, new GUILayoutOption[0]);
            StringBuilder builder = new StringBuilder(400);
            float audioLevel = UnityStats.audioLevel;
            builder.Append("  Level: " + FormatDb(audioLevel) + (!EditorUtility.audioMasterMute ? "\n" : " (MUTED)\n"));
            builder.Append($"  Clipping: {100f * UnityStats.audioClippingAmount:F1}%");
            GUILayout.Label(builder.ToString(), new GUILayoutOption[0]);
            GUI.Label(new Rect(170f, 40f, 120f, 20f), $"DSP load: {100f * UnityStats.audioDSPLoad:F1}%");
            GUI.Label(new Rect(170f, 53f, 120f, 20f), $"Stream load: {100f * UnityStats.audioStreamLoad:F1}%");
            GUILayout.Label("Graphics:", sectionHeaderStyle, new GUILayoutOption[0]);
            UpdateFrameTime();
            string text = $"{1f / Mathf.Max(m_MaxFrameTime, 1E-05f):F1} FPS ({m_MaxFrameTime * 1000f:F1}ms)";
            GUI.Label(new Rect(170f, 75f, 120f, 20f), text);
            int screenBytes = UnityStats.screenBytes;
            int num6 = UnityStats.dynamicBatchedDrawCalls - UnityStats.dynamicBatches;
            int num7 = UnityStats.staticBatchedDrawCalls - UnityStats.staticBatches;
            int num8 = UnityStats.instancedBatchedDrawCalls - UnityStats.instancedBatches;
            StringBuilder builder2 = new StringBuilder(400);
            if (m_ClientFrameTime > m_RenderFrameTime)
            {
                builder2.Append($"  CPU: main <b>{m_ClientFrameTime * 1000f:F1}</b>ms  render thread {m_RenderFrameTime * 1000f:F1}ms
");
            }
            else
            {
                builder2.Append($"  CPU: main {m_ClientFrameTime * 1000f:F1}ms  render thread <b>{m_RenderFrameTime * 1000f:F1}</b>ms
");
            }
            builder2.Append($"  Batches: <b>{UnityStats.batches}</b> 	Saved by batching: {(num6 + num7) + num8}
");
            builder2.Append($"  Tris: {FormatNumber(UnityStats.triangles)} 	Verts: {FormatNumber(UnityStats.vertices)} 
");
            builder2.Append($"  Screen: {UnityStats.screenRes} - {EditorUtility.FormatBytes(screenBytes)}
");
            builder2.Append($"  SetPass calls: {UnityStats.setPassCalls} 	Shadow casters: {UnityStats.shadowCasters} 
");
            builder2.Append($"  Visible skinned meshes: {UnityStats.visibleSkinnedMeshes}  Animations: {UnityStats.visibleAnimations}");
            GUILayout.Label(builder2.ToString(), labelStyle, new GUILayoutOption[0]);
            if (length != 0)
            {
                GUILayout.Label("Network:", sectionHeaderStyle, new GUILayoutOption[0]);
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                for (int i = 0; i < length; i++)
                {
                    GUILayout.Label(UnityStats.GetNetworkStats(i), new GUILayoutOption[0]);
                }
                GUILayout.EndHorizontal();
            }
            else
            {
                GUILayout.Label("Network: (no players connected)", sectionHeaderStyle, new GUILayoutOption[0]);
            }
            GUILayout.EndArea();
        }

        private static void UpdateFrameTime()
        {
            if (Event.current.type == EventType.Repaint)
            {
                float frameTime = UnityStats.frameTime;
                float renderTime = UnityStats.renderTime;
                m_ClientTimeAccumulator += frameTime;
                m_RenderTimeAccumulator += renderTime;
                m_MaxTimeAccumulator += Mathf.Max(frameTime, renderTime);
                m_FrameCounter++;
                bool flag = (m_ClientFrameTime == 0f) && (m_RenderFrameTime == 0f);
                bool flag2 = ((m_FrameCounter > 30) || (m_ClientTimeAccumulator > 0.3f)) || (m_RenderTimeAccumulator > 0.3f);
                if (flag || flag2)
                {
                    m_ClientFrameTime = m_ClientTimeAccumulator / ((float) m_FrameCounter);
                    m_RenderFrameTime = m_RenderTimeAccumulator / ((float) m_FrameCounter);
                    m_MaxFrameTime = m_MaxTimeAccumulator / ((float) m_FrameCounter);
                }
                if (flag2)
                {
                    m_ClientTimeAccumulator = 0f;
                    m_RenderTimeAccumulator = 0f;
                    m_MaxTimeAccumulator = 0f;
                    m_FrameCounter = 0;
                }
            }
        }

        private static GUIStyle labelStyle
        {
            get
            {
                if (s_LabelStyle == null)
                {
                    s_LabelStyle = new GUIStyle(EditorGUIUtility.GetBuiltinSkin(EditorSkin.Scene).label);
                    s_LabelStyle.richText = true;
                }
                return s_LabelStyle;
            }
        }

        private static GUIStyle sectionHeaderStyle
        {
            get
            {
                if (s_SectionHeaderStyle == null)
                {
                    s_SectionHeaderStyle = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Scene).GetStyle("BoldLabel");
                }
                return s_SectionHeaderStyle;
            }
        }
    }
}

