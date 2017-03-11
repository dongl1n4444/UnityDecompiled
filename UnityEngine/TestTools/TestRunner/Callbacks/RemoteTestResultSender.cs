namespace UnityEngine.TestTools.TestRunner.Callbacks
{
    using NUnit.Framework.Interfaces;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading;
    using System.Xml;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.Networking.PlayerConnection;
    using UnityEngine.TestTools.TestRunner;
    using UnityEngine.TestTools.TestRunner.GUI;

    internal class RemoteTestResultSender : MonoBehaviour, TestRunnerListener
    {
        public bool isBatchModeRun;
        private object m_LockQueue = new object();
        private Queue<QueueData> m_SendQueue = new Queue<QueueData>();
        public Guid runFinishedMessageId = new Guid("ffb622fc-34ad-4901-8d7b-47fb04b0bdd4");
        public Guid runInformationMessageId = new Guid("6a7f53dd-4672-461d-a7b5-9467e9393fd3");
        public Guid testResultMessageId = new Guid("83bcdc98-a846-4b8b-bf30-6a3924bd9aa0");

        private void EditorProccessedTheResult(MessageEventArgs arg0)
        {
            if ((arg0.data == null) && (Application.platform != RuntimePlatform.XboxOne))
            {
                Application.Quit();
            }
        }

        public void RunFinished(ITestResult testResults)
        {
            byte[] buffer;
            XmlWriterSettings settings = new XmlWriterSettings {
                Indent = true,
                NewLineOnAttributes = false
            };
            using (MemoryStream stream = new MemoryStream())
            {
                using (XmlWriter writer = XmlWriter.Create(stream, settings))
                {
                    testResults.ToXml(true).WriteTo(writer);
                }
                buffer = stream.ToArray();
            }
            object sendQueue = this.m_SendQueue;
            lock (sendQueue)
            {
                QueueData item = new QueueData {
                    id = this.runFinishedMessageId,
                    data = buffer
                };
                this.m_SendQueue.Enqueue(item);
            }
        }

        public void RunStarted(ITest testsToRun)
        {
            object lockQueue = this.m_LockQueue;
            lock (lockQueue)
            {
                QueueData item = new QueueData {
                    id = this.runInformationMessageId
                };
                this.m_SendQueue.Enqueue(item);
            }
        }

        [DebuggerHidden]
        public IEnumerator SendDataRoutine() => 
            new <SendDataRoutine>c__Iterator0 { $this = this };

        private byte[] SerializeObject(object objectToSerialize) => 
            Encoding.UTF8.GetBytes(JsonUtility.ToJson(objectToSerialize));

        public void Start()
        {
            UnityEngine.Networking.PlayerConnection.PlayerConnection.instance.Register(this.runFinishedMessageId, new UnityAction<MessageEventArgs>(this.EditorProccessedTheResult));
            base.StartCoroutine(this.SendDataRoutine());
        }

        public void TestFinished(ITestResult test)
        {
            TestRunnerResult objectToSerialize = new TestRunnerResult(test);
            byte[] buffer = this.SerializeObject(objectToSerialize);
            object sendQueue = this.m_SendQueue;
            lock (sendQueue)
            {
                QueueData item = new QueueData {
                    id = this.testResultMessageId,
                    data = buffer
                };
                this.m_SendQueue.Enqueue(item);
            }
        }

        public void TestStarted(ITest test)
        {
        }

        [CompilerGenerated]
        private sealed class <SendDataRoutine>c__Iterator0 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal bool $disposing;
            internal object $locvar0;
            internal int $PC;
            internal RemoteTestResultSender $this;
            internal bool <finished>__0;

            private void <>__Finally0()
            {
                Monitor.Exit(this.$locvar0);
            }

            [DebuggerHidden]
            public void Dispose()
            {
                uint num = (uint) this.$PC;
                this.$disposing = true;
                this.$PC = -1;
                switch (num)
                {
                    case 2:
                        try
                        {
                        }
                        finally
                        {
                            this.<>__Finally0();
                        }
                        break;
                }
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                bool flag = false;
                switch (num)
                {
                    case 0:
                    case 1:
                        while (!UnityEngine.Networking.PlayerConnection.PlayerConnection.instance.isConnected)
                        {
                            this.$current = new WaitForSeconds(1f);
                            if (!this.$disposing)
                            {
                                this.$PC = 1;
                            }
                            goto Label_0175;
                        }
                        this.<finished>__0 = false;
                        break;

                    case 2:
                        goto Label_008A;

                    default:
                        goto Label_0173;
                }
            Label_0069:
                this.$locvar0 = this.$this.m_SendQueue;
                Monitor.Enter(this.$locvar0);
                num = 0xfffffffd;
            Label_008A:
                try
                {
                    switch (num)
                    {
                        case 2:
                            goto Label_0069;
                    }
                    if (UnityEngine.Networking.PlayerConnection.PlayerConnection.instance.isConnected && (this.$this.m_SendQueue.Count > 0))
                    {
                        RemoteTestResultSender.QueueData data = this.$this.m_SendQueue.Dequeue();
                        UnityEngine.Networking.PlayerConnection.PlayerConnection.instance.Send(data.id, data.data);
                        this.<finished>__0 |= data.id == this.$this.runFinishedMessageId;
                        if (this.<finished>__0 && (this.$this.m_SendQueue.Count <= 0))
                        {
                            this.$PC = -1;
                            goto Label_0173;
                        }
                    }
                    this.$current = new WaitForSeconds(0.05f);
                    if (!this.$disposing)
                    {
                        this.$PC = 2;
                    }
                    flag = true;
                    goto Label_0175;
                }
                finally
                {
                    if (!flag)
                    {
                    }
                    this.<>__Finally0();
                }
                goto Label_0069;
            Label_0173:
                return false;
            Label_0175:
                return true;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            object IEnumerator<object>.Current =>
                this.$current;

            object IEnumerator.Current =>
                this.$current;
        }

        private class QueueData
        {
            [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private byte[] <data>k__BackingField;
            [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private Guid <id>k__BackingField;

            public byte[] data { get; set; }

            public Guid id { get; set; }
        }
    }
}

