namespace UnityEditorInternal
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;
    using UnityEditor;
    using UnityEditor.IMGUI.Controls;
    using UnityEngine;

    internal class ProfilerDetailedCallsView : ProfilerDetailedView
    {
        [SerializeField]
        private CallsTreeViewController m_CalleesTreeView;
        [SerializeField]
        private CallsTreeViewController m_CallersTreeView;
        [NonSerialized]
        private bool m_Initialized;
        [NonSerialized]
        private float m_TotalSelectedPropertyTime;
        [NonSerialized]
        private GUIContent m_TotalSelectedPropertyTimeLabel;
        [SerializeField]
        private SplitterState m_VertSplit;

        public ProfilerDetailedCallsView(ProfilerHierarchyGUI mainProfilerHierarchyGUI) : base(mainProfilerHierarchyGUI)
        {
            this.m_Initialized = false;
            this.m_TotalSelectedPropertyTimeLabel = new GUIContent("", "Total time of all calls of the selected function in the frame.");
            float[] relativeSizes = new float[] { 40f, 60f };
            int[] minSizes = new int[] { 50, 50 };
            this.m_VertSplit = new SplitterState(relativeSizes, minSizes, null);
        }

        public void DoGUI(GUIStyle headerStyle, int frameIndex, ProfilerViewType viewType)
        {
            string selectedPropertyPath = ProfilerDriver.selectedPropertyPath;
            if (string.IsNullOrEmpty(selectedPropertyPath))
            {
                base.DrawEmptyPane(headerStyle);
            }
            else
            {
                this.InitIfNeeded();
                this.UpdateIfNeeded(frameIndex, viewType, selectedPropertyPath);
                GUILayout.BeginVertical(new GUILayoutOption[0]);
                GUILayout.Label(this.m_TotalSelectedPropertyTimeLabel, EditorStyles.label, new GUILayoutOption[0]);
                GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true) };
                SplitterGUILayout.BeginVerticalSplit(this.m_VertSplit, options);
                Rect r = EditorGUILayout.BeginVertical(new GUILayoutOption[0]);
                this.m_CalleesTreeView.OnGUI(r);
                EditorGUILayout.EndVertical();
                r = EditorGUILayout.BeginVertical(new GUILayoutOption[0]);
                this.m_CallersTreeView.OnGUI(r);
                EditorGUILayout.EndVertical();
                SplitterGUILayout.EndHorizontalSplit();
                GUILayout.EndVertical();
            }
        }

        private static string GetProfilerPropertyName(string propertyPath)
        {
            int num = propertyPath.LastIndexOf('/');
            return ((num != -1) ? propertyPath.Substring(num + 1) : propertyPath);
        }

        private void InitIfNeeded()
        {
            if (!this.m_Initialized)
            {
                if (this.m_CallersTreeView == null)
                {
                    this.m_CallersTreeView = new CallsTreeViewController(CallsTreeView.Type.Callers);
                }
                this.m_CallersTreeView.callSelected += new CallsTreeViewController.CallSelectedCallback(this.OnCallSelected);
                if (this.m_CalleesTreeView == null)
                {
                    this.m_CalleesTreeView = new CallsTreeViewController(CallsTreeView.Type.Callees);
                }
                this.m_CalleesTreeView.callSelected += new CallsTreeViewController.CallSelectedCallback(this.OnCallSelected);
                this.m_Initialized = true;
            }
        }

        private void OnCallSelected(string path, Event evt)
        {
            EventType type = evt.type;
            if (((type == EventType.ExecuteCommand) || (type == EventType.ValidateCommand)) && (evt.commandName == "FrameSelected"))
            {
                if (type == EventType.ExecuteCommand)
                {
                    base.m_MainProfilerHierarchyGUI.SelectPath(path);
                }
                evt.Use();
            }
        }

        private void UpdateIfNeeded(int frameIndex, ProfilerViewType viewType, string selectedPropertyPath)
        {
            if (!this.m_CachedProfilerPropertyConfig.EqualsTo(frameIndex, viewType, ProfilerColumn.DontSort))
            {
                ProfilerProperty rootProperty = base.m_MainProfilerHierarchyGUI.GetRootProperty();
                string profilerPropertyName = GetProfilerPropertyName(selectedPropertyPath);
                this.m_TotalSelectedPropertyTime = 0f;
                Dictionary<string, CallInformation> dictionary = new Dictionary<string, CallInformation>();
                Dictionary<string, CallInformation> dictionary2 = new Dictionary<string, CallInformation>();
                Stack<ParentCallInfo> stack = new Stack<ParentCallInfo>();
                bool flag = false;
                while (rootProperty.Next(true))
                {
                    float columnAsSingle;
                    string propertyName = rootProperty.propertyName;
                    int depth = rootProperty.depth;
                    if ((stack.Count + 1) != depth)
                    {
                        while ((stack.Count + 1) > depth)
                        {
                            stack.Pop();
                        }
                        flag = (stack.Count != 0) && (profilerPropertyName == stack.Peek().name);
                    }
                    if (stack.Count != 0)
                    {
                        CallInformation information;
                        int num3;
                        int num4;
                        CallInformation information2;
                        ParentCallInfo info2 = stack.Peek();
                        if (profilerPropertyName == propertyName)
                        {
                            columnAsSingle = rootProperty.GetColumnAsSingle(ProfilerColumn.TotalTime);
                            num3 = (int) rootProperty.GetColumnAsSingle(ProfilerColumn.Calls);
                            num4 = (int) rootProperty.GetColumnAsSingle(ProfilerColumn.GCMemory);
                            if (!dictionary.TryGetValue(info2.name, out information))
                            {
                                information2 = new CallInformation {
                                    name = info2.name,
                                    path = info2.path,
                                    callsCount = num3,
                                    gcAllocBytes = num4,
                                    totalCallTimeMs = info2.timeMs,
                                    totalSelfTimeMs = columnAsSingle
                                };
                                dictionary.Add(info2.name, information2);
                            }
                            else
                            {
                                information.callsCount += num3;
                                information.gcAllocBytes += num4;
                                information.totalCallTimeMs += info2.timeMs;
                                information.totalSelfTimeMs += columnAsSingle;
                            }
                            this.m_TotalSelectedPropertyTime += columnAsSingle;
                        }
                        if (flag)
                        {
                            columnAsSingle = rootProperty.GetColumnAsSingle(ProfilerColumn.TotalTime);
                            num3 = (int) rootProperty.GetColumnAsSingle(ProfilerColumn.Calls);
                            num4 = (int) rootProperty.GetColumnAsSingle(ProfilerColumn.GCMemory);
                            if (!dictionary2.TryGetValue(propertyName, out information))
                            {
                                information2 = new CallInformation {
                                    name = propertyName,
                                    path = rootProperty.propertyPath,
                                    callsCount = num3,
                                    gcAllocBytes = num4,
                                    totalCallTimeMs = columnAsSingle,
                                    totalSelfTimeMs = 0.0
                                };
                                dictionary2.Add(propertyName, information2);
                            }
                            else
                            {
                                information.callsCount += num3;
                                information.gcAllocBytes += num4;
                                information.totalCallTimeMs += columnAsSingle;
                            }
                        }
                    }
                    else if (profilerPropertyName == propertyName)
                    {
                        columnAsSingle = rootProperty.GetColumnAsSingle(ProfilerColumn.TotalTime);
                        this.m_TotalSelectedPropertyTime += columnAsSingle;
                    }
                    if (rootProperty.HasChildren)
                    {
                        columnAsSingle = rootProperty.GetColumnAsSingle(ProfilerColumn.TotalTime);
                        ParentCallInfo item = new ParentCallInfo {
                            name = propertyName,
                            path = rootProperty.propertyPath,
                            timeMs = columnAsSingle
                        };
                        stack.Push(item);
                        flag = profilerPropertyName == propertyName;
                    }
                }
                CallsData callsData = new CallsData {
                    calls = dictionary.Values.ToList<CallInformation>(),
                    totalSelectedPropertyTime = this.m_TotalSelectedPropertyTime
                };
                this.m_CallersTreeView.SetCallsData(callsData);
                CallsData data2 = new CallsData {
                    calls = dictionary2.Values.ToList<CallInformation>(),
                    totalSelectedPropertyTime = this.m_TotalSelectedPropertyTime
                };
                this.m_CalleesTreeView.SetCallsData(data2);
                this.m_TotalSelectedPropertyTimeLabel.text = profilerPropertyName + $" - Total time: {this.m_TotalSelectedPropertyTime:f2} ms";
                this.m_CachedProfilerPropertyConfig.Set(frameIndex, viewType, ProfilerColumn.TotalTime);
            }
        }

        private class CallInformation
        {
            public int callsCount;
            public int gcAllocBytes;
            public string name;
            public string path;
            public double timePercent;
            public double totalCallTimeMs;
            public double totalSelfTimeMs;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct CallsData
        {
            public List<ProfilerDetailedCallsView.CallInformation> calls;
            public float totalSelectedPropertyTime;
        }

        private class CallsTreeView : TreeView
        {
            internal ProfilerDetailedCallsView.CallsData m_CallsData;
            private Type m_Type;
            private static string s_NoneText = LocalizationDatabase.GetLocalizedString("None");

            public CallsTreeView(Type type, TreeViewState treeViewState, MultiColumnHeader multicolumnHeader) : base(treeViewState, multicolumnHeader)
            {
                this.m_Type = type;
                base.showBorder = true;
                base.showAlternatingRowBackgrounds = true;
                multicolumnHeader.sortingChanged += new MultiColumnHeader.HeaderCallback(this.OnSortingChanged);
                base.Reload();
            }

            protected override TreeViewItem BuildRoot()
            {
                TreeViewItem item2 = new TreeViewItem {
                    id = 0,
                    depth = -1,
                    displayName = "Root"
                };
                TreeViewItem root = item2;
                List<TreeViewItem> rows = new List<TreeViewItem>();
                if ((this.m_CallsData.calls != null) && (this.m_CallsData.calls.Count != 0))
                {
                    rows.Capacity = this.m_CallsData.calls.Count;
                    for (int i = 0; i < this.m_CallsData.calls.Count; i++)
                    {
                        item2 = new TreeViewItem {
                            id = i + 1,
                            depth = 0,
                            displayName = this.m_CallsData.calls[i].name
                        };
                        rows.Add(item2);
                    }
                }
                else
                {
                    item2 = new TreeViewItem {
                        id = 1,
                        depth = 0,
                        displayName = s_NoneText
                    };
                    rows.Add(item2);
                }
                TreeView.SetupParentsAndChildrenFromDepths(root, rows);
                return root;
            }

            private void CellGUI(Rect cellRect, TreeViewItem item, Column column, ref TreeView.RowGUIArgs args)
            {
                if (this.m_CallsData.calls.Count == 0)
                {
                    base.RowGUI(args);
                }
                else
                {
                    ProfilerDetailedCallsView.CallInformation information = this.m_CallsData.calls[args.item.id - 1];
                    base.CenterRectUsingSingleLineHeight(ref cellRect);
                    switch (column)
                    {
                        case Column.Name:
                            TreeView.DefaultGUI.Label(cellRect, information.name, args.selected, args.focused);
                            break;

                        case Column.Calls:
                        {
                            string label = information.callsCount.ToString();
                            TreeView.DefaultGUI.LabelRightAligned(cellRect, label, args.selected, args.focused);
                            break;
                        }
                        case Column.GcAlloc:
                            TreeView.DefaultGUI.LabelRightAligned(cellRect, information.gcAllocBytes.ToString(), args.selected, args.focused);
                            break;

                        case Column.TimeMs:
                            TreeView.DefaultGUI.LabelRightAligned(cellRect, ((this.m_Type != Type.Callees) ? information.totalSelfTimeMs : information.totalCallTimeMs).ToString("f2"), args.selected, args.focused);
                            break;

                        case Column.TimePercent:
                            TreeView.DefaultGUI.LabelRightAligned(cellRect, (information.timePercent * 100.0).ToString("f2"), args.selected, args.focused);
                            break;
                    }
                }
            }

            private void OnSortingChanged(MultiColumnHeader header)
            {
                Comparison<ProfilerDetailedCallsView.CallInformation> comparison;
                <OnSortingChanged>c__AnonStorey0 storey = new <OnSortingChanged>c__AnonStorey0();
                if (header.sortedColumnIndex != -1)
                {
                    storey.orderMultiplier = !header.IsSortedAscending(header.sortedColumnIndex) ? -1 : 1;
                    switch (header.sortedColumnIndex)
                    {
                        case 0:
                            comparison = new Comparison<ProfilerDetailedCallsView.CallInformation>(storey.<>m__0);
                            goto Label_00D1;

                        case 1:
                            comparison = new Comparison<ProfilerDetailedCallsView.CallInformation>(storey.<>m__1);
                            goto Label_00D1;

                        case 2:
                            comparison = new Comparison<ProfilerDetailedCallsView.CallInformation>(storey.<>m__2);
                            goto Label_00D1;

                        case 3:
                            comparison = new Comparison<ProfilerDetailedCallsView.CallInformation>(storey.<>m__3);
                            goto Label_00D1;

                        case 4:
                            comparison = new Comparison<ProfilerDetailedCallsView.CallInformation>(storey.<>m__4);
                            goto Label_00D1;

                        case 5:
                            comparison = new Comparison<ProfilerDetailedCallsView.CallInformation>(storey.<>m__5);
                            goto Label_00D1;
                    }
                }
                return;
            Label_00D1:
                this.m_CallsData.calls.Sort(comparison);
                base.Reload();
            }

            protected override void RowGUI(TreeView.RowGUIArgs args)
            {
                TreeViewItem item = args.item;
                for (int i = 0; i < args.GetNumVisibleColumns(); i++)
                {
                    this.CellGUI(args.GetCellRect(i), item, (Column) args.GetColumn(i), ref args);
                }
            }

            public void SetCallsData(ProfilerDetailedCallsView.CallsData callsData)
            {
                this.m_CallsData = callsData;
                foreach (ProfilerDetailedCallsView.CallInformation information in this.m_CallsData.calls)
                {
                    information.timePercent = (this.m_Type != Type.Callees) ? (information.totalSelfTimeMs / information.totalCallTimeMs) : (information.totalCallTimeMs / ((double) this.m_CallsData.totalSelectedPropertyTime));
                }
                this.OnSortingChanged(base.multiColumnHeader);
            }

            [CompilerGenerated]
            private sealed class <OnSortingChanged>c__AnonStorey0
            {
                internal int orderMultiplier;

                internal int <>m__0(ProfilerDetailedCallsView.CallInformation callInfo1, ProfilerDetailedCallsView.CallInformation callInfo2) => 
                    (callInfo1.name.CompareTo(callInfo2.name) * this.orderMultiplier);

                internal int <>m__1(ProfilerDetailedCallsView.CallInformation callInfo1, ProfilerDetailedCallsView.CallInformation callInfo2) => 
                    (callInfo1.callsCount.CompareTo(callInfo2.callsCount) * this.orderMultiplier);

                internal int <>m__2(ProfilerDetailedCallsView.CallInformation callInfo1, ProfilerDetailedCallsView.CallInformation callInfo2) => 
                    (callInfo1.gcAllocBytes.CompareTo(callInfo2.gcAllocBytes) * this.orderMultiplier);

                internal int <>m__3(ProfilerDetailedCallsView.CallInformation callInfo1, ProfilerDetailedCallsView.CallInformation callInfo2) => 
                    (callInfo1.totalCallTimeMs.CompareTo(callInfo2.totalCallTimeMs) * this.orderMultiplier);

                internal int <>m__4(ProfilerDetailedCallsView.CallInformation callInfo1, ProfilerDetailedCallsView.CallInformation callInfo2) => 
                    (callInfo1.timePercent.CompareTo(callInfo2.timePercent) * this.orderMultiplier);

                internal int <>m__5(ProfilerDetailedCallsView.CallInformation callInfo1, ProfilerDetailedCallsView.CallInformation callInfo2) => 
                    (callInfo1.callsCount.CompareTo(callInfo2.callsCount) * this.orderMultiplier);
            }

            public enum Column
            {
                Name,
                Calls,
                GcAlloc,
                TimeMs,
                TimePercent,
                Count
            }

            public enum Type
            {
                Callers,
                Callees
            }
        }

        [Serializable]
        private class CallsTreeViewController
        {
            [NonSerialized]
            private bool m_Initialized;
            [SerializeField]
            private ProfilerDetailedCallsView.CallsTreeView.Type m_Type;
            [NonSerialized]
            private ProfilerDetailedCallsView.CallsTreeView m_View;
            [SerializeField]
            private MultiColumnHeaderState m_ViewHeaderState;
            [SerializeField]
            private TreeViewState m_ViewState;

            [field: CompilerGenerated, DebuggerBrowsable(0)]
            public event CallSelectedCallback callSelected;

            public CallsTreeViewController(ProfilerDetailedCallsView.CallsTreeView.Type type)
            {
                this.m_Type = type;
            }

            private MultiColumnHeaderState CreateDefaultMultiColumnHeaderState()
            {
                MultiColumnHeaderState.Column[] columns = new MultiColumnHeaderState.Column[5];
                MultiColumnHeaderState.Column column = new MultiColumnHeaderState.Column {
                    headerContent = (this.m_Type != ProfilerDetailedCallsView.CallsTreeView.Type.Callers) ? Styles.calleesLabel : Styles.callersLabel,
                    headerTextAlignment = TextAlignment.Left,
                    sortedAscending = true,
                    sortingArrowAlignment = TextAlignment.Center,
                    width = 150f,
                    minWidth = 150f,
                    autoResize = true,
                    allowToggleVisibility = false
                };
                columns[0] = column;
                column = new MultiColumnHeaderState.Column {
                    headerContent = Styles.callsLabel,
                    headerTextAlignment = TextAlignment.Right,
                    sortedAscending = false,
                    sortingArrowAlignment = TextAlignment.Center,
                    width = 60f,
                    minWidth = 60f,
                    autoResize = false,
                    allowToggleVisibility = true
                };
                columns[1] = column;
                column = new MultiColumnHeaderState.Column {
                    headerContent = Styles.gcAllocLabel,
                    headerTextAlignment = TextAlignment.Right,
                    sortedAscending = false,
                    sortingArrowAlignment = TextAlignment.Center,
                    width = 60f,
                    minWidth = 60f,
                    autoResize = false,
                    allowToggleVisibility = true
                };
                columns[2] = column;
                column = new MultiColumnHeaderState.Column {
                    headerContent = (this.m_Type != ProfilerDetailedCallsView.CallsTreeView.Type.Callers) ? Styles.timeMsCalleesLabel : Styles.timeMsCallersLabel,
                    headerTextAlignment = TextAlignment.Right,
                    sortedAscending = false,
                    sortingArrowAlignment = TextAlignment.Center,
                    width = 60f,
                    minWidth = 60f,
                    autoResize = false,
                    allowToggleVisibility = true
                };
                columns[3] = column;
                column = new MultiColumnHeaderState.Column {
                    headerContent = (this.m_Type != ProfilerDetailedCallsView.CallsTreeView.Type.Callers) ? Styles.timePctCalleesLabel : Styles.timePctCallersLabel,
                    headerTextAlignment = TextAlignment.Right,
                    sortedAscending = false,
                    sortingArrowAlignment = TextAlignment.Center,
                    width = 60f,
                    minWidth = 60f,
                    autoResize = false,
                    allowToggleVisibility = true
                };
                columns[4] = column;
                return new MultiColumnHeaderState(columns) { sortedColumnIndex = 3 };
            }

            private void HandleCommandEvents()
            {
                if ((GUIUtility.keyboardControl == this.m_View.treeViewControlID) && (this.m_ViewState.selectedIDs.Count != 0))
                {
                    int num = this.m_ViewState.selectedIDs.First<int>() - 1;
                    if (num < this.m_View.m_CallsData.calls.Count)
                    {
                        ProfilerDetailedCallsView.CallInformation information = this.m_View.m_CallsData.calls[num];
                        if (this.callSelected != null)
                        {
                            Event current = Event.current;
                            this.callSelected(information.path, current);
                        }
                    }
                }
            }

            private void InitIfNeeded()
            {
                if (!this.m_Initialized)
                {
                    if (this.m_ViewState == null)
                    {
                        this.m_ViewState = new TreeViewState();
                    }
                    bool flag = this.m_ViewHeaderState == null;
                    MultiColumnHeaderState destination = this.CreateDefaultMultiColumnHeaderState();
                    if (MultiColumnHeaderState.CanOverwriteSerializedFields(this.m_ViewHeaderState, destination))
                    {
                        MultiColumnHeaderState.OverwriteSerializedFields(this.m_ViewHeaderState, destination);
                    }
                    this.m_ViewHeaderState = destination;
                    MultiColumnHeader multicolumnHeader = new MultiColumnHeader(this.m_ViewHeaderState) {
                        height = 25f
                    };
                    if (flag)
                    {
                        multicolumnHeader.state.visibleColumns = new int[] { 0, 1, 3, 4 };
                        multicolumnHeader.ResizeToFit();
                    }
                    this.m_View = new ProfilerDetailedCallsView.CallsTreeView(this.m_Type, this.m_ViewState, multicolumnHeader);
                    this.m_Initialized = true;
                }
            }

            public void OnGUI(Rect r)
            {
                this.InitIfNeeded();
                this.m_View.OnGUI(r);
                this.HandleCommandEvents();
            }

            public void SetCallsData(ProfilerDetailedCallsView.CallsData callsData)
            {
                this.InitIfNeeded();
                this.m_View.SetCallsData(callsData);
            }

            public delegate void CallSelectedCallback(string path, Event evt);

            private static class Styles
            {
                public static GUIContent calleesLabel = new GUIContent("Calls To", "Functions which are called from the selected function\n\n(Press 'F' for frame selection)");
                public static GUIContent callersLabel = new GUIContent("Called From", "Parents the selected function is called from\n\n(Press 'F' for frame selection)");
                public static GUIContent callsLabel = new GUIContent("Calls", "Total number of calls in a selected frame");
                public static GUIContent gcAllocLabel = new GUIContent("GC Alloc");
                public static GUIContent timeMsCalleesLabel = new GUIContent("Time ms", "Total time the child call spend within selected function");
                public static GUIContent timeMsCallersLabel = new GUIContent("Time ms", "Total time the selected function spend within a parent");
                public static GUIContent timePctCalleesLabel = new GUIContent("Time %", "Shows how often child call was called from the selected function");
                public static GUIContent timePctCallersLabel = new GUIContent("Time %", "Shows how often the selected function was called from the parent call");
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct ParentCallInfo
        {
            public string name;
            public string path;
            public float timeMs;
        }
    }
}

