namespace UnityScript.Steps
{
    using Boo.Lang.Compiler.Ast;
    using Boo.Lang.Compiler.Steps;
    using System;
    using UnityScript.Core;

    [Serializable]
    public class Lint : AbstractFastVisitorCompilerStep
    {
        public bool IsBoolean(Expression e)
        {
            return (e.get_ExpressionType() == this.get_TypeSystemServices().BoolType);
        }

        public override void OnBinaryExpression(BinaryExpression node)
        {
            base.OnBinaryExpression(node);
            BinaryOperatorType type = node.get_Operator();
            if (((type == 0x1f) || (type == 30)) && (this.IsBoolean(node.get_Left()) && this.IsBoolean(node.get_Right())))
            {
                string[] strArray = (node.get_Operator() != 0x1f) ? new string[] { "||", "|" } : new string[] { "&&", "&" };
                string expectedOperator = strArray[0];
                string actualOperator = strArray[1];
                this.get_Warnings().Add(UnityScriptWarnings.BitwiseOperatorWithBooleanOperands(node.get_LexicalInfo(), expectedOperator, actualOperator));
            }
        }
    }
}

