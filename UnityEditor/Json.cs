﻿namespace UnityEditor
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    internal static class Json
    {
        public static object Deserialize(string json)
        {
            if (json == null)
            {
                return null;
            }
            return Parser.Parse(json);
        }

        public static string Serialize(object obj)
        {
            return Serializer.Serialize(obj);
        }

        private sealed class Parser : IDisposable
        {
            private StringReader json;
            private const string WORD_BREAK = "{}[],:\"";

            private Parser(string jsonString)
            {
                this.json = new StringReader(jsonString);
            }

            public void Dispose()
            {
                this.json.Dispose();
                this.json = null;
            }

            private void EatWhitespace()
            {
                while (char.IsWhiteSpace(this.PeekChar))
                {
                    this.json.Read();
                    if (this.json.Peek() == -1)
                    {
                        break;
                    }
                }
            }

            public static bool IsWordBreak(char c)
            {
                return (char.IsWhiteSpace(c) || ("{}[],:\"".IndexOf(c) != -1));
            }

            public static object Parse(string jsonString)
            {
                using (Json.Parser parser = new Json.Parser(jsonString))
                {
                    return parser.ParseValue();
                }
            }

            private List<object> ParseArray()
            {
                List<object> list = new List<object>();
                this.json.Read();
                bool flag = true;
                while (flag)
                {
                    TOKEN nextToken = this.NextToken;
                    switch (nextToken)
                    {
                        case TOKEN.SQUARED_CLOSE:
                            flag = false;
                            break;

                        case TOKEN.COMMA:
                        {
                            continue;
                        }
                        case TOKEN.NONE:
                            return null;

                        default:
                        {
                            object item = this.ParseByToken(nextToken);
                            list.Add(item);
                            break;
                        }
                    }
                }
                return list;
            }

            private object ParseByToken(TOKEN token)
            {
                switch (token)
                {
                    case TOKEN.STRING:
                        return this.ParseString();

                    case TOKEN.NUMBER:
                        return this.ParseNumber();

                    case TOKEN.TRUE:
                        return true;

                    case TOKEN.FALSE:
                        return false;

                    case TOKEN.NULL:
                        return null;

                    case TOKEN.CURLY_OPEN:
                        return this.ParseObject();

                    case TOKEN.SQUARED_OPEN:
                        return this.ParseArray();
                }
                return null;
            }

            private object ParseNumber()
            {
                double num2;
                string nextWord = this.NextWord;
                if (nextWord.IndexOf('.') == -1)
                {
                    long num;
                    long.TryParse(nextWord, NumberStyles.Any, CultureInfo.InvariantCulture, out num);
                    return num;
                }
                double.TryParse(nextWord, NumberStyles.Any, CultureInfo.InvariantCulture, out num2);
                return num2;
            }

            private Dictionary<string, object> ParseObject()
            {
                Dictionary<string, object> dictionary = new Dictionary<string, object>();
                this.json.Read();
                while (true)
                {
                    TOKEN nextToken = this.NextToken;
                    switch (nextToken)
                    {
                        case TOKEN.NONE:
                            return null;

                        case TOKEN.CURLY_CLOSE:
                            return dictionary;
                    }
                    if (nextToken != TOKEN.COMMA)
                    {
                        string str = this.ParseString();
                        if (str == null)
                        {
                            return null;
                        }
                        if (this.NextToken != TOKEN.COLON)
                        {
                            return null;
                        }
                        this.json.Read();
                        dictionary[str] = this.ParseValue();
                    }
                }
            }

            private string ParseString()
            {
                StringBuilder builder = new StringBuilder();
                this.json.Read();
                bool flag = true;
                while (flag)
                {
                    char[] chArray;
                    int num;
                    if (this.json.Peek() == -1)
                    {
                        flag = false;
                        break;
                    }
                    char nextChar = this.NextChar;
                    if (nextChar == '"')
                    {
                        flag = false;
                        continue;
                    }
                    if (nextChar != '\\')
                    {
                        goto Label_015F;
                    }
                    if (this.json.Peek() == -1)
                    {
                        flag = false;
                        continue;
                    }
                    nextChar = this.NextChar;
                    switch (nextChar)
                    {
                        case 'r':
                        {
                            builder.Append('\r');
                            continue;
                        }
                        case 't':
                        {
                            builder.Append('\t');
                            continue;
                        }
                        case 'u':
                            chArray = new char[4];
                            num = 0;
                            goto Label_0138;

                        default:
                        {
                            if (((nextChar != '"') && (nextChar != '/')) && (nextChar != '\\'))
                            {
                                if (nextChar == 'b')
                                {
                                    break;
                                }
                                if (nextChar == 'f')
                                {
                                    goto Label_00DF;
                                }
                                if (nextChar == 'n')
                                {
                                    goto Label_00ED;
                                }
                            }
                            else
                            {
                                builder.Append(nextChar);
                            }
                            continue;
                        }
                    }
                    builder.Append('\b');
                    continue;
                Label_00DF:
                    builder.Append('\f');
                    continue;
                Label_00ED:
                    builder.Append('\n');
                    continue;
                Label_0126:
                    chArray[num] = this.NextChar;
                    num++;
                Label_0138:
                    if (num < 4)
                    {
                        goto Label_0126;
                    }
                    builder.Append((char) Convert.ToInt32(new string(chArray), 0x10));
                    continue;
                Label_015F:
                    builder.Append(nextChar);
                }
                return builder.ToString();
            }

            private object ParseValue()
            {
                TOKEN nextToken = this.NextToken;
                return this.ParseByToken(nextToken);
            }

            private char NextChar
            {
                get
                {
                    return Convert.ToChar(this.json.Read());
                }
            }

            private TOKEN NextToken
            {
                get
                {
                    this.EatWhitespace();
                    if (this.json.Peek() != -1)
                    {
                        switch (this.PeekChar)
                        {
                            case ',':
                                this.json.Read();
                                return TOKEN.COMMA;

                            case '-':
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
                                return TOKEN.NUMBER;

                            case ':':
                                return TOKEN.COLON;

                            case '[':
                                return TOKEN.SQUARED_OPEN;

                            case ']':
                                this.json.Read();
                                return TOKEN.SQUARED_CLOSE;

                            case '{':
                                return TOKEN.CURLY_OPEN;

                            case '}':
                                this.json.Read();
                                return TOKEN.CURLY_CLOSE;

                            case '"':
                                return TOKEN.STRING;
                        }
                        switch (this.NextWord)
                        {
                            case "false":
                                return TOKEN.FALSE;

                            case "true":
                                return TOKEN.TRUE;

                            case "null":
                                return TOKEN.NULL;
                        }
                    }
                    return TOKEN.NONE;
                }
            }

            private string NextWord
            {
                get
                {
                    StringBuilder builder = new StringBuilder();
                    while (!IsWordBreak(this.PeekChar))
                    {
                        builder.Append(this.NextChar);
                        if (this.json.Peek() == -1)
                        {
                            break;
                        }
                    }
                    return builder.ToString();
                }
            }

            private char PeekChar
            {
                get
                {
                    return Convert.ToChar(this.json.Peek());
                }
            }

            private enum TOKEN
            {
                NONE,
                CURLY_OPEN,
                CURLY_CLOSE,
                SQUARED_OPEN,
                SQUARED_CLOSE,
                COLON,
                COMMA,
                STRING,
                NUMBER,
                TRUE,
                FALSE,
                NULL
            }
        }

        private sealed class Serializer
        {
            private StringBuilder builder = new StringBuilder();

            private Serializer()
            {
            }

            public static string Serialize(object obj)
            {
                Json.Serializer serializer = new Json.Serializer();
                serializer.SerializeValue(obj);
                return serializer.builder.ToString();
            }

            private void SerializeArray(IList anArray)
            {
                this.builder.Append('[');
                bool flag = true;
                IEnumerator enumerator = anArray.GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        object current = enumerator.Current;
                        if (!flag)
                        {
                            this.builder.Append(',');
                        }
                        this.SerializeValue(current);
                        flag = false;
                    }
                }
                finally
                {
                    IDisposable disposable = enumerator as IDisposable;
                    if (disposable != null)
                    {
                        disposable.Dispose();
                    }
                }
                this.builder.Append(']');
            }

            private void SerializeObject(IDictionary obj)
            {
                bool flag = true;
                this.builder.Append('{');
                IEnumerator enumerator = obj.Keys.GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        object current = enumerator.Current;
                        if (!flag)
                        {
                            this.builder.Append(',');
                        }
                        this.SerializeString(current.ToString());
                        this.builder.Append(':');
                        this.SerializeValue(obj[current]);
                        flag = false;
                    }
                }
                finally
                {
                    IDisposable disposable = enumerator as IDisposable;
                    if (disposable != null)
                    {
                        disposable.Dispose();
                    }
                }
                this.builder.Append('}');
            }

            private void SerializeOther(object value)
            {
                if (value is float)
                {
                    this.builder.Append(((float) value).ToString("R", CultureInfo.InvariantCulture));
                }
                else if ((((value is int) || (value is uint)) || ((value is long) || (value is sbyte))) || (((value is byte) || (value is short)) || ((value is ushort) || (value is ulong))))
                {
                    this.builder.Append(value);
                }
                else if ((value is double) || (value is decimal))
                {
                    this.builder.Append(Convert.ToDouble(value).ToString("R", CultureInfo.InvariantCulture));
                }
                else
                {
                    Dictionary<string, object> dictionary = new Dictionary<string, object>();
                    List<FieldInfo> list = Enumerable.ToList<FieldInfo>(value.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance));
                    foreach (FieldInfo info in list)
                    {
                        dictionary.Add(info.Name, info.GetValue(value));
                    }
                    List<PropertyInfo> list2 = Enumerable.ToList<PropertyInfo>(value.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance));
                    foreach (PropertyInfo info2 in list2)
                    {
                        dictionary.Add(info2.Name, info2.GetValue(value, null));
                    }
                    this.SerializeObject(dictionary);
                }
            }

            private void SerializeString(string str)
            {
                this.builder.Append('"');
                char[] chArray = str.ToCharArray();
                foreach (char ch in chArray)
                {
                    int num2;
                    switch (ch)
                    {
                        case '\b':
                        {
                            this.builder.Append(@"\b");
                            continue;
                        }
                        case '\t':
                        {
                            this.builder.Append(@"\t");
                            continue;
                        }
                        case '\n':
                        {
                            this.builder.Append(@"\n");
                            continue;
                        }
                        case '\f':
                        {
                            this.builder.Append(@"\f");
                            continue;
                        }
                        case '\r':
                        {
                            this.builder.Append(@"\r");
                            continue;
                        }
                        default:
                        {
                            if (ch != '"')
                            {
                                if (ch == '\\')
                                {
                                    break;
                                }
                                goto Label_00F4;
                            }
                            this.builder.Append("\\\"");
                            continue;
                        }
                    }
                    this.builder.Append(@"\\");
                    continue;
                Label_00F4:
                    num2 = Convert.ToInt32(ch);
                    if ((num2 >= 0x20) && (num2 <= 0x7e))
                    {
                        this.builder.Append(ch);
                    }
                    else
                    {
                        this.builder.Append(@"\u");
                        this.builder.Append(num2.ToString("x4"));
                    }
                }
                this.builder.Append('"');
            }

            private void SerializeValue(object value)
            {
                if (value == null)
                {
                    this.builder.Append("null");
                }
                else
                {
                    string str = value as string;
                    if (str != null)
                    {
                        this.SerializeString(str);
                    }
                    else if (value is bool)
                    {
                        this.builder.Append(!((bool) value) ? "false" : "true");
                    }
                    else
                    {
                        IList anArray = value as IList;
                        if (anArray != null)
                        {
                            this.SerializeArray(anArray);
                        }
                        else
                        {
                            IDictionary dictionary = value as IDictionary;
                            if (dictionary != null)
                            {
                                this.SerializeObject(dictionary);
                            }
                            else if (value is char)
                            {
                                this.SerializeString(new string((char) value, 1));
                            }
                            else
                            {
                                this.SerializeOther(value);
                            }
                        }
                    }
                }
            }
        }
    }
}

