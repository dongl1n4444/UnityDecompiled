namespace UnityEngine.UI
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.Serialization;

    /// <summary>
    /// <para>A BaseRaycaster to raycast against Graphic elements.</para>
    /// </summary>
    [AddComponentMenu("Event/Graphic Raycaster"), RequireComponent(typeof(Canvas))]
    public class GraphicRaycaster : BaseRaycaster
    {
        [CompilerGenerated]
        private static Comparison<Graphic> <>f__am$cache0;
        protected const int kNoEventMaskSet = -1;
        [SerializeField]
        protected LayerMask m_BlockingMask = -1;
        [FormerlySerializedAs("blockingObjects"), SerializeField]
        private BlockingObjects m_BlockingObjects = BlockingObjects.None;
        private Canvas m_Canvas;
        [FormerlySerializedAs("ignoreReversedGraphics"), SerializeField]
        private bool m_IgnoreReversedGraphics = true;
        [NonSerialized]
        private List<Graphic> m_RaycastResults = new List<Graphic>();
        [NonSerialized]
        private static readonly List<Graphic> s_SortedGraphics = new List<Graphic>();

        protected GraphicRaycaster()
        {
        }

        public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
        {
            if (this.canvas != null)
            {
                Vector3 position = Display.RelativeMouseAt((Vector3) eventData.position);
                int targetDisplay = this.canvas.targetDisplay;
                if (position.z == targetDisplay)
                {
                    Vector2 vector2;
                    if (position.z == 0f)
                    {
                        position = (Vector3) eventData.position;
                    }
                    if (this.eventCamera == null)
                    {
                        float width = Screen.width;
                        float height = Screen.height;
                        if ((targetDisplay > 0) && (targetDisplay < Display.displays.Length))
                        {
                            width = Display.displays[targetDisplay].systemWidth;
                            height = Display.displays[targetDisplay].systemHeight;
                        }
                        vector2 = new Vector2(position.x / width, position.y / height);
                    }
                    else
                    {
                        vector2 = this.eventCamera.ScreenToViewportPoint(position);
                    }
                    if ((((vector2.x >= 0f) && (vector2.x <= 1f)) && (vector2.y >= 0f)) && (vector2.y <= 1f))
                    {
                        float maxValue = float.MaxValue;
                        Ray r = new Ray();
                        if (this.eventCamera != null)
                        {
                            r = this.eventCamera.ScreenPointToRay(position);
                        }
                        if ((this.canvas.renderMode != RenderMode.ScreenSpaceOverlay) && (this.blockingObjects != BlockingObjects.None))
                        {
                            RaycastHit hit;
                            float f = 100f;
                            if (this.eventCamera != null)
                            {
                                f = this.eventCamera.farClipPlane - this.eventCamera.nearClipPlane;
                            }
                            if ((((this.blockingObjects == BlockingObjects.ThreeD) || (this.blockingObjects == BlockingObjects.All)) && (ReflectionMethodsCache.Singleton.raycast3D != null)) && ReflectionMethodsCache.Singleton.raycast3D(r, out hit, f, (int) this.m_BlockingMask))
                            {
                                maxValue = hit.distance;
                            }
                            if (((this.blockingObjects == BlockingObjects.TwoD) || (this.blockingObjects == BlockingObjects.All)) && (ReflectionMethodsCache.Singleton.raycast2D != null))
                            {
                                RaycastHit2D hitd = ReflectionMethodsCache.Singleton.raycast2D(r.origin, r.direction, f, (int) this.m_BlockingMask);
                                if (hitd.collider != null)
                                {
                                    maxValue = hitd.fraction * f;
                                }
                            }
                        }
                        this.m_RaycastResults.Clear();
                        Raycast(this.canvas, this.eventCamera, position, this.m_RaycastResults);
                        for (int i = 0; i < this.m_RaycastResults.Count; i++)
                        {
                            GameObject gameObject = this.m_RaycastResults[i].gameObject;
                            bool flag = true;
                            if (this.ignoreReversedGraphics)
                            {
                                if (this.eventCamera == null)
                                {
                                    Vector3 rhs = (Vector3) (gameObject.transform.rotation * Vector3.forward);
                                    flag = Vector3.Dot(Vector3.forward, rhs) > 0f;
                                }
                                else
                                {
                                    Vector3 lhs = (Vector3) (this.eventCamera.transform.rotation * Vector3.forward);
                                    Vector3 vector5 = (Vector3) (gameObject.transform.rotation * Vector3.forward);
                                    flag = Vector3.Dot(lhs, vector5) > 0f;
                                }
                            }
                            if (flag)
                            {
                                float num7 = 0f;
                                if ((this.eventCamera == null) || (this.canvas.renderMode == RenderMode.ScreenSpaceOverlay))
                                {
                                    num7 = 0f;
                                }
                                else
                                {
                                    Transform transform = gameObject.transform;
                                    Vector3 forward = transform.forward;
                                    num7 = Vector3.Dot(forward, transform.position - r.origin) / Vector3.Dot(forward, r.direction);
                                    if (num7 < 0f)
                                    {
                                        continue;
                                    }
                                }
                                if (num7 < maxValue)
                                {
                                    RaycastResult item = new RaycastResult {
                                        gameObject = gameObject,
                                        module = this,
                                        distance = num7,
                                        screenPosition = position,
                                        index = resultAppendList.Count,
                                        depth = this.m_RaycastResults[i].depth,
                                        sortingLayer = this.canvas.sortingLayerID,
                                        sortingOrder = this.canvas.sortingOrder
                                    };
                                    resultAppendList.Add(item);
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void Raycast(Canvas canvas, Camera eventCamera, Vector2 pointerPosition, List<Graphic> results)
        {
            IList<Graphic> graphicsForCanvas = GraphicRegistry.GetGraphicsForCanvas(canvas);
            for (int i = 0; i < graphicsForCanvas.Count; i++)
            {
                Graphic item = graphicsForCanvas[i];
                if (((item.depth != -1) && item.raycastTarget) && (RectTransformUtility.RectangleContainsScreenPoint(item.rectTransform, pointerPosition, eventCamera) && item.Raycast(pointerPosition, eventCamera)))
                {
                    s_SortedGraphics.Add(item);
                }
            }
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = (g1, g2) => g2.depth.CompareTo(g1.depth);
            }
            s_SortedGraphics.Sort(<>f__am$cache0);
            for (int j = 0; j < s_SortedGraphics.Count; j++)
            {
                results.Add(s_SortedGraphics[j]);
            }
            s_SortedGraphics.Clear();
        }

        /// <summary>
        /// <para>Type of objects that will block graphic raycasts.</para>
        /// </summary>
        public BlockingObjects blockingObjects
        {
            get => 
                this.m_BlockingObjects;
            set
            {
                this.m_BlockingObjects = value;
            }
        }

        private Canvas canvas
        {
            get
            {
                if (this.m_Canvas == null)
                {
                    this.m_Canvas = base.GetComponent<Canvas>();
                }
                return this.m_Canvas;
            }
        }

        /// <summary>
        /// <para>See: BaseRaycaster.</para>
        /// </summary>
        public override Camera eventCamera
        {
            get
            {
                if ((this.canvas.renderMode == RenderMode.ScreenSpaceOverlay) || ((this.canvas.renderMode == RenderMode.ScreenSpaceCamera) && (this.canvas.worldCamera == null)))
                {
                    return null;
                }
                return ((this.canvas.worldCamera == null) ? Camera.main : this.canvas.worldCamera);
            }
        }

        /// <summary>
        /// <para>Should graphics facing away from the raycaster be considered?</para>
        /// </summary>
        public bool ignoreReversedGraphics
        {
            get => 
                this.m_IgnoreReversedGraphics;
            set
            {
                this.m_IgnoreReversedGraphics = value;
            }
        }

        public override int renderOrderPriority
        {
            get
            {
                if (this.canvas.renderMode == RenderMode.ScreenSpaceOverlay)
                {
                    return this.canvas.renderOrder;
                }
                return base.renderOrderPriority;
            }
        }

        public override int sortOrderPriority
        {
            get
            {
                if (this.canvas.renderMode == RenderMode.ScreenSpaceOverlay)
                {
                    return this.canvas.sortingOrder;
                }
                return base.sortOrderPriority;
            }
        }

        /// <summary>
        /// <para>List of Raycasters to check for canvas blocking elements.</para>
        /// </summary>
        public enum BlockingObjects
        {
            None,
            TwoD,
            ThreeD,
            All
        }
    }
}

