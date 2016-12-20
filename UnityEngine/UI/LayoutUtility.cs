namespace UnityEngine.UI
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    /// <summary>
    /// <para>Utility functions for querying layout elements for their minimum, preferred, and flexible sizes.</para>
    /// </summary>
    public static class LayoutUtility
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
        [CompilerGenerated]
        private static Func<ILayoutElement, float> <>f__am$cache6;
        [CompilerGenerated]
        private static Func<ILayoutElement, float> <>f__am$cache7;

        /// <summary>
        /// <para>Returns the flexible height of the layout element.</para>
        /// </summary>
        /// <param name="rect">The RectTransform of the layout element to query.</param>
        public static float GetFlexibleHeight(RectTransform rect)
        {
            if (<>f__am$cache7 == null)
            {
                <>f__am$cache7 = new Func<ILayoutElement, float>(null, (IntPtr) <GetFlexibleHeight>m__7);
            }
            return GetLayoutProperty(rect, <>f__am$cache7, 0f);
        }

        /// <summary>
        /// <para>Returns the flexible size of the layout element.</para>
        /// </summary>
        /// <param name="rect">The RectTransform of the layout element to query.</param>
        /// <param name="axis">The axis to query. This can be 0 or 1.</param>
        public static float GetFlexibleSize(RectTransform rect, int axis)
        {
            if (axis == 0)
            {
                return GetFlexibleWidth(rect);
            }
            return GetFlexibleHeight(rect);
        }

        /// <summary>
        /// <para>Returns the flexible width of the layout element.</para>
        /// </summary>
        /// <param name="rect">The RectTransform of the layout element to query.</param>
        public static float GetFlexibleWidth(RectTransform rect)
        {
            if (<>f__am$cache3 == null)
            {
                <>f__am$cache3 = new Func<ILayoutElement, float>(null, (IntPtr) <GetFlexibleWidth>m__3);
            }
            return GetLayoutProperty(rect, <>f__am$cache3, 0f);
        }

        public static float GetLayoutProperty(RectTransform rect, Func<ILayoutElement, float> property, float defaultValue)
        {
            ILayoutElement element;
            return GetLayoutProperty(rect, property, defaultValue, out element);
        }

        public static float GetLayoutProperty(RectTransform rect, Func<ILayoutElement, float> property, float defaultValue, out ILayoutElement source)
        {
            source = null;
            if (rect == null)
            {
                return 0f;
            }
            float num2 = defaultValue;
            int num3 = -2147483648;
            List<Component> results = ListPool<Component>.Get();
            rect.GetComponents(typeof(ILayoutElement), results);
            for (int i = 0; i < results.Count; i++)
            {
                ILayoutElement element = results[i] as ILayoutElement;
                if (!(element is Behaviour) || ((Behaviour) element).isActiveAndEnabled)
                {
                    int layoutPriority = element.layoutPriority;
                    if (layoutPriority >= num3)
                    {
                        float num6 = property.Invoke(element);
                        if (num6 >= 0f)
                        {
                            if (layoutPriority > num3)
                            {
                                num2 = num6;
                                num3 = layoutPriority;
                                source = element;
                            }
                            else if (num6 > num2)
                            {
                                num2 = num6;
                                source = element;
                            }
                        }
                    }
                }
            }
            ListPool<Component>.Release(results);
            return num2;
        }

        /// <summary>
        /// <para>Returns the minimum height of the layout element.</para>
        /// </summary>
        /// <param name="rect">The RectTransform of the layout element to query.</param>
        public static float GetMinHeight(RectTransform rect)
        {
            if (<>f__am$cache4 == null)
            {
                <>f__am$cache4 = new Func<ILayoutElement, float>(null, (IntPtr) <GetMinHeight>m__4);
            }
            return GetLayoutProperty(rect, <>f__am$cache4, 0f);
        }

        /// <summary>
        /// <para>Returns the minimum size of the layout element.</para>
        /// </summary>
        /// <param name="rect">The RectTransform of the layout element to query.</param>
        /// <param name="axis">The axis to query. This can be 0 or 1.</param>
        public static float GetMinSize(RectTransform rect, int axis)
        {
            if (axis == 0)
            {
                return GetMinWidth(rect);
            }
            return GetMinHeight(rect);
        }

        /// <summary>
        /// <para>Returns the minimum width of the layout element.</para>
        /// </summary>
        /// <param name="rect">The RectTransform of the layout element to query.</param>
        public static float GetMinWidth(RectTransform rect)
        {
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = new Func<ILayoutElement, float>(null, (IntPtr) <GetMinWidth>m__0);
            }
            return GetLayoutProperty(rect, <>f__am$cache0, 0f);
        }

        /// <summary>
        /// <para>Returns the preferred height of the layout element.</para>
        /// </summary>
        /// <param name="rect">The RectTransform of the layout element to query.</param>
        public static float GetPreferredHeight(RectTransform rect)
        {
            if (<>f__am$cache5 == null)
            {
                <>f__am$cache5 = new Func<ILayoutElement, float>(null, (IntPtr) <GetPreferredHeight>m__5);
            }
            if (<>f__am$cache6 == null)
            {
                <>f__am$cache6 = new Func<ILayoutElement, float>(null, (IntPtr) <GetPreferredHeight>m__6);
            }
            return Mathf.Max(GetLayoutProperty(rect, <>f__am$cache5, 0f), GetLayoutProperty(rect, <>f__am$cache6, 0f));
        }

        /// <summary>
        /// <para>Returns the preferred size of the layout element.</para>
        /// </summary>
        /// <param name="rect">The RectTransform of the layout element to query.</param>
        /// <param name="axis">The axis to query. This can be 0 or 1.</param>
        public static float GetPreferredSize(RectTransform rect, int axis)
        {
            if (axis == 0)
            {
                return GetPreferredWidth(rect);
            }
            return GetPreferredHeight(rect);
        }

        /// <summary>
        /// <para>Returns the preferred width of the layout element.</para>
        /// </summary>
        /// <param name="rect">The RectTransform of the layout element to query.</param>
        public static float GetPreferredWidth(RectTransform rect)
        {
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = new Func<ILayoutElement, float>(null, (IntPtr) <GetPreferredWidth>m__1);
            }
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = new Func<ILayoutElement, float>(null, (IntPtr) <GetPreferredWidth>m__2);
            }
            return Mathf.Max(GetLayoutProperty(rect, <>f__am$cache1, 0f), GetLayoutProperty(rect, <>f__am$cache2, 0f));
        }
    }
}

