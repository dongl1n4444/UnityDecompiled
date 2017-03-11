namespace UnityEngine.TestTools.NUnitExtensions
{
    using NUnit.Framework.Api;
    using NUnit.Framework.Interfaces;
    using NUnit.Framework.Internal;
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    internal class UnityTestAssemblyRunner : NUnitTestAssemblyRunner
    {
        private UnityTestAssemblyBuilder unityBuilder;

        public UnityTestAssemblyRunner(UnityTestAssemblyBuilder builder) : base(builder)
        {
            this.unityBuilder = builder;
        }

        public ITest Load(Assembly[] assemblies, IDictionary<string, object> settings)
        {
            base.Settings = settings;
            if (settings.ContainsKey("RandomSeed"))
            {
                Randomizer.InitialSeed = (int) settings["RandomSeed"];
            }
            ITest test = this.unityBuilder.Build(assemblies, settings);
            base.LoadedTest = test;
            return test;
        }
    }
}

