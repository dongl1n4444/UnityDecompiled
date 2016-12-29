namespace UnityEditor
{
    using System;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Base class for Attributes that require a callback index.</para>
    /// </summary>
    [RequiredByNativeCode]
    public abstract class CallbackOrderAttribute : Attribute
    {
        protected int m_CallbackOrder;

        protected CallbackOrderAttribute()
        {
        }

        internal int callbackOrder =>
            this.m_CallbackOrder;
    }
}

