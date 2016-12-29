namespace UnityEditor.EditorTestsRunner
{
    using NUnit.Core;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;

    [Serializable]
    internal class EditorTestInfo
    {
        public string assemblyPath;
        public string[] categories;
        public string className;
        public string fullClassName;
        public string fullMethodName;
        public string fullName;
        public string id;
        public bool isIgnored;
        public string methodName;
        public string Namespace;
        public string paramName;
        [NonSerialized]
        public TestMethod testMethod;

        public EditorTestInfo(ITest testInfo)
        {
            this.id = testInfo.get_TestName().get_TestID().ToString();
            this.fullName = testInfo.get_TestName().get_FullName();
        }

        public EditorTestInfo(TestMethod testMethod)
        {
            this.methodName = testMethod.get_MethodName();
            this.fullMethodName = testMethod.get_Method().ToString();
            this.className = testMethod.get_FixtureType().Name;
            this.fullClassName = testMethod.get_ClassName();
            this.Namespace = testMethod.get_Method().ReflectedType.Namespace;
            this.fullName = testMethod.get_TestName().get_FullName();
            this.paramName = ExtractMethodCallParametersString(this.fullName);
            this.id = testMethod.get_TestName().get_TestID().GetHashCode().ToString();
            List<string> list = testMethod.get_Categories().Cast<string>().ToList<string>();
            if (testMethod.get_Parent().get_Categories().Count > 0)
            {
                list.AddRange(testMethod.get_Parent().get_Categories().Cast<string>());
            }
            if (testMethod.get_Parent() is ParameterizedMethodSuite)
            {
                list.AddRange(testMethod.get_Parent().get_Parent().get_Categories().Cast<string>());
            }
            this.categories = list.ToArray();
            this.assemblyPath = this.GetAssemblyPath(testMethod);
            this.isIgnored = testMethod.get_RunState() == 4;
        }

        private static string ExtractMethodCallParametersString(string methodFullName)
        {
            Match match = Regex.Match(methodFullName, @"\((.*)\)");
            string str = "";
            if (match.Groups[1].Success)
            {
                str = match.Groups[1].Captures[0].Value;
            }
            return str;
        }

        private string GetAssemblyPath(TestMethod testMethod)
        {
            Test test = testMethod;
            while (test != null)
            {
                test = test.get_Parent();
                if (test is TestAssembly)
                {
                    string path = (test as TestAssembly).get_TestName().get_FullName();
                    if (File.Exists(path))
                    {
                        return path;
                    }
                }
            }
            return "";
        }
    }
}

