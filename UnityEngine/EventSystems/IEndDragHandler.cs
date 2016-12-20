namespace UnityEngine.EventSystems
{
    using System;

    public interface IEndDragHandler : IEventSystemHandler
    {
        /// <summary>
        /// <para>Called by a BaseInputModule when a drag is ended.</para>
        /// </summary>
        /// <param name="eventData">Current event data.</param>
        void OnEndDrag(PointerEventData eventData);
    }
}

