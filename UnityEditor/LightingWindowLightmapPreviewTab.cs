namespace UnityEditor
{
    using System;
    using System.Collections;
    using UnityEditorInternal;
    using UnityEngine;

    internal class LightingWindowLightmapPreviewTab
    {
        private Vector2 m_ScrollPositionLightmaps = Vector2.zero;
        private Vector2 m_ScrollPositionMaps = Vector2.zero;
        private int m_SelectedLightmap = -1;
        private static Styles s_Styles;

        private static void DrawHeader(Rect rect, bool showdrawDirectionalityHeader, bool showShadowMaskHeader, float maxLightmaps)
        {
            rect.width /= maxLightmaps;
            EditorGUI.DropShadowLabel(rect, "Intensity");
            rect.x += rect.width;
            if (showdrawDirectionalityHeader)
            {
                EditorGUI.DropShadowLabel(rect, "Directionality");
                rect.x += rect.width;
            }
            if (showShadowMaskHeader)
            {
                EditorGUI.DropShadowLabel(rect, "Shadowmask");
            }
        }

        private Texture2D LightmapField(Texture2D lightmap, int index)
        {
            Rect rect = GUILayoutUtility.GetRect(100f, 100f, EditorStyles.objectField);
            this.MenuSelectLightmapUsers(rect, index);
            Texture2D textured = EditorGUI.ObjectField(rect, lightmap, typeof(Texture2D), false) as Texture2D;
            if ((index == this.m_SelectedLightmap) && (Event.current.type == EventType.Repaint))
            {
                s_Styles.selectedLightmapHighlight.Draw(rect, false, false, false, false);
            }
            return textured;
        }

        public void LightmapPreview(Rect r)
        {
            if (s_Styles == null)
            {
                s_Styles = new Styles();
            }
            GUI.Box(r, "", "PreBackground");
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Height(r.height) };
            this.m_ScrollPositionLightmaps = EditorGUILayout.BeginScrollView(this.m_ScrollPositionLightmaps, options);
            int lightmapIndex = 0;
            bool showdrawDirectionalityHeader = false;
            bool showShadowMaskHeader = false;
            foreach (LightmapData data in LightmapSettings.lightmaps)
            {
                if (data.lightmapDir != null)
                {
                    showdrawDirectionalityHeader = true;
                }
                if (data.shadowMask != null)
                {
                    showShadowMaskHeader = true;
                }
            }
            float maxLightmaps = 1f;
            if (showdrawDirectionalityHeader)
            {
                maxLightmaps++;
            }
            if (showShadowMaskHeader)
            {
                maxLightmaps++;
            }
            DrawHeader(GUILayoutUtility.GetRect(r.width, r.width, (float) 20f, (float) 20f), showdrawDirectionalityHeader, showShadowMaskHeader, maxLightmaps);
            foreach (LightmapData data2 in LightmapSettings.lightmaps)
            {
                if (((data2.lightmapColor == null) && (data2.lightmapDir == null)) && (data2.shadowMask == null))
                {
                    lightmapIndex++;
                }
                else
                {
                    Texture2D textured;
                    int num5 = (data2.lightmapColor == null) ? -1 : Math.Max(data2.lightmapColor.width, data2.lightmapColor.height);
                    int num6 = (data2.lightmapDir == null) ? -1 : Math.Max(data2.lightmapDir.width, data2.lightmapDir.height);
                    int num7 = (data2.shadowMask == null) ? -1 : Math.Max(data2.shadowMask.width, data2.shadowMask.height);
                    if (num5 > num6)
                    {
                        textured = (num5 <= num7) ? data2.shadowMask : data2.lightmapColor;
                    }
                    else
                    {
                        textured = (num6 <= num7) ? data2.shadowMask : data2.lightmapDir;
                    }
                    GUILayoutOption[] optionArray = new GUILayoutOption[] { GUILayout.MaxWidth(r.width), GUILayout.MaxHeight((float) textured.height) };
                    Rect aspectRect = GUILayoutUtility.GetAspectRect(maxLightmaps, optionArray);
                    float num8 = 5f;
                    aspectRect.width /= maxLightmaps;
                    aspectRect.width -= num8;
                    aspectRect.x += num8 / 2f;
                    EditorGUI.DrawPreviewTexture(aspectRect, data2.lightmapColor);
                    this.MenuSelectLightmapUsers(aspectRect, lightmapIndex);
                    if (data2.lightmapDir != null)
                    {
                        aspectRect.x += aspectRect.width + num8;
                        EditorGUI.DrawPreviewTexture(aspectRect, data2.lightmapDir);
                        this.MenuSelectLightmapUsers(aspectRect, lightmapIndex);
                    }
                    if (data2.shadowMask != null)
                    {
                        aspectRect.x += aspectRect.width + num8;
                        EditorGUI.DrawPreviewTexture(aspectRect, data2.shadowMask);
                        this.MenuSelectLightmapUsers(aspectRect, lightmapIndex);
                    }
                    GUILayout.Space(10f);
                    lightmapIndex++;
                }
            }
            EditorGUILayout.EndScrollView();
        }

        public void Maps()
        {
            if (s_Styles == null)
            {
                s_Styles = new Styles();
            }
            GUI.changed = false;
            if (Lightmapping.giWorkflowMode == Lightmapping.GIWorkflowMode.OnDemand)
            {
                SerializedObject obj2 = new SerializedObject(LightmapEditorSettings.GetLightmapSettings());
                EditorGUILayout.PropertyField(obj2.FindProperty("m_LightingDataAsset"), s_Styles.LightingDataAsset, new GUILayoutOption[0]);
                obj2.ApplyModifiedProperties();
            }
            GUILayout.Space(10f);
            LightmapData[] lightmaps = LightmapSettings.lightmaps;
            this.m_ScrollPositionMaps = GUILayout.BeginScrollView(this.m_ScrollPositionMaps, new GUILayoutOption[0]);
            using (new EditorGUI.DisabledScope(true))
            {
                bool flag = false;
                bool flag2 = false;
                foreach (LightmapData data in lightmaps)
                {
                    if (data.lightmapDir != null)
                    {
                        flag = true;
                    }
                    if (data.shadowMask != null)
                    {
                        flag2 = true;
                    }
                }
                for (int i = 0; i < lightmaps.Length; i++)
                {
                    GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                    GUILayout.Space(5f);
                    lightmaps[i].lightmapColor = this.LightmapField(lightmaps[i].lightmapColor, i);
                    if (flag)
                    {
                        GUILayout.Space(10f);
                        lightmaps[i].lightmapDir = this.LightmapField(lightmaps[i].lightmapDir, i);
                    }
                    if (flag2)
                    {
                        GUILayout.Space(10f);
                        lightmaps[i].shadowMask = this.LightmapField(lightmaps[i].shadowMask, i);
                    }
                    GUILayout.Space(5f);
                    LightmapConvergence lightmapConvergence = Lightmapping.GetLightmapConvergence(i);
                    GUILayout.BeginVertical(new GUILayoutOption[0]);
                    GUILayout.Label("Index: " + i, EditorStyles.miniBoldLabel, new GUILayoutOption[0]);
                    if (lightmapConvergence.IsValid())
                    {
                        GUILayout.Label("Occupied: " + InternalEditorUtility.CountToString((ulong) lightmapConvergence.occupiedTexelCount), EditorStyles.miniLabel, new GUILayoutOption[0]);
                        object[] objArray1 = new object[] { "Direct: ", lightmapConvergence.minDirectSamples, " / ", lightmapConvergence.maxDirectSamples, " / ", lightmapConvergence.avgDirectSamples, "|min / max / avg samples per texel" };
                        GUILayout.Label(EditorGUIUtility.TextContent(string.Concat(objArray1)), EditorStyles.miniLabel, new GUILayoutOption[0]);
                        object[] objArray2 = new object[] { "GI: ", lightmapConvergence.minGISamples, " / ", lightmapConvergence.maxGISamples, " / ", lightmapConvergence.avgGISamples, "|min / max / avg samples per texel" };
                        GUILayout.Label(EditorGUIUtility.TextContent(string.Concat(objArray2)), EditorStyles.miniLabel, new GUILayoutOption[0]);
                    }
                    else
                    {
                        GUILayout.Label("Occupied: N/A", EditorStyles.miniLabel, new GUILayoutOption[0]);
                        GUILayout.Label("Direct: N/A", EditorStyles.miniLabel, new GUILayoutOption[0]);
                        GUILayout.Label("GI: N/A", EditorStyles.miniLabel, new GUILayoutOption[0]);
                    }
                    float lightmapBakePerformance = Lightmapping.GetLightmapBakePerformance(i);
                    if (lightmapBakePerformance >= 0.0)
                    {
                        GUILayout.Label(lightmapBakePerformance.ToString("0.00") + " mrays/sec", EditorStyles.miniLabel, new GUILayoutOption[0]);
                    }
                    else
                    {
                        GUILayout.Label("N/A mrays/sec", EditorStyles.miniLabel, new GUILayoutOption[0]);
                    }
                    GUILayout.EndVertical();
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.EndScrollView();
        }

        private void MenuSelectLightmapUsers(Rect rect, int lightmapIndex)
        {
            if ((Event.current.type == EventType.ContextClick) && rect.Contains(Event.current.mousePosition))
            {
                string[] texts = new string[] { "Select Lightmap Users" };
                Rect position = new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 1f, 1f);
                EditorUtility.DisplayCustomMenu(position, EditorGUIUtility.TempContent(texts), -1, new EditorUtility.SelectMenuItemFunction(this.SelectLightmapUsers), lightmapIndex);
                Event.current.Use();
            }
        }

        private void SelectLightmapUsers(object userData, string[] options, int selected)
        {
            int num = (int) userData;
            ArrayList list = new ArrayList();
            MeshRenderer[] rendererArray = UnityEngine.Object.FindObjectsOfType(typeof(MeshRenderer)) as MeshRenderer[];
            foreach (MeshRenderer renderer in rendererArray)
            {
                if ((renderer != null) && (renderer.lightmapIndex == num))
                {
                    list.Add(renderer.gameObject);
                }
            }
            Terrain[] terrainArray = UnityEngine.Object.FindObjectsOfType(typeof(Terrain)) as Terrain[];
            foreach (Terrain terrain in terrainArray)
            {
                if ((terrain != null) && (terrain.lightmapIndex == num))
                {
                    list.Add(terrain.gameObject);
                }
            }
            Selection.objects = list.ToArray(typeof(UnityEngine.Object)) as UnityEngine.Object[];
        }

        public void UpdateLightmapSelection()
        {
            MeshRenderer renderer;
            Terrain terrain = null;
            if ((Selection.activeGameObject == null) || (((renderer = Selection.activeGameObject.GetComponent<MeshRenderer>()) == null) && ((terrain = Selection.activeGameObject.GetComponent<Terrain>()) == null)))
            {
                this.m_SelectedLightmap = -1;
            }
            else
            {
                this.m_SelectedLightmap = (renderer == null) ? terrain.lightmapIndex : renderer.lightmapIndex;
            }
        }

        private class Styles
        {
            public GUIContent LightingDataAsset = EditorGUIUtility.TextContent("Lighting Data Asset|A different LightingData.asset can be assigned here. These assets are generated by baking a scene in the OnDemand mode.");
            public GUIContent LightProbes = EditorGUIUtility.TextContent("Light Probes|A different LightProbes.asset can be assigned here. These assets are generated by baking a scene containing light probes.");
            public GUIContent MapsArraySize = EditorGUIUtility.TextContent("Array Size|The length of the array of lightmaps.");
            public GUIStyle selectedLightmapHighlight = "LightmapEditorSelectedHighlight";
        }
    }
}

