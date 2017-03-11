namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using UnityEngine;

    public class SearchableEditorWindow : EditorWindow
    {
        private bool m_FocusSearchField = false;
        private bool m_HasSearchFilterFocus = false;
        internal HierarchyType m_HierarchyType = HierarchyType.Assets;
        private int m_SearchFieldControlId;
        internal string m_SearchFilter = "";
        internal SearchMode m_SearchMode = SearchMode.All;
        private static int s_SearchableEditorWindowSearchField = "SearchableEditorWindowSearchField".GetHashCode();
        private static List<SearchableEditorWindow> searchableWindows = new List<SearchableEditorWindow>();

        internal void ClearSearchFilter()
        {
            this.SetSearchFilter("", this.m_SearchMode, true);
            if (EditorGUI.s_RecycledEditor != null)
            {
                EditorGUI.s_RecycledEditor.controlID = 0;
            }
        }

        internal virtual void ClickedSearchField()
        {
        }

        internal static SearchFilter CreateFilter(string searchString, SearchMode searchMode)
        {
            SearchFilter filter = new SearchFilter();
            if (!string.IsNullOrEmpty(searchString))
            {
                switch (searchMode)
                {
                    case SearchMode.All:
                        if (!SearchUtility.ParseSearchString(searchString, filter))
                        {
                            filter.nameFilter = searchString;
                            filter.classNames = new string[] { searchString };
                            filter.assetLabels = new string[] { searchString };
                            filter.assetBundleNames = new string[] { searchString };
                            filter.showAllHits = true;
                        }
                        return filter;

                    case SearchMode.Name:
                        filter.nameFilter = searchString;
                        return filter;

                    case SearchMode.Type:
                        filter.classNames = new string[] { searchString };
                        return filter;

                    case SearchMode.Label:
                        filter.assetLabels = new string[] { searchString };
                        return filter;

                    case SearchMode.AssetBundleName:
                        filter.assetBundleNames = new string[] { searchString };
                        return filter;
                }
            }
            return filter;
        }

        internal void FocusSearchField()
        {
            this.m_FocusSearchField = true;
        }

        public virtual void OnDisable()
        {
            searchableWindows.Remove(this);
        }

        public virtual void OnEnable()
        {
            searchableWindows.Add(this);
        }

        [UnityEditor.MenuItem("Assets/Find References In Scene", false, 0x19)]
        private static void OnSearchForReferences()
        {
            SearchForReferencesToInstanceID(Selection.activeInstanceID);
        }

        [UnityEditor.MenuItem("Assets/Find References In Scene", true)]
        private static bool OnSearchForReferencesValidate()
        {
            UnityEngine.Object activeObject = Selection.activeObject;
            return (((activeObject != null) && AssetDatabase.Contains(activeObject)) && !Directory.Exists(AssetDatabase.GetAssetPath(activeObject)));
        }

        internal void SearchFieldGUI()
        {
            this.SearchFieldGUI(EditorGUILayout.kLabelFloatMaxW * 1.5f);
        }

        internal void SearchFieldGUI(float maxWidth)
        {
            Rect position = GUILayoutUtility.GetRect(EditorGUILayout.kLabelFloatMaxW * 0.2f, maxWidth, 16f, 16f, EditorStyles.toolbarSearchField);
            if ((Event.current.type == EventType.MouseDown) && position.Contains(Event.current.mousePosition))
            {
                this.ClickedSearchField();
            }
            GUI.SetNextControlName("SearchFilter");
            if (this.m_FocusSearchField)
            {
                EditorGUI.FocusTextInControl("SearchFilter");
                if (Event.current.type == EventType.Repaint)
                {
                    this.m_FocusSearchField = false;
                }
            }
            int searchMode = (int) this.m_SearchMode;
            if (((Event.current.type == EventType.KeyDown) && (Event.current.keyCode == KeyCode.Escape)) && (GUI.GetNameOfFocusedControl() == "SearchFilter"))
            {
                this.SetSearchFilter("", (SearchMode) searchMode, true);
            }
            string[] names = Enum.GetNames((this.m_HierarchyType != HierarchyType.GameObjects) ? typeof(SearchMode) : typeof(SearchModeHierarchyWindow));
            this.m_SearchFieldControlId = GUIUtility.GetControlID(s_SearchableEditorWindowSearchField, FocusType.Keyboard, position);
            EditorGUI.BeginChangeCheck();
            string searchFilter = EditorGUI.ToolbarSearchField(this.m_SearchFieldControlId, position, names, ref searchMode, this.m_SearchFilter);
            if (EditorGUI.EndChangeCheck())
            {
                this.SetSearchFilter(searchFilter, (SearchMode) searchMode, true);
            }
            this.m_HasSearchFilterFocus = GUIUtility.keyboardControl == this.m_SearchFieldControlId;
            if (((Event.current.type == EventType.KeyDown) && (Event.current.keyCode == KeyCode.Escape)) && ((this.m_SearchFilter != "") && (GUIUtility.hotControl == 0)))
            {
                this.m_SearchFilter = "";
                this.SetSearchFilter(searchFilter, (SearchMode) searchMode, true);
                Event.current.Use();
                this.m_HasSearchFilterFocus = false;
            }
        }

        internal static void SearchForReferencesToInstanceID(int instanceID)
        {
            string str;
            string str2 = AssetDatabase.GetAssetPath(instanceID).Substring(7);
            if (str2.IndexOf(' ') != -1)
            {
                str2 = '"' + str2 + '"';
            }
            if (AssetDatabase.IsMainAsset(instanceID))
            {
                str = "ref:" + str2;
            }
            else
            {
                object[] objArray1 = new object[] { "ref:", instanceID, ":", str2 };
                str = string.Concat(objArray1);
            }
            foreach (SearchableEditorWindow window in searchableWindows)
            {
                if (window.m_HierarchyType == HierarchyType.GameObjects)
                {
                    window.SetSearchFilter(str, SearchMode.All, false);
                    window.m_HasSearchFilterFocus = true;
                    window.Repaint();
                }
            }
        }

        internal void SelectNextSearchResult()
        {
            foreach (SearchableEditorWindow window in searchableWindows)
            {
                if (window is SceneHierarchyWindow)
                {
                    ((SceneHierarchyWindow) window).SelectNext();
                    break;
                }
            }
        }

        internal void SelectPreviousSearchResult()
        {
            foreach (SearchableEditorWindow window in searchableWindows)
            {
                if (window is SceneHierarchyWindow)
                {
                    ((SceneHierarchyWindow) window).SelectPrevious();
                    break;
                }
            }
        }

        internal virtual void SetSearchFilter(string searchFilter, SearchMode mode, bool setAll)
        {
            this.m_SearchMode = mode;
            this.m_SearchFilter = searchFilter;
            if (setAll)
            {
                foreach (SearchableEditorWindow window in searchableWindows)
                {
                    if (((window != this) && (window.m_HierarchyType == this.m_HierarchyType)) && (window.m_HierarchyType != HierarchyType.Assets))
                    {
                        window.SetSearchFilter(this.m_SearchFilter, this.m_SearchMode, false);
                    }
                }
            }
            base.Repaint();
            EditorApplication.Internal_CallSearchHasChanged();
        }

        internal bool hasSearchFilter =>
            (this.m_SearchFilter != "");

        internal bool hasSearchFilterFocus
        {
            get => 
                this.m_HasSearchFilterFocus;
            set
            {
                this.m_HasSearchFilterFocus = value;
            }
        }

        internal SearchMode searchMode
        {
            get => 
                this.m_SearchMode;
            set
            {
                this.m_SearchMode = value;
            }
        }

        public enum SearchMode
        {
            All,
            Name,
            Type,
            Label,
            AssetBundleName
        }

        public enum SearchModeHierarchyWindow
        {
            All,
            Name,
            Type
        }
    }
}

