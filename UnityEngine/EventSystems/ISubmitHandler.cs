namespace UnityEngine.EventSystems
{
    using System;

    public interface ISubmitHandler : IEventSystemHandler
    {
        /// <summary>
        /// <para></para>
        /// </summary>
        /// <param name="eventData">Current event data.</param>
        void OnSubmit(BaseEventData eventData);
    }
}

