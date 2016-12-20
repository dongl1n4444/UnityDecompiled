namespace UnityEngine.Advertisements
{
    using System;
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

        public PlacementState GetPlacementState(string placementId)
        {
            return PlacementState.NotAvailable;
        }

        public void Initialize(string gameId, bool testMode)
        {
        }

        public bool IsReady(string placementId)
        {
            return false;
        }

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
            get
            {
                return false;
            }
            set
            {
            }
        }

        public bool isInitialized
        {
            get
            {
                return false;
            }
        }

        public bool isSupported
        {
            get
            {
                return false;
            }
        }

        public string version
        {
            get
            {
                return null;
            }
        }
    }
}

