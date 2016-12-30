namespace UnityEngine
{
    using System;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Representation of 2D vectors and points.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential), UsedByNativeCode]
    public struct Vector2
    {
        /// <summary>
        /// <para>X component of the vector.</para>
        /// </summary>
        public float x;
        /// <summary>
        /// <para>Y component of the vector.</para>
        /// </summary>
        public float y;
        public const float kEpsilon = 1E-05f;
        /// <summary>
        /// <para>Constructs a new vector with given x, y components.</para>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Vector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public float this[int index]
        {
            get
            {
                if (index != 0)
                {
                    if (index != 1)
                    {
                        throw new IndexOutOfRangeException("Invalid Vector2 index!");
                    }
                }
                else
                {
                    return this.x;
                }
                return this.y;
            }
            set
            {
                if (index != 0)
                {
                    if (index != 1)
                    {
                        throw new IndexOutOfRangeException("Invalid Vector2 index!");
                    }
                }
                else
                {
                    this.x = value;
                    return;
                }
                this.y = value;
            }
        }
        /// <summary>
        /// <para>Set x and y components of an existing Vector2.</para>
        /// </summary>
        /// <param name="newX"></param>
        /// <param name="newY"></param>
        public void Set(float newX, float newY)
        {
            this.x = newX;
            this.y = newY;
        }

        /// <summary>
        /// <para>Linearly interpolates between vectors a and b by t.</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="t"></param>
        public static Vector2 Lerp(Vector2 a, Vector2 b, float t)
        {
            t = Mathf.Clamp01(t);
            return new Vector2(a.x + ((b.x - a.x) * t), a.y + ((b.y - a.y) * t));
        }

        /// <summary>
        /// <para>Linearly interpolates between vectors a and b by t.</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="t"></param>
        public static Vector2 LerpUnclamped(Vector2 a, Vector2 b, float t) => 
            new Vector2(a.x + ((b.x - a.x) * t), a.y + ((b.y - a.y) * t));

        /// <summary>
        /// <para>Moves a point current towards target.</para>
        /// </summary>
        /// <param name="current"></param>
        /// <param name="target"></param>
        /// <param name="maxDistanceDelta"></param>
        public static Vector2 MoveTowards(Vector2 current, Vector2 target, float maxDistanceDelta)
        {
            Vector2 vector = target - current;
            float magnitude = vector.magnitude;
            if ((magnitude <= maxDistanceDelta) || (magnitude == 0f))
            {
                return target;
            }
            return (current + ((Vector2) ((vector / magnitude) * maxDistanceDelta)));
        }

        /// <summary>
        /// <para>Multiplies two vectors component-wise.</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static Vector2 Scale(Vector2 a, Vector2 b) => 
            new Vector2(a.x * b.x, a.y * b.y);

        /// <summary>
        /// <para>Multiplies every component of this vector by the same component of scale.</para>
        /// </summary>
        /// <param name="scale"></param>
        public void Scale(Vector2 scale)
        {
            this.x *= scale.x;
            this.y *= scale.y;
        }

        /// <summary>
        /// <para>Makes this vector have a magnitude of 1.</para>
        /// </summary>
        public void Normalize()
        {
            float magnitude = this.magnitude;
            if (magnitude > 1E-05f)
            {
                this = (Vector2) (this / magnitude);
            }
            else
            {
                this = zero;
            }
        }

        /// <summary>
        /// <para>Returns this vector with a magnitude of 1 (Read Only).</para>
        /// </summary>
        public Vector2 normalized
        {
            get
            {
                Vector2 vector = new Vector2(this.x, this.y);
                vector.Normalize();
                return vector;
            }
        }
        /// <summary>
        /// <para>Returns a nicely formatted string for this vector.</para>
        /// </summary>
        /// <param name="format"></param>
        public override string ToString()
        {
            object[] args = new object[] { this.x, this.y };
            return UnityString.Format("({0:F1}, {1:F1})", args);
        }

        /// <summary>
        /// <para>Returns a nicely formatted string for this vector.</para>
        /// </summary>
        /// <param name="format"></param>
        public string ToString(string format)
        {
            object[] args = new object[] { this.x.ToString(format), this.y.ToString(format) };
            return UnityString.Format("({0}, {1})", args);
        }

        public override int GetHashCode() => 
            (this.x.GetHashCode() ^ (this.y.GetHashCode() << 2));

        /// <summary>
        /// <para>Returns true if the given vector is exactly equal to this vector.</para>
        /// </summary>
        /// <param name="other"></param>
        public override bool Equals(object other)
        {
            if (!(other is Vector2))
            {
                return false;
            }
            Vector2 vector = (Vector2) other;
            return (this.x.Equals(vector.x) && this.y.Equals(vector.y));
        }

        /// <summary>
        /// <para>Reflects a vector off the vector defined by a normal.</para>
        /// </summary>
        /// <param name="inDirection"></param>
        /// <param name="inNormal"></param>
        public static Vector2 Reflect(Vector2 inDirection, Vector2 inNormal) => 
            (((Vector2) ((-2f * Dot(inNormal, inDirection)) * inNormal)) + inDirection);

        /// <summary>
        /// <para>Dot Product of two vectors.</para>
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        public static float Dot(Vector2 lhs, Vector2 rhs) => 
            ((lhs.x * rhs.x) + (lhs.y * rhs.y));

        /// <summary>
        /// <para>Returns the length of this vector (Read Only).</para>
        /// </summary>
        public float magnitude =>
            Mathf.Sqrt((this.x * this.x) + (this.y * this.y));
        /// <summary>
        /// <para>Returns the squared length of this vector (Read Only).</para>
        /// </summary>
        public float sqrMagnitude =>
            ((this.x * this.x) + (this.y * this.y));
        /// <summary>
        /// <para>Returns the angle in degrees between from and to.</para>
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public static float Angle(Vector2 from, Vector2 to) => 
            (Mathf.Acos(Mathf.Clamp(Dot(from.normalized, to.normalized), -1f, 1f)) * 57.29578f);

        /// <summary>
        /// <para>Returns the distance between a and b.</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static float Distance(Vector2 a, Vector2 b)
        {
            Vector2 vector = a - b;
            return vector.magnitude;
        }

        /// <summary>
        /// <para>Returns a copy of vector with its magnitude clamped to maxLength.</para>
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="maxLength"></param>
        public static Vector2 ClampMagnitude(Vector2 vector, float maxLength)
        {
            if (vector.sqrMagnitude > (maxLength * maxLength))
            {
                return (Vector2) (vector.normalized * maxLength);
            }
            return vector;
        }

        public static float SqrMagnitude(Vector2 a) => 
            ((a.x * a.x) + (a.y * a.y));

        public float SqrMagnitude() => 
            ((this.x * this.x) + (this.y * this.y));

        /// <summary>
        /// <para>Returns a vector that is made from the smallest components of two vectors.</para>
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        public static Vector2 Min(Vector2 lhs, Vector2 rhs) => 
            new Vector2(Mathf.Min(lhs.x, rhs.x), Mathf.Min(lhs.y, rhs.y));

        /// <summary>
        /// <para>Returns a vector that is made from the largest components of two vectors.</para>
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        public static Vector2 Max(Vector2 lhs, Vector2 rhs) => 
            new Vector2(Mathf.Max(lhs.x, rhs.x), Mathf.Max(lhs.y, rhs.y));

        public static Vector2 SmoothDamp(Vector2 current, Vector2 target, ref Vector2 currentVelocity, float smoothTime, float maxSpeed, float deltaTime)
        {
            smoothTime = Mathf.Max(0.0001f, smoothTime);
            float num = 2f / smoothTime;
            float num2 = num * deltaTime;
            float num3 = 1f / (((1f + num2) + ((0.48f * num2) * num2)) + (((0.235f * num2) * num2) * num2));
            Vector2 vector = current - target;
            Vector2 vector2 = target;
            float maxLength = maxSpeed * smoothTime;
            vector = ClampMagnitude(vector, maxLength);
            target = current - vector;
            Vector2 vector3 = (Vector2) ((currentVelocity + (num * vector)) * deltaTime);
            currentVelocity = (Vector2) ((currentVelocity - (num * vector3)) * num3);
            Vector2 vector4 = target + ((Vector2) ((vector + vector3) * num3));
            if (Dot(vector2 - current, vector4 - vector2) > 0f)
            {
                vector4 = vector2;
                currentVelocity = (Vector2) ((vector4 - vector2) / deltaTime);
            }
            return vector4;
        }

        public static Vector2 operator +(Vector2 a, Vector2 b) => 
            new Vector2(a.x + b.x, a.y + b.y);

        public static Vector2 operator -(Vector2 a, Vector2 b) => 
            new Vector2(a.x - b.x, a.y - b.y);

        public static Vector2 operator -(Vector2 a) => 
            new Vector2(-a.x, -a.y);

        public static Vector2 operator *(Vector2 a, float d) => 
            new Vector2(a.x * d, a.y * d);

        public static Vector2 operator *(float d, Vector2 a) => 
            new Vector2(a.x * d, a.y * d);

        public static Vector2 operator /(Vector2 a, float d) => 
            new Vector2(a.x / d, a.y / d);

        public static bool operator ==(Vector2 lhs, Vector2 rhs)
        {
            Vector2 vector = lhs - rhs;
            return (vector.sqrMagnitude < 9.999999E-11f);
        }

        public static bool operator !=(Vector2 lhs, Vector2 rhs) => 
            !(lhs == rhs);

        public static implicit operator Vector2(Vector3 v) => 
            new Vector2(v.x, v.y);

        public static implicit operator Vector3(Vector2 v) => 
            new Vector3(v.x, v.y, 0f);

        /// <summary>
        /// <para>Shorthand for writing Vector2(0, 0).</para>
        /// </summary>
        public static Vector2 zero =>
            new Vector2(0f, 0f);
        /// <summary>
        /// <para>Shorthand for writing Vector2(1, 1).</para>
        /// </summary>
        public static Vector2 one =>
            new Vector2(1f, 1f);
        /// <summary>
        /// <para>Shorthand for writing Vector2(0, 1).</para>
        /// </summary>
        public static Vector2 up =>
            new Vector2(0f, 1f);
        /// <summary>
        /// <para>Shorthand for writing Vector2(0, -1).</para>
        /// </summary>
        public static Vector2 down =>
            new Vector2(0f, -1f);
        /// <summary>
        /// <para>Shorthand for writing Vector2(-1, 0).</para>
        /// </summary>
        public static Vector2 left =>
            new Vector2(-1f, 0f);
        /// <summary>
        /// <para>Shorthand for writing Vector2(1, 0).</para>
        /// </summary>
        public static Vector2 right =>
            new Vector2(1f, 0f);
    }
}

