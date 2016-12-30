﻿namespace UnityEngine
{
    using System;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Representation of four-dimensional vectors.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential), UsedByNativeCode]
    public struct Vector4
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
        /// <para>W component of the vector.</para>
        /// </summary>
        public float w;
        /// <summary>
        /// <para>Creates a new vector with given x, y, z, w components.</para>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="w"></param>
        public Vector4(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        /// <summary>
        /// <para>Creates a new vector with given x, y, z components and sets w to zero.</para>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public Vector4(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = 0f;
        }

        /// <summary>
        /// <para>Creates a new vector with given x, y components and sets z and w to zero.</para>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Vector4(float x, float y)
        {
            this.x = x;
            this.y = y;
            this.z = 0f;
            this.w = 0f;
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

                    case 3:
                        return this.w;
                }
                throw new IndexOutOfRangeException("Invalid Vector4 index!");
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

                    case 3:
                        this.w = value;
                        break;

                    default:
                        throw new IndexOutOfRangeException("Invalid Vector4 index!");
                }
            }
        }
        /// <summary>
        /// <para>Set x, y, z and w components of an existing Vector4.</para>
        /// </summary>
        /// <param name="new_x"></param>
        /// <param name="new_y"></param>
        /// <param name="new_z"></param>
        /// <param name="new_w"></param>
        public void Set(float new_x, float new_y, float new_z, float new_w)
        {
            this.x = new_x;
            this.y = new_y;
            this.z = new_z;
            this.w = new_w;
        }

        /// <summary>
        /// <para>Linearly interpolates between two vectors.</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="t"></param>
        public static Vector4 Lerp(Vector4 a, Vector4 b, float t)
        {
            t = Mathf.Clamp01(t);
            return new Vector4(a.x + ((b.x - a.x) * t), a.y + ((b.y - a.y) * t), a.z + ((b.z - a.z) * t), a.w + ((b.w - a.w) * t));
        }

        /// <summary>
        /// <para>Linearly interpolates between two vectors.</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="t"></param>
        public static Vector4 LerpUnclamped(Vector4 a, Vector4 b, float t) => 
            new Vector4(a.x + ((b.x - a.x) * t), a.y + ((b.y - a.y) * t), a.z + ((b.z - a.z) * t), a.w + ((b.w - a.w) * t));

        /// <summary>
        /// <para>Moves a point current towards target.</para>
        /// </summary>
        /// <param name="current"></param>
        /// <param name="target"></param>
        /// <param name="maxDistanceDelta"></param>
        public static Vector4 MoveTowards(Vector4 current, Vector4 target, float maxDistanceDelta)
        {
            Vector4 vector = target - current;
            float magnitude = vector.magnitude;
            if ((magnitude <= maxDistanceDelta) || (magnitude == 0f))
            {
                return target;
            }
            return (current + ((Vector4) ((vector / magnitude) * maxDistanceDelta)));
        }

        /// <summary>
        /// <para>Multiplies two vectors component-wise.</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static Vector4 Scale(Vector4 a, Vector4 b) => 
            new Vector4(a.x * b.x, a.y * b.y, a.z * b.z, a.w * b.w);

        /// <summary>
        /// <para>Multiplies every component of this vector by the same component of scale.</para>
        /// </summary>
        /// <param name="scale"></param>
        public void Scale(Vector4 scale)
        {
            this.x *= scale.x;
            this.y *= scale.y;
            this.z *= scale.z;
            this.w *= scale.w;
        }

        public override int GetHashCode() => 
            (((this.x.GetHashCode() ^ (this.y.GetHashCode() << 2)) ^ (this.z.GetHashCode() >> 2)) ^ (this.w.GetHashCode() >> 1));

        /// <summary>
        /// <para>Returns true if the given vector is exactly equal to this vector.</para>
        /// </summary>
        /// <param name="other"></param>
        public override bool Equals(object other)
        {
            if (!(other is Vector4))
            {
                return false;
            }
            Vector4 vector = (Vector4) other;
            return (((this.x.Equals(vector.x) && this.y.Equals(vector.y)) && this.z.Equals(vector.z)) && this.w.Equals(vector.w));
        }

        /// <summary>
        /// <para></para>
        /// </summary>
        /// <param name="a"></param>
        public static Vector4 Normalize(Vector4 a)
        {
            float num = Magnitude(a);
            if (num > 1E-05f)
            {
                return (Vector4) (a / num);
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
                this = (Vector4) (this / num);
            }
            else
            {
                this = zero;
            }
        }

        /// <summary>
        /// <para>Returns this vector with a magnitude of 1 (Read Only).</para>
        /// </summary>
        public Vector4 normalized =>
            Normalize(this);
        /// <summary>
        /// <para>Dot Product of two vectors.</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static float Dot(Vector4 a, Vector4 b) => 
            ((((a.x * b.x) + (a.y * b.y)) + (a.z * b.z)) + (a.w * b.w));

        /// <summary>
        /// <para>Projects a vector onto another vector.</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static Vector4 Project(Vector4 a, Vector4 b) => 
            ((Vector4) ((b * Dot(a, b)) / Dot(b, b)));

        /// <summary>
        /// <para>Returns the distance between a and b.</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static float Distance(Vector4 a, Vector4 b) => 
            Magnitude(a - b);

        public static float Magnitude(Vector4 a) => 
            Mathf.Sqrt(Dot(a, a));

        /// <summary>
        /// <para>Returns the length of this vector (Read Only).</para>
        /// </summary>
        public float magnitude =>
            Mathf.Sqrt(Dot(this, this));
        /// <summary>
        /// <para>Returns the squared length of this vector (Read Only).</para>
        /// </summary>
        public float sqrMagnitude =>
            Dot(this, this);
        /// <summary>
        /// <para>Returns a vector that is made from the smallest components of two vectors.</para>
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        public static Vector4 Min(Vector4 lhs, Vector4 rhs) => 
            new Vector4(Mathf.Min(lhs.x, rhs.x), Mathf.Min(lhs.y, rhs.y), Mathf.Min(lhs.z, rhs.z), Mathf.Min(lhs.w, rhs.w));

        /// <summary>
        /// <para>Returns a vector that is made from the largest components of two vectors.</para>
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        public static Vector4 Max(Vector4 lhs, Vector4 rhs) => 
            new Vector4(Mathf.Max(lhs.x, rhs.x), Mathf.Max(lhs.y, rhs.y), Mathf.Max(lhs.z, rhs.z), Mathf.Max(lhs.w, rhs.w));

        /// <summary>
        /// <para>Shorthand for writing Vector4(0,0,0,0).</para>
        /// </summary>
        public static Vector4 zero =>
            new Vector4(0f, 0f, 0f, 0f);
        /// <summary>
        /// <para>Shorthand for writing Vector4(1,1,1,1).</para>
        /// </summary>
        public static Vector4 one =>
            new Vector4(1f, 1f, 1f, 1f);
        public static Vector4 operator +(Vector4 a, Vector4 b) => 
            new Vector4(a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w);

        public static Vector4 operator -(Vector4 a, Vector4 b) => 
            new Vector4(a.x - b.x, a.y - b.y, a.z - b.z, a.w - b.w);

        public static Vector4 operator -(Vector4 a) => 
            new Vector4(-a.x, -a.y, -a.z, -a.w);

        public static Vector4 operator *(Vector4 a, float d) => 
            new Vector4(a.x * d, a.y * d, a.z * d, a.w * d);

        public static Vector4 operator *(float d, Vector4 a) => 
            new Vector4(a.x * d, a.y * d, a.z * d, a.w * d);

        public static Vector4 operator /(Vector4 a, float d) => 
            new Vector4(a.x / d, a.y / d, a.z / d, a.w / d);

        public static bool operator ==(Vector4 lhs, Vector4 rhs) => 
            (SqrMagnitude(lhs - rhs) < 9.999999E-11f);

        public static bool operator !=(Vector4 lhs, Vector4 rhs) => 
            !(lhs == rhs);

        public static implicit operator Vector4(Vector3 v) => 
            new Vector4(v.x, v.y, v.z, 0f);

        public static implicit operator Vector3(Vector4 v) => 
            new Vector3(v.x, v.y, v.z);

        public static implicit operator Vector4(Vector2 v) => 
            new Vector4(v.x, v.y, 0f, 0f);

        public static implicit operator Vector2(Vector4 v) => 
            new Vector2(v.x, v.y);

        /// <summary>
        /// <para>Returns a nicely formatted string for this vector.</para>
        /// </summary>
        /// <param name="format"></param>
        public override string ToString()
        {
            object[] args = new object[] { this.x, this.y, this.z, this.w };
            return UnityString.Format("({0:F1}, {1:F1}, {2:F1}, {3:F1})", args);
        }

        /// <summary>
        /// <para>Returns a nicely formatted string for this vector.</para>
        /// </summary>
        /// <param name="format"></param>
        public string ToString(string format)
        {
            object[] args = new object[] { this.x.ToString(format), this.y.ToString(format), this.z.ToString(format), this.w.ToString(format) };
            return UnityString.Format("({0}, {1}, {2}, {3})", args);
        }

        public static float SqrMagnitude(Vector4 a) => 
            Dot(a, a);

        public float SqrMagnitude() => 
            Dot(this, this);
    }
}

