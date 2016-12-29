namespace UnityScript.Core
{
    using Boo.Lang.Compiler;
    using Boo.Lang.Compiler.Ast;
    using System;
    using System.Text;

    public static class UnityScriptCompilerErrors
    {
        public static CompilerError ClassExpected(LexicalInfo location, string typeName) => 
            CreateError("UCE0006", location, new StringBuilder("'").Append(typeName).Append("' is not a class. 'extends' can only be used with classes. Did you mean 'implements'?").ToString());

        private static CompilerError CreateError(string code, LexicalInfo location, string message) => 
            new CompilerError(code, location, message, null);

        public static CompilerError EvalHasBeenDisabled(LexicalInfo location, string reason) => 
            CreateError("UCE0008", location, reason);

        public static CompilerError InterfaceExpected(LexicalInfo location, string typeName) => 
            CreateError("UCE0005", location, new StringBuilder("'").Append(typeName).Append("' is not an interface. 'implements' can only be used with interfaces. Did you mean 'extends'?").ToString());

        public static CompilerError InvalidPropertyGetter(LexicalInfo location) => 
            CreateError("UCE0004", location, "Property getter cannot declare any arguments.");

        public static CompilerError InvalidPropertySetter(LexicalInfo location) => 
            CreateError("UCE0003", location, "Property setter must have a single argument named 'value'.");

        public static CompilerError KeywordCannotBeUsedAsAnIdentifier(LexicalInfo location, string keyword) => 
            CreateError("UCE0007", location, new StringBuilder("'").Append(keyword).Append("' keyword cannot be used as an identifier.").ToString());

        public static CompilerError SemicolonExpected(LexicalInfo location) => 
            CreateError("UCE0001", location, "';' expected. Insert a semicolon at the end.");

        public static CompilerError SetterCanNotDeclareReturnType(LexicalInfo location) => 
            CreateError("UCE0009", location, "Property setter can not declare return type.");

        public static CompilerError UnknownPragma(LexicalInfo location, string pragma) => 
            CreateError("UCE0002", location, new StringBuilder("Unknown pragma '").Append(pragma).Append("'.").ToString());
    }
}

