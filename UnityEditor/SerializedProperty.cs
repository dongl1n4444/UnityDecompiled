namespace UnityEditor
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Internal;

    /// <summary>
    /// <para>SerializedProperty and SerializedObject are classes for editing properties on objects in a completely generic way that automatically handles undo and styling UI for prefabs.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class SerializedProperty
    {
        private IntPtr m_Property;
        internal SerializedObject m_SerializedObject;
        internal SerializedProperty()
        {
        }

        ~SerializedProperty()
        {
            this.Dispose();
        }

        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public extern void Dispose();
        /// <summary>
        /// <para>See if contained serialized properties are equal.</para>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern bool EqualContents(SerializedProperty x, SerializedProperty y);
        /// <summary>
        /// <para>SerializedObject this property belongs to (Read Only).</para>
        /// </summary>
        public SerializedObject serializedObject
        {
            get
            {
                return this.m_SerializedObject;
            }
        }
        /// <summary>
        /// <para>Does this property represent multiple different values due to multi-object editing? (Read Only)</para>
        /// </summary>
        public bool hasMultipleDifferentValues { [MethodImpl(MethodImplOptions.InternalCall)] get; }
        internal int hasMultipleDifferentValuesBitwise { [MethodImpl(MethodImplOptions.InternalCall)] get; }
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern void SetBitAtIndexForAllTargetsImmediate(int index, bool value);
        /// <summary>
        /// <para>Nice display name of the property. (Read Only)</para>
        /// </summary>
        public string displayName { [MethodImpl(MethodImplOptions.InternalCall)] get; }
        /// <summary>
        /// <para>Name of the property. (Read Only)</para>
        /// </summary>
        public string name { [MethodImpl(MethodImplOptions.InternalCall)] get; }
        /// <summary>
        /// <para>Type name of the property. (Read Only)</para>
        /// </summary>
        public string type { [MethodImpl(MethodImplOptions.InternalCall)] get; }
        /// <summary>
        /// <para>Tooltip of the property. (Read Only)</para>
        /// </summary>
        public string tooltip { [MethodImpl(MethodImplOptions.InternalCall)] get; }
        /// <summary>
        /// <para>Nesting depth of the property. (Read Only)</para>
        /// </summary>
        public int depth { [MethodImpl(MethodImplOptions.InternalCall)] get; }
        /// <summary>
        /// <para>Full path of the property. (Read Only)</para>
        /// </summary>
        public string propertyPath { [MethodImpl(MethodImplOptions.InternalCall)] get; }
        internal int hashCodeForPropertyPathWithoutArrayIndex { [MethodImpl(MethodImplOptions.InternalCall)] get; }
        /// <summary>
        /// <para>Is this property editable? (Read Only)</para>
        /// </summary>
        public bool editable { [MethodImpl(MethodImplOptions.InternalCall)] get; }
        public bool isAnimated { [MethodImpl(MethodImplOptions.InternalCall)] get; }
        /// <summary>
        /// <para>Is this property expanded in the inspector?</para>
        /// </summary>
        public bool isExpanded { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
        /// <summary>
        /// <para>Does it have child properties? (Read Only)</para>
        /// </summary>
        public bool hasChildren { [MethodImpl(MethodImplOptions.InternalCall)] get; }
        /// <summary>
        /// <para>Does it have visible child properties? (Read Only)</para>
        /// </summary>
        public bool hasVisibleChildren { [MethodImpl(MethodImplOptions.InternalCall)] get; }
        /// <summary>
        /// <para>Is property part of a prefab instance? (Read Only)</para>
        /// </summary>
        public bool isInstantiatedPrefab { [MethodImpl(MethodImplOptions.InternalCall)] get; }
        /// <summary>
        /// <para>Is property's value different from the prefab it belongs to?</para>
        /// </summary>
        public bool prefabOverride { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
        /// <summary>
        /// <para>Type of this property (Read Only).</para>
        /// </summary>
        public SerializedPropertyType propertyType { [MethodImpl(MethodImplOptions.InternalCall)] get; }
        /// <summary>
        /// <para>Value of an integer property.</para>
        /// </summary>
        public int intValue { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
        /// <summary>
        /// <para>Value of a integer property as a long.</para>
        /// </summary>
        public long longValue { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
        /// <summary>
        /// <para>Value of a boolean property.</para>
        /// </summary>
        public bool boolValue { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
        /// <summary>
        /// <para>Value of a float property.</para>
        /// </summary>
        public float floatValue { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
        /// <summary>
        /// <para>Value of a float property as a double.</para>
        /// </summary>
        public double doubleValue { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
        /// <summary>
        /// <para>Value of a string property.</para>
        /// </summary>
        public string stringValue { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
        /// <summary>
        /// <para>Value of a color property.</para>
        /// </summary>
        public Color colorValue
        {
            get
            {
                Color color;
                this.INTERNAL_get_colorValue(out color);
                return color;
            }
            set
            {
                this.INTERNAL_set_colorValue(ref value);
            }
        }
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_get_colorValue(out Color value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_set_colorValue(ref Color value);
        /// <summary>
        /// <para>Value of a animation curve property.</para>
        /// </summary>
        public AnimationCurve animationCurveValue { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
        internal Gradient gradientValue { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
        /// <summary>
        /// <para>Value of an object reference property.</para>
        /// </summary>
        public Object objectReferenceValue { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
        public int objectReferenceInstanceIDValue { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
        internal string objectReferenceStringValue { [MethodImpl(MethodImplOptions.InternalCall)] get; }
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern bool ValidateObjectReferenceValue(Object obj);
        internal string objectReferenceTypeString { [MethodImpl(MethodImplOptions.InternalCall)] get; }
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern void AppendFoldoutPPtrValue(Object obj);
        internal string layerMaskStringValue { [MethodImpl(MethodImplOptions.InternalCall)] get; }
        /// <summary>
        /// <para>Enum index of an enum property.</para>
        /// </summary>
        public int enumValueIndex { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
        /// <summary>
        /// <para>Names of enumeration of an enum property.</para>
        /// </summary>
        public string[] enumNames { [MethodImpl(MethodImplOptions.InternalCall)] get; }
        /// <summary>
        /// <para>Display-friendly names of enumeration of an enum property.</para>
        /// </summary>
        public string[] enumDisplayNames { [MethodImpl(MethodImplOptions.InternalCall)] get; }
        /// <summary>
        /// <para>Value of a 2D vector property.</para>
        /// </summary>
        public Vector2 vector2Value
        {
            get
            {
                Vector2 vector;
                this.INTERNAL_get_vector2Value(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_vector2Value(ref value);
            }
        }
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_get_vector2Value(out Vector2 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_set_vector2Value(ref Vector2 value);
        /// <summary>
        /// <para>Value of a 3D vector property.</para>
        /// </summary>
        public Vector3 vector3Value
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_vector3Value(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_vector3Value(ref value);
            }
        }
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_get_vector3Value(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_set_vector3Value(ref Vector3 value);
        /// <summary>
        /// <para>Value of a 4D vector property.</para>
        /// </summary>
        public Vector4 vector4Value
        {
            get
            {
                Vector4 vector;
                this.INTERNAL_get_vector4Value(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_vector4Value(ref value);
            }
        }
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_get_vector4Value(out Vector4 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_set_vector4Value(ref Vector4 value);
        /// <summary>
        /// <para>Value of a quaternion property.</para>
        /// </summary>
        public Quaternion quaternionValue
        {
            get
            {
                Quaternion quaternion;
                this.INTERNAL_get_quaternionValue(out quaternion);
                return quaternion;
            }
            set
            {
                this.INTERNAL_set_quaternionValue(ref value);
            }
        }
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_get_quaternionValue(out Quaternion value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_set_quaternionValue(ref Quaternion value);
        /// <summary>
        /// <para>Value of a rectangle property.</para>
        /// </summary>
        public Rect rectValue
        {
            get
            {
                Rect rect;
                this.INTERNAL_get_rectValue(out rect);
                return rect;
            }
            set
            {
                this.INTERNAL_set_rectValue(ref value);
            }
        }
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_get_rectValue(out Rect value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_set_rectValue(ref Rect value);
        /// <summary>
        /// <para>Value of bounds property.</para>
        /// </summary>
        public Bounds boundsValue
        {
            get
            {
                Bounds bounds;
                this.INTERNAL_get_boundsValue(out bounds);
                return bounds;
            }
            set
            {
                this.INTERNAL_set_boundsValue(ref value);
            }
        }
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_get_boundsValue(out Bounds value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_set_boundsValue(ref Bounds value);
        /// <summary>
        /// <para>Move to next property.</para>
        /// </summary>
        /// <param name="enterChildren"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern bool Next(bool enterChildren);
        /// <summary>
        /// <para>Move to next visible property.</para>
        /// </summary>
        /// <param name="enterChildren"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern bool NextVisible(bool enterChildren);
        /// <summary>
        /// <para>Move to first property of the object.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void Reset();
        /// <summary>
        /// <para>Count remaining visible properties.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern int CountRemaining();
        /// <summary>
        /// <para>Count visible children of this property, including this property itself.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern int CountInProperty();
        /// <summary>
        /// <para>Returns a copy of the SerializedProperty iterator in its current state. This is useful if you want to keep a reference to the current property but continue with the iteration.</para>
        /// </summary>
        public SerializedProperty Copy()
        {
            SerializedProperty property = this.CopyInternal();
            property.m_SerializedObject = this.m_SerializedObject;
            return property;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern SerializedProperty CopyInternal();
        /// <summary>
        /// <para>Duplicates the serialized property.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern bool DuplicateCommand();
        /// <summary>
        /// <para>Deletes the serialized property.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern bool DeleteCommand();
        /// <summary>
        /// <para>Retrieves the SerializedProperty at a relative path to the current property.</para>
        /// </summary>
        /// <param name="relativePropertyPath"></param>
        public SerializedProperty FindPropertyRelative(string relativePropertyPath)
        {
            SerializedProperty property = this.Copy();
            if (property.FindPropertyRelativeInternal(relativePropertyPath))
            {
                return property;
            }
            return null;
        }

        [ExcludeFromDocs]
        public SerializedProperty GetEndProperty()
        {
            bool includeInvisible = false;
            return this.GetEndProperty(includeInvisible);
        }

        /// <summary>
        /// <para>Retrieves the SerializedProperty that defines the end range of this property.</para>
        /// </summary>
        /// <param name="includeInvisible"></param>
        public SerializedProperty GetEndProperty([DefaultValue("false")] bool includeInvisible)
        {
            SerializedProperty property = this.Copy();
            if (includeInvisible)
            {
                property.Next(false);
                return property;
            }
            property.NextVisible(false);
            return property;
        }

        /// <summary>
        /// <para>Retrieves an iterator that allows you to iterator over the current nexting of a serialized property.</para>
        /// </summary>
        [DebuggerHidden]
        public IEnumerator GetEnumerator()
        {
            return new <GetEnumerator>c__Iterator0 { $this = this };
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern bool FindPropertyInternal(string propertyPath);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern bool FindPropertyRelativeInternal(string propertyPath);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern int[] GetLayerMaskSelectedIndex();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern string[] GetLayerMaskNames();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern void ToggleLayerMaskAtIndex(int index);
        /// <summary>
        /// <para>Is this property an array? (Read Only)</para>
        /// </summary>
        public bool isArray { [MethodImpl(MethodImplOptions.InternalCall)] get; }
        /// <summary>
        /// <para>The number of elements in the array. If the SerializedObject contains multiple objects it will return the smallest number of elements. So it is always possible to iterate through the SerializedObject and only get properties found in all objects.</para>
        /// </summary>
        public int arraySize { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
        /// <summary>
        /// <para>Returns the element at the specified index in the array.</para>
        /// </summary>
        /// <param name="index"></param>
        public SerializedProperty GetArrayElementAtIndex(int index)
        {
            SerializedProperty property = this.Copy();
            if (property.GetArrayElementAtIndexInternal(index))
            {
                return property;
            }
            return null;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern bool GetArrayElementAtIndexInternal(int index);
        /// <summary>
        /// <para>Insert an empty element at the specified index in the array.</para>
        /// </summary>
        /// <param name="index"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void InsertArrayElementAtIndex(int index);
        /// <summary>
        /// <para>Delete the element at the specified index in the array.</para>
        /// </summary>
        /// <param name="index"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void DeleteArrayElementAtIndex(int index);
        /// <summary>
        /// <para>Remove all elements from the array.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void ClearArray();
        /// <summary>
        /// <para>Move an array element from srcIndex to dstIndex.</para>
        /// </summary>
        /// <param name="srcIndex"></param>
        /// <param name="dstIndex"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern bool MoveArrayElement(int srcIndex, int dstIndex);
        internal void SetToValueOfTarget(Object target)
        {
            SerializedProperty property = new SerializedObject(target).FindProperty(this.propertyPath);
            if (property == null)
            {
                Debug.LogError(target.name + " does not have the property " + this.propertyPath);
            }
            else
            {
                switch (this.propertyType)
                {
                    case SerializedPropertyType.Integer:
                        this.intValue = property.intValue;
                        break;

                    case SerializedPropertyType.Boolean:
                        this.boolValue = property.boolValue;
                        break;

                    case SerializedPropertyType.Float:
                        this.floatValue = property.floatValue;
                        break;

                    case SerializedPropertyType.String:
                        this.stringValue = property.stringValue;
                        break;

                    case SerializedPropertyType.Color:
                        this.colorValue = property.colorValue;
                        break;

                    case SerializedPropertyType.ObjectReference:
                        this.objectReferenceValue = property.objectReferenceValue;
                        break;

                    case SerializedPropertyType.LayerMask:
                        this.intValue = property.intValue;
                        break;

                    case SerializedPropertyType.Enum:
                        this.enumValueIndex = property.enumValueIndex;
                        break;

                    case SerializedPropertyType.Vector2:
                        this.vector2Value = property.vector2Value;
                        break;

                    case SerializedPropertyType.Vector3:
                        this.vector3Value = property.vector3Value;
                        break;

                    case SerializedPropertyType.Vector4:
                        this.vector4Value = property.vector4Value;
                        break;

                    case SerializedPropertyType.Rect:
                        this.rectValue = property.rectValue;
                        break;

                    case SerializedPropertyType.ArraySize:
                        this.intValue = property.intValue;
                        break;

                    case SerializedPropertyType.Character:
                        this.intValue = property.intValue;
                        break;

                    case SerializedPropertyType.AnimationCurve:
                        this.animationCurveValue = property.animationCurveValue;
                        break;

                    case SerializedPropertyType.Bounds:
                        this.boundsValue = property.boundsValue;
                        break;

                    case SerializedPropertyType.Gradient:
                        this.gradientValue = property.gradientValue;
                        break;
                }
            }
        }
        [CompilerGenerated]
        private sealed class <GetEnumerator>c__Iterator0 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal bool $disposing;
            internal int $PC;
            internal SerializedProperty $this;
            internal SerializedProperty <end>__1;
            internal int <i>__0;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$disposing = true;
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        if (!this.$this.isArray)
                        {
                            this.<end>__1 = this.$this.GetEndProperty();
                            while (this.$this.NextVisible(true) && !SerializedProperty.EqualContents(this.$this, this.<end>__1))
                            {
                                this.$current = this.$this;
                                if (!this.$disposing)
                                {
                                    this.$PC = 2;
                                }
                                goto Label_0104;
                            Label_00D2:;
                            }
                            goto Label_00FB;
                        }
                        this.<i>__0 = 0;
                        break;

                    case 1:
                        this.<i>__0++;
                        break;

                    case 2:
                        goto Label_00D2;

                    default:
                        goto Label_0102;
                }
                if (this.<i>__0 < this.$this.arraySize)
                {
                    this.$current = this.$this.GetArrayElementAtIndex(this.<i>__0);
                    if (!this.$disposing)
                    {
                        this.$PC = 1;
                    }
                    goto Label_0104;
                }
            Label_00FB:
                this.$PC = -1;
            Label_0102:
                return false;
            Label_0104:
                return true;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            object IEnumerator<object>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }
    }
}

