namespace TreeEditor
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [Serializable]
    public class TreeNode
    {
        [SerializeField]
        private int _uniqueID = -1;
        public float angle;
        public float animSeed;
        public float baseAngle;
        public float breakOffset;
        public float capRange;
        [NonSerialized]
        internal TreeGroup group;
        public int groupID;
        public Matrix4x4 matrix;
        public float offset;
        [NonSerialized]
        internal TreeNode parent;
        public int parentID;
        public float pitch;
        public Quaternion rotation;
        public float scale;
        public int seed;
        public float size;
        public TreeSpline spline = null;
        public int triEnd;
        public int triStart;
        public int vertEnd;
        public int vertStart;
        public bool visible;

        public TreeNode()
        {
            this.spline = null;
            this.parentID = 0;
            this.groupID = 0;
            this.parent = null;
            this.group = null;
            this.seed = 0x4d2;
            this.breakOffset = 1f;
            this.visible = true;
            this.animSeed = 0f;
            this.scale = 1f;
            this.rotation = Quaternion.identity;
            this.matrix = Matrix4x4.identity;
        }

        public Matrix4x4 GetLocalMatrixAtTime(float time)
        {
            Vector3 zero = Vector3.zero;
            Quaternion identity = Quaternion.identity;
            float rad = 0f;
            this.GetPropertiesAtTime(time, out zero, out identity, out rad);
            return Matrix4x4.TRS(zero, identity, Vector3.one);
        }

        public void GetPropertiesAtTime(float time, out Vector3 pos, out Quaternion rot, out float rad)
        {
            if (this.spline == null)
            {
                pos = Vector3.zero;
                rot = Quaternion.identity;
            }
            else
            {
                pos = this.spline.GetPositionAtTime(time);
                rot = this.spline.GetRotationAtTime(time);
            }
            rad = this.group.GetRadiusAtTime(this, time, false);
        }

        public float GetRadiusAtTime(float time)
        {
            return this.group.GetRadiusAtTime(this, time, false);
        }

        public float GetScale()
        {
            float scale = 1f;
            if (this.parent != null)
            {
                scale = this.parent.GetScale();
            }
            return (this.scale * scale);
        }

        public float GetSurfaceAngleAtTime(float time)
        {
            if (this.spline == null)
            {
                return 0f;
            }
            float num2 = 0f;
            Vector3 positionAtTime = this.spline.GetPositionAtTime(time);
            float num3 = this.group.GetRadiusAtTime(this, time, false);
            if (time < 0.5f)
            {
                Vector3 vector2 = this.spline.GetPositionAtTime(time + 0.01f) - positionAtTime;
                float magnitude = vector2.magnitude;
                float y = this.group.GetRadiusAtTime(this, time + 0.01f, false) - num3;
                num2 = Mathf.Atan2(y, magnitude);
            }
            else
            {
                Vector3 vector3 = positionAtTime - this.spline.GetPositionAtTime(time - 0.01f);
                float x = vector3.magnitude;
                float num7 = num3 - this.group.GetRadiusAtTime(this, time - 0.01f, false);
                num2 = Mathf.Atan2(num7, x);
            }
            return (num2 * 57.29578f);
        }

        public int uniqueID
        {
            get
            {
                return this._uniqueID;
            }
            set
            {
                if (this._uniqueID == -1)
                {
                    this._uniqueID = value;
                }
            }
        }
    }
}

