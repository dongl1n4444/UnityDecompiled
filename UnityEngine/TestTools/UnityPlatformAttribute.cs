namespace UnityEngine.TestTools
{
    using NUnit.Framework;
    using NUnit.Framework.Interfaces;
    using NUnit.Framework.Internal;
    using System;
    using System.Linq;
    using UnityEngine;

    /// <summary>
    /// <para>Specify on which platform the test should run on.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Assembly, AllowMultiple=true, Inherited=false)]
    public class UnityPlatformAttribute : NUnitAttribute, IApplyToTest
    {
        private TestPlatform[] IncludePlatforms;

        /// <summary>
        /// <para>Paltform the test can run on.</para>
        /// </summary>
        /// <param name="includePlatforms"></param>
        public UnityPlatformAttribute(params TestPlatform[] includePlatforms)
        {
            this.IncludePlatforms = includePlatforms;
        }

        public void ApplyToTest(Test test)
        {
            string testPlatform = TestContext.Parameters["platform"];
            if (((test.RunState != RunState.NotRunnable) && (test.RunState != RunState.Ignored)) && !this.IsPlatformSupported(testPlatform))
            {
                test.RunState = RunState.Skipped;
                test.Properties.Add("_SKIPREASON", "Test exluded from the platform " + testPlatform);
            }
        }

        private bool IsPlatformSupported(string testPlatform)
        {
            TestPlatform all = TestPlatform.All;
            try
            {
                all = (TestPlatform) Enum.Parse(typeof(TestPlatform), testPlatform, true);
            }
            catch
            {
                Debug.LogError("Invalid platform value: " + testPlatform);
            }
            if ((this.IncludePlatforms != null) && !this.IncludePlatforms.Contains<TestPlatform>(all))
            {
                return false;
            }
            return true;
        }
    }
}

