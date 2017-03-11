namespace UnityScriptUpdater
{
    using APIUpdater.Framework.Core.Replacements;
    using Boo.Lang.Compiler.Ast;
    using Boo.Lang.Compiler.TypeSystem;
    using BooUpdater;
    using System;

    public class UnityScriptLanguageTraits : BooBasedLanguageTraits
    {
        internal static BooBasedLanguageTraits Instance = new UnityScriptLanguageTraits();

        public override string ArrayTypeReferenceTypeName(ArrayTypeReference arrayReference) => 
            arrayReference.ElementType.ToString();

        public override int ArtificialAstNodeLength(Node node)
        {
            MethodInvocationExpression expression = (MethodInvocationExpression) node;
            return ((expression.Target.Entity.EntityType != EntityType.Constructor) ? 0 : this.NewExpression.Length);
        }

        private bool IsExpandedDefaultSwitchCase(Statement candidateBlock, Statement parentStatement)
        {
            Block block = candidateBlock as Block;
            if (block == null)
            {
                return false;
            }
            int index = block.Statements.IndexOf(parentStatement);
            if (index <= 0)
            {
                return false;
            }
            LabelStatement statement = block.Statements[index - 1] as LabelStatement;
            return ((statement != null) && statement.Name.Contains("$switch$"));
        }

        private bool IsExpandedSwitchStatementCase(Node node)
        {
            IfStatement parentNode = node.ParentNode as IfStatement;
            if (parentNode == null)
            {
                return false;
            }
            BinaryExpression condition = parentNode.Condition as BinaryExpression;
            if ((condition == null) || (condition.Left.NodeType != NodeType.ReferenceExpression))
            {
                return false;
            }
            ReferenceExpression left = condition.Left as ReferenceExpression;
            return left?.Name.Contains("$switch$");
        }

        private bool IsInsideBlock(Expression node)
        {
            Statement parentStmt = node.FindRootStatement();
            Statement parentNode = (Statement) parentStmt.ParentNode;
            return (((this.IsSwitchCondition(parentNode, parentStmt) || this.IsExpandedSwitchStatementCase(parentNode)) || this.IsExpandedDefaultSwitchCase(parentNode, parentStmt)) || ((parentNode.NodeType == NodeType.Block) && (parentNode.EndSourceLocation.Line != -1)));
        }

        private bool IsSwitchCondition(Statement candidateBlock, Statement parentStmt)
        {
            Block block = candidateBlock as Block;
            if (block == null)
            {
                return false;
            }
            Block parentNode = block.ParentNode as Block;
            return ((parentNode != null) && (parentNode.ParentNode.NodeType == NodeType.Method));
        }

        public override void WrapStatementsInBlockIfNeeded(MemberReferenceExpression node, IUpdateCollector<LexicalInfo> updateCollector)
        {
            if (!this.IsInsideBlock(node))
            {
                Statement statement = node.FindRootStatement();
                updateCollector.Insert(statement.FindExpressionRoot().SourcePosition(), "{ ", node.LexicalInfo, null).InclusiveRange = false;
                updateCollector.Insert(new SourcePosition(statement.EndSourceLocation.Line, statement.EndSourceLocation.Column + 1), " }", node.LexicalInfo, null);
            }
        }

        public override int ArrayReferenceTypeNameOffset =>
            0;

        public override string EmptyStatement =>
            "";

        public override char ImportSeparator =>
            ';';

        public override string NewExpression =>
            "new ";

        public override string StatementSeparator =>
            "; ";

        public override string VarDeclaration =>
            "var ";

        public override string VarDeclarationSeparator =>
            ":";
    }
}

