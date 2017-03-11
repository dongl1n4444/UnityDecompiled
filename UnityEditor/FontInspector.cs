namespace UnityEditor
{
    using System;
    using UnityEngine;

    [CanEditMultipleObjects, CustomEditor(typeof(Font))]
    internal class FontInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            foreach (UnityEngine.Object obj2 in base.targets)
            {
                if (obj2.hideFlags == HideFlags.NotEditable)
                {
                    return;
                }
            }
            base.DrawDefaultInspector();
        }
    }
}

