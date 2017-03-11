namespace UnityEditor
{
    using System;
    using UnityEngine;
    using UnityEngine.Audio;

    [CustomEditor(typeof(AudioMixer)), CanEditMultipleObjects]
    internal class AudioMixerInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            GUILayout.Space(10f);
            EditorGUILayout.HelpBox("Modification and inspection of built AudioMixer assets is disabled. Please modify the source asset and re-build.", MessageType.Info);
        }
    }
}

