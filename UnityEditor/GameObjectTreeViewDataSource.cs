namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEditor.IMGUI.Controls;
    using UnityEditor.SceneManagement;
    using UnityEditorInternal;
    using UnityEngine;
    using UnityEngine.Profiling;
    using UnityEngine.SceneManagement;

    internal class GameObjectTreeViewDataSource : LazyTreeViewDataSource
    {
        private const int k_DefaultStartCapacity = 0x3e8;
        private const double k_FetchDelta = 0.1;
        private const HierarchyType k_HierarchyType = HierarchyType.GameObjects;
        private const double k_LongFetchTime = 0.05;
        private const int k_MaxDelayedFetch = 5;
        private readonly int kGameObjectClassID;
        private int m_DelayedFetches;
        private double m_LastFetchTime;
        private List<TreeViewItem> m_ListOfRows;
        private bool m_NeedsChildParentReferenceSetup;
        private int m_RootInstanceID;
        private int m_RowCount;
        private bool m_RowsPartiallyInitialized;
        private int m_SearchMode;
        private string m_SearchString;
        private List<GameObjectTreeViewItem> m_StickySceneHeaderItems;
        public HierarchySorting sortingState;

        public GameObjectTreeViewDataSource(TreeViewController treeView, int rootInstanceID, bool showRoot, bool rootItemIsCollapsable) : base(treeView)
        {
            this.kGameObjectClassID = UnityType.FindTypeByName("GameObject").persistentTypeID;
            this.m_SearchString = "";
            this.m_SearchMode = 0;
            this.m_LastFetchTime = 0.0;
            this.m_DelayedFetches = 0;
            this.m_StickySceneHeaderItems = new List<GameObjectTreeViewItem>();
            this.sortingState = new TransformSorting();
            this.m_RootInstanceID = rootInstanceID;
            base.showRootItem = showRoot;
            base.rootIsCollapsable = rootItemIsCollapsable;
        }

        private bool AddSceneHeaderToSearchIfNeeded(GameObjectTreeViewItem item, HierarchyProperty property, ref int currentSceneHandle)
        {
            Scene scene = property.GetScene();
            if (currentSceneHandle != scene.handle)
            {
                currentSceneHandle = scene.handle;
                this.InitTreeViewItem(item, scene.handle, scene, true, 0, null, false, 0);
                return true;
            }
            return false;
        }

        private void AllocateBackingArrayIfNeeded()
        {
            if (base.m_Rows == null)
            {
                int capacity = (this.m_RowCount <= 0x3e8) ? 0x3e8 : this.m_RowCount;
                this.m_ListOfRows = new List<TreeViewItem>(capacity);
                base.m_Rows = this.m_ListOfRows;
            }
        }

        public override bool CanBeParent(TreeViewItem item)
        {
            this.SetupChildParentReferencesIfNeeded();
            return base.CanBeParent(item);
        }

        private void ClearSearchFilter()
        {
            this.CreateHierarchyProperty().SetSearchFilter("", 0);
        }

        private HierarchyProperty CreateHierarchyProperty()
        {
            HierarchyProperty property = new HierarchyProperty(HierarchyType.GameObjects);
            property.Reset();
            property.alphaSorted = this.IsUsingAlphaSort();
            return property;
        }

        private void CreateRootItem(HierarchyProperty property)
        {
            int depth = 0;
            if (property.isValid)
            {
                base.m_RootItem = new GameObjectTreeViewItem(this.m_RootInstanceID, depth, null, property.name);
            }
            else
            {
                base.m_RootItem = new GameObjectTreeViewItem(this.m_RootInstanceID, depth, null, "RootOfAll");
            }
            if (!base.showRootItem)
            {
                this.SetExpanded(base.m_RootItem, true);
            }
        }

        private void CreateSceneHeaderItems()
        {
            this.m_StickySceneHeaderItems.Clear();
            int sceneCount = SceneManager.sceneCount;
            for (int i = 0; i < sceneCount; i++)
            {
                Scene sceneAt = SceneManager.GetSceneAt(i);
                GameObjectTreeViewItem item = new GameObjectTreeViewItem(0, 0, null, null);
                this.InitTreeViewItem(item, sceneAt.handle, sceneAt, true, 0, null, false, 0);
                this.m_StickySceneHeaderItems.Add(item);
            }
        }

        private GameObjectTreeViewItem EnsureCreatedItem(int row)
        {
            if (row >= base.m_Rows.Count)
            {
                base.m_Rows.Add(null);
            }
            GameObjectTreeViewItem item = (GameObjectTreeViewItem) base.m_Rows[row];
            if (item == null)
            {
                item = new GameObjectTreeViewItem(0, 0, null, null);
                base.m_Rows[row] = item;
            }
            return item;
        }

        public void EnsureFullyInitialized()
        {
            if (this.m_RowsPartiallyInitialized)
            {
                this.InitializeFull();
                this.m_RowsPartiallyInitialized = false;
            }
        }

        public override void FetchData()
        {
            Profiler.BeginSample("SceneHierarchyWindow.FetchData");
            this.m_RowsPartiallyInitialized = false;
            double timeSinceStartup = EditorApplication.timeSinceStartup;
            HierarchyProperty property = this.CreateHierarchyProperty();
            if ((this.m_RootInstanceID != 0) && !property.Find(this.m_RootInstanceID, null))
            {
                Debug.LogError("Root gameobject with id " + this.m_RootInstanceID + " not found!!");
                this.m_RootInstanceID = 0;
                property.Reset();
            }
            this.CreateRootItem(property);
            base.m_NeedRefreshRows = false;
            this.m_NeedsChildParentReferenceSetup = true;
            bool subTreeWanted = this.m_RootInstanceID != 0;
            bool isSearching = !string.IsNullOrEmpty(this.m_SearchString);
            if (isSearching || subTreeWanted)
            {
                if (isSearching)
                {
                    property.SetSearchFilter(this.m_SearchString, this.m_SearchMode);
                }
                this.InitializeProgressivly(property, subTreeWanted, isSearching);
            }
            else
            {
                this.InitializeMinimal();
            }
            double num2 = EditorApplication.timeSinceStartup;
            double num3 = num2 - timeSinceStartup;
            double num4 = num2 - this.m_LastFetchTime;
            if ((num4 > 0.1) && (num3 > 0.05))
            {
                this.m_DelayedFetches++;
            }
            else
            {
                this.m_DelayedFetches = 0;
            }
            this.m_LastFetchTime = timeSinceStartup;
            base.m_TreeView.SetSelection(Selection.instanceIDs, false);
            this.CreateSceneHeaderItems();
            if (SceneHierarchyWindow.s_Debug)
            {
                Debug.Log(string.Concat(new object[] { "Fetch time: ", num3 * 1000.0, " ms, alphaSort = ", this.IsUsingAlphaSort() }));
            }
            Profiler.EndSample();
        }

        private HierarchyProperty FindHierarchyProperty(int instanceID)
        {
            if (this.IsValidHierarchyInstanceID(instanceID))
            {
                HierarchyProperty property2 = this.CreateHierarchyProperty();
                if (property2.Find(instanceID, base.m_TreeView.state.expandedIDs.ToArray()))
                {
                    return property2;
                }
            }
            return null;
        }

        public override TreeViewItem FindItem(int id)
        {
            this.RevealItem(id);
            this.SetupChildParentReferencesIfNeeded();
            return base.FindItem(id);
        }

        public override TreeViewItem GetItem(int row) => 
            base.m_Rows[row];

        protected override HashSet<int> GetParentsAbove(int id)
        {
            HashSet<int> set = new HashSet<int>();
            if (this.IsValidHierarchyInstanceID(id))
            {
                IHierarchyProperty property = new HierarchyProperty(HierarchyType.GameObjects);
                if (property.Find(id, null))
                {
                    while (property.Parent())
                    {
                        set.Add(property.instanceID);
                    }
                }
            }
            return set;
        }

        protected override HashSet<int> GetParentsBelow(int id)
        {
            HashSet<int> set = new HashSet<int>();
            if (this.IsValidHierarchyInstanceID(id))
            {
                IHierarchyProperty property = new HierarchyProperty(HierarchyType.GameObjects);
                if (property.Find(id, null))
                {
                    set.Add(id);
                    int depth = property.depth;
                    while (property.Next(null) && (property.depth > depth))
                    {
                        if (property.hasChildren)
                        {
                            set.Add(property.instanceID);
                        }
                    }
                }
            }
            return set;
        }

        public override int GetRow(int id)
        {
            if (!string.IsNullOrEmpty(this.m_SearchString))
            {
                return base.GetRow(id);
            }
            HierarchyProperty property = this.FindHierarchyProperty(id);
            if (property != null)
            {
                return property.row;
            }
            return -1;
        }

        public override IList<TreeViewItem> GetRows()
        {
            this.InitIfNeeded();
            this.EnsureFullyInitialized();
            return base.m_Rows;
        }

        private void InitializeFull()
        {
            if (SceneHierarchyWindow.debug)
            {
                Log("Init full (" + this.m_RowCount + ")");
            }
            HierarchyProperty property = this.CreateHierarchyProperty();
            this.InitializeRows(property, 0, this.m_RowCount - 1);
        }

        private void InitializeMinimal()
        {
            int num;
            int num2;
            int[] expanded = base.m_TreeView.state.expandedIDs.ToArray();
            HierarchyProperty property = this.CreateHierarchyProperty();
            this.m_RowCount = property.CountRemaining(expanded);
            this.ResizeItemList(this.m_RowCount);
            property.Reset();
            if (SceneHierarchyWindow.debug)
            {
                Log("Init minimal (" + this.m_RowCount + ")");
            }
            base.m_TreeView.gui.GetFirstAndLastRowVisible(out num, out num2);
            this.InitializeRows(property, num, num2);
            this.m_RowsPartiallyInitialized = true;
        }

        private void InitializeProgressivly(HierarchyProperty property, bool subTreeWanted, bool isSearching)
        {
            this.AllocateBackingArrayIfNeeded();
            int minDepth = !subTreeWanted ? 0 : (property.depth + 1);
            if (!isSearching)
            {
                int row = 0;
                int[] expanded = base.expandedIDs.ToArray();
                int num3 = !subTreeWanted ? 0 : (property.depth + 1);
                while (property.NextWithDepthCheck(expanded, minDepth))
                {
                    GameObjectTreeViewItem item = this.EnsureCreatedItem(row);
                    this.InitTreeViewItem(item, property, property.hasChildren, property.depth - num3);
                    row++;
                }
                this.m_RowCount = row;
            }
            else
            {
                this.m_RowCount = this.InitializeSearchResults(property, minDepth);
            }
            this.ResizeItemList(this.m_RowCount);
        }

        private void InitializeRows(HierarchyProperty property, int firstRow, int lastRow)
        {
            property.Reset();
            int[] expanded = base.expandedIDs.ToArray();
            if (firstRow > 0)
            {
                int count = firstRow;
                if (!property.Skip(count, expanded))
                {
                    Debug.LogError("Failed to skip " + count);
                }
            }
            for (int i = firstRow; property.Next(expanded) && (i <= lastRow); i++)
            {
                GameObjectTreeViewItem item = this.EnsureCreatedItem(i);
                this.InitTreeViewItem(item, property, property.hasChildren, property.depth);
            }
        }

        private int InitializeSearchResults(HierarchyProperty property, int minAllowedDepth)
        {
            int currentSceneHandle = -1;
            int row = 0;
            List<int> list = new List<int>();
            while (property.NextWithDepthCheck(null, minAllowedDepth))
            {
                GameObjectTreeViewItem item = this.EnsureCreatedItem(row);
                if (this.AddSceneHeaderToSearchIfNeeded(item, property, ref currentSceneHandle))
                {
                    row++;
                    list.Add(row);
                    if (this.IsSceneHeader(property))
                    {
                        continue;
                    }
                    item = this.EnsureCreatedItem(row);
                }
                this.InitTreeViewItem(item, property, false, 0);
                row++;
            }
            int num3 = row;
            if (list.Count > 0)
            {
                int index = list[0];
                for (int i = 1; i < list.Count; i++)
                {
                    int count = (list[i] - index) - 1;
                    this.m_ListOfRows.Sort(index, count, new TreeViewItemAlphaNumericSort());
                    index = list[i];
                }
                this.m_ListOfRows.Sort(index, num3 - index, new TreeViewItemAlphaNumericSort());
            }
            return num3;
        }

        private void InitTreeViewItem(GameObjectTreeViewItem item, HierarchyProperty property, bool itemHasChildren, int itemDepth)
        {
            this.InitTreeViewItem(item, property.instanceID, property.GetScene(), this.IsSceneHeader(property), property.colorCode, property.pptrValue, itemHasChildren, itemDepth);
        }

        private void InitTreeViewItem(GameObjectTreeViewItem item, int itemID, Scene scene, bool isSceneHeader, int colorCode, UnityEngine.Object pptrObject, bool hasChildren, int depth)
        {
            item.children = null;
            item.id = itemID;
            item.depth = depth;
            item.parent = null;
            if (isSceneHeader)
            {
                item.displayName = !string.IsNullOrEmpty(scene.name) ? scene.name : "Untitled";
            }
            else
            {
                item.displayName = null;
            }
            item.colorCode = colorCode;
            item.objectPPTR = pptrObject;
            item.shouldDisplay = true;
            item.isSceneHeader = isSceneHeader;
            item.scene = scene;
            item.icon = !isSceneHeader ? null : EditorGUIUtility.FindTexture("SceneAsset Icon");
            if (hasChildren)
            {
                item.children = LazyTreeViewDataSource.CreateChildListForCollapsedParent();
            }
        }

        public override bool IsRevealed(int id) => 
            (this.GetRow(id) != -1);

        private bool IsSceneHeader(HierarchyProperty property) => 
            (property.pptrValue == null);

        private bool IsUsingAlphaSort() => 
            (this.sortingState.GetType() == typeof(AlphabeticalSorting));

        private bool IsValidHierarchyInstanceID(int instanceID)
        {
            bool flag = SceneHierarchyWindow.IsSceneHeaderInHierarchyWindow(EditorSceneManager.GetSceneByHandle(instanceID));
            bool flag2 = InternalEditorUtility.GetClassIDWithoutLoadingObject(instanceID) == this.kGameObjectClassID;
            return (flag || flag2);
        }

        private static void Log(string text)
        {
            Debug.Log(text);
        }

        public override void OnInitialize()
        {
            base.OnInitialize();
            GameObjectTreeViewGUI gui = (GameObjectTreeViewGUI) base.m_TreeView.gui;
            gui.scrollHeightChanged += new Action(this.EnsureFullyInitialized);
            gui.scrollPositionChanged += new Action(this.EnsureFullyInitialized);
            gui.mouseAndKeyboardInput += new Action(this.EnsureFullyInitialized);
        }

        private static void Resize(List<TreeViewItem> list, int count)
        {
            int num = list.Count;
            if (count < num)
            {
                list.RemoveRange(count, num - count);
            }
            else if (count > num)
            {
                if (count > list.Capacity)
                {
                    list.Capacity = count + 20;
                }
                list.AddRange(Enumerable.Repeat<TreeViewItem>(null, count - num));
            }
        }

        private void ResizeItemList(int count)
        {
            this.AllocateBackingArrayIfNeeded();
            if (this.m_ListOfRows.Count != count)
            {
                Resize(this.m_ListOfRows, count);
            }
        }

        public override void RevealItem(int itemID)
        {
            if (this.IsValidHierarchyInstanceID(itemID))
            {
                base.RevealItem(itemID);
            }
        }

        internal void SetupChildParentReferencesIfNeeded()
        {
            this.EnsureFullyInitialized();
            if (this.m_NeedsChildParentReferenceSetup)
            {
                this.m_NeedsChildParentReferenceSetup = false;
                TreeViewUtility.SetChildParentReferences(this.GetRows(), base.m_RootItem);
            }
        }

        public bool isFetchAIssue =>
            (this.m_DelayedFetches >= 5);

        public override int rowCount =>
            this.m_RowCount;

        public List<GameObjectTreeViewItem> sceneHeaderItems =>
            this.m_StickySceneHeaderItems;

        public int searchMode
        {
            get => 
                this.m_SearchMode;
            set
            {
                this.m_SearchMode = value;
            }
        }

        public string searchString
        {
            get => 
                this.m_SearchString;
            set
            {
                if (string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(this.m_SearchString))
                {
                    this.ClearSearchFilter();
                }
                this.m_SearchString = value;
            }
        }
    }
}

