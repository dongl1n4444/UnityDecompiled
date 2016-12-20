namespace UnityEngine
{
    using System;

    /// <summary>
    /// <para>Suspends the coroutine execution until the supplied delegate evaluates to false.</para>
    /// </summary>
    public sealed class WaitWhile : CustomYieldInstruction
    {
        private Func<bool> m_Predicate;

        public WaitWhile(Func<bool> predicate)
        {
            this.m_Predicate = predicate;
        }

        public override bool keepWaiting
        {
            get
            {
                return this.m_Predicate.Invoke();
            }
        }
    }
}

