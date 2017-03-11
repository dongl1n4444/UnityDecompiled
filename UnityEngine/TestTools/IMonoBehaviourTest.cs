namespace UnityEngine.TestTools
{
    using System;

    public interface IMonoBehaviourTest
    {
        /// <summary>
        /// <para>Indicates when the test is considered finished.</para>
        /// </summary>
        bool IsTestFinished { get; }
    }
}

