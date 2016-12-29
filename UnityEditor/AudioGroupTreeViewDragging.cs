namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditor.Audio;
    using UnityEditor.IMGUI.Controls;

    internal class AudioGroupTreeViewDragging : AssetsTreeViewDragging
    {
        [CompilerGenerated]
        private static Func<AudioMixerGroupController, int> <>f__am$cache0;
        private AudioMixerGroupTreeView m_owner;

        public AudioGroupTreeViewDragging(TreeViewController treeView, AudioMixerGroupTreeView owner) : base(treeView)
        {
            this.m_owner = owner;
        }

        public override DragAndDropVisualMode DoDrag(TreeViewItem parentNode, TreeViewItem targetNode, bool perform, TreeViewDragging.DropPosition dragPos)
        {
            AudioMixerTreeViewNode node = parentNode as AudioMixerTreeViewNode;
            List<AudioMixerGroupController> groupsToBeMoved = new List<Object>(DragAndDrop.objectReferences).OfType<AudioMixerGroupController>().ToList<AudioMixerGroupController>();
            if ((node != null) && (groupsToBeMoved.Count > 0))
            {
                if (<>f__am$cache0 == null)
                {
                    <>f__am$cache0 = new Func<AudioMixerGroupController, int>(null, (IntPtr) <DoDrag>m__0);
                }
                List<int> draggedInstanceIDs = Enumerable.Select<AudioMixerGroupController, int>(groupsToBeMoved, <>f__am$cache0).ToList<int>();
                bool flag = this.ValidDrag(parentNode, draggedInstanceIDs) && !AudioMixerController.WillModificationOfTopologyCauseFeedback(this.m_owner.Controller.GetAllAudioGroupsSlow(), groupsToBeMoved, node.group, null);
                if (perform && flag)
                {
                    AudioMixerGroupController group = node.group;
                    int insertionIndex = TreeViewDragging.GetInsertionIndex(parentNode, targetNode, dragPos);
                    this.m_owner.Controller.ReparentSelection(group, insertionIndex, groupsToBeMoved);
                    this.m_owner.ReloadTree();
                    base.m_TreeView.SetSelection(draggedInstanceIDs.ToArray(), true);
                }
                return (!flag ? DragAndDropVisualMode.Rejected : DragAndDropVisualMode.Move);
            }
            return DragAndDropVisualMode.None;
        }

        public override void StartDrag(TreeViewItem draggedItem, List<int> draggedItemIDs)
        {
            if (!EditorApplication.isPlaying)
            {
                base.StartDrag(draggedItem, draggedItemIDs);
            }
        }

        private bool ValidDrag(TreeViewItem parent, List<int> draggedInstanceIDs)
        {
            for (TreeViewItem item = parent; item != null; item = item.parent)
            {
                if (draggedInstanceIDs.Contains(item.id))
                {
                    return false;
                }
            }
            return true;
        }
    }
}

