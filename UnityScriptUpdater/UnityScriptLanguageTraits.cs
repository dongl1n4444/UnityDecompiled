namespace UnityScriptUpdater
{
    using APIUpdater.Framework.Core.Replacements;
    using Boo.Lang.Compiler.Ast;
    using BooUpdater;
    using System;

    public class UnityScriptLanguageTraits : BooBasedLanguageTraits
    {
        public override string ArrayTypeReferenceTypeName(ArrayTypeReference arrayReference)
        {
            return arrayReference.get_ElementType().ToString();
        }

        public override int ArtificialAstNodeLength(Node node)
        {
            MethodInvocationExpression expression = node;
            return ((expression.get_Target().get_Entity().get_EntityType() != 0x10) ? 0 : this.NewExpression.Length);
        }

        private bool IsExpandedDefaultSwitchCase(Statement candidateBlock, Statement parentStatement)
        {
            Block block = candidateBlock as Block;
            if (block == null)
            {
                return false;
            }
            int index = block.get_Statements().IndexOf(parentStatement);
            if (index <= 0)
            {
                return false;
            }
            LabelStatement statement = block.get_Statements().get_Item(index - 1) as LabelStatement;
            return ((statement != null) && statement.get_Name().Contains("$switch$"));
        }

        private bool IsExpandedSwitchStatementCase(Node node)
        {
            IfStatement statement = node.get_ParentNode() as IfStatement;
            if (statement == null)
            {
                return false;
            }
            BinaryExpression expression = statement.get_Condition() as BinaryExpression;
            if ((expression == null) || (expression.get_Left().get_NodeType() != 0x36))
            {
                return false;
            }
            ReferenceExpression expression2 = expression.get_Left() as ReferenceExpression;
            if (expression2 == null)
            {
                return false;
            }
            return expression2.get_Name().Contains("$switch$");
        }

        private bool IsInsideBlock(Expression node)
        {
            Statement parentStmt = BooExtensions.FindRootStatement(node);
            Statement candidateBlock = parentStmt.get_ParentNode();
            return (((this.IsSwitchCondition(candidateBlock, parentStmt) || this.IsExpandedSwitchStatementCase(candidateBlock)) || this.IsExpandedDefaultSwitchCase(candidateBlock, parentStmt)) || ((candidateBlock.get_NodeType() == 0x20) && (candidateBlock.get_EndSourceLocation().get_Line() != -1)));
        }

        private bool IsSwitchCondition(Statement candidateBlock, Statement parentStmt)
        {
            Block block = candidateBlock as Block;
            if (block == null)
            {
                return false;
            }
            Block block2 = block.get_ParentNode() as Block;
            return ((block2 != null) && (block2.get_ParentNode().get_NodeType() == 0x16));
        }

        public override void WrapStatementsInBlockIfNeeded(MemberReferenceExpression node, IUpdateCollector<LexicalInfo> updateCollector)
        {
            if (!this.IsInsideBlock(node))
            {
                Statement statement = BooExtensions.FindRootStatement(node);
                updateCollector.Insert(BooExtensions.SourcePosition(BooExtensions.FindExpressionRoot(statement)), "{ ", node.get_LexicalInfo(), null).InclusiveRange = false;
                updateCollector.Insert(new SourcePosition(statement.get_EndSourceLocation().get_Line(), statement.get_EndSourceLocation().get_Column() + 1), " }", node.get_LexicalInfo(), null);
            }
        }

        public override int ArrayReferenceTypeNameOffset
        {
            get
            {
                return 0;
            }
        }

        public override string EmptyStatement
        {
            get
            {
                return "";
            }
        }

        public override char ImportSeparator
        {
            get
            {
                return ';';
            }
        }

        public override string NewExpression
        {
            get
            {
                return "new ";
            }
        }

        public override string StatementSeparator
        {
            get
            {
                return "; ";
            }
        }

        public override string VarDeclaration
        {
            get
            {
                return "var ";
            }
        }

        public override string VarDeclarationSeparator
        {
            get
            {
                return ":";
            }
        }
    }
}

