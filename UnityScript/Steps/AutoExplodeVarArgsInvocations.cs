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
            IEntityWithParameters entity = node.Target.Entity as IEntityWithParameters;
            if (entity != null)
            {
                ExpressionCollection arguments = node.Arguments;
                if (entity.AcceptVarArgs && UnityCallableResolutionServiceModule.IsArrayArgumentExplicitlyProvided(entity.GetParameters(), arguments))
                {
                    UnaryExpression expression2;
                    Expression expression = arguments[-1];
                    UnaryExpression expression1 = expression2 = new UnaryExpression();
                    int num1 = (int) (expression2.Operator = UnaryOperatorType.Explode);
                    Expression expression4 = expression2.Operand = expression;
                    IType type1 = expression2.ExpressionType = this.GetExpressionType(expression);
                    arguments.ReplaceAt(-1, expression2);
                }
            }
        }

        public override void Run()
        {
            this.Visit(this.CompileUnit);
        }
    }
}

