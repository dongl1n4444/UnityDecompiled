namespace UnityEngine.EventSystems
{
    using System;

    public interface IPointerDownHandler : IEventSystemHandler
    {
        /// <summary>
        /// <para></para>
        /// </summary>
        /// <param name="eventData">Current event data.</param>
        void OnPointerDown(PointerEventData eventData);
    }
}

