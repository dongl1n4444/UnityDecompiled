namespace UnityEngine.EventSystems
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// <para>Raycaster for casting against 2D Physics components.</para>
    /// </summary>
    [AddComponentMenu("Event/Physics 2D Raycaster"), RequireComponent(typeof(Camera))]
    public class Physics2DRaycaster : PhysicsRaycaster
    {
        protected Physics2DRaycaster()
        {
        }

        public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
        {
            if (this.eventCamera != null)
            {
                Ray ray;
                float num;
                base.ComputeRayAndDistance(eventData, out ray, out num);
                if (ReflectionMethodsCache.Singleton.getRayIntersectionAll != null)
                {
                    RaycastHit2D[] hitdArray = ReflectionMethodsCache.Singleton.getRayIntersectionAll(ray, num, base.finalEventMask);
                    if (hitdArray.Length != 0)
                    {
                        int index = 0;
                        int length = hitdArray.Length;
                        while (index < length)
                        {
                            SpriteRenderer component = hitdArray[index].collider.gameObject.GetComponent<SpriteRenderer>();
                            RaycastResult item = new RaycastResult {
                                gameObject = hitdArray[index].collider.gameObject,
                                module = this,
                                distance = Vector3.Distance(this.eventCamera.transform.position, (Vector3) hitdArray[index].point),
                                worldPosition = (Vector3) hitdArray[index].point,
                                worldNormal = (Vector3) hitdArray[index].normal,
                                screenPosition = eventData.position,
                                index = resultAppendList.Count,
                                sortingLayer = (component == null) ? 0 : component.sortingLayerID,
                                sortingOrder = (component == null) ? 0 : component.sortingOrder
                            };
                            resultAppendList.Add(item);
                            index++;
                        }
                    }
                }
            }
        }
    }
}

