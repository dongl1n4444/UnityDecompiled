namespace UnityEditor
{
    using System;
    using System.Linq;
    using UnityEditor.IMGUI.Controls;
    using UnityEngine;

    internal class AudioMixersTreeViewGUI : TreeViewGUI
    {
        public AudioMixersTreeViewGUI(TreeViewController treeView) : base(treeView)
        {
            base.k_IconWidth = 0f;
            base.k_TopRowMargin = base.k_BottomRowMargin = 2f;
        }

        public void BeginCreateNewMixer()
        {
            this.ClearRenameAndNewItemState();
            string newAssetResourceFile = string.Empty;
            AudioMixerItem selectedItem = this.GetSelectedItem();
            if ((selectedItem != null) && (selectedItem.mixer.outputAudioMixerGroup != null))
            {
                newAssetResourceFile = selectedItem.mixer.outputAudioMixerGroup.GetInstanceID().ToString();
            }
            int instanceID = 0;
            if (this.GetCreateAssetUtility().BeginNewAssetCreation(instanceID, ScriptableObject.CreateInstance<DoCreateAudioMixer>(), "NewAudioMixer.mixer", null, newAssetResourceFile))
            {
                this.SyncFakeItem();
                if (!base.GetRenameOverlay().BeginRename(this.GetCreateAssetUtility().originalName, instanceID, 0f))
                {
                    Debug.LogError("Rename not started (when creating new asset)");
                }
            }
        }

        protected override void ClearRenameAndNewItemState()
        {
            this.GetCreateAssetUtility().Clear();
            base.ClearRenameAndNewItemState();
        }

        protected CreateAssetUtility GetCreateAssetUtility()
        {
            return base.m_TreeView.state.createAssetUtility;
        }

        protected override Texture GetIconForItem(TreeViewItem node)
        {
            return null;
        }

        private AudioMixerItem GetSelectedItem()
        {
            return (base.m_TreeView.FindItem(Enumerable.FirstOrDefault<int>(base.m_TreeView.GetSelection())) as AudioMixerItem);
        }

        protected override void OnContentGUI(Rect rect, int row, TreeViewItem item, string label, bool selected, bool focused, bool useBoldFont, bool isPinging)
        {
            if (Event.current.type == EventType.Repaint)
            {
                if (!isPinging)
                {
                    float contentIndent = this.GetContentIndent(item);
                    rect.x += contentIndent;
                    rect.width -= contentIndent;
                }
                AudioMixerItem item2 = item as AudioMixerItem;
                if (item2 != null)
                {
                    GUIStyle style = !useBoldFont ? TreeViewGUI.s_Styles.lineStyle : TreeViewGUI.s_Styles.lineBoldStyle;
                    style.padding.left = (int) ((base.k_IconWidth + base.iconTotalPadding) + base.k_SpaceBetweenIconAndText);
                    style.Draw(rect, label, false, false, selected, focused);
                    item2.UpdateSuspendedString(false);
                    if (item2.labelWidth <= 0f)
                    {
                        item2.labelWidth = style.CalcSize(GUIContent.Temp(label)).x;
                    }
                    Rect position = rect;
                    position.x += item2.labelWidth + 8f;
                    using (new EditorGUI.DisabledScope(true))
                    {
                        style.Draw(position, item2.infoText, false, false, false, false);
                    }
                    if (base.iconOverlayGUI != null)
                    {
                        Rect rect3 = rect;
                        rect3.width = base.k_IconWidth + base.iconTotalPadding;
                        base.iconOverlayGUI.Invoke(item, rect3);
                    }
                }
            }
        }

        protected override void RenameEnded()
        {
            string name = !string.IsNullOrEmpty(base.GetRenameOverlay().name) ? base.GetRenameOverlay().name : base.GetRenameOverlay().originalName;
            int userData = base.GetRenameOverlay().userData;
            bool flag = this.GetCreateAssetUtility().IsCreatingNewAsset();
            if (base.GetRenameOverlay().userAcceptedRename)
            {
                if (flag)
                {
                    this.GetCreateAssetUtility().EndNewAssetCreation(name);
                    base.m_TreeView.ReloadData();
                }
                else
                {
                    ObjectNames.SetNameSmartWithInstanceID(userData, name);
                }
            }
        }

        protected override void SyncFakeItem()
        {
            if (!base.m_TreeView.data.HasFakeItem() && this.GetCreateAssetUtility().IsCreatingNewAsset())
            {
                int id = base.m_TreeView.data.root.id;
                AudioMixerItem selectedItem = this.GetSelectedItem();
                if (selectedItem != null)
                {
                    id = selectedItem.parent.id;
                }
                base.m_TreeView.data.InsertFakeItem(this.GetCreateAssetUtility().instanceID, id, this.GetCreateAssetUtility().originalName, this.GetCreateAssetUtility().icon);
            }
            if (base.m_TreeView.data.HasFakeItem() && !this.GetCreateAssetUtility().IsCreatingNewAsset())
            {
                base.m_TreeView.data.RemoveFakeItem();
            }
        }
    }
}

