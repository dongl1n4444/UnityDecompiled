namespace UnityEngine.Advertisements
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading;

    internal sealed class UnsupportedPlatform : IPlatform
    {
        public event EventHandler<ErrorEventArgs> OnError
        {
            add
            {
            }
            remove
            {
            }
        }

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event EventHandler<FinishEventArgs> OnFinish;

        public event EventHandler<ReadyEventArgs> OnReady
        {
            add
            {
            }
            remove
            {
            }
        }

        public event EventHandler<StartEventArgs> OnStart
        {
            add
            {
            }
            remove
            {
            }
        }

        public PlacementState GetPlacementState(string placementId) => 
            PlacementState.NotAvailable;

        public void Initialize(string gameId, bool testMode)
        {
        }

        public bool IsReady(string placementId) => 
            false;

        public void SetMetaData(MetaData metaData)
        {
        }

        public void Show(string placementId)
        {
            EventHandler<FinishEventArgs> onFinish = this.OnFinish;
            if (onFinish != null)
            {
                onFinish(this, new FinishEventArgs(placementId, ShowResult.Failed));
            }
        }

        public bool debugMode
        {
            get => 
                false;
            set
            {
            }
        }

        public bool isInitialized =>
            false;

        public bool isSupported =>
            false;

        public string version =>
            null;
    }
}

