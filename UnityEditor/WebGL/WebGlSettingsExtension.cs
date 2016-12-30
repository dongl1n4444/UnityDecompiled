namespace UnityEditor.WebGL
{
    using System;
    using UnityEditor;
    using UnityEditor.Modules;
    using UnityEngine;

    internal class WebGlSettingsExtension : DefaultPlayerSettingsEditorExtension
    {
        private SerializedProperty m_DefaultScreenHeightWeb;
        private SerializedProperty m_DefaultScreenWidthWeb;
        private SerializedProperty m_RunInBackground;
        private SerializedProperty m_UseWasm;
        private SerializedProperty m_WebGLCompressionFormat;
        private SerializedProperty m_WebGLDataCaching;
        private SerializedProperty m_WebGLDebugSymbols;
        private SerializedProperty m_WebGLExceptionSupport;
        private SerializedProperty m_WebGLMemorySize;
        private SerializedProperty m_WebGLNameFilesAsHashes;
        private SerializedProperty m_WebGLTemplate;
        private WebGLTemplateManager m_WebGLTemplateManager = new WebGLTemplateManager();

        public override bool CanShowUnitySplashScreen() => 
            true;

        public override bool HasPublishSection() => 
            true;

        public override bool HasResolutionSection() => 
            true;

        public override void OnEnable(PlayerSettingsEditor settingsEditor)
        {
            this.m_WebGLMemorySize = settingsEditor.FindPropertyAssert("webGLMemorySize");
            this.m_WebGLExceptionSupport = settingsEditor.FindPropertyAssert("webGLExceptionSupport");
            this.m_WebGLCompressionFormat = settingsEditor.FindPropertyAssert("webGLCompressionFormat");
            this.m_WebGLNameFilesAsHashes = settingsEditor.FindPropertyAssert("webGLNameFilesAsHashes");
            this.m_WebGLDataCaching = settingsEditor.FindPropertyAssert("webGLDataCaching");
            this.m_WebGLDebugSymbols = settingsEditor.FindPropertyAssert("webGLDebugSymbols");
            this.m_WebGLTemplate = settingsEditor.FindPropertyAssert("webGLTemplate");
            this.m_DefaultScreenWidthWeb = settingsEditor.FindPropertyAssert("defaultScreenWidthWeb");
            this.m_DefaultScreenHeightWeb = settingsEditor.FindPropertyAssert("defaultScreenHeightWeb");
            this.m_RunInBackground = settingsEditor.FindPropertyAssert("runInBackground");
            this.m_UseWasm = settingsEditor.FindPropertyAssert("webGLUseWasm");
        }

        public override void PublishSectionGUI(float h, float kLabelFloatMinW, float kLabelFloatMaxW)
        {
            EditorGUILayout.PropertyField(this.m_WebGLMemorySize, EditorGUIUtility.TextContent("WebGL Memory Size|Specifies the total size of memory available to the WebGL runtime in MB. Large sizes may be needed to get more complex content to run, but may fail to load on some browsers."), new GUILayoutOption[0]);
            this.m_WebGLMemorySize.intValue = Mathf.Clamp(this.m_WebGLMemorySize.intValue, 0x10, 0x7f0);
            if (this.m_WebGLMemorySize.intValue > 0x100)
            {
                EditorGUILayout.HelpBox("Note that using more memory then the default setting has a higher chance of your content failing to start up if the browser cannot allocate enough memory - especially on 32-bit browsers.", MessageType.Warning);
            }
            EditorGUILayout.PropertyField(this.m_WebGLExceptionSupport, EditorGUIUtility.TextContent("Enable Exceptions|Turn this on to enable exception catching in WebGL. Don't use this if you don't need to as it has a significant performance overhead."), new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_WebGLCompressionFormat, EditorGUIUtility.TextContent("Compression Format"), new GUILayoutOption[0]);
            if (PlayerSettings.WebGL.exceptionSupport == WebGLExceptionSupport.Full)
            {
                EditorGUILayout.HelpBox("Full exception support adds a lot of code to do sanity checks, which costs a lot of performance and browser memory. Only use this for debugging, and make sure to test in a 64-bit browser.", MessageType.Warning);
            }
            EditorGUILayout.PropertyField(this.m_WebGLNameFilesAsHashes, EditorGUIUtility.TextContent("Name Files As Hashes|Use MD5 hash of the uncompressed file contents as a filename for each file in the build."), new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_WebGLDataCaching, EditorGUIUtility.TextContent("Data caching|Automatically cache downloaded assets locally on users machine to skip long downloads in subsequent runs."), new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_WebGLDebugSymbols, EditorGUIUtility.TextContent("Debug Symbols|Preserve debug symbols and perform demangling of the stack trace when an error occurs. For release builds all the debug information is stored in a separate file which is downloaded from the server on demand when an error occurs. Development builds always have demangling support embedded in the main module and therefore are not affected by this option."), new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_UseWasm, EditorGUIUtility.TextContent("WebAssembly (Experimental)|Create WebAssembly build files for testing in browsers with experimental WebAssembly support."), new GUILayoutOption[0]);
            if (PlayerSettings.WebGL.useWasm)
            {
                EditorGUILayout.HelpBox("WebAssembly is an experimental feature, and not enabled in any currently shipping browsers by default. This functionality is provided for testing purposes only.", MessageType.Warning);
            }
        }

        public override void ResolutionSectionGUI(float h, float midWidth, float maxWidth)
        {
            GUILayout.Label(EditorGUIUtility.TextContent("Resolution"), EditorStyles.boldLabel, new GUILayoutOption[0]);
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(this.m_DefaultScreenWidthWeb, EditorGUIUtility.TextContent("Default Screen Width*"), new GUILayoutOption[0]);
            if (EditorGUI.EndChangeCheck() && (this.m_DefaultScreenWidthWeb.intValue < 1))
            {
                this.m_DefaultScreenWidthWeb.intValue = 1;
            }
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(this.m_DefaultScreenHeightWeb, EditorGUIUtility.TextContent("Default Screen Height*"), new GUILayoutOption[0]);
            if (EditorGUI.EndChangeCheck() && (this.m_DefaultScreenHeightWeb.intValue < 1))
            {
                this.m_DefaultScreenHeightWeb.intValue = 1;
            }
            EditorGUILayout.PropertyField(this.m_RunInBackground, EditorGUIUtility.TextContent("Run In Background*"), new GUILayoutOption[0]);
            EditorGUILayout.Space();
            GUILayout.Label(EditorGUIUtility.TextContent("WebGL Template"), EditorStyles.boldLabel, new GUILayoutOption[0]);
            this.m_WebGLTemplateManager.SelectionUI(this.m_WebGLTemplate);
        }
    }
}

