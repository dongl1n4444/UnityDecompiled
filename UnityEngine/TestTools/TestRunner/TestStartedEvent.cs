namespace UnityEngine.TestTools.TestRunner
{
    using System;
    using UnityEngine.Events;

    [Serializable]
    internal class TestStartedEvent : UnityEvent<ITest>
    {
    }
}

