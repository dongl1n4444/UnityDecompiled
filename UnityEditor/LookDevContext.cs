namespace UnityEditor
{
    using System;
    using UnityEngine;

    [Serializable]
    internal class LookDevContext
    {
        [SerializeField]
        private LookDevPropertyValue[] m_Properties = new LookDevPropertyValue[5];

        public LookDevContext()
        {
            for (int i = 0; i < 5; i++)
            {
                this.m_Properties[i] = new LookDevPropertyValue();
            }
            this.m_Properties[0].floatValue = 0f;
            this.m_Properties[1].intValue = 0;
            this.m_Properties[2].intValue = -1;
            this.m_Properties[4].intValue = -1;
            this.m_Properties[3].floatValue = 0f;
        }

        public LookDevPropertyValue GetProperty(LookDevProperty property) => 
            this.m_Properties[(int) property];

        public void UpdateProperty(LookDevProperty property, int value)
        {
            this.m_Properties[(int) property].intValue = value;
        }

        public void UpdateProperty(LookDevProperty property, float value)
        {
            this.m_Properties[(int) property].floatValue = value;
        }

        public int currentHDRIIndex
        {
            get => 
                this.m_Properties[1].intValue;
            set
            {
                this.m_Properties[1].intValue = value;
            }
        }

        public float envRotation
        {
            get => 
                this.m_Properties[3].floatValue;
            set
            {
                this.m_Properties[3].floatValue = value;
            }
        }

        public float exposureValue =>
            this.m_Properties[0].floatValue;

        public int lodIndex =>
            this.m_Properties[4].intValue;

        public int shadingMode =>
            this.m_Properties[2].intValue;

        [Serializable]
        public class LookDevPropertyValue
        {
            public float floatValue = 0f;
            public int intValue = 0;
        }
    }
}

