namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using UnityEditor.AnimatedValues;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngineInternal;

    internal class LightingWindowObjectTab
    {
        private static GUIContent[] kObjectPreviewTextureOptions = new GUIContent[] { EditorGUIUtility.TextContent("Charting"), EditorGUIUtility.TextContent("Albedo"), EditorGUIUtility.TextContent("Emissive"), EditorGUIUtility.TextContent("Realtime Intensity"), EditorGUIUtility.TextContent("Realtime Direction"), EditorGUIUtility.TextContent("Baked Intensity"), EditorGUIUtility.TextContent("Baked Direction"), EditorGUIUtility.TextContent("Baked Shadowmask") };
        private GITextureType[] kObjectPreviewTextureTypes = new GITextureType[] { GITextureType.Charting };
        private Editor m_LightEditor;
        private Editor m_LightmapParametersEditor;
        private int m_PreviousSelection;
        private GUIContent m_SelectedObjectPreviewTexture;
        private AnimBool m_ShowClampedSize = new AnimBool();
        private ZoomableArea m_ZoomablePreview;

        private Rect CenterToRect(Rect rect, Rect to)
        {
            float num = Mathf.Clamp((float) (((float) ((int) (to.width - rect.width))) / 2f), (float) 0f, (float) 2.147484E+09f);
            float num2 = Mathf.Clamp((float) (((float) ((int) (to.height - rect.height))) / 2f), (float) 0f, (float) 2.147484E+09f);
            return new Rect(rect.x + num, rect.y + num2, rect.width, rect.height);
        }

        private static bool isBuiltIn(SerializedProperty prop)
        {
            if (prop.objectReferenceValue != null)
            {
                LightmapParameters objectReferenceValue = prop.objectReferenceValue as LightmapParameters;
                return (objectReferenceValue.hideFlags == HideFlags.NotEditable);
            }
            return true;
        }

        public static bool LightmapParametersGUI(SerializedProperty prop, GUIContent content, bool advancedParameters)
        {
            EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
            if (advancedParameters)
            {
                EditorGUIInternal.AssetPopup<LightmapParameters>(prop, content, "giparams", "Scene Default Parameters");
            }
            else
            {
                EditorGUIInternal.AssetPopup<LightmapParameters>(prop, content, "giparams", "Default-Medium");
            }
            string text = "Edit...";
            if (isBuiltIn(prop))
            {
                text = "View";
            }
            bool flag = false;
            if (prop.objectReferenceValue == null)
            {
                SerializedProperty property = new SerializedObject(LightmapEditorSettings.GetLightmapSettings()).FindProperty("m_LightmapEditorSettings.m_LightmapParameters");
                using (new EditorGUI.DisabledScope(property == null))
                {
                    GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandWidth(false) };
                    if (GUILayout.Button(text, EditorStyles.miniButton, options))
                    {
                        Selection.activeObject = property.objectReferenceValue;
                        flag = true;
                    }
                }
            }
            else
            {
                GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.ExpandWidth(false) };
                if (GUILayout.Button(text, EditorStyles.miniButton, optionArray2))
                {
                    Selection.activeObject = prop.objectReferenceValue;
                    flag = true;
                }
            }
            EditorGUILayout.EndHorizontal();
            return flag;
        }

        public void ObjectPreview(Rect r)
        {
            if (r.height > 0f)
            {
                List<Texture2D> list = new List<Texture2D>();
                foreach (GITextureType type in this.kObjectPreviewTextureTypes)
                {
                    list.Add(LightmapVisualizationUtility.GetGITexture(type));
                }
                if (list.Count != 0)
                {
                    if (this.m_ZoomablePreview == null)
                    {
                        this.m_ZoomablePreview = new ZoomableArea(true);
                        this.m_ZoomablePreview.hRangeMin = 0f;
                        this.m_ZoomablePreview.vRangeMin = 0f;
                        this.m_ZoomablePreview.hRangeMax = 1f;
                        this.m_ZoomablePreview.vRangeMax = 1f;
                        this.m_ZoomablePreview.SetShownHRange(0f, 1f);
                        this.m_ZoomablePreview.SetShownVRange(0f, 1f);
                        this.m_ZoomablePreview.uniformScale = true;
                        this.m_ZoomablePreview.scaleWithWindow = true;
                    }
                    GUI.Box(r, "", "PreBackground");
                    Rect position = new Rect(r);
                    position.y++;
                    position.height = 18f;
                    GUI.Box(position, "", EditorStyles.toolbar);
                    Rect rect2 = new Rect(r);
                    rect2.y++;
                    rect2.height = 18f;
                    rect2.width = 120f;
                    Rect to = new Rect(r);
                    to.yMin += rect2.height;
                    to.yMax -= 14f;
                    to.width -= 11f;
                    int index = Array.IndexOf<GUIContent>(kObjectPreviewTextureOptions, this.m_SelectedObjectPreviewTexture);
                    if (index < 0)
                    {
                        index = 0;
                    }
                    index = EditorGUI.Popup(rect2, index, kObjectPreviewTextureOptions, EditorStyles.toolbarPopup);
                    if (index >= kObjectPreviewTextureOptions.Length)
                    {
                        index = 0;
                    }
                    this.m_SelectedObjectPreviewTexture = kObjectPreviewTextureOptions[index];
                    LightmapType lightmapType = (((this.kObjectPreviewTextureTypes[index] != GITextureType.BakedShadowMask) && (this.kObjectPreviewTextureTypes[index] != GITextureType.Baked)) && (this.kObjectPreviewTextureTypes[index] != GITextureType.BakedDirectional)) ? LightmapType.DynamicLightmap : LightmapType.StaticLightmap;
                    switch (Event.current.type)
                    {
                        case EventType.ValidateCommand:
                        case EventType.ExecuteCommand:
                            if (Event.current.commandName == "FrameSelected")
                            {
                                Vector4 lightmapTilingOffset = LightmapVisualizationUtility.GetLightmapTilingOffset(lightmapType);
                                Vector2 lhs = new Vector2(lightmapTilingOffset.z, lightmapTilingOffset.w);
                                Vector2 vector3 = lhs + new Vector2(lightmapTilingOffset.x, lightmapTilingOffset.y);
                                lhs = Vector2.Max(lhs, Vector2.zero);
                                vector3 = Vector2.Min(vector3, Vector2.one);
                                float num3 = 1f - lhs.y;
                                lhs.y = 1f - vector3.y;
                                vector3.y = num3;
                                Rect rect4 = new Rect(lhs.x, lhs.y, vector3.x - lhs.x, vector3.y - lhs.y);
                                rect4.x -= Mathf.Clamp(rect4.height - rect4.width, 0f, float.MaxValue) / 2f;
                                rect4.y -= Mathf.Clamp(rect4.width - rect4.height, 0f, float.MaxValue) / 2f;
                                float num4 = Mathf.Max(rect4.width, rect4.height);
                                rect4.height = num4;
                                rect4.width = num4;
                                this.m_ZoomablePreview.shownArea = rect4;
                                Event.current.Use();
                            }
                            break;

                        case EventType.Repaint:
                        {
                            Texture2D texture = list[index];
                            if ((texture != null) && (Event.current.type == EventType.Repaint))
                            {
                                Rect rect = new Rect(0f, 0f, (float) texture.width, (float) texture.height);
                                rect = this.ResizeRectToFit(rect, to);
                                rect = this.CenterToRect(rect, to);
                                rect = this.ScaleRectByZoomableArea(rect, this.m_ZoomablePreview);
                                Rect rect6 = new Rect(rect);
                                rect6.x += 3f;
                                rect6.y += to.y + 20f;
                                Rect drawableArea = new Rect(to);
                                drawableArea.y += rect2.height + 3f;
                                float num5 = drawableArea.y - 14f;
                                rect6.y -= num5;
                                drawableArea.y -= num5;
                                FilterMode filterMode = texture.filterMode;
                                texture.filterMode = FilterMode.Point;
                                GITextureType textureType = this.kObjectPreviewTextureTypes[index];
                                LightmapVisualizationUtility.DrawTextureWithUVOverlay(texture, Selection.activeGameObject, drawableArea, rect6, textureType);
                                texture.filterMode = filterMode;
                            }
                            break;
                        }
                    }
                    if (this.m_PreviousSelection != Selection.activeInstanceID)
                    {
                        this.m_PreviousSelection = Selection.activeInstanceID;
                        this.m_ZoomablePreview.SetShownHRange(0f, 1f);
                        this.m_ZoomablePreview.SetShownVRange(0f, 1f);
                    }
                    Rect rect8 = new Rect(r);
                    rect8.yMin += rect2.height;
                    this.m_ZoomablePreview.rect = rect8;
                    this.m_ZoomablePreview.BeginViewGUI();
                    this.m_ZoomablePreview.EndViewGUI();
                    GUILayoutUtility.GetRect(r.width, r.height);
                }
            }
        }

        public void OnDisable()
        {
            Object.DestroyImmediate(this.m_LightEditor);
            Object.DestroyImmediate(this.m_LightmapParametersEditor);
        }

        public void OnEnable(EditorWindow window)
        {
            this.m_ShowClampedSize.value = false;
            this.m_ShowClampedSize.valueChanged.AddListener(new UnityAction(window.Repaint));
        }

        private Rect ResizeRectToFit(Rect rect, Rect to)
        {
            float a = to.width / rect.width;
            float b = to.height / rect.height;
            float num3 = Mathf.Min(a, b);
            float width = (int) Mathf.Round(rect.width * num3);
            return new Rect(rect.x, rect.y, width, (int) Mathf.Round(rect.height * num3));
        }

        private Rect ScaleRectByZoomableArea(Rect rect, ZoomableArea zoomableArea)
        {
            float num = -(zoomableArea.shownArea.x / zoomableArea.shownArea.width) * rect.width;
            float num2 = ((zoomableArea.shownArea.y - (1f - zoomableArea.shownArea.height)) / zoomableArea.shownArea.height) * rect.height;
            float width = rect.width / zoomableArea.shownArea.width;
            return new Rect(rect.x + num, rect.y + num2, width, rect.height / zoomableArea.shownArea.height);
        }
    }
}

