namespace UnityEngine.EventSystems
{
    using System;

    public interface IPointerUpHandler : IEventSystemHandler
    {
        /// <summary>
        /// <para></para>
        /// </summary>
        /// <param name="eventData">Current event data.</param>
        void OnPointerUp(PointerEventData eventData);
    }
}

