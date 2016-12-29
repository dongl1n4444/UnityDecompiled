namespace UnityEngine.Events
{
    using System;
    using System.Reflection;
    using UnityEngine;
    using UnityEngineInternal;

    /// <summary>
    /// <para>A zero argument persistent callback that can be saved with the scene.</para>
    /// </summary>
    [Serializable]
    public class UnityEvent : UnityEventBase
    {
        private readonly object[] m_InvokeArray = new object[0];

        /// <summary>
        /// <para>Add a non persistent listener to the UnityEvent.</para>
        /// </summary>
        /// <param name="call">Callback function.</param>
        public void AddListener(UnityAction call)
        {
            base.AddCall(GetDelegate(call));
        }

        internal void AddPersistentListener(UnityAction call)
        {
            this.AddPersistentListener(call, UnityEventCallState.RuntimeOnly);
        }

        internal void AddPersistentListener(UnityAction call, UnityEventCallState callState)
        {
            int persistentEventCount = base.GetPersistentEventCount();
            base.AddPersistentListener();
            this.RegisterPersistentListener(persistentEventCount, call);
            base.SetPersistentListenerState(persistentEventCount, callState);
        }

        protected override MethodInfo FindMethod_Impl(string name, object targetObj) => 
            UnityEventBase.GetValidMethodInfo(targetObj, name, new System.Type[0]);

        private static BaseInvokableCall GetDelegate(UnityAction action) => 
            new InvokableCall(action);

        internal override BaseInvokableCall GetDelegate(object target, MethodInfo theFunction) => 
            new InvokableCall(target, theFunction);

        /// <summary>
        /// <para>Invoke all registered callbacks (runtime and persistent).</para>
        /// </summary>
        public void Invoke()
        {
            base.Invoke(this.m_InvokeArray);
        }

        internal void RegisterPersistentListener(int index, UnityAction call)
        {
            if (call == null)
            {
                Debug.LogWarning("Registering a Listener requires an action");
            }
            else
            {
                base.RegisterPersistentListener(index, call.Target as UnityEngine.Object, call.Method);
            }
        }

        /// <summary>
        /// <para>Remove a non persistent listener from the UnityEvent.</para>
        /// </summary>
        /// <param name="call">Callback function.</param>
        public void RemoveListener(UnityAction call)
        {
            base.RemoveListener(call.Target, call.GetMethodInfo());
        }
    }
}

