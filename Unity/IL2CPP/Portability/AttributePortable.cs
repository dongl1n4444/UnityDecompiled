namespace Unity.IL2CPP.Portability
{
    using System;
    using System.Reflection;

    public static class AttributePortable
    {
        public static Attribute GetCustomAttributePortable(MemberInfo element, Type attributeType) => 
            Attribute.GetCustomAttribute(element, attributeType);

        public static Attribute[] GetCustomAttributesPortable(MemberInfo element, bool inherit) => 
            Attribute.GetCustomAttributes(element, inherit);

        public static Attribute[] GetCustomAttributesPortable(MemberInfo element, Type attributeType) => 
            Attribute.GetCustomAttributes(element, attributeType);

        public static bool IsDefinedPortable(MethodInfo element, Type attributeType) => 
            Attribute.IsDefined(element, attributeType);
    }
}

