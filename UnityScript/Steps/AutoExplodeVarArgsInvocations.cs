namespace UnityScript.Steps
{
    using Boo.Lang.Compiler.Ast;
    using Boo.Lang.Compiler.Steps;
    using Boo.Lang.Compiler.TypeSystem;
    using System;
    using UnityScript.TypeSystem;

    [Serializable]
    public class AutoExplodeVarArgsInvocations : AbstractVisitorCompilerStep
    {
        public override void LeaveMethodInvocationExpression(MethodInvocationExpression node)
        {
            IEntityWithParameters parameters = node.get_Target().get_Entity() as IEntityWithParameters;
            if (parameters != null)
            {
                ExpressionCollection args = node.get_Arguments();
                if (parameters.get_AcceptVarArgs() && UnityCallableResolutionServiceModule.IsArrayArgumentExplicitlyProvided(parameters.GetParameters(), args))
                {
                    UnaryExpression expression2;
                    Expression expression = args.get_Item(-1);
                    UnaryExpression expression1 = expression2 = new UnaryExpression();
                    expression2.set_Operator(7);
                    expression2.set_Operand(expression);
                    expression2.set_ExpressionType(this.GetExpressionType(expression));
                    args.ReplaceAt(-1, expression2);
                }
            }
        }

        public override void Run()
        {
            this.Visit(this.get_CompileUnit());
        }
    }
}

