namespace UnityEditor
{
    using System;
    using UnityEngine;

    [CustomEditor(typeof(DoubleCurvePresetLibrary))]
    internal class DoubleCurvePresetLibraryEditor : Editor
    {
        private GenericPresetLibraryInspector<DoubleCurvePresetLibrary> m_GenericPresetLibraryInspector;

        private string GetHeader() => 
            "Particle Curve Preset Library";

        public void OnDestroy()
        {
            if (this.m_GenericPresetLibraryInspector != null)
            {
                this.m_GenericPresetLibraryInspector.OnDestroy();
            }
        }

        public void OnEnable()
        {
            this.m_GenericPresetLibraryInspector = new GenericPresetLibraryInspector<DoubleCurvePresetLibrary>(base.target, this.GetHeader(), null);
            this.m_GenericPresetLibraryInspector.presetSize = new Vector2(72f, 20f);
            this.m_GenericPresetLibraryInspector.lineSpacing = 5f;
        }

        public override void OnInspectorGUI()
        {
            if (this.m_GenericPresetLibraryInspector != null)
            {
                this.m_GenericPresetLibraryInspector.OnInspectorGUI();
            }
        }
    }
}

