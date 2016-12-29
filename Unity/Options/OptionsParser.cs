namespace Unity.Options
{
    using NDesk.Options;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text.RegularExpressions;
    using System.Threading;
    using Unity.IL2CPP.Portability;

    public sealed class OptionsParser
    {
        private readonly List<Type> _types = new List<Type>();
        [CompilerGenerated]
        private static Func<string, bool> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<Match, string> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<string, string, string> <>f__am$cache2;
        [CompilerGenerated]
        private static Func<Type, bool> <>f__mg$cache0;
        public const int HelpOutputColumnPadding = 50;
        private static readonly Regex NameBuilder = new Regex("([A-Z][a-z_0-9]*)");

        internal OptionsParser()
        {
        }

        private static Action<string> ActionFor(ProgramOptionsAttribute options, FieldInfo field)
        {
            <ActionFor>c__AnonStorey1 storey = new <ActionFor>c__AnonStorey1 {
                field = field,
                options = options
            };
            if (storey.field.FieldType.IsArray)
            {
                return new Action<string>(storey.<>m__0);
            }
            if (IsListField(storey.field))
            {
                return new Action<string>(storey.<>m__1);
            }
            if (storey.field.FieldType == typeof(bool))
            {
                return new Action<string>(storey.<>m__2);
            }
            return new Action<string>(storey.<>m__3);
        }

        internal void AddType(Type type)
        {
            this._types.Add(type);
        }

        private static string DescriptionFor(FieldInfo field) => 
            "";

        public static void DisplayHelp(TextWriter writer, Type type)
        {
            Type[] types = new Type[] { type };
            DisplayHelp(writer, types);
        }

        public static void DisplayHelp(TextWriter writer, Type[] types)
        {
            writer.WriteLine();
            writer.WriteLine("Options:");
            Dictionary<string, HelpInformation> dictionary = ParseHelpTable(types);
            foreach (KeyValuePair<string, HelpInformation> pair in dictionary)
            {
                if (pair.Value.HasSummary)
                {
                    string str;
                    if (pair.Value.FieldInfo.FieldType == typeof(bool))
                    {
                        str = $"  {pair.Key}";
                    }
                    else if (pair.Value.FieldInfo.FieldType.IsArray || IsListField(pair.Value.FieldInfo))
                    {
                        str = string.Format("  {0}=<{1},{1},..>", pair.Key, !pair.Value.HasCustomValueDescription ? "value" : pair.Value.CustomValueDescription);
                    }
                    else
                    {
                        str = $"  {pair.Key}=<{!pair.Value.HasCustomValueDescription ? "value" : pair.Value.CustomValueDescription}>";
                    }
                    if (str.Length > 50)
                    {
                        throw new InvalidOperationException($"Option to long for current padding : {pair.Key}, shorten name/value or increase padding if necessary. Over by {str.Length - 50}");
                    }
                    str = str.PadRight(50);
                    writer.WriteLine("{0}{1}", str, pair.Value.Summary);
                }
            }
        }

        public static void DisplayHelp(Assembly assembly, bool includeReferencedAssemblies = true)
        {
            DisplayHelp(Console.Out, LoadOptionTypesFromAssembly(assembly, includeReferencedAssemblies));
        }

        public static void DisplayHelp(TextWriter writer, Assembly assembly, bool includeReferencedAssemblies = true)
        {
            DisplayHelp(writer, LoadOptionTypesFromAssembly(assembly, includeReferencedAssemblies));
        }

        private void ExtendOptionSet(OptionSet optionSet, Type type)
        {
            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Static);
            foreach (FieldInfo info in fields)
            {
                ProgramOptionsAttribute options = (ProgramOptionsAttribute) type.GetCustomAttributesPortable(typeof(ProgramOptionsAttribute), false).First<object>();
                foreach (string str in this.OptionNamesFor(options, info))
                {
                    optionSet.Add(str, DescriptionFor(info), ActionFor(options, info));
                }
            }
        }

        internal static bool HasProgramOptionsAttribute(Type type) => 
            type.GetCustomAttributesPortable(typeof(ProgramOptionsAttribute), false).Any<object>();

        public static bool HelpRequested(string[] commandLine)
        {
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = new Func<string, bool>(null, (IntPtr) <HelpRequested>m__0);
            }
            return (commandLine.Count<string>(<>f__am$cache0) > 0);
        }

        private static bool IsListField(FieldInfo field) => 
            (field.FieldType.IsGenericTypePortable() && field.FieldType.GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>)));

        private static Type[] LoadOptionTypesFromAssembly(Assembly assembly, bool includeReferencedAssemblies)
        {
            List<Type> list = new List<Type>();
            Stack<Assembly> stack = new Stack<Assembly>();
            HashSet<AssemblyName> set = new HashSet<AssemblyName>(new AssemblyNameComparer());
            stack.Push(assembly);
            while (stack.Count > 0)
            {
                Assembly assembly2 = stack.Pop();
                if (set.Add(assembly2.GetName()))
                {
                    if (<>f__mg$cache0 == null)
                    {
                        <>f__mg$cache0 = new Func<Type, bool>(null, (IntPtr) HasProgramOptionsAttribute);
                    }
                    list.AddRange(assembly2.GetTypesPortable().Where<Type>(<>f__mg$cache0));
                    if (includeReferencedAssemblies)
                    {
                        foreach (AssemblyName name in assembly2.GetReferencedAssembliesPortable())
                        {
                            if (((((name.Name != "mscorlib") && !name.Name.StartsWith("System")) && !name.Name.StartsWith("Mono.Cecil")) && (name.Name != "Unity.IL2CPP.RuntimeServices")) && !set.Contains(name))
                            {
                                try
                                {
                                    Assembly item = Assembly.Load(name);
                                    stack.Push(item);
                                }
                                catch (BadImageFormatException)
                                {
                                }
                                catch (FileLoadException)
                                {
                                }
                            }
                        }
                    }
                }
            }
            return list.ToArray();
        }

        private static string NormalizeName(string name)
        {
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = new Func<Match, string>(null, (IntPtr) <NormalizeName>m__1);
            }
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = new Func<string, string, string>(null, (IntPtr) <NormalizeName>m__2);
            }
            return NameBuilder.Matches(name).Cast<Match>().Select<Match, string>(<>f__am$cache1).Aggregate<string>(<>f__am$cache2);
        }

        [DebuggerHidden]
        private IEnumerable<string> OptionNamesFor(ProgramOptionsAttribute options, FieldInfo field) => 
            new <OptionNamesFor>c__Iterator0 { 
                field = field,
                options = options,
                $PC = -2
            };

        internal string[] Parse(IEnumerable<string> commandLine) => 
            this.PrepareOptionSet().Parse(commandLine).ToArray();

        public static Dictionary<string, HelpInformation> ParseHelpTable(Type[] types)
        {
            Dictionary<string, HelpInformation> dictionary = new Dictionary<string, HelpInformation>();
            foreach (Type type in types)
            {
                foreach (FieldInfo info in type.GetFields(BindingFlags.Public | BindingFlags.Static))
                {
                    object[] customAttributes = info.GetCustomAttributes(typeof(HelpDetailsAttribute), false);
                    if (customAttributes.Length > 1)
                    {
                        throw new InvalidOperationException($"Field, {info.Name}, has more than one help attribute");
                    }
                    string key = $"--{NormalizeName(info.Name)}";
                    if (dictionary.ContainsKey(key))
                    {
                        throw new InvalidOperationException($"There are multiple options defined with the name : {key}");
                    }
                    if (!info.GetCustomAttributes(typeof(HideFromHelpAttribute), false).Any<object>())
                    {
                        HelpInformation information;
                        if (customAttributes.Length == 0)
                        {
                            information = new HelpInformation {
                                Summary = null,
                                FieldInfo = info
                            };
                            dictionary.Add(key, information);
                        }
                        else
                        {
                            HelpDetailsAttribute attribute = (HelpDetailsAttribute) customAttributes[0];
                            information = new HelpInformation {
                                Summary = attribute.Summary,
                                FieldInfo = info,
                                CustomValueDescription = attribute.CustomValueDescription
                            };
                            dictionary.Add(key, information);
                        }
                    }
                }
            }
            return dictionary;
        }

        public static Dictionary<string, HelpInformation> ParseHelpTable(Assembly assembly, bool includeReferencedAssemblies = true) => 
            ParseHelpTable(LoadOptionTypesFromAssembly(assembly, includeReferencedAssemblies));

        public static Dictionary<string, HelpInformation> ParseHelpTable(Type type, bool includeReferencedAssemblies = true)
        {
            Type[] types = new Type[] { type };
            return ParseHelpTable(types);
        }

        private static object ParseValue(Type type, string value)
        {
            <ParseValue>c__AnonStorey2 storey = new <ParseValue>c__AnonStorey2 {
                type = type,
                value = value
            };
            if (storey.type.IsEnumPortable())
            {
                return Enum.GetValues(storey.type).Cast<object>().First<object>(new Func<object, bool>(storey, (IntPtr) this.<>m__0));
            }
            object obj3 = Convert.ChangeType(storey.value, storey.type, CultureInfo.InvariantCulture);
            if (obj3 == null)
            {
                throw new NotSupportedException("Unsupported type " + storey.type.FullName);
            }
            return obj3;
        }

        public static string[] Prepare(string[] commandLine, Type[] types)
        {
            OptionsParser parser = new OptionsParser();
            foreach (Type type in types)
            {
                parser.AddType(type);
            }
            return parser.Parse(commandLine);
        }

        public static string[] Prepare(string[] commandLine, Assembly assembly, bool includeReferencedAssemblies = true) => 
            Prepare(commandLine, LoadOptionTypesFromAssembly(assembly, includeReferencedAssemblies));

        public static string[] PrepareFromFile(string argFile, Type[] types)
        {
            if (!File.Exists(argFile))
            {
                throw new FileNotFoundException(argFile);
            }
            return Prepare(File.ReadAllLines(argFile), types);
        }

        public static string[] PrepareFromFile(string argFile, Assembly assembly, bool includeReferencedAssemblies = true) => 
            Prepare(OptionsHelper.LoadArgumentsFromFile(argFile).ToArray<string>(), assembly, includeReferencedAssemblies);

        private OptionSet PrepareOptionSet()
        {
            OptionSet optionSet = new OptionSet();
            foreach (Type type in this._types)
            {
                this.ExtendOptionSet(optionSet, type);
            }
            return optionSet;
        }

        private static void SetArrayType(FieldInfo field, string value, ProgramOptionsAttribute options)
        {
            int length = 0;
            string[] strArray = SplitCollectionValues(options, value);
            Type fieldType = field.FieldType;
            Array destinationArray = (Array) field.GetValue(null);
            if (destinationArray != null)
            {
                Array sourceArray = destinationArray;
                object[] args = new object[] { sourceArray.Length + strArray.Length };
                destinationArray = (Array) Activator.CreateInstance(fieldType, args);
                Array.Copy(sourceArray, destinationArray, sourceArray.Length);
                length = sourceArray.Length;
            }
            else
            {
                object[] objArray2 = new object[] { strArray.Length };
                destinationArray = (Array) Activator.CreateInstance(fieldType, objArray2);
            }
            foreach (string str in strArray)
            {
                destinationArray.SetValue(ParseValue(fieldType.GetElementType(), str), length++);
            }
            field.SetValue(null, destinationArray);
        }

        private static void SetBasicType(FieldInfo field, string v)
        {
            field.SetValue(null, ParseValue(field.FieldType, v));
        }

        private static void SetBoolType(FieldInfo field, string v)
        {
            field.SetValue(null, true);
        }

        private static void SetListType(FieldInfo field, string value, ProgramOptionsAttribute options)
        {
            IList list;
            Type fieldType = field.FieldType;
            IList list1 = (IList) field.GetValue(null);
            if (list1 != null)
            {
                list = list1;
            }
            else
            {
                list = (IList) Activator.CreateInstance(fieldType);
            }
            foreach (string str in SplitCollectionValues(options, value))
            {
                list.Add(ParseValue(fieldType.GetGenericArguments()[0], str));
            }
            field.SetValue(null, list);
        }

        private static string[] SplitCollectionValues(ProgramOptionsAttribute options, string value)
        {
            string[] separator = new string[1];
            if (options.CollectionSeparator == null)
            {
            }
            separator[0] = ",";
            return value.Split(separator, StringSplitOptions.None);
        }

        [CompilerGenerated]
        private sealed class <ActionFor>c__AnonStorey1
        {
            internal FieldInfo field;
            internal ProgramOptionsAttribute options;

            internal void <>m__0(string v)
            {
                OptionsParser.SetArrayType(this.field, v, this.options);
            }

            internal void <>m__1(string v)
            {
                OptionsParser.SetListType(this.field, v, this.options);
            }

            internal void <>m__2(string v)
            {
                OptionsParser.SetBoolType(this.field, v);
            }

            internal void <>m__3(string v)
            {
                OptionsParser.SetBasicType(this.field, v);
            }
        }

        [CompilerGenerated]
        private sealed class <OptionNamesFor>c__Iterator0 : IEnumerable, IEnumerable<string>, IEnumerator, IDisposable, IEnumerator<string>
        {
            internal string $current;
            internal bool $disposing;
            internal int $PC;
            internal string <name>__0;
            internal FieldInfo field;
            internal ProgramOptionsAttribute options;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$disposing = true;
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        this.<name>__0 = OptionsParser.NormalizeName(this.field.Name);
                        if (this.field.FieldType != typeof(bool))
                        {
                            this.<name>__0 = this.<name>__0 + "=";
                        }
                        if (this.options.Group != null)
                        {
                            this.$current = this.options.Group + "." + this.<name>__0;
                            if (!this.$disposing)
                            {
                                this.$PC = 3;
                            }
                        }
                        else
                        {
                            this.$current = this.<name>__0;
                            if (!this.$disposing)
                            {
                                this.$PC = 1;
                            }
                        }
                        goto Label_0129;

                    case 1:
                        this.$current = OptionsParser.NormalizeName(this.field.DeclaringType.Name) + "." + this.<name>__0;
                        if (!this.$disposing)
                        {
                            this.$PC = 2;
                        }
                        goto Label_0129;

                    case 2:
                    case 3:
                        this.$PC = -1;
                        break;
                }
                return false;
            Label_0129:
                return true;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            [DebuggerHidden]
            IEnumerator<string> IEnumerable<string>.GetEnumerator()
            {
                if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
                {
                    return this;
                }
                return new OptionsParser.<OptionNamesFor>c__Iterator0 { 
                    field = this.field,
                    options = this.options
                };
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator() => 
                this.System.Collections.Generic.IEnumerable<string>.GetEnumerator();

            string IEnumerator<string>.Current =>
                this.$current;

            object IEnumerator.Current =>
                this.$current;
        }

        [CompilerGenerated]
        private sealed class <ParseValue>c__AnonStorey2
        {
            internal Type type;
            internal string value;

            internal bool <>m__0(object v) => 
                string.Equals(Enum.GetName(this.type, v), this.value, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}

