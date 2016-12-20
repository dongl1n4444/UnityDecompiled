namespace UnityEngine.EventSystems
{
    using System;

    public interface IDropHandler : IEventSystemHandler
    {
        /// <summary>
        /// <para>Called by a BaseInputModule on a target that can accept a drop.</para>
        /// </summary>
        /// <param name="eventData">Current event data.</param>
        void OnDrop(PointerEventData eventData);
    }
}

