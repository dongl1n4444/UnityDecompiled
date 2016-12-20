namespace UnityScript.Steps
{
    using Boo.Lang.Compiler.Ast;
    using System;

    public static class EvalAnnotation
    {
        [NonSerialized]
        private static readonly object Annotation = new object();

        private static void AnnotateEval(Node node)
        {
            if (!IsMarked(node))
            {
                node.Annotate(Annotation);
            }
        }

        public static bool IsMarked(Node node)
        {
            return node.ContainsAnnotation(Annotation);
        }

        public static void Mark(Method node)
        {
            AnnotateEval(node.get_DeclaringType());
            AnnotateEval(node);
        }
    }
}

