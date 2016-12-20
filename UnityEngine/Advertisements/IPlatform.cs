namespace UnityEngine.Advertisements
{
    using System;

    internal interface IPlatform
    {
        event EventHandler<ErrorEventArgs> OnError;

        event EventHandler<FinishEventArgs> OnFinish;

        event EventHandler<ReadyEventArgs> OnReady;

        event EventHandler<StartEventArgs> OnStart;

        PlacementState GetPlacementState(string placementId);
        void Initialize(string gameId, bool testMode);
        bool IsReady(string placementId);
        void SetMetaData(MetaData metaData);
        void Show(string placementId);

        bool debugMode { get; set; }

        bool isInitialized { get; }

        bool isSupported { get; }

        string version { get; }
    }
}

