namespace UnityEditor.TestTools.TestRunner
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEditor.Events;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.TestTools.TestRunner;

    internal static class PlaymodeTestsControllerExtensions
    {
        internal static T AddEventHandlerMonoBehaviour<T>(this PlaymodeTestsController controller) where T: MonoBehaviour, TestRunnerListener
        {
            T eventHandler = controller.gameObject.AddComponent<T>();
            SetListeners<T>(controller, eventHandler);
            return eventHandler;
        }

        internal static T AddEventHandlerScriptableObject<T>(this PlaymodeTestsController controller) where T: ScriptableObject, TestRunnerListener
        {
            T eventHandler = ScriptableObject.CreateInstance<T>();
            SetListeners<T>(controller, eventHandler);
            return eventHandler;
        }

        private static void SetListeners<T>(PlaymodeTestsController controller, TestRunnerListener eventHandler)
        {
            UnityEventTools.AddPersistentListener<ITest>(controller.testStartedEvent, new UnityAction<ITest>(eventHandler.TestStarted));
            UnityEventTools.AddPersistentListener<ITestResult>(controller.testFinishedEvent, new UnityAction<ITestResult>(eventHandler.TestFinished));
            UnityEventTools.AddPersistentListener<ITest>(controller.runStartedEvent, new UnityAction<ITest>(eventHandler.RunStarted));
            UnityEventTools.AddPersistentListener<ITestResult>(controller.runFinishedEvent, new UnityAction<ITestResult>(eventHandler.RunFinished));
        }
    }
}

