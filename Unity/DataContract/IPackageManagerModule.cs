namespace Unity.DataContract
{
    using System;
    using System.Collections.Generic;

    public interface IPackageManagerModule : IEditorModule, IDisposable
    {
        void CheckForUpdates();
        void LoadPackage(PackageInfo package);
        void SelectPackage(PackageInfo package);

        string editorInstallPath { get; set; }

        IEnumerable<PackageInfo> playbackEngines { get; }

        IEnumerable<PackageInfo> unityExtensions { get; }

        string unityVersion { get; set; }

        UpdateMode updateMode { get; set; }
    }
}

