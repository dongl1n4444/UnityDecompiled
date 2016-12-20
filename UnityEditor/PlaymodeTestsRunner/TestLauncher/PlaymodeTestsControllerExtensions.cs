namespace UnityEditor.PlaymodeTestsRunner.TestLauncher
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEditor.Events;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.PlaymodeTestsRunner;

    [Extension]
    internal static class PlaymodeTestsControllerExtensions
    {
        [Extension]
        internal static T AddEventHandlerMonoBehaviour<T>(PlaymodeTestsController controller) where T: MonoBehaviour, TestRunnerListener
        {
            T eventHandler = controller.gameObject.AddComponent<T>();
            SetListeners<T>(controller, eventHandler);
            return eventHandler;
        }

        [Extension]
        internal static T AddEventHandlerScriptableObject<T>(PlaymodeTestsController controller) where T: ScriptableObject, TestRunnerListener
        {
            T local = ScriptableObject.CreateInstance<T>();
            T local1 = local;
            UnityEventTools.AddPersistentListener<string>(controller.testStartedEvent, new UnityAction<string>(local1.TestStarted));
            T local3 = local;
            UnityEventTools.AddPersistentListener<TestResult>(controller.testFinishedEvent, new UnityAction<TestResult>(local3.TestFinished));
            T local4 = local;
            UnityEventTools.AddPersistentListener<string, List<string>>(controller.runStartedEvent, new UnityAction<string, List<string>>(local4.RunStarted));
            T local5 = local;
            UnityEventTools.AddPersistentListener<List<TestResult>>(controller.runFinishedEvent, new UnityAction<List<TestResult>>(local5.RunFinished));
            return local;
        }

        private static void SetListeners<T>(PlaymodeTestsController controller, TestRunnerListener eventHandler)
        {
            UnityEventTools.AddPersistentListener<string>(controller.testStartedEvent, new UnityAction<string>(eventHandler.TestStarted));
            UnityEventTools.AddPersistentListener<TestResult>(controller.testFinishedEvent, new UnityAction<TestResult>(eventHandler.TestFinished));
            UnityEventTools.AddPersistentListener<string, List<string>>(controller.runStartedEvent, new UnityAction<string, List<string>>(eventHandler.RunStarted));
            UnityEventTools.AddPersistentListener<List<TestResult>>(controller.runFinishedEvent, new UnityAction<List<TestResult>>(eventHandler.RunFinished));
        }
    }
}

