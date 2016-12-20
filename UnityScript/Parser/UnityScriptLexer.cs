namespace UnityScript.Parser
{
    using antlr;
    using antlr.collections.impl;
    using Boo.Lang.Runtime;
    using System;
    using System.Collections;
    using System.IO;

    [Serializable]
    public class UnityScriptLexer : CharScanner, TokenStream
    {
        private bool $initialized__UnityScript_Parser_UnityScriptLexer$;
        private bool $PreserveComments$26;
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
        public static readonly BitSet tokenSet_0_ = new BitSet(mk_tokenSet_0_());
        [NonSerialized]
        public static readonly BitSet tokenSet_1_ = new BitSet(mk_tokenSet_1_());
        [NonSerialized]
        public static readonly BitSet tokenSet_2_ = new BitSet(mk_tokenSet_2_());
        [NonSerialized]
        public static readonly BitSet tokenSet_3_ = new BitSet(mk_tokenSet_3_());
        [NonSerialized]
        public static readonly BitSet tokenSet_4_ = new BitSet(mk_tokenSet_4_());
        [NonSerialized]
        public static readonly BitSet tokenSet_5_ = new BitSet(mk_tokenSet_5_());
        [NonSerialized]
        public static readonly BitSet tokenSet_6_ = new BitSet(mk_tokenSet_6_());
        [NonSerialized]
        public static readonly BitSet tokenSet_7_ = new BitSet(mk_tokenSet_7_());
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

        public UnityScriptLexer(InputBuffer ib) : this(new LexerSharedInputState(ib))
        {
            if (!this.$initialized__UnityScript_Parser_UnityScriptLexer$)
            {
                this.$initialized__UnityScript_Parser_UnityScriptLexer$ = true;
            }
        }

        public UnityScriptLexer(LexerSharedInputState state) : base(state)
        {
            if (!this.$initialized__UnityScript_Parser_UnityScriptLexer$)
            {
                this.$initialized__UnityScript_Parser_UnityScriptLexer$ = true;
            }
            this.initialize();
        }

        public UnityScriptLexer(Stream ins) : this((InputBuffer) new ByteBuffer(ins))
        {
            if (!this.$initialized__UnityScript_Parser_UnityScriptLexer$)
            {
                this.$initialized__UnityScript_Parser_UnityScriptLexer$ = true;
            }
        }

        public UnityScriptLexer(TextReader r) : this((InputBuffer) new CharBuffer(r))
        {
            if (!this.$initialized__UnityScript_Parser_UnityScriptLexer$)
            {
                this.$initialized__UnityScript_Parser_UnityScriptLexer$ = true;
            }
        }

        private void initialize()
        {
            base.caseSensitiveLiterals = true;
            this.setCaseSensitive(true);
            base.literals = new Hashtable(100, (float) 0.4, null, Comparer.Default);
            base.literals.Add(",", 0x43);
            base.literals.Add("public", 0x1f);
            base.literals.Add("a string", 60);
            base.literals.Add("an identifier", 0x3b);
            base.literals.Add("]", 0x45);
            base.literals.Add("case", 50);
            base.literals.Add("break", 5);
            base.literals.Add("while", 0x2f);
            base.literals.Add("new", 0x1b);
            base.literals.Add("||", 0x4b);
            base.literals.Add("+", 0x51);
            base.literals.Add("instanceof", 0x1a);
            base.literals.Add("implements", 0x17);
            base.literals.Add("*", 0x54);
            base.literals.Add("|=", 0x47);
            base.literals.Add("typeof", 0x2c);
            base.literals.Add("@assembly", 0x65);
            base.literals.Add("[", 0x44);
            base.literals.Add(">>=", 0x62);
            base.literals.Add("not", 0x1c);
            base.literals.Add("return", 30);
            base.literals.Add("throw", 0x29);
            base.literals.Add("var", 0x2d);
            base.literals.Add(")", 0x40);
            base.literals.Add("==", 0x55);
            base.literals.Add("null", 0x1d);
            base.literals.Add("protected", 0x20);
            base.literals.Add("pragma off", 0x3a);
            base.literals.Add("@script", 100);
            base.literals.Add("class", 8);
            base.literals.Add("(", 0x3f);
            base.literals.Add("do", 10);
            base.literals.Add("~", 0x58);
            base.literals.Add("function", 0x13);
            base.literals.Add("/=", 0x34);
            base.literals.Add("super", 0x27);
            base.literals.Add("each", 12);
            base.literals.Add("@", 0x63);
            base.literals.Add("-=", 0x36);
            base.literals.Add("set", 0x25);
            base.literals.Add("+=", 0x35);
            base.literals.Add("!==", 90);
            base.literals.Add("}", 0x3e);
            base.literals.Add("interface", 0x19);
            base.literals.Add("?", 0x57);
            base.literals.Add("&", 0x48);
            base.literals.Add("internal", 0x21);
            base.literals.Add("final", 0x10);
            base.literals.Add("yield", 0x30);
            base.literals.Add("!=", 0x56);
            base.literals.Add("//", 0x38);
            base.literals.Add("===", 0x59);
            base.literals.Add("if", 0x15);
            base.literals.Add("|", 70);
            base.literals.Add("override", 0x22);
            base.literals.Add(">", 0x5f);
            base.literals.Add("as", 4);
            base.literals.Add("%", 0x53);
            base.literals.Add("catch", 7);
            base.literals.Add("try", 0x2b);
            base.literals.Add("{", 0x3d);
            base.literals.Add("=", 0x4e);
            base.literals.Add("enum", 13);
            base.literals.Add("for", 0x12);
            base.literals.Add(">>", 0x61);
            base.literals.Add("extends", 14);
            base.literals.Add("private", 0x24);
            base.literals.Add("default", 0x33);
            base.literals.Add("--", 80);
            base.literals.Add("<", 0x5b);
            base.literals.Add("false", 15);
            base.literals.Add("this", 40);
            base.literals.Add("static", 0x26);
            base.literals.Add(">=", 0x60);
            base.literals.Add("<=", 0x5c);
            base.literals.Add("partial", 0x23);
            base.literals.Add(";", 0x4d);
            base.literals.Add("get", 20);
            base.literals.Add("<<=", 0x5e);
            base.literals.Add("continue", 9);
            base.literals.Add("&&", 0x4c);
            base.literals.Add("cast", 6);
            base.literals.Add("<<", 0x5d);
            base.literals.Add("pragma on", 0x39);
            base.literals.Add(".", 0x41);
            base.literals.Add("finally", 0x11);
            base.literals.Add("else", 11);
            base.literals.Add("import", 0x16);
            base.literals.Add("++", 0x4f);
            base.literals.Add(":", 0x42);
            base.literals.Add("in", 0x18);
            base.literals.Add("switch", 0x31);
            base.literals.Add("true", 0x2a);
            base.literals.Add("-", 0x52);
            base.literals.Add("*=", 0x37);
            base.literals.Add("virtual", 0x2e);
            base.literals.Add("^", 0x49);
            base.literals.Add("&=", 0x4a);
        }

        public static bool IsDigit(char ch)
        {
            bool flag1 = ch >= '0';
            if (!flag1)
            {
                return flag1;
            }
            return (ch <= '9');
        }

        public void mADD(bool _createToken)
        {
            int num = new int();
            IToken token = null;
            int length = base.text.Length;
            num = 0x51;
            this.match("+");
            if ((_createToken && (token == null)) && (num != Token.SKIP))
            {
                token = this.makeToken(num);
                token.setText(base.text.ToString(length, base.text.Length - length));
            }
            base.returnToken_ = token;
        }

        public void mASSIGN(bool _createToken)
        {
            int num = new int();
            IToken token = null;
            int length = base.text.Length;
            num = 0x4e;
            this.match("=");
            if ((_createToken && (token == null)) && (num != Token.SKIP))
            {
                token = this.makeToken(num);
                token.setText(base.text.ToString(length, base.text.Length - length));
            }
            base.returnToken_ = token;
        }

        public void mAT(bool _createToken)
        {
            int num = new int();
            IToken token = null;
            int length = base.text.Length;
            num = 0x63;
            this.match("@");
            if ((_createToken && (token == null)) && (num != Token.SKIP))
            {
                token = this.makeToken(num);
                token.setText(base.text.ToString(length, base.text.Length - length));
            }
            base.returnToken_ = token;
        }

        public void mBITWISE_AND(bool _createToken)
        {
            int num = new int();
            IToken token = null;
            int length = base.text.Length;
            num = 0x48;
            this.match("&");
            if ((_createToken && (token == null)) && (num != Token.SKIP))
            {
                token = this.makeToken(num);
                token.setText(base.text.ToString(length, base.text.Length - length));
            }
            base.returnToken_ = token;
        }

        public void mBITWISE_NOT(bool _createToken)
        {
            int num = new int();
            IToken token = null;
            int length = base.text.Length;
            num = 0x58;
            this.match("~");
            if ((_createToken && (token == null)) && (num != Token.SKIP))
            {
                token = this.makeToken(num);
                token.setText(base.text.ToString(length, base.text.Length - length));
            }
            base.returnToken_ = token;
        }

        public void mBITWISE_OR(bool _createToken)
        {
            int num = new int();
            IToken token = null;
            int length = base.text.Length;
            num = 70;
            this.match("|");
            if ((_createToken && (token == null)) && (num != Token.SKIP))
            {
                token = this.makeToken(num);
                token.setText(base.text.ToString(length, base.text.Length - length));
            }
            base.returnToken_ = token;
        }

        public void mBITWISE_XOR(bool _createToken)
        {
            int num = new int();
            IToken token = null;
            int length = base.text.Length;
            num = 0x49;
            this.match("^");
            if ((_createToken && (token == null)) && (num != Token.SKIP))
            {
                token = this.makeToken(num);
                token.setText(base.text.ToString(length, base.text.Length - length));
            }
            base.returnToken_ = token;
        }

        public void mCOLON(bool _createToken)
        {
            int num = new int();
            IToken token = null;
            int length = base.text.Length;
            num = 0x42;
            this.match(":");
            if ((_createToken && (token == null)) && (num != Token.SKIP))
            {
                token = this.makeToken(num);
                token.setText(base.text.ToString(length, base.text.Length - length));
            }
            base.returnToken_ = token;
        }

        public void mCOMMA(bool _createToken)
        {
            int num = new int();
            IToken token = null;
            int length = base.text.Length;
            num = 0x43;
            this.match(",");
            if ((_createToken && (token == null)) && (num != Token.SKIP))
            {
                token = this.makeToken(num);
                token.setText(base.text.ToString(length, base.text.Length - length));
            }
            base.returnToken_ = token;
        }

        public void mDECREMENT(bool _createToken)
        {
            int num = new int();
            IToken token = null;
            int length = base.text.Length;
            num = 80;
            this.match("--");
            if ((_createToken && (token == null)) && (num != Token.SKIP))
            {
                token = this.makeToken(num);
                token.setText(base.text.ToString(length, base.text.Length - length));
            }
            base.returnToken_ = token;
        }

        protected void mDIGIT(bool _createToken)
        {
            int num = new int();
            IToken token = null;
            int length = base.text.Length;
            num = 0x7a;
            this.matchRange('0', '9');
            if ((_createToken && (token == null)) && (num != Token.SKIP))
            {
                token = this.makeToken(num);
                token.setText(base.text.ToString(length, base.text.Length - length));
            }
            base.returnToken_ = token;
        }

        public void mDIVISION(bool _createToken)
        {
            int sKIP = new int();
            IToken token = null;
            int length = base.text.Length;
            sKIP = 0x68;
            bool flag = false;
            if ((base.cached_LA1 == '/') && (base.cached_LA2 == '*'))
            {
                int num3 = this.mark();
                flag = true;
                base.inputState.guessing++;
                try
                {
                    this.match("/*");
                }
                catch (RecognitionException)
                {
                    flag = false;
                }
                this.rewind(num3);
                base.inputState.guessing--;
            }
            if (flag)
            {
                this.mML_COMMENT(false);
                if ((base.inputState.guessing == 0) && !this.PreserveComments)
                {
                    sKIP = Token.SKIP;
                }
            }
            else
            {
                if ((base.cached_LA1 != '/') || (1 == 0))
                {
                    throw new NoViableAltForCharException(base.cached_LA1, this.getFilename(), this.getLine(), this.getColumn());
                }
                this.match("/");
                switch (base.cached_LA1)
                {
                    case '/':
                        this.match("/");
                        while (tokenSet_1_.member((int) base.cached_LA1))
                        {
                            this.match(tokenSet_1_);
                        }
                        if (base.inputState.guessing == 0)
                        {
                            if (this.PreserveComments)
                            {
                                sKIP = 0x38;
                            }
                            else
                            {
                                sKIP = Token.SKIP;
                            }
                        }
                        break;

                    case '=':
                        this.match("=");
                        if (base.inputState.guessing == 0)
                        {
                            sKIP = 0x34;
                        }
                        break;
                }
            }
            if ((_createToken && (token == null)) && (sKIP != Token.SKIP))
            {
                token = this.makeToken(sKIP);
                token.setText(base.text.ToString(length, base.text.Length - length));
            }
            base.returnToken_ = token;
        }

        public void mDOT(bool _createToken)
        {
            int num = new int();
            IToken token = null;
            int length = base.text.Length;
            num = 0x41;
            this.match(".");
            if ((base.cached_LA1 >= '0') && (base.cached_LA1 <= '9'))
            {
                this.mDOUBLE_SUFFIX(false);
                if (base.inputState.guessing == 0)
                {
                    num = 0x6a;
                }
            }
            if ((_createToken && (token == null)) && (num != Token.SKIP))
            {
                token = this.makeToken(num);
                token.setText(base.text.ToString(length, base.text.Length - length));
            }
            base.returnToken_ = token;
        }

        public void mDOUBLE_QUOTED_STRING(bool _createToken)
        {
            int num = new int();
            IToken token = null;
            int length = base.text.Length;
            num = 60;
            int num3 = base.text.Length;
            this.match("\"");
            base.text.Length = num3;
        Label_0041:
            while (base.cached_LA1 == '\\')
            {
                this.mDQS_ESC(false);
            }
            if (tokenSet_3_.member((int) base.cached_LA1))
            {
                this.match(tokenSet_3_);
                goto Label_0041;
            }
            num3 = base.text.Length;
            this.match("\"");
            base.text.Length = num3;
            if ((_createToken && (token == null)) && (num != Token.SKIP))
            {
                token = this.makeToken(num);
                token.setText(base.text.ToString(length, base.text.Length - length));
            }
            base.returnToken_ = token;
        }

        protected void mDOUBLE_SUFFIX(bool _createToken)
        {
            int num = new int();
            IToken token = null;
            int length = base.text.Length;
            num = 110;
            int num3 = 0;
            while (true)
            {
                if ((base.cached_LA1 >= '0') && (base.cached_LA1 <= '9'))
                {
                    this.mDIGIT(false);
                }
                else
                {
                    if (num3 < 1)
                    {
                        throw new NoViableAltForCharException(base.cached_LA1, this.getFilename(), this.getLine(), this.getColumn());
                    }
                    break;
                }
                num3++;
            }
            if ((base.cached_LA1 == 'E') || (base.cached_LA1 == 'e'))
            {
                this.mEXPONENT(false);
            }
            switch (base.cached_LA1)
            {
                case 'f':
                    this.match("f");
                    break;

                case 'F':
                    this.match("F");
                    break;

                case 'd':
                    this.match("d");
                    break;

                case 'D':
                    this.match("D");
                    break;
            }
            if ((_createToken && (token == null)) && (num != Token.SKIP))
            {
                token = this.makeToken(num);
                token.setText(base.text.ToString(length, base.text.Length - length));
            }
            base.returnToken_ = token;
        }

        protected void mDQS_ESC(bool _createToken)
        {
            int num = new int();
            IToken token = null;
            int length = base.text.Length;
            num = 0x72;
            int num3 = base.text.Length;
            this.match(@"\");
            base.text.Length = num3;
            char ch = base.cached_LA1;
            switch (ch)
            {
                case '0':
                case '\\':
                case 'a':
                case 'b':
                case 'f':
                case 'n':
                case 'r':
                case 't':
                    this.mSESC(false);
                    break;

                default:
                    if (ch != '"')
                    {
                        throw new NoViableAltForCharException(base.cached_LA1, this.getFilename(), this.getLine(), this.getColumn());
                    }
                    this.match("\"");
                    break;
            }
            if ((_createToken && (token == null)) && (num != Token.SKIP))
            {
                token = this.makeToken(num);
                token.setText(base.text.ToString(length, base.text.Length - length));
            }
            base.returnToken_ = token;
        }

        public void mEOS(bool _createToken)
        {
            int num = new int();
            IToken token = null;
            int length = base.text.Length;
            num = 0x4d;
            this.match(";");
            if ((_createToken && (token == null)) && (num != Token.SKIP))
            {
                token = this.makeToken(num);
                token.setText(base.text.ToString(length, base.text.Length - length));
            }
            base.returnToken_ = token;
        }

        public void mEQUALITY(bool _createToken)
        {
            int num = new int();
            IToken token = null;
            int length = base.text.Length;
            num = 0x55;
            this.match("==");
            if ((_createToken && (token == null)) && (num != Token.SKIP))
            {
                token = this.makeToken(num);
                token.setText(base.text.ToString(length, base.text.Length - length));
            }
            base.returnToken_ = token;
        }

        protected void mEXPONENT(bool _createToken)
        {
            int num = new int();
            IToken token = null;
            int length = base.text.Length;
            num = 0x6f;
            switch (base.cached_LA1)
            {
                case 'e':
                    this.match("e");
                    break;

                case 'E':
                    this.match("E");
                    break;

                default:
                    throw new NoViableAltForCharException(base.cached_LA1, this.getFilename(), this.getLine(), this.getColumn());
            }
            switch (base.cached_LA1)
            {
                case '+':
                    this.match("+");
                    break;

                case '-':
                    this.match("-");
                    break;

                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    break;

                default:
                    throw new NoViableAltForCharException(base.cached_LA1, this.getFilename(), this.getLine(), this.getColumn());
            }
            int num3 = 0;
            while (true)
            {
                if ((base.cached_LA1 >= '0') && (base.cached_LA1 <= '9'))
                {
                    this.mDIGIT(false);
                }
                else
                {
                    if (num3 < 1)
                    {
                        throw new NoViableAltForCharException(base.cached_LA1, this.getFilename(), this.getLine(), this.getColumn());
                    }
                    break;
                }
                num3++;
            }
            if ((_createToken && (token == null)) && (num != Token.SKIP))
            {
                token = this.makeToken(num);
                token.setText(base.text.ToString(length, base.text.Length - length));
            }
            base.returnToken_ = token;
        }

        public void mGREATER_THAN(bool _createToken)
        {
            int num = new int();
            IToken token = null;
            int length = base.text.Length;
            num = 0x5f;
            this.match(">");
            if ((_createToken && (token == null)) && (num != Token.SKIP))
            {
                token = this.makeToken(num);
                token.setText(base.text.ToString(length, base.text.Length - length));
            }
            base.returnToken_ = token;
        }

        public void mGREATER_THAN_OR_EQUAL(bool _createToken)
        {
            int num = new int();
            IToken token = null;
            int length = base.text.Length;
            num = 0x60;
            this.match(">=");
            if ((_createToken && (token == null)) && (num != Token.SKIP))
            {
                token = this.makeToken(num);
                token.setText(base.text.ToString(length, base.text.Length - length));
            }
            base.returnToken_ = token;
        }

        protected void mHEXDIGIT(bool _createToken)
        {
            int num = new int();
            IToken token = null;
            int length = base.text.Length;
            num = 0x7b;
            char ch = base.cached_LA1;
            switch (ch)
            {
                case 'a':
                case 'b':
                case 'c':
                case 'd':
                case 'e':
                case 'f':
                    this.matchRange('a', 'f');
                    break;

                case 'A':
                case 'B':
                case 'C':
                case 'D':
                case 'E':
                case 'F':
                    this.matchRange('A', 'F');
                    break;

                default:
                    if ((((ch != '0') && (ch != '1')) && ((ch != '2') && (ch != '3'))) && ((((ch != '4') && (ch != '5')) && ((ch != '6') && (ch != '7'))) && ((ch != '8') && (ch != '9'))))
                    {
                        throw new NoViableAltForCharException(base.cached_LA1, this.getFilename(), this.getLine(), this.getColumn());
                    }
                    this.matchRange('0', '9');
                    break;
            }
            if ((_createToken && (token == null)) && (num != Token.SKIP))
            {
                token = this.makeToken(num);
                token.setText(base.text.ToString(length, base.text.Length - length));
            }
            base.returnToken_ = token;
        }

        public void mID(bool _createToken)
        {
            int num = new int();
            IToken token = null;
            int length = base.text.Length;
            num = 0x3b;
            this.mID_LETTER(false);
        Label_0025:
            switch (base.cached_LA1)
            {
                case 'A':
                case 'B':
                case 'C':
                case 'D':
                case 'E':
                case 'F':
                case 'G':
                case 'H':
                case 'I':
                case 'J':
                case 'K':
                case 'L':
                case 'M':
                case 'N':
                case 'O':
                case 'P':
                case 'Q':
                case 'R':
                case 'S':
                case 'T':
                case 'U':
                case 'V':
                case 'W':
                case 'X':
                case 'Y':
                case 'Z':
                case '_':
                case 'a':
                case 'b':
                case 'c':
                case 'd':
                case 'e':
                case 'f':
                case 'g':
                case 'h':
                case 'i':
                case 'j':
                case 'k':
                case 'l':
                case 'm':
                case 'n':
                case 'o':
                case 'p':
                case 'q':
                case 'r':
                case 's':
                case 't':
                case 'u':
                case 'v':
                case 'w':
                case 'x':
                case 'y':
                case 'z':
                    this.mID_LETTER(false);
                    goto Label_0025;

                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    this.mDIGIT(false);
                    goto Label_0025;
            }
            num = this.testLiteralsTable(num);
            if ((_createToken && (token == null)) && (num != Token.SKIP))
            {
                token = this.makeToken(num);
                token.setText(base.text.ToString(length, base.text.Length - length));
            }
            base.returnToken_ = token;
        }

        protected void mID_LETTER(bool _createToken)
        {
            int num = new int();
            IToken token = null;
            int length = base.text.Length;
            num = 0x79;
            char ch = base.cached_LA1;
            switch (ch)
            {
                case '_':
                    this.match("_");
                    break;

                case 'a':
                case 'b':
                case 'c':
                case 'd':
                case 'e':
                case 'f':
                case 'g':
                case 'h':
                case 'i':
                case 'j':
                case 'k':
                case 'l':
                case 'm':
                case 'n':
                case 'o':
                case 'p':
                case 'q':
                case 'r':
                case 's':
                case 't':
                case 'u':
                case 'v':
                case 'w':
                case 'x':
                case 'y':
                case 'z':
                    this.matchRange('a', 'z');
                    break;

                default:
                    if ((((((ch != 'A') && (ch != 'B')) && ((ch != 'C') && (ch != 'D'))) && (((ch != 'E') && (ch != 'F')) && ((ch != 'G') && (ch != 'H')))) && ((((ch != 'I') && (ch != 'J')) && ((ch != 'K') && (ch != 'L'))) && (((ch != 'M') && (ch != 'N')) && ((ch != 'O') && (ch != 'P'))))) && ((((ch != 'Q') && (ch != 'R')) && ((ch != 'S') && (ch != 'T'))) && ((((ch != 'U') && (ch != 'V')) && ((ch != 'W') && (ch != 'X'))) && ((ch != 'Y') && (ch != 'Z')))))
                    {
                        throw new NoViableAltForCharException(base.cached_LA1, this.getFilename(), this.getLine(), this.getColumn());
                    }
                    this.matchRange('A', 'Z');
                    break;
            }
            if ((_createToken && (token == null)) && (num != Token.SKIP))
            {
                token = this.makeToken(num);
                token.setText(base.text.ToString(length, base.text.Length - length));
            }
            base.returnToken_ = token;
        }

        public void mINCREMENT(bool _createToken)
        {
            int num = new int();
            IToken token = null;
            int length = base.text.Length;
            num = 0x4f;
            this.match("++");
            if ((_createToken && (token == null)) && (num != Token.SKIP))
            {
                token = this.makeToken(num);
                token.setText(base.text.ToString(length, base.text.Length - length));
            }
            base.returnToken_ = token;
        }

        public void mINEQUALITY(bool _createToken)
        {
            int num = new int();
            IToken token = null;
            int length = base.text.Length;
            num = 0x56;
            this.match("!=");
            if ((_createToken && (token == null)) && (num != Token.SKIP))
            {
                token = this.makeToken(num);
                token.setText(base.text.ToString(length, base.text.Length - length));
            }
            base.returnToken_ = token;
        }

        public void mINPLACE_ADD(bool _createToken)
        {
            int num = new int();
            IToken token = null;
            int length = base.text.Length;
            num = 0x35;
            this.match("+=");
            if ((_createToken && (token == null)) && (num != Token.SKIP))
            {
                token = this.makeToken(num);
                token.setText(base.text.ToString(length, base.text.Length - length));
            }
            base.returnToken_ = token;
        }

        public void mINPLACE_BITWISE_AND(bool _createToken)
        {
            int num = new int();
            IToken token = null;
            int length = base.text.Length;
            num = 0x4a;
            this.match("&=");
            if ((_createToken && (token == null)) && (num != Token.SKIP))
            {
                token = this.makeToken(num);
                token.setText(base.text.ToString(length, base.text.Length - length));
            }
            base.returnToken_ = token;
        }

        public void mINPLACE_BITWISE_OR(bool _createToken)
        {
            int num = new int();
            IToken token = null;
            int length = base.text.Length;
            num = 0x47;
            this.match("|=");
            if ((_createToken && (token == null)) && (num != Token.SKIP))
            {
                token = this.makeToken(num);
                token.setText(base.text.ToString(length, base.text.Length - length));
            }
            base.returnToken_ = token;
        }

        public void mINPLACE_BITWISE_XOR(bool _createToken)
        {
            int num = new int();
            IToken token = null;
            int length = base.text.Length;
            num = 0x66;
            this.match("^=");
            if ((_createToken && (token == null)) && (num != Token.SKIP))
            {
                token = this.makeToken(num);
                token.setText(base.text.ToString(length, base.text.Length - length));
            }
            base.returnToken_ = token;
        }

        public void mINPLACE_MULTIPLY(bool _createToken)
        {
            int num = new int();
            IToken token = null;
            int length = base.text.Length;
            num = 0x37;
            this.match("*=");
            if ((_createToken && (token == null)) && (num != Token.SKIP))
            {
                token = this.makeToken(num);
                token.setText(base.text.ToString(length, base.text.Length - length));
            }
            base.returnToken_ = token;
        }

        public void mINPLACE_SHIFT_LEFT(bool _createToken)
        {
            int num = new int();
            IToken token = null;
            int length = base.text.Length;
            num = 0x5e;
            this.match("<<=");
            if ((_createToken && (token == null)) && (num != Token.SKIP))
            {
                token = this.makeToken(num);
                token.setText(base.text.ToString(length, base.text.Length - length));
            }
            base.returnToken_ = token;
        }

        public void mINPLACE_SHIFT_RIGHT(bool _createToken)
        {
            int num = new int();
            IToken token = null;
            int length = base.text.Length;
            num = 0x62;
            this.match(">>=");
            if ((_createToken && (token == null)) && (num != Token.SKIP))
            {
                token = this.makeToken(num);
                token.setText(base.text.ToString(length, base.text.Length - length));
            }
            base.returnToken_ = token;
        }

        public void mINPLACE_SUBTRACT(bool _createToken)
        {
            int num = new int();
            IToken token = null;
            int length = base.text.Length;
            num = 0x36;
            this.match("-=");
            if ((_createToken && (token == null)) && (num != Token.SKIP))
            {
                token = this.makeToken(num);
                token.setText(base.text.ToString(length, base.text.Length - length));
            }
            base.returnToken_ = token;
        }

        public void mINT(bool _createToken)
        {
            int num = new int();
            IToken token = null;
            int length = base.text.Length;
            num = 0x6b;
            if ((base.cached_LA1 != '0') || (base.cached_LA2 != 'x'))
            {
                if (((base.cached_LA1 < '0') || (base.cached_LA1 > '9')) || (1 == 0))
                {
                    throw new NoViableAltForCharException(base.cached_LA1, this.getFilename(), this.getLine(), this.getColumn());
                }
                int num4 = 0;
                while (true)
                {
                    if ((base.cached_LA1 >= '0') && (base.cached_LA1 <= '9'))
                    {
                        this.mDIGIT(false);
                    }
                    else
                    {
                        if (num4 < 1)
                        {
                            throw new NoViableAltForCharException(base.cached_LA1, this.getFilename(), this.getLine(), this.getColumn());
                        }
                        char ch = base.cached_LA1;
                        if ((ch != 'L') && (ch != 'l'))
                        {
                            if (((ch == 'D') || (ch == 'F')) || ((ch == 'd') || (ch == 'f')))
                            {
                                switch (base.cached_LA1)
                                {
                                    case 'f':
                                        this.match("f");
                                        goto Label_02EF;

                                    case 'F':
                                        this.match("F");
                                        goto Label_02EF;

                                    case 'd':
                                        this.match("d");
                                        goto Label_02EF;

                                    case 'D':
                                        this.match("D");
                                        goto Label_02EF;
                                }
                                throw new NoViableAltForCharException(base.cached_LA1, this.getFilename(), this.getLine(), this.getColumn());
                            }
                            switch (ch)
                            {
                                case '.':
                                    this.match(".");
                                    this.mDOUBLE_SUFFIX(false);
                                    if (base.inputState.guessing == 0)
                                    {
                                        num = 0x6a;
                                    }
                                    break;

                                case 'E':
                                case 'e':
                                    this.mEXPONENT(false);
                                    if (base.inputState.guessing == 0)
                                    {
                                        num = 0x6a;
                                    }
                                    break;
                            }
                        }
                        else
                        {
                            switch (base.cached_LA1)
                            {
                                case 'l':
                                    this.match("l");
                                    break;

                                case 'L':
                                    this.match("L");
                                    break;

                                default:
                                    throw new NoViableAltForCharException(base.cached_LA1, this.getFilename(), this.getLine(), this.getColumn());
                            }
                            if (base.inputState.guessing == 0)
                            {
                                num = 0x6c;
                            }
                        }
                        goto Label_038E;
                    }
                    num4++;
                }
            }
            this.match("0x");
            int num3 = 0;
            while (true)
            {
                if (tokenSet_0_.member((int) base.cached_LA1))
                {
                    this.mHEXDIGIT(false);
                }
                else
                {
                    if (num3 < 1)
                    {
                        throw new NoViableAltForCharException(base.cached_LA1, this.getFilename(), this.getLine(), this.getColumn());
                    }
                    break;
                }
                num3++;
            }
            if ((base.cached_LA1 == 'L') || (base.cached_LA1 == 'l'))
            {
                switch (base.cached_LA1)
                {
                    case 'l':
                        this.match("l");
                        break;

                    case 'L':
                        this.match("L");
                        break;

                    default:
                        throw new NoViableAltForCharException(base.cached_LA1, this.getFilename(), this.getLine(), this.getColumn());
                }
                if (base.inputState.guessing == 0)
                {
                    num = 0x6c;
                }
            }
            goto Label_038E;
        Label_02EF:
            if (base.inputState.guessing == 0)
            {
                num = 0x6a;
            }
        Label_038E:
            if ((_createToken && (token == null)) && (num != Token.SKIP))
            {
                token = this.makeToken(num);
                token.setText(base.text.ToString(length, base.text.Length - length));
            }
            base.returnToken_ = token;
        }

        private static long[] mk_tokenSet_0_()
        {
            long[] array = new long[0x401];
            array[0] = 0x3ff000000000000L;
            array[1] = 0x7e0000007eL;
            for (int i = 2; i <= 0x400; i++)
            {
                array[RuntimeServices.NormalizeArrayIndex(array, i)] = 0L;
            }
            return array;
        }

        private static long[] mk_tokenSet_1_()
        {
            int num;
            long[] array = new long[0x800];
            array[0] = -9224L;
            for (num = 1; num <= 0x3fe; num++)
            {
                array[RuntimeServices.NormalizeArrayIndex(array, num)] = -1L;
            }
            array[0x3ff] = 0x7fffffffffffffffL;
            for (num = 0x400; num <= 0x7ff; num++)
            {
                array[RuntimeServices.NormalizeArrayIndex(array, num)] = 0L;
            }
            return array;
        }

        private static long[] mk_tokenSet_2_()
        {
            int num;
            long[] array = new long[0x800];
            array[0] = -4398046520328L;
            for (num = 1; num <= 0x3fe; num++)
            {
                array[RuntimeServices.NormalizeArrayIndex(array, num)] = -1L;
            }
            array[0x3ff] = 0x7fffffffffffffffL;
            for (num = 0x400; num <= 0x7ff; num++)
            {
                array[RuntimeServices.NormalizeArrayIndex(array, num)] = 0L;
            }
            return array;
        }

        private static long[] mk_tokenSet_3_()
        {
            int num;
            long[] array = new long[0x800];
            array[0] = -17179878408L;
            array[1] = -268435457L;
            for (num = 2; num <= 0x3fe; num++)
            {
                array[RuntimeServices.NormalizeArrayIndex(array, num)] = -1L;
            }
            array[0x3ff] = 0x7fffffffffffffffL;
            for (num = 0x400; num <= 0x7ff; num++)
            {
                array[RuntimeServices.NormalizeArrayIndex(array, num)] = 0L;
            }
            return array;
        }

        private static long[] mk_tokenSet_4_()
        {
            int num;
            long[] array = new long[0x800];
            array[0] = -549755823112L;
            array[1] = -268435457L;
            for (num = 2; num <= 0x3fe; num++)
            {
                array[RuntimeServices.NormalizeArrayIndex(array, num)] = -1L;
            }
            array[0x3ff] = 0x7fffffffffffffffL;
            for (num = 0x400; num <= 0x7ff; num++)
            {
                array[RuntimeServices.NormalizeArrayIndex(array, num)] = 0L;
            }
            return array;
        }

        private static long[] mk_tokenSet_5_()
        {
            int num;
            long[] array = new long[0x800];
            array[0] = -140741783332360L;
            for (num = 1; num <= 0x3fe; num++)
            {
                array[RuntimeServices.NormalizeArrayIndex(array, num)] = -1L;
            }
            array[0x3ff] = 0x7fffffffffffffffL;
            for (num = 0x400; num <= 0x7ff; num++)
            {
                array[RuntimeServices.NormalizeArrayIndex(array, num)] = 0L;
            }
            return array;
        }

        private static long[] mk_tokenSet_6_()
        {
            int num;
            long[] array = new long[0x800];
            array[0] = -140741783332360L;
            array[1] = -268435457L;
            for (num = 2; num <= 0x3fe; num++)
            {
                array[RuntimeServices.NormalizeArrayIndex(array, num)] = -1L;
            }
            array[0x3ff] = 0x7fffffffffffffffL;
            for (num = 0x400; num <= 0x7ff; num++)
            {
                array[RuntimeServices.NormalizeArrayIndex(array, num)] = 0L;
            }
            return array;
        }

        private static long[] mk_tokenSet_7_()
        {
            int num;
            long[] array = new long[0x800];
            array[0] = -4294977032L;
            for (num = 1; num <= 0x3fe; num++)
            {
                array[RuntimeServices.NormalizeArrayIndex(array, num)] = -1L;
            }
            array[0x3ff] = 0x7fffffffffffffffL;
            for (num = 0x400; num <= 0x7ff; num++)
            {
                array[RuntimeServices.NormalizeArrayIndex(array, num)] = 0L;
            }
            return array;
        }

        public void mLBRACE(bool _createToken)
        {
            int num = new int();
            IToken token = null;
            int length = base.text.Length;
            num = 0x3d;
            this.match("{");
            if ((_createToken && (token == null)) && (num != Token.SKIP))
            {
                token = this.makeToken(num);
                token.setText(base.text.ToString(length, base.text.Length - length));
            }
            base.returnToken_ = token;
        }

        public void mLBRACK(bool _createToken)
        {
            int num = new int();
            IToken token = null;
            int length = base.text.Length;
            num = 0x44;
            this.match("[");
            if ((_createToken && (token == null)) && (num != Token.SKIP))
            {
                token = this.makeToken(num);
                token.setText(base.text.ToString(length, base.text.Length - length));
            }
            base.returnToken_ = token;
        }

        public void mLESS_THAN(bool _createToken)
        {
            int num = new int();
            IToken token = null;
            int length = base.text.Length;
            num = 0x5b;
            this.match("<");
            if ((_createToken && (token == null)) && (num != Token.SKIP))
            {
                token = this.makeToken(num);
                token.setText(base.text.ToString(length, base.text.Length - length));
            }
            base.returnToken_ = token;
        }

        public void mLESS_THAN_OR_EQUAL(bool _createToken)
        {
            int num = new int();
            IToken token = null;
            int length = base.text.Length;
            num = 0x5c;
            this.match("<=");
            if ((_createToken && (token == null)) && (num != Token.SKIP))
            {
                token = this.makeToken(num);
                token.setText(base.text.ToString(length, base.text.Length - length));
            }
            base.returnToken_ = token;
        }

        public void mLOGICAL_AND(bool _createToken)
        {
            int num = new int();
            IToken token = null;
            int length = base.text.Length;
            num = 0x4c;
            this.match("&&");
            if ((_createToken && (token == null)) && (num != Token.SKIP))
            {
                token = this.makeToken(num);
                token.setText(base.text.ToString(length, base.text.Length - length));
            }
            base.returnToken_ = token;
        }

        public void mLOGICAL_NOT(bool _createToken)
        {
            int num = new int();
            IToken token = null;
            int length = base.text.Length;
            num = 0x67;
            this.match("!");
            if ((_createToken && (token == null)) && (num != Token.SKIP))
            {
                token = this.makeToken(num);
                token.setText(base.text.ToString(length, base.text.Length - length));
            }
            base.returnToken_ = token;
        }

        public void mLOGICAL_OR(bool _createToken)
        {
            int num = new int();
            IToken token = null;
            int length = base.text.Length;
            num = 0x4b;
            this.match("||");
            if ((_createToken && (token == null)) && (num != Token.SKIP))
            {
                token = this.makeToken(num);
                token.setText(base.text.ToString(length, base.text.Length - length));
            }
            base.returnToken_ = token;
        }

        public void mLPAREN(bool _createToken)
        {
            int num = new int();
            IToken token = null;
            int length = base.text.Length;
            num = 0x3f;
            this.match("(");
            if ((_createToken && (token == null)) && (num != Token.SKIP))
            {
                token = this.makeToken(num);
                token.setText(base.text.ToString(length, base.text.Length - length));
            }
            base.returnToken_ = token;
        }

        protected void mML_COMMENT(bool _createToken)
        {
            int num = new int();
            IToken token = null;
            int length = base.text.Length;
            num = 0x75;
            this.match("/*");
        Label_0029:
            while ((((base.cached_LA1 == '*') && (base.cached_LA2 >= '\x0003')) && ((base.cached_LA2 <= 0xfffe) && (this.LA(3) >= '\x0003'))) && ((this.LA(3) <= 0xfffe) && (this.LA(2) != '/')))
            {
                this.match("*");
            }
            if ((base.cached_LA1 == '\n') || (base.cached_LA1 == '\r'))
            {
                this.mNEWLINE(false);
                goto Label_0029;
            }
            if (tokenSet_2_.member((int) base.cached_LA1))
            {
                this.match(tokenSet_2_);
                goto Label_0029;
            }
            this.match("*/");
            if ((_createToken && (token == null)) && (num != Token.SKIP))
            {
                token = this.makeToken(num);
                token.setText(base.text.ToString(length, base.text.Length - length));
            }
            base.returnToken_ = token;
        }

        public void mMODULUS(bool _createToken)
        {
            int num = new int();
            IToken token = null;
            int length = base.text.Length;
            num = 0x53;
            this.match("%");
            if ((_createToken && (token == null)) && (num != Token.SKIP))
            {
                token = this.makeToken(num);
                token.setText(base.text.ToString(length, base.text.Length - length));
            }
            base.returnToken_ = token;
        }

        public void mMULTIPLY(bool _createToken)
        {
            int num = new int();
            IToken token = null;
            int length = base.text.Length;
            num = 0x54;
            this.match("*");
            if ((_createToken && (token == null)) && (num != Token.SKIP))
            {
                token = this.makeToken(num);
                token.setText(base.text.ToString(length, base.text.Length - length));
            }
            base.returnToken_ = token;
        }

        protected void mNEWLINE(bool _createToken)
        {
            int num = new int();
            IToken token = null;
            int length = base.text.Length;
            num = 120;
            char ch = base.cached_LA1;
            if (ch == '\n')
            {
                this.match("\n");
            }
            else
            {
                if (ch != '\r')
                {
                    throw new NoViableAltForCharException(base.cached_LA1, this.getFilename(), this.getLine(), this.getColumn());
                }
                this.match("\r");
                if (((base.cached_LA1 == '\n') && (1 != 0)) && (1 != 0))
                {
                    this.match("\n");
                }
            }
            if (base.inputState.guessing == 0)
            {
                this.newline();
            }
            if ((_createToken && (token == null)) && (num != Token.SKIP))
            {
                token = this.makeToken(num);
                token.setText(base.text.ToString(length, base.text.Length - length));
            }
            base.returnToken_ = token;
        }

        public void mPRAGMA_ON(bool _createToken)
        {
            int num = new int();
            IToken token = null;
            int length = base.text.Length;
            num = 0x39;
            IToken token2 = null;
            int num3 = base.text.Length;
            this.match("#pragma");
            base.text.Length = num3;
            int num4 = 0;
            while (true)
            {
                if ((base.cached_LA1 == '\t') || (base.cached_LA1 == ' '))
                {
                    this.mPRAGMA_WHITE_SPACE(false);
                }
                else
                {
                    if (num4 < 1)
                    {
                        throw new NoViableAltForCharException(base.cached_LA1, this.getFilename(), this.getLine(), this.getColumn());
                    }
                    break;
                }
                num4++;
            }
            this.mID(true);
            token2 = base.returnToken_;
            if ((((base.cached_LA1 != '\t') && (base.cached_LA1 != ' ')) || (((base.cached_LA2 != '\t') && (base.cached_LA2 != ' ')) && (base.cached_LA2 != 'o'))) || (((this.LA(3) != '\t') && (this.LA(3) != ' ')) && (((this.LA(3) != 'f') && (this.LA(3) != 'n')) && (this.LA(3) != 'o'))))
            {
                if ((((base.cached_LA1 != '\t') && (base.cached_LA1 != '\n')) && ((base.cached_LA1 != '\r') && (base.cached_LA1 != ' '))) || ((1 == 0) || (1 == 0)))
                {
                    throw new NoViableAltForCharException(base.cached_LA1, this.getFilename(), this.getLine(), this.getColumn());
                }
            }
            else
            {
                int num5 = 0;
                while (true)
                {
                    if ((base.cached_LA1 == '\t') || (base.cached_LA1 == ' '))
                    {
                        this.mPRAGMA_WHITE_SPACE(false);
                    }
                    else
                    {
                        if (num5 < 1)
                        {
                            throw new NoViableAltForCharException(base.cached_LA1, this.getFilename(), this.getLine(), this.getColumn());
                        }
                        break;
                    }
                    num5++;
                }
                if ((base.cached_LA1 == 'o') && (base.cached_LA2 == 'f'))
                {
                    num3 = base.text.Length;
                    this.match("off");
                    base.text.Length = num3;
                    if (base.inputState.guessing == 0)
                    {
                        num = 0x3a;
                    }
                }
                else
                {
                    if ((base.cached_LA1 != 'o') || (base.cached_LA2 != 'n'))
                    {
                        throw new NoViableAltForCharException(base.cached_LA1, this.getFilename(), this.getLine(), this.getColumn());
                    }
                    num3 = base.text.Length;
                    this.match("on");
                    base.text.Length = num3;
                }
            }
            while ((base.cached_LA1 == '\t') || (base.cached_LA1 == ' '))
            {
                this.mPRAGMA_WHITE_SPACE(false);
            }
            num3 = base.text.Length;
            this.mNEWLINE(false);
            base.text.Length = num3;
            if ((_createToken && (token == null)) && (num != Token.SKIP))
            {
                token = this.makeToken(num);
                token.setText(base.text.ToString(length, base.text.Length - length));
            }
            base.returnToken_ = token;
        }

        protected void mPRAGMA_WHITE_SPACE(bool _createToken)
        {
            int num3;
            int num = new int();
            IToken token = null;
            int length = base.text.Length;
            num = 0x70;
            switch (base.cached_LA1)
            {
                case ' ':
                    num3 = base.text.Length;
                    this.match(" ");
                    base.text.Length = num3;
                    break;

                case '\t':
                    num3 = base.text.Length;
                    this.match("\t");
                    base.text.Length = num3;
                    break;

                default:
                    throw new NoViableAltForCharException(base.cached_LA1, this.getFilename(), this.getLine(), this.getColumn());
            }
            if ((_createToken && (token == null)) && (num != Token.SKIP))
            {
                token = this.makeToken(num);
                token.setText(base.text.ToString(length, base.text.Length - length));
            }
            base.returnToken_ = token;
        }

        public void mQUESTION_MARK(bool _createToken)
        {
            int num = new int();
            IToken token = null;
            int length = base.text.Length;
            num = 0x57;
            this.match("?");
            if ((_createToken && (token == null)) && (num != Token.SKIP))
            {
                token = this.makeToken(num);
                token.setText(base.text.ToString(length, base.text.Length - length));
            }
            base.returnToken_ = token;
        }

        public void mRBRACE(bool _createToken)
        {
            int num = new int();
            IToken token = null;
            int length = base.text.Length;
            num = 0x3e;
            this.match("}");
            if ((_createToken && (token == null)) && (num != Token.SKIP))
            {
                token = this.makeToken(num);
                token.setText(base.text.ToString(length, base.text.Length - length));
            }
            base.returnToken_ = token;
        }

        public void mRBRACK(bool _createToken)
        {
            int num = new int();
            IToken token = null;
            int length = base.text.Length;
            num = 0x45;
            this.match("]");
            if ((_createToken && (token == null)) && (num != Token.SKIP))
            {
                token = this.makeToken(num);
                token.setText(base.text.ToString(length, base.text.Length - length));
            }
            base.returnToken_ = token;
        }

        protected void mRE_CHAR(bool _createToken)
        {
            int num = new int();
            IToken token = null;
            int length = base.text.Length;
            num = 0x76;
            if (base.cached_LA1 == '\\')
            {
                this.mRE_ESC(false);
            }
            else
            {
                if (!tokenSet_6_.member((int) base.cached_LA1))
                {
                    throw new NoViableAltForCharException(base.cached_LA1, this.getFilename(), this.getLine(), this.getColumn());
                }
                this.match(tokenSet_6_);
            }
            if ((_createToken && (token == null)) && (num != Token.SKIP))
            {
                token = this.makeToken(num);
                token.setText(base.text.ToString(length, base.text.Length - length));
            }
            base.returnToken_ = token;
        }

        protected void mRE_ESC(bool _createToken)
        {
            int num = new int();
            IToken token = null;
            int length = base.text.Length;
            num = 0x77;
            this.match(@"\");
            switch (base.cached_LA1)
            {
                case 'a':
                    this.match("a");
                    break;

                case 'b':
                    this.match("b");
                    break;

                case 'c':
                    this.match("c");
                    this.matchRange('A', 'Z');
                    break;

                case 't':
                    this.match("t");
                    break;

                case 'r':
                    this.match("r");
                    break;

                case 'v':
                    this.match("v");
                    break;

                case 'f':
                    this.match("f");
                    break;

                case 'n':
                    this.match("n");
                    break;

                case 'e':
                    this.match("e");
                    break;

                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                {
                    int num3 = 0;
                    while (true)
                    {
                        if (((base.cached_LA1 >= '0') && (base.cached_LA1 <= '9')) && (tokenSet_7_.member((int) base.cached_LA2) && (1 != 0)))
                        {
                            this.mDIGIT(false);
                        }
                        else
                        {
                            if (num3 < 1)
                            {
                                throw new NoViableAltForCharException(base.cached_LA1, this.getFilename(), this.getLine(), this.getColumn());
                            }
                            break;
                        }
                        num3++;
                    }
                }
                case 'x':
                    this.match("x");
                    this.mDIGIT(false);
                    this.mDIGIT(false);
                    break;

                case 'u':
                    this.match("u");
                    this.mDIGIT(false);
                    this.mDIGIT(false);
                    this.mDIGIT(false);
                    this.mDIGIT(false);
                    break;

                case '\\':
                    this.match(@"\");
                    break;

                case 'w':
                    this.match("w");
                    break;

                case 'W':
                    this.match("W");
                    break;

                case 's':
                    this.match("s");
                    break;

                case 'S':
                    this.match("S");
                    break;

                case 'd':
                    this.match("d");
                    break;

                case 'D':
                    this.match("D");
                    break;

                case 'p':
                    this.match("p");
                    break;

                case 'P':
                    this.match("P");
                    break;

                case 'A':
                    this.match("A");
                    break;

                case 'z':
                    this.match("z");
                    break;

                case 'Z':
                    this.match("Z");
                    break;

                case 'g':
                    this.match("g");
                    break;

                case 'B':
                    this.match("B");
                    break;

                case 'k':
                    this.match("k");
                    break;

                case '/':
                    this.match("/");
                    break;

                case '(':
                    this.match("(");
                    break;

                case ')':
                    this.match(")");
                    break;

                case '|':
                    this.match("|");
                    break;

                case '.':
                    this.match(".");
                    break;

                case '*':
                    this.match("*");
                    break;

                case '?':
                    this.match("?");
                    break;

                case '$':
                    this.match("$");
                    break;

                case '^':
                    this.match("^");
                    break;

                case '[':
                    this.match("[");
                    break;

                case ']':
                    this.match("]");
                    break;

                default:
                    throw new NoViableAltForCharException(base.cached_LA1, this.getFilename(), this.getLine(), this.getColumn());
            }
            if ((_createToken && (token == null)) && (num != Token.SKIP))
            {
                token = this.makeToken(num);
                token.setText(base.text.ToString(length, base.text.Length - length));
            }
            base.returnToken_ = token;
        }

        protected void mRE_LITERAL(bool _createToken)
        {
            int num = new int();
            IToken token = null;
            int length = base.text.Length;
            num = 0x69;
            this.match("/");
            int num3 = 0;
            while (true)
            {
                if (tokenSet_5_.member((int) base.cached_LA1))
                {
                    this.mRE_CHAR(false);
                }
                else
                {
                    if (num3 < 1)
                    {
                        throw new NoViableAltForCharException(base.cached_LA1, this.getFilename(), this.getLine(), this.getColumn());
                    }
                    break;
                }
                num3++;
            }
            this.match("/");
            if ((_createToken && (token == null)) && (num != Token.SKIP))
            {
                token = this.makeToken(num);
                token.setText(base.text.ToString(length, base.text.Length - length));
            }
            base.returnToken_ = token;
        }

        public void mREFERENCE_EQUALITY(bool _createToken)
        {
            int num = new int();
            IToken token = null;
            int length = base.text.Length;
            num = 0x59;
            this.match("===");
            if ((_createToken && (token == null)) && (num != Token.SKIP))
            {
                token = this.makeToken(num);
                token.setText(base.text.ToString(length, base.text.Length - length));
            }
            base.returnToken_ = token;
        }

        public void mREFERENCE_INEQUALITY(bool _createToken)
        {
            int num = new int();
            IToken token = null;
            int length = base.text.Length;
            num = 90;
            this.match("!==");
            if ((_createToken && (token == null)) && (num != Token.SKIP))
            {
                token = this.makeToken(num);
                token.setText(base.text.ToString(length, base.text.Length - length));
            }
            base.returnToken_ = token;
        }

        public void mRPAREN(bool _createToken)
        {
            int num = new int();
            IToken token = null;
            int length = base.text.Length;
            num = 0x40;
            this.match(")");
            if ((_createToken && (token == null)) && (num != Token.SKIP))
            {
                token = this.makeToken(num);
                token.setText(base.text.ToString(length, base.text.Length - length));
            }
            base.returnToken_ = token;
        }

        protected void mSESC(bool _createToken)
        {
            int num3;
            int num = new int();
            IToken token = null;
            int length = base.text.Length;
            num = 0x74;
            switch (base.cached_LA1)
            {
                case 'r':
                    num3 = base.text.Length;
                    this.match("r");
                    base.text.Length = num3;
                    if (base.inputState.guessing == 0)
                    {
                        base.text.Length = length;
                        base.text.Append("\r");
                    }
                    break;

                case 'n':
                    num3 = base.text.Length;
                    this.match("n");
                    base.text.Length = num3;
                    if (base.inputState.guessing == 0)
                    {
                        base.text.Length = length;
                        base.text.Append("\n");
                    }
                    break;

                case 't':
                    num3 = base.text.Length;
                    this.match("t");
                    base.text.Length = num3;
                    if (base.inputState.guessing == 0)
                    {
                        base.text.Length = length;
                        base.text.Append("\t");
                    }
                    break;

                case 'a':
                    num3 = base.text.Length;
                    this.match("a");
                    base.text.Length = num3;
                    if (base.inputState.guessing == 0)
                    {
                        base.text.Length = length;
                        base.text.Append("\a");
                    }
                    break;

                case 'b':
                    num3 = base.text.Length;
                    this.match("b");
                    base.text.Length = num3;
                    if (base.inputState.guessing == 0)
                    {
                        base.text.Length = length;
                        base.text.Append("\b");
                    }
                    break;

                case 'f':
                    num3 = base.text.Length;
                    this.match("f");
                    base.text.Length = num3;
                    if (base.inputState.guessing == 0)
                    {
                        base.text.Length = length;
                        base.text.Append("\f");
                    }
                    break;

                case '0':
                    num3 = base.text.Length;
                    this.match("0");
                    base.text.Length = num3;
                    if (base.inputState.guessing == 0)
                    {
                        base.text.Length = length;
                        base.text.Append("\0");
                    }
                    break;

                case '\\':
                    num3 = base.text.Length;
                    this.match(@"\");
                    base.text.Length = num3;
                    if (base.inputState.guessing == 0)
                    {
                        base.text.Length = length;
                        base.text.Append(@"\");
                    }
                    break;

                default:
                    throw new NoViableAltForCharException(base.cached_LA1, this.getFilename(), this.getLine(), this.getColumn());
            }
            if ((_createToken && (token == null)) && (num != Token.SKIP))
            {
                token = this.makeToken(num);
                token.setText(base.text.ToString(length, base.text.Length - length));
            }
            base.returnToken_ = token;
        }

        public void mSHIFT_LEFT(bool _createToken)
        {
            int num = new int();
            IToken token = null;
            int length = base.text.Length;
            num = 0x5d;
            this.match("<<");
            if ((_createToken && (token == null)) && (num != Token.SKIP))
            {
                token = this.makeToken(num);
                token.setText(base.text.ToString(length, base.text.Length - length));
            }
            base.returnToken_ = token;
        }

        public void mSHIFT_RIGHT(bool _createToken)
        {
            int num = new int();
            IToken token = null;
            int length = base.text.Length;
            num = 0x61;
            this.match(">>");
            if ((_createToken && (token == null)) && (num != Token.SKIP))
            {
                token = this.makeToken(num);
                token.setText(base.text.ToString(length, base.text.Length - length));
            }
            base.returnToken_ = token;
        }

        public void mSINGLE_QUOTED_STRING(bool _createToken)
        {
            int num = new int();
            IToken token = null;
            int length = base.text.Length;
            num = 0x6d;
            int num3 = base.text.Length;
            this.match("'");
            base.text.Length = num3;
        Label_0041:
            while (base.cached_LA1 == '\\')
            {
                this.mSQS_ESC(false);
            }
            if (tokenSet_4_.member((int) base.cached_LA1))
            {
                this.match(tokenSet_4_);
                goto Label_0041;
            }
            num3 = base.text.Length;
            this.match("'");
            base.text.Length = num3;
            if ((_createToken && (token == null)) && (num != Token.SKIP))
            {
                token = this.makeToken(num);
                token.setText(base.text.ToString(length, base.text.Length - length));
            }
            base.returnToken_ = token;
        }

        protected void mSQS_ESC(bool _createToken)
        {
            int num = new int();
            IToken token = null;
            int length = base.text.Length;
            num = 0x73;
            int num3 = base.text.Length;
            this.match(@"\");
            base.text.Length = num3;
            char ch = base.cached_LA1;
            switch (ch)
            {
                case '0':
                case '\\':
                case 'a':
                case 'b':
                case 'f':
                case 'n':
                case 'r':
                case 't':
                    this.mSESC(false);
                    break;

                default:
                    if (ch != '\'')
                    {
                        throw new NoViableAltForCharException(base.cached_LA1, this.getFilename(), this.getLine(), this.getColumn());
                    }
                    this.match("'");
                    break;
            }
            if ((_createToken && (token == null)) && (num != Token.SKIP))
            {
                token = this.makeToken(num);
                token.setText(base.text.ToString(length, base.text.Length - length));
            }
            base.returnToken_ = token;
        }

        public void mSUBTRACT(bool _createToken)
        {
            int num = new int();
            IToken token = null;
            int length = base.text.Length;
            num = 0x52;
            this.match("-");
            if ((_createToken && (token == null)) && (num != Token.SKIP))
            {
                token = this.makeToken(num);
                token.setText(base.text.ToString(length, base.text.Length - length));
            }
            base.returnToken_ = token;
        }

        public void mWHITE_SPACE(bool _createToken)
        {
            int sKIP = new int();
            IToken token = null;
            int length = base.text.Length;
            sKIP = 0x71;
            int num3 = 0;
            while (true)
            {
                switch (base.cached_LA1)
                {
                    case ' ':
                        this.match(" ");
                        break;

                    case '\t':
                        this.match("\t");
                        break;

                    case '\f':
                        this.match("\f");
                        break;

                    case '\n':
                    case '\r':
                        this.mNEWLINE(false);
                        break;

                    default:
                        if (num3 < 1)
                        {
                            throw new NoViableAltForCharException(base.cached_LA1, this.getFilename(), this.getLine(), this.getColumn());
                        }
                        if (base.inputState.guessing == 0)
                        {
                            sKIP = Token.SKIP;
                        }
                        if ((_createToken && (token == null)) && (sKIP != Token.SKIP))
                        {
                            token = this.makeToken(sKIP);
                            token.setText(base.text.ToString(length, base.text.Length - length));
                        }
                        base.returnToken_ = token;
                        return;
                }
                num3++;
            }
        }

        public override IToken nextToken()
        {
            IToken token = null;
            while (true)
            {
                int num = 0;
                this.resetText();
                try
                {
                    try
                    {
                        switch (base.cached_LA1)
                        {
                            case 'A':
                            case 'B':
                            case 'C':
                            case 'D':
                            case 'E':
                            case 'F':
                            case 'G':
                            case 'H':
                            case 'I':
                            case 'J':
                            case 'K':
                            case 'L':
                            case 'M':
                            case 'N':
                            case 'O':
                            case 'P':
                            case 'Q':
                            case 'R':
                            case 'S':
                            case 'T':
                            case 'U':
                            case 'V':
                            case 'W':
                            case 'X':
                            case 'Y':
                            case 'Z':
                            case '_':
                            case 'a':
                            case 'b':
                            case 'c':
                            case 'd':
                            case 'e':
                            case 'f':
                            case 'g':
                            case 'h':
                            case 'i':
                            case 'j':
                            case 'k':
                            case 'l':
                            case 'm':
                            case 'n':
                            case 'o':
                            case 'p':
                            case 'q':
                            case 'r':
                            case 's':
                            case 't':
                            case 'u':
                            case 'v':
                            case 'w':
                            case 'x':
                            case 'y':
                            case 'z':
                                this.mID(true);
                                token = base.returnToken_;
                                break;

                            case '0':
                            case '1':
                            case '2':
                            case '3':
                            case '4':
                            case '5':
                            case '6':
                            case '7':
                            case '8':
                            case '9':
                                this.mINT(true);
                                token = base.returnToken_;
                                break;

                            case '.':
                                this.mDOT(true);
                                token = base.returnToken_;
                                break;

                            case ':':
                                this.mCOLON(true);
                                token = base.returnToken_;
                                break;

                            case ',':
                                this.mCOMMA(true);
                                token = base.returnToken_;
                                break;

                            case '(':
                                this.mLPAREN(true);
                                token = base.returnToken_;
                                break;

                            case ')':
                                this.mRPAREN(true);
                                token = base.returnToken_;
                                break;

                            case '[':
                                this.mLBRACK(true);
                                token = base.returnToken_;
                                break;

                            case ']':
                                this.mRBRACK(true);
                                token = base.returnToken_;
                                break;

                            case '{':
                                this.mLBRACE(true);
                                token = base.returnToken_;
                                break;

                            case '}':
                                this.mRBRACE(true);
                                token = base.returnToken_;
                                break;

                            case ';':
                                this.mEOS(true);
                                token = base.returnToken_;
                                break;

                            case '#':
                                this.mPRAGMA_ON(true);
                                token = base.returnToken_;
                                break;

                            case '%':
                                this.mMODULUS(true);
                                token = base.returnToken_;
                                break;

                            case '?':
                                this.mQUESTION_MARK(true);
                                token = base.returnToken_;
                                break;

                            case '~':
                                this.mBITWISE_NOT(true);
                                token = base.returnToken_;
                                break;

                            case '@':
                                this.mAT(true);
                                token = base.returnToken_;
                                break;

                            case '/':
                                this.mDIVISION(true);
                                token = base.returnToken_;
                                break;

                            case '\t':
                            case '\n':
                            case '\f':
                            case '\r':
                            case ' ':
                                this.mWHITE_SPACE(true);
                                token = base.returnToken_;
                                break;

                            case '"':
                                this.mDOUBLE_QUOTED_STRING(true);
                                token = base.returnToken_;
                                break;

                            case '\'':
                                this.mSINGLE_QUOTED_STRING(true);
                                token = base.returnToken_;
                                break;

                            default:
                                if (((base.cached_LA1 == '=') && (base.cached_LA2 == '=')) && (this.LA(3) == '='))
                                {
                                    this.mREFERENCE_EQUALITY(true);
                                    token = base.returnToken_;
                                }
                                else if (((base.cached_LA1 == '!') && (base.cached_LA2 == '=')) && (this.LA(3) == '='))
                                {
                                    this.mREFERENCE_INEQUALITY(true);
                                    token = base.returnToken_;
                                }
                                else if (((base.cached_LA1 == '<') && (base.cached_LA2 == '<')) && (this.LA(3) == '='))
                                {
                                    this.mINPLACE_SHIFT_LEFT(true);
                                    token = base.returnToken_;
                                }
                                else if (((base.cached_LA1 == '>') && (base.cached_LA2 == '>')) && (this.LA(3) == '='))
                                {
                                    this.mINPLACE_SHIFT_RIGHT(true);
                                    token = base.returnToken_;
                                }
                                else if ((base.cached_LA1 == '|') && (base.cached_LA2 == '='))
                                {
                                    this.mINPLACE_BITWISE_OR(true);
                                    token = base.returnToken_;
                                }
                                else if ((base.cached_LA1 == '&') && (base.cached_LA2 == '='))
                                {
                                    this.mINPLACE_BITWISE_AND(true);
                                    token = base.returnToken_;
                                }
                                else if ((base.cached_LA1 == '^') && (base.cached_LA2 == '='))
                                {
                                    this.mINPLACE_BITWISE_XOR(true);
                                    token = base.returnToken_;
                                }
                                else if ((base.cached_LA1 == '|') && (base.cached_LA2 == '|'))
                                {
                                    this.mLOGICAL_OR(true);
                                    token = base.returnToken_;
                                }
                                else if ((base.cached_LA1 == '&') && (base.cached_LA2 == '&'))
                                {
                                    this.mLOGICAL_AND(true);
                                    token = base.returnToken_;
                                }
                                else if ((base.cached_LA1 == '+') && (base.cached_LA2 == '+'))
                                {
                                    this.mINCREMENT(true);
                                    token = base.returnToken_;
                                }
                                else if ((base.cached_LA1 == '-') && (base.cached_LA2 == '-'))
                                {
                                    this.mDECREMENT(true);
                                    token = base.returnToken_;
                                }
                                else if ((base.cached_LA1 == '+') && (base.cached_LA2 == '='))
                                {
                                    this.mINPLACE_ADD(true);
                                    token = base.returnToken_;
                                }
                                else if ((base.cached_LA1 == '-') && (base.cached_LA2 == '='))
                                {
                                    this.mINPLACE_SUBTRACT(true);
                                    token = base.returnToken_;
                                }
                                else if ((base.cached_LA1 == '*') && (base.cached_LA2 == '='))
                                {
                                    this.mINPLACE_MULTIPLY(true);
                                    token = base.returnToken_;
                                }
                                else if (((base.cached_LA1 == '=') && (base.cached_LA2 == '=')) && (1 != 0))
                                {
                                    this.mEQUALITY(true);
                                    token = base.returnToken_;
                                }
                                else if (((base.cached_LA1 == '!') && (base.cached_LA2 == '=')) && (1 != 0))
                                {
                                    this.mINEQUALITY(true);
                                    token = base.returnToken_;
                                }
                                else if ((base.cached_LA1 == '<') && (base.cached_LA2 == '='))
                                {
                                    this.mLESS_THAN_OR_EQUAL(true);
                                    token = base.returnToken_;
                                }
                                else if (((base.cached_LA1 == '<') && (base.cached_LA2 == '<')) && (1 != 0))
                                {
                                    this.mSHIFT_LEFT(true);
                                    token = base.returnToken_;
                                }
                                else if ((base.cached_LA1 == '>') && (base.cached_LA2 == '='))
                                {
                                    this.mGREATER_THAN_OR_EQUAL(true);
                                    token = base.returnToken_;
                                }
                                else if (((base.cached_LA1 == '>') && (base.cached_LA2 == '>')) && (1 != 0))
                                {
                                    this.mSHIFT_RIGHT(true);
                                    token = base.returnToken_;
                                }
                                else if ((base.cached_LA1 == '|') && (1 != 0))
                                {
                                    this.mBITWISE_OR(true);
                                    token = base.returnToken_;
                                }
                                else if ((base.cached_LA1 == '&') && (1 != 0))
                                {
                                    this.mBITWISE_AND(true);
                                    token = base.returnToken_;
                                }
                                else if ((base.cached_LA1 == '^') && (1 != 0))
                                {
                                    this.mBITWISE_XOR(true);
                                    token = base.returnToken_;
                                }
                                else if ((base.cached_LA1 == '=') && (1 != 0))
                                {
                                    this.mASSIGN(true);
                                    token = base.returnToken_;
                                }
                                else if ((base.cached_LA1 == '+') && (1 != 0))
                                {
                                    this.mADD(true);
                                    token = base.returnToken_;
                                }
                                else if ((base.cached_LA1 == '-') && (1 != 0))
                                {
                                    this.mSUBTRACT(true);
                                    token = base.returnToken_;
                                }
                                else if ((base.cached_LA1 == '*') && (1 != 0))
                                {
                                    this.mMULTIPLY(true);
                                    token = base.returnToken_;
                                }
                                else if ((base.cached_LA1 == '!') && (1 != 0))
                                {
                                    this.mLOGICAL_NOT(true);
                                    token = base.returnToken_;
                                }
                                else if ((base.cached_LA1 == '<') && (1 != 0))
                                {
                                    this.mLESS_THAN(true);
                                    token = base.returnToken_;
                                }
                                else if ((base.cached_LA1 == '>') && (1 != 0))
                                {
                                    this.mGREATER_THAN(true);
                                    token = base.returnToken_;
                                }
                                else
                                {
                                    if (base.cached_LA1 != CharScanner.EOF_CHAR)
                                    {
                                        throw new NoViableAltForCharException(base.cached_LA1, this.getFilename(), this.getLine(), this.getColumn());
                                    }
                                    this.uponEOF();
                                    base.returnToken_ = this.makeToken(1);
                                }
                                break;
                        }
                        if (base.returnToken_ != null)
                        {
                            num = base.returnToken_.get_Type();
                            base.returnToken_.set_Type(num);
                            return base.returnToken_;
                        }
                    }
                    catch (RecognitionException exception)
                    {
                        throw new TokenStreamRecognitionException(exception);
                    }
                }
                catch (CharStreamException exception2)
                {
                    if (exception2 is CharStreamIOException)
                    {
                        throw new TokenStreamIOException(((CharStreamIOException) exception2).io);
                    }
                    throw new TokenStreamException(exception2.Message);
                }
            }
        }

        public bool PreserveComments
        {
            get
            {
                return this.$PreserveComments$26;
            }
            set
            {
                this.$PreserveComments$26 = value;
            }
        }
    }
}

