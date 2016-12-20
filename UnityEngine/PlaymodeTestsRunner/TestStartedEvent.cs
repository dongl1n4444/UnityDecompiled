namespace UnityEngine.PlaymodeTestsRunner
{
    using System;
    using UnityEngine.Events;

    [Serializable]
    internal class TestStartedEvent : UnityEvent<string>
    {
    }
}

