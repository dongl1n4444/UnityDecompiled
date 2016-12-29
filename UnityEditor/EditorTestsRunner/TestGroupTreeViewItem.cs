namespace UnityEditor.EditorTestsRunner
{
    using NUnit.Core;
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using UnityEditor.IMGUI.Controls;
    using UnityEngine;

    internal class TestGroupTreeViewItem : EditorTestTreeViewItem
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string <GroupDescription>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string <GroupFullName>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string <GroupName>k__BackingField;
        private EditorTestResultGroup m_EditorTestGroupResult;

        public TestGroupTreeViewItem(EditorTestResultGroup result, TestSuite suite, int depth, TreeViewItem parent) : base(suite, depth, parent)
        {
            this.m_EditorTestGroupResult = result;
            this.GroupName = suite.get_TestName().get_Name();
            this.GroupFullName = suite.get_TestName().get_FullName();
            this.GroupDescription = suite.get_Description();
        }

        public override bool IsVisible(FilteringOptions options)
        {
            if ((options.categories != null) && (options.categories.Length > 0))
            {
                return false;
            }
            return base.IsVisible(options);
        }

        public string GroupDescription { get; private set; }

        public string GroupFullName { get; private set; }

        public string GroupName { get; private set; }

        public override Texture2D icon
        {
            get => 
                (GuiHelper.GetIconForResult(this.m_EditorTestGroupResult.resultState) as Texture2D);
            set
            {
                base.icon = value;
            }
        }
    }
}

