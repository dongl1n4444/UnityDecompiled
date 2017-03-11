namespace UnityEditor
{
    using System;

    /// <summary>
    /// <para>Type of a SerializedProperty.</para>
    /// </summary>
    public enum SerializedPropertyType
    {
        /// <summary>
        /// <para>AnimationCurve property.</para>
        /// </summary>
        AnimationCurve = 14,
        /// <summary>
        /// <para>Array size property.</para>
        /// </summary>
        ArraySize = 12,
        /// <summary>
        /// <para>Boolean property.</para>
        /// </summary>
        Boolean = 1,
        /// <summary>
        /// <para>Bounds property.</para>
        /// </summary>
        Bounds = 15,
        /// <summary>
        /// <para>Character property.</para>
        /// </summary>
        Character = 13,
        /// <summary>
        /// <para>Color property.</para>
        /// </summary>
        Color = 4,
        /// <summary>
        /// <para>Enumeration property.</para>
        /// </summary>
        Enum = 7,
        /// <summary>
        /// <para>A reference to another Object in the Scene. This is done via an ExposedReference type and resolves to a reference to an Object that exists in the context of the SerializedObject containing the SerializedProperty.</para>
        /// </summary>
        ExposedReference = 0x12,
        /// <summary>
        /// <para>Float property.</para>
        /// </summary>
        Float = 2,
        Generic = -1,
        /// <summary>
        /// <para>Gradient property.</para>
        /// </summary>
        Gradient = 0x10,
        /// <summary>
        /// <para>Integer property.</para>
        /// </summary>
        Integer = 0,
        /// <summary>
        /// <para>LayerMask property.</para>
        /// </summary>
        LayerMask = 6,
        /// <summary>
        /// <para>Reference to another object.</para>
        /// </summary>
        ObjectReference = 5,
        /// <summary>
        /// <para>Quaternion property.</para>
        /// </summary>
        Quaternion = 0x11,
        /// <summary>
        /// <para>Rectangle property.</para>
        /// </summary>
        Rect = 11,
        /// <summary>
        /// <para>String property.</para>
        /// </summary>
        String = 3,
        /// <summary>
        /// <para>2D vector property.</para>
        /// </summary>
        Vector2 = 8,
        /// <summary>
        /// <para>3D vector property.</para>
        /// </summary>
        Vector3 = 9,
        /// <summary>
        /// <para>4D vector property.</para>
        /// </summary>
        Vector4 = 10
    }
}

