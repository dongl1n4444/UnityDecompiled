namespace UnityEditor.Callbacks
{
    using System;
    using UnityEditor;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Add this attribute to a method to get a notification after scripts have been reloaded.</para>
    /// </summary>
    [RequiredByNativeCode]
    public sealed class DidReloadScripts : CallbackOrderAttribute
    {
        /// <summary>
        /// <para>DidReloadScripts attribute.</para>
        /// </summary>
        /// <param name="callbackOrder">Order in which separate attributes will be processed.</param>
        public DidReloadScripts()
        {
            base.m_CallbackOrder = 1;
        }

        /// <summary>
        /// <para>DidReloadScripts attribute.</para>
        /// </summary>
        /// <param name="callbackOrder">Order in which separate attributes will be processed.</param>
        public DidReloadScripts(int callbackOrder)
        {
            base.m_CallbackOrder = callbackOrder;
        }
    }
}

