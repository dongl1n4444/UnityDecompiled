namespace UnityEditor.iOS.Xcode.PBX
{
    using System;

    internal enum TokenType
    {
        EOF,
        Invalid,
        String,
        QuotedString,
        Comment,
        Semicolon,
        Comma,
        Eq,
        LParen,
        RParen,
        LBrace,
        RBrace
    }
}

