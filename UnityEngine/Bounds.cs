namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Represents an axis aligned bounding box.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential), UsedByNativeCode]
    public struct Bounds
    {
        private Vector3 m_Center;
        private Vector3 m_Extents;
        /// <summary>
        /// <para>Creates new Bounds with a given center and total size. Bound extents will be half the given size.</para>
        /// </summary>
        /// <param name="center"></param>
        /// <param name="size"></param>
        public Bounds(Vector3 center, Vector3 size)
        {
            this.m_Center = center;
            this.m_Extents = (Vector3) (size * 0.5f);
        }

        [ThreadAndSerializationSafe]
        private static bool Internal_Contains(Bounds m, Vector3 point) => 
            INTERNAL_CALL_Internal_Contains(ref m, ref point);

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern bool INTERNAL_CALL_Internal_Contains(ref Bounds m, ref Vector3 point);
        /// <summary>
        /// <para>Is point contained in the bounding box?</para>
        /// </summary>
        /// <param name="point"></param>
        public bool Contains(Vector3 point) => 
            Internal_Contains(this, point);

        private static float Internal_SqrDistance(Bounds m, Vector3 point) => 
            INTERNAL_CALL_Internal_SqrDistance(ref m, ref point);

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern float INTERNAL_CALL_Internal_SqrDistance(ref Bounds m, ref Vector3 point);
        /// <summary>
        /// <para>The smallest squared distance between the point and this bounding box.</para>
        /// </summary>
        /// <param name="point"></param>
        public float SqrDistance(Vector3 point) => 
            Internal_SqrDistance(this, point);

        private static bool Internal_IntersectRay(ref Ray ray, ref Bounds bounds, out float distance) => 
            INTERNAL_CALL_Internal_IntersectRay(ref ray, ref bounds, out distance);

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern bool INTERNAL_CALL_Internal_IntersectRay(ref Ray ray, ref Bounds bounds, out float distance);
        /// <summary>
        /// <para>Does ray intersect this bounding box?</para>
        /// </summary>
        /// <param name="ray"></param>
        public bool IntersectRay(Ray ray)
        {
            float num;
            return Internal_IntersectRay(ref ray, ref this, out num);
        }

        public bool IntersectRay(Ray ray, out float distance) => 
            Internal_IntersectRay(ref ray, ref this, out distance);

        private static Vector3 Internal_GetClosestPoint(ref Bounds bounds, ref Vector3 point)
        {
            Vector3 vector;
            INTERNAL_CALL_Internal_GetClosestPoint(ref bounds, ref point, out vector);
            return vector;
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_Internal_GetClosestPoint(ref Bounds bounds, ref Vector3 point, out Vector3 value);
        /// <summary>
        /// <para>The closest point on the bounding box.</para>
        /// </summary>
        /// <param name="point">Arbitrary point.</param>
        /// <returns>
        /// <para>The point on the bounding box or inside the bounding box.</para>
        /// </returns>
        public Vector3 ClosestPoint(Vector3 point) => 
            Internal_GetClosestPoint(ref this, ref point);

        public override int GetHashCode() => 
            (this.center.GetHashCode() ^ (this.extents.GetHashCode() << 2));

        public override bool Equals(object other)
        {
            if (!(other is Bounds))
            {
                return false;
            }
            Bounds bounds = (Bounds) other;
            return (this.center.Equals(bounds.center) && this.extents.Equals(bounds.extents));
        }

        /// <summary>
        /// <para>The center of the bounding box.</para>
        /// </summary>
        public Vector3 center
        {
            get => 
                this.m_Center;
            set
            {
                this.m_Center = value;
            }
        }
        /// <summary>
        /// <para>The total size of the box. This is always twice as large as the extents.</para>
        /// </summary>
        public Vector3 size
        {
            get => 
                ((Vector3) (this.m_Extents * 2f));
            set
            {
                this.m_Extents = (Vector3) (value * 0.5f);
            }
        }
        /// <summary>
        /// <para>The extents of the box. This is always half of the size.</para>
        /// </summary>
        public Vector3 extents
        {
            get => 
                this.m_Extents;
            set
            {
                this.m_Extents = value;
            }
        }
        /// <summary>
        /// <para>The minimal point of the box. This is always equal to center-extents.</para>
        /// </summary>
        public Vector3 min
        {
            get => 
                (this.center - this.extents);
            set
            {
                this.SetMinMax(value, this.max);
            }
        }
        /// <summary>
        /// <para>The maximal point of the box. This is always equal to center+extents.</para>
        /// </summary>
        public Vector3 max
        {
            get => 
                (this.center + this.extents);
            set
            {
                this.SetMinMax(this.min, value);
            }
        }
        public static bool operator ==(Bounds lhs, Bounds rhs) => 
            ((lhs.center == rhs.center) && (lhs.extents == rhs.extents));

        public static bool operator !=(Bounds lhs, Bounds rhs) => 
            !(lhs == rhs);

        /// <summary>
        /// <para>Sets the bounds to the min and max value of the box.</para>
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public void SetMinMax(Vector3 min, Vector3 max)
        {
            this.extents = (Vector3) ((max - min) * 0.5f);
            this.center = min + this.extents;
        }

        /// <summary>
        /// <para>Grows the Bounds to include the point.</para>
        /// </summary>
        /// <param name="point"></param>
        public void Encapsulate(Vector3 point)
        {
            this.SetMinMax(Vector3.Min(this.min, point), Vector3.Max(this.max, point));
        }

        /// <summary>
        /// <para>Grow the bounds to encapsulate the bounds.</para>
        /// </summary>
        /// <param name="bounds"></param>
        public void Encapsulate(Bounds bounds)
        {
            this.Encapsulate(bounds.center - bounds.extents);
            this.Encapsulate(bounds.center + bounds.extents);
        }

        /// <summary>
        /// <para>Expand the bounds by increasing its size by amount along each side.</para>
        /// </summary>
        /// <param name="amount"></param>
        public void Expand(float amount)
        {
            amount *= 0.5f;
            this.extents += new Vector3(amount, amount, amount);
        }

        /// <summary>
        /// <para>Expand the bounds by increasing its size by amount along each side.</para>
        /// </summary>
        /// <param name="amount"></param>
        public void Expand(Vector3 amount)
        {
            this.extents += (Vector3) (amount * 0.5f);
        }

        /// <summary>
        /// <para>Does another bounding box intersect with this bounding box?</para>
        /// </summary>
        /// <param name="bounds"></param>
        public bool Intersects(Bounds bounds) => 
            (((((this.min.x <= bounds.max.x) && (this.max.x >= bounds.min.x)) && ((this.min.y <= bounds.max.y) && (this.max.y >= bounds.min.y))) && (this.min.z <= bounds.max.z)) && (this.max.z >= bounds.min.z));

        /// <summary>
        /// <para>Returns a nicely formatted string for the bounds.</para>
        /// </summary>
        /// <param name="format"></param>
        public override string ToString()
        {
            object[] args = new object[] { this.m_Center, this.m_Extents };
            return UnityString.Format("Center: {0}, Extents: {1}", args);
        }

        /// <summary>
        /// <para>Returns a nicely formatted string for the bounds.</para>
        /// </summary>
        /// <param name="format"></param>
        public string ToString(string format)
        {
            object[] args = new object[] { this.m_Center.ToString(format), this.m_Extents.ToString(format) };
            return UnityString.Format("Center: {0}, Extents: {1}", args);
        }
    }
}

