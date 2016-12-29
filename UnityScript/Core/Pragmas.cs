namespace UnityScript.Core
{
    using Boo.Lang.Compiler.Ast;
    using Boo.Lang.Runtime;
    using System;

    public static class Pragmas
    {
        [NonSerialized]
        public const string Checked = "checked";
        [NonSerialized]
        private const bool Disabled = false;
        [NonSerialized]
        public const string Downcast = "downcast";
        [NonSerialized]
        private const bool Enabled = true;
        [NonSerialized]
        public const string Expando = "expando";
        [NonSerialized]
        public const string Implicit = "implicit";
        [NonSerialized]
        public const string Strict = "strict";
        [NonSerialized]
        private static readonly string[] ValidPragmas = new string[] { "strict", "expando", "implicit", "downcast", "checked" };

        public static void DisableOn(Module module, string pragma)
        {
            module.set_Item(pragma, false);
        }

        public static bool IsDisabledOn(Module module, string pragma) => 
            RuntimeServices.EqualityOperator(false, module.get_Item(pragma));

        public static bool IsEnabledOn(Module module, string pragma) => 
            RuntimeServices.EqualityOperator(true, module.get_Item(pragma));

        public static bool IsValid(string pragma) => 
            RuntimeServices.op_Member(pragma, ValidPragmas);

        public static bool TryToEnableOn(Module module, string pragma)
        {
            module.Annotate(pragma, true);
            return !module.ContainsAnnotation(pragma);
        }

        public static string[] All =>
            ((string[]) RuntimeServices.GetRange1(ValidPragmas, 0));
    }
}

