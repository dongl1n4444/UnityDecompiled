namespace UnityScript.Macros
{
    using Boo.Lang.Compiler.Ast;
    using Boo.Lang.Runtime;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    [CompilerGlobalScope]
    public sealed class SwitchMacroModule
    {
        private SwitchMacroModule()
        {
        }

        public static Expression ComparisonFor(Expression local, IEnumerable<Expression> expressions)
        {
            BinaryExpression expression;
            IEnumerator<Expression> enumerator = expressions.GetEnumerator();
            if (!enumerator.MoveNext())
            {
                throw new AssertionFailedException("e.MoveNext()");
            }
            BinaryExpression expression1 = expression = new BinaryExpression(LexicalInfo.Empty);
            int num1 = (int) (expression.Operator = BinaryOperatorType.Equality);
            Expression expression11 = expression.Left = Expression.Lift(local);
            Expression expression12 = expression.Right = Expression.Lift(enumerator.Current);
            Expression e = expression;
            while (enumerator.MoveNext())
            {
                BinaryExpression expression3;
                BinaryExpression expression4;
                BinaryExpression expression13 = expression4 = new BinaryExpression(LexicalInfo.Empty);
                int num2 = (int) (expression4.Operator = BinaryOperatorType.Or);
                Expression expression14 = expression4.Left = Expression.Lift(e);
                BinaryExpression expression15 = expression3 = new BinaryExpression(LexicalInfo.Empty);
                int num3 = (int) (expression3.Operator = BinaryOperatorType.Equality);
                Expression expression16 = expression3.Left = Expression.Lift(local);
                Expression expression17 = expression3.Right = Expression.Lift(enumerator.Current);
                BinaryExpression expression18 = expression4.Right = expression3;
                e = expression4;
            }
            return e;
        }

        public static bool EndsWithBreak(Block block) => 
            ((block.Statements.Count != 0) ? (block.Statements[-1] is BreakStatement) : false);

        public static GotoStatement NewGoto(LabelStatement label)
        {
            GotoStatement statement;
            GotoStatement statement1 = statement = new GotoStatement();
            ReferenceExpression expression1 = statement.Label = new ReferenceExpression(label.Name);
            return statement;
        }
    }
}

