namespace UnityEditorInternal
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEditor.IMGUI.Controls;
    using UnityEngine;

    internal class AddCurvesPopupHierarchyGUI : TreeViewGUI
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool <showPlusButton>k__BackingField;
        public EditorWindow owner;
        private GUIStyle plusButtonBackgroundStyle;
        private GUIStyle plusButtonStyle;
        private const float plusButtonWidth = 17f;

        public AddCurvesPopupHierarchyGUI(TreeViewController treeView, EditorWindow owner) : base(treeView, true)
        {
            this.plusButtonStyle = new GUIStyle("OL Plus");
            this.plusButtonBackgroundStyle = new GUIStyle("Tag MenuItem");
            this.owner = owner;
        }

        public override bool BeginRename(TreeViewItem item, float delay) => 
            false;

        protected override Texture GetIconForItem(TreeViewItem item)
        {
            if (item != null)
            {
                return item.icon;
            }
            return null;
        }

        protected override bool IsRenaming(int id) => 
            false;

        public override void OnRowGUI(Rect rowRect, TreeViewItem node, int row, bool selected, bool focused)
        {
            base.OnRowGUI(rowRect, node, row, selected, focused);
            AddCurvesPopupPropertyNode node2 = node as AddCurvesPopupPropertyNode;
            if (((node2 != null) && (node2.curveBindings != null)) && (node2.curveBindings.Length != 0))
            {
                Rect position = new Rect(rowRect.width - 17f, rowRect.yMin, 17f, this.plusButtonStyle.fixedHeight);
                GUI.Box(position, GUIContent.none, this.plusButtonBackgroundStyle);
                if (GUI.Button(position, GUIContent.none, this.plusButtonStyle))
                {
                    AddCurvesPopup.AddNewCurve(node2);
                    this.owner.Close();
                }
            }
        }

        protected override void RenameEnded()
        {
        }

        protected override void SyncFakeItem()
        {
        }

        public bool showPlusButton { get; set; }
    }
}

