namespace Unity.IL2CPP.Portability
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public static class CustomAttributeDataPortable
    {
        public static IList<CustomAttributeData> GetCustomAttributesPortable(MemberInfo target) => 
            CustomAttributeData.GetCustomAttributes(target);
    }
}

