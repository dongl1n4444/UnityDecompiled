namespace UnityScriptUpdater
{
    using APIUpdater.Framework.Core.Replacements;
    using Boo.Lang.Compiler.Ast;
    using BooUpdater;
    using System;

    internal class MethodSignatureChanger : MethodSignatureChangerBase
    {
        public MethodSignatureChanger(BooUpdateContext context) : base(context)
        {
            base.LanguageTraits = new UnityScriptLanguageTraits();
        }

        private void AddStatementSeparatorAfterJustMovedArgumentExpression(Node argument, LexicalInfo expressionStatementStart)
        {
            base.UpdateCollector.Insert(argument.get_EndSourceLocation().OffsetedBy(0, 1).SourcePosition(), base.LanguageTraits.StatementSeparator, argument.get_LexicalInfo(), expressionStatementStart);
        }

        private void AddVariableDeclarationToHoldArgumentExpression(Expression argument, Node argRootExp, string varName)
        {
            base.UpdateCollector.Insert(argRootExp.SourcePosition(), base.LanguageTraits.VarDeclaration + varName + " = ", argument.get_LexicalInfo(), null);
        }

        private static SourceRange ExtraSourceCodeRangeToCleanup(Expression argument, Expression previousArg, Expression nextArg)
        {
            if (previousArg != null)
            {
                return new SourceRange(previousArg.get_EndSourceLocation().OffsetedBy(0, 1).SourcePosition(), MethodSignatureChangerBase.FindExpressionStart(argument).OffsetedBy(0, -1).SourcePosition());
            }
            return new SourceRange(argument.get_EndSourceLocation().OffsetedBy(0, 1).SourcePosition(), MethodSignatureChangerBase.FindExpressionStart(nextArg).OffsetedBy(0, -1).SourcePosition());
        }

        private bool HasNoSeparatorLeftToRemove(int argumentCount, int index) => 
            ((argumentCount == 1) || ((index == base.RemovingAllArgsStartingAt) && (index == 0)));

        private void MoveArgumentExpressionOutsideCall(Expression argument, Node argRootExp, LexicalInfo invExpStart)
        {
            LexicalInfo updateId = MethodSignatureChangerBase.FindExpressionStart(base.Invocation);
            base.UpdateCollector.Move(new SourceRange(argRootExp.SourcePosition(), argument.get_EndSourceLocation().OffsetedBy(0, 2).SourcePosition()), invExpStart.SourcePosition(), updateId, null).InclusiveRange = false;
        }

        protected override void RemoveArgumentKeepingPossibleSideEffects(int argIndexToRemove, string methodName)
        {
            Expression expression = base.Argument(argIndexToRemove);
            Node argRootExp = expression.FindExpressionRoot();
            LexicalInfo invExpStart = MethodSignatureChangerBase.FindExpressionStart(expression.FindRootStatement());
            this.MoveArgumentExpressionOutsideCall(expression, argRootExp, invExpStart);
            this.AddStatementSeparatorAfterJustMovedArgumentExpression(expression, invExpStart);
            this.AddVariableDeclarationToHoldArgumentExpression(expression, argRootExp, base.VariableNameFor(methodName, expression));
            this.RemoveArgumentSeparator(argIndexToRemove);
        }

        private void RemoveArgumentSeparator(int argIndex)
        {
            if (!this.HasNoSeparatorLeftToRemove(base.Invocation.get_Arguments().get_Count(), argIndex))
            {
                Expression argument = base.Argument(argIndex);
                Expression nextArg = base.NextArgument(argIndex);
                Expression previousArg = base.PreviousArgument(argIndex);
                SourceRange range = ExtraSourceCodeRangeToCleanup(argument, previousArg, nextArg);
                base.UpdateCollector.Remove(range, argument.get_LexicalInfo(), null);
            }
        }
    }
}

