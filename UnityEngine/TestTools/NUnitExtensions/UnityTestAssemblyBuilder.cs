namespace UnityEngine.TestTools.NUnitExtensions
{
    using NUnit.Framework.Api;
    using NUnit.Framework.Interfaces;
    using NUnit.Framework.Internal;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using UnityEngine;

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
    }
}

