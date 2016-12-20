namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Text;
    using UnityEngine;

    internal class LookDevEnvironmentWindow
    {
        private const float kButtonHeight = 16f;
        private const float kButtonWidth = 16f;
        private Rect m_displayRect;
        private bool m_DragBeingPerformed = false;
        private Rect m_GUIRect;
        public const float m_HDRIHeaderHeight = 18f;
        public const float m_HDRIHeight = 146f;
        public const float m_HDRIWidth = 250f;
        private int m_HoveringCubeMapIndex = -1;
        public const float m_latLongHeight = 125f;
        private LookDevView m_LookDevView;
        private Rect m_PositionInLookDev;
        private bool m_RenderOverlayThumbnailOnce = false;
        private Vector2 m_ScrollPosition = new Vector2(0f, 0f);
        private Cubemap m_SelectedCubemap = null;
        private CubemapInfo m_SelectedCubemapInfo = null;
        private int m_SelectedCubeMapOffsetIndex = -1;
        private float m_SelectedCubeMapOffsetValue = 0f;
        private int m_SelectedLightIconIndex = -1;
        private Vector2 m_SelectedPositionOffset = new Vector2(0f, 0f);
        private CubemapInfo m_SelectedShadowCubemapOwnerInfo = null;
        private ShadowInfo m_SelectedShadowInfo = null;
        private static Styles s_Styles = new Styles();

        public LookDevEnvironmentWindow(LookDevView lookDevView)
        {
            this.m_LookDevView = lookDevView;
        }

        public void CancelSelection()
        {
            this.m_SelectedCubemap = null;
            this.m_SelectedCubemapInfo = null;
            this.m_SelectedShadowCubemapOwnerInfo = null;
            this.m_SelectedLightIconIndex = -1;
            this.m_SelectedShadowInfo = null;
            this.m_HoveringCubeMapIndex = -1;
            this.m_DragBeingPerformed = false;
        }

        private float ComputeAngleOffsetFromMouseCoord(Vector2 mousePosition)
        {
            return ((mousePosition.x / 250f) * 360f);
        }

        private void DrawLatLongThumbnail(CubemapInfo infos, float angleOffset, float intensity, float alpha, Rect textureRect)
        {
            GUIStyle style = "dockarea";
            LookDevResources.m_LookDevCubeToLatlong.SetVector("_WindowParams", new Vector4(this.m_displayRect.height, this.m_PositionInLookDev.y + 17f, (float) style.margin.top, EditorGUIUtility.pixelsPerPoint));
            LookDevResources.m_LookDevCubeToLatlong.SetVector("_CubeToLatLongParams", new Vector4(0.01745329f * angleOffset, alpha, intensity, 0f));
            GL.sRGBWrite = QualitySettings.activeColorSpace == ColorSpace.Linear;
            Graphics.DrawTexture(textureRect, infos.cubemap, LookDevResources.m_LookDevCubeToLatlong, 1);
            GL.sRGBWrite = false;
        }

        private void DrawSelectionFeedback(Rect textureRect, Color selectionColor1, Color selectionColor2)
        {
            float x = 0.5f;
            float num2 = textureRect.width - 0.5f;
            float y = textureRect.y + 0.5f;
            float num4 = (textureRect.y + textureRect.height) - 1f;
            float num5 = textureRect.width * 0.5f;
            Vector3[] lineSegments = new Vector3[] { new Vector3(num5, y, 0f), new Vector3(x, y, 0f), new Vector3(x, y, 0f), new Vector3(x, num4, 0f), new Vector3(x, num4, 0f), new Vector3(num5, num4, 0f) };
            Vector3[] vectorArray2 = new Vector3[] { new Vector3(num5, y, 0f), new Vector3(num2, y, 0f), new Vector3(num2, y, 0f), new Vector3(num2, num4, 0f), new Vector3(num2, num4, 0f), new Vector3(num5, num4, 0f) };
            Handles.color = selectionColor1;
            Handles.DrawLines(lineSegments);
            Handles.color = selectionColor2;
            Handles.DrawLines(vectorArray2);
        }

        public Cubemap GetCurrentSelection()
        {
            return this.m_SelectedCubemap;
        }

        private void GetFrameAndShadowTextureRect(Rect textureRect, out Rect frameTextureRect, out Rect shadowTextureRect)
        {
            frameTextureRect = textureRect;
            frameTextureRect.x += textureRect.width - styles.sLatlongFrameTexture.width;
            frameTextureRect.y += textureRect.height - (styles.sLatlongFrameTexture.height * 1.05f);
            frameTextureRect.width = styles.sLatlongFrameTexture.width;
            frameTextureRect.height = styles.sLatlongFrameTexture.height;
            shadowTextureRect = frameTextureRect;
            shadowTextureRect.x += 6f;
            shadowTextureRect.y += 4f;
            shadowTextureRect.width = 105f;
            shadowTextureRect.height = 52f;
        }

        private Rect GetInsertionRect(int envIndex)
        {
            Rect gUIRect = this.m_GUIRect;
            gUIRect.height = 21f;
            gUIRect.y = 146f * envIndex;
            return gUIRect;
        }

        public Vector2 GetSelectedPositionOffset()
        {
            return this.m_SelectedPositionOffset;
        }

        private Rect GetThumbnailRect(int envIndex)
        {
            Rect gUIRect = this.m_GUIRect;
            gUIRect.height = 125f;
            gUIRect.y = (envIndex * 146f) + 18f;
            return gUIRect;
        }

        private void HandleMouseInput()
        {
            List<CubemapInfo> hdriList = this.m_LookDevView.envLibrary.hdriList;
            Vector2 pos = new Vector2(Event.current.mousePosition.x, Event.current.mousePosition.y + this.m_ScrollPosition.y);
            Event current = Event.current;
            EventType typeForControl = current.GetTypeForControl(this.m_LookDevView.hotControl);
            switch (typeForControl)
            {
                case EventType.MouseUp:
                    if (this.m_SelectedCubemap != null)
                    {
                        Rect gUIRect = this.m_GUIRect;
                        gUIRect.yMax += 16f;
                        if (!gUIRect.Contains(Event.current.mousePosition))
                        {
                            break;
                        }
                        int insertionIndex = this.IsPositionInInsertionArea(pos);
                        if (insertionIndex == -1)
                        {
                            int num2 = this.IsPositionInThumbnailArea(pos);
                            if ((num2 != -1) && this.m_LookDevView.config.enableShadowCubemap)
                            {
                                Undo.RecordObject(this.m_LookDevView.envLibrary, "Update shadow cubemap");
                                CubemapInfo info2 = this.m_LookDevView.envLibrary.hdriList[num2];
                                if (info2 != this.m_SelectedCubemapInfo)
                                {
                                    info2.SetCubemapShadowInfo(this.m_SelectedCubemapInfo);
                                }
                                this.m_LookDevView.envLibrary.dirty = true;
                            }
                        }
                        else
                        {
                            this.ResetShadowCubemap();
                            this.m_LookDevView.envLibrary.InsertHDRI(this.m_SelectedCubemap, insertionIndex);
                        }
                        this.CancelSelection();
                    }
                    break;

                case EventType.MouseDrag:
                    if (this.m_SelectedCubeMapOffsetIndex != -1)
                    {
                        Undo.RecordObject(this.m_LookDevView.envLibrary, "");
                        CubemapInfo info = hdriList[this.m_SelectedCubeMapOffsetIndex];
                        info.angleOffset = this.ComputeAngleOffsetFromMouseCoord(pos) + this.m_SelectedCubeMapOffsetValue;
                        this.m_LookDevView.envLibrary.dirty = true;
                        Event.current.Use();
                    }
                    if (this.m_SelectedCubemapInfo != null)
                    {
                        if (this.IsPositionInInsertionArea(pos) == -1)
                        {
                            this.m_HoveringCubeMapIndex = this.IsPositionInThumbnailArea(pos);
                        }
                        else
                        {
                            this.m_HoveringCubeMapIndex = -1;
                        }
                    }
                    this.m_LookDevView.Repaint();
                    return;

                case EventType.KeyDown:
                    if (Event.current.keyCode == KeyCode.Escape)
                    {
                        this.CancelSelection();
                        this.m_LookDevView.Repaint();
                    }
                    return;

                case EventType.Repaint:
                    if (this.m_SelectedCubeMapOffsetIndex != -1)
                    {
                        EditorGUIUtility.AddCursorRect(this.m_displayRect, MouseCursor.SlideArrow);
                    }
                    return;

                case EventType.DragUpdated:
                {
                    bool flag = false;
                    foreach (Object obj3 in DragAndDrop.objectReferences)
                    {
                        Cubemap cubemap2 = obj3 as Cubemap;
                        if (cubemap2 != null)
                        {
                            flag = true;
                        }
                    }
                    DragAndDrop.visualMode = !flag ? DragAndDropVisualMode.Rejected : DragAndDropVisualMode.Link;
                    if (flag)
                    {
                        this.m_DragBeingPerformed = true;
                    }
                    current.Use();
                    return;
                }
                case EventType.DragPerform:
                {
                    int num3 = this.IsPositionInInsertionArea(pos);
                    foreach (Object obj2 in DragAndDrop.objectReferences)
                    {
                        Cubemap cubemap = obj2 as Cubemap;
                        if (cubemap != null)
                        {
                            this.m_LookDevView.envLibrary.InsertHDRI(cubemap, num3);
                        }
                    }
                    DragAndDrop.AcceptDrag();
                    this.m_DragBeingPerformed = false;
                    current.Use();
                    return;
                }
                default:
                    if (typeForControl == EventType.DragExited)
                    {
                    }
                    return;
            }
            this.m_LookDevView.Repaint();
            if ((this.m_SelectedCubeMapOffsetIndex != -1) && (Mathf.Abs(hdriList[this.m_SelectedCubeMapOffsetIndex].angleOffset) <= 10f))
            {
                Undo.RecordObject(this.m_LookDevView.envLibrary, "");
                hdriList[this.m_SelectedCubeMapOffsetIndex].angleOffset = 0f;
                this.m_LookDevView.envLibrary.dirty = true;
            }
            this.m_SelectedCubemapInfo = null;
            this.m_SelectedShadowCubemapOwnerInfo = null;
            this.m_SelectedLightIconIndex = -1;
            this.m_SelectedShadowInfo = null;
            this.m_SelectedCubeMapOffsetIndex = -1;
            this.m_HoveringCubeMapIndex = -1;
            this.m_SelectedCubeMapOffsetValue = 0f;
            GUIUtility.hotControl = 0;
        }

        private int IsPositionInInsertionArea(Vector2 pos)
        {
            for (int i = 0; i <= this.m_LookDevView.envLibrary.hdriCount; i++)
            {
                if (this.GetInsertionRect(i).Contains(pos))
                {
                    return i;
                }
            }
            return -1;
        }

        private int IsPositionInThumbnailArea(Vector2 pos)
        {
            for (int i = 0; i < this.m_LookDevView.envLibrary.hdriCount; i++)
            {
                if (this.GetThumbnailRect(i).Contains(pos))
                {
                    return i;
                }
            }
            return -1;
        }

        private Vector2 LatLongToPosition(float latitude, float longitude)
        {
            latitude = ((latitude + 90f) % 180f) - 90f;
            if (latitude < -90f)
            {
                latitude = -90f;
            }
            if (latitude > 89f)
            {
                latitude = 89f;
            }
            longitude = longitude % 360f;
            if (longitude < 0.0)
            {
                longitude = 360f + longitude;
            }
            return new Vector2((((longitude * 0.01745329f) / 6.283185f) * 2f) - 1f, (latitude * 0.01745329f) / 1.570796f);
        }

        public void OnGUI(int windowID)
        {
            if (this.m_LookDevView != null)
            {
                List<CubemapInfo> hdriList = this.m_LookDevView.envLibrary.hdriList;
                bool flag = (146f * hdriList.Count) > this.m_PositionInLookDev.height;
                if (flag)
                {
                    this.m_ScrollPosition = EditorGUILayout.BeginScrollView(this.m_ScrollPosition, new GUILayoutOption[0]);
                }
                else
                {
                    this.m_ScrollPosition = new Vector2(0f, 0f);
                }
                if (hdriList.Count == 1)
                {
                    Color color = GUI.color;
                    GUI.color = Color.gray;
                    Vector2 vector = GUI.skin.label.CalcSize(styles.sDragAndDropHDRIText);
                    Rect position = new Rect((this.m_PositionInLookDev.width * 0.5f) - (vector.x * 0.5f), (this.m_PositionInLookDev.height * 0.5f) - (vector.y * 0.5f), vector.x, vector.y);
                    GUI.Label(position, styles.sDragAndDropHDRIText);
                    GUI.color = color;
                }
                for (int i = 0; i < hdriList.Count; i++)
                {
                    Rect rect5;
                    Rect rect6;
                    CubemapInfo infos = hdriList[i];
                    ShadowInfo shadowInfo = infos.shadowInfo;
                    int intProperty = this.m_LookDevView.config.GetIntProperty(LookDevProperty.HDRI, LookDevEditionContext.Left);
                    int num3 = this.m_LookDevView.config.GetIntProperty(LookDevProperty.HDRI, LookDevEditionContext.Right);
                    if ((this.m_LookDevView.config.lookDevMode == LookDevMode.Single1) || (this.m_LookDevView.config.lookDevMode == LookDevMode.Single2))
                    {
                        num3 = -1;
                    }
                    bool flag2 = (i == intProperty) || (i == num3);
                    Color black = Color.black;
                    Color firstViewGizmoColor = Color.black;
                    GUIStyle miniLabel = EditorStyles.miniLabel;
                    if (flag2)
                    {
                        if (i == intProperty)
                        {
                            black = (Color) LookDevView.m_FirstViewGizmoColor;
                            firstViewGizmoColor = (Color) LookDevView.m_FirstViewGizmoColor;
                            miniLabel = styles.sLabelStyleFirstContext;
                        }
                        else if (i == num3)
                        {
                            black = (Color) LookDevView.m_SecondViewGizmoColor;
                            firstViewGizmoColor = (Color) LookDevView.m_SecondViewGizmoColor;
                            miniLabel = styles.sLabelStyleSecondContext;
                        }
                        if (intProperty == num3)
                        {
                            black = (Color) LookDevView.m_FirstViewGizmoColor;
                            firstViewGizmoColor = (Color) LookDevView.m_SecondViewGizmoColor;
                            miniLabel = styles.sLabelStyleBothContext;
                        }
                    }
                    GUILayoutOption[] optionArray1 = new GUILayoutOption[] { GUILayout.Width(250f) };
                    GUILayout.BeginVertical(optionArray1);
                    int num4 = hdriList.FindIndex(x => x == this.m_SelectedCubemapInfo);
                    if ((((this.m_SelectedCubemap != null) || this.m_DragBeingPerformed) && this.GetInsertionRect(i).Contains(Event.current.mousePosition)) && ((((num4 - i) != 0) && ((num4 - i) != -1)) || (num4 == -1)))
                    {
                        GUILayout.Label(GUIContent.none, styles.sSeparatorStyle, new GUILayoutOption[0]);
                        GUILayoutUtility.GetRect((float) 250f, (float) 16f);
                    }
                    GUILayout.Label(GUIContent.none, styles.sSeparatorStyle, new GUILayoutOption[0]);
                    GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.Width(250f), GUILayout.Height(18f) };
                    GUILayout.BeginHorizontal(optionArray2);
                    StringBuilder builder = new StringBuilder();
                    builder.Append(i.ToString());
                    builder.Append(" - ");
                    builder.Append(infos.cubemap.name);
                    GUILayoutOption[] optionArray3 = new GUILayoutOption[] { GUILayout.Height(18f), GUILayout.MaxWidth(175f) };
                    GUILayout.Label(builder.ToString(), miniLabel, optionArray3);
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button(styles.sEnvControlIcon, LookDevView.styles.sToolBarButton, new GUILayoutOption[0]))
                    {
                        PopupWindow.Show(GUILayoutUtility.topLevel.GetLast(), new EnvSettingsWindow(this.m_LookDevView, infos));
                        GUIUtility.ExitGUI();
                    }
                    using (new EditorGUI.DisabledScope(infos.cubemap == LookDevResources.m_DefaultHDRI))
                    {
                        if (GUILayout.Button(styles.sCloseIcon, LookDevView.styles.sToolBarButton, new GUILayoutOption[0]))
                        {
                            this.m_LookDevView.envLibrary.RemoveHDRI(infos.cubemap);
                        }
                    }
                    GUILayout.EndHorizontal();
                    Rect lastRect = GUILayoutUtility.GetLastRect();
                    if ((Event.current.type == EventType.MouseDown) && lastRect.Contains(Event.current.mousePosition))
                    {
                        Event.current.Use();
                    }
                    Rect rect = GUILayoutUtility.GetRect((float) 250f, (float) 125f);
                    rect.width = 253f;
                    float num5 = 24f;
                    float num6 = num5 * 0.5f;
                    float latitude = shadowInfo.latitude;
                    float longitude = shadowInfo.longitude;
                    Vector2 vector2 = ((Vector2) (this.LatLongToPosition(latitude, longitude + infos.angleOffset) * 0.5f)) + new Vector2(0.5f, 0.5f);
                    Rect rect3 = rect;
                    rect3.x = (rect3.x + (vector2.x * rect.width)) - num6;
                    rect3.y = (rect3.y + ((1f - vector2.y) * rect.height)) - num6;
                    rect3.width = num5;
                    rect3.height = num5;
                    Rect rect4 = rect;
                    rect4.x = (rect4.x + (vector2.x * rect.width)) - (num6 * 0.5f);
                    rect4.y = (rect4.y + ((1f - vector2.y) * rect.height)) - (num6 * 0.5f);
                    rect4.width = num5 * 0.5f;
                    rect4.height = num5 * 0.5f;
                    this.GetFrameAndShadowTextureRect(rect, out rect5, out rect6);
                    if (this.m_LookDevView.config.enableShadowCubemap)
                    {
                        EditorGUIUtility.AddCursorRect(rect4, MouseCursor.Pan);
                    }
                    if ((Event.current.type == EventType.MouseDown) && rect.Contains(Event.current.mousePosition))
                    {
                        if ((!Event.current.control && (Event.current.button == 0)) && (this.m_SelectedCubeMapOffsetIndex == -1))
                        {
                            if (this.m_LookDevView.config.enableShadowCubemap && rect4.Contains(Event.current.mousePosition))
                            {
                                this.m_SelectedLightIconIndex = i;
                                this.m_SelectedShadowInfo = shadowInfo;
                                Undo.RecordObject(this.m_LookDevView.envLibrary, "Light Icon selection");
                                this.m_SelectedShadowInfo.latitude += 0.0001f;
                                this.m_SelectedShadowInfo.longitude += 0.0001f;
                            }
                            if (this.m_SelectedShadowInfo == null)
                            {
                                Rect rect10 = rect5;
                                rect10.x += 100f;
                                rect10.y += 4f;
                                rect10.width = 11f;
                                rect10.height = 11f;
                                if (this.m_LookDevView.config.enableShadowCubemap && rect10.Contains(Event.current.mousePosition))
                                {
                                    Undo.RecordObject(this.m_LookDevView.envLibrary, "Update shadow cubemap");
                                    hdriList[i].SetCubemapShadowInfo(hdriList[i]);
                                    this.m_LookDevView.envLibrary.dirty = true;
                                }
                                else
                                {
                                    if (this.m_LookDevView.config.enableShadowCubemap && rect6.Contains(Event.current.mousePosition))
                                    {
                                        this.m_SelectedShadowCubemapOwnerInfo = hdriList[i];
                                        this.m_SelectedCubemapInfo = this.m_SelectedShadowCubemapOwnerInfo.cubemapShadowInfo;
                                    }
                                    else
                                    {
                                        this.m_SelectedCubemapInfo = hdriList[i];
                                    }
                                    this.m_SelectedPositionOffset = Event.current.mousePosition - new Vector2(rect.x, rect.y);
                                    this.m_RenderOverlayThumbnailOnce = true;
                                }
                            }
                        }
                        else if ((Event.current.control && (Event.current.button == 0)) && ((this.m_SelectedCubemapInfo == null) && (this.m_SelectedShadowInfo == null)))
                        {
                            this.m_SelectedCubeMapOffsetIndex = i;
                            this.m_SelectedCubeMapOffsetValue = infos.angleOffset - this.ComputeAngleOffsetFromMouseCoord(Event.current.mousePosition);
                        }
                        GUIUtility.hotControl = this.m_LookDevView.hotControl;
                        Event.current.Use();
                    }
                    if ((Event.current.GetTypeForControl(this.m_LookDevView.hotControl) == EventType.MouseDrag) && ((this.m_SelectedShadowInfo == shadowInfo) && (this.m_SelectedLightIconIndex == i)))
                    {
                        Vector2 mousePosition = Event.current.mousePosition;
                        mousePosition.x = (((mousePosition.x - rect.x) / rect.width) * 2f) - 1f;
                        mousePosition.y = ((1f - ((mousePosition.y - rect.y) / rect.height)) * 2f) - 1f;
                        Vector2 vector4 = PositionToLatLong(mousePosition);
                        this.m_SelectedShadowInfo.latitude = vector4.x;
                        this.m_SelectedShadowInfo.longitude = vector4.y - infos.angleOffset;
                        this.m_LookDevView.envLibrary.dirty = true;
                    }
                    if (Event.current.type == EventType.Repaint)
                    {
                        this.DrawLatLongThumbnail(infos, infos.angleOffset, 1f, 1f, rect);
                        if (this.m_LookDevView.config.enableShadowCubemap)
                        {
                            if ((infos.cubemapShadowInfo != infos) || ((this.m_HoveringCubeMapIndex == i) && (this.m_SelectedCubemapInfo != infos)))
                            {
                                CubemapInfo cubemapShadowInfo = infos.cubemapShadowInfo;
                                if ((this.m_HoveringCubeMapIndex == i) && (this.m_SelectedCubemapInfo != infos))
                                {
                                    cubemapShadowInfo = this.m_SelectedCubemapInfo;
                                }
                                float alpha = 1f;
                                if (this.m_SelectedShadowInfo == shadowInfo)
                                {
                                    alpha = 0.1f;
                                }
                                else if (((this.m_HoveringCubeMapIndex == i) && (this.m_SelectedCubemapInfo != infos)) && (infos.cubemapShadowInfo != this.m_SelectedCubemapInfo))
                                {
                                    alpha = 0.5f;
                                }
                                this.DrawLatLongThumbnail(cubemapShadowInfo, infos.angleOffset, 0.3f, alpha, rect6);
                                GL.sRGBWrite = QualitySettings.activeColorSpace == ColorSpace.Linear;
                                GUI.DrawTexture(rect5, styles.sLatlongFrameTexture);
                                GL.sRGBWrite = false;
                            }
                            GL.sRGBWrite = QualitySettings.activeColorSpace == ColorSpace.Linear;
                            GUI.DrawTexture(rect3, styles.sLightTexture);
                            GL.sRGBWrite = false;
                        }
                        if (flag2)
                        {
                            this.DrawSelectionFeedback(rect, black, firstViewGizmoColor);
                        }
                    }
                    GUILayout.EndVertical();
                }
                GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(250f) };
                GUILayout.BeginVertical(options);
                if (((this.m_SelectedCubemap != null) || this.m_DragBeingPerformed) && this.GetInsertionRect(hdriList.Count).Contains(Event.current.mousePosition))
                {
                    GUILayout.Label(GUIContent.none, styles.sSeparatorStyle, new GUILayoutOption[0]);
                    GUILayoutUtility.GetRect((float) 250f, (float) 16f);
                    GUILayout.Label(GUIContent.none, styles.sSeparatorStyle, new GUILayoutOption[0]);
                }
                GUILayout.EndVertical();
                if (flag)
                {
                    EditorGUILayout.EndScrollView();
                }
                this.HandleMouseInput();
                this.RenderOverlayThumbnailIfNeeded();
                if ((Event.current.type == EventType.Repaint) && (this.m_SelectedCubemap != null))
                {
                    this.m_LookDevView.Repaint();
                }
            }
        }

        public static Vector2 PositionToLatLong(Vector2 position)
        {
            Vector2 vector = new Vector2 {
                x = ((position.y * 3.141593f) * 0.5f) * 57.29578f,
                y = ((((position.x * 0.5f) + 0.5f) * 2f) * 3.141593f) * 57.29578f
            };
            if (vector.x < -90f)
            {
                vector.x = -90f;
            }
            if (vector.x > 89f)
            {
                vector.x = 89f;
            }
            return vector;
        }

        private void RenderOverlayThumbnailIfNeeded()
        {
            if ((this.m_RenderOverlayThumbnailOnce && (Event.current.type == EventType.Repaint)) && (this.m_SelectedCubemapInfo != null))
            {
                this.m_SelectedCubemap = this.m_SelectedCubemapInfo.cubemap;
                RenderTexture active = RenderTexture.active;
                RenderTexture.active = LookDevResources.m_SelectionTexture;
                LookDevResources.m_LookDevCubeToLatlong.SetTexture("_MainTex", this.m_SelectedCubemap);
                LookDevResources.m_LookDevCubeToLatlong.SetVector("_WindowParams", new Vector4(this.m_displayRect.height, -1000f, 2f, 1f));
                LookDevResources.m_LookDevCubeToLatlong.SetVector("_CubeToLatLongParams", new Vector4(0.01745329f * this.m_SelectedCubemapInfo.angleOffset, 0.5f, 1f, 0f));
                LookDevResources.m_LookDevCubeToLatlong.SetPass(0);
                GL.sRGBWrite = QualitySettings.activeColorSpace == ColorSpace.Linear;
                LookDevView.DrawFullScreenQuad(new Rect(0f, 0f, 250f, 125f));
                GL.sRGBWrite = false;
                RenderTexture.active = active;
                this.m_RenderOverlayThumbnailOnce = false;
            }
        }

        public void ResetShadowCubemap()
        {
            if (this.m_SelectedShadowCubemapOwnerInfo != null)
            {
                this.m_SelectedShadowCubemapOwnerInfo.SetCubemapShadowInfo(this.m_SelectedShadowCubemapOwnerInfo);
            }
        }

        public void SetRects(Rect positionInLookDev, Rect GUIRect, Rect displayRect)
        {
            this.m_PositionInLookDev = positionInLookDev;
            this.m_GUIRect = GUIRect;
            this.m_displayRect = displayRect;
        }

        public static Styles styles
        {
            get
            {
                if (s_Styles == null)
                {
                    s_Styles = new Styles();
                }
                return s_Styles;
            }
        }

        internal class EnvSettingsWindow : PopupWindowContent
        {
            private float kShadowSettingHeight = 157f;
            private float kShadowSettingWidth = 200f;
            private CubemapInfo m_CubemapInfo;
            private LookDevView m_LookDevView;
            private static Styles s_Styles = null;

            public EnvSettingsWindow(LookDevView lookDevView, CubemapInfo infos)
            {
                this.m_LookDevView = lookDevView;
                this.m_CubemapInfo = infos;
            }

            private void DrawSeparator()
            {
                GUILayout.Space(3f);
                GUILayout.Label(GUIContent.none, styles.sSeparator, new GUILayoutOption[0]);
            }

            public override Vector2 GetWindowSize()
            {
                return new Vector2(this.kShadowSettingWidth, this.kShadowSettingHeight);
            }

            public override void OnGUI(Rect rect)
            {
                GUILayout.BeginVertical(new GUILayoutOption[0]);
                GUILayout.Label(styles.sEnvironment, EditorStyles.miniLabel, new GUILayoutOption[0]);
                EditorGUI.BeginChangeCheck();
                float num = EditorGUILayout.Slider(styles.sAngleOffset, this.m_CubemapInfo.angleOffset % 360f, -360f, 360f, new GUILayoutOption[0]);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(this.m_LookDevView.envLibrary, "Changed environment settings");
                    this.m_CubemapInfo.angleOffset = num;
                    this.m_LookDevView.envLibrary.dirty = true;
                    this.m_LookDevView.Repaint();
                }
                if (GUILayout.Button(styles.sResetEnv, EditorStyles.toolbarButton, new GUILayoutOption[0]))
                {
                    Undo.RecordObject(this.m_LookDevView.envLibrary, "Changed environment settings");
                    this.m_CubemapInfo.ResetEnvInfos();
                    this.m_LookDevView.envLibrary.dirty = true;
                    this.m_LookDevView.Repaint();
                }
                using (new EditorGUI.DisabledScope(!this.m_LookDevView.config.enableShadowCubemap))
                {
                    this.DrawSeparator();
                    GUILayout.Label(styles.sShadows, EditorStyles.miniLabel, new GUILayoutOption[0]);
                    EditorGUI.BeginChangeCheck();
                    float num2 = EditorGUILayout.Slider(styles.sShadowIntensity, this.m_CubemapInfo.shadowInfo.shadowIntensity, 0f, 5f, new GUILayoutOption[0]);
                    Color color = EditorGUILayout.ColorField(styles.sShadowColor, this.m_CubemapInfo.shadowInfo.shadowColor, new GUILayoutOption[0]);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(this.m_LookDevView.envLibrary, "Changed shadow settings");
                        this.m_CubemapInfo.shadowInfo.shadowIntensity = num2;
                        this.m_CubemapInfo.shadowInfo.shadowColor = color;
                        this.m_LookDevView.envLibrary.dirty = true;
                        this.m_LookDevView.Repaint();
                    }
                    if (GUILayout.Button(styles.sBrightest, EditorStyles.toolbarButton, new GUILayoutOption[0]))
                    {
                        Undo.RecordObject(this.m_LookDevView.envLibrary, "Changed shadow settings");
                        LookDevResources.UpdateShadowInfoWithBrightestSpot(this.m_CubemapInfo);
                        this.m_LookDevView.envLibrary.dirty = true;
                        this.m_LookDevView.Repaint();
                    }
                    if (GUILayout.Button(styles.sResetShadow, EditorStyles.toolbarButton, new GUILayoutOption[0]))
                    {
                        Undo.RecordObject(this.m_LookDevView.envLibrary, "Changed shadow settings");
                        this.m_CubemapInfo.SetCubemapShadowInfo(this.m_CubemapInfo);
                        this.m_LookDevView.envLibrary.dirty = true;
                        this.m_LookDevView.Repaint();
                    }
                }
                GUILayout.EndVertical();
            }

            public static Styles styles
            {
                get
                {
                    if (s_Styles == null)
                    {
                        s_Styles = new Styles();
                    }
                    return s_Styles;
                }
            }

            public class Styles
            {
                public readonly GUIContent sAngleOffset = EditorGUIUtility.TextContent("Angle offset|Rotate the environment");
                public readonly GUIContent sBrightest = EditorGUIUtility.TextContent("Set position to brightest point|Set the shadow direction to the brightest (higher value) point of the latLong map");
                public readonly GUIContent sEnvironment = EditorGUIUtility.TextContent("Environment");
                public readonly GUIStyle sMenuItem = "MenuItem";
                public readonly GUIContent sResetEnv = EditorGUIUtility.TextContent("Reset Environment|Reset environment settings");
                public readonly GUIContent sResetShadow = EditorGUIUtility.TextContent("Reset Shadows|Reset shadow properties");
                public readonly GUIStyle sSeparator = "sv_iconselector_sep";
                public readonly GUIContent sShadowColor = EditorGUIUtility.TextContent("Color|Shadow color");
                public readonly GUIContent sShadowIntensity = EditorGUIUtility.TextContent("Shadow brightness|Shadow brightness");
                public readonly GUIContent sShadows = EditorGUIUtility.TextContent("Shadows");
            }
        }

        public class Styles
        {
            public readonly GUIContent sCloseIcon = new GUIContent(EditorGUIUtility.IconContent("LookDevClose"));
            public readonly GUIContent sDragAndDropHDRIText = EditorGUIUtility.TextContent("Drag and drop HDR panorama here.");
            public readonly GUIContent sEnvControlIcon = new GUIContent(EditorGUIUtility.IconContent("LookDevPaneOption"));
            public readonly GUIStyle sLabelStyleBothContext = new GUIStyle(EditorStyles.miniLabel);
            public readonly GUIStyle sLabelStyleFirstContext = new GUIStyle(EditorStyles.miniLabel);
            public readonly GUIStyle sLabelStyleSecondContext = new GUIStyle(EditorStyles.miniLabel);
            public readonly Texture sLatlongFrameTexture = EditorGUIUtility.FindTexture("LookDevShadowFrame");
            public readonly Texture sLightTexture = EditorGUIUtility.FindTexture("LookDevLight");
            public readonly GUIStyle sSeparatorStyle = "sv_iconselector_sep";
            public readonly GUIContent sTitle = EditorGUIUtility.TextContent("HDRI View|Manage your list of HDRI environments.");

            public Styles()
            {
                this.sLabelStyleFirstContext.normal.textColor = (Color) LookDevView.m_FirstViewGizmoColor;
                this.sLabelStyleSecondContext.normal.textColor = (Color) LookDevView.m_SecondViewGizmoColor;
                this.sLabelStyleBothContext.normal.textColor = !EditorGUIUtility.isProSkin ? Color.black : Color.white;
                this.sEnvControlIcon.tooltip = "Environment parameters";
            }
        }
    }
}

