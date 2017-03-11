namespace UnityEngine.TestTools.NUnitExtensions
{
    using NUnit.Framework.Api;
    using NUnit.Framework.Interfaces;
    using NUnit.Framework.Internal;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using UnityEngine;
    using UnityEngine.TestTools;
    using UnityEngine.TestTools.Utils;

    internal class UnityTestAssemblyBuilder : DefaultTestAssemblyBuilder
    {
        public ITest Build(Assembly[] assemblies, IDictionary<string, object> options)
        {
            TestSuite suite = new TestSuite(string.Join("_", Application.productName.Split(Path.GetInvalidFileNameChars())));
            foreach (Assembly assembly in assemblies)
            {
                TestSuite test = base.Build(assembly, options) as TestSuite;
                suite.Add(test);
            }
            return suite;
        }

        public static UnityTestAssemblyBuilder GetNUnitTestBuilder(TestPlatform testPlatform) => 
            new UnityTestAssemblyBuilder();

        public static Dictionary<string, object> GetNUnitTestBuilderSettings(TestPlatform testPlatform) => 
            new Dictionary<string, object> { { 
                "TestParameters",
                ("platform=" + testPlatform)
            } };

        internal static Assembly[] GetUserAssemblies(bool includeEditorAssemblies)
        {
            TestAssemblyProvider provider = new TestAssemblyProvider();
            return provider.GetUserAssemblies(includeEditorAssemblies).ToArray<Assembly>();
        }
    }
}

