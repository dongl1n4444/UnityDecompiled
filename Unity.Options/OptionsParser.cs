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
using System.Text.RegularExpressions;
using Unity.IL2CPP.Portability;

namespace Unity.Options
{
	public sealed class OptionsParser
	{
		private static readonly Regex NameBuilder = new Regex("([A-Z][a-z_0-9]*)");

		public const int HelpOutputColumnPadding = 50;

		private readonly List<Type> _types = new List<Type>();

		[CompilerGenerated]
		private static Func<Type, bool> <>f__mg$cache0;

		internal OptionsParser()
		{
		}

		public static string[] Prepare(string[] commandLine, Type[] types)
		{
			OptionsParser optionsParser = new OptionsParser();
			for (int i = 0; i < types.Length; i++)
			{
				Type type = types[i];
				optionsParser.AddType(type);
			}
			return optionsParser.Parse(commandLine);
		}

		public static string[] PrepareFromFile(string argFile, Type[] types)
		{
			if (!File.Exists(argFile))
			{
				throw new FileNotFoundException(argFile);
			}
			return OptionsParser.Prepare(File.ReadAllLines(argFile), types);
		}

		public static string[] PrepareFromFile(string argFile, Assembly assembly, bool includeReferencedAssemblies = true)
		{
			return OptionsParser.Prepare(OptionsHelper.LoadArgumentsFromFile(argFile).ToArray<string>(), assembly, includeReferencedAssemblies);
		}

		public static string[] Prepare(string[] commandLine, Assembly assembly, bool includeReferencedAssemblies = true)
		{
			return OptionsParser.Prepare(commandLine, OptionsParser.LoadOptionTypesFromAssembly(assembly, includeReferencedAssemblies));
		}

		public static Dictionary<string, HelpInformation> ParseHelpTable(Type type, bool includeReferencedAssemblies = true)
		{
			return OptionsParser.ParseHelpTable(new Type[]
			{
				type
			});
		}

		public static Dictionary<string, HelpInformation> ParseHelpTable(Type[] types)
		{
			Dictionary<string, HelpInformation> dictionary = new Dictionary<string, HelpInformation>();
			for (int i = 0; i < types.Length; i++)
			{
				Type type = types[i];
				FieldInfo[] fields = type.GetFields(BindingFlags.Static | BindingFlags.Public);
				for (int j = 0; j < fields.Length; j++)
				{
					FieldInfo fieldInfo = fields[j];
					object[] customAttributes = fieldInfo.GetCustomAttributes(typeof(HelpDetailsAttribute), false);
					if (customAttributes.Length > 1)
					{
						throw new InvalidOperationException(string.Format("Field, {0}, has more than one help attribute", fieldInfo.Name));
					}
					string text = string.Format("--{0}", OptionsParser.NormalizeName(fieldInfo.Name));
					if (dictionary.ContainsKey(text))
					{
						throw new InvalidOperationException(string.Format("There are multiple options defined with the name : {0}", text));
					}
					if (!fieldInfo.GetCustomAttributes(typeof(HideFromHelpAttribute), false).Any<object>())
					{
						if (customAttributes.Length == 0)
						{
							dictionary.Add(text, new HelpInformation
							{
								Summary = null,
								FieldInfo = fieldInfo
							});
						}
						else
						{
							HelpDetailsAttribute helpDetailsAttribute = (HelpDetailsAttribute)customAttributes[0];
							dictionary.Add(text, new HelpInformation
							{
								Summary = helpDetailsAttribute.Summary,
								FieldInfo = fieldInfo,
								CustomValueDescription = helpDetailsAttribute.CustomValueDescription
							});
						}
					}
				}
			}
			return dictionary;
		}

		public static Dictionary<string, HelpInformation> ParseHelpTable(Assembly assembly, bool includeReferencedAssemblies = true)
		{
			return OptionsParser.ParseHelpTable(OptionsParser.LoadOptionTypesFromAssembly(assembly, includeReferencedAssemblies));
		}

		public static void DisplayHelp(TextWriter writer, Type type)
		{
			OptionsParser.DisplayHelp(writer, new Type[]
			{
				type
			});
		}

		public static void DisplayHelp(TextWriter writer, Type[] types)
		{
			writer.WriteLine();
			writer.WriteLine("Options:");
			Dictionary<string, HelpInformation> dictionary = OptionsParser.ParseHelpTable(types);
			foreach (KeyValuePair<string, HelpInformation> current in dictionary)
			{
				if (current.Value.HasSummary)
				{
					string text;
					if (current.Value.FieldInfo.FieldType == typeof(bool))
					{
						text = string.Format("  {0}", current.Key);
					}
					else if (current.Value.FieldInfo.FieldType.IsArray || OptionsParser.IsListField(current.Value.FieldInfo))
					{
						text = string.Format("  {0}=<{1},{1},..>", current.Key, (!current.Value.HasCustomValueDescription) ? "value" : current.Value.CustomValueDescription);
					}
					else
					{
						text = string.Format("  {0}=<{1}>", current.Key, (!current.Value.HasCustomValueDescription) ? "value" : current.Value.CustomValueDescription);
					}
					if (text.Length > 50)
					{
						throw new InvalidOperationException(string.Format("Option to long for current padding : {0}, shorten name/value or increase padding if necessary. Over by {1}", current.Key, text.Length - 50));
					}
					text = text.PadRight(50);
					writer.WriteLine("{0}{1}", text, current.Value.Summary);
				}
			}
		}

		public static void DisplayHelp(TextWriter writer, Assembly assembly, bool includeReferencedAssemblies = true)
		{
			OptionsParser.DisplayHelp(writer, OptionsParser.LoadOptionTypesFromAssembly(assembly, includeReferencedAssemblies));
		}

		public static void DisplayHelp(Assembly assembly, bool includeReferencedAssemblies = true)
		{
			OptionsParser.DisplayHelp(Console.Out, OptionsParser.LoadOptionTypesFromAssembly(assembly, includeReferencedAssemblies));
		}

		public static bool HelpRequested(string[] commandLine)
		{
			return commandLine.Count((string v) => v == "--h" || v == "--help" || v == "-help") > 0;
		}

		private static Type[] LoadOptionTypesFromAssembly(Assembly assembly, bool includeReferencedAssemblies)
		{
			List<Type> list = new List<Type>();
			Stack<Assembly> stack = new Stack<Assembly>();
			HashSet<AssemblyName> hashSet = new HashSet<AssemblyName>(new AssemblyNameComparer());
			stack.Push(assembly);
			while (stack.Count > 0)
			{
				Assembly assembly2 = stack.Pop();
				if (hashSet.Add(assembly2.GetName()))
				{
					List<Type> arg_6B_0 = list;
					IEnumerable<Type> arg_66_0 = assembly2.GetTypesPortable();
					if (OptionsParser.<>f__mg$cache0 == null)
					{
						OptionsParser.<>f__mg$cache0 = new Func<Type, bool>(OptionsParser.HasProgramOptionsAttribute);
					}
					arg_6B_0.AddRange(arg_66_0.Where(OptionsParser.<>f__mg$cache0));
					if (includeReferencedAssemblies)
					{
						AssemblyName[] referencedAssembliesPortable = assembly2.GetReferencedAssembliesPortable();
						for (int i = 0; i < referencedAssembliesPortable.Length; i++)
						{
							AssemblyName assemblyName = referencedAssembliesPortable[i];
							if (!(assemblyName.Name == "mscorlib") && !assemblyName.Name.StartsWith("System") && !assemblyName.Name.StartsWith("Mono.Cecil") && !(assemblyName.Name == "Unity.IL2CPP.RuntimeServices"))
							{
								if (!hashSet.Contains(assemblyName))
								{
									try
									{
										Assembly item = Assembly.Load(assemblyName);
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
			}
			return list.ToArray();
		}

		internal void AddType(Type type)
		{
			this._types.Add(type);
		}

		internal static bool HasProgramOptionsAttribute(Type type)
		{
			return type.GetCustomAttributesPortable(typeof(ProgramOptionsAttribute), false).Any<object>();
		}

		internal string[] Parse(IEnumerable<string> commandLine)
		{
			OptionSet optionSet = this.PrepareOptionSet();
			return optionSet.Parse(commandLine).ToArray();
		}

		private OptionSet PrepareOptionSet()
		{
			OptionSet optionSet = new OptionSet();
			foreach (Type current in this._types)
			{
				this.ExtendOptionSet(optionSet, current);
			}
			return optionSet;
		}

		private void ExtendOptionSet(OptionSet optionSet, Type type)
		{
			FieldInfo[] fields = type.GetFields(BindingFlags.Static | BindingFlags.Public);
			FieldInfo[] array = fields;
			for (int i = 0; i < array.Length; i++)
			{
				FieldInfo field = array[i];
				ProgramOptionsAttribute options = (ProgramOptionsAttribute)type.GetCustomAttributesPortable(typeof(ProgramOptionsAttribute), false).First<object>();
				foreach (string current in this.OptionNamesFor(options, field))
				{
					optionSet.Add(current, OptionsParser.DescriptionFor(field), OptionsParser.ActionFor(options, field));
				}
			}
		}

		[DebuggerHidden]
		private IEnumerable<string> OptionNamesFor(ProgramOptionsAttribute options, FieldInfo field)
		{
			OptionsParser.<OptionNamesFor>c__Iterator0 <OptionNamesFor>c__Iterator = new OptionsParser.<OptionNamesFor>c__Iterator0();
			<OptionNamesFor>c__Iterator.field = field;
			<OptionNamesFor>c__Iterator.options = options;
			OptionsParser.<OptionNamesFor>c__Iterator0 expr_15 = <OptionNamesFor>c__Iterator;
			expr_15.$PC = -2;
			return expr_15;
		}

		private static string NormalizeName(string name)
		{
			return (from Match m in OptionsParser.NameBuilder.Matches(name)
			select m.Value.ToLower()).Aggregate((string buff, string s) => buff + "-" + s);
		}

		private static string DescriptionFor(FieldInfo field)
		{
			return "";
		}

		private static Action<string> ActionFor(ProgramOptionsAttribute options, FieldInfo field)
		{
			Action<string> result;
			if (field.FieldType.IsArray)
			{
				result = delegate(string v)
				{
					OptionsParser.SetArrayType(field, v, options);
				};
			}
			else if (OptionsParser.IsListField(field))
			{
				result = delegate(string v)
				{
					OptionsParser.SetListType(field, v, options);
				};
			}
			else if (field.FieldType == typeof(bool))
			{
				result = delegate(string v)
				{
					OptionsParser.SetBoolType(field, v);
				};
			}
			else
			{
				result = delegate(string v)
				{
					OptionsParser.SetBasicType(field, v);
				};
			}
			return result;
		}

		private static bool IsListField(FieldInfo field)
		{
			bool result;
			if (field.FieldType.IsGenericTypePortable())
			{
				Type genericTypeDefinition = field.FieldType.GetGenericTypeDefinition();
				if (genericTypeDefinition.IsAssignableFrom(typeof(List<>)))
				{
					result = true;
					return result;
				}
			}
			result = false;
			return result;
		}

		private static void SetListType(FieldInfo field, string value, ProgramOptionsAttribute options)
		{
			Type fieldType = field.FieldType;
			IList list = ((IList)field.GetValue(null)) ?? ((IList)Activator.CreateInstance(fieldType));
			string[] array = OptionsParser.SplitCollectionValues(options, value);
			for (int i = 0; i < array.Length; i++)
			{
				string value2 = array[i];
				list.Add(OptionsParser.ParseValue(fieldType.GetGenericArguments()[0], value2));
			}
			field.SetValue(null, list);
		}

		private static void SetArrayType(FieldInfo field, string value, ProgramOptionsAttribute options)
		{
			int num = 0;
			string[] array = OptionsParser.SplitCollectionValues(options, value);
			Type fieldType = field.FieldType;
			Array array2 = (Array)field.GetValue(null);
			if (array2 != null)
			{
				Array array3 = array2;
				array2 = (Array)Activator.CreateInstance(fieldType, new object[]
				{
					array3.Length + array.Length
				});
				Array.Copy(array3, array2, array3.Length);
				num = array3.Length;
			}
			else
			{
				array2 = (Array)Activator.CreateInstance(fieldType, new object[]
				{
					array.Length
				});
			}
			string[] array4 = array;
			for (int i = 0; i < array4.Length; i++)
			{
				string value2 = array4[i];
				array2.SetValue(OptionsParser.ParseValue(fieldType.GetElementType(), value2), num++);
			}
			field.SetValue(null, array2);
		}

		private static void SetBoolType(FieldInfo field, string v)
		{
			field.SetValue(null, true);
		}

		private static void SetBasicType(FieldInfo field, string v)
		{
			field.SetValue(null, OptionsParser.ParseValue(field.FieldType, v));
		}

		private static string[] SplitCollectionValues(ProgramOptionsAttribute options, string value)
		{
			return value.Split(new string[]
			{
				options.CollectionSeparator ?? ","
			}, StringSplitOptions.None);
		}

		private static object ParseValue(Type type, string value)
		{
			object result;
			if (type.IsEnumPortable())
			{
				result = Enum.GetValues(type).Cast<object>().First((object v) => string.Equals(Enum.GetName(type, v), value, StringComparison.InvariantCultureIgnoreCase));
			}
			else
			{
				object obj = Convert.ChangeType(value, type, CultureInfo.InvariantCulture);
				if (obj == null)
				{
					throw new NotSupportedException("Unsupported type " + type.FullName);
				}
				result = obj;
			}
			return result;
		}
	}
}
