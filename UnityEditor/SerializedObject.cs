namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Scripting;

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
            this.InternalCreate(monoObjs, null);
        }

        /// <summary>
        /// <para>Create SerializedObject for inspected object.</para>
        /// </summary>
        /// <param name="objs"></param>
        public SerializedObject(UnityEngine.Object[] objs)
        {
            this.InternalCreate(objs, null);
        }

        /// <summary>
        /// <para>Create SerializedObject for inspected object by specifying a context to be used to store and resolve ExposedReference types.</para>
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="context"></param>
        public SerializedObject(UnityEngine.Object obj, UnityEngine.Object context)
        {
            UnityEngine.Object[] monoObjs = new UnityEngine.Object[] { obj };
            this.InternalCreate(monoObjs, context);
        }

        /// <summary>
        /// <para>Create SerializedObject for inspected object by specifying a context to be used to store and resolve ExposedReference types.</para>
        /// </summary>
        /// <param name="objs"></param>
        /// <param name="context"></param>
        public SerializedObject(UnityEngine.Object[] objs, UnityEngine.Object context)
        {
            this.InternalCreate(objs, context);
        }

        /// <summary>
        /// <para>Apply property modifications.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern bool ApplyModifiedProperties();
        /// <summary>
        /// <para>Applies property modifications without registering an undo operation.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern bool ApplyModifiedPropertiesWithoutUndo();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal extern void Cache(int instanceID);
        /// <summary>
        /// <para>Copies a value from a SerializedProperty to the same serialized property on this serialized object.</para>
        /// </summary>
        /// <param name="prop"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void CopyFromSerializedProperty(SerializedProperty prop);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator, ThreadAndSerializationSafe]
        public extern void Dispose();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
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

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern SerializedProperty GetIterator_Internal();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void InternalCreate(UnityEngine.Object[] monoObjs, UnityEngine.Object context);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern SerializedObject LoadFromCache(int instanceID);
        /// <summary>
        /// <para>Update hasMultipleDifferentValues cache on the next Update() call.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void SetIsDifferentCacheDirty();
        /// <summary>
        /// <para>Update serialized object's representation.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void Update();
        /// <summary>
        /// <para>This has been made obsolete. See SerializedObject.UpdateIfRequiredOrScript instead.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), Obsolete("UpdateIfDirtyOrScript has been deprecated. Use UpdateIfRequiredOrScript instead.", false), GeneratedByOldBindingsGenerator]
        public extern void UpdateIfDirtyOrScript();
        /// <summary>
        /// <para>Update serialized object's representation, only if the object has been modified since the last call to Update or if it is a script.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern bool UpdateIfRequiredOrScript();

        /// <summary>
        /// <para>The context used to store and resolve ExposedReference types. This is set by the SerializedObject constructor.</para>
        /// </summary>
        public UnityEngine.Object context { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        internal bool hasModifiedProperties { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        internal InspectorMode inspectorMode { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Does the serialized object represents multiple objects due to multi-object editing? (Read Only)</para>
        /// </summary>
        public bool isEditingMultipleObjects { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Defines the maximum size beyond which arrays cannot be edited when multiple objects are selected.</para>
        /// </summary>
        public int maxArraySizeForMultiEditing { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The inspected object (Read Only).</para>
        /// </summary>
        public UnityEngine.Object targetObject { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>The inspected objects (Read Only).</para>
        /// </summary>
        public UnityEngine.Object[] targetObjects { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }
    }
}

