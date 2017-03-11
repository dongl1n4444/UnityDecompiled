namespace UnityEditor
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using UnityEditor.Audio;
    using UnityEditor.IMGUI.Controls;

    internal class AudioMixerTreeViewNode : TreeViewItem
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private AudioMixerGroupController <group>k__BackingField;

        public AudioMixerTreeViewNode(int instanceID, int depth, TreeViewItem parent, string displayName, AudioMixerGroupController group) : base(instanceID, depth, parent, displayName)
        {
            this.group = group;
        }

        public AudioMixerGroupController group { get; set; }
    }
}

