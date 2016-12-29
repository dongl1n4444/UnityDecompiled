namespace UnityEditorInternal
{
    using System;
    using System.Collections.Generic;
    using UnityEditor.IMGUI.Controls;

    [Serializable]
    internal class AnimationWindowHierarchyState : TreeViewState
    {
        private List<int> m_TallInstanceIDs = new List<int>();

        public void AddTallInstance(int id)
        {
            if (!this.m_TallInstanceIDs.Contains(id))
            {
                this.m_TallInstanceIDs.Add(id);
            }
        }

        public int GetTallInstancesCount() => 
            this.m_TallInstanceIDs.Count;

        public bool GetTallMode(AnimationWindowHierarchyNode node) => 
            this.m_TallInstanceIDs.Contains(node.id);

        public void SetTallMode(AnimationWindowHierarchyNode node, bool tallMode)
        {
            if (tallMode)
            {
                this.m_TallInstanceIDs.Add(node.id);
            }
            else
            {
                this.m_TallInstanceIDs.Remove(node.id);
            }
        }
    }
}

