namespace UnityEditor
{
    using System;
    using UnityEngine;
    using UnityEngine.Events;

    [Serializable]
    internal class SerializableDelayedCallback : ScriptableObject
    {
        [SerializeField]
        private UnityEvent m_Callback = new UnityEvent();
        [SerializeField]
        private long m_CallbackTicks;

        protected SerializableDelayedCallback()
        {
            EditorApplication.update = (EditorApplication.CallbackFunction) Delegate.Combine(EditorApplication.update, new EditorApplication.CallbackFunction(this.Update));
        }

        public void Cancel()
        {
            EditorApplication.update = (EditorApplication.CallbackFunction) Delegate.Remove(EditorApplication.update, new EditorApplication.CallbackFunction(this.Update));
        }

        public static SerializableDelayedCallback SubscribeCallback(UnityAction action, TimeSpan delayUntilCallback)
        {
            SerializableDelayedCallback callback = ScriptableObject.CreateInstance<SerializableDelayedCallback>();
            callback.m_CallbackTicks = DateTime.UtcNow.Add(delayUntilCallback).Ticks;
            callback.m_Callback.AddPersistentListener(action, UnityEventCallState.EditorAndRuntime);
            return callback;
        }

        private void Update()
        {
            if (DateTime.UtcNow.Ticks >= this.m_CallbackTicks)
            {
                EditorApplication.update = (EditorApplication.CallbackFunction) Delegate.Remove(EditorApplication.update, new EditorApplication.CallbackFunction(this.Update));
                this.m_Callback.Invoke();
            }
        }
    }
}

