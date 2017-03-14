namespace Unity.IL2CPP.Metadata
{
    using System;
    using System.Runtime.InteropServices;

    public class RGCTXEntry
    {
        public string FullName;
        public int GenericParameterIndex;
        public string ImageName;
        public Il2CppRGCTXDataType Type;
        public uint TypeOrMethodMetadataIndex;

        public RGCTXEntry(Il2CppRGCTXDataType type, uint typeOrMethodMetadataIndex, string fullName = null) : this(type, typeOrMethodMetadataIndex, null, -1, fullName)
        {
        }

        public RGCTXEntry(Il2CppRGCTXDataType type, uint typeOrMethodMetadataIndex, string imageName, int genericParameterIndex, string fullName = null)
        {
            this.Type = type;
            this.TypeOrMethodMetadataIndex = typeOrMethodMetadataIndex;
            this.ImageName = imageName;
            this.GenericParameterIndex = genericParameterIndex;
            this.FullName = fullName;
        }
    }
}

