namespace UnityEngine.PlaymodeTestsRunner
{
    using System;
    using UnityEngine.Events;

    [Serializable]
    internal class TestFinishedEvent : UnityEvent<TestResult>
    {
    }
}

