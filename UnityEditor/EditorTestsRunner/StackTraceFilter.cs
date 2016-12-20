namespace UnityEditor.EditorTestsRunner
{
    using System;
    using System.IO;

    internal class StackTraceFilter
    {
        public static string Filter(string stack)
        {
            if (stack == null)
            {
                return null;
            }
            StringWriter writer = new StringWriter();
            StringReader reader = new StringReader(stack);
            try
            {
                string str2;
                while ((str2 = reader.ReadLine()) != null)
                {
                    if (!FilterLine(str2))
                    {
                        writer.WriteLine(str2.Trim());
                    }
                }
            }
            catch (Exception)
            {
                return stack;
            }
            return writer.ToString();
        }

        private static bool FilterLine(string line)
        {
            string[] strArray = new string[] { "NUnit.Core.TestCase", "NUnit.Core.ExpectedExceptionTestCase", "NUnit.Core.TemplateTestCase", "NUnit.Core.TestResult", "NUnit.Core.TestSuite", "NUnit.Framework.Assertion", "NUnit.Framework.Assert", "System.Reflection.MonoMethod" };
            for (int i = 0; i < strArray.Length; i++)
            {
                if (line.IndexOf(strArray[i]) > 0)
                {
                    return true;
                }
            }
            return false;
        }
    }
}

