namespace UnityEditor.UI
{
    using System;
    using UnityEditor;
    using UnityEngine.UI;

    /// <summary>
    /// <para>Custom editor for the RectMask2d component.</para>
    /// </summary>
    [CustomEditor(typeof(RectMask2D), true), CanEditMultipleObjects]
    public class RectMask2DEditor : Editor
    {
        public override void OnInspectorGUI()
        {
        }
    }
}

