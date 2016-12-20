namespace UnityEditor.WebGL.Il2Cpp
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using UnityEditor.WebGL.Emscripten;
    using UnityEditorInternal;

    public class WebGLIl2CppNativeCodeBuilder : Il2CppNativeCodeBuilder
    {
        private readonly bool _enableExceptionSupport;
        private readonly IEnumerable<string> _jsLib;
        private readonly IEnumerable<string> _jsPre;
        private readonly IEnumerable<string> _libs;
        private readonly string _linkerFlags;

        public WebGLIl2CppNativeCodeBuilder(bool enableExceptionSupport)
        {
            this._linkerFlags = "-Oz -s NO_EXIT_RUNTIME=1";
            this._enableExceptionSupport = enableExceptionSupport;
        }

        public WebGLIl2CppNativeCodeBuilder(bool enableExceptionSupport, string linkerFlags, IEnumerable<string> libs, IEnumerable<string> jsPre, IEnumerable<string> jsLib)
        {
            this._linkerFlags = "-Oz -s NO_EXIT_RUNTIME=1";
            this._enableExceptionSupport = enableExceptionSupport;
            this._linkerFlags = linkerFlags;
            this._libs = libs;
            this._jsPre = jsPre;
            this._jsLib = jsLib;
        }

        public override IEnumerable<string> ConvertIncludesToFullPaths(IEnumerable<string> relativeIncludePaths)
        {
            return EmscriptenCompiler.GetIncludeFullPaths(relativeIncludePaths);
        }

        public override string ConvertOutputFileToFullPath(string outputFileRelativePath)
        {
            return EmscriptenCompiler.GetOutFileFullPath(outputFileRelativePath);
        }

        protected override void SetupEnvironment(ProcessStartInfo startInfo)
        {
            EmccArguments.SetupDefaultEmscriptenEnvironment(startInfo);
        }

        public override IEnumerable<string> AdditionalIl2CPPArguments
        {
            get
            {
                List<string> list = new List<string>();
                foreach (string str in this._jsPre)
                {
                    list.Add("--js-pre=\"" + str + "\"");
                }
                foreach (string str2 in this._jsLib)
                {
                    list.Add("--js-libraries=\"" + str2 + "\"");
                }
                foreach (string str3 in this._libs)
                {
                    list.Add("--additional-libraries=\"" + str3 + "\"");
                }
                return list;
            }
        }

        public override string CacheDirectory
        {
            get
            {
                return EmscriptenPaths.cacheDirForIl2CppIncrementalBuildArtifacts;
            }
        }

        public override string CompilerArchitecture
        {
            get
            {
                return "EmscriptenJavaScript";
            }
        }

        public override string CompilerFlags
        {
            get
            {
                return EmscriptenCompiler.GetCompilerFlags(this._enableExceptionSupport);
            }
        }

        public override string CompilerPlatform
        {
            get
            {
                return "WebGL";
            }
        }

        public override string LinkerFlags
        {
            get
            {
                return this._linkerFlags;
            }
        }

        public override bool SetsUpEnvironment
        {
            get
            {
                return true;
            }
        }
    }
}

