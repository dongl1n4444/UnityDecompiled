namespace Unity.IL2CPP.Portability
{
    using System;
    using System.Reflection;

    public static class AttributePortable
    {
        public static Attribute GetCustomAttributePortable(MemberInfo element, Type attributeType)
        {
            return Attribute.GetCustomAttribute(element, attributeType);
        }

        public static Attribute[] GetCustomAttributesPortable(MemberInfo element, bool inherit)
        {
            return Attribute.GetCustomAttributes(element, inherit);
        }

        public static Attribute[] GetCustomAttributesPortable(MemberInfo element, Type attributeType)
        {
            return Attribute.GetCustomAttributes(element, attributeType);
        }

        public static bool IsDefinedPortable(MethodInfo element, Type attributeType)
        {
            return Attribute.IsDefined(element, attributeType);
        }
    }
}

