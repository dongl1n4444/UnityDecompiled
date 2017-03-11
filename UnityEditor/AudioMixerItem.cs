namespace UnityEditor
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using UnityEditor.Audio;
    using UnityEditor.IMGUI.Controls;

    internal class AudioMixerItem : TreeViewItem
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private string <infoText>k__BackingField;
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private float <labelWidth>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool <lastSuspendedState>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private AudioMixerController <mixer>k__BackingField;
        private const string kSuspendedText = " - Inactive";

        public AudioMixerItem(int id, int depth, TreeViewItem parent, string displayName, AudioMixerController mixer, string infoText) : base(id, depth, parent, displayName)
        {
            this.mixer = mixer;
            this.infoText = infoText;
            this.UpdateSuspendedString(true);
        }

        private void AddSuspendedText()
        {
            if (this.infoText.IndexOf(" - Inactive", StringComparison.Ordinal) < 0)
            {
                this.infoText = this.infoText + " - Inactive";
            }
        }

        private void RemoveSuspendedText()
        {
            int index = this.infoText.IndexOf(" - Inactive", StringComparison.Ordinal);
            if (index >= 0)
            {
                this.infoText = this.infoText.Remove(index, " - Inactive".Length);
            }
        }

        public void UpdateSuspendedString(bool force)
        {
            bool isSuspended = this.mixer.isSuspended;
            if ((this.lastSuspendedState != isSuspended) || force)
            {
                this.lastSuspendedState = isSuspended;
                if (isSuspended)
                {
                    this.AddSuspendedText();
                }
                else
                {
                    this.RemoveSuspendedText();
                }
            }
        }

        public string infoText { get; set; }

        public float labelWidth { get; set; }

        private bool lastSuspendedState { get; set; }

        public AudioMixerController mixer { get; set; }
    }
}

