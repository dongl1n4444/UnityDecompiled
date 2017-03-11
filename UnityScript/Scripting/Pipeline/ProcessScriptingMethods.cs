namespace UnityScript.Scripting.Pipeline
{
    using Boo.Lang.Compiler;
    using Boo.Lang.Compiler.Ast;
    using Boo.Lang.Compiler.TypeSystem;
    using System;
    using UnityScript.Scripting;
    using UnityScript.Steps;

    [Serializable]
    public class ProcessScriptingMethods : ProcessUnityScriptMethods
    {
        protected EvaluationContext _evaluationContext;

        public ProcessScriptingMethods(EvaluationContext evaluationContext)
        {
            this._evaluationContext = evaluationContext;
        }

        public MemberReferenceExpression GetActiveScriptEntityReference(ActiveScriptEntity scriptEntity) => 
            this.CodeBuilder.CreateMemberReference(this.GetActiveScriptReference(scriptEntity), scriptEntity.Delegate);

        public MethodInvocationExpression GetActiveScriptReference(ActiveScriptEntity scriptEntity) => 
            this.CodeBuilder.CreateMethodInvocation(this.CodeBuilder.CreateReference(this.GetEvaluationContextField()), this.TypeSystemServices.Map(typeof(EvaluationContext).GetMethod("GetActiveScript")), this.CodeBuilder.CreateIntegerLiteral(scriptEntity.Script));

        public IField GetEvaluationContextField() => 
            this.NameResolutionService.ResolveField(this.CurrentType, "EvaluationContext");

        public IEntity GetScriptClassType() => 
            this.GetEntity(UtilitiesModule.GetScriptClass(this.Context));

        public IField GetScriptContainerField() => 
            this.NameResolutionService.ResolveField((IType) this.GetScriptClassType(), "ScriptContainer");

        public IField GetTargetFieldContext(EvaluationContextEntity entity) => 
            (!this.IsEvaluationContextMember(entity) ? this.GetScriptContainerField() : this.GetEvaluationContextField());

        public override bool HasSideEffect(Expression e) => 
            true;

        public bool IsEvaluationContextMember(EvaluationContextEntity entity) => 
            (entity.Delegate.DeclaringType == this.TypeSystemServices.Map(this._evaluationContext.GetType()));

        public MemberReferenceExpression MapToContextReference(EvaluationContextEntity entity)
        {
            ActiveScriptEntity scriptEntity = entity as ActiveScriptEntity;
            if (scriptEntity != null)
            {
                Evaluator.Taint(this.CompileUnit);
                return this.GetActiveScriptEntityReference(scriptEntity);
            }
            return this.CodeBuilder.CreateMemberReference(this.CodeBuilder.CreateReference(this.GetTargetFieldContext(entity)), entity.Delegate);
        }

        public override void OnReferenceExpression(ReferenceExpression node)
        {
            base.OnReferenceExpression(node);
            EvaluationContextEntity entity = node.Entity as EvaluationContextEntity;
            if (entity != null)
            {
                if (!this.ValidateContext(entity))
                {
                    IMember member = entity.Delegate;
                    this.Errors.Add(CompilerErrorFactory.InstanceRequired(node, member));
                }
                node.ParentNode.Replace(node, this.MapToContextReference(entity));
            }
        }

        public bool ValidateContext(EvaluationContextEntity entity) => 
            (this._evaluationContext.IsStaticContext ? (!entity.Delegate.IsStatic ? this.IsEvaluationContextMember(entity) : true) : true);
    }
}

