namespace UnityEditor.TestTools.TestRunner
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.TestTools;
    using UnityEngine.TestTools.TestRunner;
    using UnityEngine.TestTools.TestRunner.GUI;

    internal abstract class TestLauncherBase
    {
        [CompilerGenerated]
        private static Func<Assembly, IEnumerable<System.Type>> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<System.Type, MethodInfo> <>f__am$cache1;

        protected TestLauncherBase()
        {
        }

        protected void ExecuteSceneSetupMethods(TestRunnerFilter testRunnerFilter)
        {
            PlaymodeTestsController controller = PlaymodeTestsController.GetController();
            foreach (MethodInfo info in GetSceneSetupMethods(controller.settings.filter))
            {
                Debug.Log("Executing setup in " + info.DeclaringType.Name);
                try
                {
                    info.Invoke(null, null);
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }
        }

        private static IEnumerable<MethodInfo> GetSceneSetupMethods(TestRunnerFilter filter)
        {
            <GetSceneSetupMethods>c__AnonStorey0 storey = new <GetSceneSetupMethods>c__AnonStorey0 {
                filter = filter
            };
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = m => m.GetTypes();
            }
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = t => t.GetMethod("ProvideSetupSceneCode", BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Static);
            }
            return Enumerable.Select<System.Type, MethodInfo>(Enumerable.Where<System.Type>(Enumerable.SelectMany<Assembly, System.Type>(AppDomain.CurrentDomain.GetAssemblies(), <>f__am$cache0), new Func<System.Type, bool>(storey.<>m__0)), <>f__am$cache1);
        }

        public abstract void Run();

        [CompilerGenerated]
        private sealed class <GetSceneSetupMethods>c__AnonStorey0
        {
            internal TestRunnerFilter filter;

            internal bool <>m__0(System.Type t)
            {
                if (!this.filter.Matches(t.FullName))
                {
                    return false;
                }
                if (!Attribute.IsDefined(t, typeof(UnityTestAttribute)))
                {
                    return false;
                }
                MethodInfo method = t.GetMethod("ProvideSetupSceneCode", BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Static);
                return (null != method);
            }
        }
    }
}

