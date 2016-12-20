namespace UnityEditor
{
    using System;
    using UnityEngine;

    [CustomEditor(typeof(RenderSettings))]
    internal class RenderSettingsInspector : Editor
    {
        private Editor m_FogEditor;
        private Editor m_LightingEditor;
        private Editor m_OtherRenderingEditor;

        public virtual void OnEnable()
        {
            this.m_LightingEditor = null;
            this.m_FogEditor = null;
            this.m_OtherRenderingEditor = null;
        }

        public override void OnInspectorGUI()
        {
            this.lightingEditor.OnInspectorGUI();
            this.fogEditor.OnInspectorGUI();
            this.otherRenderingEditor.OnInspectorGUI();
        }

        private Editor fogEditor
        {
            get
            {
                Editor fogEditor;
                if (this.m_FogEditor != null)
                {
                    fogEditor = this.m_FogEditor;
                }
                else
                {
                    fogEditor = this.m_FogEditor = Editor.CreateEditor(base.target, typeof(FogEditor));
                }
                return fogEditor;
            }
        }

        private Editor lightingEditor
        {
            get
            {
                Editor lightingEditor;
                if (this.m_LightingEditor != null)
                {
                    lightingEditor = this.m_LightingEditor;
                }
                else
                {
                    lightingEditor = this.m_LightingEditor = Editor.CreateEditor(base.target, typeof(LightingEditor));
                }
                return lightingEditor;
            }
        }

        private Editor otherRenderingEditor
        {
            get
            {
                Editor otherRenderingEditor;
                if (this.m_OtherRenderingEditor != null)
                {
                    otherRenderingEditor = this.m_OtherRenderingEditor;
                }
                else
                {
                    otherRenderingEditor = this.m_OtherRenderingEditor = Editor.CreateEditor(base.target, typeof(OtherRenderingEditor));
                }
                return otherRenderingEditor;
            }
        }
    }
}

