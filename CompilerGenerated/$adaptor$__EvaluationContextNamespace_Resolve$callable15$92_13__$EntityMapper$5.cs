namespace CompilerGenerated
{
    using Boo.Lang.Compiler.TypeSystem;
    using System;
    using UnityScript.Scripting.Pipeline;

    [Serializable]
    internal sealed class $adaptor$__EvaluationContextNamespace_Resolve$callable15$92_13__$EntityMapper$5
    {
        protected __EvaluationContextNamespace_Resolve$callable15$92_13__ $from;

        public $adaptor$__EvaluationContextNamespace_Resolve$callable15$92_13__$EntityMapper$5(__EvaluationContextNamespace_Resolve$callable15$92_13__ from)
        {
            this.$from = from;
        }

        public static EvaluationContextNamespace.EntityMapper Adapt(__EvaluationContextNamespace_Resolve$callable15$92_13__ from)
        {
            return new EvaluationContextNamespace.EntityMapper(new $adaptor$__EvaluationContextNamespace_Resolve$callable15$92_13__$EntityMapper$5(from).Invoke);
        }

        public EvaluationContextEntity Invoke(IEntity entity)
        {
            return this.$from(entity);
        }
    }
}

