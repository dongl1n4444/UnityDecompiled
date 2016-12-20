namespace UnityEngine.PlaymodeTestsRunner
{
    using System;
    using UnityEngine.Events;

    [Serializable]
    internal class RunFinishedEvent : UnityEvent<List<TestResult>>
    {
    }
}

