namespace UnityEngine
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>A 2D Rectangle defined by X and Y position, width and height.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential), UsedByNativeCode]
    public struct Rect
    {
        private float m_XMin;
        private float m_YMin;
        private float m_Width;
        private float m_Height;
        /// <summary>
        /// <para>Creates a new rectangle.</para>
        /// </summary>
        /// <param name="x">The X value the rect is measured from.</param>
        /// <param name="y">The Y value the rect is measured from.</param>
        /// <param name="width">The width of the rectangle.</param>
        /// <param name="height">The height of the rectangle.</param>
        public Rect(float x, float y, float width, float height)
        {
            this.m_XMin = x;
            this.m_YMin = y;
            this.m_Width = width;
            this.m_Height = height;
        }

        /// <summary>
        /// <para>Creates a rectangle given a size and position.</para>
        /// </summary>
        /// <param name="position">The position of the minimum corner of the rect.</param>
        /// <param name="size">The width and height of the rect.</param>
        public Rect(Vector2 position, Vector2 size)
        {
            this.m_XMin = position.x;
            this.m_YMin = position.y;
            this.m_Width = size.x;
            this.m_Height = size.y;
        }

        /// <summary>
        /// <para></para>
        /// </summary>
        /// <param name="source"></param>
        public Rect(Rect source)
        {
            this.m_XMin = source.m_XMin;
            this.m_YMin = source.m_YMin;
            this.m_Width = source.m_Width;
            this.m_Height = source.m_Height;
        }

        /// <summary>
        /// <para>Shorthand for writing new Rect(0,0,0,0).</para>
        /// </summary>
        public static Rect zero =>
            new Rect(0f, 0f, 0f, 0f);
        /// <summary>
        /// <para>Creates a rectangle from min/max coordinate values.</para>
        /// </summary>
        /// <param name="xmin">The minimum X coordinate.</param>
        /// <param name="ymin">The minimum Y coordinate.</param>
        /// <param name="xmax">The maximum X coordinate.</param>
        /// <param name="ymax">The maximum Y coordinate.</param>
        /// <returns>
        /// <para>A rectangle matching the specified coordinates.</para>
        /// </returns>
        public static Rect MinMaxRect(float xmin, float ymin, float xmax, float ymax) => 
            new Rect(xmin, ymin, xmax - xmin, ymax - ymin);

        /// <summary>
        /// <para>Set components of an existing Rect.</para>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void Set(float x, float y, float width, float height)
        {
            this.m_XMin = x;
            this.m_YMin = y;
            this.m_Width = width;
            this.m_Height = height;
        }

        /// <summary>
        /// <para>The X coordinate of the rectangle.</para>
        /// </summary>
        public float x
        {
            get => 
                this.m_XMin;
            set
            {
                this.m_XMin = value;
            }
        }
        /// <summary>
        /// <para>The Y coordinate of the rectangle.</para>
        /// </summary>
        public float y
        {
            get => 
                this.m_YMin;
            set
            {
                this.m_YMin = value;
            }
        }
        /// <summary>
        /// <para>The X and Y position of the rectangle.</para>
        /// </summary>
        public Vector2 position
        {
            get => 
                new Vector2(this.m_XMin, this.m_YMin);
            set
            {
                this.m_XMin = value.x;
                this.m_YMin = value.y;
            }
        }
        /// <summary>
        /// <para>The position of the center of the rectangle.</para>
        /// </summary>
        public Vector2 center
        {
            get => 
                new Vector2(this.x + (this.m_Width / 2f), this.y + (this.m_Height / 2f));
            set
            {
                this.m_XMin = value.x - (this.m_Width / 2f);
                this.m_YMin = value.y - (this.m_Height / 2f);
            }
        }
        /// <summary>
        /// <para>The position of the minimum corner of the rectangle.</para>
        /// </summary>
        public Vector2 min
        {
            get => 
                new Vector2(this.xMin, this.yMin);
            set
            {
                this.xMin = value.x;
                this.yMin = value.y;
            }
        }
        /// <summary>
        /// <para>The position of the maximum corner of the rectangle.</para>
        /// </summary>
        public Vector2 max
        {
            get => 
                new Vector2(this.xMax, this.yMax);
            set
            {
                this.xMax = value.x;
                this.yMax = value.y;
            }
        }
        /// <summary>
        /// <para>The width of the rectangle, measured from the X position.</para>
        /// </summary>
        public float width
        {
            get => 
                this.m_Width;
            set
            {
                this.m_Width = value;
            }
        }
        /// <summary>
        /// <para>The height of the rectangle, measured from the Y position.</para>
        /// </summary>
        public float height
        {
            get => 
                this.m_Height;
            set
            {
                this.m_Height = value;
            }
        }
        /// <summary>
        /// <para>The width and height of the rectangle.</para>
        /// </summary>
        public Vector2 size
        {
            get => 
                new Vector2(this.m_Width, this.m_Height);
            set
            {
                this.m_Width = value.x;
                this.m_Height = value.y;
            }
        }
        /// <summary>
        /// <para>The minimum X coordinate of the rectangle.</para>
        /// </summary>
        public float xMin
        {
            get => 
                this.m_XMin;
            set
            {
                float xMax = this.xMax;
                this.m_XMin = value;
                this.m_Width = xMax - this.m_XMin;
            }
        }
        /// <summary>
        /// <para>The minimum Y coordinate of the rectangle.</para>
        /// </summary>
        public float yMin
        {
            get => 
                this.m_YMin;
            set
            {
                float yMax = this.yMax;
                this.m_YMin = value;
                this.m_Height = yMax - this.m_YMin;
            }
        }
        /// <summary>
        /// <para>The maximum X coordinate of the rectangle.</para>
        /// </summary>
        public float xMax
        {
            get => 
                (this.m_Width + this.m_XMin);
            set
            {
                this.m_Width = value - this.m_XMin;
            }
        }
        /// <summary>
        /// <para>The maximum Y coordinate of the rectangle.</para>
        /// </summary>
        public float yMax
        {
            get => 
                (this.m_Height + this.m_YMin);
            set
            {
                this.m_Height = value - this.m_YMin;
            }
        }
        /// <summary>
        /// <para>Returns true if the x and y components of point is a point inside this rectangle. If allowInverse is present and true, the width and height of the Rect are allowed to take negative values (ie, the min value is greater than the max), and the test will still work.</para>
        /// </summary>
        /// <param name="point">Point to test.</param>
        /// <param name="allowInverse">Does the test allow the Rect's width and height to be negative?</param>
        /// <returns>
        /// <para>True if the point lies within the specified rectangle.</para>
        /// </returns>
        public bool Contains(Vector2 point) => 
            ((((point.x >= this.xMin) && (point.x < this.xMax)) && (point.y >= this.yMin)) && (point.y < this.yMax));

        /// <summary>
        /// <para>Returns true if the x and y components of point is a point inside this rectangle. If allowInverse is present and true, the width and height of the Rect are allowed to take negative values (ie, the min value is greater than the max), and the test will still work.</para>
        /// </summary>
        /// <param name="point">Point to test.</param>
        /// <param name="allowInverse">Does the test allow the Rect's width and height to be negative?</param>
        /// <returns>
        /// <para>True if the point lies within the specified rectangle.</para>
        /// </returns>
        public bool Contains(Vector3 point) => 
            ((((point.x >= this.xMin) && (point.x < this.xMax)) && (point.y >= this.yMin)) && (point.y < this.yMax));

        /// <summary>
        /// <para>Returns true if the x and y components of point is a point inside this rectangle. If allowInverse is present and true, the width and height of the Rect are allowed to take negative values (ie, the min value is greater than the max), and the test will still work.</para>
        /// </summary>
        /// <param name="point">Point to test.</param>
        /// <param name="allowInverse">Does the test allow the Rect's width and height to be negative?</param>
        /// <returns>
        /// <para>True if the point lies within the specified rectangle.</para>
        /// </returns>
        public bool Contains(Vector3 point, bool allowInverse)
        {
            if (!allowInverse)
            {
                return this.Contains(point);
            }
            bool flag2 = false;
            if ((((this.width < 0f) && (point.x <= this.xMin)) && (point.x > this.xMax)) || (((this.width >= 0f) && (point.x >= this.xMin)) && (point.x < this.xMax)))
            {
                flag2 = true;
            }
            return (flag2 && ((((this.height < 0f) && (point.y <= this.yMin)) && (point.y > this.yMax)) || (((this.height >= 0f) && (point.y >= this.yMin)) && (point.y < this.yMax))));
        }

        private static Rect OrderMinMax(Rect rect)
        {
            if (rect.xMin > rect.xMax)
            {
                float xMin = rect.xMin;
                rect.xMin = rect.xMax;
                rect.xMax = xMin;
            }
            if (rect.yMin > rect.yMax)
            {
                float yMin = rect.yMin;
                rect.yMin = rect.yMax;
                rect.yMax = yMin;
            }
            return rect;
        }

        /// <summary>
        /// <para>Returns true if the other rectangle overlaps this one. If allowInverse is present and true, the widths and heights of the Rects are allowed to take negative values (ie, the min value is greater than the max), and the test will still work.</para>
        /// </summary>
        /// <param name="other">Other rectangle to test overlapping with.</param>
        /// <param name="allowInverse">Does the test allow the Rects' widths and heights to be negative?</param>
        public bool Overlaps(Rect other) => 
            ((((other.xMax > this.xMin) && (other.xMin < this.xMax)) && (other.yMax > this.yMin)) && (other.yMin < this.yMax));

        /// <summary>
        /// <para>Returns true if the other rectangle overlaps this one. If allowInverse is present and true, the widths and heights of the Rects are allowed to take negative values (ie, the min value is greater than the max), and the test will still work.</para>
        /// </summary>
        /// <param name="other">Other rectangle to test overlapping with.</param>
        /// <param name="allowInverse">Does the test allow the Rects' widths and heights to be negative?</param>
        public bool Overlaps(Rect other, bool allowInverse)
        {
            Rect rect = this;
            if (allowInverse)
            {
                rect = OrderMinMax(rect);
                other = OrderMinMax(other);
            }
            return rect.Overlaps(other);
        }

        /// <summary>
        /// <para>Returns a point inside a rectangle, given normalized coordinates.</para>
        /// </summary>
        /// <param name="rectangle">Rectangle to get a point inside.</param>
        /// <param name="normalizedRectCoordinates">Normalized coordinates to get a point for.</param>
        public static Vector2 NormalizedToPoint(Rect rectangle, Vector2 normalizedRectCoordinates) => 
            new Vector2(Mathf.Lerp(rectangle.x, rectangle.xMax, normalizedRectCoordinates.x), Mathf.Lerp(rectangle.y, rectangle.yMax, normalizedRectCoordinates.y));

        /// <summary>
        /// <para>Returns the normalized coordinates cooresponding the the point.</para>
        /// </summary>
        /// <param name="rectangle">Rectangle to get normalized coordinates inside.</param>
        /// <param name="point">A point inside the rectangle to get normalized coordinates for.</param>
        public static Vector2 PointToNormalized(Rect rectangle, Vector2 point) => 
            new Vector2(Mathf.InverseLerp(rectangle.x, rectangle.xMax, point.x), Mathf.InverseLerp(rectangle.y, rectangle.yMax, point.y));

        public static bool operator !=(Rect lhs, Rect rhs) => 
            !(lhs == rhs);

        public static bool operator ==(Rect lhs, Rect rhs) => 
            ((((lhs.x == rhs.x) && (lhs.y == rhs.y)) && (lhs.width == rhs.width)) && (lhs.height == rhs.height));

        public override int GetHashCode() => 
            (((this.x.GetHashCode() ^ (this.width.GetHashCode() << 2)) ^ (this.y.GetHashCode() >> 2)) ^ (this.height.GetHashCode() >> 1));

        public override bool Equals(object other)
        {
            if (!(other is Rect))
            {
                return false;
            }
            Rect rect = (Rect) other;
            return (((this.x.Equals(rect.x) && this.y.Equals(rect.y)) && this.width.Equals(rect.width)) && this.height.Equals(rect.height));
        }

        /// <summary>
        /// <para>Returns a nicely formatted string for this Rect.</para>
        /// </summary>
        /// <param name="format"></param>
        public override string ToString()
        {
            object[] args = new object[] { this.x, this.y, this.width, this.height };
            return UnityString.Format("(x:{0:F2}, y:{1:F2}, width:{2:F2}, height:{3:F2})", args);
        }

        /// <summary>
        /// <para>Returns a nicely formatted string for this Rect.</para>
        /// </summary>
        /// <param name="format"></param>
        public string ToString(string format)
        {
            object[] args = new object[] { this.x.ToString(format), this.y.ToString(format), this.width.ToString(format), this.height.ToString(format) };
            return UnityString.Format("(x:{0}, y:{1}, width:{2}, height:{3})", args);
        }

        [Obsolete("use xMin")]
        public float left =>
            this.m_XMin;
        [Obsolete("use xMax")]
        public float right =>
            (this.m_XMin + this.m_Width);
        [Obsolete("use yMin")]
        public float top =>
            this.m_YMin;
        [Obsolete("use yMax")]
        public float bottom =>
            (this.m_YMin + this.m_Height);
    }
}

