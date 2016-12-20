namespace UnityScript.Parser
{
    using System;

    [Serializable]
    public class UnityScriptTokenTypes
    {
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
    }
}

