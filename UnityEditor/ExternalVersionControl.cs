namespace UnityEditor
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct ExternalVersionControl
    {
        private string m_Value;
        public static readonly string Disabled;
        public static readonly string AutoDetect;
        public static readonly string Generic;
        [Obsolete("Asset Server VCS support has been removed.")]
        public static readonly string AssetServer;
        public ExternalVersionControl(string value)
        {
            this.m_Value = value;
        }

        public static implicit operator string(ExternalVersionControl d) => 
            d.ToString();

        public static implicit operator ExternalVersionControl(string d) => 
            new ExternalVersionControl(d);

        public override string ToString() => 
            this.m_Value;

        static ExternalVersionControl()
        {
            Disabled = "Hidden Meta Files";
            AutoDetect = "Auto detect";
            Generic = "Visible Meta Files";
            AssetServer = "Asset Server";
        }
    }
}

