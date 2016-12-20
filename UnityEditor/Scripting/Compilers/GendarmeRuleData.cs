namespace UnityEditor.Scripting.Compilers
{
    using System;

    internal class GendarmeRuleData
    {
        public string Details;
        public string File = "";
        public bool IsAssemblyError;
        public int LastIndex = 0;
        public int Line = 0;
        public string Location;
        public string Problem;
        public string Severity;
        public string Source;
        public string Target;
    }
}

