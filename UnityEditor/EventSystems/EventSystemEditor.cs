namespace UnityEditor.EventSystems
{
    using System;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.EventSystems;

    /// <summary>
    /// <para>Custom Editor for the EventSystem Component.</para>
    /// </summary>
    [CustomEditor(typeof(EventSystem), true)]
    public class EventSystemEditor : Editor
    {
        private GUIStyle m_PreviewLabelStyle;

        /// <summary>
        /// <para>Can this component be previewed in its current state?</para>
        /// </summary>
        /// <returns>
        /// <para>True if this component can be Previewed in its current state.</para>
        /// </returns>
        public override bool HasPreviewGUI()
        {
            return Application.isPlaying;
        }

        /// <summary>
        /// <para>See Editor.OnInspectorGUI.</para>
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.DrawDefaultInspector();
            EventSystem target = base.target as EventSystem;
            if (((target != null) && (target.GetComponent<BaseInputModule>() == null)) && GUILayout.Button("Add Default Input Modules", new GUILayoutOption[0]))
            {
                Undo.AddComponent<StandaloneInputModule>(target.gameObject);
            }
        }

        /// <summary>
        /// <para>Custom preview for Image component.</para>
        /// </summary>
        /// <param name="rect">Rectangle in which to draw the preview.</param>
        /// <param name="background">Background image.</param>
        public override void OnPreviewGUI(Rect rect, GUIStyle background)
        {
            EventSystem target = base.target as EventSystem;
            if (target != null)
            {
                GUI.Label(rect, target.ToString(), this.previewLabelStyle);
            }
        }

        /// <summary>
        /// <para>Does this edit require to be repainted constantly in its current state?</para>
        /// </summary>
        public override bool RequiresConstantRepaint()
        {
            return Application.isPlaying;
        }

        protected GUIStyle previewLabelStyle
        {
            get
            {
                if (this.m_PreviewLabelStyle == null)
                {
                    GUIStyle style = new GUIStyle("PreOverlayLabel") {
                        richText = true,
                        alignment = TextAnchor.UpperLeft,
                        fontStyle = FontStyle.Normal
                    };
                    this.m_PreviewLabelStyle = style;
                }
                return this.m_PreviewLabelStyle;
            }
        }
    }
}

