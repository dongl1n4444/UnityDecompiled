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
            expression.set_Operator(11);
            expression.set_Left(Expression.Lift(local));
            expression.set_Right(Expression.Lift(enumerator.Current));
            Expression expression2 = expression;
            while (enumerator.MoveNext())
            {
                BinaryExpression expression3;
                BinaryExpression expression4;
                BinaryExpression expression11 = expression4 = new BinaryExpression(LexicalInfo.Empty);
                expression4.set_Operator(0x1c);
                expression4.set_Left(Expression.Lift(expression2));
                BinaryExpression expression12 = expression3 = new BinaryExpression(LexicalInfo.Empty);
                expression3.set_Operator(11);
                expression3.set_Left(Expression.Lift(local));
                expression3.set_Right(Expression.Lift(enumerator.Current));
                expression4.set_Right(expression3);
                expression2 = expression4;
            }
            return expression2;
        }

        public static bool EndsWithBreak(Block block) => 
            ((block.get_Statements().Count != 0) ? (block.get_Statements().get_Item(-1) is BreakStatement) : false);

        public static GotoStatement NewGoto(LabelStatement label)
        {
            GotoStatement statement;
            GotoStatement statement1 = statement = new GotoStatement();
            statement.set_Label(new ReferenceExpression(label.get_Name()));
            return statement;
        }
    }
}

