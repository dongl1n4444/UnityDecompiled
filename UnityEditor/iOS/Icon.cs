namespace UnityEditor.iOS
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct Icon
    {
        public readonly DeviceType deviceType;
        public readonly float width;
        public readonly float height;
        public readonly int scale;
        public readonly string xcodefile;
        public static readonly Icon[] types;
        public Icon(DeviceType deviceType, float width, float height, int scale, string xcodefile)
        {
            this.deviceType = deviceType;
            this.width = width;
            this.height = height;
            this.scale = scale;
            this.xcodefile = xcodefile;
        }

        static Icon()
        {
            types = new Icon[] { 
                new Icon(DeviceType.iPhone, 57f, 57f, 1, "Icon.png"), new Icon(DeviceType.iPhone, 57f, 57f, 2, "Icon@2x.png"), new Icon(DeviceType.iPhone, 60f, 60f, 2, "Icon-120.png"), new Icon(DeviceType.iPhone, 60f, 60f, 3, "Icon-180.png"), new Icon(DeviceType.iPhone, 29f, 29f, 1, "Icon-Small.png"), new Icon(DeviceType.iPhone, 29f, 29f, 2, "Icon-Small@2x.png"), new Icon(DeviceType.iPhone, 29f, 29f, 3, "Icon-Small@3x.png"), new Icon(DeviceType.iPhone, 40f, 40f, 2, "Icon-Small-80.png"), new Icon(DeviceType.iPhone, 40f, 40f, 3, "Icon-Small-120.png"), new Icon(DeviceType.iPad, 29f, 29f, 1, "Icon-Small.png"), new Icon(DeviceType.iPad, 29f, 29f, 2, "Icon-Small@2x.png"), new Icon(DeviceType.iPad, 40f, 40f, 1, "Icon-Small-40.png"), new Icon(DeviceType.iPad, 40f, 40f, 2, "Icon-Small-80.png"), new Icon(DeviceType.iPad, 72f, 72f, 1, "Icon-72.png"), new Icon(DeviceType.iPad, 72f, 72f, 2, "Icon-144.png"), new Icon(DeviceType.iPad, 76f, 76f, 1, "Icon-76.png"),
                new Icon(DeviceType.iPad, 76f, 76f, 2, "Icon-152.png"), new Icon(DeviceType.iPad, 83.5f, 83.5f, 2, "Icon-167.png"), new Icon(DeviceType.AppleTV, 360f, 216f, 1, "Icon-360x216.png"), new Icon(DeviceType.AppleTV, 1280f, 768f, 1, "Icon-1280x768.png")
            };
        }
    }
}

