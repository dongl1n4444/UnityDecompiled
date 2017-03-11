namespace UnityEditor.TestRunner.TestLaunchers
{
    using System;
    using System.Linq;
    using System.Text;
    using UnityEditor;
    using UnityEditor.TestRunner.GUI;
    using UnityEditor.TestTools.TestRunner;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.Networking.PlayerConnection;
    using UnityEngine.TestTools.TestRunner;
    using UnityEngine.TestTools.TestRunner.GUI;

    internal class RemotePlayerTestController : ScriptableSingleton<RemotePlayerTestController>
    {
        private bool m_AllTestsRanSuccessfull;
        [SerializeField]
        private bool m_Init;
        [SerializeField]
        private bool m_IsBatchModeRun;
        [SerializeField]
        private PlayerLauncher m_Launcher;
        [SerializeField]
        private PlayerResultWindowUpdater m_PlayerResultWindowUpdater;
        [SerializeField]
        private RunStartedEvent m_RunFinishedEvent = new RunStartedEvent();
        [SerializeField]
        private RunStartedEvent m_RunStartedEvent = new RunStartedEvent();
        [SerializeField]
        protected SerializableDelayedCallback m_SerializableDelayedCallback;
        [SerializeField]
        private RunStartedEvent m_TestFinishedEvent = new RunStartedEvent();
        public Guid runFinishedMessageId = new Guid("ffb622fc-34ad-4901-8d7b-47fb04b0bdd4");
        public Guid runInformationMessageId = new Guid("6a7f53dd-4672-461d-a7b5-9467e9393fd3");
        public Guid testResultMessageId = new Guid("83bcdc98-a846-4b8b-bf30-6a3924bd9aa0");

        private void Connected(int playerId)
        {
        }

        private T Deserialize<T>(byte[] data) => 
            JsonUtility.FromJson<T>(Encoding.UTF8.GetString(data));

        private void Disconnected(int player)
        {
            if (!ScriptableSingleton<EditorConnection>.instance.ConnectedPlayers.Any<ConnectedPlayer>())
            {
                this.m_Init = false;
            }
        }

        public void Init(PlayerLauncher playerLauncher, bool isBatchModeRun)
        {
            this.m_IsBatchModeRun = isBatchModeRun;
            this.m_Launcher = playerLauncher;
            if (!this.m_Init)
            {
                this.m_Init = true;
                ScriptableSingleton<EditorConnection>.instance.Initialize();
                this.Subscribe();
            }
            this.m_PlayerResultWindowUpdater = ScriptableSingleton<PlayerResultWindowUpdater>.instance;
            this.m_PlayerResultWindowUpdater.ResetTestState();
        }

        private void ReceivedTestsData(MessageEventArgs messageEventArgs)
        {
            TestRunnerResult testRunnerResult = this.Deserialize<TestRunnerResult>(messageEventArgs.data);
            this.m_PlayerResultWindowUpdater.TestDone(testRunnerResult);
            this.m_AllTestsRanSuccessfull &= testRunnerResult.resultStatus == TestRunnerResult.ResultStatus.Passed;
        }

        private void RunFinished(MessageEventArgs messageEventArgs)
        {
            ScriptableSingleton<EditorConnection>.instance.Send(this.runFinishedMessageId, null);
            this.m_Launcher.WritePlayerResult(messageEventArgs.data, this.m_AllTestsRanSuccessfull);
        }

        private void RunStarted(MessageEventArgs messageEventArgs)
        {
            this.m_SerializableDelayedCallback.Cancel();
            this.m_AllTestsRanSuccessfull = true;
            this.m_PlayerResultWindowUpdater.RunStarted();
        }

        public void StartTimeoutHandler()
        {
            this.m_SerializableDelayedCallback = SerializableDelayedCallback.SubscribeCallback(new UnityAction(this.TimerDone), TimeSpan.FromMinutes(15.0));
        }

        private void Subscribe()
        {
            ScriptableSingleton<EditorConnection>.instance.RegisterConnection(new UnityAction<int>(this.Connected));
            ScriptableSingleton<EditorConnection>.instance.RegisterDisconnection(new UnityAction<int>(this.Disconnected));
            ScriptableSingleton<EditorConnection>.instance.Register(this.runInformationMessageId, new UnityAction<MessageEventArgs>(this.RunStarted));
            ScriptableSingleton<EditorConnection>.instance.Register(this.testResultMessageId, new UnityAction<MessageEventArgs>(this.ReceivedTestsData));
            ScriptableSingleton<EditorConnection>.instance.Register(this.runFinishedMessageId, new UnityAction<MessageEventArgs>(this.RunFinished));
        }

        public void TimerDone()
        {
            if (this.m_IsBatchModeRun)
            {
                EditorApplication.Exit(2);
            }
            else
            {
                this.m_PlayerResultWindowUpdater.Error();
            }
        }

        public RunStartedEvent runFinishedEvent =>
            this.m_RunFinishedEvent;

        public RunStartedEvent runStartedEvent =>
            this.m_RunStartedEvent;

        public RunStartedEvent testFinishedEvent =>
            this.m_TestFinishedEvent;
    }
}

