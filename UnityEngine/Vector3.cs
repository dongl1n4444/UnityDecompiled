namespace UnityEngine
{
    using System;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Internal;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Representation of 3D vectors and points.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential), UsedByNativeCode]
    public struct Vector3
    {
        public const float kEpsilon = 1E-05f;
        /// <summary>
        /// <para>X component of the vector.</para>
        /// </summary>
        public float x;
        /// <summary>
        /// <para>Y component of the vector.</para>
        /// </summary>
        public float y;
        /// <summary>
        /// <para>Z component of the vector.</para>
        /// </summary>
        public float z;
        /// <summary>
        /// <para>Creates a new vector with given x, y, z components.</para>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public Vector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        /// <summary>
        /// <para>Creates a new vector with given x, y components and sets z to zero.</para>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Vector3(float x, float y)
        {
            this.x = x;
            this.y = y;
            this.z = 0f;
        }

        /// <summary>
        /// <para>Spherically interpolates between two vectors.</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="t"></param>
        [ThreadAndSerializationSafe]
        public static Vector3 Slerp(Vector3 a, Vector3 b, float t)
        {
            Vector3 vector;
            INTERNAL_CALL_Slerp(ref a, ref b, t, out vector);
            return vector;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_Slerp(ref Vector3 a, ref Vector3 b, float t, out Vector3 value);
        /// <summary>
        /// <para>Spherically interpolates between two vectors.</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="t"></param>
        public static Vector3 SlerpUnclamped(Vector3 a, Vector3 b, float t)
        {
            Vector3 vector;
            INTERNAL_CALL_SlerpUnclamped(ref a, ref b, t, out vector);
            return vector;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_SlerpUnclamped(ref Vector3 a, ref Vector3 b, float t, out Vector3 value);
        private static void Internal_OrthoNormalize2(ref Vector3 a, ref Vector3 b)
        {
            INTERNAL_CALL_Internal_OrthoNormalize2(ref a, ref b);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_Internal_OrthoNormalize2(ref Vector3 a, ref Vector3 b);
        private static void Internal_OrthoNormalize3(ref Vector3 a, ref Vector3 b, ref Vector3 c)
        {
            INTERNAL_CALL_Internal_OrthoNormalize3(ref a, ref b, ref c);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_Internal_OrthoNormalize3(ref Vector3 a, ref Vector3 b, ref Vector3 c);
        public static void OrthoNormalize(ref Vector3 normal, ref Vector3 tangent)
        {
            Internal_OrthoNormalize2(ref normal, ref tangent);
        }

        public static void OrthoNormalize(ref Vector3 normal, ref Vector3 tangent, ref Vector3 binormal)
        {
            Internal_OrthoNormalize3(ref normal, ref tangent, ref binormal);
        }

        /// <summary>
        /// <para>Rotates a vector current towards target.</para>
        /// </summary>
        /// <param name="current"></param>
        /// <param name="target"></param>
        /// <param name="maxRadiansDelta"></param>
        /// <param name="maxMagnitudeDelta"></param>
        public static Vector3 RotateTowards(Vector3 current, Vector3 target, float maxRadiansDelta, float maxMagnitudeDelta)
        {
            Vector3 vector;
            INTERNAL_CALL_RotateTowards(ref current, ref target, maxRadiansDelta, maxMagnitudeDelta, out vector);
            return vector;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_RotateTowards(ref Vector3 current, ref Vector3 target, float maxRadiansDelta, float maxMagnitudeDelta, out Vector3 value);
        [Obsolete("Use Vector3.ProjectOnPlane instead.")]
        public static Vector3 Exclude(Vector3 excludeThis, Vector3 fromThat) => 
            (fromThat - Project(fromThat, excludeThis));

        /// <summary>
        /// <para>Linearly interpolates between two vectors.</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="t"></param>
        public static Vector3 Lerp(Vector3 a, Vector3 b, float t)
        {
            t = Mathf.Clamp01(t);
            return new Vector3(a.x + ((b.x - a.x) * t), a.y + ((b.y - a.y) * t), a.z + ((b.z - a.z) * t));
        }

        /// <summary>
        /// <para>Linearly interpolates between two vectors.</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="t"></param>
        public static Vector3 LerpUnclamped(Vector3 a, Vector3 b, float t) => 
            new Vector3(a.x + ((b.x - a.x) * t), a.y + ((b.y - a.y) * t), a.z + ((b.z - a.z) * t));

        /// <summary>
        /// <para>Moves a point current in a straight line towards a target point.</para>
        /// </summary>
        /// <param name="current"></param>
        /// <param name="target"></param>
        /// <param name="maxDistanceDelta"></param>
        public static Vector3 MoveTowards(Vector3 current, Vector3 target, float maxDistanceDelta)
        {
            Vector3 vector = target - current;
            float magnitude = vector.magnitude;
            if ((magnitude <= maxDistanceDelta) || (magnitude == 0f))
            {
                return target;
            }
            return (current + ((Vector3) ((vector / magnitude) * maxDistanceDelta)));
        }

        [ExcludeFromDocs]
        public static Vector3 SmoothDamp(Vector3 current, Vector3 target, ref Vector3 currentVelocity, float smoothTime, float maxSpeed)
        {
            float deltaTime = Time.deltaTime;
            return SmoothDamp(current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime);
        }

        [ExcludeFromDocs]
        public static Vector3 SmoothDamp(Vector3 current, Vector3 target, ref Vector3 currentVelocity, float smoothTime)
        {
            float deltaTime = Time.deltaTime;
            float positiveInfinity = float.PositiveInfinity;
            return SmoothDamp(current, target, ref currentVelocity, smoothTime, positiveInfinity, deltaTime);
        }

        public static Vector3 SmoothDamp(Vector3 current, Vector3 target, ref Vector3 currentVelocity, float smoothTime, [DefaultValue("Mathf.Infinity")] float maxSpeed, [DefaultValue("Time.deltaTime")] float deltaTime)
        {
            smoothTime = Mathf.Max(0.0001f, smoothTime);
            float num = 2f / smoothTime;
            float num2 = num * deltaTime;
            float num3 = 1f / (((1f + num2) + ((0.48f * num2) * num2)) + (((0.235f * num2) * num2) * num2));
            Vector3 vector = current - target;
            Vector3 vector2 = target;
            float maxLength = maxSpeed * smoothTime;
            vector = ClampMagnitude(vector, maxLength);
            target = current - vector;
            Vector3 vector3 = (Vector3) ((currentVelocity + (num * vector)) * deltaTime);
            currentVelocity = (Vector3) ((currentVelocity - (num * vector3)) * num3);
            Vector3 vector4 = target + ((Vector3) ((vector + vector3) * num3));
            if (Dot(vector2 - current, vector4 - vector2) > 0f)
            {
                vector4 = vector2;
                currentVelocity = (Vector3) ((vector4 - vector2) / deltaTime);
            }
            return vector4;
        }

        public float this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return this.x;

                    case 1:
                        return this.y;

                    case 2:
                        return this.z;
                }
                throw new IndexOutOfRangeException("Invalid Vector3 index!");
            }
            set
            {
                switch (index)
                {
                    case 0:
                        this.x = value;
                        break;

                    case 1:
                        this.y = value;
                        break;

                    case 2:
                        this.z = value;
                        break;

                    default:
                        throw new IndexOutOfRangeException("Invalid Vector3 index!");
                }
            }
        }
        /// <summary>
        /// <para>Set x, y and z components of an existing Vector3.</para>
        /// </summary>
        /// <param name="new_x"></param>
        /// <param name="new_y"></param>
        /// <param name="new_z"></param>
        public void Set(float new_x, float new_y, float new_z)
        {
            this.x = new_x;
            this.y = new_y;
            this.z = new_z;
        }

        /// <summary>
        /// <para>Multiplies two vectors component-wise.</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static Vector3 Scale(Vector3 a, Vector3 b) => 
            new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);

        /// <summary>
        /// <para>Multiplies every component of this vector by the same component of scale.</para>
        /// </summary>
        /// <param name="scale"></param>
        public void Scale(Vector3 scale)
        {
            this.x *= scale.x;
            this.y *= scale.y;
            this.z *= scale.z;
        }

        /// <summary>
        /// <para>Cross Product of two vectors.</para>
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        public static Vector3 Cross(Vector3 lhs, Vector3 rhs) => 
            new Vector3((lhs.y * rhs.z) - (lhs.z * rhs.y), (lhs.z * rhs.x) - (lhs.x * rhs.z), (lhs.x * rhs.y) - (lhs.y * rhs.x));

        public override int GetHashCode() => 
            ((this.x.GetHashCode() ^ (this.y.GetHashCode() << 2)) ^ (this.z.GetHashCode() >> 2));

        public override bool Equals(object other)
        {
            if (!(other is Vector3))
            {
                return false;
            }
            Vector3 vector = (Vector3) other;
            return ((this.x.Equals(vector.x) && this.y.Equals(vector.y)) && this.z.Equals(vector.z));
        }

        /// <summary>
        /// <para>Reflects a vector off the plane defined by a normal.</para>
        /// </summary>
        /// <param name="inDirection"></param>
        /// <param name="inNormal"></param>
        public static Vector3 Reflect(Vector3 inDirection, Vector3 inNormal) => 
            (((Vector3) ((-2f * Dot(inNormal, inDirection)) * inNormal)) + inDirection);

        /// <summary>
        /// <para></para>
        /// </summary>
        /// <param name="value"></param>
        public static Vector3 Normalize(Vector3 value)
        {
            float num = Magnitude(value);
            if (num > 1E-05f)
            {
                return (Vector3) (value / num);
            }
            return zero;
        }

        /// <summary>
        /// <para>Makes this vector have a magnitude of 1.</para>
        /// </summary>
        public void Normalize()
        {
            float num = Magnitude(this);
            if (num > 1E-05f)
            {
                this = (Vector3) (this / num);
            }
            else
            {
                this = zero;
            }
        }

        /// <summary>
        /// <para>Returns this vector with a magnitude of 1 (Read Only).</para>
        /// </summary>
        public Vector3 normalized =>
            Normalize(this);
        /// <summary>
        /// <para>Dot Product of two vectors.</para>
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        public static float Dot(Vector3 lhs, Vector3 rhs) => 
            (((lhs.x * rhs.x) + (lhs.y * rhs.y)) + (lhs.z * rhs.z));

        /// <summary>
        /// <para>Projects a vector onto another vector.</para>
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="onNormal"></param>
        public static Vector3 Project(Vector3 vector, Vector3 onNormal)
        {
            float num = Dot(onNormal, onNormal);
            if (num < Mathf.Epsilon)
            {
                return zero;
            }
            return (Vector3) ((onNormal * Dot(vector, onNormal)) / num);
        }

        /// <summary>
        /// <para>Projects a vector onto a plane defined by a normal orthogonal to the plane.</para>
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="planeNormal"></param>
        public static Vector3 ProjectOnPlane(Vector3 vector, Vector3 planeNormal) => 
            (vector - Project(vector, planeNormal));

        /// <summary>
        /// <para>Returns the angle in degrees between from and to.</para>
        /// </summary>
        /// <param name="from">The angle extends round from this vector.</param>
        /// <param name="to">The angle extends round to this vector.</param>
        public static float Angle(Vector3 from, Vector3 to) => 
            (Mathf.Acos(Mathf.Clamp(Dot(from.normalized, to.normalized), -1f, 1f)) * 57.29578f);

        /// <summary>
        /// <para>Returns the distance between a and b.</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static float Distance(Vector3 a, Vector3 b)
        {
            Vector3 vector = new Vector3(a.x - b.x, a.y - b.y, a.z - b.z);
            return Mathf.Sqrt(((vector.x * vector.x) + (vector.y * vector.y)) + (vector.z * vector.z));
        }

        /// <summary>
        /// <para>Returns a copy of vector with its magnitude clamped to maxLength.</para>
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="maxLength"></param>
        public static Vector3 ClampMagnitude(Vector3 vector, float maxLength)
        {
            if (vector.sqrMagnitude > (maxLength * maxLength))
            {
                return (Vector3) (vector.normalized * maxLength);
            }
            return vector;
        }

        public static float Magnitude(Vector3 a) => 
            Mathf.Sqrt(((a.x * a.x) + (a.y * a.y)) + (a.z * a.z));

        /// <summary>
        /// <para>Returns the length of this vector (Read Only).</para>
        /// </summary>
        public float magnitude =>
            Mathf.Sqrt(((this.x * this.x) + (this.y * this.y)) + (this.z * this.z));
        public static float SqrMagnitude(Vector3 a) => 
            (((a.x * a.x) + (a.y * a.y)) + (a.z * a.z));

        /// <summary>
        /// <para>Returns the squared length of this vector (Read Only).</para>
        /// </summary>
        public float sqrMagnitude =>
            (((this.x * this.x) + (this.y * this.y)) + (this.z * this.z));
        /// <summary>
        /// <para>Returns a vector that is made from the smallest components of two vectors.</para>
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        public static Vector3 Min(Vector3 lhs, Vector3 rhs) => 
            new Vector3(Mathf.Min(lhs.x, rhs.x), Mathf.Min(lhs.y, rhs.y), Mathf.Min(lhs.z, rhs.z));

        /// <summary>
        /// <para>Returns a vector that is made from the largest components of two vectors.</para>
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        public static Vector3 Max(Vector3 lhs, Vector3 rhs) => 
            new Vector3(Mathf.Max(lhs.x, rhs.x), Mathf.Max(lhs.y, rhs.y), Mathf.Max(lhs.z, rhs.z));

        /// <summary>
        /// <para>Shorthand for writing Vector3(0, 0, 0).</para>
        /// </summary>
        public static Vector3 zero =>
            new Vector3(0f, 0f, 0f);
        /// <summary>
        /// <para>Shorthand for writing Vector3(1, 1, 1).</para>
        /// </summary>
        public static Vector3 one =>
            new Vector3(1f, 1f, 1f);
        /// <summary>
        /// <para>Shorthand for writing Vector3(0, 0, 1).</para>
        /// </summary>
        public static Vector3 forward =>
            new Vector3(0f, 0f, 1f);
        /// <summary>
        /// <para>Shorthand for writing Vector3(0, 0, -1).</para>
        /// </summary>
        public static Vector3 back =>
            new Vector3(0f, 0f, -1f);
        /// <summary>
        /// <para>Shorthand for writing Vector3(0, 1, 0).</para>
        /// </summary>
        public static Vector3 up =>
            new Vector3(0f, 1f, 0f);
        /// <summary>
        /// <para>Shorthand for writing Vector3(0, -1, 0).</para>
        /// </summary>
        public static Vector3 down =>
            new Vector3(0f, -1f, 0f);
        /// <summary>
        /// <para>Shorthand for writing Vector3(-1, 0, 0).</para>
        /// </summary>
        public static Vector3 left =>
            new Vector3(-1f, 0f, 0f);
        /// <summary>
        /// <para>Shorthand for writing Vector3(1, 0, 0).</para>
        /// </summary>
        public static Vector3 right =>
            new Vector3(1f, 0f, 0f);
        public static Vector3 operator +(Vector3 a, Vector3 b) => 
            new Vector3(a.x + b.x, a.y + b.y, a.z + b.z);

        public static Vector3 operator -(Vector3 a, Vector3 b) => 
            new Vector3(a.x - b.x, a.y - b.y, a.z - b.z);

        public static Vector3 operator -(Vector3 a) => 
            new Vector3(-a.x, -a.y, -a.z);

        public static Vector3 operator *(Vector3 a, float d) => 
            new Vector3(a.x * d, a.y * d, a.z * d);

        public static Vector3 operator *(float d, Vector3 a) => 
            new Vector3(a.x * d, a.y * d, a.z * d);

        public static Vector3 operator /(Vector3 a, float d) => 
            new Vector3(a.x / d, a.y / d, a.z / d);

        public static bool operator ==(Vector3 lhs, Vector3 rhs) => 
            (SqrMagnitude(lhs - rhs) < 9.999999E-11f);

        public static bool operator !=(Vector3 lhs, Vector3 rhs) => 
            (SqrMagnitude(lhs - rhs) >= 9.999999E-11f);

        /// <summary>
        /// <para>Returns a nicely formatted string for this vector.</para>
        /// </summary>
        /// <param name="format"></param>
        public override string ToString()
        {
            object[] args = new object[] { this.x, this.y, this.z };
            return UnityString.Format("({0:F1}, {1:F1}, {2:F1})", args);
        }

        /// <summary>
        /// <para>Returns a nicely formatted string for this vector.</para>
        /// </summary>
        /// <param name="format"></param>
        public string ToString(string format)
        {
            object[] args = new object[] { this.x.ToString(format), this.y.ToString(format), this.z.ToString(format) };
            return UnityString.Format("({0}, {1}, {2})", args);
        }

        [Obsolete("Use Vector3.forward instead.")]
        public static Vector3 fwd =>
            new Vector3(0f, 0f, 1f);
        [Obsolete("Use Vector3.Angle instead. AngleBetween uses radians instead of degrees and was deprecated for this reason")]
        public static float AngleBetween(Vector3 from, Vector3 to) => 
            Mathf.Acos(Mathf.Clamp(Dot(from.normalized, to.normalized), -1f, 1f));
    }
}

