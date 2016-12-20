namespace UnityEngine.EventSystems
{
    using System;

    public interface ICancelHandler : IEventSystemHandler
    {
        /// <summary>
        /// <para>Called by a BaseInputModule when a Cancel event occurs.</para>
        /// </summary>
        /// <param name="eventData">Current event data.</param>
        void OnCancel(BaseEventData eventData);
    }
}

