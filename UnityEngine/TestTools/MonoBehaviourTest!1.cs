namespace UnityEngine.TestTools
{
    using System;
    using UnityEngine;

    /// <summary>
    /// <para>Wrapper for running tests that are imlpemented as MonoBehaviours.</para>
    /// </summary>
    public class MonoBehaviourTest<T> : CustomYieldInstruction where T: MonoBehaviour, IMonoBehaviourTest
    {
        private MonoBehaviour m_MonoBehaviour;

        public MonoBehaviourTest()
        {
            this.m_MonoBehaviour = new GameObject("MonoBehaviourTest: " + typeof(T).FullName).AddComponent<T>();
        }

        public override bool keepWaiting =>
            !((IMonoBehaviourTest) this.m_MonoBehaviour).IsTestFinished;
    }
}

