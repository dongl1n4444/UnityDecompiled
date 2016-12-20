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

        public MemberReferenceExpression GetActiveScriptEntityReference(ActiveScriptEntity scriptEntity)
        {
            return this.get_CodeBuilder().CreateMemberReference(this.GetActiveScriptReference(scriptEntity), scriptEntity.Delegate);
        }

        public MethodInvocationExpression GetActiveScriptReference(ActiveScriptEntity scriptEntity)
        {
            return this.get_CodeBuilder().CreateMethodInvocation(this.get_CodeBuilder().CreateReference(this.GetEvaluationContextField()), this.get_TypeSystemServices().Map(typeof(EvaluationContext).GetMethod("GetActiveScript")), this.get_CodeBuilder().CreateIntegerLiteral(scriptEntity.Script));
        }

        public IField GetEvaluationContextField()
        {
            return this.get_NameResolutionService().ResolveField(this.get_CurrentType(), "EvaluationContext");
        }

        public IEntity GetScriptClassType()
        {
            return this.GetEntity(UtilitiesModule.GetScriptClass(this.get_Context()));
        }

        public IField GetScriptContainerField()
        {
            return this.get_NameResolutionService().ResolveField((IType) this.GetScriptClassType(), "ScriptContainer");
        }

        public IField GetTargetFieldContext(EvaluationContextEntity entity)
        {
            return (!this.IsEvaluationContextMember(entity) ? this.GetScriptContainerField() : this.GetEvaluationContextField());
        }

        public override bool HasSideEffect(Expression e)
        {
            return true;
        }

        public bool IsEvaluationContextMember(EvaluationContextEntity entity)
        {
            return (entity.Delegate.get_DeclaringType() == this.get_TypeSystemServices().Map(this._evaluationContext.GetType()));
        }

        public MemberReferenceExpression MapToContextReference(EvaluationContextEntity entity)
        {
            ActiveScriptEntity scriptEntity = entity as ActiveScriptEntity;
            if (scriptEntity != null)
            {
                Evaluator.Taint(this.get_CompileUnit());
                return this.GetActiveScriptEntityReference(scriptEntity);
            }
            return this.get_CodeBuilder().CreateMemberReference(this.get_CodeBuilder().CreateReference(this.GetTargetFieldContext(entity)), entity.Delegate);
        }

        public override void OnReferenceExpression(ReferenceExpression node)
        {
            base.OnReferenceExpression(node);
            EvaluationContextEntity entity = node.get_Entity() as EvaluationContextEntity;
            if (entity != null)
            {
                if (!this.ValidateContext(entity))
                {
                    IMember member = entity.Delegate;
                    this.get_Errors().Add(CompilerErrorFactory.InstanceRequired(node, member));
                }
                node.get_ParentNode().Replace(node, this.MapToContextReference(entity));
            }
        }

        public bool ValidateContext(EvaluationContextEntity entity)
        {
            return (this._evaluationContext.IsStaticContext ? (!entity.Delegate.get_IsStatic() ? this.IsEvaluationContextMember(entity) : true) : true);
        }
    }
}

