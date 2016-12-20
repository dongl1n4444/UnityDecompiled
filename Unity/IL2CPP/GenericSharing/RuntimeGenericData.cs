namespace Unity.IL2CPP.GenericSharing
{
    using System;

    public class RuntimeGenericData
    {
        public readonly RuntimeGenericContextInfo InfoType;

        public RuntimeGenericData(RuntimeGenericContextInfo infoType)
        {
            this.InfoType = infoType;
        }
    }
}

