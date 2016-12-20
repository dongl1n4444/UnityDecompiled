namespace Unity.IL2CPP.Marshaling
{
    using System;

    public class MarshaledType
    {
        public readonly string DecoratedName;
        public readonly string Name;
        public readonly string VariableName;

        public MarshaledType(string name, string decoratedName)
        {
            this.Name = name;
            this.DecoratedName = decoratedName;
            this.VariableName = string.Empty;
        }

        public MarshaledType(string name, string decoratedName, string variableName)
        {
            this.Name = name;
            this.DecoratedName = decoratedName;
            this.VariableName = variableName;
        }
    }
}

