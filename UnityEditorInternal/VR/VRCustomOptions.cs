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
        private SerializedProperty settings;

        protected VRCustomOptions()
        {
        }

        public abstract void Draw(Rect rect);
        internal SerializedProperty FindPropertyAssert(string name)
        {
            SerializedProperty property = null;
            if (this.settings == null)
            {
                Debug.LogError("No existing VR settings. Failed to find:" + name);
                return property;
            }
            property = this.settings.FindPropertyRelative(name);
            if (property == null)
            {
                Debug.LogError("Failed to find:" + name);
            }
            return property;
        }

        public abstract float GetHeight();
        public virtual void Initialize(SerializedProperty vrEditorSettings)
        {
            this.settings = vrEditorSettings;
        }

        public bool IsExpanded { get; set; }
    }
}

