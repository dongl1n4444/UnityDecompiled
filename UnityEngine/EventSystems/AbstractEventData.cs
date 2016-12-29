namespace UnityEngine.EventSystems
{
    using System;

    /// <summary>
    /// <para>A class that can be used for sending simple events via the event system.</para>
    /// </summary>
    public abstract class AbstractEventData
    {
        protected bool m_Used;

        protected AbstractEventData()
        {
        }

        /// <summary>
        /// <para>Reset the event.</para>
        /// </summary>
        public virtual void Reset()
        {
            this.m_Used = false;
        }

        /// <summary>
        /// <para>Use the event.</para>
        /// </summary>
        public virtual void Use()
        {
            this.m_Used = true;
        }

        /// <summary>
        /// <para>Is the event used?</para>
        /// </summary>
        public virtual bool used =>
            this.m_Used;
    }
}

