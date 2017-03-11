namespace UnityEditor
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using UnityEditor.IMGUI.Controls;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    internal class GameObjectTreeViewGUI : TreeViewGUI
    {
        private float m_PrevScollPos;
        private float m_PrevTotalHeight;

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event System.Action mouseAndKeyboardInput;

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event System.Action scrollHeightChanged;

        [field: DebuggerBrowsable(0), CompilerGenerated]
        public event System.Action scrollPositionChanged;

        public GameObjectTreeViewGUI(TreeViewController treeView, bool useHorizontalScroll) : base(treeView, useHorizontalScroll)
        {
            base.k_TopRowMargin = 0f;
        }

        public override bool BeginRename(TreeViewItem item, float delay)
        {
            GameObjectTreeViewItem item2 = item as GameObjectTreeViewItem;
            if (item2 == null)
            {
                return false;
            }
            if (item2.isSceneHeader)
            {
                return false;
            }
            if ((item2.objectPPTR.hideFlags & HideFlags.NotEditable) != HideFlags.None)
            {
                UnityEngine.Debug.LogWarning("Unable to rename a GameObject with HideFlags.NotEditable.");
                return false;
            }
            return base.BeginRename(item, delay);
        }

        public override void BeginRowGUI()
        {
            this.DetectScrollChange();
            this.DetectTotalRectChange();
            this.DetectMouseDownInTreeViewRect();
            base.BeginRowGUI();
            if (this.showingStickyHeaders && (Event.current.type != EventType.Repaint))
            {
                this.DoStickySceneHeaders();
            }
        }

        private void DetectMouseDownInTreeViewRect()
        {
            if (this.mouseAndKeyboardInput != null)
            {
                Event current = Event.current;
                bool flag = (current.type == EventType.MouseDown) || (current.type == EventType.MouseUp);
                bool flag2 = (current.type == EventType.KeyDown) || (current.type == EventType.KeyUp);
                if ((flag && base.m_TreeView.GetTotalRect().Contains(current.mousePosition)) || flag2)
                {
                    this.mouseAndKeyboardInput();
                }
            }
        }

        private void DetectScrollChange()
        {
            float y = base.m_TreeView.state.scrollPos.y;
            if ((this.scrollPositionChanged != null) && !Mathf.Approximately(y, this.m_PrevScollPos))
            {
                this.scrollPositionChanged();
            }
            this.m_PrevScollPos = y;
        }

        private void DetectTotalRectChange()
        {
            float height = base.m_TreeView.GetTotalRect().height;
            if ((this.scrollHeightChanged != null) && !Mathf.Approximately(height, this.m_PrevTotalHeight))
            {
                this.scrollHeightChanged();
            }
            this.m_PrevTotalHeight = height;
        }

        protected void DoAdditionalSceneHeaderGUI(GameObjectTreeViewItem goItem, Rect rect)
        {
            Rect position = new Rect((rect.width - 16f) - 4f, rect.y + ((rect.height - 6f) * 0.5f), 16f, rect.height);
            if (Event.current.type == EventType.Repaint)
            {
                GameObjectStyles.optionsButtonStyle.Draw(position, false, false, false, false);
            }
            position.y = rect.y;
            position.height = rect.height;
            position.width = 24f;
            if (EditorGUI.DropdownButton(position, GUIContent.none, FocusType.Passive, GUIStyle.none))
            {
                base.m_TreeView.SelectionClick(goItem, true);
                base.m_TreeView.contextClickItemCallback(goItem.id);
            }
        }

        protected override void DoItemGUI(Rect rect, int row, TreeViewItem item, bool selected, bool focused, bool useBoldFont)
        {
            GameObjectTreeViewItem goItem = item as GameObjectTreeViewItem;
            if (goItem != null)
            {
                if (goItem.isSceneHeader)
                {
                    Color color = GUI.color;
                    GUI.color *= new Color(1f, 1f, 1f, 0.9f);
                    GUI.Label(rect, GUIContent.none, GameObjectStyles.sceneHeaderBg);
                    GUI.color = color;
                }
                base.DoItemGUI(rect, row, item, selected, focused, useBoldFont);
                if (goItem.isSceneHeader)
                {
                    this.DoAdditionalSceneHeaderGUI(goItem, rect);
                }
                if (SceneHierarchyWindow.s_Debug)
                {
                    GUI.Label(new Rect(rect.xMax - 70f, rect.y, 70f, rect.height), string.Concat(new object[] { "", row, " (", goItem.id, ")" }), EditorStyles.boldLabel);
                }
            }
        }

        private void DoStickySceneHeaders()
        {
            int num;
            int num2;
            this.GetFirstAndLastRowVisible(out num, out num2);
            if ((num >= 0) && (num2 >= 0))
            {
                <DoStickySceneHeaders>c__AnonStorey0 storey = new <DoStickySceneHeaders>c__AnonStorey0();
                float y = base.m_TreeView.state.scrollPos.y;
                if ((num != 0) || (y > this.topRowMargin))
                {
                    storey.firstItem = (GameObjectTreeViewItem) base.m_TreeView.data.GetItem(num);
                    GameObjectTreeViewItem item = (GameObjectTreeViewItem) base.m_TreeView.data.GetItem(num + 1);
                    bool flag = storey.firstItem.scene != item.scene;
                    float width = GUIClip.visibleRect.width;
                    Rect rowRect = this.GetRowRect(num, width);
                    if (!storey.firstItem.isSceneHeader || !Mathf.Approximately(y, rowRect.y))
                    {
                        if (!flag)
                        {
                            rowRect.y = y;
                        }
                        GameObjectTreeViewItem item2 = Enumerable.FirstOrDefault<GameObjectTreeViewItem>(((GameObjectTreeViewDataSource) base.m_TreeView.data).sceneHeaderItems, new Func<GameObjectTreeViewItem, bool>(storey.<>m__0));
                        if (item2 != null)
                        {
                            bool selected = base.m_TreeView.IsItemDragSelectedOrSelected(item2);
                            bool focused = base.m_TreeView.HasFocus();
                            bool useBoldFont = item2.scene == SceneManager.GetActiveScene();
                            this.DoItemGUI(rowRect, num, item2, selected, focused, useBoldFont);
                            if (GUI.Button(new Rect(rowRect.x, rowRect.y, rowRect.height, rowRect.height), GUIContent.none, GUIStyle.none))
                            {
                                base.m_TreeView.Frame(item2.id, true, false);
                            }
                            base.m_TreeView.HandleUnusedMouseEventsForItem(rowRect, item2, num);
                            this.HandleStickyHeaderContextClick(rowRect, item2);
                        }
                    }
                }
            }
        }

        public override void EndRowGUI()
        {
            base.EndRowGUI();
            if (this.showingStickyHeaders && (Event.current.type == EventType.Repaint))
            {
                this.DoStickySceneHeaders();
            }
        }

        public override Rect GetRectForFraming(int row)
        {
            Rect rectForFraming = base.GetRectForFraming(row);
            if (this.showingStickyHeaders && (row < base.m_TreeView.data.rowCount))
            {
                GameObjectTreeViewItem item = base.m_TreeView.data.GetItem(row) as GameObjectTreeViewItem;
                if ((item != null) && !item.isSceneHeader)
                {
                    rectForFraming.y -= base.k_LineHeight;
                    rectForFraming.height = 2f * base.k_LineHeight;
                }
            }
            return rectForFraming;
        }

        private void HandleStickyHeaderContextClick(Rect rect, GameObjectTreeViewItem sceneHeaderItem)
        {
            Event current = Event.current;
            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                if ((((current.type == EventType.MouseDown) && (current.button == 1)) || (current.type == EventType.ContextClick)) && rect.Contains(Event.current.mousePosition))
                {
                    current.Use();
                    base.m_TreeView.contextClickItemCallback(sceneHeaderItem.id);
                }
            }
            else if ((Application.platform == RuntimePlatform.WindowsEditor) && (((current.type == EventType.MouseDown) && (current.button == 1)) && rect.Contains(Event.current.mousePosition)))
            {
                current.Use();
            }
        }

        protected override void OnContentGUI(Rect rect, int row, TreeViewItem item, string label, bool selected, bool focused, bool useBoldFont, bool isPinging)
        {
            if (Event.current.type == EventType.Repaint)
            {
                GameObjectTreeViewItem item2 = item as GameObjectTreeViewItem;
                if (item2 != null)
                {
                    if (!item2.isSceneHeader)
                    {
                        if (!isPinging)
                        {
                            rect.xMin += this.GetContentIndent(item) + base.extraSpaceBeforeIconAndLabel;
                        }
                        int colorCode = item2.colorCode;
                        if (string.IsNullOrEmpty(item.displayName))
                        {
                            if (item2.objectPPTR != null)
                            {
                                item2.displayName = item2.objectPPTR.name;
                            }
                            else
                            {
                                item2.displayName = "deleted gameobject";
                            }
                            label = item2.displayName;
                        }
                        GUIStyle lineStyle = TreeViewGUI.Styles.lineStyle;
                        if (!item2.shouldDisplay)
                        {
                            lineStyle = GameObjectStyles.disabledLabel;
                        }
                        else if ((colorCode & 3) == 0)
                        {
                            lineStyle = (colorCode >= 4) ? GameObjectStyles.disabledLabel : TreeViewGUI.Styles.lineStyle;
                        }
                        else if ((colorCode & 3) == 1)
                        {
                            lineStyle = (colorCode >= 4) ? GameObjectStyles.disabledPrefabLabel : GameObjectStyles.prefabLabel;
                        }
                        else if ((colorCode & 3) == 2)
                        {
                            lineStyle = (colorCode >= 4) ? GameObjectStyles.disabledBrokenPrefabLabel : GameObjectStyles.brokenPrefabLabel;
                        }
                        Texture iconForItem = this.GetIconForItem(item);
                        lineStyle.padding.left = 0;
                        if (iconForItem != null)
                        {
                            Rect position = rect;
                            position.width = base.k_IconWidth;
                            GUI.DrawTexture(position, iconForItem, ScaleMode.ScaleToFit);
                            rect.xMin += (base.iconTotalPadding + base.k_IconWidth) + base.k_SpaceBetweenIconAndText;
                        }
                        lineStyle.Draw(rect, label, false, false, selected, focused);
                    }
                    else
                    {
                        if (item2.scene.isDirty)
                        {
                            label = label + "*";
                        }
                        switch (item2.scene.loadingState)
                        {
                            case Scene.LoadingState.NotLoaded:
                                label = label + " (not loaded)";
                                break;

                            case Scene.LoadingState.Loading:
                                label = label + " (is loading)";
                                break;
                        }
                        bool flag = item2.scene == SceneManager.GetActiveScene();
                        using (new EditorGUI.DisabledScope(!item2.scene.isLoaded))
                        {
                            base.OnContentGUI(rect, row, item, label, selected, focused, flag, isPinging);
                        }
                    }
                }
            }
        }

        public override void OnInitialize()
        {
            this.m_PrevScollPos = base.m_TreeView.state.scrollPos.y;
            this.m_PrevTotalHeight = base.m_TreeView.GetTotalRect().height;
        }

        protected override void RenameEnded()
        {
            string name = !string.IsNullOrEmpty(base.GetRenameOverlay().name) ? base.GetRenameOverlay().name : base.GetRenameOverlay().originalName;
            int userData = base.GetRenameOverlay().userData;
            if (base.GetRenameOverlay().userAcceptedRename)
            {
                ObjectNames.SetNameSmartWithInstanceID(userData, name);
                TreeViewItem item = base.m_TreeView.data.FindItem(userData);
                if (item != null)
                {
                    item.displayName = name;
                }
                EditorApplication.RepaintAnimationWindow();
            }
        }

        private bool showingStickyHeaders =>
            (SceneManager.sceneCount > 1);

        [CompilerGenerated]
        private sealed class <DoStickySceneHeaders>c__AnonStorey0
        {
            internal GameObjectTreeViewItem firstItem;

            internal bool <>m__0(GameObjectTreeViewItem p) => 
                (p.scene == this.firstItem.scene);
        }

        private enum GameObjectColorType
        {
            Normal,
            Prefab,
            BrokenPrefab,
            Count
        }

        internal static class GameObjectStyles
        {
            public static GUIStyle brokenPrefabLabel = new GUIStyle("PR BrokenPrefabLabel");
            public static GUIStyle disabledBrokenPrefabLabel = new GUIStyle("PR DisabledBrokenPrefabLabel");
            public static GUIStyle disabledLabel = new GUIStyle("PR DisabledLabel");
            public static GUIStyle disabledPrefabLabel = new GUIStyle("PR DisabledPrefabLabel");
            public static readonly int kSceneHeaderIconsInterval = 2;
            public static GUIContent loadSceneGUIContent = new GUIContent(EditorGUIUtility.FindTexture("SceneLoadIn"), "Load scene");
            public static GUIStyle optionsButtonStyle = "PaneOptions";
            public static GUIStyle prefabLabel = new GUIStyle("PR PrefabLabel");
            public static GUIContent saveSceneGUIContent = new GUIContent(EditorGUIUtility.FindTexture("SceneSave"), "Save scene");
            public static GUIStyle sceneHeaderBg = "ProjectBrowserTopBarBg";
            public static GUIContent unloadSceneGUIContent = new GUIContent(EditorGUIUtility.FindTexture("SceneLoadOut"), "Unload scene");

            static GameObjectStyles()
            {
                disabledLabel.alignment = TextAnchor.MiddleLeft;
                prefabLabel.alignment = TextAnchor.MiddleLeft;
                disabledPrefabLabel.alignment = TextAnchor.MiddleLeft;
                brokenPrefabLabel.alignment = TextAnchor.MiddleLeft;
                disabledBrokenPrefabLabel.alignment = TextAnchor.MiddleLeft;
                ClearSelectionTexture(disabledLabel);
                ClearSelectionTexture(prefabLabel);
                ClearSelectionTexture(disabledPrefabLabel);
                ClearSelectionTexture(brokenPrefabLabel);
                ClearSelectionTexture(disabledBrokenPrefabLabel);
            }

            private static void ClearSelectionTexture(GUIStyle style)
            {
                Texture2D background = style.hover.background;
                style.onNormal.background = background;
                style.onActive.background = background;
                style.onFocused.background = background;
            }
        }
    }
}

