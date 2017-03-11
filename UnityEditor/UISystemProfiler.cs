namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using UnityEditor.IMGUI.Controls;
    using UnityEditorInternal;
    using UnityEngine;

    internal class UISystemProfiler
    {
        private Material m_CompositeOverdrawMaterial;
        private UISystemPreviewWindow m_DetachedPreview;
        private MultiColumnHeaderState m_MulticolumnHeaderState;
        private UISystemProfilerRenderService m_RenderService;
        private readonly SplitterState m_TreePreviewHorizontalSplitState;
        private UISystemProfilerTreeView m_TreeViewControl;
        private UISystemProfilerTreeView.State m_UGUIProfilerTreeViewState;
        private ZoomableArea m_ZoomablePreview;

        public UISystemProfiler()
        {
            float[] relativeSizes = new float[] { 70f, 30f };
            int[] minSizes = new int[] { 100, 100 };
            this.m_TreePreviewHorizontalSplitState = new SplitterState(relativeSizes, minSizes, null);
        }

        public void CurrentAreaChanged(ProfilerArea profilerArea)
        {
            if ((profilerArea != ProfilerArea.UI) && (profilerArea != ProfilerArea.UIDetails))
            {
                if (this.m_DetachedPreview != null)
                {
                    this.m_DetachedPreview.Close();
                    this.m_DetachedPreview = null;
                }
                if (this.m_RenderService != null)
                {
                    this.m_RenderService.Dispose();
                    this.m_RenderService = null;
                }
            }
        }

        internal static void DrawPreviewToolbarButtons()
        {
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.MaxWidth(100f) };
            PreviewBackground = (Styles.PreviewBackgroundType) EditorGUILayout.IntPopup(GUIContent.none, (int) PreviewBackground, Styles.backgroundOptions, Styles.backgroundValues, EditorStyles.toolbarDropDown, options);
            GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.MaxWidth(100f) };
            PreviewRenderMode = (Styles.RenderMode) EditorGUILayout.IntPopup(GUIContent.none, (int) PreviewRenderMode, Styles.rendermodeOptions, Styles.rendermodeValues, EditorStyles.toolbarDropDown, optionArray2);
        }

        internal void DrawRenderUI()
        {
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true) };
            Rect controlRect = EditorGUILayout.GetControlRect(options);
            GUI.Box(controlRect, GUIContent.none);
            this.m_ZoomablePreview.BeginViewGUI();
            bool flag = true;
            if ((this.m_UGUIProfilerTreeViewState != null) && (Event.current.type == EventType.Repaint))
            {
                IList<int> selection = this.m_TreeViewControl.GetSelection();
                if (selection.Count > 0)
                {
                    IList<TreeViewItem> rowsFromIDs = this.m_TreeViewControl.GetRowsFromIDs(selection);
                    foreach (TreeViewItem item in rowsFromIDs)
                    {
                        Texture2D texture = null;
                        UISystemProfilerTreeView.BatchTreeViewItem item2 = item as UISystemProfilerTreeView.BatchTreeViewItem;
                        Styles.RenderMode previewRenderMode = PreviewRenderMode;
                        if (this.m_RenderService == null)
                        {
                            this.m_RenderService = new UISystemProfilerRenderService();
                        }
                        if (item2 != null)
                        {
                            texture = this.m_RenderService.GetThumbnail(item2.renderDataIndex, 1, previewRenderMode != Styles.RenderMode.Standard);
                        }
                        UISystemProfilerTreeView.CanvasTreeViewItem item3 = item as UISystemProfilerTreeView.CanvasTreeViewItem;
                        if (item3 != null)
                        {
                            texture = this.m_RenderService.GetThumbnail(item3.info.renderDataIndex, item3.info.renderDataCount, previewRenderMode != Styles.RenderMode.Standard);
                        }
                        if ((previewRenderMode == Styles.RenderMode.CompositeOverdraw) && (this.m_CompositeOverdrawMaterial == null))
                        {
                            Shader shader = Shader.Find("Hidden/UI/CompositeOverdraw");
                            if (shader != null)
                            {
                                this.m_CompositeOverdrawMaterial = new Material(shader);
                            }
                        }
                        if (texture != null)
                        {
                            float width = texture.width;
                            float height = texture.height;
                            float num3 = Math.Min((float) (controlRect.width / width), (float) (controlRect.height / height));
                            width *= num3;
                            height *= num3;
                            Rect rect2 = new Rect(controlRect.x + ((controlRect.width - width) / 2f), controlRect.y + ((controlRect.height - height) / 2f), width, height);
                            if (flag)
                            {
                                flag = false;
                                this.m_ZoomablePreview.rect = rect2;
                                Styles.PreviewBackgroundType previewBackground = PreviewBackground;
                                if (previewBackground == Styles.PreviewBackgroundType.Checkerboard)
                                {
                                    EditorGUI.DrawTransparencyCheckerTexture(this.m_ZoomablePreview.drawRect, ScaleMode.ScaleAndCrop, 0f);
                                }
                                else
                                {
                                    EditorGUI.DrawRect(this.m_ZoomablePreview.drawRect, (previewBackground != Styles.PreviewBackgroundType.Black) ? Color.white : Color.black);
                                }
                            }
                            Graphics.DrawTexture(this.m_ZoomablePreview.drawRect, texture, this.m_ZoomablePreview.shownArea, 0, 0, 0, 0, (previewRenderMode != Styles.RenderMode.CompositeOverdraw) ? EditorGUI.transparentMaterial : this.m_CompositeOverdrawMaterial);
                        }
                        if (previewRenderMode != Styles.RenderMode.Standard)
                        {
                            break;
                        }
                    }
                }
            }
            if (flag && (Event.current.type == EventType.Repaint))
            {
                this.m_ZoomablePreview.rect = controlRect;
            }
            this.m_ZoomablePreview.EndViewGUI();
        }

        internal void DrawUIPane(ProfilerWindow win, ProfilerArea profilerArea, UISystemProfilerChart detailsChart)
        {
            this.InitIfNeeded(win);
            EditorGUILayout.BeginVertical(new GUILayoutOption[0]);
            if ((this.m_DetachedPreview != null) && (this.m_DetachedPreview == null))
            {
                this.m_DetachedPreview = null;
            }
            bool detachedPreview = (bool) this.m_DetachedPreview;
            if (!detachedPreview)
            {
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                SplitterGUILayout.BeginHorizontalSplit(this.m_TreePreviewHorizontalSplitState, new GUILayoutOption[0]);
            }
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true) };
            Rect controlRect = EditorGUILayout.GetControlRect(options);
            controlRect.yMin -= EditorGUIUtility.standardVerticalSpacing;
            this.m_TreeViewControl.property = win.CreateProperty(ProfilerColumn.DontSort);
            if (!this.m_TreeViewControl.property.frameDataReady)
            {
                this.m_TreeViewControl.property.Cleanup();
                this.m_TreeViewControl.property = null;
                GUI.Label(controlRect, Styles.noData);
            }
            else
            {
                int activeVisibleFrameIndex = win.GetActiveVisibleFrameIndex();
                if ((this.m_UGUIProfilerTreeViewState != null) && (this.m_UGUIProfilerTreeViewState.lastFrame != activeVisibleFrameIndex))
                {
                    this.m_TreeViewControl.Reload();
                }
                this.m_TreeViewControl.OnGUI(controlRect);
                this.m_TreeViewControl.property.Cleanup();
            }
            if (!detachedPreview)
            {
                using (new EditorGUILayout.VerticalScope(new GUILayoutOption[0]))
                {
                    GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.ExpandWidth(true) };
                    using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar, optionArray2))
                    {
                        GUILayoutOption[] optionArray3 = new GUILayoutOption[] { GUILayout.Width(75f) };
                        if (GUILayout.Button(Styles.contentDetachRender, EditorStyles.toolbarButton, optionArray3))
                        {
                            this.m_DetachedPreview = EditorWindow.GetWindow<UISystemPreviewWindow>();
                            this.m_DetachedPreview.profiler = this;
                            this.m_DetachedPreview.Show();
                        }
                        DrawPreviewToolbarButtons();
                    }
                    this.DrawRenderUI();
                }
                GUILayout.EndHorizontal();
                SplitterGUILayout.EndHorizontalSplit();
                EditorGUI.DrawRect(new Rect(this.m_TreePreviewHorizontalSplitState.realSizes[0] + controlRect.xMin, controlRect.y, 1f, controlRect.height), Styles.separatorColor);
            }
            EditorGUILayout.EndVertical();
            if (this.m_DetachedPreview != null)
            {
                this.m_DetachedPreview.Repaint();
            }
        }

        private void InitIfNeeded(ProfilerWindow win)
        {
            if (this.m_ZoomablePreview == null)
            {
                ZoomableArea area = new ZoomableArea(true, false) {
                    hRangeMin = 0f,
                    vRangeMin = 0f,
                    hRangeMax = 1f,
                    vRangeMax = 1f
                };
                this.m_ZoomablePreview = area;
                this.m_ZoomablePreview.SetShownHRange(0f, 1f);
                this.m_ZoomablePreview.SetShownVRange(0f, 1f);
                this.m_ZoomablePreview.uniformScale = true;
                this.m_ZoomablePreview.scaleWithWindow = true;
                int num = 100;
                int num2 = 200;
                MultiColumnHeaderState.Column[] columns = new MultiColumnHeaderState.Column[8];
                MultiColumnHeaderState.Column column = new MultiColumnHeaderState.Column {
                    headerContent = EditorGUIUtility.TextContent("Object"),
                    width = 220f,
                    maxWidth = 400f,
                    canSort = true
                };
                columns[0] = column;
                column = new MultiColumnHeaderState.Column {
                    headerContent = EditorGUIUtility.TextContent("Self Batch Count"),
                    width = num,
                    maxWidth = num2
                };
                columns[1] = column;
                column = new MultiColumnHeaderState.Column {
                    headerContent = EditorGUIUtility.TextContent("Cumulative Batch Count"),
                    width = num,
                    maxWidth = num2
                };
                columns[2] = column;
                column = new MultiColumnHeaderState.Column {
                    headerContent = EditorGUIUtility.TextContent("Self Vertex Count"),
                    width = num,
                    maxWidth = num2
                };
                columns[3] = column;
                column = new MultiColumnHeaderState.Column {
                    headerContent = EditorGUIUtility.TextContent("Cumulative Vertex Count"),
                    width = num,
                    maxWidth = num2
                };
                columns[4] = column;
                column = new MultiColumnHeaderState.Column {
                    headerContent = EditorGUIUtility.TextContent("Batch Breaking Reason"),
                    width = 220f,
                    maxWidth = 400f,
                    canSort = false
                };
                columns[5] = column;
                column = new MultiColumnHeaderState.Column {
                    headerContent = EditorGUIUtility.TextContent("GameObject Count"),
                    width = num,
                    maxWidth = 400f
                };
                columns[6] = column;
                column = new MultiColumnHeaderState.Column {
                    headerContent = EditorGUIUtility.TextContent("GameObjects"),
                    width = 150f,
                    maxWidth = 400f,
                    canSort = false
                };
                columns[7] = column;
                this.m_MulticolumnHeaderState = new MultiColumnHeaderState(columns);
                foreach (MultiColumnHeaderState.Column column2 in this.m_MulticolumnHeaderState.columns)
                {
                    column2.sortingArrowAlignment = TextAlignment.Right;
                }
                UISystemProfilerTreeView.State state = new UISystemProfilerTreeView.State {
                    profilerWindow = win
                };
                this.m_UGUIProfilerTreeViewState = state;
                Headers multiColumnHeader = new Headers(this.m_MulticolumnHeaderState) {
                    canSort = true,
                    height = 21f
                };
                multiColumnHeader.sortingChanged += header => this.m_TreeViewControl.Reload();
                this.m_TreeViewControl = new UISystemProfilerTreeView(this.m_UGUIProfilerTreeViewState, multiColumnHeader);
                this.m_TreeViewControl.Reload();
            }
        }

        private static Styles.PreviewBackgroundType PreviewBackground
        {
            get => 
                ((Styles.PreviewBackgroundType) EditorPrefs.GetInt("UGUIProfiler.CheckerBoard", 0));
            set
            {
                EditorPrefs.SetInt("UGUIProfiler.CheckerBoard", (int) value);
            }
        }

        private static Styles.RenderMode PreviewRenderMode
        {
            get => 
                ((Styles.RenderMode) EditorPrefs.GetInt("UGUIProfiler.Overdraw", 0));
            set
            {
                EditorPrefs.SetInt("UGUIProfiler.Overdraw", (int) value);
            }
        }

        internal class Headers : MultiColumnHeader
        {
            public Headers(MultiColumnHeaderState state) : base(state)
            {
            }

            protected override void ColumnHeaderGUI(MultiColumnHeaderState.Column column, Rect headerRect, int columnIndex)
            {
                GUIStyle styleWrapped = this.GetStyleWrapped(column.headerTextAlignment);
                float num = styleWrapped.CalcHeight(column.headerContent, headerRect.width);
                Rect position = headerRect;
                position.yMin += (position.height - num) - 1f;
                GUI.Label(position, column.headerContent, styleWrapped);
                if (base.canSort && column.canSort)
                {
                    base.SortingButton(column, headerRect, columnIndex);
                }
            }

            internal override void DrawDivider(Rect dividerRect, MultiColumnHeaderState.Column column)
            {
            }

            internal override Rect GetArrowRect(MultiColumnHeaderState.Column column, Rect headerRect) => 
                new Rect(headerRect.xMax - MultiColumnHeader.DefaultStyles.arrowStyle.fixedWidth, headerRect.y + 5f, MultiColumnHeader.DefaultStyles.arrowStyle.fixedWidth, headerRect.height - 10f);

            private GUIStyle GetStyleWrapped(TextAlignment alignment)
            {
                switch (alignment)
                {
                    case TextAlignment.Left:
                        return UISystemProfiler.Styles.columnHeader;

                    case TextAlignment.Center:
                        return UISystemProfiler.Styles.columnHeaderCenterAligned;

                    case TextAlignment.Right:
                        return UISystemProfiler.Styles.columnHeaderRightAligned;
                }
                return UISystemProfiler.Styles.columnHeader;
            }
        }

        internal static class Styles
        {
            public static readonly GUIStyle background;
            public static GUIContent[] backgroundOptions;
            public static int[] backgroundValues;
            public static readonly GUIStyle columnHeader = "OL title";
            public static readonly GUIStyle columnHeaderCenterAligned;
            public static readonly GUIStyle columnHeaderRightAligned;
            public static GUIContent contentDetachRender;
            public static readonly GUIStyle entryEven = "OL EntryBackEven";
            public static readonly GUIStyle entryOdd = "OL EntryBackOdd";
            public static readonly GUIStyle header;
            private static readonly Color m_SeparatorColorNonPro;
            private static readonly Color m_SeparatorColorPro;
            public static readonly GUIContent noData;
            internal const string PrefCheckerBoard = "UGUIProfiler.CheckerBoard";
            internal const string PrefOverdraw = "UGUIProfiler.Overdraw";
            public static GUIContent[] rendermodeOptions;
            public static int[] rendermodeValues;
            public static readonly GUIStyle rightHeader = "OL title TextRight";

            static Styles()
            {
                GUIStyle style = new GUIStyle(columnHeader) {
                    alignment = TextAnchor.MiddleCenter
                };
                columnHeaderCenterAligned = style;
                style = new GUIStyle(columnHeader) {
                    alignment = TextAnchor.MiddleRight
                };
                columnHeaderRightAligned = style;
                background = "OL Box";
                header = "OL title";
                header.alignment = TextAnchor.MiddleLeft;
                noData = EditorGUIUtility.TextContent("No frame data available - UI profiling is only available when profiling in the editor");
                contentDetachRender = new GUIContent("Detach");
                backgroundOptions = new GUIContent[] { new GUIContent("Checkerboard"), new GUIContent("Black"), new GUIContent("White") };
                backgroundValues = new int[] { 0, 1, 2 };
                rendermodeOptions = new GUIContent[] { new GUIContent("Standard"), new GUIContent("Overdraw"), new GUIContent("Composite overdraw") };
                rendermodeValues = new int[] { 0, 1, 2 };
                m_SeparatorColorPro = new Color(0.15f, 0.15f, 0.15f);
                m_SeparatorColorNonPro = new Color(0.6f, 0.6f, 0.6f);
            }

            public static Color separatorColor =>
                (!EditorGUIUtility.isProSkin ? m_SeparatorColorNonPro : m_SeparatorColorPro);

            internal enum PreviewBackgroundType
            {
                Checkerboard,
                Black,
                White
            }

            internal enum RenderMode
            {
                Standard,
                Overdraw,
                CompositeOverdraw
            }
        }
    }
}

