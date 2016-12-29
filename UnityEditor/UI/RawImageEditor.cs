namespace UnityEditor.UI
{
    using System;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// <para>Custom editor for RawImage.</para>
    /// </summary>
    [CustomEditor(typeof(RawImage), true), CanEditMultipleObjects]
    public class RawImageEditor : GraphicEditor
    {
        private SerializedProperty m_Texture;
        private SerializedProperty m_UVRect;
        private GUIContent m_UVRectContent;

        /// <summary>
        /// <para>A string cointaining the Image details to be used as a overlay on the component Preview.</para>
        /// </summary>
        /// <returns>
        /// <para>The RawImage details.</para>
        /// </returns>
        public override string GetInfoString()
        {
            RawImage target = base.target as RawImage;
            return $"RawImage Size: {Mathf.RoundToInt(Mathf.Abs(target.rectTransform.rect.width))}x{Mathf.RoundToInt(Mathf.Abs(target.rectTransform.rect.height))}";
        }

        /// <summary>
        /// <para>Can this component be Previewed in its current state?</para>
        /// </summary>
        /// <returns>
        /// <para>True if this component can be Previewed in its current state.</para>
        /// </returns>
        public override bool HasPreviewGUI()
        {
            RawImage target = base.target as RawImage;
            if (target == null)
            {
                return false;
            }
            Rect rect = Outer(target);
            return ((rect.width > 0f) && (rect.height > 0f));
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            this.m_UVRectContent = new GUIContent("UV Rect");
            this.m_Texture = base.serializedObject.FindProperty("m_Texture");
            this.m_UVRect = base.serializedObject.FindProperty("m_UVRect");
            this.SetShowNativeSize(true);
        }

        /// <summary>
        /// <para>Implement specific RawImage inspector GUI code here. If you want to simply extend the existing editor call the base OnInspectorGUI () before doing any custom GUI code.</para>
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.serializedObject.Update();
            EditorGUILayout.PropertyField(this.m_Texture, new GUILayoutOption[0]);
            base.AppearanceControlsGUI();
            base.RaycastControlsGUI();
            EditorGUILayout.PropertyField(this.m_UVRect, this.m_UVRectContent, new GUILayoutOption[0]);
            this.SetShowNativeSize(false);
            base.NativeSizeButtonGUI();
            base.serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// <para>Custom preview for Image component.</para>
        /// </summary>
        /// <param name="rect">Rectangle in which to draw the preview.</param>
        /// <param name="background">Background image.</param>
        public override void OnPreviewGUI(Rect rect, GUIStyle background)
        {
            RawImage target = base.target as RawImage;
            Texture mainTexture = target.mainTexture;
            if (mainTexture != null)
            {
                Rect outer = Outer(target);
                SpriteDrawUtility.DrawSprite(mainTexture, rect, outer, target.uvRect, target.canvasRenderer.GetColor());
            }
        }

        private static Rect Outer(RawImage rawImage)
        {
            Rect uvRect = rawImage.uvRect;
            uvRect.xMin *= rawImage.rectTransform.rect.width;
            uvRect.xMax *= rawImage.rectTransform.rect.width;
            uvRect.yMin *= rawImage.rectTransform.rect.height;
            uvRect.yMax *= rawImage.rectTransform.rect.height;
            return uvRect;
        }

        private void SetShowNativeSize(bool instant)
        {
            base.SetShowNativeSize(this.m_Texture.objectReferenceValue != null, instant);
        }
    }
}

