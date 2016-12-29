namespace UnityEngine
{
    using System;
    using System.Collections;

    /// <summary>
    /// <para>Base class for custom yield instructions to suspend coroutines.</para>
    /// </summary>
    public abstract class CustomYieldInstruction : IEnumerator
    {
        protected CustomYieldInstruction()
        {
        }

        public bool MoveNext() => 
            this.keepWaiting;

        public void Reset()
        {
        }

        public object Current =>
            null;

        /// <summary>
        /// <para>Indicates if coroutine should be kept suspended.</para>
        /// </summary>
        public abstract bool keepWaiting { get; }
    }
}

