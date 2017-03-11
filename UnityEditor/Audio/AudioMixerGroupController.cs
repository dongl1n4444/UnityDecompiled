namespace UnityEditor.Audio
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEngine.Audio;
    using UnityEngine.Scripting;

    internal sealed class AudioMixerGroupController : AudioMixerGroup
    {
        public AudioMixerGroupController(AudioMixer owner)
        {
            Internal_CreateAudioMixerGroupController(this, owner);
        }

        public void DumpHierarchy(string title, int level)
        {
            if (title != "")
            {
                Console.WriteLine(title);
            }
            string str = "";
            int num = level;
            while (num-- > 0)
            {
                str = str + "  ";
            }
            Console.WriteLine(str + "name=" + base.name);
            str = str + "  ";
            foreach (AudioMixerEffectController controller in this.effects)
            {
                Console.WriteLine(str + "effect=" + controller.ToString());
            }
            foreach (AudioMixerGroupController controller2 in this.children)
            {
                controller2.DumpHierarchy("", level + 1);
            }
        }

        public string GetDisplayString() => 
            base.name;

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern GUID GetGUIDForPitch();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern GUID GetGUIDForVolume();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern float GetValueForPitch(AudioMixerController controller, AudioMixerSnapshotController snapshot);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern float GetValueForVolume(AudioMixerController controller, AudioMixerSnapshotController snapshot);
        public bool HasAttenuation()
        {
            foreach (AudioMixerEffectController controller in this.effects)
            {
                if (controller.IsAttenuation())
                {
                    return true;
                }
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern bool HasDependentMixers();
        public void InsertEffect(AudioMixerEffectController effect, int index)
        {
            List<AudioMixerEffectController> list = new List<AudioMixerEffectController>(this.effects) { null };
            for (int i = list.Count - 1; i > index; i--)
            {
                list[i] = list[i - 1];
            }
            list[index] = effect;
            this.effects = list.ToArray();
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void Internal_CreateAudioMixerGroupController(AudioMixerGroupController mono, AudioMixer owner);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void PreallocateGUIDs();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void SetValueForPitch(AudioMixerController controller, AudioMixerSnapshotController snapshot, float value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void SetValueForVolume(AudioMixerController controller, AudioMixerSnapshotController snapshot, float value);
        public override string ToString() => 
            base.name;

        public bool bypassEffects { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        public AudioMixerGroupController[] children { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        public AudioMixerController controller { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        public AudioMixerEffectController[] effects { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        public GUID groupID { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        public bool mute { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        public bool solo { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        public int userColorIndex { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }
    }
}

