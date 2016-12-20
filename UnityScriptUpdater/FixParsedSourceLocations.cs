namespace UnityScriptUpdater
{
    using APIUpdater.Framework.Log;
    using Boo.Lang.Compiler.Ast;
    using BooUpdater;
    using System;
    using System.Collections.Generic;

    public class FixParsedSourceLocations : FixParsedSourceLocations
    {
        public FixParsedSourceLocations(int tabSize, Dictionary<string, SourceFile> sources, IAPIUpdaterListener listener) : base(tabSize, sources, listener)
        {
            base.traits = new UnityScriptLanguageTraits();
        }

        private bool IsConstructorInvocation(MethodInvocationExpression node)
        {
            return ((node.get_Target().get_Entity() != null) && (node.get_Target().get_Entity().get_EntityType() == 0x10));
        }

        public override void OnExpressionStatement(ExpressionStatement node)
        {
            base.OnExpressionStatement(node);
            base.EnsureDocumentInitialized(node);
            Expression expression = node.get_Expression();
            SourceLocation self = base.doc.TokenSourceLocationFollowing(BooExtensions.AsLexicalInfo(expression.get_EndSourceLocation(), null), ";");
            if (self != null)
            {
                node.set_EndSourceLocation(BooExtensions.OffsetedBy(self, 0, -2));
            }
        }

        public override void OnMethodInvocationExpression(MethodInvocationExpression node)
        {
            base.OnMethodInvocationExpression(node);
            if (this.IsConstructorInvocation(node))
            {
                base.EnsureDocumentInitialized(node);
                node.set_LexicalInfo(BooExtensions.AsLexicalInfo(base.doc.FindPrevious(node.get_LexicalInfo(), 'n'), node.get_LexicalInfo().get_FileName()));
            }
        }
    }
}

