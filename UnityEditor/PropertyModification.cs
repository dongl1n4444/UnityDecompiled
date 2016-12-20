namespace UnityEditor
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Defines a single modified property.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential), RequiredByNativeCode]
    public sealed class PropertyModification
    {
        /// <summary>
        /// <para>Object that will be modified.</para>
        /// </summary>
        public Object target;
        /// <summary>
        /// <para>Property path of the property being modified (Matches as SerializedProperty.propertyPath).</para>
        /// </summary>
        public string propertyPath;
        /// <summary>
        /// <para>The value being applied.</para>
        /// </summary>
        public string value;
        /// <summary>
        /// <para>The value being applied when it is a object reference (which can not be represented as a string).</para>
        /// </summary>
        public Object objectReference;
    }
}

