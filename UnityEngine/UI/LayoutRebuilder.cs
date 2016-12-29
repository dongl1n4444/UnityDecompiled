namespace UnityEngine.UI
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Events;

    /// <summary>
    /// <para>Wrapper class for managing layout rebuilding of CanvasElement.</para>
    /// </summary>
    public class LayoutRebuilder : ICanvasElement
    {
        [CompilerGenerated]
        private static Predicate<Component> <>f__am$cache0;
        [CompilerGenerated]
        private static UnityAction<Component> <>f__am$cache1;
        [CompilerGenerated]
        private static UnityAction<Component> <>f__am$cache2;
        [CompilerGenerated]
        private static UnityAction<Component> <>f__am$cache3;
        [CompilerGenerated]
        private static UnityAction<Component> <>f__am$cache4;
        [CompilerGenerated]
        private static UnityEngine.RectTransform.ReapplyDrivenProperties <>f__mg$cache0;
        private int m_CachedHashFromTransform;
        private RectTransform m_ToRebuild;
        private static ObjectPool<LayoutRebuilder> s_Rebuilders = new ObjectPool<LayoutRebuilder>(null, new UnityAction<LayoutRebuilder>(LayoutRebuilder.<s_Rebuilders>m__0));

        static LayoutRebuilder()
        {
            if (<>f__mg$cache0 == null)
            {
                <>f__mg$cache0 = new UnityEngine.RectTransform.ReapplyDrivenProperties(LayoutRebuilder.ReapplyDrivenProperties);
            }
            RectTransform.reapplyDrivenProperties += <>f__mg$cache0;
        }

        [CompilerGenerated]
        private static void <s_Rebuilders>m__0(LayoutRebuilder x)
        {
            x.Clear();
        }

        private void Clear()
        {
            this.m_ToRebuild = null;
            this.m_CachedHashFromTransform = 0;
        }

        public override bool Equals(object obj) => 
            (obj.GetHashCode() == this.GetHashCode());

        /// <summary>
        /// <para>Forces an immediate rebuild of the layout element and child layout elements affected by the calculations.</para>
        /// </summary>
        /// <param name="layoutRoot">The layout element to perform the layout rebuild on.</param>
        public static void ForceRebuildLayoutImmediate(RectTransform layoutRoot)
        {
            LayoutRebuilder element = s_Rebuilders.Get();
            element.Initialize(layoutRoot);
            element.Rebuild(CanvasUpdate.Layout);
            s_Rebuilders.Release(element);
        }

        public override int GetHashCode() => 
            this.m_CachedHashFromTransform;

        /// <summary>
        /// <para>See ICanvasElement.GraphicUpdateComplete.</para>
        /// </summary>
        public void GraphicUpdateComplete()
        {
        }

        private void Initialize(RectTransform controller)
        {
            this.m_ToRebuild = controller;
            this.m_CachedHashFromTransform = controller.GetHashCode();
        }

        /// <summary>
        /// <para>Has the native representation of this LayoutRebuilder been destroyed?</para>
        /// </summary>
        public bool IsDestroyed() => 
            (this.m_ToRebuild == null);

        /// <summary>
        /// <para>See ICanvasElement.LayoutComplete.</para>
        /// </summary>
        public void LayoutComplete()
        {
            s_Rebuilders.Release(this);
        }

        /// <summary>
        /// <para>Mark the given RectTransform as needing it's layout to be recalculated during the next layout pass.</para>
        /// </summary>
        /// <param name="rect">Rect to rebuild.</param>
        public static void MarkLayoutForRebuild(RectTransform rect)
        {
            if (rect != null)
            {
                List<Component> comps = ListPool<Component>.Get();
                RectTransform layoutRoot = rect;
                while (true)
                {
                    RectTransform parent = layoutRoot.parent as RectTransform;
                    if (!ValidLayoutGroup(parent, comps))
                    {
                        break;
                    }
                    layoutRoot = parent;
                }
                if ((layoutRoot == rect) && !ValidController(layoutRoot, comps))
                {
                    ListPool<Component>.Release(comps);
                }
                else
                {
                    MarkLayoutRootForRebuild(layoutRoot);
                    ListPool<Component>.Release(comps);
                }
            }
        }

        private static void MarkLayoutRootForRebuild(RectTransform controller)
        {
            if (controller != null)
            {
                LayoutRebuilder element = s_Rebuilders.Get();
                element.Initialize(controller);
                if (!CanvasUpdateRegistry.TryRegisterCanvasElementForLayoutRebuild(element))
                {
                    s_Rebuilders.Release(element);
                }
            }
        }

        private void PerformLayoutCalculation(RectTransform rect, UnityAction<Component> action)
        {
            if (rect != null)
            {
                List<Component> results = ListPool<Component>.Get();
                rect.GetComponents(typeof(ILayoutElement), results);
                StripDisabledBehavioursFromList(results);
                if ((results.Count > 0) || (rect.GetComponent(typeof(ILayoutGroup)) != null))
                {
                    for (int i = 0; i < rect.childCount; i++)
                    {
                        this.PerformLayoutCalculation(rect.GetChild(i) as RectTransform, action);
                    }
                    for (int j = 0; j < results.Count; j++)
                    {
                        action(results[j]);
                    }
                }
                ListPool<Component>.Release(results);
            }
        }

        private void PerformLayoutControl(RectTransform rect, UnityAction<Component> action)
        {
            if (rect != null)
            {
                List<Component> results = ListPool<Component>.Get();
                rect.GetComponents(typeof(ILayoutController), results);
                StripDisabledBehavioursFromList(results);
                if (results.Count > 0)
                {
                    for (int i = 0; i < results.Count; i++)
                    {
                        if (results[i] is ILayoutSelfController)
                        {
                            action(results[i]);
                        }
                    }
                    for (int j = 0; j < results.Count; j++)
                    {
                        if (!(results[j] is ILayoutSelfController))
                        {
                            action(results[j]);
                        }
                    }
                    for (int k = 0; k < rect.childCount; k++)
                    {
                        this.PerformLayoutControl(rect.GetChild(k) as RectTransform, action);
                    }
                }
                ListPool<Component>.Release(results);
            }
        }

        private static void ReapplyDrivenProperties(RectTransform driven)
        {
            MarkLayoutForRebuild(driven);
        }

        /// <summary>
        /// <para>See ICanvasElement.Rebuild.</para>
        /// </summary>
        /// <param name="executing"></param>
        public void Rebuild(CanvasUpdate executing)
        {
            if (executing == CanvasUpdate.Layout)
            {
                if (<>f__am$cache1 == null)
                {
                    <>f__am$cache1 = e => (e as ILayoutElement).CalculateLayoutInputHorizontal();
                }
                this.PerformLayoutCalculation(this.m_ToRebuild, <>f__am$cache1);
                if (<>f__am$cache2 == null)
                {
                    <>f__am$cache2 = e => (e as ILayoutController).SetLayoutHorizontal();
                }
                this.PerformLayoutControl(this.m_ToRebuild, <>f__am$cache2);
                if (<>f__am$cache3 == null)
                {
                    <>f__am$cache3 = e => (e as ILayoutElement).CalculateLayoutInputVertical();
                }
                this.PerformLayoutCalculation(this.m_ToRebuild, <>f__am$cache3);
                if (<>f__am$cache4 == null)
                {
                    <>f__am$cache4 = e => (e as ILayoutController).SetLayoutVertical();
                }
                this.PerformLayoutControl(this.m_ToRebuild, <>f__am$cache4);
            }
        }

        private static void StripDisabledBehavioursFromList(List<Component> components)
        {
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = e => (e is Behaviour) && !((Behaviour) e).isActiveAndEnabled;
            }
            components.RemoveAll(<>f__am$cache0);
        }

        public override string ToString() => 
            ("(Layout Rebuilder for) " + this.m_ToRebuild);

        private static bool ValidController(RectTransform layoutRoot, List<Component> comps)
        {
            if (layoutRoot == null)
            {
                return false;
            }
            layoutRoot.GetComponents(typeof(ILayoutController), comps);
            StripDisabledBehavioursFromList(comps);
            return (comps.Count > 0);
        }

        private static bool ValidLayoutGroup(RectTransform parent, List<Component> comps)
        {
            if (parent == null)
            {
                return false;
            }
            parent.GetComponents(typeof(ILayoutGroup), comps);
            StripDisabledBehavioursFromList(comps);
            return (comps.Count > 0);
        }

        /// <summary>
        /// <para>See ICanvasElement.</para>
        /// </summary>
        public Transform transform =>
            this.m_ToRebuild;
    }
}

