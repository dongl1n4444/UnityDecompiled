namespace UnityEditor.Facebook
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEditor.BuildReporting;
    using UnityEditor.Modules;
    using UnityEditor.Utils;
    using UnityEngine;

    internal class BuildWindowExtension : DefaultBuildWindowExtension
    {
        internal GUIContent architecture = EditorGUIUtility.TextContent("Architecture|Build architecture for standalone");
        internal GUIContent facebookStandaloneTarget = EditorGUIUtility.TextContent("Target Platform|Destination platform for facebook build");
        internal BuildTarget[] facebookSubTargets;
        internal GUIContent[] facebookSubtargetStrings;
        internal GUIContent packageForSubmissionText = EditorGUIUtility.TextContent("Package build for uploading|Produce a package for submission");
        private static string s_UploadComment = "";
        internal string uploaderProcessErrorMessage = "";
        internal int uploaderProcessExitCode = 0;
        internal bool uploaderProcessFinished = false;
        internal string uploaderProcessMessage = "";

        public BuildWindowExtension()
        {
            this.SetupFacebookSubtargets();
        }

        public override bool EnabledBuildAndRunButton()
        {
            if ((Application.platform != RuntimePlatform.WindowsEditor) && (EditorUserBuildSettings.selectedFacebookTarget != BuildTarget.WebGL))
            {
                return false;
            }
            return this.IsCurrentSubtargetSupported();
        }

        public override bool EnabledBuildButton() => 
            this.IsCurrentSubtargetSupported();

        private bool IsCurrentSubtargetSupported()
        {
            BuildTarget selectedFacebookTarget = EditorUserBuildSettings.selectedFacebookTarget;
            return (ModuleManager.GetBuildPostProcessor(ModuleManager.GetTargetStringFrom(BuildPipeline.GetBuildTargetGroup(selectedFacebookTarget), selectedFacebookTarget)) != null);
        }

        private void SetupFacebookSubtargets()
        {
            List<BuildTarget> list = new List<BuildTarget>();
            List<GUIContent> list2 = new List<GUIContent>();
            list.Add(BuildTarget.StandaloneWindows);
            list2.Add(EditorGUIUtility.TextContent("Gameroom (Windows)"));
            list.Add(BuildTarget.WebGL);
            list2.Add(EditorGUIUtility.TextContent("WebGL"));
            this.facebookSubTargets = list.ToArray();
            this.facebookSubtargetStrings = list2.ToArray();
        }

        public override bool ShouldDrawDevelopmentPlayerCheckbox() => 
            this.IsCurrentSubtargetSupported();

        public override bool ShouldDrawProfilerCheckbox() => 
            this.IsCurrentSubtargetSupported();

        public override bool ShouldDrawScriptDebuggingCheckbox() => 
            (this.IsCurrentSubtargetSupported() && (EditorUserBuildSettings.selectedFacebookTarget == BuildTarget.StandaloneWindows));

        public override void ShowPlatformBuildOptions()
        {
            if (SDKManager.IsSDKOverridenIncompatibleVersion())
            {
                EditorGUILayout.HelpBox("This project contains a custom Facebook SDK for unity, which is incompatible with the Facebook build target.", MessageType.Error);
            }
            else
            {
                string str = SDKManager.ShouldShowUpdateMessage();
                if (str != null)
                {
                    EditorGUILayout.HelpBox("There is a newer version of the Facebook SDK for unity available (" + str + "). You can click on Player Settings to select it.", MessageType.Info);
                }
            }
            string appID = FacebookSettingsExtension.AppID;
            if (string.IsNullOrEmpty(appID) || (appID == "0"))
            {
                EditorGUILayout.HelpBox("Please set an App ID in Facebook Player Settings for testing your build on Facebook.", MessageType.Warning);
            }
            BuildTarget selectedFacebookTarget = EditorUserBuildSettings.selectedFacebookTarget;
            BuildTarget target2 = EditorUserBuildSettings.selectedFacebookTarget;
            int selectedIndex = Math.Max(0, Array.IndexOf<BuildTarget>(this.facebookSubTargets, selectedFacebookTarget));
            int index = EditorGUILayout.Popup(this.facebookStandaloneTarget, selectedIndex, this.facebookSubtargetStrings, new GUILayoutOption[0]);
            if (index == selectedIndex)
            {
                if (!this.IsCurrentSubtargetSupported())
                {
                    GUILayout.Space(20f);
                    GUILayout.Label("No " + BuildPlayerWindow.AvailableBuildPlatforms.GetBuildTargetDisplayName(selectedFacebookTarget) + " module loaded.", new GUILayoutOption[0]);
                    GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandWidth(false) };
                    if (GUILayout.Button("Open Download Page", EditorStyles.miniButton, options))
                    {
                        Help.BrowseURL(BuildPlayerWindow.GetPlaybackEngineDownloadURL(ModuleManager.GetTargetStringFrom(BuildPipeline.GetBuildTargetGroup(selectedFacebookTarget), selectedFacebookTarget)));
                    }
                }
                else
                {
                    EditorUserBuildSettings.facebookCreatePackageForSubmission = EditorGUILayout.Toggle(this.packageForSubmissionText, EditorUserBuildSettings.facebookCreatePackageForSubmission, new GUILayoutOption[0]);
                    this.ShowUploadUI(appID);
                }
            }
            else
            {
                target2 = this.facebookSubTargets[index];
            }
            if (target2 != EditorUserBuildSettings.selectedFacebookTarget)
            {
                EditorUserBuildSettings.selectedFacebookTarget = target2;
                GUIUtility.ExitGUI();
            }
        }

        private void ShowUploadUI(string appID)
        {
            BuildReport latestReport = BuildReport.GetLatestReport();
            Artifacts artifacts = null;
            if (latestReport != null)
            {
                UnityEngine.Object[] appendices = latestReport.GetAppendices(typeof(Artifacts));
                if (appendices.Length > 0)
                {
                    artifacts = appendices[0] as Artifacts;
                }
            }
            bool flag = ((artifacts != null) && !this.uploaderProcessFinished) && artifacts.isUploading;
            EditorGUI.BeginDisabledGroup(((artifacts == null) || flag) || (latestReport.buildTarget != EditorUserBuildSettings.selectedFacebookTarget));
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandWidth(false) };
            GUILayout.BeginHorizontal(options);
            GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.Width(160f) };
            if (GUILayout.Button("Upload last build to Facebook", EditorStyles.miniButton, optionArray2))
            {
                if (string.IsNullOrEmpty(appID) || (appID == "0"))
                {
                    string url = "https://developers.facebook.com/";
                    if (EditorUtility.DisplayDialog("No App ID is set", "Please set an App ID in Facebook Player Settings for uploading to Facebook.", "Retrieve App ID from Facebook", "Ok"))
                    {
                        Application.OpenURL(url);
                    }
                }
                else
                {
                    string facebookAccessToken = EditorUserBuildSettings.facebookAccessToken;
                    if (string.IsNullOrEmpty(facebookAccessToken))
                    {
                        string str3 = "https://developers.facebook.com/apps/" + appID + "/hosting/";
                        if (EditorUtility.DisplayDialog("No access token is set", "Please set an access token in Facebook Player Settings for uploading to Facebook.", "Retrieve access token from Facebook", "Ok"))
                        {
                            Application.OpenURL(str3);
                        }
                    }
                    else
                    {
                        <ShowUploadUI>c__AnonStorey0 storey = new <ShowUploadUI>c__AnonStorey0 {
                            $this = this
                        };
                        this.uploaderProcessFinished = false;
                        this.uploaderProcessExitCode = 0;
                        this.uploaderProcessMessage = "";
                        this.uploaderProcessErrorMessage = "";
                        artifacts.StartUpload();
                        string str4 = "http://localhost:38000/unity/build-report/latest/appendices/Artifacts/0/";
                        string str5 = "https://graph-video.facebook.com/" + appID + "/assets";
                        string str6 = (EditorUserBuildSettings.selectedFacebookTarget != BuildTarget.WebGL) ? "GAMES_DESKTOP" : "UNITY_WEBGL";
                        string arguments = $"{str4} {str5} {facebookAccessToken} {str6} "{!string.IsNullOrEmpty(s_UploadComment) ? s_UploadComment : "Uploaded by Unity Editor"}"";
                        string str8 = BuildPipeline.GetPlaybackEngineExtensionDirectory(BuildTargetGroup.Facebook, BuildTarget.StandaloneWindows, BuildOptions.CompressTextures);
                        string[] components = new string[] { str8, "Uploader.exe" };
                        string executable = Paths.Combine(components);
                        storey.p = Utilities.CreateManagedProgram(executable, arguments, null);
                        storey.p.Start(new EventHandler(storey.<>m__0));
                        storey.p.LogProcessStartInfo();
                    }
                }
            }
            GUI.SetNextControlName("UploadCommentField");
            Rect position = GUILayoutUtility.GetRect(new GUIContent(), EditorStyles.textField);
            s_UploadComment = GUI.TextField(position, s_UploadComment);
            if (string.IsNullOrEmpty(s_UploadComment) && (GUI.GetNameOfFocusedControl() != "UploadCommentField"))
            {
                using (new EditorGUI.DisabledScope(true))
                {
                    if (Event.current.type == EventType.Repaint)
                    {
                        EditorStyles.textField.Draw(position, "Enter Comment for upload", false, false, false, false);
                    }
                }
            }
            GUILayout.EndHorizontal();
            EditorGUI.EndDisabledGroup();
            if (artifacts != null)
            {
                if (flag)
                {
                    GUILayoutOption[] optionArray3 = new GUILayoutOption[] { GUILayout.Height(20f) };
                    EditorGUI.ProgressBar(EditorGUILayout.GetControlRect(optionArray3), artifacts.progress.progress, "Uploading build to Facebook");
                }
                else if (!string.IsNullOrEmpty(artifacts.progress.status))
                {
                    if (artifacts.progress.success)
                    {
                        EditorGUILayout.HelpBox("Uploaded last build successfully.", MessageType.Info);
                    }
                    else
                    {
                        EditorGUILayout.HelpBox("Failed uploading last build." + artifacts.progress.status, MessageType.Error);
                    }
                }
                else if (this.uploaderProcessFinished)
                {
                    EditorGUILayout.HelpBox(string.Concat(new object[] { "Failed uploading last build. ExitCode ", this.uploaderProcessExitCode, ". Message: ", this.uploaderProcessErrorMessage }), MessageType.Error);
                }
            }
        }

        [CompilerGenerated]
        private sealed class <ShowUploadUI>c__AnonStorey0
        {
            internal BuildWindowExtension $this;
            internal Program p;

            internal void <>m__0(object sender, EventArgs e)
            {
                this.$this.uploaderProcessFinished = true;
                this.$this.uploaderProcessExitCode = this.p.ExitCode;
                this.$this.uploaderProcessMessage = this.p.GetAllOutput();
                this.$this.uploaderProcessErrorMessage = this.p.GetErrorOutputAsString();
                Console.WriteLine("Exit code:    {0}. Output:    {1}", this.p.ExitCode, this.p.GetAllOutput());
                if (this.p.ExitCode != 0)
                {
                    Debug.LogError("Uploading build to Facebook failed: " + this.p.GetAllOutput());
                }
            }
        }
    }
}

