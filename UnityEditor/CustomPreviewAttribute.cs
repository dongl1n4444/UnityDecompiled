namespace UnityEditor
{
    using System;

    /// <summary>
    /// <para>Adds an extra preview in the Inspector for the specified type.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited=false, AllowMultiple=false)]
    public sealed class CustomPreviewAttribute : Attribute
    {
        internal Type m_Type;

        /// <summary>
        /// <para>Tells a DefaultPreview which class it's a preview for.</para>
        /// </summary>
        /// <param name="type">The type you want to create a custom preview for.</param>
        public CustomPreviewAttribute(Type type)
        {
            this.m_Type = type;
        }
    }
}

