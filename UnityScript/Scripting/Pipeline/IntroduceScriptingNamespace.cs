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
            this.NameResolutionService.GlobalNamespace = new EvaluationContextNamespace(this.TypeSystemServices, this.NameResolutionService.GlobalNamespace, this._evaluationContext);
        }
    }
}

