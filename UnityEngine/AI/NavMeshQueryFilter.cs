namespace UnityEngine.AI
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct NavMeshQueryFilter
    {
        private const int AREA_COST_ELEMENT_COUNT = 0x20;
        private int m_AreaMask;
        private int m_AgentTypeID;
        private float[] m_AreaCost;
        internal float[] costs =>
            this.m_AreaCost;
        public int areaMask
        {
            get => 
                this.m_AreaMask;
            set
            {
                this.m_AreaMask = value;
            }
        }
        public int agentTypeID
        {
            get => 
                this.m_AgentTypeID;
            set
            {
                this.m_AgentTypeID = value;
            }
        }
        public float GetAreaCost(int areaIndex)
        {
            if (this.m_AreaCost == null)
            {
                if ((areaIndex < 0) || (areaIndex >= 0x20))
                {
                    throw new IndexOutOfRangeException($"The valid range is [0:{0x1f}]");
                }
                return 1f;
            }
            return this.m_AreaCost[areaIndex];
        }

        public void SetAreaCost(int areaIndex, float cost)
        {
            if (this.m_AreaCost == null)
            {
                this.m_AreaCost = new float[0x20];
                for (int i = 0; i < 0x20; i++)
                {
                    this.m_AreaCost[i] = 1f;
                }
            }
            this.m_AreaCost[areaIndex] = cost;
        }
    }
}

