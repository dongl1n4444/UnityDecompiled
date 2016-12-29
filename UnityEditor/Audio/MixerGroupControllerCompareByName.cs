namespace UnityEditor.Audio
{
    using System;
    using System.Collections.Generic;

    internal class MixerGroupControllerCompareByName : IComparer<AudioMixerGroupController>
    {
        public int Compare(AudioMixerGroupController x, AudioMixerGroupController y) => 
            StringComparer.InvariantCultureIgnoreCase.Compare(x.GetDisplayString(), y.GetDisplayString());
    }
}

