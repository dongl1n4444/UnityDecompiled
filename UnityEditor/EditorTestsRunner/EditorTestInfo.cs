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
            this.id = testInfo.TestName.TestID.ToString();
            this.fullName = testInfo.TestName.FullName;
        }

        public EditorTestInfo(TestMethod testMethod)
        {
            this.methodName = testMethod.get_MethodName();
            this.fullMethodName = testMethod.Method.ToString();
            this.className = testMethod.FixtureType.Name;
            this.fullClassName = testMethod.get_ClassName();
            this.Namespace = testMethod.Method.ReflectedType.Namespace;
            this.fullName = testMethod.TestName.FullName;
            this.paramName = ExtractMethodCallParametersString(this.fullName);
            this.id = testMethod.TestName.TestID.GetHashCode().ToString();
            List<string> list = testMethod.Categories.Cast<string>().ToList<string>();
            if (testMethod.Parent.Categories.Count > 0)
            {
                list.AddRange(testMethod.Parent.Categories.Cast<string>());
            }
            if (testMethod.Parent is ParameterizedMethodSuite)
            {
                list.AddRange(testMethod.Parent.Parent.Categories.Cast<string>());
            }
            this.categories = list.ToArray();
            this.assemblyPath = this.GetAssemblyPath(testMethod);
            this.isIgnored = testMethod.RunState == RunState.Ignored;
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
            Test parent = testMethod;
            while (parent != null)
            {
                parent = parent.Parent;
                if (parent is TestAssembly)
                {
                    string fullName = (parent as TestAssembly).TestName.FullName;
                    if (File.Exists(fullName))
                    {
                        return fullName;
                    }
                }
            }
            return "";
        }
    }
}

