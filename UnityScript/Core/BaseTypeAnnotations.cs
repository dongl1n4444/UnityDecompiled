namespace UnityScript.Core
{
    using Boo.Lang.Compiler.Ast;
    using System;

    public static class BaseTypeAnnotations
    {
        [NonSerialized]
        private static object Extends = new object();
        [NonSerialized]
        private static object Implements = new object();

        public static void AnnotateExtends(TypeReference baseType)
        {
            baseType.Annotate(Extends);
        }

        public static void AnnotateImplements(TypeReference baseType)
        {
            baseType.Annotate(Implements);
        }

        public static bool HasExtends(TypeReference baseType) => 
            baseType.ContainsAnnotation(Extends);

        public static bool HasImplements(TypeReference baseType) => 
            baseType.ContainsAnnotation(Implements);
    }
}

