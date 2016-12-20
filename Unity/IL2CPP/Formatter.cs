namespace Unity.IL2CPP
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;

    public class Formatter
    {
        [CompilerGenerated]
        private static Func<byte, string> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<string, byte, string> <>f__am$cache1;
        [Inject]
        public static INamingService Naming;

        internal static string AggregateWithComma(ICollection<byte> bytes)
        {
            if (bytes.Count != 0)
            {
            }
            return ((<>f__am$cache0 != null) ? "0" : EnumerableExtensions.AggregateWithComma(Enumerable.Select<byte, string>(bytes, <>f__am$cache0)));
        }

        private static string CheckFloatingPointFormatting(string stringValue, [Optional, DefaultParameterValue("")] string suffix)
        {
            if (!stringValue.Contains("."))
            {
                if (stringValue.Contains("E"))
                {
                    stringValue = stringValue.Replace("E", string.Format(".0E", new object[0])) + suffix;
                    return stringValue;
                }
                stringValue = stringValue + string.Format(".0{0}", suffix);
                return stringValue;
            }
            if (suffix == "f")
            {
                stringValue = stringValue + suffix;
            }
            return stringValue;
        }

        public static string Comment(string str)
        {
            if (!CodeGenOptions.EmitComments)
            {
                return string.Empty;
            }
            return ("/* " + str + " */");
        }

        public static string EscapeString(string str)
        {
            return str.Replace(@"\", @"\\").Replace("\n", @"\n").Replace("\r", @"\r").Replace("\"", "\\\"");
        }

        public static string FormatChar(char c)
        {
            return ("(Il2CppChar)" + ((int) c));
        }

        internal static string Quote(object val)
        {
            string str = val.ToString();
            if (string.IsNullOrEmpty(str))
            {
                return Naming.Null;
            }
            return ("\"" + str + "\"");
        }

        internal static string Stringify(IEnumerable<byte> hash)
        {
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = new Func<string, byte, string>(null, (IntPtr) <Stringify>m__1);
            }
            return Quote(Enumerable.Aggregate<byte, string>(hash, "", <>f__am$cache1));
        }

        internal static string StringRepresentationFor(double value)
        {
            string stringValue = value.ToString("R", CultureInfo.InvariantCulture);
            if (double.IsPositiveInfinity(value))
            {
                return "std::numeric_limits<double>::infinity()";
            }
            if (double.IsNegativeInfinity(value))
            {
                return "-std::numeric_limits<double>::infinity()";
            }
            if (double.IsNaN(value))
            {
                return "std::numeric_limits<double>::quiet_NaN()";
            }
            return CheckFloatingPointFormatting(stringValue, "");
        }

        internal static string StringRepresentationFor(float value)
        {
            string stringValue = value.ToString("R", CultureInfo.InvariantCulture);
            if (float.IsPositiveInfinity(value))
            {
                return "std::numeric_limits<float>::infinity()";
            }
            if (float.IsNegativeInfinity(value))
            {
                return "-std::numeric_limits<float>::infinity()";
            }
            if (float.IsNaN(value))
            {
                return "std::numeric_limits<float>::quiet_NaN()";
            }
            if (value == float.MaxValue)
            {
                return "std::numeric_limits<float>::max()";
            }
            if (value == float.MinValue)
            {
                return "-std::numeric_limits<float>::max()";
            }
            return CheckFloatingPointFormatting(stringValue, "f");
        }
    }
}

