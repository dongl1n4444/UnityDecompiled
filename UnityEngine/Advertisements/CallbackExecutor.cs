namespace UnityEngine.Advertisements
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    internal sealed class CallbackExecutor : MonoBehaviour
    {
        private readonly Queue<Action<CallbackExecutor>> s_Queue = new Queue<Action<CallbackExecutor>>();

        public void Post(Action<CallbackExecutor> action)
        {
            object obj2 = this.s_Queue;
            lock (obj2)
            {
                this.s_Queue.Enqueue(action);
            }
        }

        private void Update()
        {
            object obj2 = this.s_Queue;
            lock (obj2)
            {
                while (this.s_Queue.Count > 0)
                {
                    this.s_Queue.Dequeue()(this);
                }
            }
        }
    }
}

