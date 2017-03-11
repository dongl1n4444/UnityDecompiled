namespace UnityEngine.TestTools
{
    using NUnit.Framework.Interfaces;
    using NUnit.Framework.Internal.Builders;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    internal class UnityCombinatorialStrategy : CombinatorialStrategy, ICombiningStrategy
    {
        public IEnumerable<ITestCaseData> GetTestCases(IEnumerable[] sources)
        {
            IEnumerable<ITestCaseData> testCases = base.GetTestCases(sources);
            foreach (ITestCaseData data in testCases)
            {
                data.GetType().GetProperty("ExpectedResult").SetValue(data, new object(), null);
            }
            return testCases;
        }
    }
}

