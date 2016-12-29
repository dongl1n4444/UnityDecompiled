namespace UnityScript.TypeSystem
{
    using Boo.Lang.Compiler.Services;
    using System;

    [Serializable]
    public class UnityScriptAmbiance : LanguageAmbiance
    {
        public override string DefaultGeneratorTypeFor(string typeName) => 
            "IEnumerator";

        public override string CallableKeyword =>
            "function";

        public override string EnsureKeyword =>
            "finally";

        public override string ExceptKeyword =>
            "catch";

        public override string IsaKeyword =>
            "instanceof";

        public override string IsKeyword =>
            "===";

        public override string RaiseKeyword =>
            "throw";

        public override string SelfKeyword =>
            "this";
    }
}

