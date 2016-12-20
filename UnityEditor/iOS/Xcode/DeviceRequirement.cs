namespace UnityEditor.iOS.Xcode
{
    using System;
    using System.Collections.Generic;

    internal class DeviceRequirement
    {
        internal Dictionary<string, string> values = new Dictionary<string, string>();

        public DeviceRequirement()
        {
            this.values.Add("idiom", DeviceTypeRequirement.Any);
        }

        public DeviceRequirement AddCustom(string key, string value)
        {
            if (this.values.ContainsKey(key))
            {
                this.values.Remove(key);
            }
            this.values.Add(key, value);
            return this;
        }

        public DeviceRequirement AddDevice(string device)
        {
            this.AddCustom(DeviceTypeRequirement.Key, device);
            return this;
        }

        public DeviceRequirement AddGraphics(string graphics)
        {
            this.AddCustom(GraphicsRequirement.Key, graphics);
            return this;
        }

        public DeviceRequirement AddHeightClass(string sizeClass)
        {
            this.AddCustom(SizeClassRequirement.HeightKey, sizeClass);
            return this;
        }

        public DeviceRequirement AddMemory(string memory)
        {
            this.AddCustom(MemoryRequirement.Key, memory);
            return this;
        }

        public DeviceRequirement AddScale(string scale)
        {
            this.AddCustom(ScaleRequirement.Key, scale);
            return this;
        }

        public DeviceRequirement AddWidthClass(string sizeClass)
        {
            this.AddCustom(SizeClassRequirement.WidthKey, sizeClass);
            return this;
        }
    }
}

