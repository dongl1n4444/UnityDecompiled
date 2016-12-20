namespace UnityEditor.Callbacks
{
    using System;
    using UnityEditor;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Add this attribute to a method to get a notification just after building the player.</para>
    /// </summary>
    [RequiredByNativeCode]
    public sealed class PostProcessBuildAttribute : CallbackOrderAttribute
    {
        public PostProcessBuildAttribute()
        {
            base.m_CallbackOrder = 1;
        }

        public PostProcessBuildAttribute(int callbackOrder)
        {
            base.m_CallbackOrder = callbackOrder;
        }
    }
}

