namespace UnityEngine.PlaymodeTestsRunner
{
    using System;
    using UnityEngine;

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

        public static PlaymodeTestsControllerSettings CreateRunnerSettings(TestRunnerFilter filter)
        {
            return new PlaymodeTestsControllerSettings { 
                filter = filter,
                sceneBased = false,
                bootstrapScene = null,
                resultFilePath = null,
                isBatchModeRun = false,
                originalScene = null
            };
        }
    }
}

