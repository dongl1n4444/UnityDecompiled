namespace UnityScript.Parser
{
    using antlr;
    using antlr.collections.impl;
    using Boo.Lang;
    using Boo.Lang.Compiler;
    using Boo.Lang.Compiler.Ast;
    using Boo.Lang.Parser;
    using Boo.Lang.Runtime;
    using System;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using UnityScript;
    using UnityScript.Core;

    [Serializable]
    public class UnityScriptParser : LLkParser
    {
        private bool $initialized__UnityScript_Parser_UnityScriptParser$;
        protected AttributeCollection _attributes;
        protected Boo.Lang.Compiler.CompilerContext _context;
        protected IToken _last;
        protected List _loopStack;
        [NonSerialized]
        public const int ADD = 0x51;
        [NonSerialized]
        public const int AS = 4;
        [NonSerialized]
        public const int ASSEMBLY_ATTRIBUTE_MARKER = 0x65;
        [NonSerialized]
        public const int ASSIGN = 0x4e;
        [NonSerialized]
        public const int AT = 0x63;
        [NonSerialized]
        public const int BITWISE_AND = 0x48;
        [NonSerialized]
        public const int BITWISE_NOT = 0x58;
        [NonSerialized]
        public const int BITWISE_OR = 70;
        [NonSerialized]
        public const int BITWISE_XOR = 0x49;
        [NonSerialized]
        public const int BREAK = 5;
        [NonSerialized]
        public const int CASE = 50;
        [NonSerialized]
        public const int CAST = 6;
        [NonSerialized]
        public const int CATCH = 7;
        [NonSerialized]
        public const int CLASS = 8;
        [NonSerialized]
        public const int COLON = 0x42;
        [NonSerialized]
        public const int COMMA = 0x43;
        [NonSerialized]
        public const int CONTINUE = 9;
        [NonSerialized]
        public const int DECREMENT = 80;
        [NonSerialized]
        public const int DEFAULT = 0x33;
        [NonSerialized]
        public const int DIGIT = 0x7a;
        [NonSerialized]
        public const int DIVISION = 0x68;
        [NonSerialized]
        public const int DO = 10;
        [NonSerialized]
        public const int DOT = 0x41;
        [NonSerialized]
        public const int DOUBLE = 0x6a;
        [NonSerialized]
        public const int DOUBLE_QUOTED_STRING = 60;
        [NonSerialized]
        public const int DOUBLE_SUFFIX = 110;
        [NonSerialized]
        public const int DQS_ESC = 0x72;
        [NonSerialized]
        public const int EACH = 12;
        [NonSerialized]
        public const int ELSE = 11;
        [NonSerialized]
        public const int ENUM = 13;
        [NonSerialized]
        public const int EOF = 1;
        [NonSerialized]
        public const int EOS = 0x4d;
        [NonSerialized]
        public const int EQUALITY = 0x55;
        [NonSerialized]
        public const int EXPONENT = 0x6f;
        [NonSerialized]
        public const int EXTENDS = 14;
        [NonSerialized]
        public const int FALSE = 15;
        [NonSerialized]
        public const int FINAL = 0x10;
        [NonSerialized]
        public const int FINALLY = 0x11;
        [NonSerialized]
        public const int FOR = 0x12;
        [NonSerialized]
        public const int FUNCTION = 0x13;
        [NonSerialized]
        public const int GET = 20;
        [NonSerialized]
        public const int GREATER_THAN = 0x5f;
        [NonSerialized]
        public const int GREATER_THAN_OR_EQUAL = 0x60;
        [NonSerialized]
        public const int HEXDIGIT = 0x7b;
        [NonSerialized]
        public const int ID = 0x3b;
        [NonSerialized]
        public const int ID_LETTER = 0x79;
        [NonSerialized]
        public const int IF = 0x15;
        [NonSerialized]
        public const int IMPLEMENTS = 0x17;
        [NonSerialized]
        public const int IMPORT = 0x16;
        [NonSerialized]
        public const int IN = 0x18;
        [NonSerialized]
        public const int INCREMENT = 0x4f;
        [NonSerialized]
        public const int INEQUALITY = 0x56;
        [NonSerialized]
        public const int INPLACE_ADD = 0x35;
        [NonSerialized]
        public const int INPLACE_BITWISE_AND = 0x4a;
        [NonSerialized]
        public const int INPLACE_BITWISE_OR = 0x47;
        [NonSerialized]
        public const int INPLACE_BITWISE_XOR = 0x66;
        [NonSerialized]
        public const int INPLACE_DIVISION = 0x34;
        [NonSerialized]
        public const int INPLACE_MULTIPLY = 0x37;
        [NonSerialized]
        public const int INPLACE_SHIFT_LEFT = 0x5e;
        [NonSerialized]
        public const int INPLACE_SHIFT_RIGHT = 0x62;
        [NonSerialized]
        public const int INPLACE_SUBTRACT = 0x36;
        [NonSerialized]
        public const int INSTANCEOF = 0x1a;
        [NonSerialized]
        public const int INT = 0x6b;
        [NonSerialized]
        public const int INTERFACE = 0x19;
        [NonSerialized]
        public const int INTERNAL = 0x21;
        [NonSerialized]
        public const int LBRACE = 0x3d;
        [NonSerialized]
        public const int LBRACK = 0x44;
        [NonSerialized]
        public const int LESS_THAN = 0x5b;
        [NonSerialized]
        public const int LESS_THAN_OR_EQUAL = 0x5c;
        [NonSerialized]
        public const int LOGICAL_AND = 0x4c;
        [NonSerialized]
        public const int LOGICAL_NOT = 0x67;
        [NonSerialized]
        public const int LOGICAL_OR = 0x4b;
        [NonSerialized]
        public const int LONG = 0x6c;
        [NonSerialized]
        public const int LPAREN = 0x3f;
        [NonSerialized]
        public const int ML_COMMENT = 0x75;
        [NonSerialized]
        public const int MODULUS = 0x53;
        [NonSerialized]
        public const int MULTIPLY = 0x54;
        [NonSerialized]
        public const int NEW = 0x1b;
        [NonSerialized]
        public const int NEWLINE = 120;
        [NonSerialized]
        public const int NOT = 0x1c;
        [NonSerialized]
        public const int NULL = 0x1d;
        [NonSerialized]
        public const int NULL_TREE_LOOKAHEAD = 3;
        [NonSerialized]
        public const int OVERRIDE = 0x22;
        [NonSerialized]
        public const int PARTIAL = 0x23;
        [NonSerialized]
        public const int PRAGMA_OFF = 0x3a;
        [NonSerialized]
        public const int PRAGMA_ON = 0x39;
        [NonSerialized]
        public const int PRAGMA_WHITE_SPACE = 0x70;
        [NonSerialized]
        public const int PRIVATE = 0x24;
        [NonSerialized]
        public const int PROTECTED = 0x20;
        [NonSerialized]
        public const int PUBLIC = 0x1f;
        [NonSerialized]
        public const int QUESTION_MARK = 0x57;
        [NonSerialized]
        public const int RBRACE = 0x3e;
        [NonSerialized]
        public const int RBRACK = 0x45;
        [NonSerialized]
        public const int RE_CHAR = 0x76;
        [NonSerialized]
        public const int RE_ESC = 0x77;
        [NonSerialized]
        public const int RE_LITERAL = 0x69;
        [NonSerialized]
        public const int REFERENCE_EQUALITY = 0x59;
        [NonSerialized]
        public const int REFERENCE_INEQUALITY = 90;
        [NonSerialized]
        public const int RETURN = 30;
        [NonSerialized]
        public const int RPAREN = 0x40;
        [NonSerialized]
        public const int SCRIPT_ATTRIBUTE_MARKER = 100;
        [NonSerialized]
        public const int SESC = 0x74;
        [NonSerialized]
        public const int SET = 0x25;
        [NonSerialized]
        public const int SHIFT_LEFT = 0x5d;
        [NonSerialized]
        public const int SHIFT_RIGHT = 0x61;
        [NonSerialized]
        public const int SINGLE_QUOTED_STRING = 0x6d;
        [NonSerialized]
        public const int SL_COMMENT = 0x38;
        [NonSerialized]
        public const int SQS_ESC = 0x73;
        [NonSerialized]
        public const int STATIC = 0x26;
        [NonSerialized]
        public const int SUBTRACT = 0x52;
        [NonSerialized]
        public const int SUPER = 0x27;
        [NonSerialized]
        public const int SWITCH = 0x31;
        [NonSerialized]
        public const int THIS = 40;
        [NonSerialized]
        public const int THROW = 0x29;
        [NonSerialized]
        public static readonly string[] tokenNames_ = new string[] { 
            "<0>", "EOF", "<2>", "NULL_TREE_LOOKAHEAD", "as", "break", "cast", "catch", "class", "continue", "do", "else", "each", "enum", "extends", "false",
            "final", "finally", "for", "function", "get", "if", "import", "implements", "in", "interface", "instanceof", "new", "not", "null", "return", "public",
            "protected", "internal", "override", "partial", "private", "set", "static", "super", "this", "throw", "true", "try", "typeof", "var", "virtual", "while",
            "yield", "switch", "case", "default", "/=", "+=", "-=", "*=", "//", "pragma on", "pragma off", "an identifier", "a string", "{", "}", "(",
            ")", ".", ":", ",", "[", "]", "|", "|=", "&", "^", "&=", "||", "&&", ";", "=", "++",
            "--", "+", "-", "%", "*", "==", "!=", "?", "~", "===", "!==", "<", "<=", "<<", "<<=", ">",
            ">=", ">>", ">>=", "@", "@script", "@assembly", "INPLACE_BITWISE_XOR", "LOGICAL_NOT", "DIVISION", "RE_LITERAL", "DOUBLE", "INT", "LONG", "SINGLE_QUOTED_STRING", "DOUBLE_SUFFIX", "EXPONENT",
            "PRAGMA_WHITE_SPACE", "WHITE_SPACE", "DQS_ESC", "SQS_ESC", "SESC", "ML_COMMENT", "RE_CHAR", "RE_ESC", "NEWLINE", "ID_LETTER", "DIGIT", "HEXDIGIT"
        };
        [NonSerialized]
        public static readonly BitSet tokenSet_0_ = new BitSet(mk_tokenSet_0_());
        [NonSerialized]
        public static readonly BitSet tokenSet_1_ = new BitSet(mk_tokenSet_1_());
        [NonSerialized]
        public static readonly BitSet tokenSet_10_ = new BitSet(mk_tokenSet_10_());
        [NonSerialized]
        public static readonly BitSet tokenSet_11_ = new BitSet(mk_tokenSet_11_());
        [NonSerialized]
        public static readonly BitSet tokenSet_12_ = new BitSet(mk_tokenSet_12_());
        [NonSerialized]
        public static readonly BitSet tokenSet_13_ = new BitSet(mk_tokenSet_13_());
        [NonSerialized]
        public static readonly BitSet tokenSet_14_ = new BitSet(mk_tokenSet_14_());
        [NonSerialized]
        public static readonly BitSet tokenSet_15_ = new BitSet(mk_tokenSet_15_());
        [NonSerialized]
        public static readonly BitSet tokenSet_16_ = new BitSet(mk_tokenSet_16_());
        [NonSerialized]
        public static readonly BitSet tokenSet_17_ = new BitSet(mk_tokenSet_17_());
        [NonSerialized]
        public static readonly BitSet tokenSet_18_ = new BitSet(mk_tokenSet_18_());
        [NonSerialized]
        public static readonly BitSet tokenSet_19_ = new BitSet(mk_tokenSet_19_());
        [NonSerialized]
        public static readonly BitSet tokenSet_2_ = new BitSet(mk_tokenSet_2_());
        [NonSerialized]
        public static readonly BitSet tokenSet_20_ = new BitSet(mk_tokenSet_20_());
        [NonSerialized]
        public static readonly BitSet tokenSet_21_ = new BitSet(mk_tokenSet_21_());
        [NonSerialized]
        public static readonly BitSet tokenSet_22_ = new BitSet(mk_tokenSet_22_());
        [NonSerialized]
        public static readonly BitSet tokenSet_23_ = new BitSet(mk_tokenSet_23_());
        [NonSerialized]
        public static readonly BitSet tokenSet_24_ = new BitSet(mk_tokenSet_24_());
        [NonSerialized]
        public static readonly BitSet tokenSet_25_ = new BitSet(mk_tokenSet_25_());
        [NonSerialized]
        public static readonly BitSet tokenSet_26_ = new BitSet(mk_tokenSet_26_());
        [NonSerialized]
        public static readonly BitSet tokenSet_27_ = new BitSet(mk_tokenSet_27_());
        [NonSerialized]
        public static readonly BitSet tokenSet_28_ = new BitSet(mk_tokenSet_28_());
        [NonSerialized]
        public static readonly BitSet tokenSet_29_ = new BitSet(mk_tokenSet_29_());
        [NonSerialized]
        public static readonly BitSet tokenSet_3_ = new BitSet(mk_tokenSet_3_());
        [NonSerialized]
        public static readonly BitSet tokenSet_30_ = new BitSet(mk_tokenSet_30_());
        [NonSerialized]
        public static readonly BitSet tokenSet_31_ = new BitSet(mk_tokenSet_31_());
        [NonSerialized]
        public static readonly BitSet tokenSet_32_ = new BitSet(mk_tokenSet_32_());
        [NonSerialized]
        public static readonly BitSet tokenSet_33_ = new BitSet(mk_tokenSet_33_());
        [NonSerialized]
        public static readonly BitSet tokenSet_34_ = new BitSet(mk_tokenSet_34_());
        [NonSerialized]
        public static readonly BitSet tokenSet_35_ = new BitSet(mk_tokenSet_35_());
        [NonSerialized]
        public static readonly BitSet tokenSet_36_ = new BitSet(mk_tokenSet_36_());
        [NonSerialized]
        public static readonly BitSet tokenSet_37_ = new BitSet(mk_tokenSet_37_());
        [NonSerialized]
        public static readonly BitSet tokenSet_38_ = new BitSet(mk_tokenSet_38_());
        [NonSerialized]
        public static readonly BitSet tokenSet_39_ = new BitSet(mk_tokenSet_39_());
        [NonSerialized]
        public static readonly BitSet tokenSet_4_ = new BitSet(mk_tokenSet_4_());
        [NonSerialized]
        public static readonly BitSet tokenSet_40_ = new BitSet(mk_tokenSet_40_());
        [NonSerialized]
        public static readonly BitSet tokenSet_41_ = new BitSet(mk_tokenSet_41_());
        [NonSerialized]
        public static readonly BitSet tokenSet_42_ = new BitSet(mk_tokenSet_42_());
        [NonSerialized]
        public static readonly BitSet tokenSet_43_ = new BitSet(mk_tokenSet_43_());
        [NonSerialized]
        public static readonly BitSet tokenSet_44_ = new BitSet(mk_tokenSet_44_());
        [NonSerialized]
        public static readonly BitSet tokenSet_45_ = new BitSet(mk_tokenSet_45_());
        [NonSerialized]
        public static readonly BitSet tokenSet_46_ = new BitSet(mk_tokenSet_46_());
        [NonSerialized]
        public static readonly BitSet tokenSet_47_ = new BitSet(mk_tokenSet_47_());
        [NonSerialized]
        public static readonly BitSet tokenSet_48_ = new BitSet(mk_tokenSet_48_());
        [NonSerialized]
        public static readonly BitSet tokenSet_49_ = new BitSet(mk_tokenSet_49_());
        [NonSerialized]
        public static readonly BitSet tokenSet_5_ = new BitSet(mk_tokenSet_5_());
        [NonSerialized]
        public static readonly BitSet tokenSet_50_ = new BitSet(mk_tokenSet_50_());
        [NonSerialized]
        public static readonly BitSet tokenSet_51_ = new BitSet(mk_tokenSet_51_());
        [NonSerialized]
        public static readonly BitSet tokenSet_52_ = new BitSet(mk_tokenSet_52_());
        [NonSerialized]
        public static readonly BitSet tokenSet_53_ = new BitSet(mk_tokenSet_53_());
        [NonSerialized]
        public static readonly BitSet tokenSet_54_ = new BitSet(mk_tokenSet_54_());
        [NonSerialized]
        public static readonly BitSet tokenSet_55_ = new BitSet(mk_tokenSet_55_());
        [NonSerialized]
        public static readonly BitSet tokenSet_56_ = new BitSet(mk_tokenSet_56_());
        [NonSerialized]
        public static readonly BitSet tokenSet_57_ = new BitSet(mk_tokenSet_57_());
        [NonSerialized]
        public static readonly BitSet tokenSet_58_ = new BitSet(mk_tokenSet_58_());
        [NonSerialized]
        public static readonly BitSet tokenSet_59_ = new BitSet(mk_tokenSet_59_());
        [NonSerialized]
        public static readonly BitSet tokenSet_6_ = new BitSet(mk_tokenSet_6_());
        [NonSerialized]
        public static readonly BitSet tokenSet_60_ = new BitSet(mk_tokenSet_60_());
        [NonSerialized]
        public static readonly BitSet tokenSet_7_ = new BitSet(mk_tokenSet_7_());
        [NonSerialized]
        public static readonly BitSet tokenSet_8_ = new BitSet(mk_tokenSet_8_());
        [NonSerialized]
        public static readonly BitSet tokenSet_9_ = new BitSet(mk_tokenSet_9_());
        [NonSerialized]
        public const int TRUE = 0x2a;
        [NonSerialized]
        public const int TRY = 0x2b;
        [NonSerialized]
        public const int TYPEOF = 0x2c;
        [NonSerialized]
        public const int VAR = 0x2d;
        [NonSerialized]
        public const int VIRTUAL = 0x2e;
        [NonSerialized]
        public const int WHILE = 0x2f;
        [NonSerialized]
        public const int WHITE_SPACE = 0x71;
        [NonSerialized]
        public const int YIELD = 0x30;

        public UnityScriptParser(ParserSharedInputState state) : base(state, 2)
        {
            if (!this.$initialized__UnityScript_Parser_UnityScriptParser$)
            {
                this._attributes = new AttributeCollection();
                this._loopStack = new List();
                this.$initialized__UnityScript_Parser_UnityScriptParser$ = true;
            }
            this.initialize();
        }

        public UnityScriptParser(TokenBuffer tokenBuf) : this(tokenBuf, 2)
        {
            if (!this.$initialized__UnityScript_Parser_UnityScriptParser$)
            {
                this._attributes = new AttributeCollection();
                this._loopStack = new List();
                this.$initialized__UnityScript_Parser_UnityScriptParser$ = true;
            }
        }

        public UnityScriptParser(TokenStream lexer) : this(lexer, 2)
        {
            if (!this.$initialized__UnityScript_Parser_UnityScriptParser$)
            {
                this._attributes = new AttributeCollection();
                this._loopStack = new List();
                this.$initialized__UnityScript_Parser_UnityScriptParser$ = true;
            }
        }

        protected UnityScriptParser(TokenBuffer tokenBuf, int k) : base(tokenBuf, k)
        {
            if (!this.$initialized__UnityScript_Parser_UnityScriptParser$)
            {
                this._attributes = new AttributeCollection();
                this._loopStack = new List();
                this.$initialized__UnityScript_Parser_UnityScriptParser$ = true;
            }
            this.initialize();
        }

        protected UnityScriptParser(TokenStream lexer, int k) : base(lexer, k)
        {
            if (!this.$initialized__UnityScript_Parser_UnityScriptParser$)
            {
                this._attributes = new AttributeCollection();
                this._loopStack = new List();
                this.$initialized__UnityScript_Parser_UnityScriptParser$ = true;
            }
            this.initialize();
        }

        public Method AddFunctionTo(TypeDefinition type, IToken nameToken, IToken getter, IToken setter)
        {
            Method method;
            string name = nameToken.getText();
            LexicalInfo info = ToLexicalInfo(nameToken);
            Method method1 = method = new Method(info);
            method.set_Name(name);
            Method node = !IsConstructorName(name, type) ? method : new Constructor(info);
            if ((getter != null) || (setter != null))
            {
                Property property = type.get_Members().get_Item(name) as Property;
                if (property == null)
                {
                    Property property2;
                    Property property1 = property2 = new Property(info);
                    property2.set_Name(name);
                    property = property2;
                    type.get_Members().Add(property);
                }
                if (getter != null)
                {
                    if (property.get_Getter() != null)
                    {
                        throw new AssertionFailedException("p.Getter is null");
                    }
                    property.set_Getter(node);
                }
                else
                {
                    if (property.get_Setter() != null)
                    {
                        throw new AssertionFailedException("p.Setter is null");
                    }
                    property.set_Setter(node);
                }
                this.FlushAttributes(property);
                return node;
            }
            type.get_Members().Add(node);
            this.FlushAttributes(node);
            return node;
        }

        public TypeReference anonymous_function_type()
        {
            TypeReference reference = null;
            IToken token = null;
            try
            {
                CallableTypeReference reference2;
                ParameterDeclarationCollection declarations;
                token = this.LT(1);
                this.match(0x13);
                if (base.inputState.guessing == 0)
                {
                    reference = reference2 = new CallableTypeReference(ToLexicalInfo(token));
                    declarations = reference2.get_Parameters();
                }
                this.function_type_parameters(declarations);
                if ((this.LA(1) == 0x42) && ((this.LA(2) == 0x13) || (this.LA(2) == 0x3b)))
                {
                    this.match(0x42);
                    TypeReference reference3 = this.type_reference();
                    if (base.inputState.guessing == 0)
                    {
                        reference2.set_ReturnType(reference3);
                    }
                    return reference;
                }
                if (!tokenSet_35_.member(this.LA(1)) || !tokenSet_25_.member(this.LA(2)))
                {
                    throw new NoViableAltException(this.LT(1), this.getFilename());
                }
                return reference;
            }
            catch (RecognitionException exception)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception);
                this.recover(exception, tokenSet_35_);
                return reference;
            }
            return reference;
        }

        public Expression array_initializer()
        {
            Expression expression = null;
            List<Expression> dimensions = new List<Expression>(1);
            try
            {
                TypeReference elementType = this.simple_type_reference();
                this.match(0x44);
                Expression item = this.sum();
                if (base.inputState.guessing == 0)
                {
                    dimensions.Add(item);
                }
                while (this.LA(1) == 0x43)
                {
                    this.match(0x43);
                    item = this.sum();
                    if (base.inputState.guessing == 0)
                    {
                        dimensions.Add(item);
                    }
                }
                this.match(0x45);
                if (base.inputState.guessing == 0)
                {
                    expression = CodeFactory.NewArrayInitializer(elementType.get_LexicalInfo(), elementType, dimensions);
                }
            }
            catch (RecognitionException exception)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception);
                this.recover(exception, tokenSet_20_);
                return expression;
            }
            return expression;
        }

        public Expression array_literal()
        {
            Expression expression = null;
            IToken token = null;
            try
            {
                token = this.LT(1);
                this.match(0x44);
                bool flag = false;
                if (tokenSet_16_.member(this.LA(1)) && tokenSet_59_.member(this.LA(2)))
                {
                    int num = this.mark();
                    flag = true;
                    base.inputState.guessing++;
                    try
                    {
                        this.expression();
                        this.match(0x12);
                    }
                    catch (RecognitionException)
                    {
                        flag = false;
                    }
                    this.rewind(num);
                    base.inputState.guessing--;
                }
                if (flag)
                {
                    IToken token2;
                    Declaration declaration;
                    Expression expression4;
                    Expression projection = this.expression();
                    this.match(0x12);
                    this.match(0x3f);
                    int num2 = this.LA(1);
                    switch (num2)
                    {
                        case 12:
                        case 0x10:
                        case 0x21:
                        case 0x3b:
                            token2 = this.identifier();
                            break;

                        default:
                            if (num2 != 0x2d)
                            {
                                throw new NoViableAltException(this.LT(1), this.getFilename());
                            }
                            declaration = this.declaration();
                            break;
                    }
                    this.match(0x18);
                    Expression expression3 = this.expression();
                    this.match(0x40);
                    switch (this.LA(1))
                    {
                        case 0x15:
                            this.match(0x15);
                            expression4 = this.expression();
                            break;

                        case 0x45:
                            break;

                        default:
                            throw new NoViableAltException(this.LT(1), this.getFilename());
                    }
                    if (base.inputState.guessing == 0)
                    {
                        if (token2 != null)
                        {
                            Declaration declaration2;
                            Declaration declaration1 = declaration2 = new Declaration(ToLexicalInfo(token2));
                            declaration2.set_Name(token2.getText());
                            declaration = declaration2;
                        }
                        expression = CodeFactory.NewArrayComprehension(ToLexicalInfo(token), projection, declaration, expression3, expression4);
                    }
                }
                else
                {
                    ExpressionCollection expressions;
                    if (!tokenSet_60_.member(this.LA(1)) || !tokenSet_20_.member(this.LA(2)))
                    {
                        throw new NoViableAltException(this.LT(1), this.getFilename());
                    }
                    if (base.inputState.guessing == 0)
                    {
                        ArrayLiteralExpression expression5;
                        expression = expression5 = new ArrayLiteralExpression(ToLexicalInfo(token));
                        expressions = expression5.get_Items();
                    }
                    this.expression_list(expressions);
                }
                this.match(0x45);
            }
            catch (RecognitionException exception2)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception2);
                this.recover(exception2, tokenSet_20_);
                return expression;
            }
            return expression;
        }

        public Expression assignment_expression()
        {
            Expression expression = null;
            IToken token = null;
            IToken token2 = null;
            IToken token3 = null;
            IToken token4 = null;
            IToken token5 = null;
            IToken token6 = null;
            IToken token7 = null;
            IToken token8 = null;
            IToken token9 = null;
            IToken token10 = null;
            try
            {
                expression = this.conditional_expression();
                int num = this.LA(1);
                if ((((num == 0x34) || (num == 0x35)) || ((num == 0x36) || (num == 0x37))) || ((((num == 0x47) || (num == 0x4a)) || ((num == 0x4e) || (num == 0x5e))) || ((num == 0x62) || (num == 0x66))))
                {
                    IToken token11;
                    BinaryOperatorType type;
                    num = this.LA(1);
                    switch (num)
                    {
                        case 0x4e:
                            token = this.LT(1);
                            this.match(0x4e);
                            if (base.inputState.guessing == 0)
                            {
                                token11 = token;
                                type = 15;
                            }
                            break;

                        case 0x35:
                            token2 = this.LT(1);
                            this.match(0x35);
                            if (base.inputState.guessing == 0)
                            {
                                token11 = token2;
                                type = 0x10;
                            }
                            break;

                        case 0x36:
                            token3 = this.LT(1);
                            this.match(0x36);
                            if (base.inputState.guessing == 0)
                            {
                                token11 = token3;
                                type = 0x11;
                            }
                            break;

                        case 0x37:
                            token4 = this.LT(1);
                            this.match(0x37);
                            if (base.inputState.guessing == 0)
                            {
                                token11 = token4;
                                type = 0x12;
                            }
                            break;

                        case 0x34:
                            token5 = this.LT(1);
                            this.match(0x34);
                            if (base.inputState.guessing == 0)
                            {
                                token11 = token5;
                                type = 0x13;
                            }
                            break;

                        case 0x47:
                            token6 = this.LT(1);
                            this.match(0x47);
                            if (base.inputState.guessing == 0)
                            {
                                token11 = token6;
                                type = 0x16;
                            }
                            break;

                        case 0x4a:
                            token7 = this.LT(1);
                            this.match(0x4a);
                            if (base.inputState.guessing == 0)
                            {
                                token11 = token7;
                                type = 0x15;
                            }
                            break;

                        case 0x66:
                            token8 = this.LT(1);
                            this.match(0x66);
                            if (base.inputState.guessing == 0)
                            {
                                token11 = token8;
                                type = 0x21;
                            }
                            break;

                        case 0x5e:
                            token9 = this.LT(1);
                            this.match(0x5e);
                            if (base.inputState.guessing == 0)
                            {
                                token11 = token9;
                                type = 0x23;
                            }
                            break;

                        case 0x62:
                            token10 = this.LT(1);
                            this.match(0x62);
                            if (base.inputState.guessing == 0)
                            {
                                token11 = token10;
                                type = 0x25;
                            }
                            break;

                        default:
                            throw new NoViableAltException(this.LT(1), this.getFilename());
                    }
                    Expression expression2 = this.assignment_expression();
                    if (base.inputState.guessing == 0)
                    {
                        BinaryExpression expression3 = new BinaryExpression(ToLexicalInfo(token11));
                        expression3.set_Operator(type);
                        expression3.set_Left(expression);
                        expression3.set_Right(expression2);
                        expression = expression3;
                    }
                    return expression;
                }
                switch (num)
                {
                    case 1:
                    case 5:
                    case 7:
                    case 8:
                    case 9:
                    case 10:
                    case 11:
                    case 12:
                    case 13:
                    case 15:
                    case 0x10:
                    case 0x11:
                    case 0x12:
                    case 0x13:
                    case 20:
                    case 0x15:
                    case 0x19:
                    case 0x1b:
                    case 0x1d:
                    case 30:
                    case 0x1f:
                    case 0x20:
                    case 0x21:
                    case 0x23:
                    case 0x24:
                    case 0x25:
                    case 0x26:
                    case 0x27:
                    case 40:
                    case 0x29:
                    case 0x2a:
                    case 0x2b:
                    case 0x2c:
                    case 0x2d:
                    case 0x2e:
                    case 0x2f:
                    case 0x30:
                    case 0x31:
                    case 50:
                    case 0x33:
                    case 0x3b:
                    case 60:
                    case 0x3d:
                    case 0x3e:
                    case 0x3f:
                    case 0x40:
                    case 0x44:
                    case 0x4d:
                    case 0x4f:
                    case 80:
                    case 0x52:
                    case 0x58:
                    case 0x63:
                    case 0x67:
                    case 0x69:
                    case 0x6a:
                    case 0x6b:
                    case 0x6c:
                        return expression;

                    case 0x6d:
                        return expression;
                }
                throw new NoViableAltException(this.LT(1), this.getFilename());
            }
            catch (RecognitionException exception)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception);
                this.recover(exception, tokenSet_51_);
                return expression;
            }
            return expression;
        }

        public Expression atom()
        {
            Expression expression = null;
            try
            {
                switch (this.LA(1))
                {
                    case 15:
                    case 0x1d:
                    case 0x27:
                    case 40:
                    case 0x2a:
                    case 60:
                    case 0x3d:
                    case 0x44:
                    case 0x69:
                    case 0x6a:
                    case 0x6b:
                    case 0x6c:
                    case 0x6d:
                        return this.literal();

                    case 0x13:
                        return this.function_expression();

                    case 12:
                    case 0x3b:
                        return this.simple_reference_expression();

                    case 0x3f:
                        return this.paren_expression();

                    case 0x1b:
                        return this.new_expression();

                    case 0x2c:
                        return this.typeof_expression();
                }
                throw new NoViableAltException(this.LT(1), this.getFilename());
            }
            catch (RecognitionException exception)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception);
                this.recover(exception, tokenSet_20_);
                return expression;
            }
            return expression;
        }

        public void attribute()
        {
            try
            {
                this.match(0x63);
                Attribute attribute = this.attribute_constructor();
                if (base.inputState.guessing == 0)
                {
                    this._attributes.Add(attribute);
                }
            }
            catch (RecognitionException exception)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception);
                this.recover(exception, tokenSet_21_);
            }
        }

        public Attribute attribute_constructor()
        {
            Attribute attr = null;
            try
            {
                Token token = this.qname();
                if (base.inputState.guessing == 0)
                {
                    attr = new Attribute(ToLexicalInfo(token), token.getText());
                }
                if ((this.LA(1) == 0x3f) && tokenSet_22_.member(this.LA(2)))
                {
                    this.match(0x3f);
                    switch (this.LA(1))
                    {
                        case 12:
                        case 15:
                        case 0x13:
                        case 0x1b:
                        case 0x1d:
                        case 0x27:
                        case 40:
                        case 0x2a:
                        case 0x2c:
                        case 0x3b:
                        case 60:
                        case 0x3d:
                        case 0x3f:
                        case 0x44:
                        case 0x4f:
                        case 80:
                        case 0x52:
                        case 0x58:
                        case 0x67:
                        case 0x69:
                        case 0x6a:
                        case 0x6b:
                        case 0x6c:
                        case 0x6d:
                            this.attribute_parameter(attr);
                            while (this.LA(1) == 0x43)
                            {
                                this.match(0x43);
                                this.attribute_parameter(attr);
                            }
                            break;

                        case 0x40:
                            break;

                        default:
                            throw new NoViableAltException(this.LT(1), this.getFilename());
                    }
                    this.match(0x40);
                    return attr;
                }
                if (!tokenSet_23_.member(this.LA(1)) || !tokenSet_24_.member(this.LA(2)))
                {
                    throw new NoViableAltException(this.LT(1), this.getFilename());
                }
                return attr;
            }
            catch (RecognitionException exception)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception);
                this.recover(exception, tokenSet_23_);
                return attr;
            }
            return attr;
        }

        public void attribute_parameter(Attribute attr)
        {
            try
            {
                Expression expression2;
                bool flag = false;
                if (((this.LA(1) == 12) || (this.LA(1) == 0x3b)) && ((this.LA(2) == 0x41) || (this.LA(2) == 0x4e)))
                {
                    int num = this.mark();
                    flag = true;
                    base.inputState.guessing++;
                    try
                    {
                        this.match(0x3b);
                        this.match(0x4e);
                    }
                    catch (RecognitionException)
                    {
                        flag = false;
                    }
                    this.rewind(num);
                    base.inputState.guessing--;
                }
                if (!flag)
                {
                    if (!tokenSet_16_.member(this.LA(1)) || !tokenSet_17_.member(this.LA(2)))
                    {
                        throw new NoViableAltException(this.LT(1), this.getFilename());
                    }
                    expression2 = this.expression();
                    if (base.inputState.guessing == 0)
                    {
                        attr.get_Arguments().Add(expression2);
                    }
                }
                else
                {
                    Expression expression = this.reference_expression();
                    this.match(0x4e);
                    expression2 = this.expression();
                    if (base.inputState.guessing == 0)
                    {
                        attr.get_NamedArguments().Add(new ExpressionPair(expression, expression2));
                    }
                }
            }
            catch (RecognitionException exception2)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception2);
                this.recover(exception2, tokenSet_18_);
            }
        }

        public void attributes()
        {
            this._attributes.Clear();
            try
            {
                int num = 0;
                while (true)
                {
                    if (this.LA(1) == 0x63)
                    {
                        this.attribute();
                    }
                    else
                    {
                        if (num < 1)
                        {
                            throw new NoViableAltException(this.LT(1), this.getFilename());
                        }
                        return;
                    }
                    num++;
                }
            }
            catch (RecognitionException exception)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception);
                this.recover(exception, tokenSet_6_);
            }
        }

        public Expression bitwise_and()
        {
            Expression expression = null;
            IToken token = null;
            try
            {
                expression = this.equality();
                while ((this.LA(1) == 0x48) && tokenSet_16_.member(this.LA(2)))
                {
                    BinaryOperatorType type;
                    token = this.LT(1);
                    this.match(0x48);
                    if (base.inputState.guessing == 0)
                    {
                        type = 0x1f;
                    }
                    Expression expression2 = this.equality();
                    if (base.inputState.guessing == 0)
                    {
                        BinaryExpression expression3;
                        BinaryExpression expression1 = expression3 = new BinaryExpression(ToLexicalInfo(token));
                        expression3.set_Operator(type);
                        expression3.set_Left(expression);
                        expression3.set_Right(expression2);
                        expression = expression3;
                    }
                }
                return expression;
            }
            catch (RecognitionException exception)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception);
                this.recover(exception, tokenSet_20_);
                return expression;
            }
            return expression;
        }

        public Expression bitwise_or()
        {
            Expression expression = null;
            IToken token = null;
            try
            {
                expression = this.bitwise_xor();
                while ((this.LA(1) == 70) && tokenSet_16_.member(this.LA(2)))
                {
                    BinaryOperatorType type;
                    token = this.LT(1);
                    this.match(70);
                    if (base.inputState.guessing == 0)
                    {
                        type = 30;
                    }
                    Expression expression2 = this.bitwise_xor();
                    if (base.inputState.guessing == 0)
                    {
                        BinaryExpression expression3;
                        BinaryExpression expression1 = expression3 = new BinaryExpression(ToLexicalInfo(token));
                        expression3.set_Operator(type);
                        expression3.set_Left(expression);
                        expression3.set_Right(expression2);
                        expression = expression3;
                    }
                }
                return expression;
            }
            catch (RecognitionException exception)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception);
                this.recover(exception, tokenSet_20_);
                return expression;
            }
            return expression;
        }

        public Expression bitwise_xor()
        {
            Expression expression = null;
            IToken token = null;
            try
            {
                expression = this.bitwise_and();
                while ((this.LA(1) == 0x49) && tokenSet_16_.member(this.LA(2)))
                {
                    BinaryOperatorType type;
                    token = this.LT(1);
                    this.match(0x49);
                    if (base.inputState.guessing == 0)
                    {
                        type = 0x20;
                    }
                    Expression expression2 = this.bitwise_and();
                    if (base.inputState.guessing == 0)
                    {
                        BinaryExpression expression3;
                        BinaryExpression expression1 = expression3 = new BinaryExpression(ToLexicalInfo(token));
                        expression3.set_Operator(type);
                        expression3.set_Left(expression);
                        expression3.set_Right(expression2);
                        expression = expression3;
                    }
                }
                return expression;
            }
            catch (RecognitionException exception)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception);
                this.recover(exception, tokenSet_20_);
                return expression;
            }
            return expression;
        }

        public void block(Block b)
        {
            IToken token = null;
            try
            {
                this.match(0x3d);
                while (tokenSet_2_.member(this.LA(1)))
                {
                    this.compound_or_single_stmt(b);
                }
                token = this.LT(1);
                this.match(0x3e);
                if (base.inputState.guessing == 0)
                {
                    SetEndSourceLocation(b, token);
                }
            }
            catch (RecognitionException exception)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception);
                this.recover(exception, tokenSet_20_);
            }
        }

        public BoolLiteralExpression bool_literal()
        {
            BoolLiteralExpression expression = null;
            IToken token = null;
            IToken token2 = null;
            try
            {
                switch (this.LA(1))
                {
                    case 0x2a:
                        token = this.LT(1);
                        this.match(0x2a);
                        if (base.inputState.guessing == 0)
                        {
                            BoolLiteralExpression expression2;
                            BoolLiteralExpression expression1 = expression2 = new BoolLiteralExpression(ToLexicalInfo(token));
                            expression2.set_Value(true);
                            expression = expression2;
                        }
                        return expression;

                    case 15:
                        token2 = this.LT(1);
                        this.match(15);
                        if (base.inputState.guessing == 0)
                        {
                            BoolLiteralExpression expression3;
                            BoolLiteralExpression expression4 = expression3 = new BoolLiteralExpression(ToLexicalInfo(token2));
                            expression3.set_Value(false);
                            expression = expression3;
                        }
                        return expression;
                }
                throw new NoViableAltException(this.LT(1), this.getFilename());
            }
            catch (RecognitionException exception)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception);
                this.recover(exception, tokenSet_20_);
                return expression;
            }
            return expression;
        }

        public void break_statement(Block b)
        {
            IToken token = null;
            try
            {
                token = this.LT(1);
                this.match(5);
                if (base.inputState.guessing == 0)
                {
                    b.Add(new BreakStatement(ToLexicalInfo(token)));
                }
            }
            catch (RecognitionException exception)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception);
                this.recover(exception, tokenSet_15_);
            }
        }

        public void builtin_statement(Block b)
        {
            try
            {
                int num = this.LA(1);
                switch (num)
                {
                    case 10:
                    case 0x12:
                    case 0x15:
                    case 0x2b:
                    case 0x2f:
                    case 0x31:
                        num = this.LA(1);
                        switch (num)
                        {
                            case 10:
                                this.do_while_statement(b);
                                return;

                            case 0x2f:
                                this.while_statement(b);
                                return;

                            case 0x12:
                                this.for_statement(b);
                                return;

                            case 0x15:
                                this.if_statement(b);
                                return;

                            case 0x2b:
                                this.try_statement(b);
                                return;

                            case 0x31:
                                this.switch_statement(b);
                                return;
                        }
                        throw new NoViableAltException(this.LT(1), this.getFilename());

                    default:
                        if ((((((num != 5) && (num != 9)) && ((num != 12) && (num != 15))) && (((num != 0x13) && (num != 0x1b)) && ((num != 0x1d) && (num != 30)))) && ((((num != 0x27) && (num != 40)) && ((num != 0x29) && (num != 0x2a))) && (((num != 0x2c) && (num != 0x2d)) && ((num != 0x30) && (num != 0x3b))))) && (((((num != 60) && (num != 0x3d)) && ((num != 0x3f) && (num != 0x44))) && (((num != 0x4f) && (num != 80)) && ((num != 0x52) && (num != 0x58)))) && ((((num != 0x67) && (num != 0x69)) && ((num != 0x6a) && (num != 0x6b))) && ((num != 0x6c) && (num != 0x6d)))))
                        {
                            throw new NoViableAltException(this.LT(1), this.getFilename());
                        }
                        switch (this.LA(1))
                        {
                            case 12:
                            case 15:
                            case 0x13:
                            case 0x1b:
                            case 0x1d:
                            case 0x27:
                            case 40:
                            case 0x2a:
                            case 0x2c:
                            case 0x3b:
                            case 60:
                            case 0x3d:
                            case 0x3f:
                            case 0x44:
                            case 0x4f:
                            case 80:
                            case 0x52:
                            case 0x58:
                            case 0x67:
                            case 0x69:
                            case 0x6a:
                            case 0x6b:
                            case 0x6c:
                            case 0x6d:
                                this.expression_statement(b);
                                goto Label_031D;

                            case 0x30:
                                this.yield_statement(b);
                                goto Label_031D;

                            case 30:
                                this.return_statement(b);
                                goto Label_031D;

                            case 5:
                                this.break_statement(b);
                                goto Label_031D;

                            case 9:
                                this.continue_statement(b);
                                goto Label_031D;

                            case 0x29:
                                this.throw_statement(b);
                                goto Label_031D;

                            case 0x2d:
                                this.declaration_statement(b);
                                goto Label_031D;
                        }
                        throw new NoViableAltException(this.LT(1), this.getFilename());
                }
            Label_031D:
                this.eos();
            }
            catch (RecognitionException exception)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception);
                this.recover(exception, tokenSet_15_);
            }
        }

        public TypeMember class_declaration(TypeDefinition parent)
        {
            TypeMember member = null;
            IToken token = null;
            IToken token2 = null;
            IToken token3 = null;
            try
            {
                TypeReference reference;
                ClassDefinition definition2;
                TypeReferenceCollection references;
                switch (this.LA(1))
                {
                    case 0x23:
                        token = this.LT(1);
                        this.match(0x23);
                        break;

                    case 8:
                        break;

                    default:
                        throw new NoViableAltException(this.LT(1), this.getFilename());
                }
                this.match(8);
                token2 = this.LT(1);
                this.match(0x3b);
                switch (this.LA(1))
                {
                    case 14:
                        this.match(14);
                        reference = this.type_reference();
                        break;

                    case 0x17:
                    case 0x3d:
                        break;

                    default:
                        throw new NoViableAltException(this.LT(1), this.getFilename());
                }
                if (base.inputState.guessing == 0)
                {
                    ClassDefinition definition;
                    ClassDefinition definition1 = definition = new ClassDefinition(ToLexicalInfo(token2));
                    definition.set_Name(token2.getText());
                    member = definition2 = definition;
                    references = definition2.get_BaseTypes();
                    if (reference != null)
                    {
                        references.Add(reference);
                    }
                    if (token != null)
                    {
                        definition2.set_Modifiers(definition2.get_Modifiers() | 0x400);
                    }
                    this.FlushAttributes(definition2);
                    parent.get_Members().Add(definition2);
                }
                switch (this.LA(1))
                {
                    case 0x17:
                        this.match(0x17);
                        this.type_reference_list(references);
                        break;

                    case 0x3d:
                        break;

                    default:
                        throw new NoViableAltException(this.LT(1), this.getFilename());
                }
                if (base.inputState.guessing == 0)
                {
                    foreach (TypeReference reference2 in references)
                    {
                        if (reference2 == reference)
                        {
                            BaseTypeAnnotations.AnnotateExtends(reference2);
                        }
                        else
                        {
                            BaseTypeAnnotations.AnnotateImplements(reference2);
                        }
                    }
                }
                this.match(0x3d);
                while (true)
                {
                    TypeMemberModifiers modifiers;
                    TypeMember member2;
                    if (!tokenSet_29_.member(this.LA(1)))
                    {
                        break;
                    }
                    if (base.inputState.guessing == 0)
                    {
                        modifiers = 0;
                    }
                    switch (this.LA(1))
                    {
                        case 0x63:
                            this.attributes();
                            break;

                        case 8:
                        case 13:
                        case 0x10:
                        case 0x13:
                        case 0x19:
                        case 0x1b:
                        case 0x1f:
                        case 0x20:
                        case 0x21:
                        case 0x22:
                        case 0x23:
                        case 0x24:
                        case 0x26:
                        case 0x2d:
                        case 0x2e:
                            break;

                        default:
                            throw new NoViableAltException(this.LT(1), this.getFilename());
                    }
                    if (tokenSet_30_.member(this.LA(1)) && tokenSet_31_.member(this.LA(2)))
                    {
                        modifiers = this.member_modifiers();
                    }
                    else if (!tokenSet_26_.member(this.LA(1)) || !tokenSet_32_.member(this.LA(2)))
                    {
                        throw new NoViableAltException(this.LT(1), this.getFilename());
                    }
                    int num = this.LA(1);
                    switch (num)
                    {
                        case 0x13:
                            member2 = this.function_member(definition2);
                            break;

                        case 0x2d:
                            member2 = this.field_member(definition2);
                            break;

                        case 13:
                            member2 = this.enum_declaration(definition2);
                            break;

                        case 8:
                        case 0x23:
                            member2 = this.class_declaration(definition2);
                            break;

                        default:
                            if (num != 0x19)
                            {
                                throw new NoViableAltException(this.LT(1), this.getFilename());
                            }
                            member2 = this.interface_declaration(definition2);
                            break;
                    }
                    if ((base.inputState.guessing == 0) && (member2 != null))
                    {
                        member2.set_Modifiers(member2.get_Modifiers() | modifiers);
                    }
                }
                token3 = this.LT(1);
                this.match(0x3e);
                if (base.inputState.guessing == 0)
                {
                    SetEndSourceLocation(definition2, token3);
                }
                while ((this.LA(1) == 0x4d) && tokenSet_33_.member(this.LA(2)))
                {
                    this.match(0x4d);
                }
                return member;
            }
            catch (RecognitionException exception)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception);
                this.recover(exception, tokenSet_33_);
                return member;
            }
            return member;
        }

        public Expression comparison()
        {
            Expression expression = null;
            IToken token = null;
            IToken token2 = null;
            IToken token3 = null;
            IToken token4 = null;
            IToken token5 = null;
            IToken token6 = null;
            IToken token7 = null;
            Expression expression2 = null;
            BinaryOperatorType type = 0;
            IToken token8 = null;
            try
            {
                expression = this.shift();
                while (true)
                {
                    if (!tokenSet_56_.member(this.LA(1)) || !tokenSet_57_.member(this.LA(2)))
                    {
                        return expression;
                    }
                    int num = this.LA(1);
                    if ((((num == 0x18) || (num == 0x1c)) || ((num == 0x5b) || (num == 0x5c))) || ((num == 0x5f) || (num == 0x60)))
                    {
                        num = this.LA(1);
                        switch (num)
                        {
                            case 0x1c:
                                token = this.LT(1);
                                this.match(0x1c);
                                this.match(0x18);
                                if (base.inputState.guessing == 0)
                                {
                                    type = 0x1b;
                                    token8 = token;
                                }
                                break;

                            case 0x18:
                                token2 = this.LT(1);
                                this.match(0x18);
                                if (base.inputState.guessing == 0)
                                {
                                    type = 0x1a;
                                    token8 = token2;
                                }
                                break;

                            case 0x5f:
                                token3 = this.LT(1);
                                this.match(0x5f);
                                if (base.inputState.guessing == 0)
                                {
                                    type = 9;
                                    token8 = token3;
                                }
                                break;

                            case 0x60:
                                token4 = this.LT(1);
                                this.match(0x60);
                                if (base.inputState.guessing == 0)
                                {
                                    type = 10;
                                    token8 = token4;
                                }
                                break;

                            case 0x5b:
                                token5 = this.LT(1);
                                this.match(0x5b);
                                if (base.inputState.guessing == 0)
                                {
                                    type = 7;
                                    token8 = token5;
                                }
                                break;

                            case 0x5c:
                                token6 = this.LT(1);
                                this.match(0x5c);
                                if (base.inputState.guessing == 0)
                                {
                                    type = 8;
                                    token8 = token6;
                                }
                                break;

                            default:
                                throw new NoViableAltException(this.LT(1), this.getFilename());
                        }
                        expression2 = this.shift();
                    }
                    else
                    {
                        if (num != 0x1a)
                        {
                            throw new NoViableAltException(this.LT(1), this.getFilename());
                        }
                        token7 = this.LT(1);
                        this.match(0x1a);
                        TypeReference reference = this.type_reference();
                        if (base.inputState.guessing == 0)
                        {
                            type = 0x19;
                            token8 = token7;
                            expression2 = new TypeofExpression(reference.get_LexicalInfo(), reference);
                        }
                    }
                    if (base.inputState.guessing == 0)
                    {
                        BinaryExpression expression3 = new BinaryExpression(ToLexicalInfo(token8));
                        expression3.set_Operator(type);
                        expression3.set_Left(expression);
                        expression3.set_Right(expression2);
                        expression = expression3;
                    }
                }
            }
            catch (RecognitionException exception)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception);
                this.recover(exception, tokenSet_20_);
                return expression;
            }
            return expression;
        }

        public void compound_or_single_stmt(Block b)
        {
            try
            {
                if ((this.LA(1) != 0x3d) || !tokenSet_13_.member(this.LA(2)))
                {
                    if (!tokenSet_2_.member(this.LA(1)) || !tokenSet_14_.member(this.LA(2)))
                    {
                        throw new NoViableAltException(this.LT(1), this.getFilename());
                    }
                    this.statement(b);
                }
                else
                {
                    this.compound_statement(b);
                }
            }
            catch (RecognitionException exception)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception);
                this.recover(exception, tokenSet_15_);
            }
        }

        public void compound_statement(Block b)
        {
            try
            {
                this.block(b);
                while ((this.LA(1) == 0x4d) && tokenSet_41_.member(this.LA(2)))
                {
                    this.match(0x4d);
                }
            }
            catch (RecognitionException exception)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception);
                this.recover(exception, tokenSet_41_);
            }
        }

        public Expression conditional_expression()
        {
            Expression expression = null;
            IToken token = null;
            try
            {
                expression = this.logical_or();
                if ((this.LA(1) == 0x57) && tokenSet_16_.member(this.LA(2)))
                {
                    token = this.LT(1);
                    this.match(0x57);
                    Expression expression2 = this.logical_or();
                    this.match(0x42);
                    Expression expression3 = this.conditional_expression();
                    if (base.inputState.guessing == 0)
                    {
                        ConditionalExpression expression4;
                        ConditionalExpression expression1 = expression4 = new ConditionalExpression(ToLexicalInfo(token));
                        expression4.set_Condition(expression);
                        expression4.set_TrueValue(expression2);
                        expression4.set_FalseValue(expression3);
                        expression = expression4;
                    }
                    return expression;
                }
                if (!tokenSet_20_.member(this.LA(1)) || !tokenSet_28_.member(this.LA(2)))
                {
                    throw new NoViableAltException(this.LT(1), this.getFilename());
                }
                return expression;
            }
            catch (RecognitionException exception)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception);
                this.recover(exception, tokenSet_20_);
                return expression;
            }
            return expression;
        }

        public override void consume()
        {
            this._last = this.LT(1);
            base.consume();
        }

        public void continue_statement(Block b)
        {
            IToken token = null;
            try
            {
                token = this.LT(1);
                this.match(9);
                if (base.inputState.guessing == 0)
                {
                    string currentLoopLabel = this.GetCurrentLoopLabel();
                    if (currentLoopLabel != null)
                    {
                        GotoStatement statement;
                        GotoStatement statement1 = statement = new GotoStatement(ToLexicalInfo(token));
                        statement.set_Label(new ReferenceExpression(currentLoopLabel));
                        b.Add(statement);
                    }
                    else
                    {
                        b.Add(new ContinueStatement(ToLexicalInfo(token)));
                    }
                }
            }
            catch (RecognitionException exception)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception);
                this.recover(exception, tokenSet_15_);
            }
        }

        public Declaration declaration()
        {
            Declaration declaration = null;
            try
            {
                TypeReference reference;
                this.match(0x2d);
                IToken token = this.identifier();
                switch (this.LA(1))
                {
                    case 0x42:
                        this.match(0x42);
                        reference = this.type_reference();
                        break;

                    case 1:
                    case 5:
                    case 7:
                    case 8:
                    case 9:
                    case 10:
                    case 11:
                    case 12:
                    case 13:
                    case 15:
                    case 0x10:
                    case 0x11:
                    case 0x12:
                    case 0x13:
                    case 20:
                    case 0x15:
                    case 0x18:
                    case 0x19:
                    case 0x1b:
                    case 0x1d:
                    case 30:
                    case 0x1f:
                    case 0x20:
                    case 0x21:
                    case 0x23:
                    case 0x24:
                    case 0x25:
                    case 0x26:
                    case 0x27:
                    case 40:
                    case 0x29:
                    case 0x2a:
                    case 0x2b:
                    case 0x2c:
                    case 0x2d:
                    case 0x2e:
                    case 0x2f:
                    case 0x30:
                    case 0x31:
                    case 50:
                    case 0x33:
                    case 0x3b:
                    case 60:
                    case 0x3d:
                    case 0x3e:
                    case 0x3f:
                    case 0x44:
                    case 0x4d:
                    case 0x4e:
                    case 0x4f:
                    case 80:
                    case 0x52:
                    case 0x58:
                    case 0x63:
                    case 0x67:
                    case 0x69:
                    case 0x6a:
                    case 0x6b:
                    case 0x6c:
                    case 0x6d:
                        break;

                    default:
                        throw new NoViableAltException(this.LT(1), this.getFilename());
                }
                if (base.inputState.guessing == 0)
                {
                    Declaration declaration2;
                    Declaration declaration1 = declaration2 = new Declaration(ToLexicalInfo(token));
                    declaration2.set_Name(token.getText());
                    declaration2.set_Type(reference);
                    declaration = declaration2;
                }
            }
            catch (RecognitionException exception)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception);
                this.recover(exception, tokenSet_50_);
                return declaration;
            }
            return declaration;
        }

        public void declaration_statement(Block b)
        {
            try
            {
                Expression expression;
                Declaration declaration = this.declaration();
                switch (this.LA(1))
                {
                    case 0x4e:
                        this.match(0x4e);
                        expression = this.expression();
                        break;

                    case 1:
                    case 5:
                    case 7:
                    case 8:
                    case 9:
                    case 10:
                    case 11:
                    case 12:
                    case 13:
                    case 15:
                    case 0x10:
                    case 0x11:
                    case 0x12:
                    case 0x13:
                    case 20:
                    case 0x15:
                    case 0x19:
                    case 0x1b:
                    case 0x1d:
                    case 30:
                    case 0x1f:
                    case 0x20:
                    case 0x21:
                    case 0x23:
                    case 0x24:
                    case 0x25:
                    case 0x26:
                    case 0x27:
                    case 40:
                    case 0x29:
                    case 0x2a:
                    case 0x2b:
                    case 0x2c:
                    case 0x2d:
                    case 0x2e:
                    case 0x2f:
                    case 0x30:
                    case 0x31:
                    case 50:
                    case 0x33:
                    case 0x3b:
                    case 60:
                    case 0x3d:
                    case 0x3e:
                    case 0x3f:
                    case 0x44:
                    case 0x4d:
                    case 0x4f:
                    case 80:
                    case 0x52:
                    case 0x58:
                    case 0x63:
                    case 0x67:
                    case 0x69:
                    case 0x6a:
                    case 0x6b:
                    case 0x6c:
                    case 0x6d:
                        break;

                    default:
                        throw new NoViableAltException(this.LT(1), this.getFilename());
                }
                if (base.inputState.guessing == 0)
                {
                    DeclarationStatement statement;
                    DeclarationStatement statement1 = statement = new DeclarationStatement(declaration.get_LexicalInfo());
                    statement.set_Declaration(declaration);
                    statement.set_Initializer(expression);
                    DeclarationStatement statement2 = statement;
                    b.Add(statement2);
                }
            }
            catch (RecognitionException exception)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception);
                this.recover(exception, tokenSet_15_);
            }
        }

        public void do_while_statement(Block container)
        {
            IToken token = null;
            IToken token2 = null;
            try
            {
                WhileStatement statement2;
                Block block;
                token = this.LT(1);
                this.match(10);
                if (base.inputState.guessing == 0)
                {
                    WhileStatement statement;
                    WhileStatement statement1 = statement = new WhileStatement(ToLexicalInfo(token));
                    statement.set_Condition(new BoolLiteralExpression(true));
                    statement2 = statement;
                    block = statement2.get_Block();
                    container.Add(statement2);
                    this.EnterLoop(statement2);
                }
                this.block(block);
                token2 = this.LT(1);
                this.match(0x2f);
                Expression expression = this.paren_expression();
                this.eos();
                if (base.inputState.guessing == 0)
                {
                    BreakStatement statement3;
                    BreakStatement statement4 = statement3 = new BreakStatement(ToLexicalInfo(token2));
                    statement3.set_Modifier(new StatementModifier(2, expression));
                    block.Add(statement3);
                    this.LeaveLoop(statement2);
                }
            }
            catch (RecognitionException exception)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception);
                this.recover(exception, tokenSet_15_);
            }
        }

        public DoubleLiteralExpression double_literal()
        {
            DoubleLiteralExpression expression = null;
            IToken token = null;
            try
            {
                token = this.LT(1);
                this.match(0x6a);
                if (base.inputState.guessing == 0)
                {
                    expression = CodeFactory.NewDoubleLiteralExpression(ToLexicalInfo(token), token.getText());
                }
            }
            catch (RecognitionException exception)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception);
                this.recover(exception, tokenSet_20_);
                return expression;
            }
            return expression;
        }

        public void EnterLoop(object stmt)
        {
            if (stmt == null)
            {
                throw new ArgumentNullException("stmt");
            }
            this._loopStack.Push(stmt);
        }

        public TypeMember enum_declaration(TypeDefinition container)
        {
            TypeMember node = null;
            IToken token = null;
            IToken token2 = null;
            try
            {
                EnumDefinition definition2;
                this.match(13);
                token = this.LT(1);
                this.match(0x3b);
                if (base.inputState.guessing == 0)
                {
                    EnumDefinition definition;
                    EnumDefinition definition1 = definition = new EnumDefinition(ToLexicalInfo(token));
                    definition.set_Name(token.getText());
                    node = definition2 = definition;
                    this.FlushAttributes(node);
                    container.get_Members().Add(definition2);
                }
                this.match(0x3d);
                switch (this.LA(1))
                {
                    case 12:
                    case 0x10:
                    case 0x21:
                    case 0x3b:
                    case 0x63:
                        this.enum_member(definition2);
                        while ((this.LA(1) == 0x43) && tokenSet_34_.member(this.LA(2)))
                        {
                            this.match(0x43);
                            this.enum_member(definition2);
                        }
                        switch (this.LA(1))
                        {
                            case 0x43:
                                this.match(0x43);
                                goto Label_015B;
                        }
                        throw new NoViableAltException(this.LT(1), this.getFilename());

                    case 0x3e:
                        break;

                    default:
                        throw new NoViableAltException(this.LT(1), this.getFilename());
                }
            Label_015B:
                token2 = this.LT(1);
                this.match(0x3e);
                if (base.inputState.guessing == 0)
                {
                    SetEndSourceLocation(definition2, token2);
                }
                while ((this.LA(1) == 0x4d) && tokenSet_33_.member(this.LA(2)))
                {
                    this.match(0x4d);
                }
                return node;
            }
            catch (RecognitionException exception)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception);
                this.recover(exception, tokenSet_33_);
                return node;
            }
            return node;
        }

        public void enum_member(EnumDefinition container)
        {
            try
            {
                IntegerLiteralExpression expression;
                switch (this.LA(1))
                {
                    case 0x63:
                        this.attributes();
                        break;

                    case 12:
                    case 0x10:
                    case 0x21:
                    case 0x3b:
                        break;

                    default:
                        throw new NoViableAltException(this.LT(1), this.getFilename());
                }
                IToken token = this.identifier();
                switch (this.LA(1))
                {
                    case 0x4e:
                        this.match(0x4e);
                        expression = this.integer_literal();
                        break;

                    case 0x3e:
                    case 0x43:
                        break;

                    default:
                        throw new NoViableAltException(this.LT(1), this.getFilename());
                }
                if (base.inputState.guessing == 0)
                {
                    EnumMember member;
                    EnumMember member1 = member = new EnumMember(ToLexicalInfo(token));
                    member.set_Name(token.getText());
                    member.set_Initializer(expression);
                    EnumMember node = member;
                    this.FlushAttributes(node);
                    container.get_Members().Add(node);
                }
            }
            catch (RecognitionException exception)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception);
                this.recover(exception, tokenSet_40_);
            }
        }

        public IToken eos()
        {
            IToken token = null;
            IToken token2 = null;
            try
            {
                if ((this.LA(1) == 0x4d) && tokenSet_27_.member(this.LA(2)))
                {
                    int num = 0;
                    while (true)
                    {
                        if ((this.LA(1) == 0x4d) && tokenSet_27_.member(this.LA(2)))
                        {
                            token2 = this.LT(1);
                            this.match(0x4d);
                            if ((base.inputState.guessing == 0) && (token == null))
                            {
                                token = token2;
                            }
                        }
                        else
                        {
                            if (num < 1)
                            {
                                throw new NoViableAltException(this.LT(1), this.getFilename());
                            }
                            return token;
                        }
                        num++;
                    }
                }
                if (!tokenSet_27_.member(this.LA(1)) || !tokenSet_28_.member(this.LA(2)))
                {
                    throw new NoViableAltException(this.LT(1), this.getFilename());
                }
                if (base.inputState.guessing == 0)
                {
                    this.SemicolonExpected();
                }
                return token;
            }
            catch (RecognitionException exception)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception);
                this.recover(exception, tokenSet_27_);
                return token;
            }
            return token;
        }

        public Expression equality()
        {
            Expression expression = null;
            IToken token = null;
            IToken token2 = null;
            IToken token3 = null;
            IToken token4 = null;
            try
            {
                expression = this.comparison();
                while (true)
                {
                    BinaryOperatorType type;
                    IToken token5;
                    if (!tokenSet_58_.member(this.LA(1)) || !tokenSet_16_.member(this.LA(2)))
                    {
                        return expression;
                    }
                    switch (this.LA(1))
                    {
                        case 0x55:
                            token = this.LT(1);
                            this.match(0x55);
                            if (base.inputState.guessing == 0)
                            {
                                type = 11;
                                token5 = token;
                            }
                            break;

                        case 0x56:
                            token2 = this.LT(1);
                            this.match(0x56);
                            if (base.inputState.guessing == 0)
                            {
                                type = 12;
                                token5 = token2;
                            }
                            break;

                        case 0x59:
                            token3 = this.LT(1);
                            this.match(0x59);
                            if (base.inputState.guessing == 0)
                            {
                                type = 0x17;
                                token5 = token3;
                            }
                            break;

                        case 90:
                            token4 = this.LT(1);
                            this.match(90);
                            if (base.inputState.guessing == 0)
                            {
                                type = 0x18;
                                token5 = token4;
                            }
                            break;

                        default:
                            throw new NoViableAltException(this.LT(1), this.getFilename());
                    }
                    Expression expression2 = this.comparison();
                    if (base.inputState.guessing == 0)
                    {
                        expression = new BinaryExpression(ToLexicalInfo(token5), type, expression, expression2);
                    }
                }
            }
            catch (RecognitionException exception)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception);
                this.recover(exception, tokenSet_20_);
                return expression;
            }
            return expression;
        }

        public Expression expression()
        {
            Expression expression = null;
            try
            {
                expression = this.conditional_expression();
            }
            catch (RecognitionException exception)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception);
                this.recover(exception, tokenSet_20_);
                return expression;
            }
            return expression;
        }

        public void expression_list(ExpressionCollection ec)
        {
            try
            {
                if (tokenSet_16_.member(this.LA(1)) && tokenSet_17_.member(this.LA(2)))
                {
                    Expression expression = this.expression();
                    if (base.inputState.guessing == 0)
                    {
                        ec.Add(expression);
                    }
                    while (this.LA(1) == 0x43)
                    {
                        this.match(0x43);
                        expression = this.expression();
                        if (base.inputState.guessing == 0)
                        {
                            ec.Add(expression);
                        }
                    }
                }
                else if ((((this.LA(1) != 0x3d) && (this.LA(1) != 0x40)) && (this.LA(1) != 0x45)) || !tokenSet_20_.member(this.LA(2)))
                {
                    throw new NoViableAltException(this.LT(1), this.getFilename());
                }
            }
            catch (RecognitionException exception)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception);
                this.recover(exception, tokenSet_52_);
            }
        }

        public ExpressionPair expression_pair()
        {
            ExpressionPair pair = null;
            IToken token = null;
            try
            {
                Expression expression = this.expression();
                token = this.LT(1);
                this.match(0x42);
                Expression expression2 = this.expression();
                if (base.inputState.guessing == 0)
                {
                    pair = new ExpressionPair(ToLexicalInfo(token), expression, expression2);
                }
            }
            catch (RecognitionException exception)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception);
                this.recover(exception, tokenSet_40_);
                return pair;
            }
            return pair;
        }

        public void expression_statement(Block b)
        {
            try
            {
                Expression expression = this.assignment_expression();
                if (base.inputState.guessing == 0)
                {
                    b.Add(new ExpressionStatement(expression));
                }
            }
            catch (RecognitionException exception)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception);
                this.recover(exception, tokenSet_15_);
            }
        }

        public TypeMember field_member(TypeDefinition cd)
        {
            TypeMember node = null;
            try
            {
                TypeReference reference;
                Expression expression;
                Field field;
                this.match(0x2d);
                IToken token = this.identifier();
                switch (this.LA(1))
                {
                    case 0x42:
                        this.match(0x42);
                        reference = this.type_reference();
                        break;

                    case 1:
                    case 5:
                    case 8:
                    case 9:
                    case 10:
                    case 12:
                    case 13:
                    case 15:
                    case 0x10:
                    case 0x12:
                    case 0x13:
                    case 20:
                    case 0x15:
                    case 0x19:
                    case 0x1b:
                    case 0x1d:
                    case 30:
                    case 0x1f:
                    case 0x20:
                    case 0x21:
                    case 0x22:
                    case 0x23:
                    case 0x24:
                    case 0x25:
                    case 0x26:
                    case 0x27:
                    case 40:
                    case 0x29:
                    case 0x2a:
                    case 0x2b:
                    case 0x2c:
                    case 0x2d:
                    case 0x2e:
                    case 0x2f:
                    case 0x30:
                    case 0x31:
                    case 0x3b:
                    case 60:
                    case 0x3d:
                    case 0x3e:
                    case 0x3f:
                    case 0x44:
                    case 0x4d:
                    case 0x4e:
                    case 0x4f:
                    case 80:
                    case 0x52:
                    case 0x58:
                    case 0x63:
                    case 0x67:
                    case 0x69:
                    case 0x6a:
                    case 0x6b:
                    case 0x6c:
                    case 0x6d:
                        break;

                    default:
                        throw new NoViableAltException(this.LT(1), this.getFilename());
                }
                switch (this.LA(1))
                {
                    case 0x4e:
                        this.match(0x4e);
                        expression = this.expression();
                        break;

                    case 1:
                    case 5:
                    case 8:
                    case 9:
                    case 10:
                    case 12:
                    case 13:
                    case 15:
                    case 0x10:
                    case 0x12:
                    case 0x13:
                    case 20:
                    case 0x15:
                    case 0x19:
                    case 0x1b:
                    case 0x1d:
                    case 30:
                    case 0x1f:
                    case 0x20:
                    case 0x21:
                    case 0x22:
                    case 0x23:
                    case 0x24:
                    case 0x25:
                    case 0x26:
                    case 0x27:
                    case 40:
                    case 0x29:
                    case 0x2a:
                    case 0x2b:
                    case 0x2c:
                    case 0x2d:
                    case 0x2e:
                    case 0x2f:
                    case 0x30:
                    case 0x31:
                    case 0x3b:
                    case 60:
                    case 0x3d:
                    case 0x3e:
                    case 0x3f:
                    case 0x44:
                    case 0x4d:
                    case 0x4f:
                    case 80:
                    case 0x52:
                    case 0x58:
                    case 0x63:
                    case 0x67:
                    case 0x69:
                    case 0x6a:
                    case 0x6b:
                    case 0x6c:
                    case 0x6d:
                        break;

                    default:
                        throw new NoViableAltException(this.LT(1), this.getFilename());
                }
                IToken token2 = this.eos();
                if (base.inputState.guessing != 0)
                {
                    return node;
                }
                Field field1 = field = new Field(ToLexicalInfo(token));
                field.set_Name(token.getText());
                field.set_Type(reference);
                field.set_Initializer(expression);
                node = field;
                if (token2 != null)
                {
                    SetEndSourceLocation(node, token2);
                }
                this.FlushAttributes(node);
                cd.get_Members().Add(node);
            }
            catch (RecognitionException exception)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception);
                this.recover(exception, tokenSet_33_);
                return node;
            }
            return node;
        }

        public void finally_block(TryStatement s)
        {
            IToken token = null;
            try
            {
                Block block;
                token = this.LT(1);
                this.match(0x11);
                if (base.inputState.guessing == 0)
                {
                    Block block2;
                    s.set_EnsureBlock(block2 = new Block(ToLexicalInfo(token)));
                    block = block2;
                }
                this.compound_or_single_stmt(block);
            }
            catch (RecognitionException exception)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception);
                this.recover(exception, tokenSet_15_);
            }
        }

        public void FlushAttributes(INodeWithAttributes node)
        {
            node.get_Attributes().AddRange(this._attributes);
            this._attributes.Clear();
        }

        public Statement for_c(Block container)
        {
            Statement statement = null;
            try
            {
                Expression expression;
                Expression expression2;
                WhileStatement statement3;
                Block block;
                string str;
                switch (this.LA(1))
                {
                    case 0x2d:
                        this.declaration_statement(container);
                        if (base.inputState.guessing == 0)
                        {
                            statement = container.get_Statements().get_Item(-1) as DeclarationStatement;
                            if (statement != null)
                            {
                                statement.Annotate("PrivateScope");
                            }
                        }
                        break;

                    case 12:
                    case 15:
                    case 0x13:
                    case 0x1b:
                    case 0x1d:
                    case 0x27:
                    case 40:
                    case 0x2a:
                    case 0x2c:
                    case 0x3b:
                    case 60:
                    case 0x3d:
                    case 0x3f:
                    case 0x44:
                    case 0x4f:
                    case 80:
                    case 0x52:
                    case 0x58:
                    case 0x67:
                    case 0x69:
                    case 0x6a:
                    case 0x6b:
                    case 0x6c:
                    case 0x6d:
                        this.expression_statement(container);
                        break;

                    case 0x4d:
                        break;

                    default:
                        throw new NoViableAltException(this.LT(1), this.getFilename());
                }
                this.match(0x4d);
                switch (this.LA(1))
                {
                    case 12:
                    case 15:
                    case 0x13:
                    case 0x1b:
                    case 0x1d:
                    case 0x27:
                    case 40:
                    case 0x2a:
                    case 0x2c:
                    case 0x3b:
                    case 60:
                    case 0x3d:
                    case 0x3f:
                    case 0x44:
                    case 0x4f:
                    case 80:
                    case 0x52:
                    case 0x58:
                    case 0x67:
                    case 0x69:
                    case 0x6a:
                    case 0x6b:
                    case 0x6c:
                    case 0x6d:
                        expression = this.expression();
                        break;

                    case 0x4d:
                        break;

                    default:
                        throw new NoViableAltException(this.LT(1), this.getFilename());
                }
                this.match(0x4d);
                switch (this.LA(1))
                {
                    case 12:
                    case 15:
                    case 0x13:
                    case 0x1b:
                    case 0x1d:
                    case 0x27:
                    case 40:
                    case 0x2a:
                    case 0x2c:
                    case 0x3b:
                    case 60:
                    case 0x3d:
                    case 0x3f:
                    case 0x44:
                    case 0x4f:
                    case 80:
                    case 0x52:
                    case 0x58:
                    case 0x67:
                    case 0x69:
                    case 0x6a:
                    case 0x6b:
                    case 0x6c:
                    case 0x6d:
                        expression2 = this.assignment_expression();
                        break;

                    case 0x40:
                        break;

                    default:
                        throw new NoViableAltException(this.LT(1), this.getFilename());
                }
                if (base.inputState.guessing == 0)
                {
                    WhileStatement statement2;
                    WhileStatement statement1 = statement2 = new WhileStatement();
                    statement2.set_Condition(expression);
                    statement3 = statement2;
                    if (expression == null)
                    {
                        BoolLiteralExpression expression3;
                        BoolLiteralExpression expression1 = expression3 = new BoolLiteralExpression();
                        expression3.set_Value(true);
                        statement3.set_Condition(expression3);
                    }
                    block = statement3.get_Block();
                    statement = statement3;
                    str = this.SetUpLoopLabel(statement3);
                    container.Add(statement);
                    this.EnterLoop(statement3);
                }
                this.match(0x40);
                this.compound_or_single_stmt(block);
                if (base.inputState.guessing != 0)
                {
                    return statement;
                }
                this.LeaveLoop(statement3);
                if (this.IsLabelInUse(statement3))
                {
                    LabelStatement statement4;
                    LabelStatement statement5 = statement4 = new LabelStatement();
                    statement4.set_Name(str);
                    block.Add(statement4);
                }
                if (expression2 != null)
                {
                    block.Add(expression2);
                }
            }
            catch (RecognitionException exception)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception);
                this.recover(exception, tokenSet_15_);
                return statement;
            }
            return statement;
        }

        public Statement for_in(Block container)
        {
            Statement stmt = null;
            try
            {
                Declaration declaration2;
                Block block;
                int num = this.LA(1);
                switch (num)
                {
                    case 12:
                    case 0x10:
                    case 0x21:
                    case 0x3b:
                    {
                        IToken token = this.identifier();
                        if (base.inputState.guessing == 0)
                        {
                            Declaration declaration;
                            Declaration declaration1 = declaration = new Declaration(ToLexicalInfo(token));
                            declaration.set_Name(token.getText());
                            declaration2 = declaration;
                        }
                        break;
                    }
                    default:
                        if (num != 0x2d)
                        {
                            throw new NoViableAltException(this.LT(1), this.getFilename());
                        }
                        declaration2 = this.declaration();
                        if (base.inputState.guessing == 0)
                        {
                            DeclarationAnnotations.ForceNewVariable(declaration2);
                        }
                        break;
                }
                this.match(0x18);
                Expression expression = this.expression();
                if (base.inputState.guessing == 0)
                {
                    ForStatement statement2;
                    ForStatement statement1 = statement2 = new ForStatement();
                    statement2.set_Iterator(expression);
                    ForStatement statement3 = statement2;
                    statement3.get_Declarations().Add(declaration2);
                    block = statement3.get_Block();
                    stmt = statement3;
                    container.Add(stmt);
                    this.EnterLoop(stmt);
                }
                this.match(0x40);
                this.compound_or_single_stmt(block);
                if (base.inputState.guessing == 0)
                {
                    this.LeaveLoop(stmt);
                }
            }
            catch (RecognitionException exception)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception);
                this.recover(exception, tokenSet_15_);
                return stmt;
            }
            return stmt;
        }

        public void for_statement(Block container)
        {
            IToken token = null;
            try
            {
                Statement statement;
                token = this.LT(1);
                this.match(0x12);
                int num = this.LA(1);
                if (num == 12)
                {
                    this.match(12);
                    this.match(0x3f);
                    statement = this.for_in(container);
                }
                else
                {
                    if (num != 0x3f)
                    {
                        throw new NoViableAltException(this.LT(1), this.getFilename());
                    }
                    this.match(0x3f);
                    bool flag = false;
                    if (tokenSet_45_.member(this.LA(1)) && tokenSet_46_.member(this.LA(2)))
                    {
                        int num2 = this.mark();
                        flag = true;
                        base.inputState.guessing++;
                        try
                        {
                            num = this.LA(1);
                            switch (num)
                            {
                                case 12:
                                case 0x10:
                                case 0x21:
                                case 0x3b:
                                    this.identifier();
                                    break;

                                default:
                                    if (num != 0x2d)
                                    {
                                        throw new NoViableAltException(this.LT(1), this.getFilename());
                                    }
                                    this.declaration();
                                    break;
                            }
                            this.match(0x18);
                        }
                        catch (RecognitionException)
                        {
                            flag = false;
                        }
                        this.rewind(num2);
                        base.inputState.guessing--;
                    }
                    if (flag)
                    {
                        statement = this.for_in(container);
                    }
                    else
                    {
                        if (!tokenSet_47_.member(this.LA(1)) || !tokenSet_48_.member(this.LA(2)))
                        {
                            throw new NoViableAltException(this.LT(1), this.getFilename());
                        }
                        statement = this.for_c(container);
                    }
                }
                if ((base.inputState.guessing == 0) && (statement != null))
                {
                    statement.set_LexicalInfo(ToLexicalInfo(token));
                }
            }
            catch (RecognitionException exception2)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception2);
                this.recover(exception2, tokenSet_15_);
            }
        }

        public void function_body(Method method)
        {
            try
            {
                this.match(0x3f);
                switch (this.LA(1))
                {
                    case 12:
                    case 0x10:
                    case 0x21:
                    case 0x3b:
                    case 0x63:
                        this.parameter_declaration_list(method);
                        break;

                    case 0x40:
                        break;

                    default:
                        throw new NoViableAltException(this.LT(1), this.getFilename());
                }
                this.match(0x40);
                switch (this.LA(1))
                {
                    case 0x42:
                    {
                        this.match(0x42);
                        TypeReference reference = this.type_reference();
                        if (base.inputState.guessing == 0)
                        {
                            method.set_ReturnType(reference);
                        }
                        break;
                    }
                    case 0x3d:
                        break;

                    default:
                        throw new NoViableAltException(this.LT(1), this.getFilename());
                }
                this.compound_statement(method.get_Body());
                if (base.inputState.guessing == 0)
                {
                    method.set_EndSourceLocation(method.get_Body().get_EndSourceLocation());
                }
            }
            catch (RecognitionException exception)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception);
                this.recover(exception, tokenSet_33_);
            }
        }

        public BlockExpression function_expression()
        {
            BlockExpression m = null;
            IToken token = null;
            try
            {
                Block block;
                token = this.LT(1);
                this.match(0x13);
                if (base.inputState.guessing == 0)
                {
                    m = new BlockExpression(ToLexicalInfo(token));
                    m.Annotate("inline");
                    block = m.get_Body();
                }
                this.match(0x3f);
                switch (this.LA(1))
                {
                    case 12:
                    case 0x10:
                    case 0x21:
                    case 0x3b:
                    case 0x63:
                        this.parameter_declaration_list(m);
                        break;

                    case 0x40:
                        break;

                    default:
                        throw new NoViableAltException(this.LT(1), this.getFilename());
                }
                this.match(0x40);
                switch (this.LA(1))
                {
                    case 0x42:
                    {
                        this.match(0x42);
                        TypeReference reference = this.type_reference();
                        if (base.inputState.guessing == 0)
                        {
                            m.set_ReturnType(reference);
                        }
                        break;
                    }
                    case 12:
                    case 15:
                    case 0x13:
                    case 0x1b:
                    case 0x1d:
                    case 0x27:
                    case 40:
                    case 0x2a:
                    case 0x2c:
                    case 0x3b:
                    case 60:
                    case 0x3d:
                    case 0x3f:
                    case 0x44:
                    case 0x4f:
                    case 80:
                    case 0x52:
                    case 0x58:
                    case 0x67:
                    case 0x69:
                    case 0x6a:
                    case 0x6b:
                    case 0x6c:
                    case 0x6d:
                        break;

                    default:
                        throw new NoViableAltException(this.LT(1), this.getFilename());
                }
                if ((this.LA(1) == 0x3d) && tokenSet_13_.member(this.LA(2)))
                {
                    this.block(block);
                    return m;
                }
                if (!tokenSet_16_.member(this.LA(1)) || !tokenSet_20_.member(this.LA(2)))
                {
                    throw new NoViableAltException(this.LT(1), this.getFilename());
                }
                Expression expression2 = this.expression();
                if (base.inputState.guessing == 0)
                {
                    block.Add(expression2);
                }
                return m;
            }
            catch (RecognitionException exception)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception);
                this.recover(exception, tokenSet_20_);
                return m;
            }
            return m;
        }

        public TypeMember function_member(TypeDefinition cd)
        {
            TypeMember member = null;
            IToken getter = null;
            IToken setter = null;
            try
            {
                Method method;
                this.match(0x13);
                switch (this.LA(1))
                {
                    case 20:
                        getter = this.LT(1);
                        this.match(20);
                        break;

                    case 0x25:
                        setter = this.LT(1);
                        this.match(0x25);
                        break;

                    case 12:
                    case 0x10:
                    case 0x21:
                    case 0x3b:
                        break;

                    default:
                        throw new NoViableAltException(this.LT(1), this.getFilename());
                }
                IToken nameToken = this.identifier();
                if (base.inputState.guessing == 0)
                {
                    member = method = this.AddFunctionTo(cd, nameToken, getter, setter);
                }
                this.function_body(method);
            }
            catch (RecognitionException exception)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception);
                this.recover(exception, tokenSet_33_);
                return member;
            }
            return member;
        }

        public void function_type_parameters(ParameterDeclarationCollection parameters)
        {
            try
            {
                this.match(0x3f);
                switch (this.LA(1))
                {
                    case 0x13:
                    case 0x3b:
                    {
                        TypeReference reference = this.type_reference();
                        if (base.inputState.guessing == 0)
                        {
                            ParameterDeclaration declaration;
                            ParameterDeclaration declaration1 = declaration = new ParameterDeclaration();
                            declaration.set_Type(reference);
                            declaration.set_Name("arg" + parameters.Count);
                            parameters.Add(declaration);
                        }
                        while (this.LA(1) == 0x43)
                        {
                            this.match(0x43);
                            reference = this.type_reference();
                            if (base.inputState.guessing == 0)
                            {
                                ParameterDeclaration declaration2;
                                ParameterDeclaration declaration3 = declaration2 = new ParameterDeclaration();
                                declaration2.set_Type(reference);
                                declaration2.set_Name("arg" + parameters.Count);
                                parameters.Add(declaration2);
                            }
                        }
                        break;
                    }
                    case 0x40:
                        break;

                    default:
                        throw new NoViableAltException(this.LT(1), this.getFilename());
                }
                this.match(0x40);
            }
            catch (RecognitionException exception)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception);
                this.recover(exception, tokenSet_35_);
            }
        }

        public string GetCurrentLoopLabel()
        {
            object local1 = this._loopStack[-1];
            if (!(local1 is Node))
            {
            }
            Node node = (Node) RuntimeServices.Coerce(local1, typeof(Node));
            object obj2 = node.get_Item("UpdateLabel");
            if ((obj2 != null) && !node.ContainsAnnotation("LabelInUse"))
            {
                node.Annotate("LabelInUse");
            }
            if (!(obj2 is string))
            {
            }
            return ((this._loopStack.Count != 0) ? ((string) RuntimeServices.Coerce(obj2, typeof(string))) : null);
        }

        public bool GlobalVariablesBecomeFields() => 
            this.UnityScriptParameters.GlobalVariablesBecomeFields;

        public HashLiteralExpression hash_literal()
        {
            HashLiteralExpression expression = null;
            IToken token = null;
            ExpressionPair pair = null;
            try
            {
                token = this.LT(1);
                this.match(0x3d);
                if (base.inputState.guessing == 0)
                {
                    expression = new HashLiteralExpression(ToLexicalInfo(token));
                }
                switch (this.LA(1))
                {
                    case 12:
                    case 15:
                    case 0x13:
                    case 0x1b:
                    case 0x1d:
                    case 0x27:
                    case 40:
                    case 0x2a:
                    case 0x2c:
                    case 0x3b:
                    case 60:
                    case 0x3d:
                    case 0x3f:
                    case 0x44:
                    case 0x4f:
                    case 80:
                    case 0x52:
                    case 0x58:
                    case 0x67:
                    case 0x69:
                    case 0x6a:
                    case 0x6b:
                    case 0x6c:
                    case 0x6d:
                        pair = this.expression_pair();
                        if (base.inputState.guessing == 0)
                        {
                            expression.get_Items().Add(pair);
                        }
                        while (this.LA(1) == 0x43)
                        {
                            this.match(0x43);
                            pair = this.expression_pair();
                            if (base.inputState.guessing == 0)
                            {
                                expression.get_Items().Add(pair);
                            }
                        }
                        break;

                    case 0x3e:
                        break;

                    default:
                        throw new NoViableAltException(this.LT(1), this.getFilename());
                }
                this.match(0x3e);
            }
            catch (RecognitionException exception)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception);
                this.recover(exception, tokenSet_20_);
                return expression;
            }
            return expression;
        }

        public IToken identifier()
        {
            IToken token = null;
            IToken token2 = null;
            IToken token3 = null;
            IToken token4 = null;
            IToken token5 = null;
            try
            {
                switch (this.LA(1))
                {
                    case 0x3b:
                        token2 = this.LT(1);
                        this.match(0x3b);
                        if (base.inputState.guessing == 0)
                        {
                            token = token2;
                        }
                        return token;

                    case 0x10:
                        token3 = this.LT(1);
                        this.match(0x10);
                        if (base.inputState.guessing == 0)
                        {
                            token = token3;
                            this.KeywordCannotBeUsedAsAnIdentifier(token);
                        }
                        return token;

                    case 0x21:
                        token4 = this.LT(1);
                        this.match(0x21);
                        if (base.inputState.guessing == 0)
                        {
                            token = token4;
                            this.KeywordCannotBeUsedAsAnIdentifier(token);
                        }
                        return token;

                    case 12:
                        token5 = this.LT(1);
                        this.match(12);
                        if (base.inputState.guessing == 0)
                        {
                            token = token5;
                        }
                        return token;
                }
                throw new NoViableAltException(this.LT(1), this.getFilename());
            }
            catch (RecognitionException exception)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception);
                this.recover(exception, tokenSet_38_);
                return token;
            }
            return token;
        }

        public void if_statement(Block container)
        {
            IToken token = null;
            IToken token2 = null;
            try
            {
                IfStatement statement2;
                Block block;
                token = this.LT(1);
                this.match(0x15);
                Expression expression = this.paren_expression();
                if (base.inputState.guessing == 0)
                {
                    IfStatement statement;
                    Block block2;
                    IfStatement statement1 = statement = new IfStatement(ToLexicalInfo(token));
                    statement.set_Condition(expression);
                    statement2 = statement;
                    statement2.set_TrueBlock(block2 = new Block());
                    block = block2;
                    container.Add(statement2);
                }
                this.compound_or_single_stmt(block);
                if ((this.LA(1) == 11) && tokenSet_2_.member(this.LA(2)))
                {
                    token2 = this.LT(1);
                    this.match(11);
                    if (base.inputState.guessing == 0)
                    {
                        Block block3;
                        statement2.set_FalseBlock(block3 = new Block(ToLexicalInfo(token2)));
                        block = block3;
                    }
                    this.compound_or_single_stmt(block);
                }
                else if (!tokenSet_15_.member(this.LA(1)) || !tokenSet_20_.member(this.LA(2)))
                {
                    throw new NoViableAltException(this.LT(1), this.getFilename());
                }
            }
            catch (RecognitionException exception)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception);
                this.recover(exception, tokenSet_15_);
            }
        }

        public void import_directive(Module container)
        {
            IToken token = null;
            try
            {
                token = this.LT(1);
                this.match(0x16);
                Token token2 = this.qname();
                this.eos();
                if (base.inputState.guessing == 0)
                {
                    container.get_Imports().Add(new Import(ToLexicalInfo(token), new ReferenceExpression(ToLexicalInfo(token2), token2.getText())));
                }
            }
            catch (RecognitionException exception)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception);
                this.recover(exception, tokenSet_5_);
            }
        }

        protected void initialize()
        {
            base.tokenNames = tokenNames_;
        }

        public IntegerLiteralExpression integer_literal()
        {
            IntegerLiteralExpression expression = null;
            IToken token = null;
            IToken token2 = null;
            try
            {
                switch (this.LA(1))
                {
                    case 0x6b:
                        token = this.LT(1);
                        this.match(0x6b);
                        if (base.inputState.guessing == 0)
                        {
                            expression = ParseIntegerLiteralExpression(token, token.getText(), false);
                        }
                        return expression;

                    case 0x6c:
                        token2 = this.LT(1);
                        this.match(0x6c);
                        if (base.inputState.guessing == 0)
                        {
                            string s = token2.getText();
                            expression = ParseIntegerLiteralExpression(token2, RuntimeServices.Mid(s, 0, -1), true);
                        }
                        return expression;
                }
                throw new NoViableAltException(this.LT(1), this.getFilename());
            }
            catch (RecognitionException exception)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception);
                this.recover(exception, tokenSet_20_);
                return expression;
            }
            return expression;
        }

        public TypeMember interface_declaration(TypeDefinition parent)
        {
            TypeMember member = null;
            IToken token = null;
            IToken token2 = null;
            try
            {
                InterfaceDefinition definition2;
                TypeReferenceCollection references;
                this.match(0x19);
                token = this.LT(1);
                this.match(0x3b);
                if (base.inputState.guessing == 0)
                {
                    InterfaceDefinition definition;
                    InterfaceDefinition definition1 = definition = new InterfaceDefinition(ToLexicalInfo(token));
                    definition.set_Name(token.getText());
                    member = definition2 = definition;
                    references = definition2.get_BaseTypes();
                    this.FlushAttributes(definition2);
                    parent.get_Members().Add(definition2);
                }
                switch (this.LA(1))
                {
                    case 14:
                        this.match(14);
                        this.type_reference_list(references);
                        break;

                    case 0x3d:
                        break;

                    default:
                        throw new NoViableAltException(this.LT(1), this.getFilename());
                }
                this.match(0x3d);
                while (true)
                {
                    if ((this.LA(1) != 0x13) && (this.LA(1) != 0x63))
                    {
                        break;
                    }
                    switch (this.LA(1))
                    {
                        case 0x63:
                            this.attributes();
                            break;

                        case 0x13:
                            break;

                        default:
                            throw new NoViableAltException(this.LT(1), this.getFilename());
                    }
                    this.interface_member(definition2);
                }
                token2 = this.LT(1);
                this.match(0x3e);
                if (base.inputState.guessing == 0)
                {
                    SetEndSourceLocation(definition2, token2);
                }
                while ((this.LA(1) == 0x4d) && tokenSet_33_.member(this.LA(2)))
                {
                    this.match(0x4d);
                }
                return member;
            }
            catch (RecognitionException exception)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception);
                this.recover(exception, tokenSet_33_);
                return member;
            }
            return member;
        }

        public void interface_member(TypeDefinition parent)
        {
            IToken getter = null;
            IToken setter = null;
            try
            {
                Method method;
                this.match(0x13);
                switch (this.LA(1))
                {
                    case 20:
                        getter = this.LT(1);
                        this.match(20);
                        break;

                    case 0x25:
                        setter = this.LT(1);
                        this.match(0x25);
                        break;

                    case 12:
                    case 0x10:
                    case 0x21:
                    case 0x3b:
                        break;

                    default:
                        throw new NoViableAltException(this.LT(1), this.getFilename());
                }
                IToken nameToken = this.identifier();
                if (base.inputState.guessing == 0)
                {
                    method = this.AddFunctionTo(parent, nameToken, getter, setter);
                }
                this.match(0x3f);
                switch (this.LA(1))
                {
                    case 12:
                    case 0x10:
                    case 0x21:
                    case 0x3b:
                    case 0x63:
                        this.parameter_declaration_list(method);
                        break;

                    case 0x40:
                        break;

                    default:
                        throw new NoViableAltException(this.LT(1), this.getFilename());
                }
                this.match(0x40);
                switch (this.LA(1))
                {
                    case 0x42:
                    {
                        this.match(0x42);
                        TypeReference reference = this.type_reference();
                        if (base.inputState.guessing == 0)
                        {
                            method.set_ReturnType(reference);
                        }
                        break;
                    }
                    case 0x13:
                    case 0x3e:
                    case 0x4d:
                    case 0x63:
                        break;

                    default:
                        throw new NoViableAltException(this.LT(1), this.getFilename());
                }
                while (this.LA(1) == 0x4d)
                {
                    this.match(0x4d);
                }
            }
            catch (RecognitionException exception)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception);
                this.recover(exception, tokenSet_37_);
            }
        }

        public static bool IsConstructorName(string name, TypeDefinition type)
        {
            bool flag1 = type.get_NodeType() != 11;
            if (!flag1)
            {
                return flag1;
            }
            return (name == type.get_Name());
        }

        public bool IsLabelInUse(Node node) => 
            node.ContainsAnnotation("LabelInUse");

        protected void KeywordCannotBeUsedAsAnIdentifier(IToken token)
        {
            this.ReportError(UnityScriptCompilerErrors.KeywordCannotBeUsedAsAnIdentifier(ToLexicalInfo(token), token.getText()));
        }

        public void LeaveLoop(object stmt)
        {
            if (stmt == null)
            {
                throw new ArgumentNullException("stmt");
            }
            object obj2 = this._loopStack.Pop();
            if (stmt != obj2)
            {
                throw new AssertionFailedException("stmt is top");
            }
        }

        public Expression literal()
        {
            Expression expression = null;
            try
            {
                switch (this.LA(1))
                {
                    case 0x6b:
                    case 0x6c:
                        return this.integer_literal();

                    case 60:
                    case 0x6d:
                        return this.string_literal();

                    case 0x44:
                        return this.array_literal();

                    case 0x3d:
                        return this.hash_literal();

                    case 0x69:
                        return this.re_literal();

                    case 15:
                    case 0x2a:
                        return this.bool_literal();

                    case 0x1d:
                        return this.null_literal();

                    case 40:
                        return this.self_literal();

                    case 0x27:
                        return this.super_literal();

                    case 0x6a:
                        return this.double_literal();
                }
                throw new NoViableAltException(this.LT(1), this.getFilename());
            }
            catch (RecognitionException exception)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception);
                this.recover(exception, tokenSet_20_);
                return expression;
            }
            return expression;
        }

        public Expression logical_and()
        {
            Expression expression = null;
            IToken token = null;
            try
            {
                expression = this.bitwise_or();
                while ((this.LA(1) == 0x4c) && tokenSet_16_.member(this.LA(2)))
                {
                    token = this.LT(1);
                    this.match(0x4c);
                    Expression expression2 = this.bitwise_or();
                    if (base.inputState.guessing == 0)
                    {
                        BinaryExpression expression3;
                        BinaryExpression expression1 = expression3 = new BinaryExpression(ToLexicalInfo(token));
                        expression3.set_Operator(0x1d);
                        expression3.set_Left(expression);
                        expression3.set_Right(expression2);
                        expression = expression3;
                    }
                }
                return expression;
            }
            catch (RecognitionException exception)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception);
                this.recover(exception, tokenSet_20_);
                return expression;
            }
            return expression;
        }

        public Expression logical_or()
        {
            Expression expression = null;
            IToken token = null;
            try
            {
                expression = this.logical_and();
                while ((this.LA(1) == 0x4b) && tokenSet_16_.member(this.LA(2)))
                {
                    token = this.LT(1);
                    this.match(0x4b);
                    Expression expression2 = this.logical_and();
                    if (base.inputState.guessing == 0)
                    {
                        BinaryExpression expression3;
                        BinaryExpression expression1 = expression3 = new BinaryExpression(ToLexicalInfo(token));
                        expression3.set_Operator(0x1c);
                        expression3.set_Left(expression);
                        expression3.set_Right(expression2);
                        expression = expression3;
                    }
                }
                return expression;
            }
            catch (RecognitionException exception)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception);
                this.recover(exception, tokenSet_20_);
                return expression;
            }
            return expression;
        }

        public void macro_application_block(Block container)
        {
            MacroStatement statement = new MacroStatement();
            ExpressionCollection ec = statement.get_Arguments();
            Block b = statement.get_Body();
            try
            {
                Token token = this.member();
                bool flag = false;
                if ((this.LA(1) == 0x3d) && tokenSet_13_.member(this.LA(2)))
                {
                    int num = this.mark();
                    flag = true;
                    base.inputState.guessing++;
                    try
                    {
                        this.match(0x3d);
                    }
                    catch (RecognitionException)
                    {
                        flag = false;
                    }
                    this.rewind(num);
                    base.inputState.guessing--;
                }
                if (flag)
                {
                    this.compound_statement(b);
                }
                else
                {
                    if (!tokenSet_16_.member(this.LA(1)) || !tokenSet_44_.member(this.LA(2)))
                    {
                        throw new NoViableAltException(this.LT(1), this.getFilename());
                    }
                    this.expression_list(ec);
                    this.compound_statement(b);
                }
                if (base.inputState.guessing == 0)
                {
                    statement.set_LexicalInfo(ToLexicalInfo(token));
                    statement.set_Name(token.getText());
                    container.Add(statement);
                }
            }
            catch (RecognitionException exception2)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception2);
                this.recover(exception2, tokenSet_15_);
            }
        }

        public void macro_application_test()
        {
            try
            {
                if (!tokenSet_42_.member(this.LA(1)) || (this.LA(2) != 0x3d))
                {
                    if (!tokenSet_42_.member(this.LA(1)) || !tokenSet_16_.member(this.LA(2)))
                    {
                        throw new NoViableAltException(this.LT(1), this.getFilename());
                    }
                    this.member();
                    this.expression_list(null);
                    this.match(0x3d);
                }
                else
                {
                    this.member();
                    this.match(0x3d);
                }
            }
            catch (RecognitionException exception)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception);
                this.recover(exception, tokenSet_4_);
            }
        }

        public string MacroName(string baseName) => 
            new StringBuilder("UnityScript.Macros.").Append(RuntimeServices.Mid(baseName, 0, 1).ToUpper()).Append(baseName.Substring(1)).ToString();

        public Token member()
        {
            Token token = null;
            IToken token2 = null;
            IToken token3 = null;
            IToken token4 = null;
            IToken token5 = null;
            try
            {
                switch (this.LA(1))
                {
                    case 0x3b:
                        token2 = this.LT(1);
                        this.match(0x3b);
                        if (base.inputState.guessing == 0)
                        {
                            token = token2;
                        }
                        return token;

                    case 0x25:
                        token3 = this.LT(1);
                        this.match(0x25);
                        if (base.inputState.guessing == 0)
                        {
                            token = token3;
                        }
                        return token;

                    case 20:
                        token4 = this.LT(1);
                        this.match(20);
                        if (base.inputState.guessing == 0)
                        {
                            token = token4;
                        }
                        return token;

                    case 12:
                        token5 = this.LT(1);
                        this.match(12);
                        if (base.inputState.guessing == 0)
                        {
                            token = token5;
                        }
                        return token;
                }
                throw new NoViableAltException(this.LT(1), this.getFilename());
            }
            catch (RecognitionException exception)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception);
                this.recover(exception, tokenSet_20_);
                return token;
            }
            return token;
        }

        public TypeMemberModifiers member_modifiers()
        {
            TypeMemberModifiers modifiers = new TypeMemberModifiers();
            IToken token = null;
            modifiers = 0;
            try
            {
                int num;
            Label_0011:
                num = this.LA(1);
                switch (num)
                {
                    case 0x10:
                        this.match(0x10);
                        if (base.inputState.guessing == 0)
                        {
                            modifiers |= 0x40;
                        }
                        goto Label_0011;

                    case 0x22:
                        this.match(0x22);
                        if (base.inputState.guessing == 0)
                        {
                            modifiers |= 0x100;
                        }
                        goto Label_0011;

                    case 0x1f:
                        this.match(0x1f);
                        if (base.inputState.guessing == 0)
                        {
                            modifiers |= 8;
                        }
                        goto Label_0011;

                    case 0x24:
                        this.match(0x24);
                        if (base.inputState.guessing == 0)
                        {
                            modifiers |= 1;
                        }
                        goto Label_0011;

                    case 0x20:
                        this.match(0x20);
                        if (base.inputState.guessing == 0)
                        {
                            modifiers |= 4;
                        }
                        goto Label_0011;

                    case 0x21:
                        this.match(0x21);
                        if (base.inputState.guessing == 0)
                        {
                            modifiers |= 2;
                        }
                        goto Label_0011;

                    case 0x26:
                        this.match(0x26);
                        if (base.inputState.guessing == 0)
                        {
                            modifiers |= 0x20;
                        }
                        goto Label_0011;

                    case 0x1b:
                        this.match(0x1b);
                        if (base.inputState.guessing == 0)
                        {
                            modifiers |= 0x800;
                        }
                        goto Label_0011;
                }
                if (num != 0x2e)
                {
                    return modifiers;
                }
                token = this.LT(1);
                this.match(0x2e);
                if (base.inputState.guessing == 0)
                {
                    this.VirtualKeywordHasNoEffect(token);
                }
                goto Label_0011;
            }
            catch (RecognitionException exception)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception);
                this.recover(exception, tokenSet_26_);
                return modifiers;
            }
            return modifiers;
        }

        public Expression member_reference_expression(Expression target)
        {
            Expression expression = null;
            IToken token = null;
            expression = target;
            try
            {
                int num = this.LA(1);
                if (num == 0x5b)
                {
                    TypeReferenceCollection references;
                    token = this.LT(1);
                    this.match(0x5b);
                    if (base.inputState.guessing == 0)
                    {
                        GenericReferenceExpression expression2;
                        GenericReferenceExpression expression3;
                        GenericReferenceExpression expression1 = expression2 = new GenericReferenceExpression(ToLexicalInfo(token));
                        expression2.set_Target(expression);
                        expression = expression3 = expression2;
                        references = expression3.get_GenericArguments();
                    }
                    this.type_reference_list(references);
                    this.match(0x5f);
                    return expression;
                }
                if (((num != 12) && (num != 20)) && ((num != 0x25) && (num != 0x3b)))
                {
                    throw new NoViableAltException(this.LT(1), this.getFilename());
                }
                Token token2 = this.member();
                if (base.inputState.guessing == 0)
                {
                    MemberReferenceExpression expression4;
                    MemberReferenceExpression expression7 = expression4 = new MemberReferenceExpression(ToLexicalInfo(token2));
                    expression4.set_Target(expression);
                    expression4.set_Name(token2.getText());
                    expression = expression4;
                }
                return expression;
            }
            catch (RecognitionException exception)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception);
                this.recover(exception, tokenSet_20_);
                return expression;
            }
            return expression;
        }

        private static long[] mk_tokenSet_0_()
        {
            long[] numArray1 = new long[2];
            numArray1[0] = 0x605b82092100L;
            return numArray1;
        }

        private static long[] mk_tokenSet_1_()
        {
            long[] numArray1 = new long[2];
            numArray1[0] = 0x800607b82193100L;
            return numArray1;
        }

        private static long[] mk_tokenSet_10_()
        {
            long[] numArray1 = new long[2];
            numArray1[0] = 0x405b82092100L;
            return numArray1;
        }

        private static long[] mk_tokenSet_11_()
        {
            long[] numArray1 = new long[2];
            numArray1[0] = 0x800407b82193100L;
            return numArray1;
        }

        private static long[] mk_tokenSet_12_()
        {
            long[] numArray1 = new long[4];
            numArray1[0] = -5187020888368892126L;
            numArray1[1] = 0x3e880105a010L;
            return numArray1;
        }

        private static long[] mk_tokenSet_13_()
        {
            long[] numArray1 = new long[4];
            numArray1[0] = -575405631708817888L;
            numArray1[1] = 0x3e800105a010L;
            return numArray1;
        }

        private static long[] mk_tokenSet_14_()
        {
            long[] numArray1 = new long[4];
            numArray1[0] = -504403175457964046L;
            numArray1[1] = 0x3fcffffffff2L;
            return numArray1;
        }

        private static long[] mk_tokenSet_15_()
        {
            long[] numArray1 = new long[4];
            numArray1[0] = -571957170220843102L;
            numArray1[1] = 0x3e880105a010L;
            return numArray1;
        }

        private static long[] mk_tokenSet_16_()
        {
            long[] numArray1 = new long[4];
            numArray1[0] = -5188123130559164416L;
            numArray1[1] = 0x3e8001058010L;
            return numArray1;
        }

        private static long[] mk_tokenSet_17_()
        {
            long[] numArray1 = new long[4];
            numArray1[0] = -576437111779454896L;
            numArray1[1] = 0x3f83bfff9b7bL;
            return numArray1;
        }

        private static long[] mk_tokenSet_18_()
        {
            long[] numArray1 = new long[4];
            numArray1[1] = 9L;
            return numArray1;
        }

        private static long[] mk_tokenSet_19_()
        {
            long[] numArray1 = new long[4];
            numArray1[0] = -9223372036854775808L;
            numArray1[1] = 0x4000L;
            return numArray1;
        }

        private static long[] mk_tokenSet_2_()
        {
            long[] numArray1 = new long[4];
            numArray1[0] = -5187091650136205792L;
            numArray1[1] = 0x3e800105a010L;
            return numArray1;
        }

        private static long[] mk_tokenSet_20_()
        {
            long[] numArray1 = new long[4];
            numArray1[0] = -504403158278094862L;
            numArray1[1] = 0x3fcfffffffffL;
            return numArray1;
        }

        private static long[] mk_tokenSet_21_()
        {
            long[] numArray1 = new long[4];
            numArray1[0] = 0x800605f8a093100L;
            numArray1[1] = 0x800000000L;
            return numArray1;
        }

        private static long[] mk_tokenSet_22_()
        {
            long[] numArray1 = new long[4];
            numArray1[0] = -5188123130559164416L;
            numArray1[1] = 0x3e8001058011L;
            return numArray1;
        }

        private static long[] mk_tokenSet_23_()
        {
            long[] numArray1 = new long[4];
            numArray1[0] = -4754675306957261022L;
            numArray1[1] = 0x3e880105a010L;
            return numArray1;
        }

        private static long[] mk_tokenSet_24_()
        {
            long[] numArray1 = new long[4];
            numArray1[0] = -75435293766994062L;
            numArray1[1] = 0x3fcfffffffffL;
            return numArray1;
        }

        private static long[] mk_tokenSet_25_()
        {
            long[] numArray1 = new long[4];
            numArray1[0] = -72057594037944334L;
            numArray1[1] = 0x3fcfffffffffL;
            return numArray1;
        }

        private static long[] mk_tokenSet_26_()
        {
            long[] numArray1 = new long[2];
            numArray1[0] = 0x200802082100L;
            return numArray1;
        }

        private static long[] mk_tokenSet_27_()
        {
            long[] numArray1 = new long[4];
            numArray1[0] = -139611588809211998L;
            numArray1[1] = 0x3e880105a010L;
            return numArray1;
        }

        private static long[] mk_tokenSet_28_()
        {
            long[] numArray1 = new long[4];
            numArray1[0] = -72057594046332942L;
            numArray1[1] = 0x3fcfffffffffL;
            return numArray1;
        }

        private static long[] mk_tokenSet_29_()
        {
            long[] numArray1 = new long[4];
            numArray1[0] = 0x605f8a092100L;
            numArray1[1] = 0x800000000L;
            return numArray1;
        }

        private static long[] mk_tokenSet_3_()
        {
            long[] numArray1 = new long[4];
            numArray1[0] = -507780875178625166L;
            numArray1[1] = 0x3fcffffffff2L;
            return numArray1;
        }

        private static long[] mk_tokenSet_30_()
        {
            long[] numArray1 = new long[2];
            numArray1[0] = 0x605f8a092100L;
            return numArray1;
        }

        private static long[] mk_tokenSet_31_()
        {
            long[] numArray1 = new long[2];
            numArray1[0] = 0x800607f8a193100L;
            return numArray1;
        }

        private static long[] mk_tokenSet_32_()
        {
            long[] numArray1 = new long[2];
            numArray1[0] = 0x800002200111100L;
            return numArray1;
        }

        private static long[] mk_tokenSet_33_()
        {
            long[] numArray1 = new long[4];
            numArray1[0] = -575334852761635038L;
            numArray1[1] = 0x3e880105a010L;
            return numArray1;
        }

        private static long[] mk_tokenSet_34_()
        {
            long[] numArray1 = new long[4];
            numArray1[0] = 0x800000200011000L;
            numArray1[1] = 0x800000000L;
            return numArray1;
        }

        private static long[] mk_tokenSet_35_()
        {
            long[] numArray1 = new long[4];
            numArray1[0] = -504403158269706254L;
            numArray1[1] = 0x3fcfffffffffL;
            return numArray1;
        }

        private static long[] mk_tokenSet_36_()
        {
            long[] numArray1 = new long[4];
            numArray1[0] = 0x2000000000000000L;
            numArray1[1] = 0x80000000L;
            return numArray1;
        }

        private static long[] mk_tokenSet_37_()
        {
            long[] numArray1 = new long[4];
            numArray1[0] = 0x4000000000080000L;
            numArray1[1] = 0x800000000L;
            return numArray1;
        }

        private static long[] mk_tokenSet_38_()
        {
            long[] numArray1 = new long[4];
            numArray1[0] = -571957153024196702L;
            numArray1[1] = 0x3e880105e01dL;
            return numArray1;
        }

        private static long[] mk_tokenSet_39_()
        {
            long[] numArray1 = new long[4];
            numArray1[1] = 1L;
            return numArray1;
        }

        private static long[] mk_tokenSet_4_()
        {
            long[] numArray1 = new long[2];
            numArray1[0] = 2L;
            return numArray1;
        }

        private static long[] mk_tokenSet_40_()
        {
            long[] numArray1 = new long[4];
            numArray1[0] = 0x4000000000000000L;
            numArray1[1] = 8L;
            return numArray1;
        }

        private static long[] mk_tokenSet_41_()
        {
            long[] numArray1 = new long[4];
            numArray1[0] = -571957153040973918L;
            numArray1[1] = 0x3e880105a010L;
            return numArray1;
        }

        private static long[] mk_tokenSet_42_()
        {
            long[] numArray1 = new long[2];
            numArray1[0] = 0x800002000101000L;
            return numArray1;
        }

        private static long[] mk_tokenSet_43_()
        {
            long[] numArray1 = new long[4];
            numArray1[0] = -5187091787576207840L;
            numArray1[1] = 0x3e8001058010L;
            return numArray1;
        }

        private static long[] mk_tokenSet_44_()
        {
            long[] numArray1 = new long[4];
            numArray1[0] = -575405631356496272L;
            numArray1[1] = 0x3f83bfffbb7aL;
            return numArray1;
        }

        private static long[] mk_tokenSet_45_()
        {
            long[] numArray1 = new long[2];
            numArray1[0] = 0x800200200011000L;
            return numArray1;
        }

        private static long[] mk_tokenSet_46_()
        {
            long[] numArray1 = new long[2];
            numArray1[0] = 0x800000201011000L;
            return numArray1;
        }

        private static long[] mk_tokenSet_47_()
        {
            long[] numArray1 = new long[4];
            numArray1[0] = -5188087946187075584L;
            numArray1[1] = 0x3e800105a010L;
            return numArray1;
        }

        private static long[] mk_tokenSet_48_()
        {
            long[] numArray1 = new long[4];
            numArray1[0] = -508883108778897328L;
            numArray1[1] = 0x3fc7fffffff2L;
            return numArray1;
        }

        private static long[] mk_tokenSet_49_()
        {
            long[] numArray1 = new long[4];
            numArray1[0] = -571957169868521486L;
            numArray1[1] = 0x3f8bbfffbb72L;
            return numArray1;
        }

        private static long[] mk_tokenSet_5_()
        {
            long[] numArray1 = new long[4];
            numArray1[0] = -4754675324137130206L;
            numArray1[1] = 0x3e880105a010L;
            return numArray1;
        }

        private static long[] mk_tokenSet_50_()
        {
            long[] numArray1 = new long[4];
            numArray1[0] = -571957170204065886L;
            numArray1[1] = 0x3e880105e010L;
            return numArray1;
        }

        private static long[] mk_tokenSet_51_()
        {
            long[] numArray1 = new long[4];
            numArray1[0] = -571957170220843102L;
            numArray1[1] = 0x3e880105a011L;
            return numArray1;
        }

        private static long[] mk_tokenSet_52_()
        {
            long[] numArray1 = new long[4];
            numArray1[0] = 0x2000000000000000L;
            numArray1[1] = 0x21L;
            return numArray1;
        }

        private static long[] mk_tokenSet_53_()
        {
            long[] numArray1 = new long[4];
            numArray1[1] = 40L;
            return numArray1;
        }

        private static long[] mk_tokenSet_54_()
        {
            long[] numArray1 = new long[4];
            numArray1[0] = -5188123130559164416L;
            numArray1[1] = 0x3e8001058014L;
            return numArray1;
        }

        private static long[] mk_tokenSet_55_()
        {
            long[] numArray1 = new long[4];
            numArray1[0] = 0x800002000101000L;
            numArray1[1] = 0x8000000L;
            return numArray1;
        }

        private static long[] mk_tokenSet_56_()
        {
            long[] numArray1 = new long[4];
            numArray1[0] = 0x15000000L;
            numArray1[1] = 0x198000000L;
            return numArray1;
        }

        private static long[] mk_tokenSet_57_()
        {
            long[] numArray1 = new long[4];
            numArray1[0] = -5188123130542387200L;
            numArray1[1] = 0x3e8001058010L;
            return numArray1;
        }

        private static long[] mk_tokenSet_58_()
        {
            long[] numArray1 = new long[4];
            numArray1[1] = 0x6600000L;
            return numArray1;
        }

        private static long[] mk_tokenSet_59_()
        {
            long[] numArray1 = new long[4];
            numArray1[0] = -576437111779192752L;
            numArray1[1] = 0x3f83bfff9b72L;
            return numArray1;
        }

        private static long[] mk_tokenSet_6_()
        {
            long[] numArray1 = new long[2];
            numArray1[0] = 0x800605f8a093100L;
            return numArray1;
        }

        private static long[] mk_tokenSet_60_()
        {
            long[] numArray1 = new long[4];
            numArray1[0] = -5188123130559164416L;
            numArray1[1] = 0x3e8001058030L;
            return numArray1;
        }

        private static long[] mk_tokenSet_7_()
        {
            long[] numArray1 = new long[2];
            numArray1[0] = 0x605380010000L;
            return numArray1;
        }

        private static long[] mk_tokenSet_8_()
        {
            long[] numArray1 = new long[2];
            numArray1[0] = 0x800605380011000L;
            return numArray1;
        }

        private static long[] mk_tokenSet_9_()
        {
            long[] numArray1 = new long[2];
            numArray1[0] = 0x800000200011000L;
            return numArray1;
        }

        public void module_field(Module m)
        {
            try
            {
                TypeMemberModifiers modifiers = this.module_member_modifiers();
                TypeMember member = this.field_member(m);
                if (base.inputState.guessing == 0)
                {
                    member.set_Modifiers(member.get_Modifiers() | modifiers);
                }
            }
            catch (RecognitionException exception)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception);
                this.recover(exception, tokenSet_12_);
            }
        }

        public void module_member(Module module)
        {
            Block b = module.get_Globals();
            try
            {
                bool flag = false;
                if ((tokenSet_7_.member(this.LA(1)) && tokenSet_8_.member(this.LA(2))) && this.GlobalVariablesBecomeFields())
                {
                    int num = this.mark();
                    flag = true;
                    base.inputState.guessing++;
                    try
                    {
                        this.module_member_modifiers();
                        this.match(0x2d);
                    }
                    catch (RecognitionException)
                    {
                        flag = false;
                    }
                    this.rewind(num);
                    base.inputState.guessing--;
                }
                if (flag)
                {
                    this.module_field(module);
                }
                else if ((this.LA(1) == 0x2d) && tokenSet_9_.member(this.LA(2)))
                {
                    this.declaration_statement(b);
                    this.eos();
                }
                else
                {
                    TypeMember member;
                    if (!tokenSet_10_.member(this.LA(1)) || !tokenSet_11_.member(this.LA(2)))
                    {
                        throw new NoViableAltException(this.LT(1), this.getFilename());
                    }
                    TypeMemberModifiers modifiers = this.module_member_modifiers();
                    switch (this.LA(1))
                    {
                        case 8:
                        case 0x23:
                            member = this.class_declaration(module);
                            break;

                        case 0x19:
                            member = this.interface_declaration(module);
                            break;

                        case 13:
                            member = this.enum_declaration(module);
                            break;

                        case 0x13:
                            member = this.function_member(module);
                            break;

                        default:
                            throw new NoViableAltException(this.LT(1), this.getFilename());
                    }
                    if ((base.inputState.guessing == 0) && (member != null))
                    {
                        member.set_Modifiers(member.get_Modifiers() | modifiers);
                    }
                }
            }
            catch (RecognitionException exception2)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception2);
                this.recover(exception2, tokenSet_12_);
            }
        }

        public TypeMemberModifiers module_member_modifiers()
        {
            TypeMemberModifiers modifiers = new TypeMemberModifiers();
            IToken token = null;
            modifiers = 0;
            try
            {
                int num;
            Label_0011:
                num = this.LA(1);
                switch (num)
                {
                    case 0x10:
                        this.match(0x10);
                        if (base.inputState.guessing == 0)
                        {
                            modifiers |= 0x40;
                        }
                        goto Label_0011;

                    case 0x1f:
                        this.match(0x1f);
                        if (base.inputState.guessing == 0)
                        {
                            modifiers |= 8;
                        }
                        goto Label_0011;

                    case 0x24:
                        this.match(0x24);
                        if (base.inputState.guessing == 0)
                        {
                            modifiers |= 1;
                        }
                        goto Label_0011;

                    case 0x20:
                        this.match(0x20);
                        if (base.inputState.guessing == 0)
                        {
                            modifiers |= 4;
                        }
                        goto Label_0011;

                    case 0x21:
                        this.match(0x21);
                        if (base.inputState.guessing == 0)
                        {
                            modifiers |= 2;
                        }
                        goto Label_0011;

                    case 0x26:
                        this.match(0x26);
                        if (base.inputState.guessing == 0)
                        {
                            modifiers |= 0x20;
                        }
                        goto Label_0011;
                }
                if (num != 0x2e)
                {
                    return modifiers;
                }
                token = this.LT(1);
                this.match(0x2e);
                if (base.inputState.guessing == 0)
                {
                    this.VirtualKeywordHasNoEffect(token);
                }
                goto Label_0011;
            }
            catch (RecognitionException exception)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception);
                this.recover(exception, tokenSet_26_);
                return modifiers;
            }
            return modifiers;
        }

        public Expression new_array_expression()
        {
            Expression expression = null;
            try
            {
                this.match(0x1b);
                expression = this.array_initializer();
            }
            catch (RecognitionException exception)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception);
                this.recover(exception, tokenSet_20_);
                return expression;
            }
            return expression;
        }

        public Expression new_expression()
        {
            Expression expression = null;
            try
            {
                ExpressionCollection expressions;
                bool flag = false;
                if ((this.LA(1) == 0x1b) && (this.LA(2) == 0x3b))
                {
                    int num = this.mark();
                    flag = true;
                    base.inputState.guessing++;
                    try
                    {
                        this.new_array_expression();
                    }
                    catch (RecognitionException)
                    {
                        flag = false;
                    }
                    this.rewind(num);
                    base.inputState.guessing--;
                }
                if (flag)
                {
                    return this.new_array_expression();
                }
                if ((this.LA(1) != 0x1b) || ((this.LA(2) != 12) && (this.LA(2) != 0x3b)))
                {
                    throw new NoViableAltException(this.LT(1), this.getFilename());
                }
                this.match(0x1b);
                Expression expression2 = this.reference_expression();
                if (base.inputState.guessing == 0)
                {
                    MethodInvocationExpression expression3;
                    MethodInvocationExpression expression4;
                    MethodInvocationExpression expression1 = expression3 = new MethodInvocationExpression(expression2.get_LexicalInfo());
                    expression3.set_Target(expression2);
                    expression = expression4 = expression3;
                    expressions = expression4.get_Arguments();
                }
                this.match(0x3f);
                this.expression_list(expressions);
                this.match(0x40);
                return expression;
            }
            catch (RecognitionException exception2)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception2);
                this.recover(exception2, tokenSet_20_);
                return expression;
            }
            return expression;
        }

        public NullLiteralExpression null_literal()
        {
            NullLiteralExpression expression = null;
            IToken token = null;
            try
            {
                token = this.LT(1);
                this.match(0x1d);
                if (base.inputState.guessing == 0)
                {
                    expression = new NullLiteralExpression(ToLexicalInfo(token));
                }
            }
            catch (RecognitionException exception)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception);
                this.recover(exception, tokenSet_20_);
                return expression;
            }
            return expression;
        }

        public void parameter_declaration(INodeWithParameters m)
        {
            try
            {
                TypeReference reference;
                switch (this.LA(1))
                {
                    case 0x63:
                        this.attributes();
                        break;

                    case 12:
                    case 0x10:
                    case 0x21:
                    case 0x3b:
                        break;

                    default:
                        throw new NoViableAltException(this.LT(1), this.getFilename());
                }
                IToken token = this.identifier();
                switch (this.LA(1))
                {
                    case 0x42:
                        this.match(0x42);
                        reference = this.type_reference();
                        break;

                    case 0x40:
                    case 0x43:
                        break;

                    default:
                        throw new NoViableAltException(this.LT(1), this.getFilename());
                }
                if (base.inputState.guessing == 0)
                {
                    ParameterDeclaration declaration;
                    ParameterDeclaration declaration1 = declaration = new ParameterDeclaration(ToLexicalInfo(token));
                    declaration.set_Name(token.getText());
                    declaration.set_Type(reference);
                    ParameterDeclaration node = declaration;
                    m.get_Parameters().Add(node);
                    this.FlushAttributes(node);
                }
            }
            catch (RecognitionException exception)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception);
                this.recover(exception, tokenSet_18_);
            }
        }

        public void parameter_declaration_list(INodeWithParameters m)
        {
            try
            {
                this.parameter_declaration(m);
                while (this.LA(1) == 0x43)
                {
                    this.match(0x43);
                    this.parameter_declaration(m);
                }
            }
            catch (RecognitionException exception)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception);
                this.recover(exception, tokenSet_39_);
            }
        }

        public Expression paren_expression()
        {
            Expression expression = null;
            try
            {
                this.match(0x3f);
                expression = this.expression();
                this.match(0x40);
            }
            catch (RecognitionException exception)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception);
                this.recover(exception, tokenSet_20_);
                return expression;
            }
            return expression;
        }

        public static Expression ParseExpression(TextReader expression, string fileName, Boo.Lang.Compiler.CompilerContext context)
        {
            UnityScriptParser parser;
            UnityScriptLexer lexer = UnityScriptLexerFor(expression, fileName, TabSizeFromContext(context));
            if (lexer == null)
            {
                Expression expression2;
                return expression2;
            }
            UnityScriptParser parser1 = parser = new UnityScriptParser(lexer);
            Boo.Lang.Compiler.CompilerContext context1 = parser.CompilerContext = context;
            UnityScriptParser parser2 = parser;
            parser2.setFilename(fileName);
            try
            {
                return parser2.expression();
            }
            catch (TokenStreamRecognitionException exception)
            {
                parser2.reportError(exception.recog);
            }
            return null;
        }

        public static Expression ParseExpression(string expression, string fileName, Boo.Lang.Compiler.CompilerContext context) => 
            ParseExpression(new StringReader(expression), fileName, context);

        public static IntegerLiteralExpression ParseIntegerLiteralExpression(IToken token, string s, bool isLong)
        {
            long num;
            string str = "0x";
            if (s.StartsWith(str))
            {
                num = long.Parse(s.Substring(str.Length), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture);
            }
            else
            {
                num = long.Parse(s, CultureInfo.InvariantCulture);
            }
            return new IntegerLiteralExpression(ToLexicalInfo(token), num, isLong);
        }

        public static void ParseReader(TextReader reader, string fileName, Boo.Lang.Compiler.CompilerContext context, CompileUnit targetCompileUnit)
        {
            UnityScriptLexer lexer = UnityScriptLexerFor(reader, fileName, TabSizeFromContext(context));
            if (lexer == null)
            {
                targetCompileUnit.get_Modules().Add(CodeFactory.NewModule(fileName));
            }
            else
            {
                UnityScriptParser parser;
                UnityScriptParser parser1 = parser = new UnityScriptParser(lexer);
                Boo.Lang.Compiler.CompilerContext context1 = parser.CompilerContext = context;
                UnityScriptParser parser2 = parser;
                parser2.setFilename(fileName);
                try
                {
                    parser2.start(targetCompileUnit);
                }
                catch (TokenStreamRecognitionException exception)
                {
                    parser2.reportError(exception.recog);
                }
            }
        }

        public Expression postfix_unary_expression()
        {
            Expression expression = null;
            IToken token = null;
            IToken token2 = null;
            try
            {
                IToken token3;
                UnaryOperatorType type;
                expression = this.slicing_expression();
                if ((this.LA(1) == 0x4f) && tokenSet_20_.member(this.LA(2)))
                {
                    token = this.LT(1);
                    this.match(0x4f);
                    if (base.inputState.guessing == 0)
                    {
                        token3 = token;
                        type = 4;
                    }
                }
                else if ((this.LA(1) == 80) && tokenSet_20_.member(this.LA(2)))
                {
                    token2 = this.LT(1);
                    this.match(80);
                    if (base.inputState.guessing == 0)
                    {
                        token3 = token2;
                        type = 5;
                    }
                }
                else if (!tokenSet_20_.member(this.LA(1)) || !tokenSet_28_.member(this.LA(2)))
                {
                    throw new NoViableAltException(this.LT(1), this.getFilename());
                }
                if ((base.inputState.guessing == 0) && (token3 != null))
                {
                    expression = new UnaryExpression(ToLexicalInfo(token3), type, expression);
                }
            }
            catch (RecognitionException exception)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception);
                this.recover(exception, tokenSet_20_);
                return expression;
            }
            return expression;
        }

        public void pragma_directive(Module container)
        {
            IToken token = null;
            IToken token2 = null;
            try
            {
                IToken token3;
                switch (this.LA(1))
                {
                    case 0x39:
                        token = this.LT(1);
                        this.match(0x39);
                        if (base.inputState.guessing == 0)
                        {
                            token3 = token;
                        }
                        break;

                    case 0x3a:
                        token2 = this.LT(1);
                        this.match(0x3a);
                        if (base.inputState.guessing == 0)
                        {
                            token3 = token2;
                        }
                        break;

                    default:
                        throw new NoViableAltException(this.LT(1), this.getFilename());
                }
                if (base.inputState.guessing == 0)
                {
                    string pragma = token3.getText();
                    if (Pragmas.IsValid(pragma))
                    {
                        if (token != null)
                        {
                            Pragmas.TryToEnableOn(container, pragma);
                        }
                        else
                        {
                            Pragmas.DisableOn(container, pragma);
                        }
                    }
                    else
                    {
                        this.ReportError(UnityScriptCompilerErrors.UnknownPragma(ToLexicalInfo(token3), pragma));
                    }
                }
            }
            catch (RecognitionException exception)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception);
                this.recover(exception, tokenSet_5_);
            }
        }

        public Expression prefix_unary_expression()
        {
            Expression expression = null;
            IToken token = null;
            IToken token2 = null;
            IToken token3 = null;
            IToken token4 = null;
            IToken token5 = null;
            UnaryOperatorType type = 0;
            try
            {
                IToken token6;
                switch (this.LA(1))
                {
                    case 0x52:
                        token = this.LT(1);
                        this.match(0x52);
                        if (base.inputState.guessing == 0)
                        {
                            token6 = token;
                            type = 1;
                        }
                        break;

                    case 0x4f:
                        token2 = this.LT(1);
                        this.match(0x4f);
                        if (base.inputState.guessing == 0)
                        {
                            token6 = token2;
                            type = 2;
                        }
                        break;

                    case 80:
                        token3 = this.LT(1);
                        this.match(80);
                        if (base.inputState.guessing == 0)
                        {
                            token6 = token3;
                            type = 3;
                        }
                        break;

                    case 0x67:
                        token4 = this.LT(1);
                        this.match(0x67);
                        if (base.inputState.guessing == 0)
                        {
                            token6 = token4;
                            type = 6;
                        }
                        break;

                    case 0x58:
                        token5 = this.LT(1);
                        this.match(0x58);
                        if (base.inputState.guessing == 0)
                        {
                            token6 = token5;
                            type = 8;
                        }
                        break;

                    default:
                        throw new NoViableAltException(this.LT(1), this.getFilename());
                }
                expression = this.unary_expression();
                if (base.inputState.guessing == 0)
                {
                    expression = new UnaryExpression(ToLexicalInfo(token6), type, expression);
                }
            }
            catch (RecognitionException exception)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception);
                this.recover(exception, tokenSet_20_);
                return expression;
            }
            return expression;
        }

        public Token qname()
        {
            Token token = null;
            IToken token2 = null;
            IToken token3 = null;
            try
            {
                StringBuilder builder;
                token2 = this.LT(1);
                this.match(0x3b);
                if (base.inputState.guessing == 0)
                {
                    token = token2;
                    builder = new StringBuilder();
                    builder.Append(token2.getText());
                }
                while ((this.LA(1) == 0x41) && (this.LA(2) == 0x3b))
                {
                    this.match(0x41);
                    token3 = this.LT(1);
                    this.match(0x3b);
                    if (base.inputState.guessing == 0)
                    {
                        builder.Append(".");
                        builder.Append(token3.getText());
                    }
                }
                if (base.inputState.guessing == 0)
                {
                    token.setText(builder.ToString());
                }
            }
            catch (RecognitionException exception)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception);
                this.recover(exception, tokenSet_25_);
                return token;
            }
            return token;
        }

        public RELiteralExpression re_literal()
        {
            RELiteralExpression expression = null;
            IToken token = null;
            try
            {
                token = this.LT(1);
                this.match(0x69);
                if (base.inputState.guessing == 0)
                {
                    expression = new RELiteralExpression(ToLexicalInfo(token), token.getText());
                }
            }
            catch (RecognitionException exception)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception);
                this.recover(exception, tokenSet_20_);
                return expression;
            }
            return expression;
        }

        public Expression reference_expression()
        {
            Expression target = null;
            try
            {
                target = this.simple_reference_expression();
                while (this.LA(1) == 0x41)
                {
                    this.match(0x41);
                    target = this.member_reference_expression(target);
                }
                return target;
            }
            catch (RecognitionException exception)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception);
                this.recover(exception, tokenSet_19_);
                return target;
            }
            return target;
        }

        public override void reportError(RecognitionException x)
        {
            LexicalInfo info = new LexicalInfo(x.getFilename(), x.getLine(), x.getColumn());
            NoViableAltException exception = x as NoViableAltException;
            if (exception != null)
            {
                this.ReportError(CompilerErrorFactory.UnexpectedToken(info, x, exception.token.getText()));
            }
            else
            {
                this.ReportError(CompilerErrorFactory.GenericParserError(info, x));
            }
        }

        protected void ReportError(CompilerError error)
        {
            this._context.get_Errors().Add(error);
        }

        public void return_statement(Block b)
        {
            IToken token = null;
            try
            {
                Expression expression;
                token = this.LT(1);
                this.match(30);
                if (tokenSet_16_.member(this.LA(1)) && tokenSet_49_.member(this.LA(2)))
                {
                    expression = this.expression();
                }
                else if (!tokenSet_15_.member(this.LA(1)) || !tokenSet_20_.member(this.LA(2)))
                {
                    throw new NoViableAltException(this.LT(1), this.getFilename());
                }
                if (base.inputState.guessing == 0)
                {
                    ReturnStatement statement;
                    ReturnStatement statement1 = statement = new ReturnStatement(ToLexicalInfo(token));
                    statement.set_Expression(expression);
                    b.Add(statement);
                }
            }
            catch (RecognitionException exception)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception);
                this.recover(exception, tokenSet_15_);
            }
        }

        public void script_or_assembly_attribute(Module m)
        {
            IToken token = null;
            try
            {
                this.match(0x63);
                token = this.LT(1);
                this.match(0x3b);
                Attribute attribute = this.attribute_constructor();
                if (base.inputState.guessing == 0)
                {
                    string str = token.getText();
                    if (str == "assembly")
                    {
                        m.get_AssemblyAttributes().Add(attribute);
                    }
                    else if (str == "script")
                    {
                        m.get_Attributes().Add(attribute);
                    }
                    else
                    {
                        this.UnexpectedToken(token);
                    }
                }
            }
            catch (RecognitionException exception)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception);
                this.recover(exception, tokenSet_5_);
            }
        }

        public SelfLiteralExpression self_literal()
        {
            SelfLiteralExpression expression = null;
            IToken token = null;
            try
            {
                token = this.LT(1);
                this.match(40);
                if (base.inputState.guessing == 0)
                {
                    expression = new SelfLiteralExpression(ToLexicalInfo(token));
                }
            }
            catch (RecognitionException exception)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception);
                this.recover(exception, tokenSet_20_);
                return expression;
            }
            return expression;
        }

        protected void SemicolonExpected()
        {
            LexicalInfo info;
            if (this._last != null)
            {
                info = new LexicalInfo(this._last.getFilename(), this._last.getLine(), this._last.getColumn() + this._last.getText().Length);
            }
            else
            {
                info = ToLexicalInfo(this.LT(1));
            }
            this.ReportError(UnityScriptCompilerErrors.SemicolonExpected(info));
        }

        public static void SetEndSourceLocation(Node node, IToken token)
        {
            node.set_EndSourceLocation(ToSourceLocation(token));
        }

        public string SetUpLoopLabel(Node node)
        {
            string[] textArray1 = new string[] { "for" };
            string uniqueName = this._context.GetUniqueName(textArray1);
            node.set_Item("UpdateLabel", uniqueName);
            return uniqueName;
        }

        public Expression shift()
        {
            Expression expression = null;
            IToken token = null;
            IToken token2 = null;
            try
            {
                expression = this.sum();
                while (true)
                {
                    BinaryOperatorType type;
                    IToken token3;
                    if ((this.LA(1) != 0x5d) && (this.LA(1) != 0x61))
                    {
                        return expression;
                    }
                    if (!tokenSet_16_.member(this.LA(2)))
                    {
                        return expression;
                    }
                    switch (this.LA(1))
                    {
                        case 0x5d:
                            token = this.LT(1);
                            this.match(0x5d);
                            if (base.inputState.guessing == 0)
                            {
                                type = 0x22;
                                token3 = token;
                            }
                            break;

                        case 0x61:
                            token2 = this.LT(1);
                            this.match(0x61);
                            if (base.inputState.guessing == 0)
                            {
                                type = 0x24;
                                token3 = token2;
                            }
                            break;

                        default:
                            throw new NoViableAltException(this.LT(1), this.getFilename());
                    }
                    Expression expression2 = this.sum();
                    if (base.inputState.guessing == 0)
                    {
                        BinaryExpression expression3;
                        BinaryExpression expression1 = expression3 = new BinaryExpression(ToLexicalInfo(token3));
                        expression3.set_Operator(type);
                        expression3.set_Left(expression);
                        expression3.set_Right(expression2);
                        expression = expression3;
                    }
                }
            }
            catch (RecognitionException exception)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception);
                this.recover(exception, tokenSet_20_);
                return expression;
            }
            return expression;
        }

        public Expression simple_reference_expression()
        {
            Expression expression = null;
            IToken token = null;
            IToken token2 = null;
            try
            {
                switch (this.LA(1))
                {
                    case 0x3b:
                        token = this.LT(1);
                        this.match(0x3b);
                        break;

                    case 12:
                        token2 = this.LT(1);
                        this.match(12);
                        if (base.inputState.guessing == 0)
                        {
                            token = token2;
                        }
                        break;

                    default:
                        throw new NoViableAltException(this.LT(1), this.getFilename());
                }
                if (base.inputState.guessing == 0)
                {
                    ReferenceExpression expression2;
                    ReferenceExpression expression1 = expression2 = new ReferenceExpression(ToLexicalInfo(token));
                    expression2.set_Name(token.getText());
                    expression = expression2;
                }
            }
            catch (RecognitionException exception)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception);
                this.recover(exception, tokenSet_20_);
                return expression;
            }
            return expression;
        }

        public TypeReference simple_type_reference()
        {
            TypeReference reference = null;
            try
            {
                Token token = this.qname();
                if ((this.LA(1) == 0x41) && (this.LA(2) == 0x5b))
                {
                    TypeReferenceCollection references;
                    this.match(0x41);
                    this.match(0x5b);
                    if (base.inputState.guessing == 0)
                    {
                        GenericTypeReference reference2;
                        reference = reference2 = new GenericTypeReference(ToLexicalInfo(token), token.getText());
                        references = reference2.get_GenericArguments();
                    }
                    this.type_reference_list(references);
                    this.match(0x5f);
                    return reference;
                }
                if (!tokenSet_35_.member(this.LA(1)) || !tokenSet_25_.member(this.LA(2)))
                {
                    throw new NoViableAltException(this.LT(1), this.getFilename());
                }
                if (base.inputState.guessing == 0)
                {
                    SimpleTypeReference reference3;
                    SimpleTypeReference reference1 = reference3 = new SimpleTypeReference(ToLexicalInfo(token));
                    reference3.set_Name(token.getText());
                    reference = reference3;
                }
                return reference;
            }
            catch (RecognitionException exception)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception);
                this.recover(exception, tokenSet_35_);
                return reference;
            }
            return reference;
        }

        public void slice(SlicingExpression se)
        {
            Expression expression = null;
            Expression expression2 = null;
            Expression expression3 = null;
            try
            {
                int num = this.LA(1);
                if (num == 0x42)
                {
                    this.match(0x42);
                    if (base.inputState.guessing == 0)
                    {
                        expression = OmittedExpression.Default;
                    }
                    num = this.LA(1);
                    switch (num)
                    {
                        case 12:
                        case 15:
                        case 0x13:
                        case 0x1b:
                        case 0x1d:
                        case 0x27:
                        case 40:
                        case 0x2a:
                        case 0x2c:
                        case 0x3b:
                        case 60:
                        case 0x3d:
                        case 0x3f:
                        case 0x44:
                        case 0x4f:
                        case 80:
                        case 0x52:
                        case 0x58:
                        case 0x67:
                        case 0x69:
                        case 0x6a:
                        case 0x6b:
                        case 0x6c:
                        case 0x6d:
                            expression2 = this.expression();
                            goto Label_03F1;

                        case 0x42:
                            this.match(0x42);
                            if (base.inputState.guessing == 0)
                            {
                                expression2 = OmittedExpression.Default;
                            }
                            expression3 = this.expression();
                            goto Label_03F1;

                        case 0x43:
                        case 0x45:
                            goto Label_03F1;
                    }
                    throw new NoViableAltException(this.LT(1), this.getFilename());
                }
                if ((((((num != 12) && (num != 15)) && ((num != 0x13) && (num != 0x1b))) && (((num != 0x1d) && (num != 0x27)) && ((num != 40) && (num != 0x2a)))) && ((((num != 0x2c) && (num != 0x3b)) && ((num != 60) && (num != 0x3d))) && (((num != 0x3f) && (num != 0x44)) && ((num != 0x4f) && (num != 80))))) && ((((num != 0x52) && (num != 0x58)) && ((num != 0x67) && (num != 0x69))) && (((num != 0x6a) && (num != 0x6b)) && ((num != 0x6c) && (num != 0x6d)))))
                {
                    throw new NoViableAltException(this.LT(1), this.getFilename());
                }
                expression = this.expression();
                num = this.LA(1);
                if (num == 0x42)
                {
                    this.match(0x42);
                    num = this.LA(1);
                    switch (num)
                    {
                        case 12:
                        case 15:
                        case 0x13:
                        case 0x1b:
                        case 0x1d:
                        case 0x27:
                        case 40:
                        case 0x2a:
                        case 0x2c:
                        case 0x3b:
                        case 60:
                        case 0x3d:
                        case 0x3f:
                        case 0x44:
                        case 0x4f:
                        case 80:
                        case 0x52:
                        case 0x58:
                        case 0x67:
                        case 0x69:
                        case 0x6a:
                        case 0x6b:
                        case 0x6c:
                        case 0x6d:
                            expression2 = this.expression();
                            break;

                        default:
                            if (((num != 0x42) && (num != 0x43)) && (num != 0x45))
                            {
                                throw new NoViableAltException(this.LT(1), this.getFilename());
                            }
                            if (base.inputState.guessing == 0)
                            {
                                expression2 = OmittedExpression.Default;
                            }
                            break;
                    }
                    num = this.LA(1);
                    switch (num)
                    {
                        case 0x42:
                            this.match(0x42);
                            expression3 = this.expression();
                            goto Label_03F1;

                        case 0x43:
                        case 0x45:
                            goto Label_03F1;

                        default:
                            throw new NoViableAltException(this.LT(1), this.getFilename());
                    }
                }
                switch (num)
                {
                    case 0x43:
                    case 0x45:
                        break;

                    default:
                        throw new NoViableAltException(this.LT(1), this.getFilename());
                }
            Label_03F1:
                if (base.inputState.guessing == 0)
                {
                    se.get_Indices().Add(new Slice(expression, expression2, expression3));
                }
            }
            catch (RecognitionException exception)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception);
                this.recover(exception, tokenSet_53_);
            }
        }

        public Expression slicing_expression()
        {
            Expression target = null;
            IToken token = null;
            IToken token2 = null;
            SlicingExpression se = null;
            MethodInvocationExpression expression3 = null;
            ExpressionCollection ec = null;
            try
            {
                target = this.atom();
                while (true)
                {
                    while ((this.LA(1) == 0x44) && tokenSet_54_.member(this.LA(2)))
                    {
                        token = this.LT(1);
                        this.match(0x44);
                        if (base.inputState.guessing == 0)
                        {
                            se = new SlicingExpression(ToLexicalInfo(token));
                            se.set_Target(target);
                            target = se;
                        }
                        this.slice(se);
                        while (this.LA(1) == 0x43)
                        {
                            this.match(0x43);
                            this.slice(se);
                        }
                        this.match(0x45);
                    }
                    if ((this.LA(1) == 0x41) && tokenSet_55_.member(this.LA(2)))
                    {
                        this.match(0x41);
                        target = this.member_reference_expression(target);
                    }
                    else
                    {
                        if ((this.LA(1) != 0x3f) || !tokenSet_22_.member(this.LA(2)))
                        {
                            return target;
                        }
                        token2 = this.LT(1);
                        this.match(0x3f);
                        if (base.inputState.guessing == 0)
                        {
                            expression3 = new MethodInvocationExpression(ToLexicalInfo(token2));
                            expression3.set_Target(target);
                            target = expression3;
                            ec = expression3.get_Arguments();
                        }
                        this.expression_list(ec);
                        this.match(0x40);
                    }
                }
            }
            catch (RecognitionException exception)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception);
                this.recover(exception, tokenSet_20_);
                return target;
            }
            return target;
        }

        public void start(CompileUnit cu)
        {
            IToken token = null;
            Module container = CodeFactory.NewModule(this.getFilename());
            cu.get_Modules().Add(container);
            Block b = container.get_Globals();
            try
            {
                bool flag2;
            Label_0026:
                switch (this.LA(1))
                {
                    case 0x16:
                        this.import_directive(container);
                        goto Label_0026;

                    case 0x39:
                    case 0x3a:
                        this.pragma_directive(container);
                        goto Label_0026;

                    default:
                    {
                        bool flag = false;
                        if ((this.LA(1) == 0x63) && (this.LA(2) == 0x3b))
                        {
                            int num2 = this.mark();
                            flag = true;
                            base.inputState.guessing++;
                            try
                            {
                                this.match(0x63);
                                this.match(0x3b);
                                this.match(0x3b);
                            }
                            catch (RecognitionException)
                            {
                                flag = false;
                            }
                            this.rewind(num2);
                            base.inputState.guessing--;
                        }
                        if (flag)
                        {
                            this.script_or_assembly_attribute(container);
                            goto Label_0026;
                        }
                        break;
                    }
                }
            Label_0109:
                flag2 = false;
                if ((this.LA(1) == 0x63) && (this.LA(2) == 0x3b))
                {
                    int num3 = this.mark();
                    flag2 = true;
                    base.inputState.guessing++;
                    try
                    {
                        this.match(0x63);
                        this.match(0x3b);
                        this.match(0x3b);
                    }
                    catch (RecognitionException)
                    {
                        flag2 = false;
                    }
                    this.rewind(num3);
                    base.inputState.guessing--;
                }
                if (flag2)
                {
                    this.script_or_assembly_attribute(container);
                    goto Label_0109;
                }
                if ((this.LA(1) == 0x63) && (this.LA(2) == 0x3b))
                {
                    this.attributes();
                    this.module_member(container);
                    goto Label_0109;
                }
                if (tokenSet_0_.member(this.LA(1)) && tokenSet_1_.member(this.LA(2)))
                {
                    this.module_member(container);
                    goto Label_0109;
                }
                if (tokenSet_2_.member(this.LA(1)) && tokenSet_3_.member(this.LA(2)))
                {
                    this.compound_or_single_stmt(b);
                    goto Label_0109;
                }
                token = this.LT(1);
                this.match(1);
                if (base.inputState.guessing == 0)
                {
                    SetEndSourceLocation(container, token);
                }
            }
            catch (RecognitionException exception3)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception3);
                this.recover(exception3, tokenSet_4_);
            }
        }

        public void statement(Block b)
        {
            try
            {
                bool flag = false;
                if (tokenSet_42_.member(this.LA(1)) && tokenSet_16_.member(this.LA(2)))
                {
                    int num = this.mark();
                    flag = true;
                    base.inputState.guessing++;
                    try
                    {
                        this.macro_application_test();
                    }
                    catch (RecognitionException)
                    {
                        flag = false;
                    }
                    this.rewind(num);
                    base.inputState.guessing--;
                }
                if (flag)
                {
                    this.macro_application_block(b);
                }
                else if (tokenSet_43_.member(this.LA(1)) && tokenSet_14_.member(this.LA(2)))
                {
                    this.builtin_statement(b);
                }
                else
                {
                    if (this.LA(1) != 0x4d)
                    {
                        throw new NoViableAltException(this.LT(1), this.getFilename());
                    }
                    int num2 = 0;
                    while (true)
                    {
                        if ((this.LA(1) == 0x4d) && tokenSet_15_.member(this.LA(2)))
                        {
                            this.match(0x4d);
                        }
                        else
                        {
                            if (num2 < 1)
                            {
                                throw new NoViableAltException(this.LT(1), this.getFilename());
                            }
                            return;
                        }
                        num2++;
                    }
                }
            }
            catch (RecognitionException exception2)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception2);
                this.recover(exception2, tokenSet_15_);
            }
        }

        public Expression string_literal()
        {
            Expression expression = null;
            IToken token = null;
            IToken token2 = null;
            try
            {
                IToken token3;
                switch (this.LA(1))
                {
                    case 60:
                        token = this.LT(1);
                        this.match(60);
                        if (base.inputState.guessing == 0)
                        {
                            token3 = token;
                        }
                        break;

                    case 0x6d:
                        token2 = this.LT(1);
                        this.match(0x6d);
                        if (base.inputState.guessing == 0)
                        {
                            token3 = token2;
                        }
                        break;

                    default:
                        throw new NoViableAltException(this.LT(1), this.getFilename());
                }
                if (base.inputState.guessing == 0)
                {
                    expression = new StringLiteralExpression(ToLexicalInfo(token3), token3.getText());
                }
            }
            catch (RecognitionException exception)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception);
                this.recover(exception, tokenSet_20_);
                return expression;
            }
            return expression;
        }

        public Expression sum()
        {
            Expression expression = null;
            IToken token = null;
            IToken token2 = null;
            BinaryOperatorType type = 0;
            try
            {
                expression = this.term();
                while (true)
                {
                    IToken token3;
                    if ((this.LA(1) != 0x51) && (this.LA(1) != 0x52))
                    {
                        return expression;
                    }
                    if (!tokenSet_16_.member(this.LA(2)))
                    {
                        return expression;
                    }
                    switch (this.LA(1))
                    {
                        case 0x51:
                            token = this.LT(1);
                            this.match(0x51);
                            if (base.inputState.guessing == 0)
                            {
                                token3 = token;
                                type = 1;
                            }
                            break;

                        case 0x52:
                            token2 = this.LT(1);
                            this.match(0x52);
                            if (base.inputState.guessing == 0)
                            {
                                token3 = token2;
                                type = 2;
                            }
                            break;

                        default:
                            throw new NoViableAltException(this.LT(1), this.getFilename());
                    }
                    Expression expression2 = this.term();
                    if (base.inputState.guessing == 0)
                    {
                        BinaryExpression expression3 = new BinaryExpression(ToLexicalInfo(token3));
                        expression3.set_Operator(type);
                        expression3.set_Left(expression);
                        expression3.set_Right(expression2);
                        expression = expression3;
                    }
                }
            }
            catch (RecognitionException exception)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception);
                this.recover(exception, tokenSet_20_);
                return expression;
            }
            return expression;
        }

        public SuperLiteralExpression super_literal()
        {
            SuperLiteralExpression expression = null;
            IToken token = null;
            try
            {
                token = this.LT(1);
                this.match(0x27);
                if (base.inputState.guessing == 0)
                {
                    expression = new SuperLiteralExpression(ToLexicalInfo(token));
                }
            }
            catch (RecognitionException exception)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception);
                this.recover(exception, tokenSet_20_);
                return expression;
            }
            return expression;
        }

        public void switch_statement(Block container)
        {
            IToken token = null;
            IToken token2 = null;
            IToken token3 = null;
            IToken token4 = null;
            try
            {
                Block block;
                MacroStatement statement4;
                Block block2;
                token = this.LT(1);
                this.match(0x31);
                Expression expression = this.paren_expression();
                if (base.inputState.guessing == 0)
                {
                    MacroStatement statement;
                    MacroStatement statement1 = statement = new MacroStatement(ToLexicalInfo(token));
                    statement.set_Name(this.MacroName(token.getText()));
                    MacroStatement statement2 = statement;
                    statement2.get_Arguments().Add(expression);
                    block = statement2.get_Body();
                    container.Add(statement2);
                }
                this.match(0x3d);
            Label_0088:
                if (this.LA(1) == 50)
                {
                    token2 = this.LT(1);
                    this.match(50);
                    expression = this.expression();
                    this.match(0x42);
                    if (base.inputState.guessing == 0)
                    {
                        MacroStatement statement3;
                        MacroStatement statement6 = statement3 = new MacroStatement(ToLexicalInfo(token2));
                        statement3.set_Name(token2.getText());
                        statement4 = statement3;
                        statement4.get_Arguments().Add(expression);
                        block2 = statement4.get_Body();
                        block.Add(statement4);
                    }
                    while (this.LA(1) == 50)
                    {
                        token3 = this.LT(1);
                        this.match(50);
                        expression = this.expression();
                        this.match(0x42);
                        if (base.inputState.guessing == 0)
                        {
                            statement4.get_Arguments().Add(expression);
                        }
                    }
                    int num = 0;
                    while (true)
                    {
                        if (tokenSet_2_.member(this.LA(1)))
                        {
                            this.statement(block2);
                        }
                        else
                        {
                            if (num < 1)
                            {
                                throw new NoViableAltException(this.LT(1), this.getFilename());
                            }
                            goto Label_0088;
                        }
                        num++;
                    }
                }
                int num2 = this.LA(1);
                if (num2 == 0x33)
                {
                    token4 = this.LT(1);
                    this.match(0x33);
                    this.match(0x42);
                    if (base.inputState.guessing == 0)
                    {
                        MacroStatement statement5;
                        MacroStatement statement7 = statement5 = new MacroStatement(ToLexicalInfo(token4));
                        statement5.set_Name(token4.getText());
                        statement4 = statement5;
                        block2 = statement4.get_Body();
                        block.Add(statement4);
                    }
                    int num3 = 0;
                    while (true)
                    {
                        if (tokenSet_2_.member(this.LA(1)))
                        {
                            this.statement(block2);
                        }
                        else
                        {
                            if (num3 < 1)
                            {
                                throw new NoViableAltException(this.LT(1), this.getFilename());
                            }
                            goto Label_02C9;
                        }
                        num3++;
                    }
                }
                if (num2 != 0x3e)
                {
                    throw new NoViableAltException(this.LT(1), this.getFilename());
                }
            Label_02C9:
                this.match(0x3e);
                while ((this.LA(1) == 0x4d) && tokenSet_15_.member(this.LA(2)))
                {
                    this.match(0x4d);
                }
            }
            catch (RecognitionException exception)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception);
                this.recover(exception, tokenSet_15_);
            }
        }

        public static int TabSizeFromContext(Boo.Lang.Compiler.CompilerContext context)
        {
            int num;
            Boo.Lang.Compiler.CompilerContext context2 = context;
            if (context2 is Boo.Lang.Compiler.CompilerContext)
            {
                Boo.Lang.Compiler.CompilerContext context3;
                Boo.Lang.Compiler.CompilerContext context1 = context3 = context2;
                if ((1 != 0) && (context3.get_Parameters() is UnityScriptCompilerParameters))
                {
                    UnityScriptCompilerParameters parameters;
                    UnityScriptCompilerParameters parameters1 = parameters = context3.get_Parameters();
                    if (1 != 0)
                    {
                        int num1 = num = parameters.TabSize;
                    }
                }
            }
            return ((1 == 0) ? 8 : num);
        }

        public Expression term()
        {
            Expression expression = null;
            IToken token = null;
            IToken token2 = null;
            IToken token3 = null;
            BinaryOperatorType type = 0;
            try
            {
                expression = this.unary_expression();
                while (true)
                {
                    IToken token4;
                    if (((this.LA(1) != 0x53) && (this.LA(1) != 0x54)) && (this.LA(1) != 0x68))
                    {
                        return expression;
                    }
                    if (!tokenSet_16_.member(this.LA(2)))
                    {
                        return expression;
                    }
                    switch (this.LA(1))
                    {
                        case 0x54:
                            token = this.LT(1);
                            this.match(0x54);
                            if (base.inputState.guessing == 0)
                            {
                                type = 3;
                                token4 = token;
                            }
                            break;

                        case 0x68:
                            token2 = this.LT(1);
                            this.match(0x68);
                            if (base.inputState.guessing == 0)
                            {
                                type = 4;
                                token4 = token2;
                            }
                            break;

                        case 0x53:
                            token3 = this.LT(1);
                            this.match(0x53);
                            if (base.inputState.guessing == 0)
                            {
                                type = 5;
                                token4 = token3;
                            }
                            break;

                        default:
                            throw new NoViableAltException(this.LT(1), this.getFilename());
                    }
                    Expression expression2 = this.unary_expression();
                    if (base.inputState.guessing == 0)
                    {
                        BinaryExpression expression3 = new BinaryExpression(ToLexicalInfo(token4));
                        expression3.set_Operator(type);
                        expression3.set_Left(expression);
                        expression3.set_Right(expression2);
                        expression = expression3;
                    }
                }
            }
            catch (RecognitionException exception)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception);
                this.recover(exception, tokenSet_20_);
                return expression;
            }
            return expression;
        }

        public void throw_statement(Block b)
        {
            IToken token = null;
            try
            {
                Expression expression;
                token = this.LT(1);
                this.match(0x29);
                if (tokenSet_16_.member(this.LA(1)) && tokenSet_49_.member(this.LA(2)))
                {
                    expression = this.expression();
                }
                else if (!tokenSet_15_.member(this.LA(1)) || !tokenSet_20_.member(this.LA(2)))
                {
                    throw new NoViableAltException(this.LT(1), this.getFilename());
                }
                if (base.inputState.guessing == 0)
                {
                    RaiseStatement statement;
                    RaiseStatement statement1 = statement = new RaiseStatement(ToLexicalInfo(token));
                    statement.set_Exception(expression);
                    b.Add(statement);
                }
            }
            catch (RecognitionException exception)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception);
                this.recover(exception, tokenSet_15_);
            }
        }

        public static LexicalInfo ToLexicalInfo(IToken token) => 
            new LexicalInfo(token.getFilename(), token.getLine(), token.getColumn());

        public static SourceLocation ToSourceLocation(IToken token)
        {
            string str = token.getText();
            int num = (str != null) ? (str.Length - 1) : 0;
            return new SourceLocation(token.getLine(), token.getColumn() + num);
        }

        public void try_statement(Block container)
        {
            IToken token = null;
            IToken token2 = null;
            IToken token3 = null;
            try
            {
                TryStatement statement;
                Block block;
                token = this.LT(1);
                this.match(0x2b);
                if (base.inputState.guessing == 0)
                {
                    statement = new TryStatement(ToLexicalInfo(token));
                    block = statement.get_ProtectedBlock();
                    container.Add(statement);
                }
                this.compound_or_single_stmt(block);
                while (true)
                {
                    TypeReference reference;
                    if ((this.LA(1) != 7) || (this.LA(2) != 0x3f))
                    {
                        break;
                    }
                    token2 = this.LT(1);
                    this.match(7);
                    this.match(0x3f);
                    token3 = this.LT(1);
                    this.match(0x3b);
                    switch (this.LA(1))
                    {
                        case 0x42:
                            this.match(0x42);
                            reference = this.type_reference();
                            break;

                        case 0x40:
                            break;

                        default:
                            throw new NoViableAltException(this.LT(1), this.getFilename());
                    }
                    this.match(0x40);
                    if (base.inputState.guessing == 0)
                    {
                        Declaration declaration;
                        ExceptionHandler handler;
                        if (reference == null)
                        {
                            reference = new SimpleTypeReference(ToLexicalInfo(token3), "System.Exception");
                        }
                        ExceptionHandler handler1 = handler = new ExceptionHandler(ToLexicalInfo(token2));
                        Declaration declaration1 = declaration = new Declaration(ToLexicalInfo(token3));
                        declaration.set_Name(token3.getText());
                        declaration.set_Type(reference);
                        handler.set_Declaration(declaration);
                        ExceptionHandler handler2 = handler;
                        statement.get_ExceptionHandlers().Add(handler2);
                        block = handler2.get_Block();
                        reference = null;
                    }
                    this.compound_or_single_stmt(block);
                }
                if ((this.LA(1) != 0x11) || !tokenSet_2_.member(this.LA(2)))
                {
                    if (!tokenSet_15_.member(this.LA(1)) || !tokenSet_20_.member(this.LA(2)))
                    {
                        throw new NoViableAltException(this.LT(1), this.getFilename());
                    }
                }
                else
                {
                    this.finally_block(statement);
                }
            }
            catch (RecognitionException exception)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception);
                this.recover(exception, tokenSet_15_);
            }
        }

        public TypeReference type_reference()
        {
            TypeReference reference = null;
            IToken token = null;
            int num = 1;
            try
            {
                switch (this.LA(1))
                {
                    case 0x3b:
                        reference = this.simple_type_reference();
                        break;

                    case 0x13:
                        reference = this.anonymous_function_type();
                        break;

                    default:
                        throw new NoViableAltException(this.LT(1), this.getFilename());
                }
                if ((this.LA(1) == 0x44) && ((this.LA(2) == 0x43) || (this.LA(2) == 0x45)))
                {
                    token = this.LT(1);
                    this.match(0x44);
                    while (this.LA(1) == 0x43)
                    {
                        this.match(0x43);
                        if (base.inputState.guessing == 0)
                        {
                            num++;
                        }
                    }
                    this.match(0x45);
                    if (base.inputState.guessing == 0)
                    {
                        reference = new ArrayTypeReference(reference.get_LexicalInfo(), reference, new IntegerLiteralExpression(ToLexicalInfo(token), (long) num));
                    }
                    return reference;
                }
                if (!tokenSet_35_.member(this.LA(1)) || !tokenSet_25_.member(this.LA(2)))
                {
                    throw new NoViableAltException(this.LT(1), this.getFilename());
                }
                return reference;
            }
            catch (RecognitionException exception)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception);
                this.recover(exception, tokenSet_35_);
                return reference;
            }
            return reference;
        }

        public void type_reference_list(TypeReferenceCollection typeReferences)
        {
            try
            {
                TypeReference reference = this.type_reference();
                if (base.inputState.guessing == 0)
                {
                    typeReferences.Add(reference);
                }
                while (this.LA(1) == 0x43)
                {
                    this.match(0x43);
                    reference = this.type_reference();
                    if (base.inputState.guessing == 0)
                    {
                        typeReferences.Add(reference);
                    }
                }
            }
            catch (RecognitionException exception)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception);
                this.recover(exception, tokenSet_36_);
            }
        }

        public Expression typeof_expression()
        {
            Expression expression = null;
            try
            {
                bool flag = false;
                if ((this.LA(1) == 0x2c) && tokenSet_16_.member(this.LA(2)))
                {
                    int num = this.mark();
                    flag = true;
                    base.inputState.guessing++;
                    try
                    {
                        this.match(0x2c);
                        this.match(0x3f);
                        this.expression();
                        this.match(0x40);
                    }
                    catch (RecognitionException)
                    {
                        flag = false;
                    }
                    this.rewind(num);
                    base.inputState.guessing--;
                }
                if (flag)
                {
                    return this.typeof_with_expression();
                }
                if ((this.LA(1) != 0x2c) || !tokenSet_16_.member(this.LA(2)))
                {
                    throw new NoViableAltException(this.LT(1), this.getFilename());
                }
                return this.typeof_expression_alt();
            }
            catch (RecognitionException exception2)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception2);
                this.recover(exception2, tokenSet_20_);
                return expression;
            }
            return expression;
        }

        protected Expression typeof_expression_alt()
        {
            Expression expression = null;
            try
            {
                bool flag = false;
                if ((this.LA(1) == 0x2c) && (this.LA(2) == 0x3f))
                {
                    int num = this.mark();
                    flag = true;
                    base.inputState.guessing++;
                    try
                    {
                        this.match(0x2c);
                        this.match(0x3f);
                        this.type_reference();
                        this.match(0x40);
                    }
                    catch (RecognitionException)
                    {
                        flag = false;
                    }
                    this.rewind(num);
                    base.inputState.guessing--;
                }
                if (flag)
                {
                    return this.typeof_with_typeref();
                }
                if ((this.LA(1) != 0x2c) || !tokenSet_16_.member(this.LA(2)))
                {
                    throw new NoViableAltException(this.LT(1), this.getFilename());
                }
                return this.typeof_with_expression();
            }
            catch (RecognitionException exception2)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception2);
                this.recover(exception2, tokenSet_20_);
                return expression;
            }
            return expression;
        }

        protected Expression typeof_with_expression()
        {
            Expression expression = null;
            IToken token = null;
            try
            {
                Expression expression2;
                token = this.LT(1);
                this.match(0x2c);
                if ((this.LA(1) == 0x3f) && tokenSet_16_.member(this.LA(2)))
                {
                    this.match(0x3f);
                    expression2 = this.expression();
                    this.match(0x40);
                }
                else
                {
                    if (!tokenSet_16_.member(this.LA(1)) || !tokenSet_20_.member(this.LA(2)))
                    {
                        throw new NoViableAltException(this.LT(1), this.getFilename());
                    }
                    expression2 = this.expression();
                }
                if (base.inputState.guessing == 0)
                {
                    ReferenceExpression expression4;
                    MethodInvocationExpression expression3 = new MethodInvocationExpression(ToLexicalInfo(token));
                    ReferenceExpression expression1 = expression4 = new ReferenceExpression(ToLexicalInfo(token));
                    expression4.set_Name(token.getText());
                    expression3.set_Target(expression4);
                    expression3.get_Arguments().Add(expression2);
                    expression = expression3;
                }
            }
            catch (RecognitionException exception)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception);
                this.recover(exception, tokenSet_20_);
                return expression;
            }
            return expression;
        }

        protected Expression typeof_with_typeref()
        {
            Expression expression = null;
            IToken token = null;
            try
            {
                token = this.LT(1);
                this.match(0x2c);
                this.match(0x3f);
                TypeReference reference = this.type_reference();
                this.match(0x40);
                if (base.inputState.guessing == 0)
                {
                    expression = new TypeofExpression(ToLexicalInfo(token), reference);
                }
            }
            catch (RecognitionException exception)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception);
                this.recover(exception, tokenSet_20_);
                return expression;
            }
            return expression;
        }

        public Expression unary_expression()
        {
            Expression expression = null;
            IToken token = null;
            IToken token2 = null;
            try
            {
                TypeReference reference;
                int num = this.LA(1);
                switch (num)
                {
                    case 0x4f:
                    case 80:
                    case 0x52:
                    case 0x58:
                    case 0x67:
                        expression = this.prefix_unary_expression();
                        break;

                    default:
                        if (((((num != 12) && (num != 15)) && ((num != 0x13) && (num != 0x1b))) && (((num != 0x1d) && (num != 0x27)) && ((num != 40) && (num != 0x2a)))) && ((((num != 0x2c) && (num != 0x3b)) && ((num != 60) && (num != 0x3d))) && ((((num != 0x3f) && (num != 0x44)) && ((num != 0x69) && (num != 0x6a))) && (((num != 0x6b) && (num != 0x6c)) && (num != 0x6d)))))
                        {
                            throw new NoViableAltException(this.LT(1), this.getFilename());
                        }
                        expression = this.postfix_unary_expression();
                        break;
                }
                if ((this.LA(1) == 4) && ((this.LA(2) == 0x13) || (this.LA(2) == 0x3b)))
                {
                    token = this.LT(1);
                    this.match(4);
                    reference = this.type_reference();
                    if (base.inputState.guessing == 0)
                    {
                        TryCastExpression expression2;
                        TryCastExpression expression1 = expression2 = new TryCastExpression(ToLexicalInfo(token));
                        expression2.set_Target(expression);
                        expression2.set_Type(reference);
                        expression = expression2;
                    }
                    return expression;
                }
                if ((this.LA(1) == 6) && ((this.LA(2) == 0x13) || (this.LA(2) == 0x3b)))
                {
                    token2 = this.LT(1);
                    this.match(6);
                    reference = this.type_reference();
                    if (base.inputState.guessing == 0)
                    {
                        CastExpression expression3;
                        CastExpression expression6 = expression3 = new CastExpression(ToLexicalInfo(token2));
                        expression3.set_Target(expression);
                        expression3.set_Type(reference);
                        expression = expression3;
                    }
                    return expression;
                }
                if (!tokenSet_20_.member(this.LA(1)) || !tokenSet_28_.member(this.LA(2)))
                {
                    throw new NoViableAltException(this.LT(1), this.getFilename());
                }
                return expression;
            }
            catch (RecognitionException exception)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception);
                this.recover(exception, tokenSet_20_);
                return expression;
            }
            return expression;
        }

        protected void UnexpectedToken(IToken token)
        {
            this.ReportError(CompilerErrorFactory.UnexpectedToken(ToLexicalInfo(token), null, token.getText()));
        }

        public static UnityScriptLexer UnityScriptLexerFor(TextReader reader, string fileName, int tabSize)
        {
            UnityScriptLexer lexer = new UnityScriptLexer(reader);
            lexer.setFilename(fileName);
            lexer.setTokenCreator(new BooToken.BooTokenCreator());
            lexer.setTabSize(tabSize);
            return ((reader.Peek() != -1) ? lexer : null);
        }

        protected void VirtualKeywordHasNoEffect(IToken token)
        {
            this._context.get_Warnings().Add(UnityScriptWarnings.VirtualKeywordHasNoEffect(ToLexicalInfo(token)));
        }

        public void while_statement(Block container)
        {
            IToken token = null;
            try
            {
                WhileStatement statement2;
                Block block;
                token = this.LT(1);
                this.match(0x2f);
                Expression expression = this.paren_expression();
                if (base.inputState.guessing == 0)
                {
                    WhileStatement statement;
                    WhileStatement statement1 = statement = new WhileStatement(ToLexicalInfo(token));
                    statement.set_Condition(expression);
                    statement2 = statement;
                    block = statement2.get_Block();
                    container.Add(statement2);
                    this.EnterLoop(statement2);
                }
                this.compound_or_single_stmt(block);
                if (base.inputState.guessing == 0)
                {
                    this.LeaveLoop(statement2);
                }
            }
            catch (RecognitionException exception)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception);
                this.recover(exception, tokenSet_15_);
            }
        }

        public void yield_statement(Block b)
        {
            IToken token = null;
            try
            {
                Expression expression;
                token = this.LT(1);
                this.match(0x30);
                if (tokenSet_16_.member(this.LA(1)) && tokenSet_49_.member(this.LA(2)))
                {
                    expression = this.expression();
                }
                else if (!tokenSet_15_.member(this.LA(1)) || !tokenSet_20_.member(this.LA(2)))
                {
                    throw new NoViableAltException(this.LT(1), this.getFilename());
                }
                if (base.inputState.guessing == 0)
                {
                    YieldStatement statement;
                    YieldStatement statement1 = statement = new YieldStatement(ToLexicalInfo(token));
                    statement.set_Expression(expression);
                    b.Add(statement);
                }
            }
            catch (RecognitionException exception)
            {
                if (base.inputState.guessing != 0)
                {
                    throw;
                }
                this.reportError(exception);
                this.recover(exception, tokenSet_15_);
            }
        }

        public Boo.Lang.Compiler.CompilerContext CompilerContext
        {
            get => 
                this._context;
            set
            {
                this._context = value;
            }
        }

        public UnityScriptCompilerParameters UnityScriptParameters =>
            ((UnityScriptCompilerParameters) this._context.get_Parameters());
    }
}

