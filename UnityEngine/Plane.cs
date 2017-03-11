namespace UnityEngine
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Representation of a plane in 3D space.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential), UsedByNativeCode]
    public struct Plane
    {
        private Vector3 m_Normal;
        private float m_Distance;
        /// <summary>
        /// <para>Creates a plane.</para>
        /// </summary>
        /// <param name="inNormal"></param>
        /// <param name="inPoint"></param>
        public Plane(Vector3 inNormal, Vector3 inPoint)
        {
            this.m_Normal = Vector3.Normalize(inNormal);
            this.m_Distance = -Vector3.Dot(inNormal, inPoint);
        }

        /// <summary>
        /// <para>Creates a plane.</para>
        /// </summary>
        /// <param name="inNormal"></param>
        /// <param name="d"></param>
        public Plane(Vector3 inNormal, float d)
        {
            this.m_Normal = Vector3.Normalize(inNormal);
            this.m_Distance = d;
        }

        /// <summary>
        /// <para>Creates a plane.</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        public Plane(Vector3 a, Vector3 b, Vector3 c)
        {
            this.m_Normal = Vector3.Normalize(Vector3.Cross(b - a, c - a));
            this.m_Distance = -Vector3.Dot(this.m_Normal, a);
        }

        /// <summary>
        /// <para>Normal vector of the plane.</para>
        /// </summary>
        public Vector3 normal
        {
            get => 
                this.m_Normal;
            set
            {
                this.m_Normal = value;
            }
        }
        /// <summary>
        /// <para>Distance from the origin to the plane.</para>
        /// </summary>
        public float distance
        {
            get => 
                this.m_Distance;
            set
            {
                this.m_Distance = value;
            }
        }
        /// <summary>
        /// <para>Sets a plane using a point that lies within it along with a normal to orient it.</para>
        /// </summary>
        /// <param name="inNormal">The plane's normal vector.</param>
        /// <param name="inPoint">A point that lies on the plane.</param>
        public void SetNormalAndPosition(Vector3 inNormal, Vector3 inPoint)
        {
            this.m_Normal = Vector3.Normalize(inNormal);
            this.m_Distance = -Vector3.Dot(inNormal, inPoint);
        }

        /// <summary>
        /// <para>Sets a plane using three points that lie within it.  The points go around clockwise as you look down on the top surface of the plane.</para>
        /// </summary>
        /// <param name="a">First point in clockwise order.</param>
        /// <param name="b">Second point in clockwise order.</param>
        /// <param name="c">Third point in clockwise order.</param>
        public void Set3Points(Vector3 a, Vector3 b, Vector3 c)
        {
            this.m_Normal = Vector3.Normalize(Vector3.Cross(b - a, c - a));
            this.m_Distance = -Vector3.Dot(this.m_Normal, a);
        }

        /// <summary>
        /// <para>Makes the plane face in the opposite direction.</para>
        /// </summary>
        public void Flip()
        {
            this.m_Normal = -this.m_Normal;
            this.m_Distance = -this.m_Distance;
        }

        /// <summary>
        /// <para>Returns a copy of the plane that faces in the opposite direction.</para>
        /// </summary>
        public Plane flipped =>
            new Plane(-this.m_Normal, -this.m_Distance);
        /// <summary>
        /// <para>Moves the plane in space by the translation vector.</para>
        /// </summary>
        /// <param name="translation">The offset in space to move the plane with.</param>
        public void Translate(Vector3 translation)
        {
            this.m_Distance += Vector3.Dot(this.m_Normal, translation);
        }

        /// <summary>
        /// <para>Returns a copy of the given plane that is moved in space by the given translation.</para>
        /// </summary>
        /// <param name="plane">The plane to move in space.</param>
        /// <param name="translation">The offset in space to move the plane with.</param>
        /// <returns>
        /// <para>The translated plane.</para>
        /// </returns>
        public static Plane Translate(Plane plane, Vector3 translation) => 
            new Plane(plane.m_Normal, plane.m_Distance += Vector3.Dot(plane.m_Normal, translation));

        /// <summary>
        /// <para>For a given point returns the closest point on the plane.</para>
        /// </summary>
        /// <param name="point">The point to project onto the plane.</param>
        /// <returns>
        /// <para>A point on the plane that is closest to point.</para>
        /// </returns>
        public Vector3 ClosestPointOnPlane(Vector3 point)
        {
            float num = Vector3.Dot(this.m_Normal, point) + this.m_Distance;
            return (point - ((Vector3) (this.m_Normal * num)));
        }

        /// <summary>
        /// <para>Returns a signed distance from plane to point.</para>
        /// </summary>
        /// <param name="point"></param>
        public float GetDistanceToPoint(Vector3 point) => 
            (Vector3.Dot(this.m_Normal, point) + this.m_Distance);

        /// <summary>
        /// <para>Is a point on the positive side of the plane?</para>
        /// </summary>
        /// <param name="point"></param>
        public bool GetSide(Vector3 point) => 
            ((Vector3.Dot(this.m_Normal, point) + this.m_Distance) > 0f);

        /// <summary>
        /// <para>Are two points on the same side of the plane?</para>
        /// </summary>
        /// <param name="inPt0"></param>
        /// <param name="inPt1"></param>
        public bool SameSide(Vector3 inPt0, Vector3 inPt1)
        {
            float distanceToPoint = this.GetDistanceToPoint(inPt0);
            float num2 = this.GetDistanceToPoint(inPt1);
            return (((distanceToPoint > 0f) && (num2 > 0f)) || ((distanceToPoint <= 0f) && (num2 <= 0f)));
        }

        public bool Raycast(Ray ray, out float enter)
        {
            float a = Vector3.Dot(ray.direction, this.m_Normal);
            float num2 = -Vector3.Dot(ray.origin, this.m_Normal) - this.m_Distance;
            if (Mathf.Approximately(a, 0f))
            {
                enter = 0f;
                return false;
            }
            enter = num2 / a;
            return (enter > 0f);
        }

        public override string ToString()
        {
            object[] args = new object[] { this.m_Normal.x, this.m_Normal.y, this.m_Normal.z, this.m_Distance };
            return UnityString.Format("(normal:({0:F1}, {1:F1}, {2:F1}), distance:{3:F1})", args);
        }

        public string ToString(string format)
        {
            object[] args = new object[] { this.m_Normal.x.ToString(format), this.m_Normal.y.ToString(format), this.m_Normal.z.ToString(format), this.m_Distance.ToString(format) };
            return UnityString.Format("(normal:({0}, {1}, {2}), distance:{3})", args);
        }
    }
}

