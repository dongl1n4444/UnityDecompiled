namespace UnityEditor.iOS.Xcode.PBX
{
    using System;

    internal class Parser
    {
        private int currPos;
        private TokenList tokens;

        public Parser(TokenList tokens)
        {
            this.tokens = tokens;
            this.currPos = this.SkipComments(0);
        }

        private string GetErrorMsg()
        {
            return ("Invalid PBX project (parsing line " + this.tokens[this.currPos].line + ")");
        }

        private int Inc()
        {
            int currPos = this.currPos;
            this.currPos = this.IncInternal(this.currPos);
            return currPos;
        }

        private int IncInternal(int pos)
        {
            if (pos >= this.tokens.Count)
            {
                return pos;
            }
            pos++;
            return this.SkipComments(pos);
        }

        public IdentifierAST ParseIdentifier()
        {
            if ((this.Tok() != UnityEditor.iOS.Xcode.PBX.TokenType.String) && (this.Tok() != UnityEditor.iOS.Xcode.PBX.TokenType.QuotedString))
            {
                throw new Exception(this.GetErrorMsg());
            }
            return new IdentifierAST { value = this.Inc() };
        }

        public KeyValueAST ParseKeyValue()
        {
            KeyValueAST east = new KeyValueAST {
                key = this.ParseIdentifier()
            };
            if (this.Tok() != UnityEditor.iOS.Xcode.PBX.TokenType.Eq)
            {
                throw new Exception(this.GetErrorMsg());
            }
            this.Inc();
            east.value = this.ParseValue();
            this.SkipIf(UnityEditor.iOS.Xcode.PBX.TokenType.Semicolon);
            return east;
        }

        public ArrayAST ParseList()
        {
            if (this.Tok() != UnityEditor.iOS.Xcode.PBX.TokenType.LParen)
            {
                throw new Exception(this.GetErrorMsg());
            }
            this.Inc();
            ArrayAST yast = new ArrayAST();
            while ((this.Tok() != UnityEditor.iOS.Xcode.PBX.TokenType.RParen) && (this.Tok() != UnityEditor.iOS.Xcode.PBX.TokenType.EOF))
            {
                yast.values.Add(this.ParseValue());
                this.SkipIf(UnityEditor.iOS.Xcode.PBX.TokenType.Comma);
            }
            this.SkipIf(UnityEditor.iOS.Xcode.PBX.TokenType.RParen);
            return yast;
        }

        public TreeAST ParseTree()
        {
            if (this.Tok() != UnityEditor.iOS.Xcode.PBX.TokenType.LBrace)
            {
                throw new Exception(this.GetErrorMsg());
            }
            this.Inc();
            TreeAST east = new TreeAST();
            while ((this.Tok() != UnityEditor.iOS.Xcode.PBX.TokenType.RBrace) && (this.Tok() != UnityEditor.iOS.Xcode.PBX.TokenType.EOF))
            {
                east.values.Add(this.ParseKeyValue());
            }
            this.SkipIf(UnityEditor.iOS.Xcode.PBX.TokenType.RBrace);
            return east;
        }

        public ValueAST ParseValue()
        {
            if ((this.Tok() == UnityEditor.iOS.Xcode.PBX.TokenType.String) || (this.Tok() == UnityEditor.iOS.Xcode.PBX.TokenType.QuotedString))
            {
                return this.ParseIdentifier();
            }
            if (this.Tok() == UnityEditor.iOS.Xcode.PBX.TokenType.LBrace)
            {
                return this.ParseTree();
            }
            if (this.Tok() != UnityEditor.iOS.Xcode.PBX.TokenType.LParen)
            {
                throw new Exception(this.GetErrorMsg());
            }
            return this.ParseList();
        }

        private int SkipComments(int pos)
        {
            while ((pos < this.tokens.Count) && (this.tokens[pos].type == UnityEditor.iOS.Xcode.PBX.TokenType.Comment))
            {
                pos++;
            }
            return pos;
        }

        private void SkipIf(UnityEditor.iOS.Xcode.PBX.TokenType type)
        {
            if (this.Tok() == type)
            {
                this.Inc();
            }
        }

        private UnityEditor.iOS.Xcode.PBX.TokenType Tok()
        {
            if (this.currPos >= this.tokens.Count)
            {
                return UnityEditor.iOS.Xcode.PBX.TokenType.EOF;
            }
            return this.tokens[this.currPos].type;
        }
    }
}

