namespace UnityEditor.Scripting
{
    using ICSharpCode.NRefactory.Ast;
    using ICSharpCode.NRefactory.Visitors;
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    internal class InvalidTypeOrNamespaceErrorTypeMapper : AbstractAstVisitor
    {
        private readonly int _column;
        private readonly int _line;
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private string <Found>k__BackingField;

        private InvalidTypeOrNamespaceErrorTypeMapper(int line, int column)
        {
            this._line = line;
            this._column = column;
        }

        public static string IsTypeMovedToNamespaceError(CompilationUnit cu, int line, int column)
        {
            InvalidTypeOrNamespaceErrorTypeMapper visitor = new InvalidTypeOrNamespaceErrorTypeMapper(line, column);
            cu.AcceptVisitor(visitor, null);
            return visitor.Found;
        }

        public override object VisitTypeReference(TypeReference typeReference, object data)
        {
            bool flag = (this._column >= typeReference.StartLocation.Column) && (this._column < (typeReference.StartLocation.Column + typeReference.Type.Length));
            if ((typeReference.StartLocation.Line == this._line) && flag)
            {
                this.Found = typeReference.Type;
                return true;
            }
            return base.VisitTypeReference(typeReference, data);
        }

        public string Found { get; private set; }
    }
}

