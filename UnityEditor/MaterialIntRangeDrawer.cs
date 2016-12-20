namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class MaterialIntRangeDrawer : MaterialPropertyDrawer
    {
        public override void OnGUI(Rect position, MaterialProperty prop, GUIContent label, MaterialEditor editor)
        {
            if (prop.type != MaterialProperty.PropType.Range)
            {
                GUIContent content = EditorGUIUtility.TempContent("IntRange used on a non-range property: " + prop.name, EditorGUIUtility.GetHelpIcon(MessageType.Warning));
                EditorGUI.LabelField(position, content, EditorStyles.helpBox);
            }
            else
            {
                MaterialEditor.DoIntRangeProperty(position, prop, label);
            }
        }
    }
}

