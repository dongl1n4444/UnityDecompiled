namespace UnityEditor
{
    using System;
    using UnityEngine;

    [CanEditMultipleObjects, CustomEditor(typeof(ParticleRenderer))]
    internal class ParticleRendererEditor : RendererEditorBase
    {
        public override void OnEnable()
        {
            base.OnEnable();
            base.InitializeProbeFields();
        }

        public override void OnInspectorGUI()
        {
            base.serializedObject.Update();
            Editor.DrawPropertiesExcluding(base.serializedObject, RendererEditorBase.Probes.GetFieldsStringArray());
            base.m_Probes.OnGUI(base.targets, (Renderer) base.target, false);
            base.serializedObject.ApplyModifiedProperties();
        }
    }
}

