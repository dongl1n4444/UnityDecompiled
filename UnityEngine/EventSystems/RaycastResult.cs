namespace UnityEngine.EventSystems
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    /// <summary>
    /// <para>A hit result from a BaseRaycaster.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct RaycastResult
    {
        private GameObject m_GameObject;
        /// <summary>
        /// <para>BaseInputModule that raised the hit.</para>
        /// </summary>
        public BaseRaycaster module;
        /// <summary>
        /// <para>Distance to the hit.</para>
        /// </summary>
        public float distance;
        /// <summary>
        /// <para>Hit index.</para>
        /// </summary>
        public float index;
        /// <summary>
        /// <para>The relative depth of the element.</para>
        /// </summary>
        public int depth;
        /// <summary>
        /// <para>The SortingLayer of the hit object.</para>
        /// </summary>
        public int sortingLayer;
        /// <summary>
        /// <para>The SortingOrder for the hit object.</para>
        /// </summary>
        public int sortingOrder;
        /// <summary>
        /// <para>The world position of the where the raycast has hit.</para>
        /// </summary>
        public Vector3 worldPosition;
        /// <summary>
        /// <para>The normal at the hit location of the raycast.</para>
        /// </summary>
        public Vector3 worldNormal;
        /// <summary>
        /// <para>The screen position from which the raycast was generated.</para>
        /// </summary>
        public Vector2 screenPosition;
        /// <summary>
        /// <para>The GameObject that was hit by the raycast.</para>
        /// </summary>
        public GameObject gameObject
        {
            get => 
                this.m_GameObject;
            set
            {
                this.m_GameObject = value;
            }
        }
        /// <summary>
        /// <para>Is there an associated module and a hit GameObject.</para>
        /// </summary>
        public bool isValid =>
            ((this.module != null) && (this.gameObject != null));
        /// <summary>
        /// <para>Reset the result.</para>
        /// </summary>
        public void Clear()
        {
            this.gameObject = null;
            this.module = null;
            this.distance = 0f;
            this.index = 0f;
            this.depth = 0;
            this.sortingLayer = 0;
            this.sortingOrder = 0;
            this.worldNormal = Vector3.up;
            this.worldPosition = Vector3.zero;
            this.screenPosition = Vector2.zero;
        }

        public override string ToString()
        {
            if (!this.isValid)
            {
                return "";
            }
            object[] objArray1 = new object[] { 
                "Name: ", this.gameObject, "\nmodule: ", this.module, "\ndistance: ", this.distance, "\nindex: ", this.index, "\ndepth: ", this.depth, "\nworldNormal: ", this.worldNormal, "\nworldPosition: ", this.worldPosition, "\nscreenPosition: ", this.screenPosition,
                "\nmodule.sortOrderPriority: ", this.module.sortOrderPriority, "\nmodule.renderOrderPriority: ", this.module.renderOrderPriority, "\nsortingLayer: ", this.sortingLayer, "\nsortingOrder: ", this.sortingOrder
            };
            return string.Concat(objArray1);
        }
    }
}

