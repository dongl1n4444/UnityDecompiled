namespace UnityEngine.TestTools.TestRunner
{
    using System;
    using UnityEngine;
    using UnityEngine.TestTools.TestRunner.GUI;

    [Serializable]
    internal class PlaymodeTestsControllerSettings
    {
        public string bootstrapScene;
        [SerializeField]
        public TestRunnerFilter filter;
        public bool isBatchModeRun;
        public string originalScene;
        public string resultFilePath;
        public bool sceneBased;

        public static PlaymodeTestsControllerSettings CreateRunnerSettings(TestRunnerFilter filter) => 
            new PlaymodeTestsControllerSettings { 
                filter = filter,
                sceneBased = false,
                bootstrapScene = null,
                resultFilePath = null,
                isBatchModeRun = false,
                originalScene = null
            };
    }
}

