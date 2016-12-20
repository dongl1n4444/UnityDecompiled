namespace UnityEngine.UI
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// <para>Mask related utility class.</para>
    /// </summary>
    public class MaskUtilities
    {
        /// <summary>
        /// <para>Find a root Canvas.</para>
        /// </summary>
        /// <param name="start">Search start.</param>
        /// <returns>
        /// <para>Canvas transform.</para>
        /// </returns>
        public static Transform FindRootSortOverrideCanvas(Transform start)
        {
            List<Canvas> results = ListPool<Canvas>.Get();
            start.GetComponentsInParent<Canvas>(false, results);
            Canvas canvas = null;
            for (int i = 0; i < results.Count; i++)
            {
                canvas = results[i];
                if (canvas.overrideSorting)
                {
                    break;
                }
            }
            ListPool<Canvas>.Release(results);
            return ((canvas == null) ? null : canvas.transform);
        }

        /// <summary>
        /// <para>Find the correct RectMask2D for a given IClippable.</para>
        /// </summary>
        /// <param name="transform">Clippable to search from.</param>
        /// <param name="clippable"></param>
        public static RectMask2D GetRectMaskForClippable(IClippable clippable)
        {
            List<RectMask2D> results = ListPool<RectMask2D>.Get();
            List<Canvas> list2 = ListPool<Canvas>.Get();
            RectMask2D maskd = null;
            clippable.rectTransform.GetComponentsInParent<RectMask2D>(false, results);
            if (results.Count > 0)
            {
                for (int i = 0; i < results.Count; i++)
                {
                    maskd = results[i];
                    if (maskd.gameObject == clippable.gameObject)
                    {
                        maskd = null;
                    }
                    else if (!maskd.isActiveAndEnabled)
                    {
                        maskd = null;
                    }
                    else
                    {
                        clippable.rectTransform.GetComponentsInParent<Canvas>(false, list2);
                        for (int j = list2.Count - 1; j >= 0; j--)
                        {
                            if (!IsDescendantOrSelf(list2[j].transform, maskd.transform) && list2[j].overrideSorting)
                            {
                                return null;
                            }
                        }
                        return maskd;
                    }
                }
            }
            ListPool<RectMask2D>.Release(results);
            ListPool<Canvas>.Release(list2);
            return maskd;
        }

        public static void GetRectMasksForClip(RectMask2D clipper, List<RectMask2D> masks)
        {
            masks.Clear();
            List<Canvas> results = ListPool<Canvas>.Get();
            List<RectMask2D> list2 = ListPool<RectMask2D>.Get();
            clipper.transform.GetComponentsInParent<RectMask2D>(false, list2);
            if (list2.Count > 0)
            {
                clipper.transform.GetComponentsInParent<Canvas>(false, results);
                for (int i = list2.Count - 1; i >= 0; i--)
                {
                    if (!list2[i].IsActive())
                    {
                        continue;
                    }
                    bool flag = true;
                    for (int j = results.Count - 1; j >= 0; j--)
                    {
                        if (!IsDescendantOrSelf(results[j].transform, list2[i].transform) && results[j].overrideSorting)
                        {
                            flag = false;
                            break;
                        }
                    }
                    if (flag)
                    {
                        masks.Add(list2[i]);
                    }
                }
            }
            ListPool<RectMask2D>.Release(list2);
            ListPool<Canvas>.Release(results);
        }

        /// <summary>
        /// <para>Find the stencil depth for a given element.</para>
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="stopAfter"></param>
        public static int GetStencilDepth(Transform transform, Transform stopAfter)
        {
            int num = 0;
            if (transform != stopAfter)
            {
                Transform parent = transform.parent;
                List<Mask> results = ListPool<Mask>.Get();
                while (parent != null)
                {
                    parent.GetComponents<Mask>(results);
                    for (int i = 0; i < results.Count; i++)
                    {
                        if (((results[i] != null) && results[i].MaskEnabled()) && results[i].graphic.IsActive())
                        {
                            num++;
                            break;
                        }
                    }
                    if (parent == stopAfter)
                    {
                        break;
                    }
                    parent = parent.parent;
                }
                ListPool<Mask>.Release(results);
            }
            return num;
        }

        /// <summary>
        /// <para>Helper function to determine if the child is a descendant of father or is father.</para>
        /// </summary>
        /// <param name="father">The transform to compare against.</param>
        /// <param name="child">The starting transform to search up the hierarchy.</param>
        /// <returns>
        /// <para>Is child equal to father or is a descendant.</para>
        /// </returns>
        public static bool IsDescendantOrSelf(Transform father, Transform child)
        {
            if ((father != null) && (child != null))
            {
                if (father == child)
                {
                    return true;
                }
                while (child.parent != null)
                {
                    if (child.parent == father)
                    {
                        return true;
                    }
                    child = child.parent;
                }
            }
            return false;
        }

        /// <summary>
        /// <para>Notify all IClippables under the given component that they need to recalculate clipping.</para>
        /// </summary>
        /// <param name="mask"></param>
        public static void Notify2DMaskStateChanged(Component mask)
        {
            List<Component> results = ListPool<Component>.Get();
            mask.GetComponentsInChildren<Component>(results);
            for (int i = 0; i < results.Count; i++)
            {
                if ((results[i] != null) && (results[i].gameObject != mask.gameObject))
                {
                    IClippable clippable = results[i] as IClippable;
                    if (clippable != null)
                    {
                        clippable.RecalculateClipping();
                    }
                }
            }
            ListPool<Component>.Release(results);
        }

        /// <summary>
        /// <para>Notify all IMaskable under the given component that they need to recalculate masking.</para>
        /// </summary>
        /// <param name="mask"></param>
        public static void NotifyStencilStateChanged(Component mask)
        {
            List<Component> results = ListPool<Component>.Get();
            mask.GetComponentsInChildren<Component>(results);
            for (int i = 0; i < results.Count; i++)
            {
                if ((results[i] != null) && (results[i].gameObject != mask.gameObject))
                {
                    IMaskable maskable = results[i] as IMaskable;
                    if (maskable != null)
                    {
                        maskable.RecalculateMasking();
                    }
                }
            }
            ListPool<Component>.Release(results);
        }
    }
}

