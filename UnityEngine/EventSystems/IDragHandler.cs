namespace UnityEngine.EventSystems
{
    using System;

    public interface IDragHandler : IEventSystemHandler
    {
        /// <summary>
        /// <para>When draging is occuring this will be called every time the cursor is moved.</para>
        /// </summary>
        /// <param name="eventData">Current event data.</param>
        void OnDrag(PointerEventData eventData);
    }
}

