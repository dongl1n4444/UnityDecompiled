namespace UnityEditor.Android
{
    using System;
    using UnityEditor;
    using UnityEngine;

    internal class AndroidNDKTools
    {
        public readonly string NDKRootDir;
        private static AndroidNDKTools s_Instance;

        private AndroidNDKTools(string ndkRoot)
        {
            this.NDKRootDir = ndkRoot;
        }

        public static AndroidNDKTools GetInstance()
        {
            string ndkPath = EditorPrefs.GetString("AndroidNdkRoot");
            if (!AndroidNdkRoot.VerifyNdkDir(ndkPath))
            {
                ndkPath = AndroidNdkRoot.Browse(ndkPath);
                if (!AndroidNdkRoot.VerifyNdkDir(ndkPath))
                {
                    return null;
                }
                EditorPrefs.SetString("AndroidNdkRoot", ndkPath);
            }
            if ((s_Instance == null) || (ndkPath != s_Instance.NDKRootDir))
            {
                s_Instance = new AndroidNDKTools(ndkPath);
            }
            return s_Instance;
        }

        public static AndroidNDKTools GetInstanceOrThrowException()
        {
            AndroidNDKTools instance = GetInstance();
            if (instance == null)
            {
                throw new UnityException("Unable to locate Android NDK!");
            }
            return instance;
        }
    }
}

