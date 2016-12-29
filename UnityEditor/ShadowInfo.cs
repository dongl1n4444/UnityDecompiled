namespace UnityEditor
{
    using System;
    using UnityEngine;

    [Serializable]
    internal class ShadowInfo
    {
        [SerializeField]
        private float m_Latitude = 60f;
        [SerializeField]
        private float m_Longitude = 299f;
        [SerializeField]
        private Color m_ShadowColor = Color.white;
        [SerializeField]
        private float m_ShadowIntensity = 1f;

        private void ConformLatLong()
        {
            if (this.m_Latitude < -90f)
            {
                this.m_Latitude = -90f;
            }
            if (this.m_Latitude > 89f)
            {
                this.m_Latitude = 89f;
            }
            this.m_Longitude = this.m_Longitude % 360f;
            if (this.m_Longitude < 0.0)
            {
                this.m_Longitude = 360f + this.m_Longitude;
            }
        }

        public float latitude
        {
            get => 
                this.m_Latitude;
            set
            {
                this.m_Latitude = value;
                this.ConformLatLong();
            }
        }

        public float longitude
        {
            get => 
                this.m_Longitude;
            set
            {
                this.m_Longitude = value;
                this.ConformLatLong();
            }
        }

        public Color shadowColor
        {
            get => 
                this.m_ShadowColor;
            set
            {
                this.m_ShadowColor = value;
            }
        }

        public float shadowIntensity
        {
            get => 
                this.m_ShadowIntensity;
            set
            {
                this.m_ShadowIntensity = value;
            }
        }
    }
}

