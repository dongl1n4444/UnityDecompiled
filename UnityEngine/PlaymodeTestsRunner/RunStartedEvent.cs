namespace UnityEngine.PlaymodeTestsRunner
{
    using System;
    using UnityEngine.Events;

    [Serializable]
    internal class RunStartedEvent : UnityEvent<string, List<string>>
    {
    }
}

