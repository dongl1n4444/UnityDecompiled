namespace UnityScript.Scripting.Pipeline
{
    using Boo.Lang.Compiler.Steps;
    using System;
    using UnityScript.Scripting;

    [Serializable]
    public class IntroduceScriptingNamespace : AbstractCompilerStep
    {
        protected EvaluationContext _evaluationContext;

        public IntroduceScriptingNamespace(EvaluationContext evaluationContext)
        {
            this._evaluationContext = evaluationContext;
        }

        public override void Run()
        {
            this.get_NameResolutionService().set_GlobalNamespace(new EvaluationContextNamespace(this.get_TypeSystemServices(), this.get_NameResolutionService().get_GlobalNamespace(), this._evaluationContext));
        }
    }
}

