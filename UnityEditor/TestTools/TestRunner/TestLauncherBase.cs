namespace UnityEditor.TestTools.TestRunner
{
    using NUnit.Framework.Interfaces;
    using System;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.TestTools;

    internal abstract class TestLauncherBase
    {
        protected TestLauncherBase()
        {
        }

        protected void ExecutePreBuildSetupMethods(ITest tests, ITestFilter testRunnerFilter)
        {
            PrebuildSetupAttributeFinder finder = new PrebuildSetupAttributeFinder();
            AssetDatabase.StartAssetEditing();
            foreach (System.Type type in finder.Search(tests, testRunnerFilter))
            {
                try
                {
                    IPrebuildSetup setup = (IPrebuildSetup) Activator.CreateInstance(type);
                    object[] args = new object[] { type.FullName };
                    Debug.LogFormat("Executing setup for: {0}", args);
                    setup.Setup();
                }
                catch (InvalidCastException)
                {
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }
            AssetDatabase.StopAssetEditing();
        }

        public abstract void Run();
    }
}

