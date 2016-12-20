namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEditor.Audio;
    using UnityEditor.IMGUI.Controls;
    using UnityEngine;

    internal class AudioGroupTreeViewGUI : TreeViewGUI
    {
        private readonly float column1Width;
        private readonly Texture2D k_VisibleON;
        public AudioMixerController m_Controller;
        public Action<AudioMixerTreeViewNode, bool> NodeWasToggled;

        public AudioGroupTreeViewGUI(TreeViewController treeView) : base(treeView)
        {
            this.column1Width = 20f;
            this.k_VisibleON = EditorGUIUtility.FindTexture("VisibilityOn");
            this.m_Controller = null;
            base.k_BaseIndent = this.column1Width;
            base.k_IconWidth = 0f;
            base.k_TopRowMargin = base.k_BottomRowMargin = 2f;
        }

        protected override Texture GetIconForItem(TreeViewItem node)
        {
            if ((node != null) && (node.icon != null))
            {
                return node.icon;
            }
            return null;
        }

        public override void OnRowGUI(Rect rowRect, TreeViewItem node, int row, bool selected, bool focused)
        {
            Event current = Event.current;
            this.DoItemGUI(rowRect, row, node, selected, focused, false);
            if (this.m_Controller != null)
            {
                AudioMixerTreeViewNode audioNode = node as AudioMixerTreeViewNode;
                if (audioNode != null)
                {
                    bool visible = this.m_Controller.CurrentViewContainsGroup(audioNode.group.groupID);
                    float num = 3f;
                    Rect position = new Rect(rowRect.x + num, rowRect.y, 16f, 16f);
                    Rect rect = new Rect(position.x + 1f, position.y + 1f, position.width - 2f, position.height - 2f);
                    int userColorIndex = audioNode.group.userColorIndex;
                    if (userColorIndex > 0)
                    {
                        EditorGUI.DrawRect(new Rect(rowRect.x, rect.y, 2f, rect.height), AudioMixerColorCodes.GetColor(userColorIndex));
                    }
                    EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 0.2f));
                    if (visible)
                    {
                        GUI.DrawTexture(position, this.k_VisibleON);
                    }
                    Rect rect3 = new Rect(2f, rowRect.y, rowRect.height, rowRect.height);
                    if ((((current.type == EventType.MouseUp) && (current.button == 0)) && rect3.Contains(current.mousePosition)) && (this.NodeWasToggled != null))
                    {
                        this.NodeWasToggled.Invoke(audioNode, !visible);
                    }
                    if ((current.type == EventType.ContextClick) && position.Contains(current.mousePosition))
                    {
                        this.OpenGroupContextMenu(audioNode, visible);
                        current.Use();
                    }
                }
            }
        }

        private void OpenGroupContextMenu(AudioMixerTreeViewNode audioNode, bool visible)
        {
            AudioMixerGroupController[] controllerArray;
            <OpenGroupContextMenu>c__AnonStorey0 storey = new <OpenGroupContextMenu>c__AnonStorey0 {
                audioNode = audioNode,
                visible = visible,
                $this = this
            };
            GenericMenu menu = new GenericMenu();
            if (this.NodeWasToggled != null)
            {
                menu.AddItem(new GUIContent(!storey.visible ? "Show Group" : "Hide group"), false, new GenericMenu.MenuFunction(storey.<>m__0));
            }
            menu.AddSeparator(string.Empty);
            if (this.m_Controller.CachedSelection.Contains(storey.audioNode.group))
            {
                controllerArray = this.m_Controller.CachedSelection.ToArray();
            }
            else
            {
                controllerArray = new AudioMixerGroupController[] { storey.audioNode.group };
            }
            AudioMixerColorCodes.AddColorItemsToGenericMenu(menu, controllerArray);
            menu.ShowAsContext();
        }

        protected override void RenameEnded()
        {
            if (base.GetRenameOverlay().userAcceptedRename)
            {
                string name = !string.IsNullOrEmpty(base.GetRenameOverlay().name) ? base.GetRenameOverlay().name : base.GetRenameOverlay().originalName;
                int userData = base.GetRenameOverlay().userData;
                AudioMixerTreeViewNode node = base.m_TreeView.FindItem(userData) as AudioMixerTreeViewNode;
                if (node != null)
                {
                    ObjectNames.SetNameSmartWithInstanceID(userData, name);
                    foreach (AudioMixerEffectController controller in node.group.effects)
                    {
                        controller.ClearCachedDisplayName();
                    }
                    base.m_TreeView.ReloadData();
                    if (this.m_Controller != null)
                    {
                        this.m_Controller.OnSubAssetChanged();
                    }
                }
            }
        }

        protected override void SyncFakeItem()
        {
        }

        [CompilerGenerated]
        private sealed class <OpenGroupContextMenu>c__AnonStorey0
        {
            internal AudioGroupTreeViewGUI $this;
            internal AudioMixerTreeViewNode audioNode;
            internal bool visible;

            internal void <>m__0()
            {
                this.$this.NodeWasToggled.Invoke(this.audioNode, !this.visible);
            }
        }
    }
}

