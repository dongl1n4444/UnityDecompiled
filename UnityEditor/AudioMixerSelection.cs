namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditor.Audio;
    using UnityEngine;

    internal class AudioMixerSelection
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private List<AudioMixerGroupController> <ChannelStripSelection>k__BackingField;
        private AudioMixerController m_Controller;

        public AudioMixerSelection(AudioMixerController controller)
        {
            this.m_Controller = controller;
            this.ChannelStripSelection = new List<AudioMixerGroupController>();
            this.SyncToUnitySelection();
        }

        public void ClearChannelStrips()
        {
            Selection.objects = new UnityEngine.Object[0];
        }

        public bool HasSingleChannelStripSelection() => 
            (this.ChannelStripSelection.Count == 1);

        private void RefreshCachedChannelStripSelection()
        {
            UnityEngine.Object[] filtered = Selection.GetFiltered(typeof(AudioMixerGroupController), UnityEditor.SelectionMode.Deep);
            this.ChannelStripSelection = new List<AudioMixerGroupController>();
            List<AudioMixerGroupController> allAudioGroupsSlow = this.m_Controller.GetAllAudioGroupsSlow();
            foreach (AudioMixerGroupController controller in allAudioGroupsSlow)
            {
                if (filtered.Contains<UnityEngine.Object>(controller))
                {
                    this.ChannelStripSelection.Add(controller);
                }
            }
        }

        public void Sanitize()
        {
            this.RefreshCachedChannelStripSelection();
        }

        public void SetChannelStrips(List<AudioMixerGroupController> newSelection)
        {
            Selection.objects = newSelection.ToArray();
        }

        public void SetSingleChannelStrip(AudioMixerGroupController group)
        {
            AudioMixerGroupController[] controllerArray1 = new AudioMixerGroupController[] { group };
            Selection.objects = controllerArray1;
        }

        public void SyncToUnitySelection()
        {
            if (this.m_Controller != null)
            {
                this.RefreshCachedChannelStripSelection();
            }
        }

        public void ToggleChannelStrip(AudioMixerGroupController group)
        {
            List<UnityEngine.Object> list = new List<UnityEngine.Object>(Selection.objects);
            if (list.Contains(group))
            {
                list.Remove(group);
            }
            else
            {
                list.Add(group);
            }
            Selection.objects = list.ToArray();
        }

        public List<AudioMixerGroupController> ChannelStripSelection { get; private set; }
    }
}

