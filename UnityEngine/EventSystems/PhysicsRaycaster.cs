namespace UnityEngine.EventSystems
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// <para>Raycaster for casting against 3D Physics components.</para>
    /// </summary>
    [AddComponentMenu("Event/Physics Raycaster"), RequireComponent(typeof(Camera))]
    public class PhysicsRaycaster : BaseRaycaster
    {
        [CompilerGenerated]
        private static Comparison<RaycastHit> <>f__am$cache0;
        protected const int kNoEventMaskSet = -1;
        protected Camera m_EventCamera;
        [SerializeField]
        protected LayerMask m_EventMask = -1;

        protected PhysicsRaycaster()
        {
        }

        protected void ComputeRayAndDistance(PointerEventData eventData, out Ray ray, out float distanceToClipPlane)
        {
            ray = this.eventCamera.ScreenPointToRay((Vector3) eventData.position);
            float z = ray.direction.z;
            distanceToClipPlane = !Mathf.Approximately(0f, z) ? Mathf.Abs((float) ((this.eventCamera.farClipPlane - this.eventCamera.nearClipPlane) / z)) : float.PositiveInfinity;
        }

        public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
        {
            if (this.eventCamera != null)
            {
                Ray ray;
                float num;
                this.ComputeRayAndDistance(eventData, out ray, out num);
                if (ReflectionMethodsCache.Singleton.raycast3DAll != null)
                {
                    RaycastHit[] array = ReflectionMethodsCache.Singleton.raycast3DAll(ray, num, this.finalEventMask);
                    if (array.Length > 1)
                    {
                        if (<>f__am$cache0 == null)
                        {
                            <>f__am$cache0 = (r1, r2) => r1.distance.CompareTo(r2.distance);
                        }
                        Array.Sort<RaycastHit>(array, <>f__am$cache0);
                    }
                    if (array.Length != 0)
                    {
                        int index = 0;
                        int length = array.Length;
                        while (index < length)
                        {
                            RaycastResult item = new RaycastResult {
                                gameObject = array[index].collider.gameObject,
                                module = this,
                                distance = array[index].distance,
                                worldPosition = array[index].point,
                                worldNormal = array[index].normal,
                                screenPosition = eventData.position,
                                index = resultAppendList.Count,
                                sortingLayer = 0,
                                sortingOrder = 0
                            };
                            resultAppendList.Add(item);
                            index++;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// <para>Get the depth of the configured camera.</para>
        /// </summary>
        public virtual int depth =>
            ((this.eventCamera == null) ? 0xffffff : ((int) this.eventCamera.depth));

        /// <summary>
        /// <para>Get the camera that is used for this module.</para>
        /// </summary>
        public override Camera eventCamera
        {
            get
            {
                Camera eventCamera;
                if (this.m_EventCamera == null)
                {
                    this.m_EventCamera = base.GetComponent<Camera>();
                }
                if (this.m_EventCamera != null)
                {
                    eventCamera = this.m_EventCamera;
                }
                else
                {
                    eventCamera = Camera.main;
                }
                return eventCamera;
            }
        }

        /// <summary>
        /// <para>Mask of allowed raycast events.</para>
        /// </summary>
        public LayerMask eventMask
        {
            get => 
                this.m_EventMask;
            set
            {
                this.m_EventMask = value;
            }
        }

        /// <summary>
        /// <para>Logical and of Camera mask and eventMask.</para>
        /// </summary>
        public int finalEventMask =>
            ((this.eventCamera == null) ? -1 : (this.eventCamera.cullingMask & this.m_EventMask));
    }
}

