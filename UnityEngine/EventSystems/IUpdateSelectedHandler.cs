namespace UnityEngine.EventSystems
{
    using System;

    public interface IUpdateSelectedHandler : IEventSystemHandler
    {
        /// <summary>
        /// <para>Called by the EventSystem when the object associated with this EventTrigger is updated.</para>
        /// </summary>
        /// <param name="eventData">Current event data.</param>
        void OnUpdateSelected(BaseEventData eventData);
    }
}

