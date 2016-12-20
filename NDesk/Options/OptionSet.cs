namespace NDesk.Options
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Text.RegularExpressions;
    using Unity.IL2CPP.Portability;

    public class OptionSet : KeyedCollection<string, Option>
    {
        [CompilerGenerated]
        private static ConverterPortable<string, string> <>f__am$cache0;
        private ConverterPortable<string, string> localizer;
        private const int OptionWidth = 0x1d;
        private readonly Regex ValueOption;

        public OptionSet() : this(<>f__am$cache0)
        {
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = new ConverterPortable<string, string>(OptionSet.<OptionSet>m__0);
            }
        }

        public OptionSet(ConverterPortable<string, string> localizer)
        {
            this.ValueOption = new Regex("^(?<flag>--|-|/)(?<name>[^:=]+)((?<sep>[:=])(?<value>.*))?$");
            this.localizer = localizer;
        }

        [CompilerGenerated]
        private static string <OptionSet>m__0(string f)
        {
            return f;
        }

        public OptionSet Add(Option option)
        {
            base.Add(option);
            return this;
        }

        public OptionSet Add(string prototype, OptionAction<string, string> action)
        {
            return this.Add(prototype, null, action);
        }

        public OptionSet Add<TKey, TValue>(string prototype, OptionAction<TKey, TValue> action)
        {
            return this.Add<TKey, TValue>(prototype, null, action);
        }

        public OptionSet Add(string prototype, Action<string> action)
        {
            return this.Add(prototype, null, action);
        }

        public OptionSet Add<T>(string prototype, Action<T> action)
        {
            return this.Add<T>(prototype, null, action);
        }

        public OptionSet Add(string prototype, string description, OptionAction<string, string> action)
        {
            <Add>c__AnonStorey1 storey = new <Add>c__AnonStorey1 {
                action = action
            };
            if (storey.action == null)
            {
                throw new ArgumentNullException("action");
            }
            Option item = new ActionOption(prototype, description, 2, new Action<OptionValueCollection>(storey.<>m__0));
            base.Add(item);
            return this;
        }

        public OptionSet Add<TKey, TValue>(string prototype, string description, OptionAction<TKey, TValue> action)
        {
            return this.Add(new ActionOption<TKey, TValue>(prototype, description, action));
        }

        public OptionSet Add(string prototype, string description, Action<string> action)
        {
            <Add>c__AnonStorey0 storey = new <Add>c__AnonStorey0 {
                action = action
            };
            if (storey.action == null)
            {
                throw new ArgumentNullException("action");
            }
            Option item = new ActionOption(prototype, description, 1, new Action<OptionValueCollection>(storey.<>m__0));
            base.Add(item);
            return this;
        }

        public OptionSet Add<T>(string prototype, string description, Action<T> action)
        {
            return this.Add(new ActionOption<T>(prototype, description, action));
        }

        private void AddImpl(Option option)
        {
            if (option == null)
            {
                throw new ArgumentNullException("option");
            }
            List<string> list = new List<string>(option.Names.Length);
            try
            {
                for (int i = 1; i < option.Names.Length; i++)
                {
                    base.Dictionary.Add(option.Names[i], option);
                    list.Add(option.Names[i]);
                }
            }
            catch (Exception)
            {
                foreach (string str in list)
                {
                    base.Dictionary.Remove(str);
                }
                throw;
            }
        }

        protected virtual OptionContext CreateOptionContext()
        {
            return new OptionContext(this);
        }

        private static string GetArgumentName(int index, int maxIndex, string description)
        {
            if (description != null)
            {
                string[] strArray;
                if (maxIndex == 1)
                {
                    strArray = new string[] { "{0:", "{" };
                }
                else
                {
                    strArray = new string[] { "{" + index + ":" };
                }
                for (int i = 0; i < strArray.Length; i++)
                {
                    int num2;
                    int startIndex = 0;
                    do
                    {
                        num2 = description.IndexOf(strArray[i], startIndex);
                    }
                    while (((num2 >= 0) && (startIndex != 0)) && (description[startIndex++ - 1] == '{'));
                    if (num2 != -1)
                    {
                        int num4 = description.IndexOf("}", num2);
                        if (num4 != -1)
                        {
                            return description.Substring(num2 + strArray[i].Length, (num4 - num2) - strArray[i].Length);
                        }
                    }
                }
            }
            return ((maxIndex != 1) ? ("VALUE" + (index + 1)) : "VALUE");
        }

        private static string GetDescription(string description)
        {
            if (description == null)
            {
                return string.Empty;
            }
            StringBuilder builder = new StringBuilder(description.Length);
            int startIndex = -1;
            for (int i = 0; i < description.Length; i++)
            {
                switch (description[i])
                {
                    case '{':
                    {
                        if (i != startIndex)
                        {
                            break;
                        }
                        builder.Append('{');
                        startIndex = -1;
                        continue;
                    }
                    case '}':
                        if (startIndex < 0)
                        {
                            if (((i + 1) == description.Length) || (description[i + 1] != '}'))
                            {
                                throw new InvalidOperationException("Invalid option description: " + description);
                            }
                            goto Label_00B5;
                        }
                        goto Label_00CB;

                    case ':':
                        goto Label_00E5;

                    default:
                        goto Label_00FA;
                }
                if (startIndex < 0)
                {
                    startIndex = i + 1;
                }
                continue;
            Label_00B5:
                i++;
                builder.Append("}");
                continue;
            Label_00CB:
                builder.Append(description.Substring(startIndex, i - startIndex));
                startIndex = -1;
                continue;
            Label_00E5:
                if (startIndex >= 0)
                {
                    startIndex = i + 1;
                    continue;
                }
            Label_00FA:
                if (startIndex < 0)
                {
                    builder.Append(description[i]);
                }
            }
            return builder.ToString();
        }

        protected override string GetKeyForItem(Option item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("option");
            }
            if ((item.Names == null) || (item.Names.Length <= 0))
            {
                throw new InvalidOperationException("Option has no names!");
            }
            return item.Names[0];
        }

        private static int GetLineEnd(int start, int length, string description)
        {
            int num = Math.Min(start + length, description.Length);
            int num2 = -1;
            for (int i = start; i < num; i++)
            {
                switch (description[i])
                {
                    case '\t':
                    case '\v':
                    case ',':
                    case '-':
                    case '.':
                    case ' ':
                    case ';':
                        num2 = i;
                        break;

                    case '\n':
                        return i;
                }
            }
            if ((num2 == -1) || (num == description.Length))
            {
                return num;
            }
            return num2;
        }

        private static List<string> GetLines(string description)
        {
            int num3;
            List<string> list = new List<string>();
            if (string.IsNullOrEmpty(description))
            {
                list.Add(string.Empty);
                return list;
            }
            int length = 0x31;
            int start = 0;
            do
            {
                num3 = GetLineEnd(start, length, description);
                bool flag = false;
                if (num3 < description.Length)
                {
                    char c = description[num3];
                    if ((c == '-') || (char.IsWhiteSpace(c) && (c != '\n')))
                    {
                        num3++;
                    }
                    else if (c != '\n')
                    {
                        flag = true;
                        num3--;
                    }
                }
                list.Add(description.Substring(start, num3 - start));
                if (flag)
                {
                    List<string> list3;
                    int num4;
                    (list3 = list)[num4 = list.Count - 1] = list3[num4] + "-";
                }
                start = num3;
                if ((start < description.Length) && (description[start] == '\n'))
                {
                    start++;
                }
            }
            while (num3 < description.Length);
            return list;
        }

        private static int GetNextOptionIndex(string[] names, int i)
        {
            while ((i < names.Length) && (names[i] == "<>"))
            {
                i++;
            }
            return i;
        }

        [Obsolete("Use KeyedCollection.this[string]")]
        protected Option GetOptionForName(string option)
        {
            if (option == null)
            {
                throw new ArgumentNullException("option");
            }
            try
            {
                return base[option];
            }
            catch (KeyNotFoundException)
            {
                return null;
            }
        }

        protected bool GetOptionParts(string argument, out string flag, out string name, out string sep, out string value)
        {
            string str;
            if (argument == null)
            {
                throw new ArgumentNullException("argument");
            }
            value = (string) (str = null);
            sep = str = str;
            flag = name = str;
            Match match = this.ValueOption.Match(argument);
            if (!match.Success)
            {
                return false;
            }
            flag = match.Groups["flag"].Value;
            name = match.Groups["name"].Value;
            if (match.Groups["sep"].Success && match.Groups["value"].Success)
            {
                sep = match.Groups["sep"].Value;
                value = match.Groups["value"].Value;
            }
            return true;
        }

        protected override void InsertItem(int index, Option item)
        {
            base.InsertItem(index, item);
            this.AddImpl(item);
        }

        private static void Invoke(OptionContext c, string name, string value, Option option)
        {
            c.OptionName = name;
            c.Option = option;
            c.OptionValues.Add(value);
            option.Invoke(c);
        }

        public List<string> Parse(IEnumerable<string> arguments)
        {
            OptionContext c = this.CreateOptionContext();
            c.OptionIndex = -1;
            bool flag = true;
            List<string> extra = new List<string>();
            Option def = !base.Contains("<>") ? null : base["<>"];
            foreach (string str in arguments)
            {
                c.OptionIndex++;
                if (str == "--")
                {
                    flag = false;
                }
                else if (!flag)
                {
                    Unprocessed(extra, def, c, str);
                }
                else if (!this.Parse(str, c))
                {
                    Unprocessed(extra, def, c, str);
                }
            }
            if (c.Option != null)
            {
                c.Option.Invoke(c);
            }
            return extra;
        }

        protected virtual bool Parse(string argument, OptionContext context)
        {
            string str;
            string str2;
            string str3;
            string str4;
            if (context.Option != null)
            {
                this.ParseValue(argument, context);
                return true;
            }
            if (!this.GetOptionParts(argument, out str, out str2, out str3, out str4))
            {
                return false;
            }
            if (!base.Contains(str2))
            {
                if (this.ParseBool(argument, str2, context))
                {
                    return true;
                }
                string[] textArray1 = new string[] { str2 + str3 + str4 };
                return this.ParseBundledValue(str, string.Concat(textArray1), context);
            }
            Option option = base[str2];
            context.OptionName = str + str2;
            context.Option = option;
            switch (option.OptionValueType)
            {
                case OptionValueType.None:
                    context.OptionValues.Add(str2);
                    context.Option.Invoke(context);
                    break;

                case OptionValueType.Optional:
                case OptionValueType.Required:
                    this.ParseValue(str4, context);
                    break;
            }
            return true;
        }

        private bool ParseBool(string option, string n, OptionContext c)
        {
            string str;
            if (((n.Length >= 1) && ((n[n.Length - 1] == '+') || (n[n.Length - 1] == '-'))) && base.Contains(str = n.Substring(0, n.Length - 1)))
            {
                Option option2 = base[str];
                string item = (n[n.Length - 1] != '+') ? null : option;
                c.OptionName = option;
                c.Option = option2;
                c.OptionValues.Add(item);
                option2.Invoke(c);
                return true;
            }
            return false;
        }

        private bool ParseBundledValue(string f, string n, OptionContext c)
        {
            if (f != "-")
            {
                return false;
            }
            for (int i = 0; i < n.Length; i++)
            {
                string optionName = f + n[i].ToString();
                string key = n[i].ToString();
                if (!base.Contains(key))
                {
                    if (i != 0)
                    {
                        throw new OptionException(string.Format(this.localizer("Cannot bundle unregistered option '{0}'."), optionName), optionName);
                    }
                    return false;
                }
                Option option = base[key];
                switch (option.OptionValueType)
                {
                    case OptionValueType.None:
                        Invoke(c, optionName, n, option);
                        break;

                    case OptionValueType.Optional:
                    case OptionValueType.Required:
                    {
                        string str3 = n.Substring(i + 1);
                        c.Option = option;
                        c.OptionName = optionName;
                        this.ParseValue((str3.Length == 0) ? null : str3, c);
                        return true;
                    }
                    default:
                        throw new InvalidOperationException("Unknown OptionValueType: " + option.OptionValueType);
                }
            }
            return true;
        }

        private void ParseValue(string option, OptionContext c)
        {
            if (option != null)
            {
                foreach (string str in (c.Option.ValueSeparators == null) ? new string[] { option } : option.Split(c.Option.ValueSeparators, StringSplitOptions.None))
                {
                    char[] trimChars = new char[] { '"' };
                    c.OptionValues.Add(str.Trim(trimChars));
                }
            }
            if ((c.OptionValues.Count == c.Option.MaxValueCount) || (c.Option.OptionValueType == OptionValueType.Optional))
            {
                c.Option.Invoke(c);
            }
            else if (c.OptionValues.Count > c.Option.MaxValueCount)
            {
                throw new OptionException(this.localizer(string.Format("Error: Found {0} option values when expecting {1}.", c.OptionValues.Count, c.Option.MaxValueCount)), c.OptionName);
            }
        }

        protected override void RemoveItem(int index)
        {
            base.RemoveItem(index);
            Option option = base.Items[index];
            for (int i = 1; i < option.Names.Length; i++)
            {
                base.Dictionary.Remove(option.Names[i]);
            }
        }

        protected override void SetItem(int index, Option item)
        {
            base.SetItem(index, item);
            this.RemoveItem(index);
            this.AddImpl(item);
        }

        private static bool Unprocessed(ICollection<string> extra, Option def, OptionContext c, string argument)
        {
            if (def == null)
            {
                extra.Add(argument);
                return false;
            }
            c.OptionValues.Add(argument);
            c.Option = def;
            c.Option.Invoke(c);
            return false;
        }

        private static void Write(TextWriter o, ref int n, string s)
        {
            n += s.Length;
            o.Write(s);
        }

        public void WriteOptionDescriptions(TextWriter o)
        {
            foreach (Option option in this)
            {
                int written = 0;
                if (this.WriteOptionPrototype(o, option, ref written))
                {
                    if (written < 0x1d)
                    {
                        o.Write(new string(' ', 0x1d - written));
                    }
                    else
                    {
                        o.WriteLine();
                        o.Write(new string(' ', 0x1d));
                    }
                    List<string> lines = GetLines(this.localizer(GetDescription(option.Description)));
                    o.WriteLine(lines[0]);
                    string str = new string(' ', 0x1f);
                    for (int i = 1; i < lines.Count; i++)
                    {
                        o.Write(str);
                        o.WriteLine(lines[i]);
                    }
                }
            }
        }

        private bool WriteOptionPrototype(TextWriter o, Option p, ref int written)
        {
            string[] names = p.Names;
            int nextOptionIndex = GetNextOptionIndex(names, 0);
            if (nextOptionIndex == names.Length)
            {
                return false;
            }
            if (names[nextOptionIndex].Length == 1)
            {
                Write(o, ref written, "  -");
                Write(o, ref written, names[0]);
            }
            else
            {
                Write(o, ref written, "      --");
                Write(o, ref written, names[0]);
            }
            for (nextOptionIndex = GetNextOptionIndex(names, nextOptionIndex + 1); nextOptionIndex < names.Length; nextOptionIndex = GetNextOptionIndex(names, nextOptionIndex + 1))
            {
                Write(o, ref written, ", ");
                Write(o, ref written, (names[nextOptionIndex].Length != 1) ? "--" : "-");
                Write(o, ref written, names[nextOptionIndex]);
            }
            if ((p.OptionValueType == OptionValueType.Optional) || (p.OptionValueType == OptionValueType.Required))
            {
                if (p.OptionValueType == OptionValueType.Optional)
                {
                    Write(o, ref written, this.localizer("["));
                }
                Write(o, ref written, this.localizer("=" + GetArgumentName(0, p.MaxValueCount, p.Description)));
                string str = ((p.ValueSeparators == null) || (p.ValueSeparators.Length <= 0)) ? " " : p.ValueSeparators[0];
                for (int i = 1; i < p.MaxValueCount; i++)
                {
                    Write(o, ref written, this.localizer(str + GetArgumentName(i, p.MaxValueCount, p.Description)));
                }
                if (p.OptionValueType == OptionValueType.Optional)
                {
                    Write(o, ref written, this.localizer("]"));
                }
            }
            return true;
        }

        public ConverterPortable<string, string> MessageLocalizer
        {
            get
            {
                return this.localizer;
            }
        }

        [CompilerGenerated]
        private sealed class <Add>c__AnonStorey0
        {
            internal Action<string> action;

            internal void <>m__0(OptionValueCollection v)
            {
                this.action(v[0]);
            }
        }

        [CompilerGenerated]
        private sealed class <Add>c__AnonStorey1
        {
            internal OptionAction<string, string> action;

            internal void <>m__0(OptionValueCollection v)
            {
                this.action(v[0], v[1]);
            }
        }

        private sealed class ActionOption : Option
        {
            private Action<OptionValueCollection> action;

            public ActionOption(string prototype, string description, int count, Action<OptionValueCollection> action) : base(prototype, description, count)
            {
                if (action == null)
                {
                    throw new ArgumentNullException("action");
                }
                this.action = action;
            }

            protected override void OnParseComplete(OptionContext c)
            {
                this.action(c.OptionValues);
            }
        }

        private sealed class ActionOption<T> : Option
        {
            private Action<T> action;

            public ActionOption(string prototype, string description, Action<T> action) : base(prototype, description, 1)
            {
                if (action == null)
                {
                    throw new ArgumentNullException("action");
                }
                this.action = action;
            }

            protected override void OnParseComplete(OptionContext c)
            {
                this.action(Option.Parse<T>(c.OptionValues[0], c));
            }
        }

        private sealed class ActionOption<TKey, TValue> : Option
        {
            private OptionAction<TKey, TValue> action;

            public ActionOption(string prototype, string description, OptionAction<TKey, TValue> action) : base(prototype, description, 2)
            {
                if (action == null)
                {
                    throw new ArgumentNullException("action");
                }
                this.action = action;
            }

            protected override void OnParseComplete(OptionContext c)
            {
                this.action(Option.Parse<TKey>(c.OptionValues[0], c), Option.Parse<TValue>(c.OptionValues[1], c));
            }
        }
    }
}

