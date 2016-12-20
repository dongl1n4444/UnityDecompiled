namespace UnityEditor.Events
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UI;

    [CustomPreview(typeof(GameObject))]
    internal class LayoutPropertiesPreview : ObjectPreview
    {
        [CompilerGenerated]
        private static Func<ILayoutElement, float> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<ILayoutElement, float> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<ILayoutElement, float> <>f__am$cache2;
        [CompilerGenerated]
        private static Func<ILayoutElement, float> <>f__am$cache3;
        [CompilerGenerated]
        private static Func<ILayoutElement, float> <>f__am$cache4;
        [CompilerGenerated]
        private static Func<ILayoutElement, float> <>f__am$cache5;
        private const float kLabelWidth = 110f;
        private const float kValueWidth = 100f;
        private Styles m_Styles = new Styles();
        private GUIContent m_Title;

        public override GUIContent GetPreviewTitle()
        {
            if (this.m_Title == null)
            {
                this.m_Title = new GUIContent("Layout Properties");
            }
            return this.m_Title;
        }

        public override bool HasPreviewGUI()
        {
            GameObject target = this.target as GameObject;
            if (target == null)
            {
                return false;
            }
            return (target.GetComponent(typeof(ILayoutElement)) != null);
        }

        public override void Initialize(UnityEngine.Object[] targets)
        {
            base.Initialize(targets);
        }

        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            if (Event.current.type == EventType.Repaint)
            {
                if (this.m_Styles == null)
                {
                    this.m_Styles = new Styles();
                }
                GameObject target = this.target as GameObject;
                RectTransform transform = target.transform as RectTransform;
                if (transform != null)
                {
                    r = new RectOffset(-5, -5, -5, -5).Add(r);
                    r.height = EditorGUIUtility.singleLineHeight;
                    Rect position = r;
                    Rect rect2 = r;
                    Rect rect3 = r;
                    position.width = 110f;
                    rect2.xMin += 110f;
                    rect2.width = 100f;
                    rect3.xMin += 210f;
                    GUI.Label(position, "Property", this.m_Styles.headerStyle);
                    GUI.Label(rect2, "Value", this.m_Styles.headerStyle);
                    GUI.Label(rect3, "Source", this.m_Styles.headerStyle);
                    position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                    rect2.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                    rect3.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                    ILayoutElement source = null;
                    if (<>f__am$cache0 == null)
                    {
                        <>f__am$cache0 = new Func<ILayoutElement, float>(null, (IntPtr) <OnPreviewGUI>m__0);
                    }
                    this.ShowProp(ref position, ref rect2, ref rect3, "Min Width", LayoutUtility.GetLayoutProperty(transform, <>f__am$cache0, 0f, out source).ToString(), source);
                    if (<>f__am$cache1 == null)
                    {
                        <>f__am$cache1 = new Func<ILayoutElement, float>(null, (IntPtr) <OnPreviewGUI>m__1);
                    }
                    this.ShowProp(ref position, ref rect2, ref rect3, "Min Height", LayoutUtility.GetLayoutProperty(transform, <>f__am$cache1, 0f, out source).ToString(), source);
                    if (<>f__am$cache2 == null)
                    {
                        <>f__am$cache2 = new Func<ILayoutElement, float>(null, (IntPtr) <OnPreviewGUI>m__2);
                    }
                    this.ShowProp(ref position, ref rect2, ref rect3, "Preferred Width", LayoutUtility.GetLayoutProperty(transform, <>f__am$cache2, 0f, out source).ToString(), source);
                    if (<>f__am$cache3 == null)
                    {
                        <>f__am$cache3 = new Func<ILayoutElement, float>(null, (IntPtr) <OnPreviewGUI>m__3);
                    }
                    this.ShowProp(ref position, ref rect2, ref rect3, "Preferred Height", LayoutUtility.GetLayoutProperty(transform, <>f__am$cache3, 0f, out source).ToString(), source);
                    float num5 = 0f;
                    if (<>f__am$cache4 == null)
                    {
                        <>f__am$cache4 = new Func<ILayoutElement, float>(null, (IntPtr) <OnPreviewGUI>m__4);
                    }
                    num5 = LayoutUtility.GetLayoutProperty(transform, <>f__am$cache4, 0f, out source);
                    this.ShowProp(ref position, ref rect2, ref rect3, "Flexible Width", (num5 <= 0f) ? "disabled" : ("enabled (" + num5.ToString() + ")"), source);
                    if (<>f__am$cache5 == null)
                    {
                        <>f__am$cache5 = new Func<ILayoutElement, float>(null, (IntPtr) <OnPreviewGUI>m__5);
                    }
                    num5 = LayoutUtility.GetLayoutProperty(transform, <>f__am$cache5, 0f, out source);
                    this.ShowProp(ref position, ref rect2, ref rect3, "Flexible Height", (num5 <= 0f) ? "disabled" : ("enabled (" + num5.ToString() + ")"), source);
                    if (transform.GetComponent<LayoutElement>() == null)
                    {
                        Rect rect4 = new Rect(position.x, position.y + 10f, r.width, EditorGUIUtility.singleLineHeight);
                        GUI.Label(rect4, "Add a LayoutElement to override values.", this.m_Styles.labelStyle);
                    }
                }
            }
        }

        private void ShowProp(ref Rect labelRect, ref Rect valueRect, ref Rect sourceRect, string label, string value, ILayoutElement source)
        {
            GUI.Label(labelRect, label, this.m_Styles.labelStyle);
            GUI.Label(valueRect, value, this.m_Styles.labelStyle);
            GUI.Label(sourceRect, (source != null) ? source.GetType().Name : "none", this.m_Styles.labelStyle);
            labelRect.y += EditorGUIUtility.singleLineHeight;
            valueRect.y += EditorGUIUtility.singleLineHeight;
            sourceRect.y += EditorGUIUtility.singleLineHeight;
        }

        private class Styles
        {
            public GUIStyle headerStyle = new GUIStyle(EditorStyles.boldLabel);
            public GUIStyle labelStyle = new GUIStyle(EditorStyles.label);

            public Styles()
            {
                Color color = new Color(0.7f, 0.7f, 0.7f);
                RectOffset padding = this.labelStyle.padding;
                padding.right += 4;
                this.labelStyle.normal.textColor = color;
                RectOffset offset2 = this.headerStyle.padding;
                offset2.right += 4;
                this.headerStyle.normal.textColor = color;
            }
        }
    }
}

