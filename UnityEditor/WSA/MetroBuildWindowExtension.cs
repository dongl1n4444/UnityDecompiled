namespace UnityEditor.WSA
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using UnityEditor;
    using UnityEditor.Modules;
    using UnityEditorInternal;
    using UnityEngine;

    internal class MetroBuildWindowExtension : DefaultBuildWindowExtension
    {
        private GUIContent copyReferences = EditorGUIUtility.TextContent("Copy References|Copy Unity references next to the generated solution? If you don't copy them, the generated solution will reference them from Unity installation path, thus saving disk space.");
        private GUIContent generateCSharpProjectsText = EditorGUIUtility.TextContent("Unity C# Projects|Generates Assembly-CSharp Visual Studio projects for debugging purposes, available only for .NET scripting backend");
        private TargetExtension targetExtension;
        private GUIContent uapBuildType = EditorGUIUtility.TextContent("UWP Build Type");
        private WSAUWPBuildType[] uwpBuildTypes;
        private GUIContent[] uwpBuildTypeStrings;
        private GUIContent wsaBuildAndRunDeployTarget = EditorGUIUtility.TextContent("Build and Run on|On which device the application should run when you click Build And Run button. Note: this option is ignored when you click Build button.");
        public WSABuildAndRunDeployTarget[] wsaBuildAndRunDeployTargets;
        public GUIContent[] wsaBuildAndRunDeployTargetStrings;
        private GUIContent wsaSDK = EditorGUIUtility.TextContent("SDK|Which Windows SDK does your application target?");
        private WSASDK[] wsaSDKs;
        private GUIContent[] wsaSDKStrings;
        private GUIContent wsaSubtarget = EditorGUIUtility.TextContent("Target device|Specific device type for which resources get optimized");
        private WSASubtarget[] wsaSubtargets = new WSASubtarget[] { WSASubtarget.AnyDevice };
        private GUIContent[] wsaSubtargetStrings = new GUIContent[] { EditorGUIUtility.TextContent("Any device"), EditorGUIUtility.TextContent("PC"), EditorGUIUtility.TextContent("Mobile"), EditorGUIUtility.TextContent("HoloLens") };
        public WSABuildAndRunDeployTarget[] wsaUWPBuildAndRunDeployTargets;

        public MetroBuildWindowExtension(TargetExtension targetExtension)
        {
            WSAUWPBuildType[] typeArray1 = new WSAUWPBuildType[2];
            typeArray1[1] = WSAUWPBuildType.D3D;
            this.uwpBuildTypes = typeArray1;
            this.uwpBuildTypeStrings = new GUIContent[] { EditorGUIUtility.TextContent("XAML"), EditorGUIUtility.TextContent("D3D") };
            WSABuildAndRunDeployTarget[] targetArray1 = new WSABuildAndRunDeployTarget[3];
            targetArray1[1] = WSABuildAndRunDeployTarget.WindowsPhone;
            targetArray1[2] = WSABuildAndRunDeployTarget.LocalMachineAndWindowsPhone;
            this.wsaBuildAndRunDeployTargets = targetArray1;
            WSABuildAndRunDeployTarget[] targetArray2 = new WSABuildAndRunDeployTarget[2];
            targetArray2[1] = WSABuildAndRunDeployTarget.WindowsPhone;
            this.wsaUWPBuildAndRunDeployTargets = targetArray2;
            this.wsaBuildAndRunDeployTargetStrings = new GUIContent[] { EditorGUIUtility.TextContent("Local Machine"), EditorGUIUtility.TextContent("Windows Phone"), EditorGUIUtility.TextContent("Local Machine and Windows Phone") };
            Dictionary<WSASDK, GUIContent> dictionary = new Dictionary<WSASDK, GUIContent> {
                { 
                    WSASDK.SDK81,
                    EditorGUIUtility.TextContent("8.1")
                },
                { 
                    WSASDK.PhoneSDK81,
                    EditorGUIUtility.TextContent("Phone 8.1")
                },
                { 
                    WSASDK.UniversalSDK81,
                    EditorGUIUtility.TextContent("Universal 8.1")
                },
                { 
                    WSASDK.UWP,
                    EditorGUIUtility.TextContent("Universal 10")
                }
            };
            this.wsaSDKs = new WSASDK[] { WSASDK.SDK81 };
            this.wsaSDKStrings = new GUIContent[this.wsaSDKs.Length];
            for (int i = 0; i < this.wsaSDKs.Length; i++)
            {
                this.wsaSDKStrings[i] = dictionary[this.wsaSDKs[i]];
            }
            this.targetExtension = targetExtension;
        }

        private void CopyPdbFilesToAppX()
        {
            string playbackEngineDirectory = BuildPipeline.GetPlaybackEngineDirectory(BuildTarget.WSAPlayer, BuildOptions.CompressTextures);
            string format = "";
            string str3 = "";
            if (EditorUserBuildSettings.wsaSDK != WSASDK.UWP)
            {
                throw new NotImplementedException("Copying pdb files not implemented for " + EditorUserBuildSettings.wsaSDK);
            }
            string str4 = "dotnet";
            if (PlayerSettings.GetScriptingBackend(BuildTargetGroup.WSA) == ScriptingImplementation.IL2CPP)
            {
                str4 = "il2cpp";
            }
            format = Path.Combine(playbackEngineDirectory, string.Format(@"Players\UAP\{0}\{{0}}\debug", str4));
            str3 = Path.Combine(playbackEngineDirectory, string.Format(@"SourceBuild\{0}\{0}\bin\{{0}}\Debug\AppX", Application.productName));
            string[] strArray = new string[] { "x86", "x64" };
            foreach (string str5 in strArray)
            {
                string path = string.Format(format, str5);
                string str7 = string.Format(str3, str5);
                if (Directory.Exists(path) && Directory.Exists(str7))
                {
                    string[] files = Directory.GetFiles(path, "UnityPlayer*.pdb");
                    StringBuilder builder = new StringBuilder();
                    foreach (string str8 in files)
                    {
                        string destFileName = Path.Combine(str7, Path.GetFileName(str8));
                        File.Copy(str8, destFileName, true);
                        builder.AppendFormat("Copied {0} to {1}\n", str8, destFileName);
                    }
                    Debug.Log("Following pdb files were copied:\n" + builder);
                }
            }
        }

        public override bool EnabledBuildAndRunButton()
        {
            return (this.EnabledBuildButton() && !EditorUserBuildSettings.wsaGenerateReferenceProjects);
        }

        public override bool EnabledBuildButton()
        {
            if (!InternalEditorUtility.RunningUnderWindows8())
            {
                return false;
            }
            if (this.CurrentScriptingBackend == ScriptingImplementation.WinRTDotNET)
            {
                return this.targetExtension.hasDotNetPlayers;
            }
            if (EditorUserBuildSettings.wsaSDK != WSASDK.UWP)
            {
                return false;
            }
            return this.targetExtension.hasIl2CppPlayers;
        }

        public override bool ShouldDrawScriptDebuggingCheckbox()
        {
            return false;
        }

        public override void ShowPlatformBuildOptions()
        {
            if (!InternalEditorUtility.RunningUnderWindows8())
            {
                GUILayout.Label(EditorGUIUtility.TextContent("You have to use Windows 8 or greater for this build."), EditorStyles.wordWrappedLabel, new GUILayoutOption[0]);
            }
            int selectedIndex = Math.Max(0, Array.IndexOf<WSASDK>(this.wsaSDKs, EditorUserBuildSettings.wsaSDK));
            selectedIndex = EditorGUILayout.Popup(this.wsaSDK, selectedIndex, this.wsaSDKStrings, new GUILayoutOption[0]);
            EditorUserBuildSettings.wsaSDK = this.wsaSDKs[selectedIndex];
            selectedIndex = Math.Max(0, Array.IndexOf<WSASubtarget>(this.wsaSubtargets, EditorUserBuildSettings.wsaSubtarget));
            selectedIndex = EditorGUILayout.Popup(this.wsaSubtarget, selectedIndex, this.wsaSubtargetStrings, new GUILayoutOption[0]);
            EditorUserBuildSettings.wsaSubtarget = this.wsaSubtargets[selectedIndex];
            GUI.enabled = EditorUserBuildSettings.wsaSDK == WSASDK.UWP;
            selectedIndex = Math.Max(0, Array.IndexOf<WSAUWPBuildType>(this.uwpBuildTypes, EditorUserBuildSettings.wsaUWPBuildType));
            selectedIndex = EditorGUILayout.Popup(this.uapBuildType, selectedIndex, this.uwpBuildTypeStrings, new GUILayoutOption[0]);
            EditorUserBuildSettings.wsaUWPBuildType = this.uwpBuildTypes[selectedIndex];
            GUI.enabled = (EditorUserBuildSettings.wsaSDK == WSASDK.UniversalSDK81) || (EditorUserBuildSettings.wsaSDK == WSASDK.UWP);
            selectedIndex = Math.Max(0, Array.IndexOf<WSABuildAndRunDeployTarget>(this.wsaBuildAndRunDeployTargets, EditorUserBuildSettings.wsaBuildAndRunDeployTarget));
            selectedIndex = EditorGUILayout.Popup(this.wsaBuildAndRunDeployTarget, selectedIndex, this.wsaBuildAndRunDeployTargetStrings, new GUILayoutOption[0]);
            EditorUserBuildSettings.wsaBuildAndRunDeployTarget = this.wsaBuildAndRunDeployTargets[selectedIndex];
            GUI.enabled = true;
            ScriptingImplementation currentScriptingBackend = this.CurrentScriptingBackend;
            if (currentScriptingBackend == ScriptingImplementation.WinRTDotNET)
            {
                if (!this.targetExtension.hasDotNetPlayers)
                {
                    EditorGUILayout.HelpBox("Currently selected scripting backend (.NET) is not installed.", MessageType.Error);
                }
            }
            else if (EditorUserBuildSettings.wsaSDK != WSASDK.UWP)
            {
                EditorGUILayout.HelpBox("Currently selected scripting backend (IL2CPP) does not support selected SDK.", MessageType.Error);
            }
            else if (!this.targetExtension.hasIl2CppPlayers)
            {
                EditorGUILayout.HelpBox("Currently selected scripting backend (IL2CPP) is not installed.", MessageType.Error);
            }
            UserBuildSettings.copyReferences = EditorGUILayout.Toggle(this.copyReferences, UserBuildSettings.copyReferences, new GUILayoutOption[0]);
            EditorGUILayout.LabelField(EditorGUIUtility.TextContent("Debugging"), EditorStyles.boldLabel, new GUILayoutOption[0]);
            EditorGUI.BeginDisabledGroup(currentScriptingBackend != ScriptingImplementation.WinRTDotNET);
            EditorUserBuildSettings.wsaGenerateReferenceProjects = EditorGUILayout.Toggle(this.generateCSharpProjectsText, EditorUserBuildSettings.wsaGenerateReferenceProjects, new GUILayoutOption[0]);
            EditorGUI.EndDisabledGroup();
            if (Unsupported.IsDeveloperBuild() && GUILayout.Button("Copy Pdb files to SourceBuild", new GUILayoutOption[0]))
            {
                this.CopyPdbFilesToAppX();
            }
        }

        private ScriptingImplementation CurrentScriptingBackend
        {
            get
            {
                return PlayerSettings.GetScriptingBackend(BuildTargetGroup.WSA);
            }
        }
    }
}

