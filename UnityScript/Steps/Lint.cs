namespace UnityScript.Steps
{
    using Boo.Lang.Compiler.Ast;
    using Boo.Lang.Compiler.Steps;
    using System;
    using UnityScript.Core;

    [Serializable]
    public class Lint : AbstractFastVisitorCompilerStep
    {
        public bool IsBoolean(Expression e) => 
            (e.ExpressionType == this.TypeSystemServices.BoolType);

        public override void OnBinaryExpression(BinaryExpression node)
        {
            base.OnBinaryExpression(node);
            BinaryOperatorType @operator = node.Operator;
            if (((@operator == BinaryOperatorType.BitwiseAnd) || (@operator == BinaryOperatorType.BitwiseOr)) && (this.IsBoolean(node.Left) && this.IsBoolean(node.Right)))
            {
                string[] strArray = (node.Operator != BinaryOperatorType.BitwiseAnd) ? new string[] { "||", "|" } : new string[] { "&&", "&" };
                string expectedOperator = strArray[0];
                string actualOperator = strArray[1];
                this.Warnings.Add(UnityScriptWarnings.BitwiseOperatorWithBooleanOperands(node.LexicalInfo, expectedOperator, actualOperator));
            }
        }
    }
}

