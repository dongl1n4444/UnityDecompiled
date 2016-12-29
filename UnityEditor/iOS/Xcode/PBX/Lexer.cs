namespace UnityEditor.iOS.Xcode.PBX
{
    using System;

    internal class Lexer
    {
        private int length;
        private int line;
        private int pos;
        private string text;

        private bool IsOperator(char ch) => 
            ((((ch == ';') || (ch == ',')) || ((ch == '=') || (ch == '('))) || (((ch == ')') || (ch == '{')) || (ch == '}')));

        public TokenList ScanAll()
        {
            TokenList list = new TokenList();
            while (true)
            {
                Token tok = new Token();
                this.ScanOne(tok);
                list.Add(tok);
                if (tok.type == UnityEditor.iOS.Xcode.PBX.TokenType.EOF)
                {
                    return list;
                }
            }
        }

        private void ScanComment(Token tok)
        {
            tok.type = UnityEditor.iOS.Xcode.PBX.TokenType.Comment;
            tok.begin = this.pos;
            this.pos += 2;
            while (this.pos < this.length)
            {
                if (this.text[this.pos] == '\n')
                {
                    break;
                }
                this.pos++;
            }
            this.UpdateNewlineStats(this.text[this.pos]);
            this.pos++;
            tok.end = this.pos;
            tok.line = this.line;
        }

        private void ScanMultilineComment(Token tok)
        {
            tok.type = UnityEditor.iOS.Xcode.PBX.TokenType.Comment;
            tok.begin = this.pos;
            this.pos += 2;
            while (this.pos < this.length)
            {
                if ((this.text[this.pos] == '*') && (this.text[this.pos + 1] == '/'))
                {
                    break;
                }
                this.UpdateNewlineStats(this.text[this.pos]);
                this.pos++;
            }
            this.pos += 2;
            tok.end = this.pos;
            tok.line = this.line;
        }

        private void ScanOne(Token tok)
        {
            while ((this.pos < this.length) && char.IsWhiteSpace(this.text[this.pos]))
            {
                this.UpdateNewlineStats(this.text[this.pos]);
                this.pos++;
            }
            if (this.pos >= this.length)
            {
                tok.type = UnityEditor.iOS.Xcode.PBX.TokenType.EOF;
            }
            else
            {
                char ch = this.text[this.pos];
                char ch2 = this.text[this.pos + 1];
                if (ch == '"')
                {
                    this.ScanQuotedString(tok);
                }
                else if ((ch == '/') && (ch2 == '*'))
                {
                    this.ScanMultilineComment(tok);
                }
                else if ((ch == '/') && (ch2 == '/'))
                {
                    this.ScanComment(tok);
                }
                else if (this.IsOperator(ch))
                {
                    this.ScanOperator(tok);
                }
                else
                {
                    this.ScanString(tok);
                }
            }
        }

        private void ScanOperator(Token tok)
        {
            switch (this.text[this.pos])
            {
                case '(':
                    this.ScanOperatorSpecific(tok, UnityEditor.iOS.Xcode.PBX.TokenType.LParen);
                    return;

                case ')':
                    this.ScanOperatorSpecific(tok, UnityEditor.iOS.Xcode.PBX.TokenType.RParen);
                    return;

                case ',':
                    this.ScanOperatorSpecific(tok, UnityEditor.iOS.Xcode.PBX.TokenType.Comma);
                    return;

                case ';':
                    this.ScanOperatorSpecific(tok, UnityEditor.iOS.Xcode.PBX.TokenType.Semicolon);
                    return;

                case '=':
                    this.ScanOperatorSpecific(tok, UnityEditor.iOS.Xcode.PBX.TokenType.Eq);
                    return;

                case '{':
                    this.ScanOperatorSpecific(tok, UnityEditor.iOS.Xcode.PBX.TokenType.LBrace);
                    break;

                case '}':
                    this.ScanOperatorSpecific(tok, UnityEditor.iOS.Xcode.PBX.TokenType.RBrace);
                    break;
            }
        }

        private void ScanOperatorSpecific(Token tok, UnityEditor.iOS.Xcode.PBX.TokenType type)
        {
            tok.type = type;
            tok.begin = this.pos;
            this.pos++;
            tok.end = this.pos;
            tok.line = this.line;
        }

        private void ScanQuotedString(Token tok)
        {
            tok.type = UnityEditor.iOS.Xcode.PBX.TokenType.QuotedString;
            tok.begin = this.pos;
            this.pos++;
            while (this.pos < this.length)
            {
                if ((this.text[this.pos] == '\\') && (this.text[this.pos + 1] == '"'))
                {
                    this.pos += 2;
                }
                else
                {
                    if (this.text[this.pos] == '"')
                    {
                        break;
                    }
                    this.UpdateNewlineStats(this.text[this.pos]);
                    this.pos++;
                }
            }
            this.pos++;
            tok.end = this.pos;
            tok.line = this.line;
        }

        private void ScanString(Token tok)
        {
            tok.type = UnityEditor.iOS.Xcode.PBX.TokenType.String;
            tok.begin = this.pos;
            while (this.pos < this.length)
            {
                char c = this.text[this.pos];
                char ch2 = this.text[this.pos + 1];
                if (((char.IsWhiteSpace(c) || (c == '"')) || (((c == '/') && (ch2 == '*')) || ((c == '/') && (ch2 == '/')))) || this.IsOperator(c))
                {
                    break;
                }
                this.pos++;
            }
            tok.end = this.pos;
            tok.line = this.line;
        }

        public void SetText(string text)
        {
            this.text = text + "    ";
            this.pos = 0;
            this.length = text.Length;
            this.line = 0;
        }

        public static TokenList Tokenize(string text)
        {
            Lexer lexer = new Lexer();
            lexer.SetText(text);
            return lexer.ScanAll();
        }

        private void UpdateNewlineStats(char ch)
        {
            if (ch == '\n')
            {
                this.line++;
            }
        }
    }
}

