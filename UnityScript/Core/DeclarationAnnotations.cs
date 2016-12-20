namespace UnityScript.Core
{
    using Boo.Lang.Compiler.Ast;
    using System;

    public static class DeclarationAnnotations
    {
        [NonSerialized]
        private static object NewVariableAnnotation = new object();

        public static void ForceNewVariable(Declaration d)
        {
            d.Annotate(NewVariableAnnotation);
        }

        public static bool ShouldForceNewVariableFor(Declaration d)
        {
            return d.ContainsAnnotation(NewVariableAnnotation);
        }
    }
}

