using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Unity.IL2CPP.IoC;
using Unity.IL2CPP.IoCServices;

namespace Unity.IL2CPP
{
	public class Formatter
	{
		[Inject]
		public static INamingService Naming;

		internal static string StringRepresentationFor(double value)
		{
			string text = value.ToString("R", CultureInfo.InvariantCulture);
			if (double.IsPositiveInfinity(value))
			{
				text = "std::numeric_limits<double>::infinity()";
			}
			else if (double.IsNegativeInfinity(value))
			{
				text = "-std::numeric_limits<double>::infinity()";
			}
			else if (double.IsNaN(value))
			{
				text = "std::numeric_limits<double>::quiet_NaN()";
			}
			else
			{
				text = Formatter.CheckFloatingPointFormatting(text, "");
			}
			return text;
		}

		internal static string StringRepresentationFor(float value)
		{
			string text = value.ToString("R", CultureInfo.InvariantCulture);
			if (float.IsPositiveInfinity(value))
			{
				text = "std::numeric_limits<float>::infinity()";
			}
			else if (float.IsNegativeInfinity(value))
			{
				text = "-std::numeric_limits<float>::infinity()";
			}
			else if (float.IsNaN(value))
			{
				text = "std::numeric_limits<float>::quiet_NaN()";
			}
			else if (value == 3.40282347E+38f)
			{
				text = "std::numeric_limits<float>::max()";
			}
			else if (value == -3.40282347E+38f)
			{
				text = "-std::numeric_limits<float>::max()";
			}
			else
			{
				text = Formatter.CheckFloatingPointFormatting(text, "f");
			}
			return text;
		}

		private static string CheckFloatingPointFormatting(string stringValue, string suffix = "")
		{
			if (!stringValue.Contains("."))
			{
				if (stringValue.Contains("E"))
				{
					stringValue = stringValue.Replace("E", string.Format(".0E", new object[0])) + suffix;
				}
				else
				{
					stringValue += string.Format(".0{0}", suffix);
				}
			}
			else if (suffix == "f")
			{
				stringValue += suffix;
			}
			return stringValue;
		}

		public static string EscapeString(string str)
		{
			return str.Replace("\\", "\\\\").Replace("\n", "\\n").Replace("\r", "\\r").Replace("\"", "\\\"");
		}

		public static string FormatChar(char c)
		{
			return "(Il2CppChar)" + (int)c;
		}

		public static string Comment(string str)
		{
			string result;
			if (!CodeGenOptions.EmitComments)
			{
				result = string.Empty;
			}
			else
			{
				result = "/* " + str + " */";
			}
			return result;
		}

		internal static string AggregateWithComma(ICollection<byte> bytes)
		{
			string arg_3E_0;
			if (bytes.Count != 0)
			{
				arg_3E_0 = (from s in bytes
				select "0x" + s.ToString("X2")).AggregateWithComma();
			}
			else
			{
				arg_3E_0 = "0";
			}
			return arg_3E_0;
		}

		internal static string Stringify(IEnumerable<byte> hash)
		{
			return Formatter.Quote(hash.Aggregate("", (string buff, byte s) => buff + "\\x" + s.ToString("X")));
		}

		internal static string Quote(object val)
		{
			string text = val.ToString();
			string result;
			if (string.IsNullOrEmpty(text))
			{
				result = Formatter.Naming.Null;
			}
			else
			{
				result = "\"" + text + "\"";
			}
			return result;
		}
	}
}
