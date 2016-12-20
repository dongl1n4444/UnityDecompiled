namespace Unity.DataContract
{
    using System;

    public interface IEditorModule : IDisposable
    {
        void Initialize();
        void Shutdown(bool wait);

        PackageInfo moduleInfo { get; set; }
    }
}

