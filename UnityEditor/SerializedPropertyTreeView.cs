namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEditor.IMGUI.Controls;
    using UnityEngine;
    using UnityEngine.Profiling;

    internal class SerializedPropertyTreeView : TreeView
    {
        [CompilerGenerated]
        private static Func<TreeViewItem, UnityEngine.Object> <>f__am$cache0;
        [CompilerGenerated]
        private static Comparison<TreeViewItem> <>f__am$cache1;
        [CompilerGenerated]
        private static Comparison<TreeViewItem> <>f__am$cache2;
        private bool m_bFilterSelection;
        private int m_ChangedId;
        private ColumnInternal[] m_ColumnsInternal;
        private SerializedPropertyDataStore m_DataStore;
        private List<TreeViewItem> m_Items;
        private int[] m_SelectionFilter;

        public SerializedPropertyTreeView(TreeViewState state, MultiColumnHeader multicolumnHeader, SerializedPropertyDataStore dataStore) : base(state, multicolumnHeader)
        {
            this.m_DataStore = dataStore;
            int length = base.multiColumnHeader.state.columns.Length;
            this.m_ColumnsInternal = new ColumnInternal[length];
            for (int i = 0; i < length; i++)
            {
                Column column = this.Col(i);
                if (column.propertyName != null)
                {
                    this.m_ColumnsInternal[i].dependencyProps = new SerializedProperty[column.propertyName.Length];
                }
            }
            base.multiColumnHeader.sortingChanged += new MultiColumnHeader.HeaderCallback(this.OnSortingChanged);
            base.multiColumnHeader.visibleColumnsChanged += new MultiColumnHeader.HeaderCallback(this.OnVisibleColumnChanged);
            base.showAlternatingRowBackgrounds = true;
            base.showBorder = true;
            base.rowHeight = EditorGUIUtility.singleLineHeight;
        }

        protected override void BeforeRowsGUI()
        {
            int num;
            int num2;
            IList<TreeViewItem> rows = this.GetRows();
            base.GetFirstAndLastVisibleRows(out num, out num2);
            if (num2 != -1)
            {
                for (int i = num; i <= num2; i++)
                {
                    ((SerializedPropertyItem) rows[i]).GetData().Update();
                }
            }
            IList<TreeViewItem> list2 = base.FindRows(base.GetSelection());
            foreach (SerializedPropertyItem item in list2)
            {
                item.GetData().Update();
            }
            base.BeforeRowsGUI();
        }

        protected override TreeViewItem BuildRoot() => 
            new SerializedPropertyItem(-1, -1, null);

        protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
        {
            if (this.m_Items == null)
            {
                SerializedPropertyDataStore.Data[] elements = this.m_DataStore.GetElements();
                this.m_Items = new List<TreeViewItem>(elements.Length);
                for (int i = 0; i < elements.Length; i++)
                {
                    SerializedPropertyItem item = new SerializedPropertyItem(elements[i].objectId, 0, elements[i]);
                    this.m_Items.Add(item);
                }
            }
            IEnumerable<TreeViewItem> items = this.m_Items;
            if (this.m_bFilterSelection)
            {
                if (this.m_SelectionFilter == null)
                {
                    this.m_SelectionFilter = Selection.instanceIDs;
                }
                items = from item in this.m_Items
                    where this.m_SelectionFilter.Contains<int>(item.id)
                    select item;
            }
            else
            {
                this.m_SelectionFilter = null;
            }
            List<TreeViewItem> rows = this.Filter(items).ToList<TreeViewItem>();
            if (base.multiColumnHeader.sortedColumnIndex >= 0)
            {
                this.Sort(rows, base.multiColumnHeader.sortedColumnIndex);
            }
            this.m_ChangedId = 0;
            TreeViewUtility.SetParentAndChildrenForItems(rows, root);
            return rows;
        }

        private void CellGUI(Rect cellRect, SerializedPropertyItem item, int columnIndex, ref TreeView.RowGUIArgs args)
        {
            Profiler.BeginSample("SerializedPropertyTreeView.CellGUI");
            base.CenterRectUsingSingleLineHeight(ref cellRect);
            SerializedPropertyDataStore.Data data = item.GetData();
            Column column = (Column) base.multiColumnHeader.GetColumn(columnIndex);
            if (column.drawDelegate == DefaultDelegates.s_DrawName)
            {
                Profiler.BeginSample("SerializedPropertyTreeView.OnItemGUI.LabelField");
                TreeView.DefaultGUI.Label(cellRect, data.name, base.IsSelected(args.item.id), false);
                Profiler.EndSample();
            }
            else if (column.drawDelegate != null)
            {
                SerializedProperty[] properties = data.properties;
                int num = (column.dependencyIndices == null) ? 0 : column.dependencyIndices.Length;
                for (int i = 0; i < num; i++)
                {
                    this.m_ColumnsInternal[columnIndex].dependencyProps[i] = properties[column.dependencyIndices[i]];
                }
                if (((args.item.id == base.state.lastClickedID) && base.HasFocus()) && (columnIndex == base.multiColumnHeader.state.visibleColumns[(base.multiColumnHeader.state.visibleColumns[0] != 0) ? 0 : 1]))
                {
                    GUI.SetNextControlName(Styles.focusHelper);
                }
                SerializedProperty prop = data.properties[columnIndex];
                EditorGUI.BeginChangeCheck();
                Profiler.BeginSample("SerializedPropertyTreeView.OnItemGUI.drawDelegate");
                column.drawDelegate(cellRect, prop, this.m_ColumnsInternal[columnIndex].dependencyProps);
                Profiler.EndSample();
                if (EditorGUI.EndChangeCheck())
                {
                    this.m_ChangedId = ((column.filter == null) || !column.filter.Active()) ? this.m_ChangedId : GUIUtility.keyboardControl;
                    data.Store();
                    IList<int> selection = base.GetSelection();
                    if (selection.Contains(data.objectId))
                    {
                        IList<TreeViewItem> list2 = base.FindRows(selection);
                        if (<>f__am$cache0 == null)
                        {
                            <>f__am$cache0 = r => ((SerializedPropertyItem) r).GetData().serializedObject.targetObject;
                        }
                        Undo.RecordObjects(Enumerable.Select<TreeViewItem, UnityEngine.Object>(list2, <>f__am$cache0).ToArray<UnityEngine.Object>(), "Modify Multiple Properties");
                        foreach (TreeViewItem item2 in list2)
                        {
                            if (item2.id != args.item.id)
                            {
                                SerializedPropertyDataStore.Data data2 = ((SerializedPropertyItem) item2).GetData();
                                if (IsEditable(data2.serializedObject.targetObject))
                                {
                                    if (column.copyDelegate != null)
                                    {
                                        column.copyDelegate(data2.properties[columnIndex], prop);
                                    }
                                    else
                                    {
                                        DefaultDelegates.s_CopyDefault(data2.properties[columnIndex], prop);
                                    }
                                    data2.Store();
                                }
                            }
                        }
                    }
                }
                Profiler.EndSample();
            }
        }

        private Column Col(int idx) => 
            ((Column) base.multiColumnHeader.state.columns[idx]);

        public void DeserializeState(string uid)
        {
            this.m_bFilterSelection = SessionState.GetBool(uid + Styles.serializeFilterSelection, false);
            for (int i = 0; i < base.multiColumnHeader.state.columns.Length; i++)
            {
                SerializedPropertyFilters.IFilter filter = this.Col(i).filter;
                if (filter != null)
                {
                    string str = SessionState.GetString(uid + Styles.serializeFilter + i, null);
                    if (!string.IsNullOrEmpty(str))
                    {
                        filter.DeserializeState(str);
                    }
                }
            }
            string str2 = SessionState.GetString(uid + Styles.serializeTreeViewState, "");
            string str3 = SessionState.GetString(uid + Styles.serializeColumnHeaderState, "");
            if (!string.IsNullOrEmpty(str2))
            {
                JsonUtility.FromJsonOverwrite(str2, base.state);
            }
            if (!string.IsNullOrEmpty(str3))
            {
                JsonUtility.FromJsonOverwrite(str3, base.multiColumnHeader.state);
            }
        }

        private IEnumerable<TreeViewItem> Filter(IEnumerable<TreeViewItem> rows)
        {
            IEnumerable<TreeViewItem> enumerable = rows;
            int length = this.m_ColumnsInternal.Length;
            for (int i = 0; i < length; i++)
            {
                <Filter>c__AnonStorey2 storey = new <Filter>c__AnonStorey2();
                if (this.IsColumnVisible(i))
                {
                    storey.c = this.Col(i);
                    storey.idx = i;
                    if ((storey.c.filter != null) && storey.c.filter.Active())
                    {
                        if (storey.c.filter.GetType().Equals(typeof(SerializedPropertyFilters.Name)))
                        {
                            <Filter>c__AnonStorey1 storey2 = new <Filter>c__AnonStorey1 {
                                f = (SerializedPropertyFilters.Name) storey.c.filter
                            };
                            enumerable = Enumerable.Where<TreeViewItem>(enumerable, new Func<TreeViewItem, bool>(storey2.<>m__0));
                        }
                        else
                        {
                            enumerable = Enumerable.Where<TreeViewItem>(enumerable, new Func<TreeViewItem, bool>(storey.<>m__0));
                        }
                    }
                }
            }
            return enumerable;
        }

        public void FullReload()
        {
            this.m_Items = null;
            base.Reload();
        }

        private bool IsColumnVisible(int idx)
        {
            for (int i = 0; i < base.multiColumnHeader.state.visibleColumns.Length; i++)
            {
                if (base.multiColumnHeader.state.visibleColumns[i] == idx)
                {
                    return true;
                }
            }
            return false;
        }

        private static bool IsEditable(UnityEngine.Object target) => 
            ((target.hideFlags & HideFlags.NotEditable) == HideFlags.None);

        public bool IsFilteredDirty() => 
            ((this.m_ChangedId != 0) && ((this.m_ChangedId != GUIUtility.keyboardControl) || !EditorGUIUtility.editingTextField));

        protected override void KeyEvent()
        {
            if ((Event.current.type == EventType.KeyDown) && (Event.current.character == '\t'))
            {
                GUI.FocusControl(Styles.focusHelper);
                Event.current.Use();
            }
        }

        public void OnFilterGUI(Rect r)
        {
            EditorGUI.BeginChangeCheck();
            float width = r.width;
            r.width = 16f;
            this.m_bFilterSelection = EditorGUI.Toggle(r, this.m_bFilterSelection);
            r.x += r.width;
            r.width = GUI.skin.label.CalcSize(Styles.filterSelection).x;
            EditorGUI.LabelField(r, Styles.filterSelection);
            r.width = 300f;
            r.x = (width - r.width) + 10f;
            for (int i = 0; i < base.multiColumnHeader.state.columns.Length; i++)
            {
                if (this.IsColumnVisible(i))
                {
                    Column column = this.Col(i);
                    if ((column.filter != null) && column.filter.GetType().Equals(typeof(SerializedPropertyFilters.Name)))
                    {
                        column.filter.OnGUI(r);
                    }
                }
            }
            if (EditorGUI.EndChangeCheck())
            {
                base.Reload();
            }
        }

        private void OnSortingChanged(MultiColumnHeader multiColumnHeader)
        {
            IList<TreeViewItem> rows = this.GetRows();
            this.Sort(rows, multiColumnHeader.sortedColumnIndex);
        }

        private void OnVisibleColumnChanged(MultiColumnHeader header)
        {
            base.Reload();
        }

        protected override void RowGUI(TreeView.RowGUIArgs args)
        {
            SerializedPropertyItem item = (SerializedPropertyItem) args.item;
            for (int i = 0; i < args.GetNumVisibleColumns(); i++)
            {
                this.CellGUI(args.GetCellRect(i), item, args.GetColumn(i), ref args);
            }
        }

        protected override void SelectionChanged(IList<int> selectedIds)
        {
            Selection.instanceIDs = selectedIds.ToArray<int>();
        }

        public void SerializeState(string uid)
        {
            SessionState.SetBool(uid + Styles.serializeFilterSelection, this.m_bFilterSelection);
            for (int i = 0; i < base.multiColumnHeader.state.columns.Length; i++)
            {
                SerializedPropertyFilters.IFilter filter = this.Col(i).filter;
                if (filter != null)
                {
                    string str = filter.SerializeState();
                    if (!string.IsNullOrEmpty(str))
                    {
                        SessionState.SetString(uid + Styles.serializeFilter + i, str);
                    }
                }
            }
            SessionState.SetString(uid + Styles.serializeTreeViewState, JsonUtility.ToJson(base.state));
            SessionState.SetString(uid + Styles.serializeColumnHeaderState, JsonUtility.ToJson(base.multiColumnHeader.state));
        }

        private void Sort(IList<TreeViewItem> rows, int sortIdx)
        {
            <Sort>c__AnonStorey0 storey = new <Sort>c__AnonStorey0 {
                sortIdx = sortIdx
            };
            bool flag = base.multiColumnHeader.IsSortedAscending(storey.sortIdx);
            storey.comp = this.Col(storey.sortIdx).compareDelegate;
            List<TreeViewItem> list = rows as List<TreeViewItem>;
            if (storey.comp != null)
            {
                Comparison<TreeViewItem> comparison;
                Comparison<TreeViewItem> comparison2;
                if (storey.comp == DefaultDelegates.s_CompareName)
                {
                    if (<>f__am$cache1 == null)
                    {
                        <>f__am$cache1 = (lhs, rhs) => EditorUtility.NaturalCompare(((SerializedPropertyItem) lhs).GetData().name, ((SerializedPropertyItem) rhs).GetData().name);
                    }
                    comparison = <>f__am$cache1;
                    if (<>f__am$cache2 == null)
                    {
                        <>f__am$cache2 = (lhs, rhs) => -EditorUtility.NaturalCompare(((SerializedPropertyItem) lhs).GetData().name, ((SerializedPropertyItem) rhs).GetData().name);
                    }
                    comparison2 = <>f__am$cache2;
                }
                else
                {
                    comparison = new Comparison<TreeViewItem>(storey.<>m__0);
                    comparison2 = new Comparison<TreeViewItem>(storey.<>m__1);
                }
                list.Sort(!flag ? comparison2 : comparison);
            }
        }

        public bool Update()
        {
            int num;
            int num2;
            IList<TreeViewItem> rows = this.GetRows();
            base.GetFirstAndLastVisibleRows(out num, out num2);
            bool flag = false;
            if (num2 != -1)
            {
                for (int i = num; i <= num2; i++)
                {
                    flag = flag || ((SerializedPropertyItem) rows[i]).GetData().Update();
                }
            }
            return flag;
        }

        [CompilerGenerated]
        private sealed class <Filter>c__AnonStorey1
        {
            internal SerializedPropertyFilters.Name f;

            internal bool <>m__0(TreeViewItem item) => 
                this.f.Filter(((SerializedPropertyTreeView.SerializedPropertyItem) item).GetData().name);
        }

        [CompilerGenerated]
        private sealed class <Filter>c__AnonStorey2
        {
            internal SerializedPropertyTreeView.Column c;
            internal int idx;

            internal bool <>m__0(TreeViewItem item) => 
                this.c.filter.Filter(((SerializedPropertyTreeView.SerializedPropertyItem) item).GetData().properties[this.idx]);
        }

        [CompilerGenerated]
        private sealed class <Sort>c__AnonStorey0
        {
            internal SerializedPropertyTreeView.Column.CompareEntry comp;
            internal int sortIdx;

            internal int <>m__0(TreeViewItem lhs, TreeViewItem rhs) => 
                this.comp(((SerializedPropertyTreeView.SerializedPropertyItem) lhs).GetData().properties[this.sortIdx], ((SerializedPropertyTreeView.SerializedPropertyItem) rhs).GetData().properties[this.sortIdx]);

            internal int <>m__1(TreeViewItem lhs, TreeViewItem rhs) => 
                -this.comp(((SerializedPropertyTreeView.SerializedPropertyItem) lhs).GetData().properties[this.sortIdx], ((SerializedPropertyTreeView.SerializedPropertyItem) rhs).GetData().properties[this.sortIdx]);
        }

        internal class Column : MultiColumnHeaderState.Column
        {
            public CompareEntry compareDelegate;
            public CopyDelegate copyDelegate;
            public int[] dependencyIndices;
            public DrawEntry drawDelegate;
            public SerializedPropertyFilters.IFilter filter;
            public string propertyName;

            public delegate int CompareEntry(SerializedProperty lhs, SerializedProperty rhs);

            public delegate void CopyDelegate(SerializedProperty target, SerializedProperty source);

            public delegate void DrawEntry(Rect r, SerializedProperty prop, SerializedProperty[] dependencies);
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct ColumnInternal
        {
            public SerializedProperty[] dependencyProps;
        }

        internal class DefaultDelegates
        {
            public static readonly SerializedPropertyTreeView.Column.CompareEntry s_CompareCheckbox = new SerializedPropertyTreeView.Column.CompareEntry(SerializedPropertyTreeView.DefaultDelegates.<s_CompareCheckbox>m__4);
            public static readonly SerializedPropertyTreeView.Column.CompareEntry s_CompareColor = new SerializedPropertyTreeView.Column.CompareEntry(SerializedPropertyTreeView.DefaultDelegates.<s_CompareColor>m__7);
            public static readonly SerializedPropertyTreeView.Column.CompareEntry s_CompareEnum = new SerializedPropertyTreeView.Column.CompareEntry(SerializedPropertyTreeView.DefaultDelegates.<s_CompareEnum>m__5);
            public static readonly SerializedPropertyTreeView.Column.CompareEntry s_CompareFloat = new SerializedPropertyTreeView.Column.CompareEntry(SerializedPropertyTreeView.DefaultDelegates.<s_CompareFloat>m__3);
            public static readonly SerializedPropertyTreeView.Column.CompareEntry s_CompareInt = new SerializedPropertyTreeView.Column.CompareEntry(SerializedPropertyTreeView.DefaultDelegates.<s_CompareInt>m__6);
            public static readonly SerializedPropertyTreeView.Column.CompareEntry s_CompareName = new SerializedPropertyTreeView.Column.CompareEntry(SerializedPropertyTreeView.DefaultDelegates.<s_CompareName>m__8);
            public static readonly SerializedPropertyTreeView.Column.CopyDelegate s_CopyDefault = new SerializedPropertyTreeView.Column.CopyDelegate(SerializedPropertyTreeView.DefaultDelegates.<s_CopyDefault>m__9);
            public static readonly SerializedPropertyTreeView.Column.DrawEntry s_DrawCheckbox = new SerializedPropertyTreeView.Column.DrawEntry(SerializedPropertyTreeView.DefaultDelegates.<s_DrawCheckbox>m__1);
            public static readonly SerializedPropertyTreeView.Column.DrawEntry s_DrawDefault = new SerializedPropertyTreeView.Column.DrawEntry(SerializedPropertyTreeView.DefaultDelegates.<s_DrawDefault>m__0);
            public static readonly SerializedPropertyTreeView.Column.DrawEntry s_DrawName = new SerializedPropertyTreeView.Column.DrawEntry(SerializedPropertyTreeView.DefaultDelegates.<s_DrawName>m__2);

            [CompilerGenerated]
            private static int <s_CompareCheckbox>m__4(SerializedProperty lhs, SerializedProperty rhs) => 
                lhs.boolValue.CompareTo(rhs.boolValue);

            [CompilerGenerated]
            private static int <s_CompareColor>m__7(SerializedProperty lhs, SerializedProperty rhs)
            {
                float num;
                float num2;
                float num3;
                float num4;
                float num5;
                float num6;
                Color.RGBToHSV(lhs.colorValue, out num, out num2, out num3);
                Color.RGBToHSV(rhs.colorValue, out num4, out num5, out num6);
                return num.CompareTo(num4);
            }

            [CompilerGenerated]
            private static int <s_CompareEnum>m__5(SerializedProperty lhs, SerializedProperty rhs) => 
                lhs.enumValueIndex.CompareTo(rhs.enumValueIndex);

            [CompilerGenerated]
            private static int <s_CompareFloat>m__3(SerializedProperty lhs, SerializedProperty rhs) => 
                lhs.floatValue.CompareTo(rhs.floatValue);

            [CompilerGenerated]
            private static int <s_CompareInt>m__6(SerializedProperty lhs, SerializedProperty rhs) => 
                lhs.intValue.CompareTo(rhs.intValue);

            [CompilerGenerated]
            private static int <s_CompareName>m__8(SerializedProperty lhs, SerializedProperty rhs) => 
                0;

            [CompilerGenerated]
            private static void <s_CopyDefault>m__9(SerializedProperty target, SerializedProperty source)
            {
                target.serializedObject.CopyFromSerializedProperty(source);
            }

            [CompilerGenerated]
            private static void <s_DrawCheckbox>m__1(Rect r, SerializedProperty prop, SerializedProperty[] dependencies)
            {
                Profiler.BeginSample("PropDrawCheckbox");
                float num = (r.width / 2f) - 8f;
                r.x += (num < 0f) ? 0f : num;
                EditorGUI.PropertyField(r, prop, GUIContent.none);
                Profiler.EndSample();
            }

            [CompilerGenerated]
            private static void <s_DrawDefault>m__0(Rect r, SerializedProperty prop, SerializedProperty[] dependencies)
            {
                Profiler.BeginSample("PropDrawDefault");
                EditorGUI.PropertyField(r, prop, GUIContent.none);
                Profiler.EndSample();
            }

            [CompilerGenerated]
            private static void <s_DrawName>m__2(Rect r, SerializedProperty prop, SerializedProperty[] dependencies)
            {
            }
        }

        internal class SerializedPropertyItem : TreeViewItem
        {
            private SerializedPropertyDataStore.Data m_Data;

            public SerializedPropertyItem(int id, int depth, SerializedPropertyDataStore.Data ltd) : base(id, depth, (ltd == null) ? "root" : ltd.name)
            {
                this.m_Data = ltd;
            }

            public SerializedPropertyDataStore.Data GetData() => 
                this.m_Data;
        }

        internal static class Styles
        {
            public static readonly GUIStyle entryEven = "OL EntryBackEven";
            public static readonly GUIStyle entryOdd = "OL EntryBackOdd";
            public static readonly GUIContent filterDisable = EditorGUIUtility.TextContent("Disable All|Disables all filters.");
            public static readonly GUIContent filterInvert = EditorGUIUtility.TextContent("Invert Result|Inverts the filtered results.");
            public static readonly GUIContent filterSelection = EditorGUIUtility.TextContent("Lock Selection|Limits the table contents to the active selection.");
            public static readonly string focusHelper = "SerializedPropertyTreeViewFocusHelper";
            public static readonly string serializeColumnHeaderState = "_ColumnHeaderState";
            public static readonly string serializeFilter = "_Filter_";
            public static readonly string serializeFilterDisable = "_FilterDisable";
            public static readonly string serializeFilterInvert = "_FilterInvert";
            public static readonly string serializeFilterSelection = "_FilterSelection";
            public static readonly string serializeTreeViewState = "_TreeViewState";
        }
    }
}

