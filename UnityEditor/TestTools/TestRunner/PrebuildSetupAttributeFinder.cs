namespace UnityEditor.TestTools.TestRunner
{
    using NUnit.Framework.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEngine.TestTools;

    internal class PrebuildSetupAttributeFinder
    {
        [CompilerGenerated]
        private static Func<ITest, IEnumerable<PrebuildSetupAttribute>> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<ITest, IEnumerable<PrebuildSetupAttribute>> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<PrebuildSetupAttribute, bool> <>f__am$cache2;
        [CompilerGenerated]
        private static Func<PrebuildSetupAttribute, Type> <>f__am$cache3;
        [CompilerGenerated]
        private static Func<ITest, bool> <>f__am$cache4;
        [CompilerGenerated]
        private static Func<ITest, Type> <>f__am$cache5;

        private static void GetMatchingTests(ITest tests, ITestFilter filter, ref List<ITest> resultList)
        {
            foreach (ITest test in tests.Tests)
            {
                if (test.IsSuite)
                {
                    GetMatchingTests(test, filter, ref resultList);
                }
                else if (filter.Pass(test))
                {
                    resultList.Add(test);
                }
            }
        }

        private IEnumerable<Type> GetTypesFromInterface(List<ITest> selectedTests)
        {
            if (<>f__am$cache4 == null)
            {
                <>f__am$cache4 = t => typeof(IPrebuildSetup).IsAssignableFrom(t.Method.TypeInfo.Type);
            }
            if (<>f__am$cache5 == null)
            {
                <>f__am$cache5 = t => t.Method.TypeInfo.Type;
            }
            return Enumerable.Select<ITest, Type>(Enumerable.Where<ITest>(selectedTests, <>f__am$cache4), <>f__am$cache5);
        }

        private static IEnumerable<Type> GetTypesFromPrebuildAttributes(IEnumerable<ITest> tests)
        {
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = t => t.Method.GetCustomAttributes<PrebuildSetupAttribute>(true);
            }
            IEnumerable<PrebuildSetupAttribute> collection = Enumerable.SelectMany<ITest, PrebuildSetupAttribute>(tests, <>f__am$cache0);
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = t => t.Method.TypeInfo.GetCustomAttributes<PrebuildSetupAttribute>(true);
            }
            IEnumerable<PrebuildSetupAttribute> enumerable2 = Enumerable.SelectMany<ITest, PrebuildSetupAttribute>(tests, <>f__am$cache1);
            List<PrebuildSetupAttribute> list = new List<PrebuildSetupAttribute>();
            list.AddRange(collection);
            list.AddRange(enumerable2);
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = a => a.SetupClass != null;
            }
            if (<>f__am$cache3 == null)
            {
                <>f__am$cache3 = a => a.SetupClass;
            }
            return Enumerable.Select<PrebuildSetupAttribute, Type>(Enumerable.Where<PrebuildSetupAttribute>(list, <>f__am$cache2), <>f__am$cache3);
        }

        public IEnumerable<Type> Search(ITest tests, ITestFilter filter)
        {
            List<ITest> resultList = new List<ITest>();
            GetMatchingTests(tests, filter, ref resultList);
            List<Type> source = new List<Type>();
            source.AddRange(GetTypesFromPrebuildAttributes(resultList));
            source.AddRange(this.GetTypesFromInterface(resultList));
            return source.Distinct<Type>();
        }
    }
}

