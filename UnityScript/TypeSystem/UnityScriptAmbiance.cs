namespace UnityScript.TypeSystem
{
    using Boo.Lang.Compiler.Services;
    using System;

    [Serializable]
    public class UnityScriptAmbiance : LanguageAmbiance
    {
        public override string DefaultGeneratorTypeFor(string typeName)
        {
            return "IEnumerator";
        }

        public override string CallableKeyword
        {
            get
            {
                return "function";
            }
        }

        public override string EnsureKeyword
        {
            get
            {
                return "finally";
            }
        }

        public override string ExceptKeyword
        {
            get
            {
                return "catch";
            }
        }

        public override string IsaKeyword
        {
            get
            {
                return "instanceof";
            }
        }

        public override string IsKeyword
        {
            get
            {
                return "===";
            }
        }

        public override string RaiseKeyword
        {
            get
            {
                return "throw";
            }
        }

        public override string SelfKeyword
        {
            get
            {
                return "this";
            }
        }
    }
}

