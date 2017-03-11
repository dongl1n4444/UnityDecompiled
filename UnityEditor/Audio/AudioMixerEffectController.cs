namespace UnityEditor.Audio
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEditor;
    using UnityEngine;

    internal sealed class AudioMixerEffectController : UnityEngine.Object
    {
        private string m_DisplayName;
        private int m_LastCachedGroupDisplayNameID;

        public AudioMixerEffectController(string name)
        {
            Internal_CreateAudioMixerEffectController(this, name);
        }

        public void ClearCachedDisplayName()
        {
            this.m_DisplayName = null;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern bool ContainsParameterGUID(GUID guid);
        public bool DisallowsBypass() => 
            (((this.IsSend() || this.IsReceive()) || this.IsDuckVolume()) || this.IsAttenuation());

        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern float GetCPUUsage(AudioMixerController controller);
        public string GetDisplayString(Dictionary<AudioMixerEffectController, AudioMixerGroupController> effectMap)
        {
            AudioMixerGroupController controller = effectMap[this];
            if ((controller.GetInstanceID() != this.m_LastCachedGroupDisplayNameID) || (this.m_DisplayName == null))
            {
                this.m_DisplayName = controller.GetDisplayString() + AudioMixerController.s_GroupEffectDisplaySeperator + AudioMixerController.FixNameForPopupMenu(this.effectName);
                this.m_LastCachedGroupDisplayNameID = controller.GetInstanceID();
            }
            return this.m_DisplayName;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern bool GetFloatBuffer(AudioMixerController controller, string name, out float[] data, int numsamples);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern GUID GetGUIDForMixLevel();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern GUID GetGUIDForParameter(string parameterName);
        public string GetSendTargetDisplayString(Dictionary<AudioMixerEffectController, AudioMixerGroupController> effectMap) => 
            ((this.sendTarget == null) ? string.Empty : this.sendTarget.GetDisplayString(effectMap));

        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern float GetValueForMixLevel(AudioMixerController controller, AudioMixerSnapshotController snapshot);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern float GetValueForParameter(AudioMixerController controller, AudioMixerSnapshotController snapshot, string parameterName);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void Internal_CreateAudioMixerEffectController(AudioMixerEffectController mono, string name);
        public bool IsAttenuation() => 
            (this.effectName == "Attenuation");

        public bool IsDuckVolume() => 
            (this.effectName == "Duck Volume");

        public bool IsReceive() => 
            (this.effectName == "Receive");

        public bool IsSend() => 
            (this.effectName == "Send");

        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void PreallocateGUIDs();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void SetValueForMixLevel(AudioMixerController controller, AudioMixerSnapshotController snapshot, float value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void SetValueForParameter(AudioMixerController controller, AudioMixerSnapshotController snapshot, string parameterName, float value);

        public bool bypass { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        public GUID effectID { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        public string effectName { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        public bool enableWetMix { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        public AudioMixerEffectController sendTarget { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
    }
}

