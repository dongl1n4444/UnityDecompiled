namespace UnityEngine.EventSystems
{
    using System;
    using UnityEngine;

    /// <summary>
    /// <para>A class that contains the base event data that is common to all event types in the new EventSystem.</para>
    /// </summary>
    public class BaseEventData : AbstractEventData
    {
        private readonly EventSystem m_EventSystem;

        /// <summary>
        /// <para>Construct a BaseEventData tied to the passed EventSystem.</para>
        /// </summary>
        /// <param name="eventSystem"></param>
        public BaseEventData(EventSystem eventSystem)
        {
            this.m_EventSystem = eventSystem;
        }

        /// <summary>
        /// <para>A reference to the BaseInputModule that sent this event.</para>
        /// </summary>
        public BaseInputModule currentInputModule =>
            this.m_EventSystem.currentInputModule;

        /// <summary>
        /// <para>The object currently considered selected by the EventSystem.</para>
        /// </summary>
        public GameObject selectedObject
        {
            get => 
                this.m_EventSystem.currentSelectedGameObject;
            set
            {
                this.m_EventSystem.SetSelectedGameObject(value, this);
            }
        }
    }
}

