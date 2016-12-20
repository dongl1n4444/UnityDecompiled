namespace UnityEngine
{
    using System;
    using System.Collections;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Internal;

    /// <summary>
    /// <para>Position, rotation and scale of an object.</para>
    /// </summary>
    public class Transform : Component, IEnumerable
    {
        protected Transform()
        {
        }

        /// <summary>
        /// <para>Unparents all children.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void DetachChildren();
        /// <summary>
        /// <para>Finds a child by name and returns it.</para>
        /// </summary>
        /// <param name="name">Name of child to be found.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern Transform Find(string name);
        public Transform FindChild(string name)
        {
            return this.Find(name);
        }

        /// <summary>
        /// <para>Returns a transform child by index.</para>
        /// </summary>
        /// <param name="index">Index of the child transform to return. Must be smaller than Transform.childCount.</param>
        /// <returns>
        /// <para>Transform child by index.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern Transform GetChild(int index);
        [MethodImpl(MethodImplOptions.InternalCall), Obsolete("use Transform.childCount instead.")]
        public extern int GetChildCount();
        public IEnumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        internal Vector3 GetLocalEulerAngles(RotationOrder order)
        {
            Vector3 vector;
            INTERNAL_CALL_GetLocalEulerAngles(this, order, out vector);
            return vector;
        }

        /// <summary>
        /// <para>Gets the sibling index.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern int GetSiblingIndex();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_GetLocalEulerAngles(Transform self, RotationOrder order, out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_InverseTransformDirection(Transform self, ref Vector3 direction, out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_InverseTransformPoint(Transform self, ref Vector3 position, out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_InverseTransformVector(Transform self, ref Vector3 vector, out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_LookAt(Transform self, ref Vector3 worldPosition, ref Vector3 worldUp);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_RotateAround(Transform self, ref Vector3 axis, float angle);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_RotateAroundInternal(Transform self, ref Vector3 axis, float angle);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_RotateAroundLocal(Transform self, ref Vector3 axis, float angle);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_SetLocalEulerAngles(Transform self, ref Vector3 euler, RotationOrder order);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_SetLocalEulerHint(Transform self, ref Vector3 euler);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_TransformDirection(Transform self, ref Vector3 direction, out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_TransformPoint(Transform self, ref Vector3 position, out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_TransformVector(Transform self, ref Vector3 vector, out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_get_localPosition(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_get_localRotation(out Quaternion value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_get_localScale(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_get_localToWorldMatrix(out Matrix4x4 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_get_lossyScale(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_get_position(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_get_rotation(out Quaternion value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_get_worldToLocalMatrix(out Matrix4x4 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_set_localPosition(ref Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_set_localRotation(ref Quaternion value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_set_localScale(ref Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_set_position(ref Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_set_rotation(ref Quaternion value);
        /// <summary>
        /// <para>Transforms a direction from world space to local space. The opposite of Transform.TransformDirection.</para>
        /// </summary>
        /// <param name="direction"></param>
        public Vector3 InverseTransformDirection(Vector3 direction)
        {
            Vector3 vector;
            INTERNAL_CALL_InverseTransformDirection(this, ref direction, out vector);
            return vector;
        }

        /// <summary>
        /// <para>Transforms the direction x, y, z from world space to local space. The opposite of Transform.TransformDirection.</para>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public Vector3 InverseTransformDirection(float x, float y, float z)
        {
            return this.InverseTransformDirection(new Vector3(x, y, z));
        }

        /// <summary>
        /// <para>Transforms position from world space to local space.</para>
        /// </summary>
        /// <param name="position"></param>
        public Vector3 InverseTransformPoint(Vector3 position)
        {
            Vector3 vector;
            INTERNAL_CALL_InverseTransformPoint(this, ref position, out vector);
            return vector;
        }

        /// <summary>
        /// <para>Transforms the position x, y, z from world space to local space. The opposite of Transform.TransformPoint.</para>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public Vector3 InverseTransformPoint(float x, float y, float z)
        {
            return this.InverseTransformPoint(new Vector3(x, y, z));
        }

        /// <summary>
        /// <para>Transforms a vector from world space to local space. The opposite of Transform.TransformVector.</para>
        /// </summary>
        /// <param name="vector"></param>
        public Vector3 InverseTransformVector(Vector3 vector)
        {
            Vector3 vector2;
            INTERNAL_CALL_InverseTransformVector(this, ref vector, out vector2);
            return vector2;
        }

        /// <summary>
        /// <para>Transforms the vector x, y, z from world space to local space. The opposite of Transform.TransformVector.</para>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public Vector3 InverseTransformVector(float x, float y, float z)
        {
            return this.InverseTransformVector(new Vector3(x, y, z));
        }

        /// <summary>
        /// <para>Is this transform a child of parent?</para>
        /// </summary>
        /// <param name="parent"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern bool IsChildOf(Transform parent);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern bool IsNonUniformScaleTransform();
        /// <summary>
        /// <para>Rotates the transform so the forward vector points at target's current position.</para>
        /// </summary>
        /// <param name="target">Object to point towards.</param>
        /// <param name="worldUp">Vector specifying the upward direction.</param>
        [ExcludeFromDocs]
        public void LookAt(Transform target)
        {
            Vector3 up = Vector3.up;
            this.LookAt(target, up);
        }

        /// <summary>
        /// <para>Rotates the transform so the forward vector points at worldPosition.</para>
        /// </summary>
        /// <param name="worldPosition">Point to look at.</param>
        /// <param name="worldUp">Vector specifying the upward direction.</param>
        [ExcludeFromDocs]
        public void LookAt(Vector3 worldPosition)
        {
            Vector3 up = Vector3.up;
            INTERNAL_CALL_LookAt(this, ref worldPosition, ref up);
        }

        /// <summary>
        /// <para>Rotates the transform so the forward vector points at target's current position.</para>
        /// </summary>
        /// <param name="target">Object to point towards.</param>
        /// <param name="worldUp">Vector specifying the upward direction.</param>
        public void LookAt(Transform target, [DefaultValue("Vector3.up")] Vector3 worldUp)
        {
            if (target != null)
            {
                this.LookAt(target.position, worldUp);
            }
        }

        /// <summary>
        /// <para>Rotates the transform so the forward vector points at worldPosition.</para>
        /// </summary>
        /// <param name="worldPosition">Point to look at.</param>
        /// <param name="worldUp">Vector specifying the upward direction.</param>
        public void LookAt(Vector3 worldPosition, [DefaultValue("Vector3.up")] Vector3 worldUp)
        {
            INTERNAL_CALL_LookAt(this, ref worldPosition, ref worldUp);
        }

        [ExcludeFromDocs]
        public void Rotate(Vector3 eulerAngles)
        {
            Space self = Space.Self;
            this.Rotate(eulerAngles, self);
        }

        [ExcludeFromDocs]
        public void Rotate(Vector3 axis, float angle)
        {
            Space self = Space.Self;
            this.Rotate(axis, angle, self);
        }

        /// <summary>
        /// <para>Applies a rotation of eulerAngles.z degrees around the z axis, eulerAngles.x degrees around the x axis, and eulerAngles.y degrees around the y axis (in that order).</para>
        /// </summary>
        /// <param name="eulerAngles">Rotation to apply.</param>
        /// <param name="relativeTo">Rotation is local to object or World.</param>
        public void Rotate(Vector3 eulerAngles, [DefaultValue("Space.Self")] Space relativeTo)
        {
            Quaternion quaternion = Quaternion.Euler(eulerAngles.x, eulerAngles.y, eulerAngles.z);
            if (relativeTo == Space.Self)
            {
                this.localRotation *= quaternion;
            }
            else
            {
                this.rotation *= (Quaternion.Inverse(this.rotation) * quaternion) * this.rotation;
            }
        }

        [ExcludeFromDocs]
        public void Rotate(float xAngle, float yAngle, float zAngle)
        {
            Space self = Space.Self;
            this.Rotate(xAngle, yAngle, zAngle, self);
        }

        /// <summary>
        /// <para>Rotates the object around axis by angle degrees.</para>
        /// </summary>
        /// <param name="axis">Axis to apply rotation to.</param>
        /// <param name="angle">Degrees to rotation to apply.</param>
        /// <param name="relativeTo">Rotation is local to object or World.</param>
        public void Rotate(Vector3 axis, float angle, [DefaultValue("Space.Self")] Space relativeTo)
        {
            if (relativeTo == Space.Self)
            {
                this.RotateAroundInternal(base.transform.TransformDirection(axis), angle * 0.01745329f);
            }
            else
            {
                this.RotateAroundInternal(axis, angle * 0.01745329f);
            }
        }

        /// <summary>
        /// <para>Applies a rotation of zAngle degrees around the z axis, xAngle degrees around the x axis, and yAngle degrees around the y axis (in that order).</para>
        /// </summary>
        /// <param name="xAngle">Degrees to rotate around the X axis.</param>
        /// <param name="yAngle">Degrees to rotate around the Y axis.</param>
        /// <param name="zAngle">Degrees to rotate around the Z axis.</param>
        /// <param name="relativeTo">Rotation is local to object or World.</param>
        public void Rotate(float xAngle, float yAngle, float zAngle, [DefaultValue("Space.Self")] Space relativeTo)
        {
            this.Rotate(new Vector3(xAngle, yAngle, zAngle), relativeTo);
        }

        /// <summary>
        /// <para></para>
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="angle"></param>
        [Obsolete("use Transform.Rotate instead.")]
        public void RotateAround(Vector3 axis, float angle)
        {
            INTERNAL_CALL_RotateAround(this, ref axis, angle);
        }

        /// <summary>
        /// <para>Rotates the transform about axis passing through point in world coordinates by angle degrees.</para>
        /// </summary>
        /// <param name="point"></param>
        /// <param name="axis"></param>
        /// <param name="angle"></param>
        public void RotateAround(Vector3 point, Vector3 axis, float angle)
        {
            Vector3 position = this.position;
            Quaternion quaternion = Quaternion.AngleAxis(angle, axis);
            Vector3 vector2 = position - point;
            vector2 = (Vector3) (quaternion * vector2);
            position = point + vector2;
            this.position = position;
            this.RotateAroundInternal(axis, angle * 0.01745329f);
        }

        internal void RotateAroundInternal(Vector3 axis, float angle)
        {
            INTERNAL_CALL_RotateAroundInternal(this, ref axis, angle);
        }

        [Obsolete("use Transform.Rotate instead.")]
        public void RotateAroundLocal(Vector3 axis, float angle)
        {
            INTERNAL_CALL_RotateAroundLocal(this, ref axis, angle);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern void SendTransformChangedScale();
        /// <summary>
        /// <para>Move the transform to the start of the local transform list.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void SetAsFirstSibling();
        /// <summary>
        /// <para>Move the transform to the end of the local transform list.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void SetAsLastSibling();
        internal void SetLocalEulerAngles(Vector3 euler, RotationOrder order)
        {
            INTERNAL_CALL_SetLocalEulerAngles(this, ref euler, order);
        }

        internal void SetLocalEulerHint(Vector3 euler)
        {
            INTERNAL_CALL_SetLocalEulerHint(this, ref euler);
        }

        /// <summary>
        /// <para>Set the parent of the transform.</para>
        /// </summary>
        /// <param name="parent">The parent Transform to use.</param>
        /// <param name="worldPositionStays">If true, the parent-relative position, scale and
        /// rotation are modified such that the object keeps the same world space position,
        /// rotation and scale as before.</param>
        public void SetParent(Transform parent)
        {
            this.SetParent(parent, true);
        }

        /// <summary>
        /// <para>Set the parent of the transform.</para>
        /// </summary>
        /// <param name="parent">The parent Transform to use.</param>
        /// <param name="worldPositionStays">If true, the parent-relative position, scale and
        /// rotation are modified such that the object keeps the same world space position,
        /// rotation and scale as before.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void SetParent(Transform parent, bool worldPositionStays);
        /// <summary>
        /// <para>Sets the sibling index.</para>
        /// </summary>
        /// <param name="index">Index to set.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void SetSiblingIndex(int index);
        /// <summary>
        /// <para>Transforms direction from local space to world space.</para>
        /// </summary>
        /// <param name="direction"></param>
        public Vector3 TransformDirection(Vector3 direction)
        {
            Vector3 vector;
            INTERNAL_CALL_TransformDirection(this, ref direction, out vector);
            return vector;
        }

        /// <summary>
        /// <para>Transforms direction x, y, z from local space to world space.</para>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public Vector3 TransformDirection(float x, float y, float z)
        {
            return this.TransformDirection(new Vector3(x, y, z));
        }

        /// <summary>
        /// <para>Transforms position from local space to world space.</para>
        /// </summary>
        /// <param name="position"></param>
        public Vector3 TransformPoint(Vector3 position)
        {
            Vector3 vector;
            INTERNAL_CALL_TransformPoint(this, ref position, out vector);
            return vector;
        }

        /// <summary>
        /// <para>Transforms the position x, y, z from local space to world space.</para>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public Vector3 TransformPoint(float x, float y, float z)
        {
            return this.TransformPoint(new Vector3(x, y, z));
        }

        /// <summary>
        /// <para>Transforms vector from local space to world space.</para>
        /// </summary>
        /// <param name="vector"></param>
        public Vector3 TransformVector(Vector3 vector)
        {
            Vector3 vector2;
            INTERNAL_CALL_TransformVector(this, ref vector, out vector2);
            return vector2;
        }

        /// <summary>
        /// <para>Transforms vector x, y, z from local space to world space.</para>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public Vector3 TransformVector(float x, float y, float z)
        {
            return this.TransformVector(new Vector3(x, y, z));
        }

        /// <summary>
        /// <para>Moves the transform in the direction and distance of translation.</para>
        /// </summary>
        /// <param name="translation"></param>
        /// <param name="relativeTo"></param>
        [ExcludeFromDocs]
        public void Translate(Vector3 translation)
        {
            Space self = Space.Self;
            this.Translate(translation, self);
        }

        /// <summary>
        /// <para>Moves the transform in the direction and distance of translation.</para>
        /// </summary>
        /// <param name="translation"></param>
        /// <param name="relativeTo"></param>
        public void Translate(Vector3 translation, [DefaultValue("Space.Self")] Space relativeTo)
        {
            if (relativeTo == Space.World)
            {
                this.position += translation;
            }
            else
            {
                this.position += this.TransformDirection(translation);
            }
        }

        /// <summary>
        /// <para>Moves the transform in the direction and distance of translation.</para>
        /// </summary>
        /// <param name="translation"></param>
        /// <param name="relativeTo"></param>
        public void Translate(Vector3 translation, Transform relativeTo)
        {
            if (relativeTo != null)
            {
                this.position += relativeTo.TransformDirection(translation);
            }
            else
            {
                this.position += translation;
            }
        }

        /// <summary>
        /// <para>Moves the transform by x along the x axis, y along the y axis, and z along the z axis.</para>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="relativeTo"></param>
        [ExcludeFromDocs]
        public void Translate(float x, float y, float z)
        {
            Space self = Space.Self;
            this.Translate(x, y, z, self);
        }

        /// <summary>
        /// <para>Moves the transform by x along the x axis, y along the y axis, and z along the z axis.</para>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="relativeTo"></param>
        public void Translate(float x, float y, float z, [DefaultValue("Space.Self")] Space relativeTo)
        {
            this.Translate(new Vector3(x, y, z), relativeTo);
        }

        /// <summary>
        /// <para>Moves the transform by x along the x axis, y along the y axis, and z along the z axis.</para>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="relativeTo"></param>
        public void Translate(float x, float y, float z, Transform relativeTo)
        {
            this.Translate(new Vector3(x, y, z), relativeTo);
        }

        /// <summary>
        /// <para>The number of children the Transform has.</para>
        /// </summary>
        public int childCount { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>The rotation as Euler angles in degrees.</para>
        /// </summary>
        public Vector3 eulerAngles
        {
            get
            {
                return this.rotation.eulerAngles;
            }
            set
            {
                this.rotation = Quaternion.Euler(value);
            }
        }

        /// <summary>
        /// <para>The blue axis of the transform in world space.</para>
        /// </summary>
        public Vector3 forward
        {
            get
            {
                return (Vector3) (this.rotation * Vector3.forward);
            }
            set
            {
                this.rotation = Quaternion.LookRotation(value);
            }
        }

        /// <summary>
        /// <para>Has the transform changed since the last time the flag was set to 'false'?</para>
        /// </summary>
        public bool hasChanged { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The transform capacity of the transform's hierarchy data structure.</para>
        /// </summary>
        public int hierarchyCapacity { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The number of transforms in the transform's hierarchy data structure.</para>
        /// </summary>
        public int hierarchyCount { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>The rotation as Euler angles in degrees relative to the parent transform's rotation.</para>
        /// </summary>
        public Vector3 localEulerAngles
        {
            get
            {
                return this.localRotation.eulerAngles;
            }
            set
            {
                this.localRotation = Quaternion.Euler(value);
            }
        }

        /// <summary>
        /// <para>Position of the transform relative to the parent transform.</para>
        /// </summary>
        public Vector3 localPosition
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_localPosition(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_localPosition(ref value);
            }
        }

        /// <summary>
        /// <para>The rotation of the transform relative to the parent transform's rotation.</para>
        /// </summary>
        public Quaternion localRotation
        {
            get
            {
                Quaternion quaternion;
                this.INTERNAL_get_localRotation(out quaternion);
                return quaternion;
            }
            set
            {
                this.INTERNAL_set_localRotation(ref value);
            }
        }

        /// <summary>
        /// <para>The scale of the transform relative to the parent.</para>
        /// </summary>
        public Vector3 localScale
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_localScale(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_localScale(ref value);
            }
        }

        /// <summary>
        /// <para>Matrix that transforms a point from local space into world space (Read Only).</para>
        /// </summary>
        public Matrix4x4 localToWorldMatrix
        {
            get
            {
                Matrix4x4 matrixx;
                this.INTERNAL_get_localToWorldMatrix(out matrixx);
                return matrixx;
            }
        }

        /// <summary>
        /// <para>The global scale of the object (Read Only).</para>
        /// </summary>
        public Vector3 lossyScale
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_lossyScale(out vector);
                return vector;
            }
        }

        /// <summary>
        /// <para>The parent of the transform.</para>
        /// </summary>
        public Transform parent
        {
            get
            {
                return this.parentInternal;
            }
            set
            {
                if (this is RectTransform)
                {
                    Debug.LogWarning("Parent of RectTransform is being set with parent property. Consider using the SetParent method instead, with the worldPositionStays argument set to false. This will retain local orientation and scale rather than world orientation and scale, which can prevent common UI scaling issues.", this);
                }
                this.parentInternal = value;
            }
        }

        internal Transform parentInternal { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The position of the transform in world space.</para>
        /// </summary>
        public Vector3 position
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_position(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_position(ref value);
            }
        }

        /// <summary>
        /// <para>The red axis of the transform in world space.</para>
        /// </summary>
        public Vector3 right
        {
            get
            {
                return (Vector3) (this.rotation * Vector3.right);
            }
            set
            {
                this.rotation = Quaternion.FromToRotation(Vector3.right, value);
            }
        }

        /// <summary>
        /// <para>Returns the topmost transform in the hierarchy.</para>
        /// </summary>
        public Transform root { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>The rotation of the transform in world space stored as a Quaternion.</para>
        /// </summary>
        public Quaternion rotation
        {
            get
            {
                Quaternion quaternion;
                this.INTERNAL_get_rotation(out quaternion);
                return quaternion;
            }
            set
            {
                this.INTERNAL_set_rotation(ref value);
            }
        }

        internal RotationOrder rotationOrder { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The green axis of the transform in world space.</para>
        /// </summary>
        public Vector3 up
        {
            get
            {
                return (Vector3) (this.rotation * Vector3.up);
            }
            set
            {
                this.rotation = Quaternion.FromToRotation(Vector3.up, value);
            }
        }

        /// <summary>
        /// <para>Matrix that transforms a point from world space into local space (Read Only).</para>
        /// </summary>
        public Matrix4x4 worldToLocalMatrix
        {
            get
            {
                Matrix4x4 matrixx;
                this.INTERNAL_get_worldToLocalMatrix(out matrixx);
                return matrixx;
            }
        }

        private sealed class Enumerator : IEnumerator
        {
            private int currentIndex = -1;
            private Transform outer;

            internal Enumerator(Transform outer)
            {
                this.outer = outer;
            }

            public bool MoveNext()
            {
                int childCount = this.outer.childCount;
                return (++this.currentIndex < childCount);
            }

            public void Reset()
            {
                this.currentIndex = -1;
            }

            public object Current
            {
                get
                {
                    return this.outer.GetChild(this.currentIndex);
                }
            }
        }
    }
}

