namespace NDesk.Options
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    public abstract class Option
    {
        private int count;
        private string description;
        private string[] names;
        private static readonly char[] NameTerminator = new char[] { '=', ':' };
        private string prototype;
        private string[] separators;
        private NDesk.Options.OptionValueType type;

        protected Option(string prototype, string description) : this(prototype, description, 1)
        {
        }

        protected Option(string prototype, string description, int maxValueCount)
        {
            if (prototype == null)
            {
                throw new ArgumentNullException("prototype");
            }
            if (prototype.Length == 0)
            {
                throw new ArgumentException("Cannot be the empty string.", "prototype");
            }
            if (maxValueCount < 0)
            {
                throw new ArgumentOutOfRangeException("maxValueCount");
            }
            this.prototype = prototype;
            char[] separator = new char[] { '|' };
            this.names = prototype.Split(separator);
            this.description = description;
            this.count = maxValueCount;
            this.type = this.ParsePrototype();
            if ((this.count == 0) && (this.type != NDesk.Options.OptionValueType.None))
            {
                throw new ArgumentException("Cannot provide maxValueCount of 0 for OptionValueType.Required or OptionValueType.Optional.", "maxValueCount");
            }
            if ((this.type == NDesk.Options.OptionValueType.None) && (maxValueCount > 1))
            {
                throw new ArgumentException(string.Format("Cannot provide maxValueCount of {0} for OptionValueType.None.", maxValueCount), "maxValueCount");
            }
            if ((Array.IndexOf<string>(this.names, "<>") >= 0) && (((this.names.Length == 1) && (this.type != NDesk.Options.OptionValueType.None)) || ((this.names.Length > 1) && (this.MaxValueCount > 1))))
            {
                throw new ArgumentException("The default option handler '<>' cannot require values.", "prototype");
            }
        }

        private static void AddSeparators(string name, int end, ICollection<string> seps)
        {
            int startIndex = -1;
            for (int i = end + 1; i < name.Length; i++)
            {
                switch (name[i])
                {
                    case '{':
                        if (startIndex != -1)
                        {
                            throw new ArgumentException(string.Format("Ill-formed name/value separator found in \"{0}\".", name), "prototype");
                        }
                        startIndex = i + 1;
                        break;

                    case '}':
                        if (startIndex == -1)
                        {
                            throw new ArgumentException(string.Format("Ill-formed name/value separator found in \"{0}\".", name), "prototype");
                        }
                        seps.Add(name.Substring(startIndex, i - startIndex));
                        startIndex = -1;
                        break;

                    default:
                        if (startIndex == -1)
                        {
                            seps.Add(name[i].ToString());
                        }
                        break;
                }
            }
            if (startIndex != -1)
            {
                throw new ArgumentException(string.Format("Ill-formed name/value separator found in \"{0}\".", name), "prototype");
            }
        }

        public string[] GetNames()
        {
            return (string[]) this.names.Clone();
        }

        public string[] GetValueSeparators()
        {
            if (this.separators == null)
            {
                return new string[0];
            }
            return (string[]) this.separators.Clone();
        }

        public void Invoke(OptionContext c)
        {
            this.OnParseComplete(c);
            c.OptionName = null;
            c.Option = null;
            c.OptionValues.Clear();
        }

        protected abstract void OnParseComplete(OptionContext c);
        protected static T Parse<T>(string value, OptionContext c)
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
            T local = default(T);
            try
            {
                if (value != null)
                {
                    local = (T) converter.ConvertFromString(value);
                }
            }
            catch (Exception exception)
            {
                throw new OptionException(string.Format(c.OptionSet.MessageLocalizer("Could not convert string `{0}' to type {1} for option `{2}'."), value, typeof(T).Name, c.OptionName), c.OptionName, exception);
            }
            return local;
        }

        private NDesk.Options.OptionValueType ParsePrototype()
        {
            char ch = '\0';
            List<string> seps = new List<string>();
            for (int i = 0; i < this.names.Length; i++)
            {
                string name = this.names[i];
                if (name.Length == 0)
                {
                    throw new ArgumentException("Empty option names are not supported.", "prototype");
                }
                int length = name.IndexOfAny(NameTerminator);
                if (length != -1)
                {
                    this.names[i] = name.Substring(0, length);
                    if ((ch != '\0') && (ch != name[length]))
                    {
                        throw new ArgumentException(string.Format("Conflicting option types: '{0}' vs. '{1}'.", ch, name[length]), "prototype");
                    }
                    ch = name[length];
                    AddSeparators(name, length, seps);
                }
            }
            if (ch == '\0')
            {
                return NDesk.Options.OptionValueType.None;
            }
            if ((this.count <= 1) && (seps.Count != 0))
            {
                throw new ArgumentException(string.Format("Cannot provide key/value separators for Options taking {0} value(s).", this.count), "prototype");
            }
            if (this.count > 1)
            {
                if (seps.Count == 0)
                {
                    this.separators = new string[] { ":", "=" };
                }
                else if ((seps.Count == 1) && (seps[0].Length == 0))
                {
                    this.separators = null;
                }
                else
                {
                    this.separators = seps.ToArray();
                }
            }
            return ((ch != '=') ? NDesk.Options.OptionValueType.Optional : NDesk.Options.OptionValueType.Required);
        }

        public override string ToString()
        {
            return this.Prototype;
        }

        public string Description
        {
            get
            {
                return this.description;
            }
        }

        public int MaxValueCount
        {
            get
            {
                return this.count;
            }
        }

        internal string[] Names
        {
            get
            {
                return this.names;
            }
        }

        public NDesk.Options.OptionValueType OptionValueType
        {
            get
            {
                return this.type;
            }
        }

        public string Prototype
        {
            get
            {
                return this.prototype;
            }
        }

        internal string[] ValueSeparators
        {
            get
            {
                return this.separators;
            }
        }
    }
}

