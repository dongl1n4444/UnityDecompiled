namespace UnityEngine.EventSystems
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// <para>Base class for any RayCaster.</para>
    /// </summary>
    public abstract class BaseRaycaster : UIBehaviour
    {
        protected BaseRaycaster()
        {
        }

        /// <summary>
        /// <para>See MonoBehaviour.OnDisable.</para>
        /// </summary>
        protected override void OnDisable()
        {
            RaycasterManager.RemoveRaycasters(this);
            base.OnDisable();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            RaycasterManager.AddRaycaster(this);
        }

        public abstract void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList);
        public override string ToString()
        {
            object[] objArray1 = new object[] { "Name: ", base.gameObject, "\neventCamera: ", this.eventCamera, "\nsortOrderPriority: ", this.sortOrderPriority, "\nrenderOrderPriority: ", this.renderOrderPriority };
            return string.Concat(objArray1);
        }

        /// <summary>
        /// <para>The camera that will generate rays for this raycaster.</para>
        /// </summary>
        public abstract Camera eventCamera { get; }

        /// <summary>
        /// <para>Priority of the caster relative to other casters.</para>
        /// </summary>
        [Obsolete("Please use sortOrderPriority and renderOrderPriority", false)]
        public virtual int priority
        {
            get
            {
                return 0;
            }
        }

        /// <summary>
        /// <para>Priority of the raycaster based upon render order.</para>
        /// </summary>
        public virtual int renderOrderPriority
        {
            get
            {
                return -2147483648;
            }
        }

        /// <summary>
        /// <para>Priority of the raycaster based upon sort order.</para>
        /// </summary>
        public virtual int sortOrderPriority
        {
            get
            {
                return -2147483648;
            }
        }
    }
}

