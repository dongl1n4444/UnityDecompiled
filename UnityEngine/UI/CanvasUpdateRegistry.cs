namespace UnityEngine.UI
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.UI.Collections;

    /// <summary>
    /// <para>A place where CanvasElements can register themselves for rebuilding.</para>
    /// </summary>
    public class CanvasUpdateRegistry
    {
        [CompilerGenerated]
        private static Comparison<ICanvasElement> <>f__mg$cache0;
        private readonly IndexedSet<ICanvasElement> m_GraphicRebuildQueue = new IndexedSet<ICanvasElement>();
        private readonly IndexedSet<ICanvasElement> m_LayoutRebuildQueue = new IndexedSet<ICanvasElement>();
        private bool m_PerformingGraphicUpdate;
        private bool m_PerformingLayoutUpdate;
        private static CanvasUpdateRegistry s_Instance;
        private static readonly Comparison<ICanvasElement> s_SortLayoutFunction;

        static CanvasUpdateRegistry()
        {
            if (<>f__mg$cache0 == null)
            {
                <>f__mg$cache0 = new Comparison<ICanvasElement>(CanvasUpdateRegistry.SortLayoutList);
            }
            s_SortLayoutFunction = <>f__mg$cache0;
        }

        protected CanvasUpdateRegistry()
        {
            Canvas.willRenderCanvases += new Canvas.WillRenderCanvases(this.PerformUpdate);
        }

        private void CleanInvalidItems()
        {
            for (int i = this.m_LayoutRebuildQueue.Count - 1; i >= 0; i--)
            {
                ICanvasElement element = this.m_LayoutRebuildQueue[i];
                if (element == null)
                {
                    this.m_LayoutRebuildQueue.RemoveAt(i);
                }
                else if (element.IsDestroyed())
                {
                    this.m_LayoutRebuildQueue.RemoveAt(i);
                    element.LayoutComplete();
                }
            }
            for (int j = this.m_GraphicRebuildQueue.Count - 1; j >= 0; j--)
            {
                ICanvasElement element2 = this.m_GraphicRebuildQueue[j];
                if (element2 == null)
                {
                    this.m_GraphicRebuildQueue.RemoveAt(j);
                }
                else if (element2.IsDestroyed())
                {
                    this.m_GraphicRebuildQueue.RemoveAt(j);
                    element2.GraphicUpdateComplete();
                }
            }
        }

        private bool InternalRegisterCanvasElementForGraphicRebuild(ICanvasElement element)
        {
            if (this.m_PerformingGraphicUpdate)
            {
                Debug.LogError(string.Format("Trying to add {0} for graphic rebuild while we are already inside a graphic rebuild loop. This is not supported.", element));
                return false;
            }
            return this.m_GraphicRebuildQueue.AddUnique(element);
        }

        private bool InternalRegisterCanvasElementForLayoutRebuild(ICanvasElement element)
        {
            if (this.m_LayoutRebuildQueue.Contains(element))
            {
                return false;
            }
            return this.m_LayoutRebuildQueue.AddUnique(element);
        }

        private void InternalUnRegisterCanvasElementForGraphicRebuild(ICanvasElement element)
        {
            if (this.m_PerformingGraphicUpdate)
            {
                Debug.LogError(string.Format("Trying to remove {0} from rebuild list while we are already inside a rebuild loop. This is not supported.", element));
            }
            else
            {
                element.GraphicUpdateComplete();
                instance.m_GraphicRebuildQueue.Remove(element);
            }
        }

        private void InternalUnRegisterCanvasElementForLayoutRebuild(ICanvasElement element)
        {
            if (this.m_PerformingLayoutUpdate)
            {
                Debug.LogError(string.Format("Trying to remove {0} from rebuild list while we are already inside a rebuild loop. This is not supported.", element));
            }
            else
            {
                element.LayoutComplete();
                instance.m_LayoutRebuildQueue.Remove(element);
            }
        }

        /// <summary>
        /// <para>Are graphics being rebuild.</para>
        /// </summary>
        /// <returns>
        /// <para>Rebuilding graphics.</para>
        /// </returns>
        public static bool IsRebuildingGraphics()
        {
            return instance.m_PerformingGraphicUpdate;
        }

        /// <summary>
        /// <para>Is layout being rebuilt?</para>
        /// </summary>
        /// <returns>
        /// <para>Rebuilding layout.</para>
        /// </returns>
        public static bool IsRebuildingLayout()
        {
            return instance.m_PerformingLayoutUpdate;
        }

        private bool ObjectValidForUpdate(ICanvasElement element)
        {
            bool flag = element != null;
            if (element is UnityEngine.Object)
            {
                flag = element is UnityEngine.Object;
            }
            return flag;
        }

        private static int ParentCount(Transform child)
        {
            if (child == null)
            {
                return 0;
            }
            Transform parent = child.parent;
            int num2 = 0;
            while (parent != null)
            {
                num2++;
                parent = parent.parent;
            }
            return num2;
        }

        private void PerformUpdate()
        {
            this.CleanInvalidItems();
            this.m_PerformingLayoutUpdate = true;
            this.m_LayoutRebuildQueue.Sort(s_SortLayoutFunction);
            for (int i = 0; i <= 2; i++)
            {
                for (int n = 0; n < this.m_LayoutRebuildQueue.Count; n++)
                {
                    ICanvasElement element = instance.m_LayoutRebuildQueue[n];
                    try
                    {
                        if (this.ObjectValidForUpdate(element))
                        {
                            element.Rebuild((CanvasUpdate) i);
                        }
                    }
                    catch (Exception exception)
                    {
                        Debug.LogException(exception, element.transform);
                    }
                }
            }
            for (int j = 0; j < this.m_LayoutRebuildQueue.Count; j++)
            {
                this.m_LayoutRebuildQueue[j].LayoutComplete();
            }
            instance.m_LayoutRebuildQueue.Clear();
            this.m_PerformingLayoutUpdate = false;
            ClipperRegistry.instance.Cull();
            this.m_PerformingGraphicUpdate = true;
            for (int k = 3; k < 5; k++)
            {
                for (int num5 = 0; num5 < instance.m_GraphicRebuildQueue.Count; num5++)
                {
                    try
                    {
                        ICanvasElement element2 = instance.m_GraphicRebuildQueue[num5];
                        if (this.ObjectValidForUpdate(element2))
                        {
                            element2.Rebuild((CanvasUpdate) k);
                        }
                    }
                    catch (Exception exception2)
                    {
                        Debug.LogException(exception2, instance.m_GraphicRebuildQueue[num5].transform);
                    }
                }
            }
            for (int m = 0; m < this.m_GraphicRebuildQueue.Count; m++)
            {
                this.m_GraphicRebuildQueue[m].GraphicUpdateComplete();
            }
            instance.m_GraphicRebuildQueue.Clear();
            this.m_PerformingGraphicUpdate = false;
        }

        /// <summary>
        /// <para>Rebuild the graphics of the given element.</para>
        /// </summary>
        /// <param name="element">Element to rebuild.</param>
        public static void RegisterCanvasElementForGraphicRebuild(ICanvasElement element)
        {
            instance.InternalRegisterCanvasElementForGraphicRebuild(element);
        }

        /// <summary>
        /// <para>Rebuild the layout of the given element.</para>
        /// </summary>
        /// <param name="element">Element to rebuild.</param>
        public static void RegisterCanvasElementForLayoutRebuild(ICanvasElement element)
        {
            instance.InternalRegisterCanvasElementForLayoutRebuild(element);
        }

        private static int SortLayoutList(ICanvasElement x, ICanvasElement y)
        {
            Transform child = x.transform;
            Transform transform = y.transform;
            return (ParentCount(child) - ParentCount(transform));
        }

        /// <summary>
        /// <para>Rebuild the layout of the given element.</para>
        /// </summary>
        /// <param name="element">Element to rebuild.</param>
        /// <returns>
        /// <para>Was the element scheduled.</para>
        /// </returns>
        public static bool TryRegisterCanvasElementForGraphicRebuild(ICanvasElement element)
        {
            return instance.InternalRegisterCanvasElementForGraphicRebuild(element);
        }

        /// <summary>
        /// <para>Was the element scheduled.</para>
        /// </summary>
        /// <param name="element">Element to rebuild.</param>
        /// <returns>
        /// <para>Was the element scheduled.</para>
        /// </returns>
        public static bool TryRegisterCanvasElementForLayoutRebuild(ICanvasElement element)
        {
            return instance.InternalRegisterCanvasElementForLayoutRebuild(element);
        }

        /// <summary>
        /// <para>Remove the given element from rebuild.</para>
        /// </summary>
        /// <param name="element">Element to remove.</param>
        public static void UnRegisterCanvasElementForRebuild(ICanvasElement element)
        {
            instance.InternalUnRegisterCanvasElementForLayoutRebuild(element);
            instance.InternalUnRegisterCanvasElementForGraphicRebuild(element);
        }

        /// <summary>
        /// <para>Get the singleton registry.</para>
        /// </summary>
        public static CanvasUpdateRegistry instance
        {
            get
            {
                if (s_Instance == null)
                {
                    s_Instance = new CanvasUpdateRegistry();
                }
                return s_Instance;
            }
        }
    }
}

