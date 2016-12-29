namespace Unity.IL2CPP.Metadata.Fields
{
    using System;

    public class FieldDefaultValue
    {
        public readonly int _dataIndex;
        public readonly int _fieldIndex;
        public readonly int _typeIndex;

        public FieldDefaultValue(int fieldIndex, int typeIndex, int dataIndex)
        {
            this._fieldIndex = fieldIndex;
            this._typeIndex = typeIndex;
            this._dataIndex = dataIndex;
        }

        public override string ToString() => 
            $"{{ {this._fieldIndex}, {this._typeIndex}, {this._dataIndex} }}";
    }
}

