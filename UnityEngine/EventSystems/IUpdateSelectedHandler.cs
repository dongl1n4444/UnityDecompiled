namespace UnityEngine.EventSystems
{
    using System;

    public interface IUpdateSelectedHandler : IEventSystemHandler
    {
        /// <summary>
        /// <para></para>
        /// </summary>
        /// <param name="eventData">Current event data.</param>
        void OnUpdateSelected(BaseEventData eventData);
    }
}

