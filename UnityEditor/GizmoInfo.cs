namespace UnityEditor
{
    using System;
    using UnityEngine;

    [Serializable]
    internal class GizmoInfo
    {
        [SerializeField]
        private float m_Angle = 0f;
        [SerializeField]
        private Vector2 m_Center = new Vector2(0f, 0f);
        [SerializeField]
        private float m_Length = 0.2f;
        [SerializeField]
        private Vector4 m_Plane;
        [SerializeField]
        private Vector4 m_PlaneOrtho;
        [SerializeField]
        private Vector2 m_Point1;
        [SerializeField]
        private Vector2 m_Point2;

        public GizmoInfo()
        {
            this.Update(this.m_Center, this.m_Length, this.m_Angle);
        }

        private Vector4 Get2DPlane(Vector2 firstPoint, float angle)
        {
            Vector4 vector = new Vector4();
            angle = angle % 6.283185f;
            Vector2 vector2 = new Vector2(firstPoint.x + Mathf.Sin(angle), firstPoint.y + Mathf.Cos(angle));
            Vector2 vector3 = vector2 - firstPoint;
            if (Mathf.Abs(vector3.x) < 1E-05)
            {
                vector.Set(-1f, 0f, firstPoint.x, 0f);
                float num = (Mathf.Cos(angle) <= 0f) ? -1f : 1f;
                vector = (Vector4) (vector * num);
            }
            else
            {
                float num2 = vector3.y / vector3.x;
                vector.Set(-num2, 1f, -(firstPoint.y - (num2 * firstPoint.x)), 0f);
            }
            if (angle > 3.141593f)
            {
                vector = -vector;
            }
            float num3 = Mathf.Sqrt((vector.x * vector.x) + (vector.y * vector.y));
            return (Vector4) (vector / num3);
        }

        public void Update(Vector2 point1, Vector2 point2)
        {
            this.m_Point1 = point1;
            this.m_Point2 = point2;
            this.m_Center = (Vector2) ((point1 + point2) * 0.5f);
            Vector2 vector = point2 - point1;
            this.m_Length = vector.magnitude * 0.5f;
            Vector3 rhs = (Vector3) this.Get2DPlane(this.m_Center, 0f);
            float num = Vector3.Dot(new Vector3(point1.x, point1.y, 1f), rhs);
            Vector2 vector3 = point1 - point2;
            this.m_Angle = 0.01745329f * Vector2.Angle(new Vector2(0f, 1f), vector3.normalized);
            if (num > 0f)
            {
                this.m_Angle = 6.283185f - this.m_Angle;
            }
            this.m_Plane = this.Get2DPlane(this.m_Center, this.m_Angle);
            this.m_PlaneOrtho = this.Get2DPlane(this.m_Center, this.m_Angle + 1.570796f);
        }

        public void Update(Vector2 center, float length, float angle)
        {
            this.m_Center = center;
            this.m_Length = length;
            this.m_Angle = angle;
            this.m_Plane = this.Get2DPlane(this.m_Center, this.m_Angle);
            this.m_PlaneOrtho = this.Get2DPlane(this.m_Center, this.m_Angle + 1.570796f);
            Vector2 vector = new Vector2(this.m_PlaneOrtho.x, this.m_PlaneOrtho.y);
            this.m_Point1 = this.m_Center + ((Vector2) (vector * this.m_Length));
            this.m_Point2 = this.m_Center - ((Vector2) (vector * this.m_Length));
        }

        public float angle =>
            this.m_Angle;

        public Vector2 center =>
            this.m_Center;

        public float length =>
            this.m_Length;

        public Vector4 plane =>
            this.m_Plane;

        public Vector4 planeOrtho =>
            this.m_PlaneOrtho;

        public Vector2 point1 =>
            this.m_Point1;

        public Vector2 point2 =>
            this.m_Point2;
    }
}

