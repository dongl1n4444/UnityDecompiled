namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    /// <summary>
    /// <para>SerializedObject and SerializedProperty are classes for editing properties on objects in a completely generic way that automatically handles undo and styling UI for prefabs.</para>
    /// </summary>
    public sealed class SerializedObject
    {
        private IntPtr m_Property;

        /// <summary>
        /// <para>Create SerializedObject for inspected object.</para>
        /// </summary>
        /// <param name="obj"></param>
        public SerializedObject(UnityEngine.Object obj)
        {
            UnityEngine.Object[] monoObjs = new UnityEngine.Object[] { obj };
            this.InternalCreate(monoObjs);
        }

        /// <summary>
        /// <para>Create SerializedObject for inspected object.</para>
        /// </summary>
        /// <param name="objs"></param>
        public SerializedObject(UnityEngine.Object[] objs)
        {
            this.InternalCreate(objs);
        }

        /// <summary>
        /// <para>Apply property modifications.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern bool ApplyModifiedProperties();
        /// <summary>
        /// <para>Applies property modifications without registering an undo operation.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern bool ApplyModifiedPropertiesWithoutUndo();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern void Cache(int instanceID);
        /// <summary>
        /// <para>Copies a value from a SerializedProperty to the same serialized property on this serialized object.</para>
        /// </summary>
        /// <param name="prop"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void CopyFromSerializedProperty(SerializedProperty prop);
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public extern void Dispose();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern PropertyModification ExtractPropertyModification(string propertyPath);
        ~SerializedObject()
        {
            this.Dispose();
        }

        /// <summary>
        /// <para>Find serialized property by name.</para>
        /// </summary>
        /// <param name="propertyPath"></param>
        public SerializedProperty FindProperty(string propertyPath)
        {
            SerializedProperty property = this.GetIterator_Internal();
            property.m_SerializedObject = this;
            if (property.FindPropertyInternal(propertyPath))
            {
                return property;
            }
            return null;
        }

        /// <summary>
        /// <para>Get the first serialized property.</para>
        /// </summary>
        public SerializedProperty GetIterator()
        {
            SerializedProperty property = this.GetIterator_Internal();
            property.m_SerializedObject = this;
            return property;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern SerializedProperty GetIterator_Internal();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void InternalCreate(UnityEngine.Object[] monoObjs);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern SerializedObject LoadFromCache(int instanceID);
        /// <summary>
        /// <para>Update hasMultipleDifferentValues cache on the next Update() call.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void SetIsDifferentCacheDirty();
        /// <summary>
        /// <para>Update serialized object's representation.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void Update();
        /// <summary>
        /// <para>Update serialized object's representation, only if the object has been modified since the last call to Update or if it is a script.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void UpdateIfDirtyOrScript();

        internal bool hasModifiedProperties { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        internal InspectorMode inspectorMode { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Does the serialized object represents multiple objects due to multi-object editing? (Read Only)</para>
        /// </summary>
        public bool isEditingMultipleObjects { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Defines the maximum size beyond which arrays cannot be edited when multiple objects are selected.</para>
        /// </summary>
        public int maxArraySizeForMultiEditing { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The inspected object (Read Only).</para>
        /// </summary>
        public UnityEngine.Object targetObject { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>The inspected objects (Read Only).</para>
        /// </summary>
        public UnityEngine.Object[] targetObjects { [MethodImpl(MethodImplOptions.InternalCall)] get; }
    }
}

