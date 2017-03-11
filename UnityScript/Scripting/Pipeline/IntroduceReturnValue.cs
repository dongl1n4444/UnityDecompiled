namespace UnityScript.Scripting.Pipeline
{
    using Boo.Lang.Compiler.Ast;
    using Boo.Lang.Compiler.Steps;
    using Boo.Lang.Compiler.TypeSystem;
    using Boo.Lang.Runtime;
    using System;
    using UnityScript.Steps;

    [Serializable]
    public class IntroduceReturnValue : AbstractCompilerStep
    {
        public bool IsVoid(Expression e) => 
            RuntimeServices.EqualityOperator(TypeSystemServices.GetExpressionType(e), this.TypeSystemServices.VoidType);

        public ExpressionStatement LastExpressionStatement(Method method)
        {
            StatementCollection statements = method.Body.Statements;
            return ((statements.Count != 0) ? (statements[-1] as ExpressionStatement) : null);
        }

        public override void Run()
        {
            ClassDefinition scriptClass = UtilitiesModule.GetScriptClass(this.Context);
            if (scriptClass != null)
            {
                Method method = (Method) scriptClass.Members["Run"];
                ExpressionStatement existing = this.LastExpressionStatement(method);
                if ((existing != null) && !this.IsVoid(existing.Expression))
                {
                    existing.ParentNode.Replace(existing, new ReturnStatement(existing.Expression));
                }
            }
        }
    }
}

