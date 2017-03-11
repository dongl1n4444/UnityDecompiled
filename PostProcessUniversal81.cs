using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Modules;
using UnityEditor.WSA;

internal class PostProcessUniversal81 : PostProcessWSA
{
    private readonly PostProcessWSA _phone;
    private readonly string _phoneStagingArea;
    private readonly string _stagingAreaShared;
    private readonly PostProcessWSA _store;
    private readonly string _storeStagingArea;

    public PostProcessUniversal81(BuildPostProcessArgs args) : base(args, WSASDK.UniversalSDK81, null)
    {
        this._stagingAreaShared = Utility.CombinePath(base.StagingArea, base.VisualStudioName + ".Shared");
        this._storeStagingArea = Utility.CombinePath(base.StagingArea, base.VisualStudioName + ".Windows");
        this._phoneStagingArea = Utility.CombinePath(base.StagingArea, base.VisualStudioName + ".WindowsPhone");
        this._store = new PostProcessStore81(args, this._storeStagingArea);
        this._phone = new PostProcessPhone81(args, this._phoneStagingArea);
    }

    public override void CheckSDK()
    {
        this._store.CheckSDK();
        this._phone.CheckSDK();
    }

    public override void CopyImages()
    {
        this._store.CopyImages();
        this._phone.CopyImages();
    }

    public override void CopyPlayerFiles()
    {
        this._store.CopyPlayerFiles();
        this._phone.CopyPlayerFiles();
    }

    public override void CopyPlugins()
    {
        this._store.CopyPlugins();
        this._phone.CopyPlugins();
    }

    public override void CopyStreamingAssets()
    {
        this._store.CopyStreamingAssets();
        this._phone.CopyStreamingAssets();
    }

    public override void CopyTestCertificate()
    {
        this._store.CopyTestCertificate();
        this._phone.CopyTestCertificate();
    }

    public override void CreateCommandLineArgsPlaceholder()
    {
        this._store.CreateCommandLineArgsPlaceholder();
        this._phone.CreateCommandLineArgsPlaceholder();
    }

    public override void CreateManifest()
    {
        this._store.CreateManifest();
        this._phone.CreateManifest();
    }

    protected override ManifestWSA CreateManifestBuilder()
    {
        throw new InvalidOperationException();
    }

    public override void CreateRegistryTxtFiles()
    {
        this._store.CreateRegistryTxtFiles();
        this._phone.CreateRegistryTxtFiles();
    }

    public override void CreateVisualStudioSolution()
    {
        string sourceDirName = Path.Combine(base.StagingArea, "Data");
        string destDirName = Path.Combine(this._stagingAreaShared, "Data");
        Directory.Move(sourceDirName, destDirName);
        base.CreateVisualStudioSolution();
    }

    protected override void CreateWindowsResourceFile()
    {
        base.CreateWindowsResourceFile(this._storeStagingArea);
        base.CreateWindowsResourceFile(this._phoneStagingArea);
    }

    public override void DeleteMdbFiles()
    {
        this._store.DeleteMdbFiles();
        this._phone.DeleteMdbFiles();
    }

    public override void EnumerateAllManagedAssemblies()
    {
        this._store.EnumerateAllManagedAssemblies();
        this._phone.EnumerateAllManagedAssemblies();
    }

    protected override string GetAssemblyConverterPlatform()
    {
        throw new InvalidOperationException();
    }

    protected override string GetPlatformAssemblyPath()
    {
        throw new InvalidOperationException();
    }

    protected override string GetReferenceAssembliesDirectory()
    {
        throw new InvalidOperationException();
    }

    protected override string GetResourceCompilerPath() => 
        Path.Combine(MetroCompilationExtension.GetWindowsKitDirectory(WSASDK.SDK81), @"bin\x86\rc.exe");

    protected override string GetTemplateDirectorySource() => 
        base.GetTemplateDirectorySource("Windows81");

    protected override string GetTemplateDirectoryTarget() => 
        this._stagingAreaShared;

    protected override Version GetToolsVersion() => 
        new Version(12, 0);

    public override void MoveDataManagedToRoot()
    {
        this._store.MoveDataManagedToRoot();
        this._phone.MoveDataManagedToRoot();
    }

    public override void OverwriteUnityAssemblies()
    {
        this._store.OverwriteUnityAssemblies();
        this._phone.OverwriteUnityAssemblies();
    }

    public override void Process()
    {
        Directory.CreateDirectory(this._stagingAreaShared);
        Utility.CopyDirectoryContents(base.StagingAreaDataManaged, this._store.StagingAreaDataManaged, false);
        Utility.MoveDirectory(base.StagingAreaDataManaged, this._phone.StagingAreaDataManaged, null);
        string path = @"Temp\Phone";
        if (Directory.Exists(path))
        {
            Utility.MoveDirectory(path, this._phone.StagingAreaDataManaged, null);
        }
        base.Process();
    }

    public override void RunReferenceRewriter()
    {
        this._store.RunReferenceRewriter();
        this._phone.RunReferenceRewriter();
    }

    public override void RunSerializationWeaver()
    {
        this._store.RunSerializationWeaver();
        this._phone.RunSerializationWeaver();
    }

    protected override Dictionary<WSASDK, LibraryCollection> TEMP_GetLibraryCollections() => 
        new Dictionary<WSASDK, LibraryCollection>(2) { 
            { 
                WSASDK.SDK81,
                this._store.Libraries
            },
            { 
                WSASDK.PhoneSDK81,
                this._phone.Libraries
            }
        };
}

