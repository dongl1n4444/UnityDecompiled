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
            RuntimeServices.EqualityOperator(TypeSystemServices.GetExpressionType(e), this.get_TypeSystemServices().VoidType);

        public ExpressionStatement LastExpressionStatement(Method method)
        {
            StatementCollection statements = method.get_Body().get_Statements();
            return ((statements.Count != 0) ? (statements.get_Item(-1) as ExpressionStatement) : null);
        }

        public override void Run()
        {
            ClassDefinition scriptClass = UtilitiesModule.GetScriptClass(this.get_Context());
            if (scriptClass != null)
            {
                Method method = scriptClass.get_Members().get_Item("Run");
                ExpressionStatement statement = this.LastExpressionStatement(method);
                if ((statement != null) && !this.IsVoid(statement.get_Expression()))
                {
                    statement.get_ParentNode().Replace(statement, new ReturnStatement(statement.get_Expression()));
                }
            }
        }
    }
}

