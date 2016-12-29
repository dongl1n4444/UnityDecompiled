namespace UnityEditor.WebGL.Il2Cpp
{
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEditor.BuildReporting;
    using UnityEditorInternal;

    internal class WebGlIl2CppPlatformProvider : BaseIl2CppPlatformProvider
    {
        public IEnumerable<string> JsLib;
        public IEnumerable<string> JsPre;
        public IEnumerable<string> Libs;
        public string LinkerFlags;
        private readonly BuildReport m_BuildReport;
        private readonly bool m_IsDevelopmentBuild;
        private readonly string m_NativeLibraryFileName;

        public WebGlIl2CppPlatformProvider(BuildTarget target, bool isDevelopmentBuild, string dataDirectory, string nativeLibraryFileName, BuildReport _buildReport) : base(target, Path.Combine(dataDirectory, "Libraries"))
        {
            this.LinkerFlags = "";
            this.Libs = new string[0];
            this.JsPre = new string[0];
            this.JsLib = new string[0];
            this.m_IsDevelopmentBuild = isDevelopmentBuild;
            this.m_NativeLibraryFileName = nativeLibraryFileName;
            this.m_BuildReport = _buildReport;
        }

        public override Il2CppNativeCodeBuilder CreateIl2CppNativeCodeBuilder() => 
            new WebGLIl2CppNativeCodeBuilder(PlayerSettings.WebGL.exceptionSupport == WebGLExceptionSupport.Full, this.LinkerFlags, this.Libs, this.JsPre, this.JsLib);

        public override BuildReport buildReport =>
            this.m_BuildReport;

        public override bool developmentMode =>
            this.m_IsDevelopmentBuild;

        public override bool emitNullChecks =>
            (PlayerSettings.WebGL.exceptionSupport == WebGLExceptionSupport.Full);

        public override bool enableArrayBoundsCheck =>
            (PlayerSettings.WebGL.exceptionSupport == WebGLExceptionSupport.Full);

        public override bool enableStackTraces =>
            (PlayerSettings.WebGL.exceptionSupport == WebGLExceptionSupport.Full);

        public override string[] includePaths =>
            new string[0];

        public override string[] libraryPaths =>
            new string[0];

        public override bool loadSymbols =>
            this.m_IsDevelopmentBuild;

        public override string nativeLibraryFileName =>
            this.m_NativeLibraryFileName;

        public override bool supportsEngineStripping =>
            true;
    }
}

