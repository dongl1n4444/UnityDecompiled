namespace UnityEditor.iOS.Xcode.PBX
{
    using System.Collections.Generic;

    internal class TreeAST : ValueAST
    {
        public List<KeyValueAST> values = new List<KeyValueAST>();
    }
}

