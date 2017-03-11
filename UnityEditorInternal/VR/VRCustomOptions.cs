namespace UnityEditorInternal.VR
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEngine;

    internal abstract class VRCustomOptions
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private bool <IsExpanded>k__BackingField;
        private SerializedProperty editorSettings;
        private SerializedProperty playerSettings;

        protected VRCustomOptions()
        {
        }

        public abstract Rect Draw(Rect rect);
        internal SerializedProperty FindPropertyAssert(string name)
        {
            SerializedProperty property = null;
            if ((this.editorSettings == null) && (this.playerSettings == null))
            {
                UnityEngine.Debug.LogError("No existing VR settings. Failed to find:" + name);
                return property;
            }
            bool flag = false;
            if (this.editorSettings != null)
            {
                property = this.editorSettings.FindPropertyRelative(name);
                if (property != null)
                {
                    flag = true;
                }
            }
            if (!flag && (this.playerSettings != null))
            {
                property = this.playerSettings.FindPropertyRelative(name);
                if (property != null)
                {
                    flag = true;
                }
            }
            if (!flag)
            {
                UnityEngine.Debug.LogError("Failed to find property:" + name);
            }
            return property;
        }

        public abstract float GetHeight();
        public virtual void Initialize(SerializedObject settings)
        {
            this.Initialize(settings, "");
        }

        public virtual void Initialize(SerializedObject settings, string propertyName)
        {
            this.editorSettings = settings.FindProperty("vrEditorSettings");
            if ((this.editorSettings != null) && !string.IsNullOrEmpty(propertyName))
            {
                this.editorSettings = this.editorSettings.FindPropertyRelative(propertyName);
            }
            this.playerSettings = settings.FindProperty("vrSettings");
            if ((this.playerSettings != null) && !string.IsNullOrEmpty(propertyName))
            {
                this.playerSettings = this.playerSettings.FindPropertyRelative(propertyName);
            }
        }

        public bool IsExpanded { get; set; }
    }
}

