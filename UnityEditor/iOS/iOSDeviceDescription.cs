namespace UnityEditor.iOS
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct iOSDeviceDescription
    {
        public readonly int vendorId;
        public readonly int productId;
        public readonly int revision;
        public readonly string modelId;
        public readonly string type;
        public readonly string model;
        public iOSDeviceDescription(int vendorId, int productId, int revision, string modelId, string type, string model)
        {
            this.vendorId = vendorId;
            this.productId = productId;
            this.revision = revision;
            this.modelId = modelId;
            this.type = type;
            this.model = model;
        }

        public bool IsValid()
        {
            return ((this.type != null) && (this.model != null));
        }
    }
}

