﻿namespace UnityEditor
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal class GenericPresetLibraryInspector<T> where T: ScriptableObject
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private PresetLibraryEditorState.ItemViewMode <itemViewMode>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private float <lineSpacing>k__BackingField;
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private RectOffset <marginsForGrid>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private RectOffset <marginsForList>k__BackingField;
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private int <maxShowNumPresets>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Vector2 <presetSize>k__BackingField;
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private bool <useOnePixelOverlappedGrid>k__BackingField;
        private readonly Action<string> m_EditButtonClickedCallback;
        private readonly VerticalGrid m_Grid;
        private readonly string m_Header;
        private float m_LastRepaintedWidth;
        private readonly ScriptableObjectSaveLoadHelper<T> m_SaveLoadHelper;
        private readonly Object m_Target;
        private static GUIStyle s_EditButtonStyle;

        public GenericPresetLibraryInspector(Object target, string header, Action<string> editButtonClicked)
        {
            this.m_LastRepaintedWidth = -1f;
            this.m_Target = target;
            this.m_Header = header;
            this.m_EditButtonClickedCallback = editButtonClicked;
            string extension = Path.GetExtension(AssetDatabase.GetAssetPath(this.m_Target.GetInstanceID()));
            if (!string.IsNullOrEmpty(extension))
            {
                char[] trimChars = new char[] { '.' };
                extension = extension.TrimStart(trimChars);
            }
            this.m_SaveLoadHelper = new ScriptableObjectSaveLoadHelper<T>(extension, SaveType.Text);
            this.m_Grid = new VerticalGrid();
            this.maxShowNumPresets = 0x31;
            this.presetSize = new Vector2(14f, 14f);
            this.lineSpacing = 1f;
            this.useOnePixelOverlappedGrid = false;
            this.marginsForList = new RectOffset(10, 10, 5, 5);
            this.marginsForGrid = new RectOffset(10, 10, 5, 5);
            this.itemViewMode = PresetLibraryEditorState.ItemViewMode.List;
        }

        private void DrawPresets(string libraryPath)
        {
            if (GUIClip.visibleRect.width > 0f)
            {
                this.m_LastRepaintedWidth = GUIClip.visibleRect.width;
            }
            if (this.m_LastRepaintedWidth < 0f)
            {
                GUILayoutUtility.GetRect((float) 1f, (float) 1f);
                HandleUtility.Repaint();
            }
            else
            {
                PresetLibrary library = ScriptableSingleton<PresetLibraryManager>.instance.GetLibrary<T>(this.m_SaveLoadHelper, libraryPath) as PresetLibrary;
                if (library == null)
                {
                    Debug.Log("Could not load preset library '" + libraryPath + "'");
                }
                else
                {
                    this.SetupGrid(this.m_LastRepaintedWidth, library.Count(), this.itemViewMode);
                    int num = Mathf.Min(library.Count(), this.maxShowNumPresets);
                    int num2 = library.Count() - num;
                    float height = this.m_Grid.CalcRect(num - 1, 0f).yMax + ((num2 <= 0) ? 0f : 20f);
                    Rect rect = GUILayoutUtility.GetRect(1f, height);
                    float num4 = this.presetSize.x + 6f;
                    for (int i = 0; i < num; i++)
                    {
                        Rect rect5 = this.m_Grid.CalcRect(i, rect.y);
                        Rect rect6 = new Rect(rect5.x, rect5.y, this.presetSize.x, this.presetSize.y);
                        library.Draw(rect6, i);
                        if (this.itemViewMode == PresetLibraryEditorState.ItemViewMode.List)
                        {
                            Rect position = new Rect(rect5.x + num4, rect5.y, rect5.width - num4, rect5.height);
                            string name = library.GetName(i);
                            GUI.Label(position, name);
                        }
                    }
                    if (num2 > 0)
                    {
                        Rect rect8 = new Rect(this.m_Grid.CalcRect(0, 0f).x, (rect.y + height) - 20f, rect.width, 20f);
                        GUI.Label(rect8, string.Format("+ {0} more...", num2));
                    }
                }
            }
        }

        public void OnDestroy()
        {
            ScriptableSingleton<PresetLibraryManager>.instance.UnloadAllLibrariesFor<T>(this.m_SaveLoadHelper);
        }

        public void OnInspectorGUI()
        {
            if (GenericPresetLibraryInspector<T>.s_EditButtonStyle == null)
            {
                GenericPresetLibraryInspector<T>.s_EditButtonStyle = new GUIStyle(EditorStyles.miniButton);
                GenericPresetLibraryInspector<T>.s_EditButtonStyle.margin.top = 7;
            }
            string str2 = Path.ChangeExtension(AssetDatabase.GetAssetPath(this.m_Target.GetInstanceID()), null);
            bool flag = str2.Contains("/Editor/");
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.Label(this.m_Header, EditorStyles.boldLabel, new GUILayoutOption[0]);
            GUILayout.FlexibleSpace();
            if (((flag && (this.m_EditButtonClickedCallback != null)) && GUILayout.Button("Edit...", GenericPresetLibraryInspector<T>.s_EditButtonStyle, new GUILayoutOption[0])) && (this.m_EditButtonClickedCallback != null))
            {
                this.m_EditButtonClickedCallback(str2);
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(6f);
            if (!flag)
            {
                GUIContent content = new GUIContent("Preset libraries should be placed in an 'Editor' folder.", EditorGUIUtility.warningIcon);
                GUILayout.Label(content, EditorStyles.helpBox, new GUILayoutOption[0]);
            }
            this.DrawPresets(str2);
        }

        private void SetupGrid(float availableWidth, int itemCount, PresetLibraryEditorState.ItemViewMode presetsViewMode)
        {
            this.m_Grid.useFixedHorizontalSpacing = this.useOnePixelOverlappedGrid;
            this.m_Grid.fixedHorizontalSpacing = !this.useOnePixelOverlappedGrid ? ((float) 0) : ((float) (-1));
            if (presetsViewMode != PresetLibraryEditorState.ItemViewMode.Grid)
            {
                if (presetsViewMode == PresetLibraryEditorState.ItemViewMode.List)
                {
                    this.m_Grid.fixedWidth = availableWidth;
                    this.m_Grid.topMargin = this.marginsForList.top;
                    this.m_Grid.bottomMargin = this.marginsForList.bottom;
                    this.m_Grid.leftMargin = this.marginsForList.left;
                    this.m_Grid.rightMargin = this.marginsForList.right;
                    this.m_Grid.verticalSpacing = this.lineSpacing;
                    this.m_Grid.minHorizontalSpacing = 0f;
                    this.m_Grid.itemSize = new Vector2(availableWidth - this.m_Grid.leftMargin, this.presetSize.y);
                    this.m_Grid.InitNumRowsAndColumns(itemCount, 0x7fffffff);
                }
            }
            else
            {
                this.m_Grid.fixedWidth = availableWidth;
                this.m_Grid.topMargin = this.marginsForGrid.top;
                this.m_Grid.bottomMargin = this.marginsForGrid.bottom;
                this.m_Grid.leftMargin = this.marginsForGrid.left;
                this.m_Grid.rightMargin = this.marginsForGrid.right;
                this.m_Grid.verticalSpacing = !this.useOnePixelOverlappedGrid ? this.lineSpacing : -1f;
                this.m_Grid.minHorizontalSpacing = 8f;
                this.m_Grid.itemSize = this.presetSize;
                this.m_Grid.InitNumRowsAndColumns(itemCount, 0x7fffffff);
            }
        }

        public string extension
        {
            get
            {
                return this.m_SaveLoadHelper.fileExtensionWithoutDot;
            }
        }

        public PresetLibraryEditorState.ItemViewMode itemViewMode { get; set; }

        public float lineSpacing { get; set; }

        public RectOffset marginsForGrid { get; set; }

        public RectOffset marginsForList { get; set; }

        public int maxShowNumPresets { get; set; }

        public Vector2 presetSize { get; set; }

        public bool useOnePixelOverlappedGrid { get; set; }
    }
}

