namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    /// <summary>
    /// <para>Utility class containing helper methods for working with  RectTransform.</para>
    /// </summary>
    public sealed class RectTransformUtility
    {
        private static Vector3[] s_Corners = new Vector3[4];

        private RectTransformUtility()
        {
        }

        public static Bounds CalculateRelativeRectTransformBounds(Transform trans) => 
            CalculateRelativeRectTransformBounds(trans, trans);

        public static Bounds CalculateRelativeRectTransformBounds(Transform root, Transform child)
        {
            RectTransform[] componentsInChildren = child.GetComponentsInChildren<RectTransform>(false);
            if (componentsInChildren.Length > 0)
            {
                Vector3 rhs = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
                Vector3 vector2 = new Vector3(float.MinValue, float.MinValue, float.MinValue);
                Matrix4x4 worldToLocalMatrix = root.worldToLocalMatrix;
                int index = 0;
                int length = componentsInChildren.Length;
                while (index < length)
                {
                    componentsInChildren[index].GetWorldCorners(s_Corners);
                    for (int i = 0; i < 4; i++)
                    {
                        Vector3 lhs = worldToLocalMatrix.MultiplyPoint3x4(s_Corners[i]);
                        rhs = Vector3.Min(lhs, rhs);
                        vector2 = Vector3.Max(lhs, vector2);
                    }
                    index++;
                }
                Bounds bounds = new Bounds(rhs, Vector3.zero);
                bounds.Encapsulate(vector2);
                return bounds;
            }
            return new Bounds(Vector3.zero, Vector3.zero);
        }

        /// <summary>
        /// <para>Flips the horizontal and vertical axes of the RectTransform size and alignment, and optionally its children as well.</para>
        /// </summary>
        /// <param name="rect">The RectTransform to flip.</param>
        /// <param name="keepPositioning">Flips around the pivot if true. Flips within the parent rect if false.</param>
        /// <param name="recursive">Flip the children as well?</param>
        public static void FlipLayoutAxes(RectTransform rect, bool keepPositioning, bool recursive)
        {
            if (rect != null)
            {
                if (recursive)
                {
                    for (int i = 0; i < rect.childCount; i++)
                    {
                        RectTransform child = rect.GetChild(i) as RectTransform;
                        if (child != null)
                        {
                            FlipLayoutAxes(child, false, true);
                        }
                    }
                }
                rect.pivot = GetTransposed(rect.pivot);
                rect.sizeDelta = GetTransposed(rect.sizeDelta);
                if (!keepPositioning)
                {
                    rect.anchoredPosition = GetTransposed(rect.anchoredPosition);
                    rect.anchorMin = GetTransposed(rect.anchorMin);
                    rect.anchorMax = GetTransposed(rect.anchorMax);
                }
            }
        }

        /// <summary>
        /// <para>Flips the alignment of the RectTransform along the horizontal or vertical axis, and optionally its children as well.</para>
        /// </summary>
        /// <param name="rect">The RectTransform to flip.</param>
        /// <param name="keepPositioning">Flips around the pivot if true. Flips within the parent rect if false.</param>
        /// <param name="recursive">Flip the children as well?</param>
        /// <param name="axis">The axis to flip along. 0 is horizontal and 1 is vertical.</param>
        public static void FlipLayoutOnAxis(RectTransform rect, int axis, bool keepPositioning, bool recursive)
        {
            if (rect != null)
            {
                if (recursive)
                {
                    for (int i = 0; i < rect.childCount; i++)
                    {
                        RectTransform child = rect.GetChild(i) as RectTransform;
                        if (child != null)
                        {
                            FlipLayoutOnAxis(child, axis, false, true);
                        }
                    }
                }
                Vector2 pivot = rect.pivot;
                pivot[axis] = 1f - pivot[axis];
                rect.pivot = pivot;
                if (!keepPositioning)
                {
                    Vector2 anchoredPosition = rect.anchoredPosition;
                    anchoredPosition[axis] = -anchoredPosition[axis];
                    rect.anchoredPosition = anchoredPosition;
                    Vector2 anchorMin = rect.anchorMin;
                    Vector2 anchorMax = rect.anchorMax;
                    float num2 = anchorMin[axis];
                    anchorMin[axis] = 1f - anchorMax[axis];
                    anchorMax[axis] = 1f - num2;
                    rect.anchorMin = anchorMin;
                    rect.anchorMax = anchorMax;
                }
            }
        }

        private static Vector2 GetTransposed(Vector2 input) => 
            new Vector2(input.y, input.x);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_PixelAdjustPoint(ref Vector2 point, Transform elementTransform, Canvas canvas, out Vector2 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_PixelAdjustRect(RectTransform rectTransform, Canvas canvas, out Rect value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern bool INTERNAL_CALL_RectangleContainsScreenPoint(RectTransform rect, ref Vector2 screenPoint, Camera cam);
        /// <summary>
        /// <para>Convert a given point in screen space into a pixel correct point.</para>
        /// </summary>
        /// <param name="point"></param>
        /// <param name="elementTransform"></param>
        /// <param name="canvas"></param>
        /// <returns>
        /// <para>Pixel adjusted point.</para>
        /// </returns>
        public static Vector2 PixelAdjustPoint(Vector2 point, Transform elementTransform, Canvas canvas)
        {
            Vector2 vector;
            INTERNAL_CALL_PixelAdjustPoint(ref point, elementTransform, canvas, out vector);
            return vector;
        }

        /// <summary>
        /// <para>Given a rect transform, return the corner points in pixel accurate coordinates.</para>
        /// </summary>
        /// <param name="rectTransform"></param>
        /// <param name="canvas"></param>
        /// <returns>
        /// <para>Pixel adjusted rect.</para>
        /// </returns>
        public static Rect PixelAdjustRect(RectTransform rectTransform, Canvas canvas)
        {
            Rect rect;
            INTERNAL_CALL_PixelAdjustRect(rectTransform, canvas, out rect);
            return rect;
        }

        public static bool RectangleContainsScreenPoint(RectTransform rect, Vector2 screenPoint) => 
            RectangleContainsScreenPoint(rect, screenPoint, null);

        /// <summary>
        /// <para>Does the RectTransform contain the screen point as seen from the given camera?</para>
        /// </summary>
        /// <param name="rect">The RectTransform to test with.</param>
        /// <param name="screenPoint">The screen point to test.</param>
        /// <param name="cam">The camera from which the test is performed from. (Optional)</param>
        /// <returns>
        /// <para>True if the point is inside the rectangle.</para>
        /// </returns>
        public static bool RectangleContainsScreenPoint(RectTransform rect, Vector2 screenPoint, Camera cam) => 
            INTERNAL_CALL_RectangleContainsScreenPoint(rect, ref screenPoint, cam);

        public static bool ScreenPointToLocalPointInRectangle(RectTransform rect, Vector2 screenPoint, Camera cam, out Vector2 localPoint)
        {
            Vector3 vector;
            localPoint = Vector2.zero;
            if (ScreenPointToWorldPointInRectangle(rect, screenPoint, cam, out vector))
            {
                localPoint = rect.InverseTransformPoint(vector);
                return true;
            }
            return false;
        }

        public static Ray ScreenPointToRay(Camera cam, Vector2 screenPos)
        {
            if (cam != null)
            {
                return cam.ScreenPointToRay((Vector3) screenPos);
            }
            Vector3 origin = (Vector3) screenPos;
            origin.z -= 100f;
            return new Ray(origin, Vector3.forward);
        }

        public static bool ScreenPointToWorldPointInRectangle(RectTransform rect, Vector2 screenPoint, Camera cam, out Vector3 worldPoint)
        {
            float num;
            worldPoint = (Vector3) Vector2.zero;
            Ray ray = ScreenPointToRay(cam, screenPoint);
            Plane plane = new Plane((Vector3) (rect.rotation * Vector3.back), rect.position);
            if (!plane.Raycast(ray, out num))
            {
                return false;
            }
            worldPoint = ray.GetPoint(num);
            return true;
        }

        public static Vector2 WorldToScreenPoint(Camera cam, Vector3 worldPoint) => 
            cam?.WorldToScreenPoint(worldPoint);
    }
}

