namespace Unity.IL2CPP.Metadata.Fields
{
    using System;

    public class FieldMarshaledSize
    {
        public readonly int _fieldIndex;
        public readonly int _size;
        public readonly int _typeIndex;

        public FieldMarshaledSize(int fieldIndex, int typeIndex, int size)
        {
            this._fieldIndex = fieldIndex;
            this._typeIndex = typeIndex;
            this._size = size;
        }

        public override string ToString() => 
            $"{{ {this._fieldIndex}, {this._typeIndex}, {this._size} }}";
    }
}

