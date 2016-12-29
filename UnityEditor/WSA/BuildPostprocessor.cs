namespace UnityEditor.WSA
{
    using System;
    using UnityEditor;
    using UnityEditor.Modules;

    internal sealed class BuildPostprocessor : DefaultBuildPostprocessor
    {
        public override void LaunchPlayer(BuildLaunchPlayerArgs args)
        {
            ApplicationLauncher.BuildAndRun(args);
        }

        public override void PostProcess(BuildPostProcessArgs args)
        {
            PostProcessWSA swsa;
            WSASDK wsaSDK = EditorUserBuildSettings.wsaSDK;
            switch (wsaSDK)
            {
                case WSASDK.SDK81:
                    swsa = new PostProcessStore81(args, null);
                    break;

                case WSASDK.PhoneSDK81:
                    swsa = new PostProcessPhone81(args, null);
                    break;

                case WSASDK.UniversalSDK81:
                    swsa = new PostProcessUniversal81(args);
                    break;

                case WSASDK.UWP:
                    if (PlayerSettings.GetScriptingBackend(BuildTargetGroup.WSA) != ScriptingImplementation.IL2CPP)
                    {
                        swsa = new PostProcessUAPDotNet(args, null);
                        break;
                    }
                    swsa = new PostProcessUAPIl2Cpp(args, null);
                    break;

                default:
                    throw new NotSupportedException($"{wsaSDK} is not supported.");
            }
            swsa.Process();
        }

        public override string PrepareForBuild(BuildOptions options, BuildTarget target)
        {
            if (EditorUserBuildSettings.wsaSDK != WSASDK.UWP)
            {
                return null;
            }
            if (Utility.UseIl2CppScriptingBackend())
            {
                return null;
            }
            return NuGet.Restore(target);
        }

        public override bool SupportsInstallInBuildFolder() => 
            true;
    }
}

