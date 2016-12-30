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

    internal class EmissionTreeView : TreeView
    {
        [CompilerGenerated]
        private static Func<TreeViewItem, int> <>f__am$cache0;
        private bool m_bFilterDisable;
        private bool m_bFilterInvert;
        private bool m_bFilterSelection;
        private EmissionTreeDataStore m_DataStore;
        private FocusHelper m_Focus;
        private List<TreeViewItem> m_Items;
        private SerializedPropertyFilters.Name m_MaterialFilter;
        private EmissionMode m_Mode;
        private SerializedPropertyFilters.Name m_NameFilter;
        private List<int> m_SelectedIds;
        private List<int> m_SelectedItems;
        private FocusHelper m_ShiftFocus;

        public EmissionTreeView(TreeViewState state, MultiColumnHeader header, EmissionTreeDataStore dataStore) : base(state, header)
        {
            this.m_SelectedItems = new List<int>();
            this.m_SelectedIds = new List<int>();
            this.m_Focus = new FocusHelper();
            this.m_ShiftFocus = new FocusHelper();
            this.m_NameFilter = new SerializedPropertyFilters.Name();
            this.m_MaterialFilter = new SerializedPropertyFilters.Name();
            this.m_Mode = EmissionMode.Any;
            this.m_DataStore = dataStore;
            base.multiColumnHeader.sortingChanged += new MultiColumnHeader.HeaderCallback(this.OnSortingChanged);
            base.baseIndent = 20f;
            this.m_Focus.itemId = this.m_ShiftFocus.itemId = -1;
            base.showAlternatingRowBackgrounds = true;
            base.showBorder = true;
        }

        protected override void BeforeRowsGUI()
        {
            this.m_DataStore.Update();
            base.BeforeRowsGUI();
        }

        protected override TreeViewItem BuildRoot() => 
            new EmissionItem(-1, -1, new EmissionTreeDataStore.Data());

        protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
        {
            if (this.m_Items == null)
            {
                List<EmissionTreeDataStore.Data> elements = this.m_DataStore.GetElements();
                this.m_Items = new List<TreeViewItem>(elements.Count<EmissionTreeDataStore.Data>());
                for (int i = 0; i < elements.Count; i++)
                {
                    this.m_Items.Add(new EmissionItem(i, 0, elements[i]));
                }
            }
            IEnumerable<TreeViewItem> items = this.m_Items;
            if (this.m_bFilterSelection)
            {
                items = Enumerable.Where<TreeViewItem>(this.m_Items, delegate (TreeViewItem tvi) {
                    EmissionItem item = (EmissionItem) tvi;
                    return this.m_SelectedIds.Contains(item.Id(ColumnIndex.Name)) || this.m_SelectedIds.Contains(item.Id(ColumnIndex.Material));
                });
            }
            if (!this.m_bFilterDisable)
            {
                items = this.Filter(items);
            }
            List<TreeViewItem> rows = items.ToList<TreeViewItem>();
            this.Sort(rows, base.multiColumnHeader.sortedColumnIndex);
            TreeViewUtility.SetParentAndChildrenForItems(rows, root);
            return rows;
        }

        protected override bool CanMultiSelect(TreeViewItem item) => 
            true;

        private void CellGUI(Rect cellRect, EmissionItem item, int columnIndex, ref TreeView.RowGUIArgs args)
        {
            int num;
            int num2;
            bool flag;
            Profiler.BeginSample("EmissionTreeView.OnItemGUI");
            base.GetFirstAndLastVisibleRows(out num, out num2);
            GUIStyle style = item.SelectStyle((ColumnIndex) columnIndex, this.m_Focus.id, this.m_Focus.itemId, out flag);
            bool on = this.m_SelectedIds.Contains(item.Id((ColumnIndex) columnIndex));
            if ((on && base.HasFocus()) && (Event.current.type == EventType.Repaint))
            {
                Styles.s_Selection.Draw(cellRect, false, false, true, true);
            }
            if (Event.current.type == EventType.Repaint)
            {
                style.Draw(cellRect, GUIContent.Temp(item.Name((ColumnIndex) columnIndex), !flag ? "" : Styles.s_Warning), false, false, on, false);
                if (flag)
                {
                    GUI.Label(cellRect, GUIContent.Temp("", Styles.s_Warning));
                }
            }
            if ((Event.current.type == EventType.MouseDown) && cellRect.Contains(Event.current.mousePosition))
            {
                if (Event.current.modifiers == EventModifiers.None)
                {
                    this.m_SelectedItems.Clear();
                    this.m_SelectedIds.Clear();
                    this.m_ShiftFocus.itemId = -1;
                    this.m_ShiftFocus.id = 0;
                }
                if (Event.current.modifiers == EventModifiers.Shift)
                {
                    if (this.m_ShiftFocus.itemId == -1)
                    {
                        this.m_ShiftFocus = this.m_Focus;
                    }
                    int num3 = (this.m_ShiftFocus.row > args.row) ? args.row : this.m_ShiftFocus.row;
                    int num4 = (this.m_ShiftFocus.row > args.row) ? this.m_ShiftFocus.row : args.row;
                    int num5 = (this.m_ShiftFocus.col > columnIndex) ? columnIndex : this.m_ShiftFocus.col;
                    int num6 = (this.m_ShiftFocus.col > columnIndex) ? this.m_ShiftFocus.col : columnIndex;
                    this.m_SelectedIds.Clear();
                    IList<TreeViewItem> rows = this.GetRows();
                    for (int i = num3; i <= num4; i++)
                    {
                        for (int j = num5; j <= num6; j++)
                        {
                            int num9 = ((EmissionItem) rows[i]).Id((ColumnIndex) j);
                            if (!this.m_SelectedIds.Contains(num9))
                            {
                                this.m_SelectedIds.Add(num9);
                            }
                        }
                    }
                }
                else if (!this.m_SelectedIds.Contains(item.Id((ColumnIndex) columnIndex)))
                {
                    this.m_SelectedIds.Add(item.Id((ColumnIndex) columnIndex));
                }
                this.m_Focus.row = args.row;
                this.m_Focus.col = columnIndex;
                this.m_Focus.id = item.Id((ColumnIndex) columnIndex);
                this.m_Focus.itemId = item.id;
                this.SyncIdsToRows();
                Event.current.Use();
            }
            Profiler.EndSample();
        }

        public void DeserializeState(string uid)
        {
            this.m_bFilterSelection = SessionState.GetBool(uid + SerializedPropertyTreeView.Styles.serializeFilterSelection, false);
            this.m_bFilterDisable = SessionState.GetBool(uid + SerializedPropertyTreeView.Styles.serializeFilterDisable, false);
            this.m_bFilterInvert = SessionState.GetBool(uid + SerializedPropertyTreeView.Styles.serializeFilterInvert, false);
            this.m_NameFilter.DeserializeState(SessionState.GetString(uid + Styles.s_NameFilter, ""));
            this.m_MaterialFilter.DeserializeState(SessionState.GetString(uid + Styles.s_MaterialFilter, ""));
            this.m_Mode = (EmissionMode) SessionState.GetInt(uid + Styles.s_ModeFilter, 0);
            string str = SessionState.GetString(uid + SerializedPropertyTreeView.Styles.serializeTreeViewState, "");
            string str2 = SessionState.GetString(uid + SerializedPropertyTreeView.Styles.serializeColumnHeaderState, "");
            if (!string.IsNullOrEmpty(str))
            {
                JsonUtility.FromJsonOverwrite(str, base.state);
            }
            if (!string.IsNullOrEmpty(str2))
            {
                JsonUtility.FromJsonOverwrite(str2, base.multiColumnHeader.state);
            }
        }

        private IEnumerable<TreeViewItem> Filter(IEnumerable<TreeViewItem> rows)
        {
            IEnumerable<TreeViewItem> second = rows;
            if (this.m_NameFilter.Active())
            {
                second = from item in second
                    where this.m_NameFilter.Filter(((EmissionItem) item).Name(ColumnIndex.Name))
                    select item;
            }
            if (this.m_MaterialFilter.Active())
            {
                second = from item in second
                    where this.m_MaterialFilter.Filter(((EmissionItem) item).Name(ColumnIndex.Material))
                    select item;
            }
            if ((base.multiColumnHeader.state.visibleColumns.Count<int>() == 3) && (this.m_Mode != EmissionMode.Any))
            {
                second = from item in second
                    where ((EmissionItem) item).GIMode() == this.m_Mode
                    select item;
            }
            return (!this.m_bFilterInvert ? second : rows.Except<TreeViewItem>(second));
        }

        public void FullReload()
        {
            this.m_Items = null;
            base.Reload();
        }

        protected override void KeyEvent()
        {
            if ((Event.current.type == EventType.KeyDown) && (((((Event.current.keyCode == KeyCode.DownArrow) || (Event.current.keyCode == KeyCode.UpArrow)) || ((Event.current.keyCode == KeyCode.LeftArrow) || (Event.current.keyCode == KeyCode.RightArrow))) || (((Event.current.keyCode == KeyCode.Home) || (Event.current.keyCode == KeyCode.End)) || ((Event.current.keyCode == KeyCode.PageUp) || (Event.current.keyCode == KeyCode.PageDown)))) || (Event.current.character == '\t')))
            {
                IList<TreeViewItem> rows = this.GetRows();
                if (rows.Count == 0)
                {
                    Event.current.Use();
                }
                else
                {
                    if ((!Event.current.control && !Event.current.shift) || (Event.current.character == '\t'))
                    {
                        this.m_SelectedIds.Clear();
                        this.m_SelectedItems.Clear();
                        this.m_ShiftFocus.itemId = -1;
                        this.m_ShiftFocus.id = 0;
                    }
                    else if (Event.current.shift && (this.m_ShiftFocus.itemId == -1))
                    {
                        this.m_ShiftFocus = this.m_Focus;
                    }
                    if (this.m_Focus.id == 0)
                    {
                        this.m_Focus.row = 0;
                        this.m_Focus.col = 0;
                        this.m_Focus.id = ((EmissionItem) rows[0]).Id(ColumnIndex.Name);
                        this.m_Focus.itemId = rows[0].id;
                        this.m_SelectedIds.Add(this.m_Focus.id);
                        this.m_SelectedItems.Add(this.m_Focus.itemId);
                        Event.current.Use();
                    }
                    else
                    {
                        if (Event.current.character == '\t')
                        {
                            int num = !Event.current.shift ? this.m_Focus.col : -(this.m_Focus.col ^ 1);
                            this.m_Focus.row += num;
                            this.m_Focus.col ^= 1;
                            this.m_Focus.row = Math.Min(Math.Max(0, this.m_Focus.row), rows.Count<TreeViewItem>() - 1);
                        }
                        else
                        {
                            int num2;
                            int num3;
                            base.GetFirstAndLastVisibleRows(out num2, out num3);
                            int num4 = Math.Max(1, (num3 - num2) - 1);
                            switch (Event.current.keyCode)
                            {
                                case KeyCode.UpArrow:
                                    this.m_Focus.row = (this.m_Focus.row <= 0) ? 0 : (this.m_Focus.row - 1);
                                    break;

                                case KeyCode.DownArrow:
                                    this.m_Focus.row = ((this.m_Focus.row + 1) >= rows.Count) ? this.m_Focus.row : (this.m_Focus.row + 1);
                                    break;

                                case KeyCode.RightArrow:
                                    this.m_Focus.col = 1;
                                    break;

                                case KeyCode.LeftArrow:
                                    this.m_Focus.col = 0;
                                    break;

                                case KeyCode.Home:
                                    this.m_Focus.row = 0;
                                    break;

                                case KeyCode.End:
                                    this.m_Focus.row = rows.Count<TreeViewItem>() - 1;
                                    break;

                                case KeyCode.PageUp:
                                    this.m_Focus.row = Math.Max(this.m_Focus.row - num4, 0);
                                    break;

                                case KeyCode.PageDown:
                                    this.m_Focus.row = Math.Min((int) (this.m_Focus.row + num4), (int) (rows.Count<TreeViewItem>() - 1));
                                    break;
                            }
                        }
                        this.m_Focus.id = ((EmissionItem) rows[this.m_Focus.row]).Id((ColumnIndex) this.m_Focus.col);
                        this.m_Focus.itemId = rows[this.m_Focus.row].id;
                        if (Event.current.shift)
                        {
                            int num5 = (this.m_ShiftFocus.row > this.m_Focus.row) ? this.m_Focus.row : this.m_ShiftFocus.row;
                            int num6 = (this.m_ShiftFocus.row > this.m_Focus.row) ? this.m_ShiftFocus.row : this.m_Focus.row;
                            int num7 = (this.m_ShiftFocus.col > this.m_Focus.col) ? this.m_Focus.col : this.m_ShiftFocus.col;
                            int num8 = (this.m_ShiftFocus.col > this.m_Focus.col) ? this.m_ShiftFocus.col : this.m_Focus.col;
                            this.m_SelectedIds.Clear();
                            for (int i = num5; i <= num6; i++)
                            {
                                for (int j = num7; j <= num8; j++)
                                {
                                    int item = ((EmissionItem) rows[i]).Id((ColumnIndex) j);
                                    if (!this.m_SelectedIds.Contains(item))
                                    {
                                        this.m_SelectedIds.Add(item);
                                    }
                                }
                            }
                        }
                        else if (!this.m_SelectedIds.Contains(this.m_Focus.id))
                        {
                            this.m_SelectedIds.Add(this.m_Focus.id);
                        }
                        this.SyncIdsToRows();
                        base.FrameItem(this.m_Focus.itemId);
                        Event.current.Use();
                    }
                }
            }
        }

        public void OnFilterGUI(Rect r)
        {
            EditorGUI.BeginChangeCheck();
            float width = r.width;
            r.width = 16f;
            this.m_bFilterSelection = EditorGUI.Toggle(r, this.m_bFilterSelection);
            r.x += r.width;
            r.width = GUI.skin.label.CalcSize(SerializedPropertyTreeView.Styles.filterSelection).x;
            EditorGUI.LabelField(r, SerializedPropertyTreeView.Styles.filterSelection);
            r.width = 300f;
            r.x = width - r.width;
            this.m_NameFilter.OnGUI(r);
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

        protected override void RowGUI(TreeView.RowGUIArgs args)
        {
            EmissionItem item = (EmissionItem) args.item;
            if (this.m_SelectedItems.Contains(args.item.id) && (Event.current.type == EventType.Repaint))
            {
                Styles.s_Selection.Draw(args.rowRect, false, false, true, false);
            }
            for (int i = 0; i < args.GetNumVisibleColumns(); i++)
            {
                this.CellGUI(args.GetCellRect(i), item, args.GetColumn(i), ref args);
            }
        }

        public void SerializeState(string uid)
        {
            SessionState.SetBool(uid + SerializedPropertyTreeView.Styles.serializeFilterSelection, this.m_bFilterSelection);
            SessionState.SetBool(uid + SerializedPropertyTreeView.Styles.serializeFilterDisable, this.m_bFilterDisable);
            SessionState.SetBool(uid + SerializedPropertyTreeView.Styles.serializeFilterInvert, this.m_bFilterInvert);
            SessionState.SetString(uid + Styles.s_NameFilter, this.m_NameFilter.SerializeState());
            SessionState.SetString(uid + Styles.s_MaterialFilter, this.m_MaterialFilter.SerializeState());
            SessionState.SetInt(uid + Styles.s_ModeFilter, (int) this.m_Mode);
            SessionState.SetString(uid + SerializedPropertyTreeView.Styles.serializeTreeViewState, JsonUtility.ToJson(base.state));
            SessionState.SetString(uid + SerializedPropertyTreeView.Styles.serializeColumnHeaderState, JsonUtility.ToJson(base.multiColumnHeader.state));
        }

        private void Sort(IList<TreeViewItem> rows, int sortIdx)
        {
            <Sort>c__AnonStorey0 storey = new <Sort>c__AnonStorey0 {
                sortIdx = sortIdx
            };
            storey.asc = !base.multiColumnHeader.IsSortedAscending(storey.sortIdx) ? -1 : 1;
            List<TreeViewItem> list = rows as List<TreeViewItem>;
            Comparison<TreeViewItem> comparison = new Comparison<TreeViewItem>(storey.<>m__0);
            list.Sort(comparison);
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].id == this.m_Focus.itemId)
                {
                    this.m_Focus.row = i;
                    break;
                }
            }
        }

        private void SyncIdsToRows()
        {
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = tvi => tvi.id;
            }
            this.m_SelectedItems = Enumerable.Select<TreeViewItem, int>(from tvi in this.m_Items
                where this.m_SelectedIds.Contains(((EmissionItem) tvi).Id(ColumnIndex.Name)) || this.m_SelectedIds.Contains(((EmissionItem) tvi).Id(ColumnIndex.Material))
                select tvi, <>f__am$cache0).ToList<int>();
            Selection.instanceIDs = this.m_SelectedIds.ToArray();
        }

        public void SyncSelection()
        {
            if (((Selection.instanceIDs == null) || (this.m_SelectedIds.Count != Selection.instanceIDs.Length)) || this.m_SelectedIds.Except<int>(Selection.instanceIDs).Any<int>())
            {
                this.m_Focus.id = 0;
                this.m_Focus.row = 0;
                this.m_Focus.col = 0;
                this.m_Focus.itemId = -1;
                this.m_SelectedIds.Clear();
                this.m_SelectedItems.Clear();
                if (Selection.instanceIDs != null)
                {
                    foreach (int num in Selection.instanceIDs)
                    {
                        foreach (EmissionItem item in this.m_Items)
                        {
                            if ((item.Id(ColumnIndex.Name) == num) || (item.Id(ColumnIndex.Material) == num))
                            {
                                this.m_SelectedIds.Add(num);
                                this.m_SelectedItems.Add(item.id);
                                this.m_Focus.id = num;
                                this.m_Focus.itemId = item.id;
                            }
                        }
                    }
                    if (this.m_SelectedItems.Count<int>() != 0)
                    {
                        IList<TreeViewItem> rows = this.GetRows();
                        for (int i = 0; i < rows.Count; i++)
                        {
                            EmissionItem item2 = (EmissionItem) rows[i];
                            if (item2.id == this.m_Focus.itemId)
                            {
                                this.m_Focus.row = i;
                                this.m_Focus.col = (item2.Id(ColumnIndex.Name) != this.m_Focus.id) ? 1 : 0;
                                break;
                            }
                        }
                    }
                }
            }
        }

        [CompilerGenerated]
        private sealed class <Sort>c__AnonStorey0
        {
            internal int asc;
            internal int sortIdx;

            internal int <>m__0(TreeViewItem lhs, TreeViewItem rhs)
            {
                EmissionTreeView.EmissionItem item = (EmissionTreeView.EmissionItem) lhs;
                EmissionTreeView.EmissionItem item2 = (EmissionTreeView.EmissionItem) rhs;
                int num = EditorUtility.NaturalCompare(item.Name((EmissionTreeView.ColumnIndex) this.sortIdx), item2.Name((EmissionTreeView.ColumnIndex) this.sortIdx));
                if (num == 0)
                {
                    num = item.Id((EmissionTreeView.ColumnIndex) this.sortIdx).CompareTo(item2.Id((EmissionTreeView.ColumnIndex) this.sortIdx));
                }
                return (num * this.asc);
            }
        }

        internal enum ColumnIndex
        {
            Name,
            Material,
            GIMode
        }

        internal class EmissionItem : TreeViewItem
        {
            private EmissionTreeDataStore.Data m_Data;

            public EmissionItem(int id, int depth, EmissionTreeDataStore.Data data) : base(id, depth)
            {
                this.m_Data = data;
            }

            public static bool Equals(EmissionTreeView.EmissionItem lhs, EmissionTreeView.EmissionItem rhs, EmissionTreeView.ColumnIndex idx)
            {
                if (idx != EmissionTreeView.ColumnIndex.Name)
                {
                    if (idx == EmissionTreeView.ColumnIndex.Material)
                    {
                        return (lhs.m_Data.MaterialId() == rhs.m_Data.MaterialId());
                    }
                    if (idx == EmissionTreeView.ColumnIndex.GIMode)
                    {
                        return (lhs.m_Data.GIModeId() == rhs.m_Data.GIModeId());
                    }
                }
                else
                {
                    return (lhs.m_Data.ObjectId() == rhs.m_Data.ObjectId());
                }
                return false;
            }

            public EmissionTreeView.EmissionMode GIMode() => 
                (!this.m_Data.isBaked ? EmissionTreeView.EmissionMode.Realtime : EmissionTreeView.EmissionMode.Baked);

            public int Id(EmissionTreeView.ColumnIndex idx)
            {
                if (idx != EmissionTreeView.ColumnIndex.Name)
                {
                    if (idx == EmissionTreeView.ColumnIndex.Material)
                    {
                        return this.m_Data.MaterialId();
                    }
                    if (idx == EmissionTreeView.ColumnIndex.GIMode)
                    {
                        return this.m_Data.MaterialId();
                    }
                }
                else
                {
                    return this.m_Data.ObjectId();
                }
                return 0;
            }

            public string Name(EmissionTreeView.ColumnIndex idx)
            {
                if (idx != EmissionTreeView.ColumnIndex.Name)
                {
                    if (idx == EmissionTreeView.ColumnIndex.Material)
                    {
                        return this.m_Data.MaterialName();
                    }
                    if (idx == EmissionTreeView.ColumnIndex.GIMode)
                    {
                        return this.m_Data.GIModeName();
                    }
                }
                else
                {
                    return this.m_Data.ObjectName();
                }
                return "";
            }

            public GUIStyle SelectStyle(EmissionTreeView.ColumnIndex idx, int focusId, int focusItemId, out bool bWarning)
            {
                bool flag = focusId == this.Id(idx);
                bool flag2 = focusItemId == this.id;
                bWarning = false;
                if ((idx == EmissionTreeView.ColumnIndex.Name) || !this.m_Data.IsBlack())
                {
                    return ((!flag2 || !flag) ? TreeView.DefaultStyles.label : TreeView.DefaultStyles.boldLabel);
                }
                bWarning = true;
                return ((!flag2 || !flag) ? EmissionTreeView.Styles.s_Error : EmissionTreeView.Styles.s_ErrorBold);
            }
        }

        internal enum EmissionMode
        {
            Any,
            Realtime,
            Baked
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct FocusHelper
        {
            public int row;
            public int col;
            public int id;
            public int itemId;
        }

        internal static class Styles
        {
            public static readonly GUIStyle s_EntryEven = "OL EntryBackEven";
            public static readonly GUIStyle s_EntryOdd = "OL EntryBackOdd";
            public static GUIStyle s_Error;
            public static GUIStyle s_ErrorBold;
            public static readonly string s_MaterialFilter;
            public static readonly string s_ModeFilter;
            public static readonly string[] s_ModeNames = new string[] { "Any", "Realtime", "Baked" };
            public static readonly int[] s_ModeOptions;
            public static readonly string s_NameFilter;
            public static readonly GUIStyle s_Selection = "PR Label";
            public static readonly string s_Warning = "Emissive color is black - change the color or select a different Global Illumination mode.";

            static Styles()
            {
                int[] numArray1 = new int[3];
                numArray1[1] = 1;
                numArray1[2] = 2;
                s_ModeOptions = numArray1;
                s_NameFilter = "_NameFilter";
                s_MaterialFilter = "_MaterialFilter";
                s_ModeFilter = "_ModeFilter";
                s_Error = new GUIStyle(TreeView.DefaultStyles.label);
                s_ErrorBold = new GUIStyle(TreeView.DefaultStyles.boldLabel);
                Color color = new Color(1f, 0.1f, 0.1f);
                s_ErrorBold.onNormal.textColor = color;
                s_Error.onNormal.textColor = color;
                color = new Color(0.8f, 0.1f, 0.1f);
                s_ErrorBold.normal.textColor = color;
                s_Error.normal.textColor = color;
            }
        }
    }
}

