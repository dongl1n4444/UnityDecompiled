namespace Unity.IL2CPP.IoCServices
{
    using System;
    using System.Collections.Generic;

    public interface IIl2CppGenericInstCollectorWriterService
    {
        void Add(IList<TypeReference> types);
    }
}

