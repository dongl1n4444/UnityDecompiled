namespace UnityEditorInternal
{
    using System;
    using UnityEditor;
    using UnityEditor.IMGUI.Controls;

    internal class AnimationWindowHierarchyPropertyNode : AnimationWindowHierarchyNode
    {
        public bool isPptrNode;

        public AnimationWindowHierarchyPropertyNode(System.Type animatableObjectType, int setId, string propertyName, string path, TreeViewItem parent, EditorCurveBinding binding, bool isPptrNode) : base(AnimationWindowUtility.GetPropertyNodeID(setId, path, animatableObjectType, propertyName), (parent == null) ? -1 : (parent.depth + 1), parent, animatableObjectType, propertyName, path, AnimationWindowUtility.GetNicePropertyDisplayName(animatableObjectType, propertyName))
        {
            base.binding = new EditorCurveBinding?(binding);
            this.isPptrNode = isPptrNode;
        }
    }
}

