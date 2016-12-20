namespace Unity.IL2CPP.Common
{
    using System;
    using System.Runtime.InteropServices;

    public class TypeNameParser
    {
        private bool _acceptAssemblyName;
        private readonly char[] _data;
        private readonly int _end;
        private TypeNameParseInfo _info;
        private readonly bool _isNested;
        private int _p;

        private TypeNameParser(char[] data, int begin, int end, TypeNameParseInfo info, bool isNested)
        {
            this._data = data;
            this._p = begin;
            this._end = end;
            this._info = info;
            this._isNested = isNested;
            this._acceptAssemblyName = true;
        }

        private void ConsumeAssemblyIdentifier()
        {
            do
            {
                switch (this._data[this._p])
                {
                    case '[':
                    case ']':
                    case '`':
                        return;

                    case '\\':
                        this.Next(false);
                        break;

                    case '*':
                    case '+':
                    case ',':
                    case '"':
                    case '&':
                    case '=':
                        return;
                }
            }
            while (this.Next(false));
        }

        private void ConsumeIdentifier()
        {
            do
            {
                switch (this._data[this._p])
                {
                    case '*':
                    case '+':
                    case ',':
                    case '.':
                        return;

                    case '[':
                    case ']':
                    case '`':
                        return;

                    case '\\':
                        this.Next(false);
                        break;

                    case '"':
                    case '&':
                    case '=':
                        return;
                }
            }
            while (this.Next(false));
        }

        private bool ConsumeNumber(ref int value)
        {
            if (!char.IsDigit(this._data[this._p]))
            {
                return false;
            }
            int startIndex = this._p;
            while (char.IsDigit(this._data[this._p]))
            {
                if (!this.Next(false))
                {
                    break;
                }
            }
            string s = new string(this._data, startIndex, this._p - startIndex);
            value = int.Parse(s);
            return true;
        }

        private void ConsumePropertyIdentifier()
        {
            char ch;
            do
            {
                ch = this._data[this._p];
            }
            while ((ch != '=') && this.Next(false));
        }

        private void ConsumePropertyValue()
        {
            do
            {
                switch (this._data[this._p])
                {
                    case ',':
                    case ']':
                        return;
                }
            }
            while (this.Next(false));
        }

        private bool CurrentIs(char v)
        {
            if (this.IsEOL)
            {
                return false;
            }
            return (this._data[this._p] == v);
        }

        private void InitializeParser()
        {
            this.SkipWhites();
        }

        private bool Next([Optional, DefaultParameterValue(false)] bool skipWhites)
        {
            this._p++;
            if (skipWhites)
            {
                this.SkipWhites();
            }
            return !this.IsEOL;
        }

        private bool NextWillBe(char v, [Optional, DefaultParameterValue(false)] bool skipWhites)
        {
            if (this.IsEOL)
            {
                return false;
            }
            int num = 1;
            if ((this._p + num) >= this._end)
            {
                return false;
            }
            if (skipWhites)
            {
                while ((this._data[this._p + num] == ' ') || (this._data[this._p + num] == '\t'))
                {
                    num++;
                    if ((this._p + num) >= this._end)
                    {
                        return false;
                    }
                }
            }
            return (this._data[this._p + num] == v);
        }

        private bool Parse([Optional, DefaultParameterValue(true)] bool acceptAssemblyName)
        {
            this._acceptAssemblyName = acceptAssemblyName;
            int arity = 0;
            this.InitializeParser();
            if (this.IsEOL)
            {
                return false;
            }
            if (!this.ParseTypeName(ref arity))
            {
                return false;
            }
            if (!this.ParseNestedTypeOptional(ref arity))
            {
                return false;
            }
            if (!this.ParseTypeArgumentsOptional(ref arity))
            {
                return false;
            }
            if (!this.ParsePointerModifiersOptional())
            {
                return false;
            }
            if (!this.ParseArrayModifierOptional())
            {
                return false;
            }
            if (!this.ParseByRefModifiersOptional())
            {
                return false;
            }
            if (!this.ParseAssemblyNameOptional())
            {
                return false;
            }
            return ((this._p == this._end) || this._isNested);
        }

        public static TypeNameParseInfo Parse(string name)
        {
            TypeNameParseInfo info = new TypeNameParseInfo();
            char[] data = name.ToCharArray();
            TypeNameParser parser = new TypeNameParser(data, 0, data.Length, info, false);
            return (parser.Parse(true) ? info : null);
        }

        private bool ParseArrayModifierOptional()
        {
            this.SkipWhites();
            if (this.IsEOL)
            {
                return true;
            }
            if (!this.CurrentIs('['))
            {
                return true;
            }
            if ((!this.NextWillBe(']', true) && !this.NextWillBe(',', true)) && !this.NextWillBe('*', true))
            {
                return true;
            }
            if (!this.Next(true))
            {
                return false;
            }
            int item = 1;
        Label_0073:
            if (this.CurrentIs(']'))
            {
                this.Next(true);
            }
            else
            {
                if (this.CurrentIs(','))
                {
                    item++;
                    if (!this.Next(true))
                    {
                        return false;
                    }
                    goto Label_0073;
                }
                if (this.CurrentIs('*'))
                {
                    this._info.Modifiers.Add(-2);
                    if (!this.Next(true))
                    {
                        return false;
                    }
                    goto Label_0073;
                }
                return false;
            }
            this._info.Modifiers.Add(item);
            return this.ParseArrayModifierOptional();
        }

        private bool ParseAssemblyName()
        {
            int startIndex = this._p;
            this.ConsumeAssemblyIdentifier();
            this._info.Assembly.Name = new string(this._data, startIndex, this._p - startIndex);
            this.SkipWhites();
            return this.ParsePropertiesOptional();
        }

        private bool ParseAssemblyNameOptional()
        {
            if (!this._acceptAssemblyName)
            {
                return true;
            }
            if (!this.CurrentIs(','))
            {
                return true;
            }
            if (!this.Next(false))
            {
                return false;
            }
            this.SkipWhites();
            return this.ParseAssemblyName();
        }

        private bool ParseByRefModifiersOptional()
        {
            if (!this.IsEOL)
            {
                if (!this.CurrentIs('&'))
                {
                    return true;
                }
                if (this._info.Modifiers.Contains(0))
                {
                    return false;
                }
                this._info.Modifiers.Add(0);
                this.Next(true);
            }
            return true;
        }

        private bool ParseNestedTypeOptional(ref int arity)
        {
            while (this.CurrentIs('+'))
            {
                if (!this.Next(false))
                {
                    return false;
                }
                int num = 0;
                int startIndex = this._p;
                this.ConsumeIdentifier();
                if (this.CurrentIs('`'))
                {
                    if (!this.Next(false))
                    {
                        return false;
                    }
                    if (!this.ConsumeNumber(ref num))
                    {
                        return false;
                    }
                    arity += num;
                }
                this._info.Nested.Add(new string(this._data, startIndex, this._p - startIndex));
                this._info.Arities.Add(num);
            }
            return true;
        }

        private bool ParsePointerModifiersOptional()
        {
            if (!this.IsEOL)
            {
                while (this.CurrentIs('*'))
                {
                    this._info.Modifiers.Add(-1);
                    if (!this.Next(true))
                    {
                        break;
                    }
                }
            }
            return true;
        }

        private bool ParsePropertiesOptional()
        {
            while (this.CurrentIs(','))
            {
                string str2;
                if (this.Next(true))
                {
                    int startIndex = this._p;
                    this.ConsumePropertyIdentifier();
                    string str = new string(this._data, startIndex, this._p - startIndex);
                    if (!this.CurrentIs('='))
                    {
                        return false;
                    }
                    if (!this.Next(false))
                    {
                        return false;
                    }
                    startIndex = this._p;
                    this.ConsumePropertyValue();
                    str2 = new string(this._data, startIndex, this._p - startIndex);
                    if (str == null)
                    {
                        goto Label_017E;
                    }
                    if (str != "Version")
                    {
                        if (str == "PublicKey")
                        {
                            goto Label_00ED;
                        }
                        if (str == "PublicKeyToken")
                        {
                            goto Label_0113;
                        }
                        if (str == "Culture")
                        {
                            goto Label_0168;
                        }
                        goto Label_017E;
                    }
                    if (ParseVersion(str2, ref this._info))
                    {
                        continue;
                    }
                }
                return false;
            Label_00ED:
                if (str2 != "null")
                {
                    this._info.Assembly.PublicKey = str2;
                }
                continue;
            Label_0113:
                if (str2 != "null")
                {
                    if (str2.Length != 0x10)
                    {
                        return false;
                    }
                    char[] sourceArray = str2.ToCharArray();
                    Array.Copy(sourceArray, this._info.Assembly.PublicKeyToken, Math.Min(0x11, sourceArray.Length));
                }
                continue;
            Label_0168:
                this._info.Assembly.Culture = str2;
                continue;
            Label_017E:
                return false;
            }
            return true;
        }

        private bool ParseTypeArgumentsOptional(ref int arity)
        {
            bool flag2;
            this.SkipWhites();
            if (this.IsEOL)
            {
                return true;
            }
            if (!this.CurrentIs('['))
            {
                return true;
            }
            if ((this.NextWillBe(']', true) || this.NextWillBe(',', true)) || this.NextWillBe('*', true))
            {
                return true;
            }
            if (!this.Next(true))
            {
                return false;
            }
            this._info.TypeArguments.Capacity = arity;
        Label_0083:
            flag2 = false;
            if (this.CurrentIs('['))
            {
                flag2 = true;
                if (!this.Next(true))
                {
                    return false;
                }
            }
            TypeNameParseInfo info = new TypeNameParseInfo();
            TypeNameParser parser = new TypeNameParser(this._data, this._p, this._end, info, true);
            if (!parser.Parse(flag2))
            {
                return false;
            }
            this._p = parser._p;
            this._info.TypeArguments.Add(info);
            this.SkipWhites();
            if (this.IsEOL)
            {
                return false;
            }
            if (flag2)
            {
                if (!this.CurrentIs(']'))
                {
                    return false;
                }
                if (!this.Next(true))
                {
                    return false;
                }
            }
            if (!this.CurrentIs(']'))
            {
                if (!this.CurrentIs(','))
                {
                    return false;
                }
                if (!this.Next(true))
                {
                    return false;
                }
                goto Label_0083;
            }
            if (this._info.TypeArguments.Count != arity)
            {
                return false;
            }
            this.Next(true);
            return true;
        }

        private bool ParseTypeName(ref int arity)
        {
            int startIndex = this._p;
            int num2 = this._end;
            while (true)
            {
                this.ConsumeIdentifier();
                if (!this.CurrentIs('.'))
                {
                    break;
                }
                num2 = this._p;
                if (!this.Next(false))
                {
                    return false;
                }
            }
            if (this.CurrentIs('`'))
            {
                if (!this.Next(false))
                {
                    return false;
                }
                if (!this.ConsumeNumber(ref arity))
                {
                    return false;
                }
            }
            if (num2 == this._end)
            {
                this._info.Name = new string(this._data, startIndex, this._p - startIndex);
            }
            else
            {
                this._info.Namespace = new string(this._data, startIndex, num2 - startIndex);
                this._info.Name = new string(this._data, num2 + 1, (this._p - num2) - 1);
            }
            this._info.Arities.Add(arity);
            return true;
        }

        private static bool ParseVersion(string version, ref TypeNameParseInfo info)
        {
            ushort num;
            ushort num2;
            ushort num3;
            ushort num4;
            if (!ParseVersion(version, out num, out num2, out num3, out num4))
            {
                return false;
            }
            info.Assembly.Major = num;
            info.Assembly.Minor = num2;
            info.Assembly.Build = num3;
            info.Assembly.Revision = num4;
            return true;
        }

        private static bool ParseVersion(string version, out ushort major, out ushort minor, out ushort build, out ushort revision)
        {
            major = 0;
            minor = 0;
            build = 0;
            revision = 0;
            int startIndex = 0;
            int index = version.IndexOf('.');
            if (index == -1)
            {
                return false;
            }
            major = ushort.Parse(version.Substring(startIndex, index - startIndex));
            startIndex = index + 1;
            index = version.IndexOf('.', startIndex);
            if (index == -1)
            {
                return false;
            }
            minor = ushort.Parse(version.Substring(startIndex, index - startIndex));
            startIndex = index + 1;
            index = version.IndexOf('.', startIndex);
            if (index == -1)
            {
                return false;
            }
            build = ushort.Parse(version.Substring(startIndex, index - startIndex));
            startIndex = index + 1;
            revision = ushort.Parse(version.Substring(startIndex, version.Length - startIndex));
            return true;
        }

        private void SkipWhites()
        {
            while ((this.CurrentIs(' ') || this.CurrentIs('\t')) && !this.IsEOL)
            {
                this._p++;
            }
        }

        private bool IsEOL
        {
            get
            {
                return (this._p >= this._end);
            }
        }
    }
}

