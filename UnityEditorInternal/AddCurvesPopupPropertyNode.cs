namespace UnityEditorInternal
{
    using System;
    using UnityEditor;
    using UnityEditor.IMGUI.Controls;

    internal class AddCurvesPopupPropertyNode : TreeViewItem
    {
        public EditorCurveBinding[] curveBindings;
        public AnimationWindowSelectionItem selectionItem;

        public AddCurvesPopupPropertyNode(TreeViewItem parent, AnimationWindowSelectionItem selectionItem, EditorCurveBinding[] curveBindings) : base(curveBindings[0].GetHashCode(), parent.depth + 1, parent, AnimationWindowUtility.NicifyPropertyGroupName(curveBindings[0].type, AnimationWindowUtility.GetPropertyGroupName(curveBindings[0].propertyName)))
        {
            this.selectionItem = selectionItem;
            this.curveBindings = curveBindings;
        }

        public override int CompareTo(TreeViewItem other)
        {
            AddCurvesPopupPropertyNode node = other as AddCurvesPopupPropertyNode;
            if (node != null)
            {
                if (this.displayName.Contains("Rotation") && node.displayName.Contains("Position"))
                {
                    return 1;
                }
                if (this.displayName.Contains("Position") && node.displayName.Contains("Rotation"))
                {
                    return -1;
                }
            }
            return base.CompareTo(other);
        }
    }
}

