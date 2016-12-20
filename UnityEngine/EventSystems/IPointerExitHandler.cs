namespace UnityEngine.EventSystems
{
    using System;

    public interface IPointerExitHandler : IEventSystemHandler
    {
        /// <summary>
        /// <para></para>
        /// </summary>
        /// <param name="eventData">Current event data.</param>
        void OnPointerExit(PointerEventData eventData);
    }
}

