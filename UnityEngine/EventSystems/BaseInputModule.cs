namespace UnityEngine.EventSystems
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// <para>A base module that raises events and sends them to GameObjects.</para>
    /// </summary>
    [RequireComponent(typeof(EventSystem))]
    public abstract class BaseInputModule : UIBehaviour
    {
        private AxisEventData m_AxisEventData;
        private BaseEventData m_BaseEventData;
        private BaseInput m_DefaultInput;
        private EventSystem m_EventSystem;
        protected BaseInput m_InputOverride;
        [NonSerialized]
        protected List<RaycastResult> m_RaycastResultCache = new List<RaycastResult>();

        protected BaseInputModule()
        {
        }

        /// <summary>
        /// <para>Called when the module is activated. Override this if you want custom code to execute when you activate your module.</para>
        /// </summary>
        public virtual void ActivateModule()
        {
        }

        /// <summary>
        /// <para>Called when the module is deactivated. Override this if you want custom code to execute when you deactivate your module.</para>
        /// </summary>
        public virtual void DeactivateModule()
        {
        }

        /// <summary>
        /// <para>Given an input movement, determine the best MoveDirection.</para>
        /// </summary>
        /// <param name="x">X movement.</param>
        /// <param name="y">Y movement.</param>
        /// <param name="deadZone">Dead zone.</param>
        protected static MoveDirection DetermineMoveDirection(float x, float y) => 
            DetermineMoveDirection(x, y, 0.6f);

        /// <summary>
        /// <para>Given an input movement, determine the best MoveDirection.</para>
        /// </summary>
        /// <param name="x">X movement.</param>
        /// <param name="y">Y movement.</param>
        /// <param name="deadZone">Dead zone.</param>
        protected static MoveDirection DetermineMoveDirection(float x, float y, float deadZone)
        {
            Vector2 vector = new Vector2(x, y);
            if (vector.sqrMagnitude < (deadZone * deadZone))
            {
                return MoveDirection.None;
            }
            if (Mathf.Abs(x) > Mathf.Abs(y))
            {
                if (x > 0f)
                {
                    return MoveDirection.Right;
                }
                return MoveDirection.Left;
            }
            if (y > 0f)
            {
                return MoveDirection.Up;
            }
            return MoveDirection.Down;
        }

        /// <summary>
        /// <para>Given 2 GameObjects, return a common root GameObject (or null).</para>
        /// </summary>
        /// <param name="g1"></param>
        /// <param name="g2"></param>
        protected static GameObject FindCommonRoot(GameObject g1, GameObject g2)
        {
            if ((g1 != null) && (g2 != null))
            {
                for (Transform transform = g1.transform; transform != null; transform = transform.parent)
                {
                    for (Transform transform2 = g2.transform; transform2 != null; transform2 = transform2.parent)
                    {
                        if (transform == transform2)
                        {
                            return transform.gameObject;
                        }
                    }
                }
            }
            return null;
        }

        protected static RaycastResult FindFirstRaycast(List<RaycastResult> candidates)
        {
            for (int i = 0; i < candidates.Count; i++)
            {
                RaycastResult result = candidates[i];
                if (result.gameObject != null)
                {
                    return candidates[i];
                }
            }
            return new RaycastResult();
        }

        /// <summary>
        /// <para>Given some input data generate an AxisEventData that can be used by the event system.</para>
        /// </summary>
        /// <param name="x">X movement.</param>
        /// <param name="y">Y movement.</param>
        /// <param name="moveDeadZone">Dead Zone.</param>
        protected virtual AxisEventData GetAxisEventData(float x, float y, float moveDeadZone)
        {
            if (this.m_AxisEventData == null)
            {
                this.m_AxisEventData = new AxisEventData(this.eventSystem);
            }
            this.m_AxisEventData.Reset();
            this.m_AxisEventData.moveVector = new Vector2(x, y);
            this.m_AxisEventData.moveDir = DetermineMoveDirection(x, y, moveDeadZone);
            return this.m_AxisEventData;
        }

        /// <summary>
        /// <para>Generate a BaseEventData that can be used by the EventSystem.</para>
        /// </summary>
        protected virtual BaseEventData GetBaseEventData()
        {
            if (this.m_BaseEventData == null)
            {
                this.m_BaseEventData = new BaseEventData(this.eventSystem);
            }
            this.m_BaseEventData.Reset();
            return this.m_BaseEventData;
        }

        /// <summary>
        /// <para>Handle sending enter and exit events when a new enter targer is found.</para>
        /// </summary>
        /// <param name="currentPointerData"></param>
        /// <param name="newEnterTarget"></param>
        protected void HandlePointerExitAndEnter(PointerEventData currentPointerData, GameObject newEnterTarget)
        {
            if ((newEnterTarget == null) || (currentPointerData.pointerEnter == null))
            {
                for (int i = 0; i < currentPointerData.hovered.Count; i++)
                {
                    ExecuteEvents.Execute<IPointerExitHandler>(currentPointerData.hovered[i], currentPointerData, ExecuteEvents.pointerExitHandler);
                }
                currentPointerData.hovered.Clear();
                if (newEnterTarget == null)
                {
                    currentPointerData.pointerEnter = newEnterTarget;
                    return;
                }
            }
            if ((currentPointerData.pointerEnter != newEnterTarget) || (newEnterTarget == null))
            {
                GameObject obj2 = FindCommonRoot(currentPointerData.pointerEnter, newEnterTarget);
                if (currentPointerData.pointerEnter != null)
                {
                    for (Transform transform = currentPointerData.pointerEnter.transform; transform != null; transform = transform.parent)
                    {
                        if ((obj2 != null) && (obj2.transform == transform))
                        {
                            break;
                        }
                        ExecuteEvents.Execute<IPointerExitHandler>(transform.gameObject, currentPointerData, ExecuteEvents.pointerExitHandler);
                        currentPointerData.hovered.Remove(transform.gameObject);
                    }
                }
                currentPointerData.pointerEnter = newEnterTarget;
                if (newEnterTarget != null)
                {
                    for (Transform transform2 = newEnterTarget.transform; (transform2 != null) && (transform2.gameObject != obj2); transform2 = transform2.parent)
                    {
                        ExecuteEvents.Execute<IPointerEnterHandler>(transform2.gameObject, currentPointerData, ExecuteEvents.pointerEnterHandler);
                        currentPointerData.hovered.Add(transform2.gameObject);
                    }
                }
            }
        }

        /// <summary>
        /// <para>Check to see if the module is supported. Override this if you have a platfrom specific module (eg. TouchInputModule that you do not want to activate on standalone.</para>
        /// </summary>
        /// <returns>
        /// <para>Is the module supported.</para>
        /// </returns>
        public virtual bool IsModuleSupported() => 
            true;

        /// <summary>
        /// <para>Is the pointer with the given ID over an EventSystem object?</para>
        /// </summary>
        /// <param name="pointerId">Pointer ID.</param>
        public virtual bool IsPointerOverGameObject(int pointerId) => 
            false;

        /// <summary>
        /// <para>See MonoBehaviour.OnDisable.</para>
        /// </summary>
        protected override void OnDisable()
        {
            this.m_EventSystem.UpdateModules();
            base.OnDisable();
        }

        /// <summary>
        /// <para>See MonoBehaviour.OnEnable.</para>
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            this.m_EventSystem = base.GetComponent<EventSystem>();
            this.m_EventSystem.UpdateModules();
        }

        /// <summary>
        /// <para>Process the current tick for the module.</para>
        /// </summary>
        public abstract void Process();
        /// <summary>
        /// <para>Should be activated.</para>
        /// </summary>
        /// <returns>
        /// <para>Should the module be activated.</para>
        /// </returns>
        public virtual bool ShouldActivateModule() => 
            (base.enabled && base.gameObject.activeInHierarchy);

        /// <summary>
        /// <para>Update the internal state of the Module.</para>
        /// </summary>
        public virtual void UpdateModule()
        {
        }

        protected EventSystem eventSystem =>
            this.m_EventSystem;

        /// <summary>
        /// <para>The current BaseInput being used by the input module.</para>
        /// </summary>
        public BaseInput input
        {
            get
            {
                if (this.m_InputOverride != null)
                {
                    return this.m_InputOverride;
                }
                if (this.m_DefaultInput == null)
                {
                    BaseInput[] components = base.GetComponents<BaseInput>();
                    foreach (BaseInput input2 in components)
                    {
                        if ((input2 != null) && (input2.GetType() == typeof(BaseInput)))
                        {
                            this.m_DefaultInput = input2;
                            break;
                        }
                    }
                    if (this.m_DefaultInput == null)
                    {
                        this.m_DefaultInput = base.gameObject.AddComponent<BaseInput>();
                    }
                }
                return this.m_DefaultInput;
            }
        }
    }
}

