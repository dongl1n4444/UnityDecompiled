namespace UnityEditor.Callbacks
{
    using System;
    using UnityEditor;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Add this attribute to a method to get a notification just after building the scene.</para>
    /// </summary>
    [RequiredByNativeCode]
    public sealed class PostProcessSceneAttribute : CallbackOrderAttribute
    {
        private int m_version;

        public PostProcessSceneAttribute()
        {
            base.m_CallbackOrder = 1;
            this.m_version = 0;
        }

        public PostProcessSceneAttribute(int callbackOrder)
        {
            base.m_CallbackOrder = callbackOrder;
            this.m_version = 0;
        }

        public PostProcessSceneAttribute(int callbackOrder, int version)
        {
            base.m_CallbackOrder = callbackOrder;
            this.m_version = version;
        }

        internal int version =>
            this.m_version;
    }
}

