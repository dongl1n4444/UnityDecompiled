namespace UnityEngine.U2D.Interface
{
    using System;

    internal class EventSystem : IEventSystem
    {
        public IEvent current =>
            new Event();
    }
}

