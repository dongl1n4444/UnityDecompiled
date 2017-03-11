namespace UnityScriptUpdater
{
    using APIUpdater.Framework.Log;
    using Boo.Lang.Compiler.Ast;
    using Boo.Lang.Compiler.TypeSystem;
    using BooUpdater;
    using System;
    using System.Collections.Generic;

    public class FixParsedSourceLocations : BooUpdater.FixParsedSourceLocations
    {
        public FixParsedSourceLocations(int tabSize, Dictionary<string, SourceFile> sources, IAPIUpdaterListener listener) : base(tabSize, sources, listener)
        {
            base.traits = new UnityScriptLanguageTraits();
        }

        private bool IsConstructorInvocation(MethodInvocationExpression node) => 
            ((node.Target.Entity != null) && (node.Target.Entity.EntityType == EntityType.Constructor));

        public override void OnExpressionStatement(ExpressionStatement node)
        {
            base.OnExpressionStatement(node);
            base.EnsureDocumentInitialized(node);
            Expression expression = node.Expression;
            SourceLocation self = base.doc.TokenSourceLocationFollowing(expression.EndSourceLocation.AsLexicalInfo(null), ";");
            if (self != null)
            {
                node.EndSourceLocation = self.OffsetedBy(0, -2);
            }
        }

        public override void OnMethodInvocationExpression(MethodInvocationExpression node)
        {
            base.OnMethodInvocationExpression(node);
            if (this.IsConstructorInvocation(node))
            {
                base.EnsureDocumentInitialized(node);
                node.LexicalInfo = base.doc.FindPrevious(node.LexicalInfo, 'n').AsLexicalInfo(node.LexicalInfo.FileName);
            }
        }
    }
}

