namespace Unity.IL2CPP.Metadata
{
    using System;

    public class ParameterDefaultValue
    {
        public readonly int _dataIndex;
        public readonly int _parameterIndex;
        public readonly int _typeIndex;

        public ParameterDefaultValue(int parameterIndex, int typeIndex, int dataIndex)
        {
            this._parameterIndex = parameterIndex;
            this._typeIndex = typeIndex;
            this._dataIndex = dataIndex;
        }

        public override string ToString() => 
            $"{{ {this._parameterIndex}, {this._typeIndex}, {this._dataIndex} }}";
    }
}

