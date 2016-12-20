namespace UnityEditor.iOS.Xcode.PBX
{
    using System.Collections.Generic;

    internal class ArrayAST : ValueAST
    {
        public List<ValueAST> values = new List<ValueAST>();
    }
}

