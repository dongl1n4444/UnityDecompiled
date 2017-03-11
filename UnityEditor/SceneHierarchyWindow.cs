namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditor.IMGUI.Controls;
    using UnityEditor.SceneManagement;
    using UnityEditorInternal;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.SceneManagement;

    [EditorWindowTitle(title="Hierarchy", useTypeNameAsIconName=true)]
    internal class SceneHierarchyWindow : SearchableEditorWindow, IHasCustomMenu
    {
        [CompilerGenerated]
        private static Func<Transform, GameObject> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<Transform, GameObject> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<Scene, string> <>f__am$cache2;
        [CompilerGenerated]
        private static Func<Scene, bool> <>f__am$cache3;
        [CompilerGenerated]
        private static Func<int, Scene> <>f__am$cache4;
        [CompilerGenerated]
        private static Func<Scene, bool> <>f__am$cache5;
        [CompilerGenerated]
        private static GenericMenu.MenuFunction <>f__mg$cache0;
        private const int kInvalidSceneHandle = 0;
        private bool m_AllowAlphaNumericalSort;
        [SerializeField]
        private int m_CurrenRootInstanceID = 0;
        [SerializeField]
        private string m_CurrentSortingName = "";
        private bool m_Debug;
        [NonSerialized]
        private bool m_DidSelectSearchResult;
        [SerializeField]
        private List<string> m_ExpandedScenes = new List<string>();
        [NonSerialized]
        private bool m_FrameOnSelectionSync;
        [NonSerialized]
        private int m_LastFramedID = -1;
        [NonSerialized]
        private double m_LastUserInteractionTime;
        [SerializeField]
        private bool m_Locked;
        [NonSerialized]
        private bool m_SelectionSyncNeeded;
        private Dictionary<string, HierarchySorting> m_SortingObjects = null;
        private TreeViewController m_TreeView;
        private int m_TreeViewKeyboardControlID;
        [NonSerialized]
        private bool m_TreeViewReloadNeeded;
        [SerializeField]
        private TreeViewState m_TreeViewState;
        private static SceneHierarchyWindow s_LastInteractedHierarchy;
        private static List<SceneHierarchyWindow> s_SceneHierarchyWindow = new List<SceneHierarchyWindow>();
        private static Styles s_Styles;

        private void AddCreateGameObjectItemsToMenu(GenericMenu menu, UnityEngine.Object[] context, bool includeCreateEmptyChild, bool includeGameObjectInPath, int targetSceneHandle)
        {
            string[] submenus = Unsupported.GetSubmenus("GameObject");
            foreach (string str in submenus)
            {
                UnityEngine.Object[] temporaryContext = context;
                if (includeCreateEmptyChild || (str.ToLower() != "GameObject/Create Empty Child".ToLower()))
                {
                    if (str.EndsWith("..."))
                    {
                        temporaryContext = null;
                    }
                    if (str.ToLower() == "GameObject/Center On Children".ToLower())
                    {
                        break;
                    }
                    string replacementMenuString = str;
                    if (!includeGameObjectInPath)
                    {
                        replacementMenuString = str.Substring(11);
                    }
                    MenuUtils.ExtractMenuItemWithPath(str, menu, replacementMenuString, temporaryContext, targetSceneHandle, new Action<string, UnityEngine.Object[], int>(this.BeforeCreateGameObjectMenuItemWasExecuted), new Action<string, UnityEngine.Object[], int>(this.AfterCreateGameObjectMenuItemWasExecuted));
                }
            }
        }

        public virtual void AddItemsToMenu(GenericMenu menu)
        {
            if (Unsupported.IsDeveloperBuild())
            {
                if (<>f__mg$cache0 == null)
                {
                    <>f__mg$cache0 = new GenericMenu.MenuFunction(SceneHierarchyWindow.ToggleDebugMode);
                }
                menu.AddItem(new GUIContent("DEVELOPER/Toggle DebugMode"), false, <>f__mg$cache0);
            }
        }

        private void AddNewScene(object userData)
        {
            Scene sceneByPath = SceneManager.GetSceneByPath("");
            if (sceneByPath.IsValid())
            {
                string text = EditorGUIUtility.TextContent("Save Untitled Scene").text;
                string message = EditorGUIUtility.TextContent("Existing Untitled scene needs to be saved before creating a new scene. Only one untitled scene is supported at a time.").text;
                if (!EditorUtility.DisplayDialog(text, message, "Save", "Cancel"))
                {
                    return;
                }
                EditorSceneManager.SaveScene(sceneByPath);
            }
            Scene src = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Additive);
            int handle = (int) userData;
            Scene sceneByHandle = EditorSceneManager.GetSceneByHandle(handle);
            if (sceneByHandle.IsValid())
            {
                EditorSceneManager.MoveSceneAfter(src, sceneByHandle);
            }
        }

        private void AfterCreateGameObjectMenuItemWasExecuted(string menuPath, UnityEngine.Object[] contextObjects, int userData)
        {
            EditorSceneManager.SetTargetSceneForNewGameObjects(0);
            if (this.m_Locked)
            {
                this.m_FrameOnSelectionSync = true;
            }
        }

        private void Awake()
        {
            base.m_HierarchyType = HierarchyType.GameObjects;
            if (this.m_TreeViewState != null)
            {
                this.m_TreeViewState.OnAwake();
            }
        }

        private void BeforeCreateGameObjectMenuItemWasExecuted(string menuPath, UnityEngine.Object[] contextObjects, int userData)
        {
            int sceneHandle = userData;
            EditorSceneManager.SetTargetSceneForNewGameObjects(sceneHandle);
        }

        private void CloseSelectedScenes(bool removeScenes)
        {
            List<int> selectedScenes = this.GetSelectedScenes();
            if (EditorSceneManager.SaveModifiedScenesIfUserWantsTo(this.GetModifiedScenes(selectedScenes)))
            {
                foreach (int num in selectedScenes)
                {
                    EditorSceneManager.CloseScene(EditorSceneManager.GetSceneByHandle(num), removeScenes);
                }
                EditorApplication.RequestRepaintAllViews();
            }
        }

        private void ContextClickOutsideItems()
        {
            Event.current.Use();
            GenericMenu menu = new GenericMenu();
            this.CreateGameObjectContextClick(menu, 0);
            menu.ShowAsContext();
        }

        private void CopyGO()
        {
            Unsupported.CopyGameObjectsToPasteboard();
        }

        private void CreateGameObjectContextClick(GenericMenu menu, int contextClickedItemID)
        {
            menu.AddItem(EditorGUIUtility.TextContent("Copy"), false, new GenericMenu.MenuFunction(this.CopyGO));
            menu.AddItem(EditorGUIUtility.TextContent("Paste"), false, new GenericMenu.MenuFunction(this.PasteGO));
            menu.AddSeparator("");
            if (!base.hasSearchFilter && (this.m_TreeViewState.selectedIDs.Count == 1))
            {
                menu.AddItem(EditorGUIUtility.TextContent("Rename"), false, new GenericMenu.MenuFunction(this.RenameGO));
            }
            else
            {
                menu.AddDisabledItem(EditorGUIUtility.TextContent("Rename"));
            }
            menu.AddItem(EditorGUIUtility.TextContent("Duplicate"), false, new GenericMenu.MenuFunction(this.DuplicateGO));
            menu.AddItem(EditorGUIUtility.TextContent("Delete"), false, new GenericMenu.MenuFunction(this.DeleteGO));
            menu.AddSeparator("");
            bool flag = false;
            if (this.m_TreeViewState.selectedIDs.Count == 1)
            {
                GameObjectTreeViewItem item = this.treeView.FindItem(this.m_TreeViewState.selectedIDs[0]) as GameObjectTreeViewItem;
                if (item != null)
                {
                    <CreateGameObjectContextClick>c__AnonStorey0 storey = new <CreateGameObjectContextClick>c__AnonStorey0 {
                        prefab = PrefabUtility.GetPrefabParent(item.objectPPTR)
                    };
                    if (storey.prefab != null)
                    {
                        menu.AddItem(EditorGUIUtility.TextContent("Select Prefab"), false, new GenericMenu.MenuFunction(storey.<>m__0));
                        flag = true;
                    }
                }
            }
            if (!flag)
            {
                menu.AddDisabledItem(EditorGUIUtility.TextContent("Select Prefab"));
            }
            menu.AddSeparator("");
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = t => t.gameObject;
            }
            this.AddCreateGameObjectItemsToMenu(menu, Enumerable.Select<Transform, GameObject>(Selection.transforms, <>f__am$cache0).ToArray<GameObject>(), false, false, 0);
            menu.ShowAsContext();
        }

        private void CreateGameObjectPopup()
        {
            Rect position = GUILayoutUtility.GetRect(s_Styles.createContent, EditorStyles.toolbarDropDown, null);
            if (Event.current.type == EventType.Repaint)
            {
                EditorStyles.toolbarDropDown.Draw(position, s_Styles.createContent, false, false, false, false);
            }
            if ((Event.current.type == EventType.MouseDown) && position.Contains(Event.current.mousePosition))
            {
                GUIUtility.hotControl = 0;
                GenericMenu menu = new GenericMenu();
                this.AddCreateGameObjectItemsToMenu(menu, null, true, false, 0);
                menu.DropDown(position);
                Event.current.Use();
            }
        }

        private void CreateMultiSceneHeaderContextClick(GenericMenu menu, int contextClickedItemID)
        {
            Scene sceneByHandle = EditorSceneManager.GetSceneByHandle(contextClickedItemID);
            if (!IsSceneHeaderInHierarchyWindow(sceneByHandle))
            {
                Debug.LogError("Context clicked item is not a scene");
            }
            else
            {
                bool flag = SceneManager.sceneCount > 1;
                if (sceneByHandle.isLoaded)
                {
                    GUIContent content = EditorGUIUtility.TextContent("Set Active Scene");
                    if (flag && (SceneManager.GetActiveScene() != sceneByHandle))
                    {
                        menu.AddItem(content, false, new GenericMenu.MenuFunction2(this.SetSceneActive), contextClickedItemID);
                    }
                    else
                    {
                        menu.AddDisabledItem(content);
                    }
                    menu.AddSeparator("");
                }
                if (sceneByHandle.isLoaded)
                {
                    if (!EditorApplication.isPlaying)
                    {
                        menu.AddItem(EditorGUIUtility.TextContent("Save Scene"), false, new GenericMenu.MenuFunction2(this.SaveSelectedScenes), contextClickedItemID);
                        menu.AddItem(EditorGUIUtility.TextContent("Save Scene As"), false, new GenericMenu.MenuFunction2(this.SaveSceneAs), contextClickedItemID);
                        if (flag)
                        {
                            menu.AddItem(EditorGUIUtility.TextContent("Save All"), false, new GenericMenu.MenuFunction2(this.SaveAllScenes), contextClickedItemID);
                        }
                        else
                        {
                            menu.AddDisabledItem(EditorGUIUtility.TextContent("Save All"));
                        }
                    }
                    else
                    {
                        menu.AddDisabledItem(EditorGUIUtility.TextContent("Save Scene"));
                        menu.AddDisabledItem(EditorGUIUtility.TextContent("Save Scene As"));
                        menu.AddDisabledItem(EditorGUIUtility.TextContent("Save All"));
                    }
                    menu.AddSeparator("");
                }
                bool flag2 = EditorSceneManager.loadedSceneCount != this.GetNumLoadedScenesInSelection();
                if (sceneByHandle.isLoaded)
                {
                    GUIContent content2 = EditorGUIUtility.TextContent("Unload Scene");
                    if ((flag2 && !EditorApplication.isPlaying) && !string.IsNullOrEmpty(sceneByHandle.path))
                    {
                        menu.AddItem(content2, false, new GenericMenu.MenuFunction2(this.UnloadSelectedScenes), contextClickedItemID);
                    }
                    else
                    {
                        menu.AddDisabledItem(content2);
                    }
                }
                else
                {
                    GUIContent content3 = EditorGUIUtility.TextContent("Load Scene");
                    if (!EditorApplication.isPlaying)
                    {
                        menu.AddItem(content3, false, new GenericMenu.MenuFunction2(this.LoadSelectedScenes), contextClickedItemID);
                    }
                    else
                    {
                        menu.AddDisabledItem(content3);
                    }
                }
                GUIContent content4 = EditorGUIUtility.TextContent("Remove Scene");
                bool flag5 = this.GetSelectedScenes().Count == SceneManager.sceneCount;
                if ((flag2 && !flag5) && !EditorApplication.isPlaying)
                {
                    menu.AddItem(content4, false, new GenericMenu.MenuFunction2(this.RemoveSelectedScenes), contextClickedItemID);
                }
                else
                {
                    menu.AddDisabledItem(content4);
                }
                if (sceneByHandle.isLoaded)
                {
                    GUIContent content5 = EditorGUIUtility.TextContent("Discard changes");
                    List<int> selectedScenes = this.GetSelectedScenes();
                    Scene[] modifiedScenes = this.GetModifiedScenes(selectedScenes);
                    if (!EditorApplication.isPlaying && (modifiedScenes.Length > 0))
                    {
                        menu.AddItem(content5, false, new GenericMenu.MenuFunction2(this.DiscardChangesInSelectedScenes), contextClickedItemID);
                    }
                    else
                    {
                        menu.AddDisabledItem(content5);
                    }
                }
                menu.AddSeparator("");
                GUIContent content6 = EditorGUIUtility.TextContent("Select Scene Asset");
                if (!string.IsNullOrEmpty(sceneByHandle.path))
                {
                    menu.AddItem(content6, false, new GenericMenu.MenuFunction2(this.SelectSceneAsset), contextClickedItemID);
                }
                else
                {
                    menu.AddDisabledItem(content6);
                }
                GUIContent content7 = EditorGUIUtility.TextContent("Add New Scene");
                if (!EditorApplication.isPlaying)
                {
                    menu.AddItem(content7, false, new GenericMenu.MenuFunction2(this.AddNewScene), contextClickedItemID);
                }
                else
                {
                    menu.AddDisabledItem(content7);
                }
                if (sceneByHandle.isLoaded)
                {
                    menu.AddSeparator("");
                    if (<>f__am$cache1 == null)
                    {
                        <>f__am$cache1 = t => t.gameObject;
                    }
                    this.AddCreateGameObjectItemsToMenu(menu, Enumerable.Select<Transform, GameObject>(Selection.transforms, <>f__am$cache1).ToArray<GameObject>(), false, true, sceneByHandle.handle);
                }
            }
        }

        private void DeleteGO()
        {
            Unsupported.DeleteGameObjectSelection();
        }

        private void DetectUserInteraction()
        {
            Event current = Event.current;
            if ((current.type != EventType.Layout) && (current.type != EventType.Repaint))
            {
                this.m_LastUserInteractionTime = EditorApplication.timeSinceStartup;
            }
        }

        public void DirtySortingMethods()
        {
            this.m_AllowAlphaNumericalSort = EditorPrefs.GetBool("AllowAlphaNumericHierarchy", false);
            this.SetUpSortMethodLists();
            this.treeView.SetSelection(this.treeView.GetSelection(), true);
            this.treeView.ReloadData();
        }

        private void DiscardChangesInSelectedScenes(object userData)
        {
            IEnumerable<string> expandedSceneNames = this.GetExpandedSceneNames();
            List<int> selectedScenes = this.GetSelectedScenes();
            Scene[] modifiedScenes = this.GetModifiedScenes(selectedScenes);
            if (<>f__am$cache3 == null)
            {
                <>f__am$cache3 = scene => !string.IsNullOrEmpty(scene.path);
            }
            Scene[] sceneArray2 = Enumerable.Where<Scene>(modifiedScenes, <>f__am$cache3).ToArray<Scene>();
            if (this.UserAllowedDiscardingChanges(sceneArray2))
            {
                if (sceneArray2.Length != modifiedScenes.Length)
                {
                    Debug.LogWarning("Discarding changes in a scene that have not yet been saved is not supported. Save the scene first or create a new scene.");
                }
                foreach (Scene scene in sceneArray2)
                {
                    EditorSceneManager.ReloadScene(scene);
                }
                if (SceneManager.sceneCount == 1)
                {
                    this.SetScenesExpanded(expandedSceneNames.ToList<string>());
                }
                EditorApplication.RequestRepaintAllViews();
            }
        }

        private float DoSearchResultPathGUI()
        {
            if (!base.hasSearchFilter)
            {
                return 0f;
            }
            GUILayout.FlexibleSpace();
            Rect rect = EditorGUILayout.BeginVertical(EditorStyles.inspectorBig, new GUILayoutOption[0]);
            GUILayout.Label("Path:", new GUILayoutOption[0]);
            if (this.m_TreeView.HasSelection())
            {
                int instanceID = this.m_TreeView.GetSelection()[0];
                IHierarchyProperty property = new HierarchyProperty(HierarchyType.GameObjects);
                property.Find(instanceID, null);
                if (property.isValid)
                {
                    do
                    {
                        EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
                        GUILayout.Label(property.icon, new GUILayoutOption[0]);
                        GUILayout.Label(property.name, new GUILayoutOption[0]);
                        GUILayout.FlexibleSpace();
                        EditorGUILayout.EndHorizontal();
                    }
                    while (property.Parent());
                }
            }
            EditorGUILayout.EndVertical();
            GUILayout.Space(0f);
            return rect.height;
        }

        private void DoToolbar()
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar, new GUILayoutOption[0]);
            this.CreateGameObjectPopup();
            GUILayout.Space(6f);
            if (s_Debug)
            {
                int num;
                int num2;
                this.m_TreeView.gui.GetFirstAndLastRowVisible(out num, out num2);
                GUILayout.Label($"{this.m_TreeView.data.rowCount} ({num}, {num2})", EditorStyles.miniLabel, new GUILayoutOption[0]);
                GUILayout.Space(6f);
            }
            GUILayout.FlexibleSpace();
            Event current = Event.current;
            if ((base.hasSearchFilterFocus && (current.type == EventType.KeyDown)) && ((current.keyCode == KeyCode.DownArrow) || (current.keyCode == KeyCode.UpArrow)))
            {
                GUIUtility.keyboardControl = this.m_TreeViewKeyboardControlID;
                if (this.treeView.IsLastClickedPartOfRows())
                {
                    this.treeView.Frame(this.treeView.state.lastClickedID, true, false);
                    this.m_DidSelectSearchResult = !string.IsNullOrEmpty(base.m_SearchFilter);
                }
                else
                {
                    this.treeView.OffsetSelection(1);
                }
                current.Use();
            }
            base.SearchFieldGUI();
            GUILayout.Space(6f);
            if (this.hasSortMethods)
            {
                if (Application.isPlaying && ((GameObjectTreeViewDataSource) this.treeView.data).isFetchAIssue)
                {
                    GUILayout.Toggle(false, s_Styles.fetchWarning, s_Styles.MiniButton, new GUILayoutOption[0]);
                }
                this.SortMethodsDropDown();
            }
            GUILayout.EndHorizontal();
        }

        private void DoTreeView(float searchPathHeight)
        {
            Rect treeViewRect = this.treeViewRect;
            treeViewRect.height -= searchPathHeight;
            this.treeView.OnGUI(treeViewRect, this.m_TreeViewKeyboardControlID);
        }

        private void DuplicateGO()
        {
            Unsupported.DuplicateGameObjectsUsingPasteboard();
        }

        private void ExecuteCommands()
        {
            Event current = Event.current;
            if ((current.type == EventType.ExecuteCommand) || (current.type == EventType.ValidateCommand))
            {
                bool flag = current.type == EventType.ExecuteCommand;
                if ((current.commandName == "Delete") || (current.commandName == "SoftDelete"))
                {
                    if (flag)
                    {
                        this.DeleteGO();
                    }
                    current.Use();
                    GUIUtility.ExitGUI();
                }
                else if (current.commandName == "Duplicate")
                {
                    if (flag)
                    {
                        this.DuplicateGO();
                    }
                    current.Use();
                    GUIUtility.ExitGUI();
                }
                else if (current.commandName == "Copy")
                {
                    if (flag)
                    {
                        this.CopyGO();
                    }
                    current.Use();
                    GUIUtility.ExitGUI();
                }
                else if (current.commandName == "Paste")
                {
                    if (flag)
                    {
                        this.PasteGO();
                    }
                    current.Use();
                    GUIUtility.ExitGUI();
                }
                else if (current.commandName == "SelectAll")
                {
                    if (flag)
                    {
                        this.SelectAll();
                    }
                    current.Use();
                    GUIUtility.ExitGUI();
                }
                else if (current.commandName == "FrameSelected")
                {
                    if (current.type == EventType.ExecuteCommand)
                    {
                        this.FrameObjectPrivate(Selection.activeInstanceID, true, true, true);
                    }
                    current.Use();
                    GUIUtility.ExitGUI();
                }
                else if (current.commandName == "Find")
                {
                    if (current.type == EventType.ExecuteCommand)
                    {
                        base.FocusSearchField();
                    }
                    current.Use();
                }
            }
        }

        private void ExpandTreeViewItem(int id, bool expand)
        {
            TreeViewDataSource data = this.treeView.data as TreeViewDataSource;
            if (data != null)
            {
                data.SetExpanded(id, expand);
            }
        }

        public void FrameObject(int instanceID, bool ping)
        {
            this.FrameObjectPrivate(instanceID, true, ping, true);
        }

        private void FrameObjectPrivate(int instanceID, bool frame, bool ping, bool animatedFraming)
        {
            if (instanceID != 0)
            {
                if (this.m_LastFramedID != instanceID)
                {
                    this.treeView.EndPing();
                }
                this.SetSearchFilter("", SearchableEditorWindow.SearchMode.All, true);
                this.m_LastFramedID = instanceID;
                this.treeView.Frame(instanceID, frame, ping, animatedFraming);
                this.FrameObjectPrivate(InternalEditorUtility.GetGameObjectInstanceIDFromComponent(instanceID), frame, ping, animatedFraming);
            }
        }

        public static List<SceneHierarchyWindow> GetAllSceneHierarchyWindows() => 
            s_SceneHierarchyWindow;

        public string[] GetCurrentVisibleObjects()
        {
            IList<TreeViewItem> rows = this.m_TreeView.data.GetRows();
            string[] strArray = new string[rows.Count];
            for (int i = 0; i < rows.Count; i++)
            {
                strArray[i] = rows[i].displayName;
            }
            return strArray;
        }

        private IEnumerable<string> GetExpandedSceneNames()
        {
            List<string> list = new List<string>();
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene sceneAt = SceneManager.GetSceneAt(i);
                if (this.treeView.data.IsExpanded(sceneAt.handle))
                {
                    list.Add(sceneAt.name);
                }
            }
            return list;
        }

        private Scene[] GetModifiedScenes(List<int> handles)
        {
            if (<>f__am$cache4 == null)
            {
                <>f__am$cache4 = handle => EditorSceneManager.GetSceneByHandle(handle);
            }
            if (<>f__am$cache5 == null)
            {
                <>f__am$cache5 = scene => scene.isDirty;
            }
            return Enumerable.Where<Scene>(Enumerable.Select<int, Scene>(handles, <>f__am$cache4), <>f__am$cache5).ToArray<Scene>();
        }

        private string GetNameForType(System.Type type) => 
            type.Name;

        private int GetNumLoadedScenesInSelection()
        {
            int num = 0;
            foreach (int num2 in this.GetSelectedScenes())
            {
                if (EditorSceneManager.GetSceneByHandle(num2).isLoaded)
                {
                    num++;
                }
            }
            return num;
        }

        private List<int> GetSelectedGameObjects()
        {
            List<int> list = new List<int>();
            int[] selection = this.m_TreeView.GetSelection();
            foreach (int num in selection)
            {
                if (!IsSceneHeaderInHierarchyWindow(EditorSceneManager.GetSceneByHandle(num)))
                {
                    list.Add(num);
                }
            }
            return list;
        }

        private List<int> GetSelectedScenes()
        {
            List<int> list = new List<int>();
            int[] selection = this.m_TreeView.GetSelection();
            foreach (int num in selection)
            {
                if (IsSceneHeaderInHierarchyWindow(EditorSceneManager.GetSceneByHandle(num)))
                {
                    list.Add(num);
                }
            }
            return list;
        }

        private void Init()
        {
            if (this.m_TreeViewState == null)
            {
                this.m_TreeViewState = new TreeViewState();
            }
            this.m_TreeView = new TreeViewController(this, this.m_TreeViewState);
            this.m_TreeView.itemDoubleClickedCallback = (Action<int>) Delegate.Combine(this.m_TreeView.itemDoubleClickedCallback, new Action<int>(this.TreeViewItemDoubleClicked));
            this.m_TreeView.selectionChangedCallback = (Action<int[]>) Delegate.Combine(this.m_TreeView.selectionChangedCallback, new Action<int[]>(this.TreeViewSelectionChanged));
            this.m_TreeView.onGUIRowCallback = (Action<int, Rect>) Delegate.Combine(this.m_TreeView.onGUIRowCallback, new Action<int, Rect>(this.OnGUIAssetCallback));
            this.m_TreeView.dragEndedCallback = (Action<int[], bool>) Delegate.Combine(this.m_TreeView.dragEndedCallback, new Action<int[], bool>(this.OnDragEndedCallback));
            this.m_TreeView.contextClickItemCallback = (Action<int>) Delegate.Combine(this.m_TreeView.contextClickItemCallback, new Action<int>(this.ItemContextClick));
            this.m_TreeView.contextClickOutsideItemsCallback = (System.Action) Delegate.Combine(this.m_TreeView.contextClickOutsideItemsCallback, new System.Action(this.ContextClickOutsideItems));
            this.m_TreeView.deselectOnUnhandledMouseDown = true;
            bool showRoot = false;
            bool rootItemIsCollapsable = false;
            GameObjectTreeViewDataSource data = new GameObjectTreeViewDataSource(this.m_TreeView, this.m_CurrenRootInstanceID, showRoot, rootItemIsCollapsable);
            GameObjectsTreeViewDragging dragging = new GameObjectsTreeViewDragging(this.m_TreeView);
            GameObjectTreeViewGUI gui = new GameObjectTreeViewGUI(this.m_TreeView, false);
            this.m_TreeView.Init(this.treeViewRect, data, gui, dragging);
            data.searchMode = (int) base.m_SearchMode;
            data.searchString = base.m_SearchFilter;
            this.m_AllowAlphaNumericalSort = EditorPrefs.GetBool("AllowAlphaNumericHierarchy", false) || InternalEditorUtility.inBatchMode;
            this.SetUpSortMethodLists();
            this.m_TreeView.ReloadData();
        }

        public static bool IsSceneHeaderInHierarchyWindow(Scene scene) => 
            scene.IsValid();

        private bool IsTreeViewSelectionInSyncWithBackend() => 
            ((this.m_TreeView != null) && this.m_TreeView.state.selectedIDs.SequenceEqual<int>(Selection.instanceIDs));

        private void ItemContextClick(int contextClickedItemID)
        {
            Event.current.Use();
            GenericMenu menu = new GenericMenu();
            if (IsSceneHeaderInHierarchyWindow(EditorSceneManager.GetSceneByHandle(contextClickedItemID)))
            {
                this.CreateMultiSceneHeaderContextClick(menu, contextClickedItemID);
            }
            else
            {
                this.CreateGameObjectContextClick(menu, contextClickedItemID);
            }
            menu.ShowAsContext();
        }

        private void LoadSelectedScenes(object userdata)
        {
            List<int> selectedScenes = this.GetSelectedScenes();
            foreach (int num in selectedScenes)
            {
                Scene sceneByHandle = EditorSceneManager.GetSceneByHandle(num);
                if (!sceneByHandle.isLoaded)
                {
                    EditorSceneManager.OpenScene(sceneByHandle.path, OpenSceneMode.Additive);
                }
            }
            EditorApplication.RequestRepaintAllViews();
        }

        private void OnBecameVisible()
        {
            if (SceneManager.sceneCount > 0)
            {
                this.treeViewReloadNeeded = true;
            }
        }

        public void OnDestroy()
        {
            if (s_LastInteractedHierarchy == this)
            {
                s_LastInteractedHierarchy = null;
                foreach (SceneHierarchyWindow window in s_SceneHierarchyWindow)
                {
                    if (window != this)
                    {
                        s_LastInteractedHierarchy = window;
                    }
                }
            }
        }

        public override void OnDisable()
        {
            EditorApplication.projectWindowChanged = (EditorApplication.CallbackFunction) Delegate.Remove(EditorApplication.projectWindowChanged, new EditorApplication.CallbackFunction(this.ReloadData));
            EditorApplication.editorApplicationQuit = (UnityAction) Delegate.Remove(EditorApplication.editorApplicationQuit, new UnityAction(this.OnQuit));
            EditorApplication.searchChanged = (EditorApplication.CallbackFunction) Delegate.Remove(EditorApplication.searchChanged, new EditorApplication.CallbackFunction(this.SearchChanged));
            EditorApplication.projectWasLoaded = (UnityAction) Delegate.Remove(EditorApplication.projectWasLoaded, new UnityAction(this.OnProjectWasLoaded));
            EditorSceneManager.sceneWasCreated = (UnityAction<Scene, NewSceneMode>) Delegate.Remove(EditorSceneManager.sceneWasCreated, new UnityAction<Scene, NewSceneMode>(this.OnSceneWasCreated));
            EditorSceneManager.sceneWasOpened = (UnityAction<Scene, OpenSceneMode>) Delegate.Remove(EditorSceneManager.sceneWasOpened, new UnityAction<Scene, OpenSceneMode>(this.OnSceneWasOpened));
            s_SceneHierarchyWindow.Remove(this);
        }

        private void OnDragEndedCallback(int[] draggedInstanceIds, bool draggedItemsFromOwnTreeView)
        {
            if ((draggedInstanceIds != null) && draggedItemsFromOwnTreeView)
            {
                this.ReloadData();
                this.treeView.SetSelection(draggedInstanceIds, true);
                this.treeView.NotifyListenersThatSelectionChanged();
                base.Repaint();
                GUIUtility.ExitGUI();
            }
        }

        public override void OnEnable()
        {
            base.OnEnable();
            base.titleContent = base.GetLocalizedTitleContent();
            s_SceneHierarchyWindow.Add(this);
            EditorApplication.projectWindowChanged = (EditorApplication.CallbackFunction) Delegate.Combine(EditorApplication.projectWindowChanged, new EditorApplication.CallbackFunction(this.ReloadData));
            EditorApplication.editorApplicationQuit = (UnityAction) Delegate.Combine(EditorApplication.editorApplicationQuit, new UnityAction(this.OnQuit));
            EditorApplication.searchChanged = (EditorApplication.CallbackFunction) Delegate.Combine(EditorApplication.searchChanged, new EditorApplication.CallbackFunction(this.SearchChanged));
            EditorApplication.projectWasLoaded = (UnityAction) Delegate.Combine(EditorApplication.projectWasLoaded, new UnityAction(this.OnProjectWasLoaded));
            EditorSceneManager.sceneWasCreated = (UnityAction<Scene, NewSceneMode>) Delegate.Combine(EditorSceneManager.sceneWasCreated, new UnityAction<Scene, NewSceneMode>(this.OnSceneWasCreated));
            EditorSceneManager.sceneWasOpened = (UnityAction<Scene, OpenSceneMode>) Delegate.Combine(EditorSceneManager.sceneWasOpened, new UnityAction<Scene, OpenSceneMode>(this.OnSceneWasOpened));
            s_LastInteractedHierarchy = this;
        }

        private void OnEvent()
        {
            this.treeView.OnEvent();
        }

        private void OnGUI()
        {
            if (s_Styles == null)
            {
                s_Styles = new Styles();
            }
            this.DetectUserInteraction();
            this.SyncIfNeeded();
            this.m_TreeViewKeyboardControlID = GUIUtility.GetControlID(FocusType.Keyboard);
            this.OnEvent();
            Rect rect = new Rect(0f, 0f, base.position.width, base.position.height);
            Event current = Event.current;
            if ((current.type == EventType.MouseDown) && rect.Contains(current.mousePosition))
            {
                this.treeView.EndPing();
                this.SetAsLastInteractedHierarchy();
            }
            this.DoToolbar();
            float searchPathHeight = this.DoSearchResultPathGUI();
            this.DoTreeView(searchPathHeight);
            this.ExecuteCommands();
        }

        private void OnGUIAssetCallback(int instanceID, Rect rect)
        {
            if (EditorApplication.hierarchyWindowItemOnGUI != null)
            {
                EditorApplication.hierarchyWindowItemOnGUI(instanceID, rect);
            }
        }

        private void OnHierarchyChange()
        {
            if (this.m_TreeView != null)
            {
                this.m_TreeView.EndNameEditing(false);
            }
            this.treeViewReloadNeeded = true;
        }

        private void OnLostFocus()
        {
            this.treeView.EndNameEditing(true);
            EditorGUI.EndEditingActiveTextField();
        }

        private void OnProjectWasLoaded()
        {
            this.m_TreeViewState.expandedIDs.Clear();
            if (SceneManager.sceneCount == 1)
            {
                this.treeView.data.SetExpanded(SceneManager.GetSceneAt(0).handle, true);
            }
            this.SetScenesExpanded(this.m_ExpandedScenes);
        }

        private void OnQuit()
        {
            this.m_ExpandedScenes = this.GetExpandedSceneNames().ToList<string>();
        }

        private void OnSceneWasCreated(Scene scene, NewSceneMode mode)
        {
            this.ExpandTreeViewItem(scene.handle, true);
        }

        private void OnSceneWasOpened(Scene scene, OpenSceneMode mode)
        {
            this.ExpandTreeViewItem(scene.handle, true);
        }

        private void OnSelectionChange()
        {
            if (!this.IsTreeViewSelectionInSyncWithBackend())
            {
                this.selectionSyncNeeded = true;
            }
            else if (s_Debug)
            {
                Debug.Log("OnSelectionChange: Selection is already in sync so no framing will happen");
            }
        }

        private void PasteGO()
        {
            Unsupported.PasteGameObjectsFromPasteboard();
        }

        public void ReloadData()
        {
            if (this.m_TreeView == null)
            {
                this.Init();
            }
            else
            {
                this.m_TreeView.ReloadData();
            }
        }

        private void RemoveSelectedScenes(object userData)
        {
            this.CloseSelectedScenes(true);
        }

        private void RenameGO()
        {
            this.treeView.BeginNameEditing(0f);
        }

        private void SaveAllScenes(object userdata)
        {
            EditorSceneManager.SaveOpenScenes();
        }

        private void SaveSceneAs(object userdata)
        {
            int handle = (int) userdata;
            Scene sceneByHandle = EditorSceneManager.GetSceneByHandle(handle);
            if (sceneByHandle.isLoaded)
            {
                EditorSceneManager.SaveSceneAs(sceneByHandle);
            }
        }

        private void SaveSelectedScenes(object userdata)
        {
            List<int> selectedScenes = this.GetSelectedScenes();
            foreach (int num in selectedScenes)
            {
                Scene sceneByHandle = EditorSceneManager.GetSceneByHandle(num);
                if (sceneByHandle.isLoaded)
                {
                    EditorSceneManager.SaveScene(sceneByHandle);
                }
            }
        }

        public void SearchChanged()
        {
            GameObjectTreeViewDataSource data = (GameObjectTreeViewDataSource) this.treeView.data;
            if ((data.searchMode != base.searchMode) || (data.searchString != base.m_SearchFilter))
            {
                data.searchMode = (int) base.searchMode;
                data.searchString = base.m_SearchFilter;
                if (base.m_SearchFilter == "")
                {
                    this.treeView.Frame(Selection.activeInstanceID, true, false);
                }
                this.ReloadData();
            }
        }

        private void SelectAll()
        {
            int[] rowIDs = this.treeView.GetRowIDs();
            this.treeView.SetSelection(rowIDs, false);
            this.TreeViewSelectionChanged(rowIDs);
        }

        internal void SelectNext()
        {
            this.m_TreeView.OffsetSelection(1);
        }

        internal void SelectPrevious()
        {
            this.m_TreeView.OffsetSelection(-1);
        }

        private void SelectSceneAsset(object userData)
        {
            int handle = (int) userData;
            int instanceIDFromGUID = AssetDatabase.GetInstanceIDFromGUID(AssetDatabase.AssetPathToGUID(EditorSceneManager.GetSceneByHandle(handle).path));
            Selection.activeInstanceID = instanceIDFromGUID;
            EditorGUIUtility.PingObject(instanceIDFromGUID);
        }

        private void SetAsLastInteractedHierarchy()
        {
            s_LastInteractedHierarchy = this;
        }

        public void SetCurrentRootInstanceID(int instanceID)
        {
            this.m_CurrenRootInstanceID = instanceID;
            this.Init();
            GUIUtility.ExitGUI();
        }

        public void SetExpandedRecursive(int id, bool expand)
        {
            TreeViewItem item = this.treeView.data.FindItem(id);
            if (item == null)
            {
                this.ReloadData();
                item = this.treeView.data.FindItem(id);
            }
            if (item != null)
            {
                this.treeView.data.SetExpandedWithChildren(item, expand);
            }
        }

        private void SetSceneActive(object userData)
        {
            int handle = (int) userData;
            SceneManager.SetActiveScene(EditorSceneManager.GetSceneByHandle(handle));
        }

        private void SetScenesExpanded(List<string> sceneNames)
        {
            List<int> list = new List<int>();
            foreach (string str in sceneNames)
            {
                Scene sceneByName = SceneManager.GetSceneByName(str);
                if (sceneByName.IsValid())
                {
                    list.Add(sceneByName.handle);
                }
            }
            if (list.Count > 0)
            {
                this.treeView.data.SetExpandedIDs(list.ToArray());
            }
        }

        internal override void SetSearchFilter(string searchFilter, SearchableEditorWindow.SearchMode searchMode, bool setAll)
        {
            base.SetSearchFilter(searchFilter, searchMode, setAll);
            if (this.m_DidSelectSearchResult && string.IsNullOrEmpty(searchFilter))
            {
                this.m_DidSelectSearchResult = false;
                this.FrameObjectPrivate(Selection.activeInstanceID, true, false, false);
                if (GUIUtility.keyboardControl == 0)
                {
                    GUIUtility.keyboardControl = this.m_TreeViewKeyboardControlID;
                }
            }
        }

        private void SetSortFunction(string sortTypeName)
        {
            if (!this.m_SortingObjects.ContainsKey(sortTypeName))
            {
                Debug.LogError("Invalid search type name: " + sortTypeName);
            }
            else
            {
                this.currentSortingName = sortTypeName;
                if (this.treeView.GetSelection().Any<int>())
                {
                    this.treeView.Frame(this.treeView.GetSelection().First<int>(), true, false);
                }
                this.treeView.ReloadData();
            }
        }

        public void SetSortFunction(System.Type sortType)
        {
            this.SetSortFunction(this.GetNameForType(sortType));
        }

        private void SetUpSortMethodLists()
        {
            this.m_SortingObjects = new Dictionary<string, HierarchySorting>();
            TransformSorting sorting = new TransformSorting();
            this.m_SortingObjects.Add(this.GetNameForType(sorting.GetType()), sorting);
            if (this.m_AllowAlphaNumericalSort || !InternalEditorUtility.isHumanControllingUs)
            {
                AlphabeticalSorting sorting2 = new AlphabeticalSorting();
                this.m_SortingObjects.Add(this.GetNameForType(sorting2.GetType()), sorting2);
            }
            this.currentSortingName = this.m_CurrentSortingName;
        }

        protected virtual void ShowButton(Rect r)
        {
            if (s_Styles == null)
            {
                s_Styles = new Styles();
            }
            this.m_Locked = GUI.Toggle(r, this.m_Locked, GUIContent.none, s_Styles.lockButton);
        }

        private void SortFunctionCallback(SceneHierarchySortingWindow.InputData data)
        {
            this.SetSortFunction(data.m_TypeName);
        }

        private void SortMethodsDropDown()
        {
            if (this.hasSortMethods)
            {
                GUIContent defaultSortingContent = this.m_SortingObjects[this.currentSortingName].content;
                if (defaultSortingContent == null)
                {
                    defaultSortingContent = s_Styles.defaultSortingContent;
                    defaultSortingContent.tooltip = this.currentSortingName;
                }
                Rect position = GUILayoutUtility.GetRect(defaultSortingContent, EditorStyles.toolbarButton);
                if (EditorGUI.ButtonMouseDown(position, defaultSortingContent, FocusType.Passive, EditorStyles.toolbarButton))
                {
                    List<SceneHierarchySortingWindow.InputData> list = new List<SceneHierarchySortingWindow.InputData>();
                    foreach (KeyValuePair<string, HierarchySorting> pair in this.m_SortingObjects)
                    {
                        SceneHierarchySortingWindow.InputData item = new SceneHierarchySortingWindow.InputData {
                            m_TypeName = pair.Key,
                            m_Name = ObjectNames.NicifyVariableName(pair.Key),
                            m_Selected = pair.Key == this.m_CurrentSortingName
                        };
                        list.Add(item);
                    }
                    if (SceneHierarchySortingWindow.ShowAtPosition(new Vector2(position.x, position.y + position.height), list, new SceneHierarchySortingWindow.OnSelectCallback(this.SortFunctionCallback)))
                    {
                        GUIUtility.ExitGUI();
                    }
                }
            }
        }

        private void SyncIfNeeded()
        {
            if (this.treeViewReloadNeeded)
            {
                this.treeViewReloadNeeded = false;
                this.ReloadData();
            }
            if (this.selectionSyncNeeded)
            {
                this.selectionSyncNeeded = false;
                bool flag = (EditorApplication.timeSinceStartup - this.m_LastUserInteractionTime) < 0.2;
                bool revealSelectionAndFrameLastSelected = (!this.m_Locked || this.m_FrameOnSelectionSync) || flag;
                bool animatedFraming = flag && revealSelectionAndFrameLastSelected;
                this.m_FrameOnSelectionSync = false;
                this.treeView.SetSelection(Selection.instanceIDs, revealSelectionAndFrameLastSelected, animatedFraming);
            }
        }

        private static void ToggleDebugMode()
        {
            s_Debug = !s_Debug;
        }

        private void TreeViewItemDoubleClicked(int instanceID)
        {
            Scene sceneByHandle = EditorSceneManager.GetSceneByHandle(instanceID);
            if (IsSceneHeaderInHierarchyWindow(sceneByHandle))
            {
                if (sceneByHandle.isLoaded)
                {
                    SceneManager.SetActiveScene(sceneByHandle);
                }
            }
            else
            {
                SceneView.FrameLastActiveSceneView();
            }
        }

        private void TreeViewSelectionChanged(int[] ids)
        {
            Selection.instanceIDs = ids;
            this.m_DidSelectSearchResult = !string.IsNullOrEmpty(base.m_SearchFilter);
        }

        private void UnloadSelectedScenes(object userdata)
        {
            this.CloseSelectedScenes(false);
        }

        private bool UserAllowedDiscardingChanges(Scene[] modifiedScenes)
        {
            string localizedString = LocalizationDatabase.GetLocalizedString("Discard Changes");
            string format = LocalizationDatabase.GetLocalizedString("Are you sure you want to discard the changes in the following scenes:\n\n   {0}\n\nYour changes will be lost.");
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = scene => scene.name;
            }
            string str3 = string.Join("\n   ", Enumerable.Select<Scene, string>(modifiedScenes, <>f__am$cache2).ToArray<string>());
            format = string.Format(format, str3);
            return EditorUtility.DisplayDialog(localizedString, format, LocalizationDatabase.GetLocalizedString("OK"), LocalizationDatabase.GetLocalizedString("Cancel"));
        }

        private string currentSortingName
        {
            get => 
                this.m_CurrentSortingName;
            set
            {
                this.m_CurrentSortingName = value;
                if (!this.m_SortingObjects.ContainsKey(this.m_CurrentSortingName))
                {
                    this.m_CurrentSortingName = this.GetNameForType(typeof(TransformSorting));
                }
                GameObjectTreeViewDataSource data = (GameObjectTreeViewDataSource) this.treeView.data;
                data.sortingState = this.m_SortingObjects[this.m_CurrentSortingName];
            }
        }

        internal static bool debug
        {
            get => 
                lastInteractedHierarchyWindow.m_Debug;
            set
            {
                lastInteractedHierarchyWindow.m_Debug = value;
            }
        }

        private bool hasSortMethods =>
            (this.m_SortingObjects.Count > 1);

        public static SceneHierarchyWindow lastInteractedHierarchyWindow =>
            s_LastInteractedHierarchy;

        public static bool s_Debug
        {
            get => 
                SessionState.GetBool("HierarchyWindowDebug", false);
            set
            {
                SessionState.SetBool("HierarchyWindowDebug", value);
            }
        }

        private bool selectionSyncNeeded
        {
            get => 
                this.m_SelectionSyncNeeded;
            set
            {
                this.m_SelectionSyncNeeded = value;
                if (value)
                {
                    base.Repaint();
                    if (s_Debug)
                    {
                        Debug.Log("Selection sync and frameing on next event");
                    }
                }
            }
        }

        private TreeViewController treeView
        {
            get
            {
                if (this.m_TreeView == null)
                {
                    this.Init();
                }
                return this.m_TreeView;
            }
        }

        private Rect treeViewRect =>
            new Rect(0f, 17f, base.position.width, base.position.height - 17f);

        private bool treeViewReloadNeeded
        {
            get => 
                this.m_TreeViewReloadNeeded;
            set
            {
                this.m_TreeViewReloadNeeded = value;
                if (value)
                {
                    base.Repaint();
                    if (s_Debug)
                    {
                        Debug.Log("Reload treeview on next event");
                    }
                }
            }
        }

        [CompilerGenerated]
        private sealed class <CreateGameObjectContextClick>c__AnonStorey0
        {
            internal UnityEngine.Object prefab;

            internal void <>m__0()
            {
                Selection.activeObject = this.prefab;
                EditorGUIUtility.PingObject(this.prefab.GetInstanceID());
            }
        }

        private class Styles
        {
            public GUIContent createContent = new GUIContent("Create");
            public GUIContent defaultSortingContent = new GUIContent(EditorGUIUtility.FindTexture("CustomSorting"));
            public GUIContent fetchWarning = new GUIContent("", EditorGUIUtility.FindTexture("console.warnicon.sml"), "The current sorting method is taking a lot of time. Consider using 'Transform Sort' in playmode for better performance.");
            private const string kCustomSorting = "CustomSorting";
            private const string kWarningMessage = "The current sorting method is taking a lot of time. Consider using 'Transform Sort' in playmode for better performance.";
            private const string kWarningSymbol = "console.warnicon.sml";
            public GUIStyle lockButton = "IN LockButton";
            public GUIStyle MiniButton = "ToolbarButton";
        }
    }
}

