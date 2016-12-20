namespace UnityEngine.EventSystems
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Text;
    using UnityEngine;
    using UnityEngine.Serialization;

    /// <summary>
    /// <para>Handles input, raycasting, and sending events.</para>
    /// </summary>
    [AddComponentMenu("Event/Event System")]
    public class EventSystem : UIBehaviour
    {
        [CompilerGenerated]
        private static Comparison<RaycastResult> <>f__mg$cache0;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private static EventSystem <current>k__BackingField;
        private BaseInputModule m_CurrentInputModule;
        private GameObject m_CurrentSelected;
        [SerializeField]
        private int m_DragThreshold = 5;
        private BaseEventData m_DummyData;
        [SerializeField, FormerlySerializedAs("m_Selected")]
        private GameObject m_FirstSelected;
        private bool m_Paused;
        private bool m_SelectionGuard;
        [SerializeField]
        private bool m_sendNavigationEvents = true;
        private List<BaseInputModule> m_SystemInputModules = new List<BaseInputModule>();
        private static readonly Comparison<RaycastResult> s_RaycastComparer;

        static EventSystem()
        {
            if (<>f__mg$cache0 == null)
            {
                <>f__mg$cache0 = new Comparison<RaycastResult>(EventSystem.RaycastComparer);
            }
            s_RaycastComparer = <>f__mg$cache0;
        }

        protected EventSystem()
        {
        }

        private void ChangeEventModule(BaseInputModule module)
        {
            if (this.m_CurrentInputModule != module)
            {
                if (this.m_CurrentInputModule != null)
                {
                    this.m_CurrentInputModule.DeactivateModule();
                }
                if (module != null)
                {
                    module.ActivateModule();
                }
                this.m_CurrentInputModule = module;
            }
        }

        /// <summary>
        /// <para>Is the pointer with the given ID over an EventSystem object?</para>
        /// </summary>
        /// <param name="pointerId">Pointer (touch / mouse) ID.</param>
        public bool IsPointerOverGameObject()
        {
            return this.IsPointerOverGameObject(-1);
        }

        /// <summary>
        /// <para>Is the pointer with the given ID over an EventSystem object?</para>
        /// </summary>
        /// <param name="pointerId">Pointer (touch / mouse) ID.</param>
        public bool IsPointerOverGameObject(int pointerId)
        {
            if (this.m_CurrentInputModule == null)
            {
                return false;
            }
            return this.m_CurrentInputModule.IsPointerOverGameObject(pointerId);
        }

        protected virtual void OnApplicationFocus(bool hasFocus)
        {
            if (((SystemInfo.operatingSystemFamily == OperatingSystemFamily.Windows) || (SystemInfo.operatingSystemFamily == OperatingSystemFamily.Linux)) || (SystemInfo.operatingSystemFamily == OperatingSystemFamily.MacOSX))
            {
                this.m_Paused = !hasFocus;
            }
        }

        /// <summary>
        /// <para>See MonoBehaviour.OnDisable.</para>
        /// </summary>
        protected override void OnDisable()
        {
            if (this.m_CurrentInputModule != null)
            {
                this.m_CurrentInputModule.DeactivateModule();
                this.m_CurrentInputModule = null;
            }
            if (current == this)
            {
                current = null;
            }
            base.OnDisable();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (current == null)
            {
                current = this;
            }
            else
            {
                UnityEngine.Debug.LogWarning("Multiple EventSystems in scene... this is not supported");
            }
        }

        public void RaycastAll(PointerEventData eventData, List<RaycastResult> raycastResults)
        {
            raycastResults.Clear();
            List<BaseRaycaster> raycasters = RaycasterManager.GetRaycasters();
            for (int i = 0; i < raycasters.Count; i++)
            {
                BaseRaycaster raycaster = raycasters[i];
                if ((raycaster != null) && raycaster.IsActive())
                {
                    raycaster.Raycast(eventData, raycastResults);
                }
            }
            raycastResults.Sort(s_RaycastComparer);
        }

        private static int RaycastComparer(RaycastResult lhs, RaycastResult rhs)
        {
            if (lhs.module != rhs.module)
            {
                if (((lhs.module.eventCamera != null) && (rhs.module.eventCamera != null)) && (lhs.module.eventCamera.depth != rhs.module.eventCamera.depth))
                {
                    if (lhs.module.eventCamera.depth < rhs.module.eventCamera.depth)
                    {
                        return 1;
                    }
                    if (lhs.module.eventCamera.depth == rhs.module.eventCamera.depth)
                    {
                        return 0;
                    }
                    return -1;
                }
                if (lhs.module.sortOrderPriority != rhs.module.sortOrderPriority)
                {
                    return rhs.module.sortOrderPriority.CompareTo(lhs.module.sortOrderPriority);
                }
                if (lhs.module.renderOrderPriority != rhs.module.renderOrderPriority)
                {
                    return rhs.module.renderOrderPriority.CompareTo(lhs.module.renderOrderPriority);
                }
            }
            if (lhs.sortingLayer != rhs.sortingLayer)
            {
                int layerValueFromID = SortingLayer.GetLayerValueFromID(rhs.sortingLayer);
                int num5 = SortingLayer.GetLayerValueFromID(lhs.sortingLayer);
                return layerValueFromID.CompareTo(num5);
            }
            if (lhs.sortingOrder != rhs.sortingOrder)
            {
                return rhs.sortingOrder.CompareTo(lhs.sortingOrder);
            }
            if (lhs.depth != rhs.depth)
            {
                return rhs.depth.CompareTo(lhs.depth);
            }
            if (lhs.distance != rhs.distance)
            {
                return lhs.distance.CompareTo(rhs.distance);
            }
            return lhs.index.CompareTo(rhs.index);
        }

        public void SetSelectedGameObject(GameObject selected)
        {
            this.SetSelectedGameObject(selected, this.baseEventDataCache);
        }

        /// <summary>
        /// <para>Set the object as selected. Will send an OnDeselect the the old selected object and OnSelect to the new selected object.</para>
        /// </summary>
        /// <param name="selected">GameObject to select.</param>
        /// <param name="pointer">Associated EventData.</param>
        public void SetSelectedGameObject(GameObject selected, BaseEventData pointer)
        {
            if (this.m_SelectionGuard)
            {
                UnityEngine.Debug.LogError("Attempting to select " + selected + "while already selecting an object.");
            }
            else
            {
                this.m_SelectionGuard = true;
                if (selected == this.m_CurrentSelected)
                {
                    this.m_SelectionGuard = false;
                }
                else
                {
                    ExecuteEvents.Execute<IDeselectHandler>(this.m_CurrentSelected, pointer, ExecuteEvents.deselectHandler);
                    this.m_CurrentSelected = selected;
                    ExecuteEvents.Execute<ISelectHandler>(this.m_CurrentSelected, pointer, ExecuteEvents.selectHandler);
                    this.m_SelectionGuard = false;
                }
            }
        }

        private void TickModules()
        {
            for (int i = 0; i < this.m_SystemInputModules.Count; i++)
            {
                if (this.m_SystemInputModules[i] != null)
                {
                    this.m_SystemInputModules[i].UpdateModule();
                }
            }
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("<b>Selected:</b>" + this.currentSelectedGameObject);
            builder.AppendLine();
            builder.AppendLine();
            builder.AppendLine((this.m_CurrentInputModule == null) ? "No module" : this.m_CurrentInputModule.ToString());
            return builder.ToString();
        }

        protected virtual void Update()
        {
            if ((current == this) && !this.m_Paused)
            {
                this.TickModules();
                bool flag = false;
                for (int i = 0; i < this.m_SystemInputModules.Count; i++)
                {
                    BaseInputModule module = this.m_SystemInputModules[i];
                    if (module.IsModuleSupported() && module.ShouldActivateModule())
                    {
                        if (this.m_CurrentInputModule != module)
                        {
                            this.ChangeEventModule(module);
                            flag = true;
                        }
                        break;
                    }
                }
                if (this.m_CurrentInputModule == null)
                {
                    for (int j = 0; j < this.m_SystemInputModules.Count; j++)
                    {
                        BaseInputModule module2 = this.m_SystemInputModules[j];
                        if (module2.IsModuleSupported())
                        {
                            this.ChangeEventModule(module2);
                            flag = true;
                            break;
                        }
                    }
                }
                if (!flag && (this.m_CurrentInputModule != null))
                {
                    this.m_CurrentInputModule.Process();
                }
            }
        }

        /// <summary>
        /// <para>Recalculate the internal list of BaseInputModules.</para>
        /// </summary>
        public void UpdateModules()
        {
            base.GetComponents<BaseInputModule>(this.m_SystemInputModules);
            for (int i = this.m_SystemInputModules.Count - 1; i >= 0; i--)
            {
                if ((this.m_SystemInputModules[i] == null) || !this.m_SystemInputModules[i].IsActive())
                {
                    this.m_SystemInputModules.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// <para>Returns true if the EventSystem is already in a SetSelectedGameObject.</para>
        /// </summary>
        public bool alreadySelecting
        {
            get
            {
                return this.m_SelectionGuard;
            }
        }

        private BaseEventData baseEventDataCache
        {
            get
            {
                if (this.m_DummyData == null)
                {
                    this.m_DummyData = new BaseEventData(this);
                }
                return this.m_DummyData;
            }
        }

        /// <summary>
        /// <para>Return the current EventSystem.</para>
        /// </summary>
        public static EventSystem current
        {
            [CompilerGenerated]
            get
            {
                return <current>k__BackingField;
            }
            [CompilerGenerated]
            set
            {
                <current>k__BackingField = value;
            }
        }

        /// <summary>
        /// <para>The currently active EventSystems.BaseInputModule.</para>
        /// </summary>
        public BaseInputModule currentInputModule
        {
            get
            {
                return this.m_CurrentInputModule;
            }
        }

        /// <summary>
        /// <para>The GameObject currently considered active by the EventSystem.</para>
        /// </summary>
        public GameObject currentSelectedGameObject
        {
            get
            {
                return this.m_CurrentSelected;
            }
        }

        /// <summary>
        /// <para>The GameObject that was selected first.</para>
        /// </summary>
        public GameObject firstSelectedGameObject
        {
            get
            {
                return this.m_FirstSelected;
            }
            set
            {
                this.m_FirstSelected = value;
            }
        }

        [Obsolete("lastSelectedGameObject is no longer supported")]
        public GameObject lastSelectedGameObject
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// <para>The soft area for dragging in pixels.</para>
        /// </summary>
        public int pixelDragThreshold
        {
            get
            {
                return this.m_DragThreshold;
            }
            set
            {
                this.m_DragThreshold = value;
            }
        }

        /// <summary>
        /// <para>Should the EventSystem allow navigation events (move  submit  cancel).</para>
        /// </summary>
        public bool sendNavigationEvents
        {
            get
            {
                return this.m_sendNavigationEvents;
            }
            set
            {
                this.m_sendNavigationEvents = value;
            }
        }
    }
}

