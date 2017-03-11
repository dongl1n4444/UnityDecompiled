namespace UnityEngine.TestTools.TestRunner
{
    using System;
    using UnityEngine.Events;

    [Serializable]
    internal class RunStartedEvent : UnityEvent<ITest>
    {
    }
}

