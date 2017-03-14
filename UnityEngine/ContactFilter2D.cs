namespace UnityEngine
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// <para>A set of parameters for filtering contact results.</para>
    /// </summary>
    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct ContactFilter2D
    {
        /// <summary>
        /// <para>Sets to filter contact results based on trigger collider involvement.</para>
        /// </summary>
        public bool useTriggers;
        /// <summary>
        /// <para>Sets the contact filter to filter results by layer mask.</para>
        /// </summary>
        public bool useLayerMask;
        /// <summary>
        /// <para>Sets the contact filter to filter the results by depth using minDepth and maxDepth.</para>
        /// </summary>
        public bool useDepth;
        /// <summary>
        /// <para>Sets the contact filter to filter the results by the collision's normal angle using minNormalAngle and maxNormalAngle.</para>
        /// </summary>
        public bool useNormalAngle;
        /// <summary>
        /// <para>Sets the contact filter to filter the results that only include Collider2D on the layers defined by the layer mask.</para>
        /// </summary>
        public LayerMask layerMask;
        /// <summary>
        /// <para>Sets the contact filter to filter the results to only include Collider2D with a Z coordinate (depth) greater than this value.</para>
        /// </summary>
        public float minDepth;
        /// <summary>
        /// <para>Sets the contact filter to filter the results to only include Collider2D with a Z coordinate (depth) less than this value.</para>
        /// </summary>
        public float maxDepth;
        /// <summary>
        /// <para>Sets the contact filter to filter the results to only include contacts with collision normal angles that are greater than this angle.</para>
        /// </summary>
        public float minNormalAngle;
        /// <summary>
        /// <para>Sets the contact filter to filter the results to only include contacts with collision normal angles that are less than this angle.</para>
        /// </summary>
        public float maxNormalAngle;
        /// <summary>
        /// <para>Sets the contact filter to not filter any ContactPoint2D.</para>
        /// </summary>
        /// <returns>
        /// <para>A copy of the contact filter set to not filter any ContactPoint2D.</para>
        /// </returns>
        public ContactFilter2D NoFilter()
        {
            this.useTriggers = false;
            this.useLayerMask = false;
            this.layerMask = -1;
            this.useDepth = false;
            this.minDepth = float.NegativeInfinity;
            this.maxDepth = float.PositiveInfinity;
            this.useNormalAngle = false;
            this.minNormalAngle = float.NegativeInfinity;
            this.maxNormalAngle = float.PositiveInfinity;
            return this;
        }

        /// <summary>
        /// <para>Sets the layerMask filter property using the layerMask parameter provided and also enables layer mask filtering by setting useLayerMask to true.</para>
        /// </summary>
        /// <param name="layerMask">The value used to set the layerMask.</param>
        public void SetLayerMask(LayerMask layerMask)
        {
            this.layerMask = layerMask;
            this.useLayerMask = true;
        }

        /// <summary>
        /// <para>Turns off layer mask filtering by simple setting useLayerMask to false.  The associated value of layerMask is not changed.</para>
        /// </summary>
        public void ClearLayerMask()
        {
            this.useLayerMask = false;
        }

        /// <summary>
        /// <para>Sets the minDepth and maxDepth filter properties and turns on depth filtering by setting useDepth to true.</para>
        /// </summary>
        /// <param name="minDepth">The value used to set minDepth.</param>
        /// <param name="maxDepth">The value used to set maxDepth.</param>
        public void SetDepth(float minDepth, float maxDepth)
        {
            this.minDepth = minDepth;
            this.maxDepth = maxDepth;
            this.useDepth = true;
        }

        /// <summary>
        /// <para>Turns off depth filtering by simply setting useDepth to false.  The associated values of minDepth and maxDepth are not changed.</para>
        /// </summary>
        public void ClearDepth()
        {
            this.useDepth = false;
        }

        /// <summary>
        /// <para>Sets the minNormalAngle and maxNormalAngle filter properties and turns on normal angle filtering by setting useNormalAngle to true.</para>
        /// </summary>
        /// <param name="minNormalAngle">The value used to set the minNormalAngle.</param>
        /// <param name="maxNormalAngle">The value used to set the maxNormalAngle.</param>
        public void SetNormalAngle(float minNormalAngle, float maxNormalAngle)
        {
            this.minNormalAngle = minNormalAngle;
            this.maxNormalAngle = maxNormalAngle;
            this.useNormalAngle = true;
        }

        /// <summary>
        /// <para>Turns off normal angle filtering by setting useNormalAngle to false. The associated values of minNormalAngle and maxNormalAngle are not changed.</para>
        /// </summary>
        public void ClearNormalAngle()
        {
            this.useNormalAngle = false;
        }

        internal static ContactFilter2D CreateLegacyFilter(int layerMask, float minDepth, float maxDepth)
        {
            ContactFilter2D filterd = new ContactFilter2D {
                useTriggers = Physics2D.queriesHitTriggers
            };
            filterd.SetLayerMask(layerMask);
            filterd.SetDepth(minDepth, maxDepth);
            return filterd;
        }
    }
}

