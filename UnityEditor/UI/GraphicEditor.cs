namespace UnityEditor.UI
{
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEditor.AnimatedValues;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    /// <summary>
    /// <para>Extend this class to write your own graphic editor.</para>
    /// </summary>
    [CustomEditor(typeof(MaskableGraphic), false), CanEditMultipleObjects]
    public class GraphicEditor : Editor
    {
        [CompilerGenerated]
        private static Func<UnityEngine.Object, Graphic> <>f__am$cache0;
        protected SerializedProperty m_Color;
        private GUIContent m_CorrectButtonContent;
        protected SerializedProperty m_Material;
        protected SerializedProperty m_RaycastTarget;
        protected SerializedProperty m_Script;
        protected AnimBool m_ShowNativeSize;

        /// <summary>
        /// <para>GUI related to the appearance of the graphic. Color and Material properties appear here.</para>
        /// </summary>
        protected void AppearanceControlsGUI()
        {
            EditorGUILayout.PropertyField(this.m_Color, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_Material, new GUILayoutOption[0]);
        }

        /// <summary>
        /// <para>GUI for showing a button that sets the size of the RectTransform to the native size for this Graphic.</para>
        /// </summary>
        protected void NativeSizeButtonGUI()
        {
            if (EditorGUILayout.BeginFadeGroup(this.m_ShowNativeSize.faded))
            {
                EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
                GUILayout.Space(EditorGUIUtility.labelWidth);
                if (GUILayout.Button(this.m_CorrectButtonContent, EditorStyles.miniButton, new GUILayoutOption[0]))
                {
                    if (<>f__am$cache0 == null)
                    {
                        <>f__am$cache0 = new Func<UnityEngine.Object, Graphic>(null, (IntPtr) <NativeSizeButtonGUI>m__0);
                    }
                    foreach (Graphic graphic in Enumerable.Select<UnityEngine.Object, Graphic>(base.targets, <>f__am$cache0))
                    {
                        Undo.RecordObject(graphic.rectTransform, "Set Native Size");
                        graphic.SetNativeSize();
                        EditorUtility.SetDirty(graphic);
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndFadeGroup();
        }

        /// <summary>
        /// <para>See MonoBehaviour.OnDisable.</para>
        /// </summary>
        protected virtual void OnDisable()
        {
            Tools.hidden = false;
            this.m_ShowNativeSize.valueChanged.RemoveListener(new UnityAction(this.Repaint));
        }

        protected virtual void OnEnable()
        {
            this.m_CorrectButtonContent = new GUIContent("Set Native Size", "Sets the size to match the content.");
            this.m_Script = base.serializedObject.FindProperty("m_Script");
            this.m_Color = base.serializedObject.FindProperty("m_Color");
            this.m_Material = base.serializedObject.FindProperty("m_Material");
            this.m_RaycastTarget = base.serializedObject.FindProperty("m_RaycastTarget");
            this.m_ShowNativeSize = new AnimBool(false);
            this.m_ShowNativeSize.valueChanged.AddListener(new UnityAction(this.Repaint));
        }

        /// <summary>
        /// <para>Implement specific GraphicEditor inspector GUI code here. If you want to simply extend the existing editor call the base OnInspectorGUI () before doing any custom GUI code.</para>
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.serializedObject.Update();
            EditorGUILayout.PropertyField(this.m_Script, new GUILayoutOption[0]);
            this.AppearanceControlsGUI();
            this.RaycastControlsGUI();
            base.serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// <para>GUI related to the Raycasting settings for the graphic.</para>
        /// </summary>
        protected void RaycastControlsGUI()
        {
            EditorGUILayout.PropertyField(this.m_RaycastTarget, new GUILayoutOption[0]);
        }

        /// <summary>
        /// <para>Set if the 'Set Native Size' button should be visible for this editor.</para>
        /// </summary>
        /// <param name="show"></param>
        /// <param name="instant"></param>
        protected void SetShowNativeSize(bool show, bool instant)
        {
            if (instant)
            {
                this.m_ShowNativeSize.value = show;
            }
            else
            {
                this.m_ShowNativeSize.target = show;
            }
        }
    }
}

