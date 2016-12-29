namespace UnityEngine.Internal
{
    using System;

    [Serializable, AttributeUsage(AttributeTargets.GenericParameter | AttributeTargets.Parameter)]
    public class DefaultValueAttribute : Attribute
    {
        private object DefaultValue;

        public DefaultValueAttribute(string value)
        {
            this.DefaultValue = value;
        }

        public override bool Equals(object obj)
        {
            DefaultValueAttribute attribute = obj as DefaultValueAttribute;
            if (attribute == null)
            {
                return false;
            }
            return this.DefaultValue?.Equals(attribute.Value);
        }

        public override int GetHashCode() => 
            this.DefaultValue?.GetHashCode();

        public object Value =>
            this.DefaultValue;
    }
}

